/* Copyright (c) 1996-2013 Clickteam
 *
 * This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
 * 
 * Permission is hereby granted to any person obtaining a legal copy 
 * of Clickteam Multimedia Fusion 2 to use or modify this source code for 
 * debugging, optimizing, or customizing applications created with 
 * Clickteam Multimedia Fusion 2.  Any other use of this source code is prohibited.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
//----------------------------------------------------------------------------------
//
// CRUNLOOP : BOucle principale
//
//----------------------------------------------------------------------------------
package RunLoop;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Random;

import javax.microedition.khronos.egl.EGL10;
import javax.microedition.khronos.egl.EGL11;

import Actions.CLoop;
import Animations.CAnim;
import Animations.CRAni;
import Application.CJoystick;
import Application.CJoystickEmulated;
import Application.CJoystickFIRETV;
import Application.CJoystickNEXUSTV;
import Application.CJoystickOUYA;
import Application.CRunApp;
import Application.CRunFrame;
import Banks.CImage;
import Events.CEvent;
import Events.CEventProgram;
import Events.CForEach;
import Events.CPosOnLoop;
import Events.CQualToOiList;
import Expressions.CExp;
import Expressions.CValue;
import Expressions.EXP_END;
import Expressions.EXP_STRING;
import Extensions.CExtStorage;
import Extensions.CRunExtension;
import Frame.CLO;
import Frame.CLayer;
import Movements.CMove;
import Movements.CMoveBullet;
import Movements.CMoveDef;
import Movements.CMoveDefExtension;
import Movements.CMoveExtension;
import Movements.CMovePlatform;
import Movements.CRMvt;
import OI.COC;
import OI.COCBackground;
import OI.COI;
import OI.CObjectCommon;
import Objects.CActive;
import Objects.CCCA;
import Objects.CCounter;
import Objects.CExtension;
import Objects.CLives;
import Objects.CObject;
import Objects.CQuestion;
import Objects.CRCom;
import Objects.CScore;
import Objects.CText;
import OpenGL.GLRenderer;
import Params.CParamExpression;
import Params.CPositionInfo;
import Params.PARAM_CREATE;
import Runtime.Log;
import Runtime.MMFRuntime;
import Runtime.SurfaceView;
import Services.CArrayList;
import Services.CFontInfo;
import Services.CRect;
import Services.CServices;
import Sprites.CColMask;
import Sprites.CMask;
import Sprites.CRSpr;
import Sprites.CSprite;
import Sprites.CSpriteGen;
import Values.CRVal;
import android.os.SystemClock;

public class CRun
{
	// Flags
	public static final short GAMEFLAGS_VBLINDEP = 0x0002;
	public static final short GAMEFLAGS_LIMITEDSCROLL = 0x0004;
	public static final short GAMEFLAGS_FIRSTLOOPFADEIN = 0x0010;
	public static final short GAMEFLAGS_LOADONCALL = 0x0020;
	public static final short GAMEFLAGS_REALGAME = 0x0040;
	public static final short GAMEFLAGS_PLAY = 0x0080;
	public static final short GAMEFLAGS_INITIALISING = 0x0200;

	// Flags pour DrawLevel
	public static final short DLF_DONTUPDATE = 0x0002;
	public static final short DLF_DRAWOBJECTS = 0x0004;
	public static final short DLF_RESTARTLEVEL = 0x0008;
	public static final short DLF_DONTUPDATECOLMASK = 0x0010;
	public static final short DLF_COLMASKCLIPPED = 0x0020;
	public static final short DLF_SKIPLAYER0 = 0x0040;
	public static final short DLF_REDRAWLAYER = 0x0080;
	public static final short DLF_STARTLEVEL = 0x0100;
	public static final short GAME_XBORDER = 480;
	public static final short GAME_YBORDER = 300;
	public static final short COLMASK_XMARGIN = 64;
	public static final short COLMASK_YMARGIN = 16;
	public static final int WRAP_X = 1;
	public static final int WRAP_Y = 2;
	public static final int WRAP_XY = 4;
	public static final byte plMasks[][] =
		{
		{
			(byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00
		},
		{
			(byte) 0xFF, (byte) 0x00, (byte) 0x00, (byte) 0x00
		},
		{
			(byte) 0xFF, (byte) 0xFF, (byte) 0x00, (byte) 0x00
		},
		{
			(byte) 0xFF, (byte) 0xFF, (byte) 0xFF, (byte) 0x00
		},
		{
			(byte) 0xFF, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF
		},
		};

	// Collision detection acceleration
	public static final int OBJZONE_WIDTH = 512;
	public static final int OBJZONE_HEIGHT = 512;

	// Flags pour rh3Scrolling
	public static final int RH3SCROLLING_SCROLL = 0x0001;
	public static final int RH3SCROLLING_REDRAWLAYERS = 0x0002;
	public static final int RH3SCROLLING_REDRAWALL = 0x0004;
	public static final int RH3SCROLLING_REDRAWTOTALCOLMASK = 0x0008;

	// Types d'obstacles
	public static final int OBSTACLE_NONE = 0;
	public static final int OBSTACLE_SOLID = 1;
	public static final int OBSTACLE_PLATFORM = 2;
	public static final int OBSTACLE_LADDER = 3;
	public static final int OBSTACLE_TRANSPARENT = 4;		// for Add Backdrop

	//Flags pour createobject
	public static final short COF_NOMOVEMENT = 0x0001;
	public static final short COF_HIDDEN = 0x0002;
	public static final short COF_FIRSTTEXT = 0x0004;
	public static final short COF_CREATEDATSTART = 0x0008;
	public static final short MAX_FRAMERATE = 10;

	// Main loop exit codes
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static final short LOOPEXIT_NEXTLEVEL = 1;
	public static final short LOOPEXIT_PREVLEVEL = 2;
	public static final short LOOPEXIT_GOTOLEVEL = 3;
	public static final short LOOPEXIT_NEWGAME = 4;
	public static final short LOOPEXIT_PAUSEGAME = 5;
	public static final short LOOPEXIT_ENDGAME = -2;
	public static final short LOOPEXIT_QUIT = 100;
	public static final short LOOPEXIT_RESTART = 101;
	public static final short LOOPEXIT_APPLETPAUSE = 102;
	public static final short BORDER_LEFT = 1;
	public static final short BORDER_RIGHT = 2;
	public static final short BORDER_TOP = 4;
	public static final short BORDER_BOTTOM = 8;
	public static final short BORDER_ALL = 15;

	public static final int FANIDENTIFIER=0x42324641;
	public static final int TREADMILLIDENTIFIER=0x4232544D;
	public static final int PARTICULESIDENTIFIER = 0x42326AF3;
	public static final int MAGNETIDENTIFIER = 0x42369856;
	public static final int ROPEANDCHAINIDENTIFIER = 0x4232EFFA;
    public static final int JOINTIDENTIFIER = 0x423296EF;
	public static final int BASEIDENTIFIER = 0x42324547;

	// Table d'elimination des entrees/sorties impossibles
	// ---------------------------------------------------
	short Table_InOut[] =
		{
			0, // 0000
			BORDER_LEFT, // 0001 BORDER_LEFT
			BORDER_RIGHT, // 0010 BORDER_RIGHT
			0, // 0011
			BORDER_TOP, // 0100 BORDER_TOP
			BORDER_TOP + BORDER_LEFT, // 0101
			BORDER_TOP + BORDER_RIGHT, // 0110
			0, // 0111
			BORDER_BOTTOM, // 1000 BORDER BOTTOM
			BORDER_BOTTOM + BORDER_LEFT, // 1001
			BORDER_BOTTOM + BORDER_RIGHT, // 1010
			0, // 1011
			0, // 1100
			0, // 1101
			0, // 1110
			0							// 1111
		};
	// RunHeader
	public CSpriteGen spriteGen;
	public CRunApp rhApp;						/// Application info
	public CRunFrame rhFrame;					/// Frame info
	public int rhMaxOI;
	public byte rhStopFlag;					/// Current movement needs to be stopped
	public byte rhEvFlag; 					/// Event evaluation flag
	public int rhNPlayers;					/// Number of players
	public short rhGameFlags;					/// Game flags
	public short rhFree;						/// Alignment
	public byte rhPlayer[] = new byte[4];					/// Current players entry
	public short rhQuit;
	public short rhQuitBis; 					/// Secondary quit (scrollings)
	public int rhReturn;					/// Value to return to the editor
	public int rhQuitParam;
	public int rhNObjects;
	public int rhMaxObjects;
	public CObjInfo rhOiList[];                     		/// ObjectInfo list
	public CEventProgram rhEvtProg = null;
	public int rhLevelSx;				/// Window size
	public int rhLevelSy;
	public int rhWindowX;   				/// Start of window in X/Y
	public int rhWindowY;
	public int rhVBLDeltaOld;				/// Number of VBL
	public int rhVBLObjet;				/// For the objects
	public int rhVBLOld;				/// For the counter
	public short rhMT_VBLStep;   			/// Path movement variables
	public short rhMT_VBLCount;
	public int rhMT_MoveStep;
	public int rhLoopCount;				/// Number of loops since start of level
	public long rhTimer;				/// Timer in 1/50 since start of level
	public long rhTimerOld;				/// For delta calculation
	public int rhTimerDelta;				/// For delta calculation
	public short rhFree4;				/// Alignment
	public int rhOiListPtr;				/// OI list enumeration
	public short rhObListNext;				/// Branch label
	public byte rhMouseUsed;					// Players using the mouse
	public short rhDestroyPos;
	public byte rh2OldPlayer[] = new byte[4];				/// Previous player entries
	public byte rh2NewPlayer[] = new byte[4];				/// Modified player entries
	public byte rh2InputMask[] = new byte[4];				/// Inhibated players entries
	public byte rh2MouseKeys;				/// Mousekey entries
	public short rh2CreationCount;			/// Number of objects created since beginning of frame
	public short rh2Free;
	public short rh2Free2;
	public int rh2MouseSaveX;				/// Mouse saving when pause
	public int rh2MouseSaveY;				/// Mouse saving when pause
	public int rh2PauseCompteur;
	public long rh2PauseTimer;
	public int rh2PauseVbl;
	public int rh4MouseXCenter;
	public int rh4MouseYCenter;
	public int rh3DisplayX;				/// To scroll
	public int rh3DisplayY;
	public int rh3WindowSx;   				/// Window size
	public int rh3WindowSy;
	public short rh3CollisionCount;			/// Collision counter
	public char rh3Scrolling;				/// Flag: we need to scroll
	public int rh3Panic;
	public int rh3PanicBase;
	public int rh3PanicPile;
	public int rh3XMinimum;   				/// Object inactivation coordinates
	public int rh3YMinimum;
	public int rh3XMaximum;
	public int rh3YMaximum;
	public int rh3XMinimumKill;			/// Object destruction coordinates
	public int rh3YMinimumKill;
	public int rh3XMaximumKill;
	public int rh3YMaximumKill;
	public int rh3Graine;
	public Random random;

	//	short		rh4KpxNumOfWindowProcs;					// Number of routines to call
	//	kpxMsg		rh4KpxWindowProc[KPX_MAXNUMBER];		// Message handle routines
	//	kpxLib		rh4KpxFunctions[KPX_MAXFUNCTIONS];		// Available internal routines
	//	CALLANIMATIONS	rh4Animations;						
	//	CALLDIRATSTART	rh4DirAtStart;						
	//	CALLMOVEIT		rh4MoveIt;
	//	CALLAPPROACHOBJECT rh4ApproachObject;
	//	CALLCOLLISIONS rh4Collisions;
	//	CALLTESTPOSITION rh4TestPosition;
	//	CALLGETJOYSTICK rh4GetJoystick;
	//	CALLCOLMASKTESTRECT rh4ColMaskTestRect;
	//	CALLCOLMASKTESTPOINT rh4ColMaskTestPoint;
	public short rh4DemoMode;
	public int rh4PauseKey;
	public String rh4CurrentFastLoop;
	public int rh4EndOfPause;
	public String rh4PSaveFilename;
	public int rh4SaveVersion;
	public int rh4MusicHandle;
	public int rh4MusicFlags;
	public int rh4MusicLoops;
	public int rh4LoadCount;

	//	LPDWORD		rh4TimerEventsBase;				// Timer events base

	//	short		rh4DroppedFlag;
	//	short		rh4NDroppedFiles;
	//	LPSTR		rh4DroppedFiles;
	public Map <String, CLoop> rh4FastLoops = null;
	//	LPSTR		rh4CreationErrorMessages;
	public CValue rh4ExpValue1;				/// New V2
	public CValue rh4ExpValue2;
	public int rh4KpxReturn;				/// WindowProc return
	public int rh4ObjectCurCreate;
	public int rh4ObjectAddCreate;
	public short rh4FakeKey;				/// For step through : fake key pressed
	public char rh4DoUpdate;				/// Flag for screen update on first loop
	public boolean rh4MenuEaten = false;			/// Menu handled in an event?
	public int rh4OnCloseCount;			/// For OnClose event
	public short rh4ScrMode;				/// Current screen mode
	public int rh4VBLDelta;				/// Number of VBL
	public int rh4LoopTheoric;				/// Theorical VBL counter
	public int rh4EventCount;
	public ArrayList<CBackDraw> rh4BackDrawRoutines = null;
	//  public short rh4LastQuickDisplay;			/// Quick - display list
	//  public short rh4FirstQuickDisplay;			/// Quick-display object list
	public int rh4WindowDeltaX;			/// For scrolling
	public int rh4WindowDeltaY;
	public int rh4TimeOut;				/// For time-out!
	public int rh4TabCounter;				/// Objects with tabulation
	public int rh4PosPile;				/// Expression evaluation pile position
	public CValue rh4Results[];				/// Result pile
	public CExp rh4Operators[];				/// Operators pile
	public CExp rh4OpeNull;
	public int rh4CurToken;
	public CExp rh4Tokens[];
	public static final int MAX_INTERMEDIATERESULTS = 128;
	public int rh4FrameRateArray[] = new int[MAX_FRAMERATE];             /// Framerate calculation buffer
	public int rh4FrameRatePos;						/// Position in buffer
	public int rh4FrameRatePrevious;					/// Previous time
	public int rhDestroyList[];			/// Destroy list address
	public int rh4SaveFrame;
	public int rh4SaveFrameCount;
	public double rh4MvtTimerCoef;
	public CObject rhObjectList[] = null;			/// Object list address
	public boolean bOperande;
	public CRunBaseParent rh4Box2DBase;
	public Boolean rh4Box2DObject=false;
	public Boolean rh4Box2DSearched;
	public Boolean bodiesCreated;
	public TimerEvents rh4TimerEvents;
	public ArrayList<CPosOnLoop> rh4PosOnLoop;
	public CCCA modalSubapp = null;

	public int rhJoystickMask;
	public CForEach rh4ForEachs=null;
	public CForEach rh4CurrentForEach;
	public CForEach rh4CurrentForEach2=null;

	// Regular pause action signal
	public boolean bPaused=false;
	public int returnPausedEvent;
	public int pauseEOLCount;
	
	// Collisions optimisations
	private ArrayList<CSprite> rhTempSprColList;
	private ArrayList<CRunExtension> eoglList = null;

	public CLoop findFastLoop (String name)
	{
		return rh4FastLoops.get (name);
	}

	public CLoop addFastLoop (String name)
	{
        CLoop pLoop = findFastLoop (name);
        if (pLoop == null)
        {
            pLoop=new CLoop();
            pLoop.name = name;
            rh4FastLoops.put (name, pLoop);
        }
		return pLoop;
	}

	public CRun()
	{
	}

	public CRun(CRunApp app)
	{
		rhApp = app;
		rhFrame = app.frame;

		// Rempli les CValue pour l'evaluation d'expression
		rh4Results = new CValue[MAX_INTERMEDIATERESULTS];
		rh4Operators = new CExp[MAX_INTERMEDIATERESULTS];
		int n;
		for (n = 0; n < MAX_INTERMEDIATERESULTS; n++)
		{
			rh4Results[n] = new CValue();
		}
		//Arrays.fill(rh4Results, new CValue());
		rh4OpeNull = new EXP_END();
		rh4OpeNull.code = 0;
	}

	public int allocRunHeader()
	{
		// L'object list
		rhObjectList = new CObject[rhFrame.maxObjects];

		// Le programme d'evenements
		rhEvtProg = rhFrame.evtProg;

		// Compte les objinfos
		rhMaxOI = 0;
		COI oi;
		for (oi = rhApp.OIList.getFirstOI(); oi != null; oi = rhApp.OIList.getNextOI())
		{
			if (oi.oiType >= COI.OBJ_SPR)
			{
				rhMaxOI++;
			}
		}

		// L'OIlist
		rhOiList = new CObjInfo[rhMaxOI];
		//int n;
		//for (n = 0; n < rhMaxOI; n++)
		//{
		//	rhOiList[n] = null;
		//}
		Arrays.fill(rhOiList, null);
		// Random generator
		if (rhFrame.m_wRandomSeed == -1)
		{
			int tick = (int) System.currentTimeMillis();
			rh3Graine = (short) (tick);				// Fait un randomize
		}
		else
		{
			rh3Graine = rhFrame.m_wRandomSeed;			// Fixe la valeur donn���e
		}

		random = new Random (rh3Graine);

		// Le generateur de sprites
		spriteGen = new CSpriteGen(rhApp.imageBank, rhApp, rhFrame);

		// La destroy-list
		rhDestroyList = new int[(rhFrame.maxObjects / 32) + 1];

		// Les fast loops
		rh4FastLoops = new HashMap <String, CLoop> ();
		//rh4CurrentFastLoop = new String("");
		rh4CurrentFastLoop = "";

		// Le buffer d'objets
		rhMaxObjects = rhFrame.maxObjects;

		// INITIALISATION DU RESTE DES DONNEES
		rhNPlayers = Math.max(rhEvtProg.nPlayers, rhNPlayers);
		rhWindowX = rhFrame.leX;
		rhWindowY = rhFrame.leY;
		rhLevelSx = rhFrame.leVirtualRect.right;
		if (rhLevelSx == -1)
		{
			rhLevelSx = 0x7FFFF000;		// 2147479552
		}
		rhLevelSy = rhFrame.leVirtualRect.bottom;
		if (rhLevelSy == -1)
		{
			rhLevelSy = 0x7FFFF000;		// 2147479552
		}
		rhNObjects = 0;
		rhStopFlag = 0;
		rhQuit = 0;
		rhQuitBis = 0;
		rhGameFlags &= (GAMEFLAGS_PLAY);
		rhGameFlags |= GAMEFLAGS_LIMITEDSCROLL;
		rh3Panic = 0;
		//   rh4FirstQuickDisplay = (short) 0x8000;
		//    rh4LastQuickDisplay = (short) 0x8000;
		rh4MouseXCenter = rhFrame.leEditWinWidth / 2;
		rh4MouseYCenter = rhFrame.leEditWinHeight / 2;
		rh4FrameRatePos = 0;
		rh4FrameRatePrevious = 0;
		//	CreateCoxHandleRoutines();
		rh4BackDrawRoutines = null;
		rh4SaveFrame = 0;
		rh4SaveFrameCount = -2;
		rh4MvtTimerCoef = 0;

		rhGameFlags |= GAMEFLAGS_REALGAME;

		// Header valide!
		rhFrame.rhOK = true;

		rhJoystickMask=0xFF;

		/* Because this routine reset the window (scrolling) position */

		rhApp.updateWindowPos ();

		return 0;
	}

	public void freeRunHeader()
	{
		rhFrame.rhOK = false;

		// Si demo
		rhObjectList = null;
		rhOiList = null;
		rhDestroyList = null;
		rh4PSaveFilename = null;
		rh4CurrentFastLoop = null;
		//Cleaning FastLoops
		if(this.rh4FastLoops != null
				&& !this.rh4FastLoops.isEmpty()) {
			this.rh4FastLoops.clear();                    
			this.rh4FastLoops = null;
		}
		spriteGen = null;
		if(rh4BackDrawRoutines != null)
			rh4BackDrawRoutines.clear();		

		//rh4BackDrawRoutines = null;
		Runtime.getRuntime().gc();
	}

	public int initRunLoop(boolean bFade)
	{
		int error = 0;

		error = allocRunHeader();
		if (error != 0)
		{
			return error;
		}

		if (bFade)
		{
			rhGameFlags |= GAMEFLAGS_FIRSTLOOPFADEIN;
		}

		initAsmLoop();

		y_InitLevel();
		//      f_InitLoop();

		error = prepareFrame();
		if (error != 0)
		{
			return error;
		}


		error = createFrameObjects(bFade);
		if (error != 0)
		{
			return error;
		}

		redrawLevel((short) (DLF_DONTUPDATE | DLF_STARTLEVEL));

		loadGlobalObjectsData();

		rh4PosOnLoop = null;
		rhEvtProg.prepareProgram();
		rhEvtProg.assemblePrograms(this);

		rhQuitParam = 0;
		f_InitLoop();

		// Nettoie la memoire
		Runtime.getRuntime().gc();

		rh2PauseTimer = 0;
		rhTimerOld = System.currentTimeMillis();
		pauseEOLCount = -1;
		return 0;
	}

	// Un tour de boucle
	public int doRunLoop()
	{
		// Appel du jeu
		rhApp.appRunFlags |= CRunApp.ARF_INGAMELOOP;
		int quit = f_GameLoop();
		rhApp.appRunFlags &= ~CRunApp.ARF_INGAMELOOP;

		//if (rhApp.soundPlayer != null)
		//	rhApp.soundPlayer.tick ();

		// Si fin de FADE IN, detruit les sprites
		if ((rhGameFlags & GAMEFLAGS_FIRSTLOOPFADEIN) != 0)
		{
			f_RemoveObjects();
			rhFrame.fadeTimerDelta = System.currentTimeMillis() - rhTimerOld;
			rhFrame.fadeVblDelta = rhApp.newGetCptVbl() - rhVBLOld;
			y_KillLevel(true);
			rhEvtProg.unBranchPrograms();
		}

		if (quit != 0)
		{
			switch (quit)
			{
			// Passe en pause
			case 5:		// LOOPEXIT_PAUSEGAME:
				rhQuit = 0;
				pause();
				quit = 0;
				break;

				// Redemarre la frame
			case 101:	// LOOPEXIT_RESTART:
				if (rhFrame.fade)
				{
					break;
				}

				// Sortie du niveau preceddent
				rhApp.soundPlayer.stopAllSounds();
				killFrameObjects();
				y_KillLevel(false);
				rhEvtProg.unBranchPrograms();
				freeRunHeader();

				// Redemarre la frame
				rhFrame.leX = rhFrame.leLastScrlX = 0;
				rhFrame.leY = rhFrame.leLastScrlY = 0;
				allocRunHeader();
				initAsmLoop();
				y_InitLevel();
				prepareFrame();
				//		    f_InitLoop();
				createFrameObjects(false);
				loadGlobalObjectsData();
                redrawLevel((short) (DLF_DONTUPDATE | DLF_RESTARTLEVEL));
				rh4PosOnLoop = null;
				rhEvtProg.prepareProgram();
				rhEvtProg.assemblePrograms(this);
				f_InitLoop();
				quit = 0;
				rhQuitParam = 0;
				break;

			case 100:	    // LOOPEXIT_QUIT:
			case -2:	    // LOOPEXIT_ENDGAME:
				rhEvtProg.handle_GlobalEvents(((-4 << 16) | 65533));	// CNDL_QUITAPPLICATION
				break;

			case 102:	    // LOOPEXIT_APPLETPAUSE
				appletPause();
				quit = rhQuit;
				break;
			}
		}
		return quit;
	}

	public void appletPause()
	{
		pause();

		do
		{
			try
			{
				Thread.sleep(20);
			}
			catch (InterruptedException e)
			{
			}
		} while (rhQuit == LOOPEXIT_APPLETPAUSE);

		resume();
	}

	// Sortie de la boucle
	public int killRunLoop(int quit, boolean bLeaveSamples)
	{
		int quitParam;

		// Filtre les codes internes
		if (quit > 100)
		{
			quit = LOOPEXIT_ENDGAME;
		}
		quitParam = rhQuitParam;
		saveGlobalObjectsData();
		killFrameObjects();

		y_KillLevel(bLeaveSamples);
		rhEvtProg.unBranchPrograms();
		freeRunHeader();

		return (CServices.MAKELONG(quit, quitParam));
	}

	public void y_InitLevel()
	{
		resetFrameLayers(-1, false);
	}

	public void initAsmLoop()
	{
		if (rh3Panic == 0)
		{
			spriteGen.winSetColMode(CSpriteGen.CM_BITMAP);				// Collisions precises
			f_ObjMem_Init();
		}
	}

	public void f_ObjMem_Init()
	{
		//for (int i = 0; i < rhMaxObjects; i++)
		//{
		//	rhObjectList[i] = null;

		Arrays.fill(rhObjectList, null);
	}

	// PREPARATION DE LA FRAME AU RUN
	public int prepareFrame()
	{
		COI oiPtr;
		CObjectCommon ocPtr;
		short n, type;

		// Flags de RUN
		/* if ((rhApp.gaFlags & CRunApp.GA_SPEEDINDEPENDANT) != 0 && rhFrame.fade == false)
        {
            rhGameFlags |= GAMEFLAGS_VBLINDEP;
        }
        else
        {
            rhGameFlags &= ~GAMEFLAGS_VBLINDEP;
        } */
		rhGameFlags |= GAMEFLAGS_LOADONCALL;
		rhGameFlags |= GAMEFLAGS_INITIALISING;				// Empeche les evenements...

		// Initialisation du programme
		rh2CreationCount = 0;

		// Initialise la table OiList
		CLO loPtr;
		int count = 0;

		// L'OIlist
		rhOiList = new CObjInfo[rhMaxOI];
		for (oiPtr = rhApp.OIList.getFirstOI(); oiPtr != null; oiPtr = rhApp.OIList.getNextOI())
		{
			type = oiPtr.oiType;
			if (type >= COI.OBJ_SPR)
			{
				rhOiList[count] = new CObjInfo();
				rhOiList[count].copyData(oiPtr);

				// Retrouve un HFII pour les objets TEXT ou QUESTIONS (PARAM_SYSCREATE)
				rhOiList[count].oilHFII = -1;
				if (type == COI.OBJ_TEXT || type == COI.OBJ_QUEST)
				{
					for (loPtr = rhFrame.LOList.first_LevObj(); loPtr != null; loPtr = rhFrame.LOList.next_LevObj())
					{
						if (loPtr.loOiHandle == rhOiList[count].oilOi)
						{
							rhOiList[count].oilHFII = loPtr.loHandle;
							break;
						}
					}
				}
				count++;

				// Flag mouse used...
				ocPtr = (CObjectCommon) oiPtr.oiOC;
				if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0 && ocPtr.ocMovements != null)
				{
					for (n = 0; n < ocPtr.ocMovements.nMovements; n++)
					{
						CMoveDef mvPtr = ocPtr.ocMovements.moveList[n];
						if (mvPtr.mvType == CMoveDef.MVTYPE_MOUSE)
						{
							rhMouseUsed |= 1 << (mvPtr.mvControl - 1);
						}
					}
				}
			}
		}

		//for (int i = 0; i < rhFrame.nLayers; i++)
		//{
		//	CLayer layer = rhFrame.layers[i];
		//	layer.nZOrderMax = 1;
		//}

		for (CLayer layer : rhFrame.layers)
			if(layer != null)
				layer.nZOrderMax = 1;

		return 0;
	}

	// POSITIONNE LA FRAME AU DEBUT
	public int createFrameObjects(boolean fade)
	{
		COI oiPtr;
		CObjectCommon ocPtr;
		short type;
		int n = 0;
		short creatFlags;
		CLO loPtr;

		int error = 0;
		for (n = 0     , loPtr=rhFrame.LOList.first_LevObj(); loPtr!=null; n++, loPtr = rhFrame.LOList.next_LevObj())
		{
			oiPtr = rhApp.OIList.getOIFromHandle(loPtr.loOiHandle);
			ocPtr = (CObjectCommon) oiPtr.oiOC;
			type = oiPtr.oiType;

			creatFlags = COF_CREATEDATSTART;

			// Objet pas dans le bon mode || cree au milieu du jeu. SKIP
			if (loPtr.loParentType != CLO.PARENT_NONE)
			{
				continue;
			}

			// Objet texte: marque comme non destructible
			if (type == COI.OBJ_TEXT)
			{
				creatFlags |= COF_FIRSTTEXT;
			}

			// Objet iconise non texte . SKIP
			if ((ocPtr.ocFlags2 & CObjectCommon.OCFLAGS2_VISIBLEATSTART) == 0)
			{
				if (type == COI.OBJ_QUEST)
				{
					continue;
				}
				creatFlags |= COF_HIDDEN;
			}

			// En mode preparation de fadein, si objet extension & runbeforefadein==0 . SKIP
			if (fade)
			{
				if (type >= COI.KPX_BASE)
				{
					if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_RUNBEFOREFADEIN) == 0)
					{
						continue;
					}
				}
			}

			// Creation de l'objet                
			if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_DONTCREATEATSTART) == 0)
			{
				f_CreateObject(loPtr.loHandle, loPtr.loOiHandle, 0x80000000, 0x80000000, -1, creatFlags, -1, -1);
			}
		}
		rhGameFlags &= ~GAMEFLAGS_INITIALISING;
		return error;
	}

	public void createRemainingFrameObjects()
	{
		COI oiPtr;
		CObjectCommon ocPtr;
		short type;
		int n=0;
		short creatFlags;
		CLO loPtr;

		rhGameFlags &= ~GAMEFLAGS_FIRSTLOOPFADEIN;

		redrawLevel(DLF_DONTUPDATE | DLF_STARTLEVEL);
		F_ReInitObjects();

		for (n = 0     , loPtr=rhFrame.LOList.first_LevObj(); loPtr!=null; n++, loPtr = rhFrame.LOList.next_LevObj())
		{
			oiPtr = rhApp.OIList.getOIFromHandle(loPtr.loOiHandle);
			ocPtr = (CObjectCommon) oiPtr.oiOC;
			type = oiPtr.oiType;

			if (type < COI.KPX_BASE)
			{
				continue;
			}
			if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_RUNBEFOREFADEIN) != 0)
			{
				continue;
			}

			creatFlags = COF_CREATEDATSTART;

			// Objet pas dans le bon mode || cree au milieu du jeu. SKIP
			if (loPtr.loParentType != CLO.PARENT_NONE)
			{
				continue;
			}

			// Objet iconise non texte . SKIP
			if ((ocPtr.ocFlags2 & CObjectCommon.OCFLAGS2_VISIBLEATSTART) == 0)
			{
				if (type != COI.OBJ_TEXT)
				{
					continue;
				}
				creatFlags |= COF_HIDDEN;
			}

			// Creation de l'objet                
			if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_DONTCREATEATSTART) == 0)
			{
				f_CreateObject(loPtr.loHandle, loPtr.loOiHandle, 0x80000000, 0x80000000, -1, creatFlags, -1, -1);
			}
		}
		rhEvtProg.assemblePrograms(this);

		// Remet le timer
		rhTimerOld = System.currentTimeMillis() - rhFrame.fadeTimerDelta;
		rhVBLOld = (rhApp.newGetCptVbl() - rhFrame.fadeVblDelta);
	}

	public void F_ReInitObjects()
	{
		int count = rhNObjects;
		// Make for Performance
		CObject[] localObjectList = rhObjectList;
		for(CObject hoPtr : localObjectList) {
			if (hoPtr != null) {
				--count;
				if(hoPtr.ros != null)
				{
					hoPtr.ros.reInit_Spr(false);
				}
				if(count == 0)
					break;
			}
		}
	}

	// DESTRUCTION DES OBJETS DE LA SCENE
	public void killFrameObjects()
	{
		// Arrete tous les sprites, en mode FAST
		short n = 0;
		// Kill all the objects except for the Physics Engine objects

		for(CObject pHo : rhObjectList) {
			if(rhNObjects == 0)
				break;
			if (pHo != null && (pHo.hoType<32 || pHo.hoCommon.ocIdentifier!=CRun.BASEIDENTIFIER) )
				f_KillObject(n, true);
			n++;		
		}
		// Kill the Physics Engine objects
		n = 0;
		for(CObject pHo : rhObjectList) {
			if(rhNObjects == 0)
				break;
			if (pHo != null && pHo.hoType>=32 && pHo.hoCommon.ocIdentifier==CRun.BASEIDENTIFIER )
				f_KillObject(n, true);
			n++;		
		}

		//      rh4FirstQuickDisplay = (short) 0x8000;
	}

	// End frame
	public void y_KillLevel(boolean bLeaveSamples)
	{
		// Clear ladders & additional backdrops
		resetFrameLayers(-1, false);

		// ++v1.03.35: stop sounds only if not GANF_SAMPLESOVERFRAMES
		if (!bLeaveSamples)
		{
			if ((rhApp.gaNewFlags & CRunApp.GANF_SAMPLESOVERFRAMES) == 0)
			{
				rhApp.soundPlayer.stopAllSounds();
				rhApp.musicPlayer.stopAllMusics();
			}
		}
	}

	public void resetFrameLayers(int nLayer, boolean bDeleteFrame)
	{
		int i, nLayers;

		if (nLayer == -1)
		{
			i = 0;
			nLayers = rhFrame.nLayers;
		}
		else
		{
			i = nLayer;
			nLayers = (nLayer + 1);
		}

		for (i = 0; i < nLayers; i++)
		{
			CLayer pLayer = rhFrame.layers[i];

			// Delete backdrop sprites

			int j;
			CLO plo;
			int nLOs = pLayer.nBkdLOs;
			for (j = 0; j < nLOs; j++)
			{
				plo = rhFrame.LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex + j));

				// Delete sprite
				for (int ns = 0; ns < 4; ns++)
				{
					if (plo.loSpr[ns] != null)
					{
						spriteGen.delSpriteFast(plo.loSpr[ns]);
						plo.loSpr[ns] = null;
					}
				}
			}

			if (pLayer.pBkd2 != null)
			{
				int pLayer_pBkd2_size = pLayer.pBkd2.size();
				for (j = 0; j < pLayer_pBkd2_size; j++)
				{
					CBkd2 pbkd = pLayer.pBkd2.get(j);
					// Delete sprite
					for (int ns = 0; ns < 4; ns++)
					{
						if (pbkd.pSpr[ns] != null)
						{
							spriteGen.delSpriteFast(pbkd.pSpr[ns]);
							pbkd.pSpr[ns] = null;
						}
					}
				}
			}

			// Initialize permanent data
			pLayer.dwOptions = pLayer.backUp_dwOptions;
			pLayer.xCoef = pLayer.backUp_xCoef;
			pLayer.yCoef = pLayer.backUp_yCoef;
			pLayer.nBkdLOs = pLayer.backUp_nBkdLOs;
			pLayer.nFirstLOIndex = pLayer.backUp_nFirstLOIndex;

			// Initialize volatil data
			pLayer.x = pLayer.y = pLayer.dx = pLayer.dy = 0;

			// Free additional backdrops
			pLayer.pBkd2 = null;

			// Free ladders
			pLayer.pLadders = null;

			// Free loZones
			if(pLayer.m_loZones != null)
				pLayer.m_loZones.clear();
			pLayer.m_loZones = null;
		}
	}

	// VIRE LES SPRITES EN FIN DE FADE IN
	// ----------------------------------
	void f_RemoveObjects()
	{
		int count = rhNObjects;
		CObject[] localObjectList = rhObjectList;
		for(CObject hoPtr : localObjectList)
		//for(CObject hoPtr : rhObjectList)
		{
			if (hoPtr != null) {
				--count;
				if(hoPtr.ros != null)
				{
					if (hoPtr.roc.rcSprite != null)
					{
						// Save Z-order value before deleting sprite
						hoPtr.ros.rsZOrder = hoPtr.roc.rcSprite.sprZOrder;

						// Delete sprite
						spriteGen.delSpriteFast(hoPtr.roc.rcSprite);
					}
					/*   if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_QUICKDISPLAY) != 0)
		            {
		                remove_QuickDisplay(hoPtr);
		            } */
				}
				if(count == 0)
					break;
			}
		}
	}

	// -------------------------------------------------------------------------------
	// SAUVEGARDE / RESTAURATION DES OBJETS GLOBAUX
	// -------------------------------------------------------------------------------
	/** Saves the global objects into a temporary data zone.
	 * This data zone is recovered at the next frame, to restore the 
	 * values in the global objects.
	 */
	public void saveGlobalObjectsData()
	{
		CObject hoPtr;
		CObjInfo oilPtr;
		int oil, obj;
		COI oiPtr;
		String name;
		short o;
		int rhOiList_length = rhOiList.length;
		for (oil = 0; oil < rhOiList_length; oil++)
		{
			oilPtr = rhOiList[oil];

			// Un objet defini?
			o = oilPtr.oilObject;
			if (oilPtr.oilOi != 0x7FFF && (o & 0x8000) == 0)
			{
				oiPtr = rhApp.OIList.getOIFromHandle(oilPtr.oilOi);

				// Un objet global?
				if ((oiPtr.oiFlags & COI.OIF_GLOBAL) != 0)
				{
					hoPtr = rhObjectList[o];

					// Un objet sauvable?
					if (oilPtr.oilType != COI.OBJ_TEXT && oilPtr.oilType != COI.OBJ_COUNTER && hoPtr.rov == null)
					{
						continue;
					}

					// Recherche un element deja defini
					name = oilPtr.oilName + "::" + Integer.toString(oilPtr.oilType);

					if (rhApp.adGO == null)
					{
						rhApp.adGO = new ArrayList<CSaveGlobal>();
					}

					// Rechercher l'objet dans les objets 
					boolean flag = false;
					CSaveGlobal save = null;
					int rhApp_adGO_size = rhApp.adGO.size();
					for (obj = 0; obj < rhApp_adGO_size; obj++)
					{
						save = rhApp.adGO.get(obj);
						if (name.compareTo(save.name) == 0)
						{
							flag = true;
							break;
						}
					}
					if (flag == false)
					{
						save = new CSaveGlobal();
						save.name = name;
						save.objects = new ArrayList<Object>();
						rhApp.adGO.add(save);
					}
					else
					{
						save.objects.clear();
					}

					// Stockage des valeurs...
					int n;
					while (true)
					{
						hoPtr = rhObjectList[o];

						// Stocke
						if (oilPtr.oilType == COI.OBJ_TEXT)
						{
							CText text = (CText) hoPtr;
							CSaveGlobalText saveText = new CSaveGlobalText();
							saveText.text = text.rsTextBuffer;
							saveText.rsMini = text.rsMini;
							save.objects.add(saveText);
						}
						else if (oilPtr.oilType == COI.OBJ_COUNTER)
						{
							CCounter counter = (CCounter) hoPtr;
							CSaveGlobalCounter saveCounter = new CSaveGlobalCounter();
							saveCounter.value = new CValue(counter.rsValue);
							saveCounter.rsMini = counter.rsMini;
							saveCounter.rsMaxi = counter.rsMaxi;
							saveCounter.rsMiniDouble = counter.rsMiniDouble;
							saveCounter.rsMaxiDouble = counter.rsMaxiDouble;
							save.objects.add(saveCounter);
						}
						else
						{
							CSaveGlobalValues saveValues = new CSaveGlobalValues();
							saveValues.rvNumberOfValues = hoPtr.rov.rvNumberOfValues;
							saveValues.flags = hoPtr.rov.rvValueFlags;
							saveValues.values = new CValue[hoPtr.rov.rvNumberOfValues];
							for (n = 0; n < hoPtr.rov.rvNumberOfValues; n++)
							{
								saveValues.values[n] = null;
								if (hoPtr.rov.rvValues[n] != null)
								{
									saveValues.values[n] = new CValue(hoPtr.rov.rvValues[n]);
								}
							}
							saveValues.rvNumberOfStrings = hoPtr.rov.rvNumberOfStrings;
							saveValues.strings = new String[hoPtr.rov.rvNumberOfStrings];
							for (n = 0; n < hoPtr.rov.rvNumberOfStrings; n++)
							{
								saveValues.strings[n] = null;
								if (hoPtr.rov.rvStrings[n] != null)
								{
									saveValues.strings[n] = new String(hoPtr.rov.rvStrings[n]);
								}
							}
							save.objects.add(saveValues);
						}

						// Un autre objet?
						o = hoPtr.hoNumNext;
						if ((o & 0x8000) != 0)
						{
							break;
						}
					}
				}
			}
		}
	}

	/** Restores the global objects values.
	 * Called at start of frame, copies the values in the 
	 * global objects to restore their values.
	 */
	public void loadGlobalObjectsData()
	{
		CObject hoPtr;
		CObjInfo oilPtr;
		int oil, obj;
		COI oiPtr;
		String name;
		short o;

		if (rhApp.adGO == null)
		{
			return;
		}

		int rhOiList_length = rhOiList.length;
		for (oil = 0; oil < rhOiList_length; oil++)
		{
			oilPtr = rhOiList[oil];

			// Un objet defini?
			o = oilPtr.oilObject;
			if (oilPtr.oilOi != 0x7FFF && (o & 0x8000) == 0)
			{
				oiPtr = rhApp.OIList.getOIFromHandle(oilPtr.oilOi);

				// Un objet global?
				if ((oiPtr.oiFlags & COI.OIF_GLOBAL) != 0)
				{
					name = oilPtr.oilName + "::" + Integer.toString(oilPtr.oilType);

					// Recherche dans les headers
					int rhApp_adGO_size = rhApp.adGO.size();
					for (obj = 0; obj < rhApp_adGO_size; obj++)
					{
						CSaveGlobal save = rhApp.adGO.get(obj);
						if (name.compareTo(save.name) == 0)
						{
							int count = 0;
							while (true)
							{
								hoPtr = rhObjectList[o];

								if (oilPtr.oilType == COI.OBJ_TEXT)
								{
									CSaveGlobalText saveText = (CSaveGlobalText) save.objects.get(count);
									CText text = (CText) hoPtr;
									text.rsTextBuffer = saveText.text;
									text.rsMini = saveText.rsMini;
								}
								else if (oilPtr.oilType == COI.OBJ_COUNTER)
								{
									CSaveGlobalCounter saveCounter = (CSaveGlobalCounter) save.objects.get(count);
									CCounter counter = (CCounter) hoPtr;
									counter.rsValue = new CValue(saveCounter.value);
									counter.rsMini = saveCounter.rsMini;
									counter.rsMaxi = saveCounter.rsMaxi;
									counter.rsMiniDouble = saveCounter.rsMiniDouble;
									counter.rsMaxiDouble = saveCounter.rsMaxiDouble;
								}
								else
								{
									CSaveGlobalValues saveValues = (CSaveGlobalValues) save.objects.get(count);
								    if (saveValues.rvNumberOfValues > hoPtr.rov.rvNumberOfValues)
								    {
										if ( !hoPtr.rov.extendValues(saveValues.rvNumberOfValues) )
										{
											return;
										}
								    }
								    if (saveValues.rvNumberOfStrings > hoPtr.rov.rvNumberOfStrings)
								    {
										if ( !hoPtr.rov.extendStrings(saveValues.rvNumberOfStrings) )
										{
											return;
										}
								    }
									hoPtr.rov.rvNumberOfValues = saveValues.rvNumberOfValues;
									hoPtr.rov.rvNumberOfStrings = saveValues.rvNumberOfStrings;
									hoPtr.rov.rvValueFlags = saveValues.flags;
									int n;
									for (n = 0; n < saveValues.rvNumberOfValues; n++)
									{
										if (saveValues.values[n] != null)
										{
											hoPtr.rov.rvValues[n] = null;
											hoPtr.rov.rvValues[n] = new CValue(saveValues.values[n]);
										}
									}
									for (n = 0; n < saveValues.rvNumberOfStrings; n++)
									{
										if (saveValues.strings[n] != null)
										{
											hoPtr.rov.rvStrings[n] = new String(saveValues.strings[n]);
										}
									}
								}

								// Un autre objet?
								o = hoPtr.hoNumNext;
								if ((o & 0x8000) != 0)
								{
									break;
								}

								// Regarde si il existe un suivant...
								count++;
								if (count >= save.objects.size())
								{
									break;
								}
							}
							break;
						}
					}
				}
			}
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////
	//
	// GAMEF.CPP
	//
	/** Creates a new object.
	 */
	public int f_CreateObject(short hlo, short oi, int coordX, int coordY, int initDir, short flags, int nLayer, int numCreation)
	{
		while (true)
		{
			CCreateObjectInfo cob = new CCreateObjectInfo();

			// Trouve l'adresse du LO	
			// ~~~~~~~~~~~~~~~~~~~~~~~~~
			CLO loPtr = null;
			if (hlo != -1)
			{
				loPtr = rhFrame.LOList.getLOFromHandle(hlo);
			}

			// Trouve l'adresse du HFRAN
			// ~~~~~~~~~~~~~~~~~~~~~~~~~
			COI oiPtr = rhApp.OIList.getOIFromHandle(oi);
			CObjectCommon ocPtr = (CObjectCommon) oiPtr.oiOC;

			// Flag visible at start
			// --------------------
			if ((ocPtr.ocFlags2 & CObjectCommon.OCFLAGS2_VISIBLEATSTART) == 0)
			{
				flags |= COF_HIDDEN;
			}

			// Pas trop d'objets?
			// ~~~~~~~~~~~~~~~~~~
			if (rhNObjects >= rhMaxObjects)
			{
				break;
			}

			// Cree l'objet
			CObject hoPtr = null;
			switch (oiPtr.oiType)
			{
			case 2:         // OBJ_SPR
				hoPtr = new CActive();
				break;
			case 3:         // OBJ_TEXT
				hoPtr = new CText();
				break;
			case 4:         // OBJ_QUEST
				hoPtr = new CQuestion();
				break;
			case 5:         // OBJ_SCORE
				hoPtr = new CScore();
				break;
			case 6:         // OBJ_LIVES
				hoPtr = new CLives();
				break;
			case 7:         // OBJ_COUNTER
				hoPtr = new CCounter();
				break;
				/* case 8:         // OBJ_RTF
                    hoPtr = new CRtf();
                    break; */
			case 9:         // OBJ_CCA
				hoPtr = new CCCA();
				break;
			default:        // Extensions
				hoPtr = new CExtension(oiPtr.oiType, this);
				if (((CExtension)hoPtr).ext==null)
				{
					hoPtr=null;
				}
				break;
			}

			if (hoPtr == null)
				break;

			// Insere l'objet dans la liste
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (numCreation < 0)
			{
				for (numCreation = 0; numCreation < rhMaxObjects; numCreation++)
				{
					if (rhObjectList[numCreation] == null)
					{
						break;
					}
				}
			}
			if (numCreation >= rhMaxObjects)
			{
				return -1;
			}
			rhObjectList[numCreation] = hoPtr;
			rhNObjects++;
			hoPtr.hoIdentifier = ocPtr.ocIdentifier;			//; L'identifier ASCII de l'objet
			hoPtr.hoOEFlags = ocPtr.ocOEFlags;
			if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_WINDOWPROC)!=0
					|| hoPtr instanceof CCCA) /* sub-applications are sprites too in Android */
			{
				hoPtr.hoOEFlags|=CObjectCommon.OEFLAG_SPRITES;
			}

			// Gestion de la boucle principale
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (numCreation > rh4ObjectCurCreate)				// Si objet DEVANT l'objet courant
			{
				rh4ObjectAddCreate++;					// Il faut explorer encore!
			}
			//            flagPlus=1;										//; Si une erreur ensuite...

			// Rempli la structure headerObject
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			hoPtr.hoNumber = (short) numCreation;					  			//; Numero de l'objet
			rh2CreationCount++;
			if (rh2CreationCount == 0)					// V243 protection tour de compteur
			{
				rh2CreationCount = 1;
			}
			hoPtr.hoCreationId = rh2CreationCount;		// Numero de creation!
			hoPtr.hoOi = oi;										//; L'OI
			hoPtr.hoHFII = hlo;									//; le HLO
			hoPtr.hoType = oiPtr.oiType;					//; Le type de l'objet
			oi_Insert(hoPtr);									//; Branche dans la liste
			hoPtr.hoAdRunHeader = this;							//; L'adresse de rhPtr
			hoPtr.hoCallRoutine = true;

			// --------------------------------------------
			// Gestion des LOADONCALL
			// --------------------------------------------
			if ( (oiPtr.oiLoadFlags & COI.OILF_ELTLOADED) == 0 )
				oiPtr.loadOnCall(rhApp);
			
			// Adresse de l'objectsCommon
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~
			hoPtr.hoCommon = ocPtr;

            // Sprite en mode inbitate?
            if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MANUALSLEEP) == 0)
            {
                // On detruit... sauf si...
                hoPtr.hoOEFlags &= ~CObjectCommon.OEFLAG_NEVERSLEEP;

                // On teste des collisions avec le decor?
                if ((hoPtr.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKBACK) != 0)
                {
                    // Si masque des collisions general
                    if ((rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0)
                    {
                        hoPtr.hoOEFlags |= CObjectCommon.OEFLAG_NEVERSLEEP;
                    }
                }
                // Ou test des collisions normal
                if ((hoPtr.hoLimitFlags & (CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_QUICKBORDER)) != 0)
                {
                    hoPtr.hoOEFlags |= CObjectCommon.OEFLAG_NEVERSLEEP;
                }
            }

			// Rempli la structure CreateObjectInfo (virer X et Y)
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			int x = coordX;									// X
			if (x == 0x80000000)
			{
				x = loPtr.loX;
			}
			cob.cobX = x;
			hoPtr.hoX = x;

			int y = coordY;									// Y
			if (y == 0x80000000)
			{
				y = loPtr.loY;
			}
			cob.cobY = y;
			hoPtr.hoY = y;

			// Set layer
			if ( loPtr != null )
			{
				if ( nLayer == -1 )
				{
					nLayer = loPtr.loLayer;
				}
			}
			//else
			//{
			//	nLayer = 0;
			//}
			cob.cobLayer = nLayer;
			hoPtr.hoLayer = nLayer;

			// Set z-order value
			CLayer pLayer = rhFrame.layers[nLayer];
			pLayer.nZOrderMax++;
			cob.cobZOrder = pLayer.nZOrderMax;

			cob.cobFlags = flags;								//; Flags creation
			cob.cobDir = initDir;								//; Direction initiale
			cob.cobLevObj = loPtr;							//; Huge levobj

			// --------------------------------------------
			// Gestion des Animations / Mouvements / Values
			// --------------------------------------------

			// Debut des structures facultatives	
			hoPtr.roc = null;
			if ((hoPtr.hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0)
			{
				hoPtr.roc = new CRCom();
				hoPtr.roc.init();
			}

			// Appel des routines d'initialisation Movements
			// ---------------------------------------------
			hoPtr.rom = null;
			if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0)
			{
				hoPtr.rom = new CRMvt();
				if ((cob.cobFlags & COF_NOMOVEMENT) == 0)
				{
					hoPtr.rom.init(0, hoPtr, ocPtr, cob, -1);
				}
			}

			// Appel des routines d'initialisation Animation
			// ---------------------------------------------
			hoPtr.roa = null;
			if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_ANIMATIONS) != 0)
			{
				hoPtr.roa = new CRAni();
				hoPtr.roa.init(hoPtr);
			}

			// Appel des routines d'initialisation sprite 1
			// ---------------------------------------------
			hoPtr.ros = null;
			if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0)
			{
				hoPtr.ros = new CRSpr();
				hoPtr.ros.init1(hoPtr, ocPtr, cob);
			}

			// Appel des routines d'initialisation Values
			// ---------------------------------------------
			hoPtr.rov = null;
			if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_VALUES) != 0)
			{
				hoPtr.rov = new CRVal();
				hoPtr.rov.init(hoPtr, ocPtr, cob);
			}

			// -----------------------------------------------
			// Appel de la routine d'initialisation standard
			// -----------------------------------------------
			hoPtr.init(ocPtr, cob);

			// Appel des routines d'initialisation sprite 2
			// ---------------------------------------------
			if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0)
			{
				hoPtr.ros.init2();
			}
			// Sortie sans erreur
			return numCreation;									// Retourne avec EAX=NOBJECT
		}
		return -1;
	}

	// --------------------------------------------------------------------------
	// --------------------------------------------------------------------------
	// DESTRUCTION D'UN OBJET
	// --------------------------------------------------------------------------
	// --------------------------------------------------------------------------
	/** Destroys an object.
	 */
	public void f_KillObject(int nObject, boolean bFast)
	{
		// Pointe l'objet
		// ~~~~~~~~~~~~~~
		CObject hoPtr = rhObjectList[nObject];
		if (hoPtr == null)
		{
			return;
		}

		// V243 Si sprite a moitie efface
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (bFast == true && hoPtr.hoCreationId == 0)
		{
			rhObjectList[nObject] = null;
			rhNObjects--;
			return;
		}

		// Vire les pointeurs dans les routines SHOOT
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		killShootPtr(hoPtr);

		// Detruit les mouvements, les values
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (hoPtr.rom != null)
		{
			hoPtr.rom.kill(bFast);
		}
		if (hoPtr.rov != null)
		{
			hoPtr.rov.kill(bFast);
		}
		if (hoPtr.ros != null)
		{
			hoPtr.ros.kill(bFast);
		}
		if (hoPtr.roc != null)
		{
			hoPtr.roc.kill(bFast);
		}

		/*	// Decremente les indicateurs de tabulation
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        if (hoPtr.hoOEFlags&OEFLAG_TABSTOP && hoPtr.hoOEFlags&OEFLAG_WINDOWPROC)
        {
        rh4.rh4TabCounter--;
        }
		 */
		// Appelle la routine de destruction specifique
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		hoPtr.kill(bFast);

		// Enleve le OI (pas en mode panique!)
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		oi_Delete(hoPtr);

		// Discard?
		CObjInfo poil = hoPtr.hoOiList;
		if ( poil != null && poil.oilObject == (short)0x8000 )		// last object instance?
		{
			//Log.Log ("Last instance of: " + poil.oilName);

			// Is the object discardable?
			COI oiPtr = rhApp.OIList.getOIFromHandle(poil.oilOi);
			if ( oiPtr != null && (oiPtr.oiFlags & COI.OIF_DISCARDABLE) != 0 )
			{
				//Log.Log ("Discard: " + poil.oilName);

				// Discard object
				oiPtr.discard(rhApp);
			}
		}

		hoPtr.hoCreationId = 0;
		rhObjectList[nObject] = null;
		// Ajout Yves Build 242
		rhNObjects--;

		// Efface les appels aux routines d'affichage
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		hoPtr.hoCallRoutine = false;
	}

	// Call some objects at the end of the game loop
	// ---------------------------------------------
	public void eoglList_addObject(CRunExtension ext)
	{
		if ( eoglList == null )
			eoglList = new ArrayList<CRunExtension>();
		if ( !eoglList.contains(ext) )
			eoglList.add(ext);
	}
	public void eoglList_call()
	{
		if ( eoglList != null  && eoglList.size() > 0)
		{
			//int lg = eoglList.size();
			//if ( lg != 0 )
			//	Log.Log("eoglList_call: " + lg);
			for (CRunExtension ext : eoglList)
				ext.onEndOfGameLoop();
			eoglList.clear();
		}
	}
	

	// ---------------------------------------------------------------------------
	// ---------------------------------------------------------------------------
	// GESTION DE LA LISTE DE DESTROY
	// ---------------------------------------------------------------------------
	// ---------------------------------------------------------------------------

	// Additionne un objet a la liste de destroy
	// ------------------------------------------
	/** Adds an object to the destroy list.
	 */
	public void destroy_Add(int hoNumber)
	{
		rhDestroyList[hoNumber / 32] |= (1 << (hoNumber & 31));
		rhDestroyPos++;
	}

	// Detruit tous les sprites de la liste
	// ------------------------------------
	/** Destroys all the objects in the destroy list.
	 */
	public void destroy_List()
	{
		//Log.Log("Destroy List entered ...");
		if (rhDestroyPos == 0)
		{
			return;
		}

		int nob = 0;
		int dw;
		int count;
		while (nob < rhMaxObjects)
		{
			dw = rhDestroyList[nob / 32];
			if (dw != 0)
			{
				rhDestroyList[nob / 32] = 0;
				for (count = 0; dw != 0 && count < 32; count++)
				{
					if ((dw & 1) != 0)
					{
						// Appeler le message NO MORE OBJECT juste avant la destruction (si objet sprite!)
						CObject pHo = rhObjectList[nob + count];
						if (pHo != null)
						{
							if (pHo.hoOiList.oilNObjects == 1)
							{
								rhEvtProg.handle_Event(pHo, (pHo.hoType | (-33 << 16)));	    // CNDL_EXTNOMOREOBJECT
							}
						}
						// Detruit l'objet!
						f_KillObject(nob + count, false);
						rhDestroyPos--;
					}
					dw = dw >> 1;
				}
				if (rhDestroyPos == 0)
				{
					return;
				}
			}
			nob += 32;
		}
	}

	// Detruit les references aux objets shoot
	// ---------------------------------------
	/** Destroys the references to the shooted objects.
	 */
	void killShootPtr(CObject hoSource)
	{
		int count = rhNObjects;
		CObject[] localObjectList = rhObjectList;
		for(CObject hoPtr : localObjectList) {
			if (hoPtr != null) {
				--count;
				if(hoPtr.rom != null)
				{
					if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_BULLET)
					{
						CMoveBullet mBullet = (CMoveBullet) hoPtr.rom.rmMovement;
						if (mBullet.MBul_ShootObject == hoSource && mBullet.MBul_Wait == true)
						{
							mBullet.startBullet();
						}
					}
				}
				if(count == 0)
					break;
			}
		}
	}

	// Insere l'objet [esi] dans les liste d'OI
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/** Inserts a new object in the objInfoList.
	 */
	public void oi_Insert(CObject pHo)
	{
		short oi = pHo.hoOi;

		int num;
		for (num = 0; num < rhMaxOI; num++)
		{
			if (rhOiList[num].oilOi == oi)
			{
				break;
			}
		}
		CObjInfo poil = rhOiList[num];

		if ((poil.oilObject & 0x8000) != 0)
		{
			// N'existe pas avant
			poil.oilObject = pHo.hoNumber;
			pHo.hoNumPrev = (short) (num | 0x8000);
			pHo.hoNumNext = (short) 0x8000;
		}
		else
		{
			// Existe avant: insere en tete de liste
			CObject pHo2 = rhObjectList[poil.oilObject];
			pHo.hoNumPrev = pHo2.hoNumPrev;
			pHo2.hoNumPrev = pHo.hoNumber;
			pHo.hoNumNext = pHo2.hoNumber;
			poil.oilObject = pHo.hoNumber;
		}

		// Prend les donnees contenues dans oiList
		pHo.hoEvents = poil.oilEvents;					// Les evenements
		pHo.hoOiList = poil;							// L'adresse dans la liste OiList
		pHo.hoLimitFlags = poil.oilLimitFlags;
		if (pHo.hoHFII == -1)					// Si le HFII est mauvais, met le premier disponible
		{
			pHo.hoHFII = poil.oilHFII;
		}
		else
		{
			if (poil.oilHFII == -1)
			{
				poil.oilHFII = pHo.hoHFII;
			}
		}
		poil.oilNObjects += 1;						// Un objet de plus de meme OI
	}

	// Delete un OI retourne le numero de l'objet suivant en AX
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/** Deletes an object from the objInfoList.
	 */
	void oi_Delete(CObject pHo)
	{
		// Decremente dans la liste des OI
		CObjInfo poil = pHo.hoOiList;
		poil.oilNObjects -= 1;

		// Gere les precedents/suivants
		if (pHo.hoNumPrev >= 0)
		{
			CObject pHo2 = rhObjectList[pHo.hoNumPrev];
			if (pHo.hoNumNext >= 0)
			{
				// Au milieu
				CObject pHo3 = rhObjectList[pHo.hoNumNext];
				if (pHo2 != null)
				{
					pHo2.hoNumNext = pHo.hoNumNext;
				}
				if (pHo3 != null)
				{
					pHo3.hoNumPrev = pHo.hoNumPrev;
				}
			}
			else
			{
				if (pHo2 != null)
				{
					pHo2.hoNumNext = (short) 0x8000;
				}
			}
		}
		else
		{
			// Au debut de la liste
			if (pHo.hoNumNext >= 0)
			{
				CObject pHo2 = rhObjectList[pHo.hoNumNext];
				if (pHo2 != null)
				{
					pHo2.hoNumPrev = pHo.hoNumPrev;
					poil.oilObject = pHo2.hoNumber;
				}
			}
			// Plus rien dans la liste!
			else
			{
				poil.oilObject = (short) 0x8000;
			}
		}
	}
	
	public void pauseEOL()
	{
		pauseEOLCount = rh4EventCount;
	}

	/** Goes into pause mode.
	 */
	public void pause()
	{
		// Le compteur de sauvegarde
		// ~~~~~~~~~~~~~~~~~~~~~~~~~
		rh2PauseCompteur++;
		if (rh2PauseCompteur == 1)
		{
			Log.Log ("CRun: pause()");

		    MMFRuntime.inst.setFrameRate (0);
			// Sauve le timer
			// ~~~~~~~~~~~~~~
			rh2PauseTimer = System.currentTimeMillis();

			// Sauve le VBL
			// ~~~~~~~~~~~~
			rh2PauseVbl = rhApp.newGetCptVbl() - rhVBLOld;

			// Arret de tous les objets
			// ~~~~~~~~~~~~~~~~~~~~~~~~
			int count = rhNObjects;
			CObject[] localObjectList = rhObjectList;
			for(CObject hoPtr : localObjectList) {
				if (hoPtr == null)
					continue;
				--count;
				if(hoPtr.hoType >= COI.KPX_BASE)
				{
					CExtension e = (CExtension) hoPtr;
					e.ext.pauseRunObject();
				}
				if(count == 0)
					break;

			}
			// Arret des musiques et des sons
			// ------------------------------
			if((rhApp.gaNewFlags & CRunApp.GANF_SAMPLESEVENIFNOTFOCUS) == 0)
				rhApp.soundPlayer.pause2();
		}
	}

	/** Quits the pause mode.
	 */
	public void resume()
	{
		// Uniquement au dernier retour
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (rh2PauseCompteur != 0)
		{
			rh2PauseCompteur--;
			if (rh2PauseCompteur == 0)
			{
				Log.Log ("CRun: resume()");

				/* still got a valid surface? */
				// Build 284 not check anymore for surface here, if not exist will be created at SurfaceView
				// start from resumeanyway
				//if (SurfaceView.hasSurface)
                MMFRuntime.inst.setFrameRate (rhApp.gaFrameRate);

				// Remet tous les objets
				// ~~~~~~~~~~~~~~~~~~~~~
				int count = rhNObjects;
				CObject[] localObjectList = rhObjectList;
				for(CObject hoPtr : localObjectList) {
					if (hoPtr == null)
						continue;
					--count;
					if(hoPtr.hoType >= COI.KPX_BASE)
					{
						CExtension e = (CExtension) hoPtr;
						e.ext.continueRunObject();
					}
					if(count == 0)
						break;

				}
				// Remet le timer
				// ~~~~~~~~~~~~~~
				long tick = System.currentTimeMillis();
				tick -= rh2PauseTimer;
				rhTimerOld += tick;

				// Remet le VBL
				// ~~~~~~~~~~~~
				rhVBLOld = rhApp.newGetCptVbl() - rh2PauseVbl;
				rh4PauseKey = 0;

				// Remet des musiques et des sons
				// ------------------------------
				if(returnPausedEvent == 0)
					rhApp.soundPlayer.resume2();
				bPaused = false;
				
				// Reload Image for Joystick
				// ------------------------------
				if(rhApp.joystick != null 
						&& (rhApp.joystick.joyBack == null || rhApp.joystick.joyFront == null))
					rhApp.joystick.loadImages(); 		// if not Loaded, please load again
			}
		}
	}


	public void redrawLevel(int flags)
	{
		int			lgEdit, htEdit;
		int			i, obst, v = 0, cm_box = 0, x2edit, y2edit;
		boolean		flgColMaskEmpty = false;
		short		img = 0;
		CLO			plo = null;
		CObject		hoPtr = null;
		int			dxLayer = 0, dyLayer = 0;
		boolean		bTotalColMask = ((rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0);
		boolean		bUpdateColMask = ((flags & DLF_DONTUPDATECOLMASK) == 0);
		boolean		bSkipLayer0 = ((flags & DLF_SKIPLAYER0) != 0);
		CRect		rc = new CRect();
		int			nLayer = 0;
		boolean		bLayer0Invisible = false;

		if(rhFrame.colMask == null)
		{
			bUpdateColMask = false;
		}

		rc.left = rc.top = 0;
		rc.right = rhFrame.leEditWinWidth;
		rc.bottom = rhFrame.leEditWinHeight;
		lgEdit = rc.right;
		x2edit = lgEdit - 1;
		htEdit = rc.bottom;
		y2edit = htEdit - 1;

		// Hide or show layers? => hide or show objects
		if ( (flags & (DLF_DRAWOBJECTS|DLF_STARTLEVEL|DLF_RESTARTLEVEL)) != 0 )
		{
			for (; nLayer < rhFrame.nLayers; nLayer++)
			{
				CLayer pLayer = rhFrame.layers[nLayer];
				if ( (pLayer.dwOptions & CLayer.FLOPT_TOSHOW) != 0 )
					f_ShowAllObjects(nLayer, true);
				if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
					f_ShowAllObjects(nLayer, false);
			}
		}

		// Hide background objects from layers to hide
		// Build 248 : Move here before F_UpdateWindowPos
		nLayer = 0;

		for(CLayer pLayer : rhFrame.layers) {
			//if(nLayer >= rhFrame.nLayers)
			//	break;
			// Hide layer?
			if ( pLayer != null && (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
			{
				// Delete background sprites
				int nLOs = pLayer.nBkdLOs;
				for (i=0; i<nLOs; i++)
				{
					plo = rhFrame.LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex+i));

					// Delete sprite
					for(CSprite spr : plo.loSpr) {
						if(spr != null) {
							spriteGen.delSprite(spr);
						}
					}
					Arrays.fill(plo.loSpr, null);
				}
			}
			//nLayer++;
		}

		// Clear background and update objects
		if ((flags & DLF_DRAWOBJECTS) != 0)
		{
			// Redraw layer? force sprites to be cleared
			if ( (flags & DLF_REDRAWLAYER) != 0 )
			{
				// Force re-display of all the sprites
				spriteGen.activeSprite(null, CSpriteGen.AS_REDRAW, null);
			}

			// Update active sprites and clear background
			f_UpdateWindowPos(rhFrame.leX, rhFrame.leY);

			// Force re-display of all the sprites
			spriteGen.activeSprite(null, CSpriteGen.AS_REDRAW, null);
		}

		// Erase collisions bitmap
		if ( bUpdateColMask )
		{
			if(rhFrame.colMask != null)
			{
				if ( bTotalColMask ) {
					rhFrame.colMask.setOrigin(0, 0);
				}
				if ( (flags & DLF_COLMASKCLIPPED) == 0 )
					rhFrame.colMask.fill((short) 0);
				else
					rhFrame.colMask.fillRectangle(-32767, -32767, +32767, +32767, 0);
			}

			flgColMaskEmpty = true;
		}

		int nPlayfieldWidth = rhFrame.leWidth;
		int nPlayfieldHeight = rhFrame.leHeight;

		nLayer = 0;

		if ( bSkipLayer0 )
			nLayer++;

		// Display backdrop objects
		for ( ; nLayer < rhFrame.nLayers; nLayer++)
		{
			CLayer pLayer = rhFrame.layers[nLayer];

			// Update layer coordinates
			pLayer.x += pLayer.dx;
			pLayer.y += pLayer.dy;
			pLayer.dx = 0;
			pLayer.dy = 0;

			// Show layer?
			if ( (pLayer.dwOptions & CLayer.FLOPT_TOSHOW) != 0 )
				pLayer.dwOptions |= CLayer.FLOPT_VISIBLE;

			// Invisible layer? continue
			if ( (pLayer.dwOptions & CLayer.FLOPT_VISIBLE) == 0 )
			{
				if ( nLayer != 0 || bUpdateColMask == false )
					continue;
				bLayer0Invisible = true;
			}

			// Redraw layer?
			/*  if ( (flags & DLF_REDRAWLAYER) != 0 )
            {
                if ( (pLayer.dwOptions & CLayer.FLOPT_REDRAW) == 0 )
                    continue;
            }
            pLayer.dwOptions &= ~CLayer.FLOPT_REDRAW;*/

			// Get layer offset
			boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
			boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
			boolean bWrap = (bWrapHorz | bWrapVert);
			dxLayer = rhFrame.leX;
			dyLayer = rhFrame.leY;

			// Apply layer scrolling coefs
			if ( (pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0 )
			{
				if ( (pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0 )
					dxLayer = (int)(dxLayer * pLayer.xCoef);
				if ( (pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0 )
					dyLayer = (int)(dyLayer * pLayer.yCoef);
			}

			// Add layer offset
			dxLayer += pLayer.x;
			dyLayer += pLayer.y;

			// Limit dxLayer/dyLayer to playfield width/height
			if ( bWrapHorz )
				dxLayer %= nPlayfieldWidth;
			if ( bWrapVert )
				dyLayer %= nPlayfieldHeight;

			// Reset ladders
			y_Ladder_Reset(nLayer);

			// Display Bkd LOs
			int nLOs = pLayer.nBkdLOs;
			// plo = rhFrame.LOList.getLOFromIndex((short) pLayer.nFirstLOIndex);

			// Hide layer?
			if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
			{
				// Layer 0 => set invisible flag and redraw it (for collision mask)
				if ( nLayer == 0 )
					bLayer0Invisible = true;
			}

			// Display layer
			if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) == 0 || nLayer == 0 )
			{
				boolean bSaveBkd = ((pLayer.dwOptions & CLayer.FLOPT_NOSAVEBKD) == 0);

				// Show layer? show all objects
				if ( (pLayer.dwOptions & CLayer.FLOPT_TOSHOW) != 0 )
					pLayer.dwOptions &= ~CLayer.FLOPT_TOSHOW;

				// Display background objects
				int dwWrapFlags = 0;
				int nSprite = 0;

				for (i=0; i<nLOs; i++)
				{
					plo = rhFrame.LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex+i));
					int typeObj = plo.loType;
					boolean bOut = true;

					int nCurSprite = nSprite;

					do
					{
						COI poi = null;
						COC poc = null;
						CObjectCommon pOCommon;

						// Get object position
						if ( typeObj < COI.OBJ_SPR )
						{
							rc.left = plo.loX - dxLayer;
							rc.top = plo.loY - dyLayer;
						}
						else
						{
							// Dynamic item => must be a background object
							poi = rhApp.OIList.getOIFromHandle(plo.loOiHandle);
							//Log.Log("Name "+poi.oiName);

							if ( poi==null || poi.oiOC==null )
							{ 
								dwWrapFlags = 0; 
								nSprite = 0; 
								break;
							}
							poc = poi.oiOC;
							pOCommon = (CObjectCommon) poc;
							//Log.Log("Flags "+pOCommon.ocOEFlags);
							if ((pOCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0 
									|| (hoPtr = find_HeaderObject(plo.loHandle)) == null)
							{ 
								dwWrapFlags = 0;
								nSprite = 0;
								break;
							}
							rc.left = hoPtr.hoX - rhFrame.leX - hoPtr.hoImgXSpot;
							rc.top = hoPtr.hoY - rhFrame.leY - hoPtr.hoImgYSpot;
						}

						// On the right of the display? next object (only if no total colmask)
						if ( !bTotalColMask && !bWrap && (rc.left >= x2edit + COLMASK_XMARGIN + 32 || rc.top >= y2edit + COLMASK_YMARGIN) )
						{
							dwWrapFlags = 0;
							nSprite = 0;
							break;
						}

						// Get object rectangle & new flags
						if ( typeObj < COI.OBJ_SPR )
						{
							poi = rhApp.OIList.getOIFromHandle(plo.loOiHandle);
							if (poi == null || poi.oiOC == null)
							{ dwWrapFlags = 0; nSprite = 0; break; }
							poc = poi.oiOC;
							rc.right = rc.left + poc.ocCx;
							rc.bottom = rc.top + poc.ocCy;
							v = poc.ocObstacleType;
							cm_box = poc.ocColMode;
							// Set Anti-alias for backdrop
							//poc.ocAntialias = poi.oiAntialias;
						}
						else
						{
							rc.right = rc.left + hoPtr.hoImgWidth;
							rc.bottom = rc.top + hoPtr.hoImgHeight;
							pOCommon = (CObjectCommon)poi.oiOC;
							v = ((pOCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT);
							cm_box = ((pOCommon.ocFlags2 & CObjectCommon.OCFLAGS2_COLBOX) != 0) ? 1 : 0;
						}

						// Wrap
						if ( bWrap )
						{
							switch ( nSprite ) {

							// Normal sprite: test if other sprites  displayed
							case 0:
								// Wrap horizontally?
								if ( bWrapHorz && (rc.left < 0 || rc.right > nPlayfieldWidth) )
								{
									// Wrap horizontally and vertically?
									if ( bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight) )
									{
										nSprite = 3;
										dwWrapFlags |= (WRAP_X | WRAP_Y | WRAP_XY);
									}

									// Wrap horizontally only
									else
									{
										nSprite = 1;
										dwWrapFlags |= (WRAP_X);
									}
								}

								// Wrap vertically?
								else if ( bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight) )
								{
									nSprite = 2;
									dwWrapFlags |= (WRAP_Y);
								}

								// Delete other sprites
								if ( (dwWrapFlags & WRAP_X) == 0 && plo.loSpr[1] != null )
								{
									spriteGen.delSprite(plo.loSpr[1]);
									plo.loSpr[1] = null;
								}
								if ( (dwWrapFlags & WRAP_Y) == 0 && plo.loSpr[2] != null )
								{
									spriteGen.delSprite(plo.loSpr[2]);
									plo.loSpr[2] = null;
								}
								if ( (dwWrapFlags & WRAP_XY) == 0 && plo.loSpr[3] != null )
								{
									spriteGen.delSprite(plo.loSpr[3]);
									plo.loSpr[3] = null;
								}
								break;

								// Other sprite instance: wrap horizontally
							case 1:

								// Wrap
								if ( rc.left < 0 )				// (rc.right + curFrame.m_leX) <= 0
								{
									int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
									rc.left += dx;
									rc.right += dx;
								}
								else if ( rc.right > nPlayfieldWidth )	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
								{
									int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
									rc.left -= dx;
									rc.right -= dx;
								}

								// Remove flag
								dwWrapFlags &= ~WRAP_X;

								// Calculate next sprite to display
								nSprite = 0;
								if ( (dwWrapFlags & WRAP_Y) != 0 )
									nSprite = 2;
								break;

								// Other sprite instance: wrap vertically
							case 2:

								// Wrap
								if ( rc.top < 0 )				// (rc.bottom + curFrame.m_leY) <= 0
								{
									int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
									rc.top += dy;
									rc.bottom += dy;
								}
								else if ( rc.bottom > nPlayfieldHeight )		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
								{
									int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
									rc.top -= dy;
									rc.bottom -= dy;
								}

								// Remove flag
								dwWrapFlags &= ~WRAP_Y;

								// Calculate next sprite to display
								nSprite = 0;
								if ( (dwWrapFlags & WRAP_X) != 0 )
									nSprite = 1;
								break;

								// Other sprite instance: wrap horizontally and vertically
							case 3:

								// Wrap
								if ( rc.left < 0 )				// (rc.right + curFrame.m_leX) <= 0
								{
									int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
									rc.left += dx;
									rc.right += dx;
								}
								else if ( rc.right > nPlayfieldWidth )	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
								{
									int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
									rc.left -= dx;
									rc.right -= dx;
								}
								if ( rc.top < 0 )				// (rc.bottom + curFrame.m_leY) <= 0
								{
									int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
									rc.top += dy;
									rc.bottom += dy;
								}
								else if ( rc.bottom > nPlayfieldHeight )		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
								{
									int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
									rc.top -= dy;
									rc.bottom -= dy;
								}

								// Remove flag
								dwWrapFlags &= ~WRAP_XY;

								// Calculate next sprite to display
								nSprite = 2;
								break;
							}
						}
						//Log.Log("after Wrap l:"+rc.left+" t:"+rc.top+" r:"+rc.right+" b:"+rc.bottom);
						// Ladder?
						if ( v == OBSTACLE_LADDER )
						{
							y_Ladder_Add(nLayer, rc.left, rc.top, rc.right, rc.bottom);
							cm_box = 1;		// Fill rectangle in collision masque
						}

						// Obstacle in layer 0?
						if ( nLayer == 0 && bUpdateColMask && v != OBSTACLE_TRANSPARENT &&
								(bTotalColMask || (rc.right >= -COLMASK_XMARGIN-32 && rc.bottom >= -COLMASK_YMARGIN))
								)
						{
							CMask pMask = null;

							// Retablir coords absolues si TOTALCOLMASK
							if ( bTotalColMask )
							{
								//OffsetRect (&rc, dxLayer, dyLayer);	// pCurFrame.m_leX, pCurFrame.m_leY);
								rc.left += dxLayer;
								rc.top += dyLayer;
								rc.right += dxLayer;
								rc.bottom += dyLayer;
							}

							// Update collisions bitmap (meme si objet en dehors)
							obst = 0;
							if ( v == OBSTACLE_SOLID )
							{
								obst = CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM;
								flgColMaskEmpty = false;
							}

							pMask = null;
							// Ajouter le masque "obstacle"
							if ( flgColMaskEmpty == false )
							{
								if ( cm_box!=0 )
								{
									if(rhFrame.colMask != null)
										rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right-1, rc.bottom-1, obst);
								}
								else
								{
									if ( pMask == null )
									{
										if ( typeObj < COI.OBJ_SPR )
										{
											img = ((COCBackground) poc).ocImage;
											CImage image = rhApp.imageBank.getImageFromHandle(img);
											if(image != null)
												pMask = image.getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
										}
										else
										{
											pMask = hoPtr.getCollisionMask(CMask.GCMF_OBSTACLE);
										}
									}
									if(rhFrame.colMask != null)
									{
										if ( pMask == null )
											rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right-1, rc.bottom-1, obst);
										else
											rhFrame.colMask.orMask(pMask, rc.left, rc.top, CColMask.CM_OBSTACLE|CColMask.CM_PLATFORM, obst);
									}
								}
							}

							// Ajouter le masque plateforme
							if ( v == OBSTACLE_PLATFORM )
							{
								flgColMaskEmpty = false;
								if ( cm_box!=0 )
								{
									if(rhFrame.colMask != null)
										rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right-1, (Math.min(rc.top+CColMask.HEIGHT_PLATFORM,rc.bottom)-1), CColMask.CM_PLATFORM);
								}
								else
								{
									if ( pMask == null )
									{
										if ( typeObj < COI.OBJ_SPR )
										{
											img = ((COCBackground) poc).ocImage;
											CImage image = rhApp.imageBank.getImageFromHandle(img);
											if(image != null)
												pMask = image.getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
										}
										else
										{
											pMask = hoPtr.getCollisionMask(CMask.GCMF_OBSTACLE);
										}
									}
									if(rhFrame.colMask != null)
									{
										if ( pMask == null )
											rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right-1, (Math.min(rc.top+CColMask.HEIGHT_PLATFORM,rc.bottom)-1), CColMask.CM_PLATFORM);
										else
											rhFrame.colMask.orPlatformMask(pMask, rc.left, rc.top);

									}
								}
							}

							// Retablir coords absolues si TOTALCOLMASK
							if ( bTotalColMask )
							{
								//OffsetRect (&rc, -dxLayer, -dyLayer);	// -pCurFrame.m_leX, -pCurFrame.m_leY);
								rc.left -= dxLayer;
								rc.top -= dyLayer;
								rc.right -= dxLayer;
								rc.bottom -= dyLayer;
							}
						}

						// Display object?
						//Log.Log("redrawlevel Left "+rc.left+" top "+rc.top+" x2edit "+x2edit+" y2edit "+y2edit);
						//Log.Log("Layer:" +pLayer.pName+ " sprite index:"+nSprite);
						if ( rc.left <= x2edit && rc.top <= y2edit && rc.right >= 0 && rc.bottom >= 0 )
						{
							// In "displayable" area
							bOut = false;

							////////////////////////////////////////
							// Non-background layer => create sprite
							if ( nLayer > 0 || !bLayer0Invisible)
							{
								int dwFlags = CSprite.SF_BACKGROUND | CSprite.SF_NOHOTSPOT | CSprite.SF_INACTIF;
								if ( !bSaveBkd )
									dwFlags |= CSprite.SF_NOSAVE;
								switch (v)
								{
								case OBSTACLE_SOLID:
									dwFlags |= (CSprite.SF_OBSTACLE | CSprite.SF_RAMBO);
									break;
								case OBSTACLE_PLATFORM:
									dwFlags |= (CSprite.SF_PLATFORM | CSprite.SF_RAMBO);
									break;
								}
								
								// Create sprite only if not already created
								if ( plo.loSpr[nSprite] == null )
								{
									switch ( typeObj ) {

									// QuickBackdrop: ownerdraw sprite
									case COI.OBJ_BOX:
										plo.loSpr[nSprite] = spriteGen.addOwnerDrawSprite(rc.left, rc.top, rc.right, rc.bottom, plo.loLayer, i*4+nCurSprite, 0, dwFlags | CSprite.SF_COLBOX, null, poc);
										//ppSpr = AddOwnerDrawSprite(idEditWin, rc.left, rc.top, rc.right, rc.bottom,
										//							plo.loLayer, i4+nCurSprite, NULL, (dwFlags | SF_COLBOX), (int)plo, (int)DrawOwnerDrawQuickBkd);
										break;

										// Backdrop: sprite
									case COI.OBJ_BKD:
										plo.loSpr[nSprite] = spriteGen.addSprite(rc.left, rc.top, ((COCBackground)poc).ocImage, plo.loLayer, i*4+nCurSprite, 0, dwFlags, hoPtr);
										spriteGen.modifSpriteEffect(plo.loSpr[nSprite], poi.oiInkEffect, poi.oiInkEffectParam);
										break;

										// Extension
									default:
										if ( hoPtr != null )
										{
											plo.loSpr[nSprite] = spriteGen.addOwnerDrawSprite(rc.left, rc.top, rc.right, rc.bottom, plo.loLayer, i*4+nCurSprite, 0, (dwFlags | CSprite.SF_COLBOX), hoPtr, hoPtr);
											//ppSpr = AddOwnerDrawSprite(idEditWin, rc.left, rc.top, rc.right, rc.bottom, plo.loLayer, i4+nCurSprite, NULL, (dwFlags | SF_COLBOX), (int)plo, (int)DrawOwnerDrawBackgroundExt);
										}
										break;
									}
								}

								// Otherwise update sprite coordinates
								else
								{
									switch ( typeObj )
									{
									// QuickBackdrop: ownerdraw sprite
									case COI.OBJ_BOX:
									{
										CRect rcSpr = plo.loSpr[nSprite].getSpriteRect();

										//GetSpriteRectNew (idEditWin, ppSpr, &rcSpr);
										if ( rc.left != rcSpr.left || rc.top != rcSpr.top || rc.right != rcSpr.right || rc.bottom != rcSpr.bottom )
											spriteGen.modifOwnerDrawSprite(plo.loSpr[nSprite], rc.left, rc.top, rc.right, rc.bottom);
										//ModifOwnerDrawSprite(idEditWin, ppSpr, rc.left, rc.top, rc.right, rc.bottom);
									}
									break;

									// Backdrop: sprite
									case COI.OBJ_BKD:
										spriteGen.modifSprite(plo.loSpr[nSprite], rc.left, rc.top, ((COCBackground)poc).ocImage);
										//ModifSprite(idEditWin, ppSpr, rc.left, rc.top, ((LPBackdrop_OC)poc).ocImage);
										break;

										// Extension: TODO: force the object to be re-displayed?
									default:
										if ( hoPtr != null )
											spriteGen.modifOwnerDrawSprite(plo.loSpr[nSprite], rc.left, rc.top, rc.right, rc.bottom);
										//ModifOwnerDrawSprite(idEditWin, ppSpr, rc.left, rc.top, rc.right, rc.bottom);
										break;
									}
								}
							}
						}
					} while (false);

					// Object out of visible area: delete sprite
					if ( bOut )
					{
						// Delete sprite
						if ( plo.loSpr[nSprite] != null && typeObj < COI.KPX_BASE )
						{
							spriteGen.delSprite(plo.loSpr[nSprite]);
							plo.loSpr[nSprite] = null;
						}
					}

					// Re-display the same object but wrapped
					if ( dwWrapFlags != 0 )
					{
						i--;
						// plo = rhFrame.LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex+i));
					}
				}
			}

			// Display backdrop objects created at your application
			// Unflag DLF_DONTUPDATECOLMASK so method can handle collision mask accordingly
			if ( pLayer.pBkd2 != null )
				displayBkd2Layer(pLayer, nLayer, flags&~DLF_DONTUPDATECOLMASK, x2edit, y2edit, flgColMaskEmpty);
			//DisplayBkd2Layer(pLayer, nLayer, flags, x2edit, y2edit, flgColMaskEmpty);

			// Hide layer?
			if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
				pLayer.dwOptions &= ~(CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE);
		}

		// Update colmask offset coordinates if total collision mask
		if ( bTotalColMask )
		{
			CLayer pLayer = rhFrame.layers[0];

			dxLayer = rhFrame.leX;
			dyLayer = rhFrame.leY;

			if ( (pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0 )
			{
				if ( (pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0 )
					dxLayer = (int)(dxLayer * pLayer.xCoef);
				if ( (pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0 )
					dyLayer = (int)(dyLayer * pLayer.yCoef);
			}

			// Add layer offset
			dxLayer += pLayer.x;
			dyLayer += pLayer.y;

			if(rhFrame.colMask != null) {
				rhFrame.colMask.setOrigin(dxLayer, dyLayer);
				//Log.Log("dxLayer: "+dxLayer+" dylayer: "+dyLayer+" pLayerX: "+pLayer.x+" pLayerY: "+pLayer.y);
			}
		}

	}
	
	public void scrollLayers() {
		scrollLevel();
	}

	// Scroll frame
	/** Scrolls the level.
	 */
	void scrollLevel()
	{
		int xSrc, ySrc, xDest, yDest;
		boolean flgScroll, flgScrollMask;
		int lg, ht, lgLog, htLog;

		// Calcul rectangles de scrolling
		lgLog = rhFrame.leEditWinWidth;
		htLog = rhFrame.leEditWinHeight;

		float xCoef = 1.0f;
		float yCoef = 1.0f;

		if (rhFrame.nLayers > 0)
		{
			CLayer pLayer0 = rhFrame.layers[0];
			xCoef = pLayer0.xCoef;
			yCoef = pLayer0.yCoef;
		}

		int oX = rhFrame.leLastScrlX;
		int nX = rh3DisplayX;

		if (xCoef != 1.0f)
		{
			oX = (int) (oX * xCoef);
			nX = (int) (nX * xCoef);
		}

		if (nX < oX)
		{
			xSrc = 0;
			xDest = oX - nX;
			rhFrame.leLastScrlX = rh3DisplayX;
		}
		else
		{
			xSrc = nX - oX;
			xDest = 0;
			if (xSrc != 0)
			{
				rhFrame.leLastScrlX = rh3DisplayX;
			}
		}

		int oY = rhFrame.leLastScrlY;
		int nY = rh3DisplayY;

		if (yCoef != 1.0f)
		{
			oY = (int) (oY * yCoef);
			nY = (int) (nY * yCoef);
		}

		if (nY < oY)
		{
			ySrc = 0;
			yDest = oY - nY;

			rhFrame.leLastScrlY = rh3DisplayY;
		}
		else
		{
			ySrc = nY - oY;
			yDest = 0;

			if (ySrc != 0)
			{
				rhFrame.leLastScrlY = rh3DisplayY;
			}
		}

		lg = lgLog - xSrc - xDest;
		ht = htLog - ySrc - yDest;

		//Log.Log(" lg "+lg+" lgLog "+lgLog+" xSrc "+xSrc+" xDest "+xDest+" ht "+ht+" htLog "+htLog+" ySrc "+ySrc+" yDest "+yDest);

		// Update coordinates
		rhFrame.leX = rh3DisplayX;
		rhFrame.leY = rh3DisplayY;

		// Clear sprites
		spriteGen.activeSprite(null, CSpriteGen.AS_REDRAW, null);		// AS_DEACTIVATE

		// Hide or show layers? => hide or show objects
		for (int nLayer = 0; nLayer < rhFrame.nLayers; nLayer++)
		{
			CLayer pLayer = rhFrame.layers[nLayer];
			if ((pLayer.dwOptions & CLayer.FLOPT_TOSHOW) != 0)
			{
				f_ShowAllObjects(nLayer, true);
			}
			if ((pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0)
			{
				f_ShowAllObjects(nLayer, false);
			}
		}

		f_UpdateWindowPos(rhFrame.leX, rhFrame.leY);

		// Scrolling
		flgScroll = flgScrollMask = false;

		if (lg > lgLog / 4 && ht > htLog / 4)
		{
			// This never happens, because always reach here with a difference between values
			if (lg == lgLog && ht == htLog)
			{
				flgScroll = true;
				flgScrollMask = true;
			}
			else
			{
				// Scroll ecran logique
				if (lg > 0 && ht > 0)
					flgScroll = true;

			}
		}

		// If no Scrolling repost All
		if (flgScroll == false)
		{
			redrawLevel(DLF_DONTUPDATE);
		}

		// Si scrolling effectue, refaire un redraw clippe
		else
		{
			boolean bRedrawDone = false;

			if (xSrc != 0 || xDest != 0)
			{
				if (flgScrollMask != false)
				{
					// ColMask_SetClip (idEditWin, &rcClipH);
					redrawLevel(DLF_DONTUPDATE | DLF_COLMASKCLIPPED);
				}
				else
				{
					redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK);
				}

				bRedrawDone = true;
			}

			if (ySrc != 0 || yDest != 0)
			{
				if (flgScrollMask != false)
				{
					// ColMask_SetClip (idEditWin, &rcClipV);
					redrawLevel(DLF_DONTUPDATE | DLF_COLMASKCLIPPED);
				}
				else
				{
					redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK);
				}

				bRedrawDone = true;
			}

			// Redraw not done? redraw layers greater than 0
			if (!bRedrawDone && rhFrame.nLayers > 0)
			{
				if ((rhFrame.layers[0].dwOptions & (CLayer.FLOPT_REDRAW)) != 0)
				{
					redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK);
				}
				else
				{
					redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK | DLF_SKIPLAYER0);
				}
			}           
		}
	}

	/** Update of level coordinates in case of a scrolling but when everything has to be redrawn   
	 */
	public void updateScrollLevelPos()
	{
		int xSrc, ySrc;

		float xCoef = (float) 1.0;
		float yCoef = (float) 1.0;

		if (rhFrame.nLayers > 0)
		{
			CLayer pLayer0 = rhFrame.layers[0];
			xCoef = pLayer0.xCoef;
			yCoef = pLayer0.yCoef;
		}

		int oX = rhFrame.leLastScrlX;
		int nX = rh3DisplayX;

		if (xCoef != 1.0)
		{
			oX = (int) (oX * xCoef);
			nX = (int) (nX * xCoef);
		}

		if (nX < oX)
		{
			xSrc = 0;
			//	    xDest = oX - nX;
			rhFrame.leLastScrlX = rh3DisplayX;
		}
		else
		{
			xSrc = nX - oX;
			//	    xDest = 0;
			if (xSrc != 0)
			{
				rhFrame.leLastScrlX = rh3DisplayX;
			}
		}

		int oY = rhFrame.leLastScrlY;
		int nY = rh3DisplayY;

		if (yCoef != 1.0)
		{
			oY = (int) (oY * yCoef);
			nY = (int) (nY * yCoef);
		}

		if (nY < oY)
		{
			ySrc = 0;
			//	    yDest = oY - nY;

			rhFrame.leLastScrlY = rh3DisplayY;
		}
		else
		{
			ySrc = nY - oY;
			//	    yDest = 0;

			if (ySrc != 0)
			{
				rhFrame.leLastScrlY = rh3DisplayY;
			}
		}

		// Update coordinates
		rhFrame.leX = rh3DisplayX;
		rhFrame.leY = rh3DisplayY;

		//Log.Log("ScrollLevelPos "+" X: "+ rh3DisplayX+" Y: "+rh3DisplayY);

		rhApp.updateWindowPos();
	}

	public void transitionDrawFrame()
	{
		rh3Scrolling |= RH3SCROLLING_REDRAWLAYERS;
		screen_Update();
	}
	
	// ---------------------------------------------------------------------------
	// SCREEN UPDATE: actualise ecrans ze sprites
	// ---------------------------------------------------------------------------
	/** Actualisation of the screen and sprites.
	 */
	public void screen_Update()
	{
		if(GLRenderer.inst == null || !SurfaceView.hasSurface)
			return;
		
		if (rhApp.parentApp == null)
			GLRenderer.inst.clear (rhApp.gaBorderColour);

		int background;
		if(rhApp.frame != null)
			background = rhApp.frame.leBackground;
		else
			background = rhApp.gaBorderColour;

		synchronized(GLRenderer.inst)
		{
			GLRenderer.inst.fillZone (0, 0, rhApp.gaCxWin, rhApp.gaCyWin, background, 
					GLRenderer.BOP_MASK, (rhApp.frame.leFlags & CRunFrame.LEF_NOSURFACE )!=0 ? 128 : 0);
		}
		if (rh3Scrolling!=0)
		{
			if ((rh3Scrolling&RH3SCROLLING_REDRAWALL)!=0)
			{
				// Update scroll pos if scrolling
				if (rhFrame.leX != rh3DisplayX || rhFrame.leY != rh3DisplayY)
					updateScrollLevelPos();

				// Redraw everything
				int flags = DLF_DRAWOBJECTS | DLF_REDRAWLAYER;
				if ((rh3Scrolling & RH3SCROLLING_REDRAWTOTALCOLMASK) == 0 && (rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0)
					flags |= DLF_DONTUPDATECOLMASK;

				//Log.Log("Redraw Everything");
				redrawLevel(flags);
 
				rh3DisplayX=rhWindowX;
				rh3DisplayY=rhWindowY;
			}
			else if ((rh3Scrolling&RH3SCROLLING_SCROLL)!=0)
			{
				// Update scroll pos if scrolling
				if (rhFrame.leX != rh3DisplayX || rhFrame.leY != rh3DisplayY)
				{
					//Log.Log(" Screen Update Scrolling");
					scrollLevel();
				}
			}
			else
			{
				//Log.Log("Redrawing the level");
				redrawLevel(DLF_DONTUPDATE|DLF_DRAWOBJECTS|DLF_REDRAWLAYER);
			}
		}

		spriteGen.spriteClear();
		spriteGen.spriteUpdate();
		spriteGen.spriteDraw();

		rh3Scrolling=0;

		if (rhApp.parentApp == null)
		{
			synchronized(GLRenderer.inst)
			{
				GLRenderer.inst.drawJoystick ();
			}
			
			//if(rh4EventCount == 0)
			//	SurfaceView.inst.firstWait();
			
			if(!SurfaceView.inst.swapBuffers()) {
				int error = SurfaceView.inst.eglError; 
	            switch (error) {
	            case EGL10.EGL_SUCCESS:
	                break;
	            case EGL11.EGL_CONTEXT_LOST:
					if(MMFRuntime.inst.app != null) {
						MMFRuntime.inst.mainView.removeView (SurfaceView.inst);
						SurfaceView.inst = null;
						MMFRuntime.inst.app.setSurfaceEnabled (true);
					}
					break;
	            }

			}
 		}
	}

	// Modification d'un objet background
	// -----------------------------------
	public void modif_RedrawLevel(CExtension hoPtr)
	{
		// Test if we should redraw the collision mask
		boolean bRedrawTotalColMask = false;
		if (hoPtr != null)
		{
			int dwColMaskFlags = hoPtr.hoCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK;

			// Layer 1 => redraw collision mask if no ladder
			// Layer > 1 => redraw collision mask if obstacle or platform
			if (hoPtr.getCollisionMask(-1) != null)
			{
				if ((hoPtr.hoLayer == 0 && dwColMaskFlags != CObjectCommon.OCFLAGS2_OBSTACLE_LADDER) || (hoPtr.hoLayer > 0 && (dwColMaskFlags == CObjectCommon.OCFLAGS2_OBSTACLE_SOLID || dwColMaskFlags == CObjectCommon.OCFLAGS2_OBSTACLE_PLATFORM)))
				{
					bRedrawTotalColMask = true;
				}
			}
		}
		ohRedrawLevel(bRedrawTotalColMask);
	}

	// -----------------------------------------------------------------------
	// Scale une coordonn���e en fonction du strech de l'ecran
	// -----------------------------------------------------------------------

	public int scaleX(int x)
	{
		return x;
	}

	public int scaleY(int y)
	{
		return y;
	}

	// -----------------------------------------------------------------------
	// TROUVE LE HO
	// -----------------------------------------------------------------------
	public CObject find_HeaderObject(short hlo)
	{
		int count = rhNObjects;
		CObject[] localObjectList = rhObjectList;
		for(CObject ob : localObjectList) {
			if (ob == null)
				continue;
			--count;
			if(hlo == ob.hoHFII)
				return ob;
			if(count == 0)
				break;
		}
		return null;
	}

	// -----------------------------------------------------------------------
	// SCROLLING DU TERRAIN, UPDATE LES POSITIONS
	// -----------------------------------------------------------------------
	/** Update of the object's positions in case of scrolling.
	 */
	public void f_UpdateWindowPos(int newX, int newY)
	{
		// Change le pointeurs dans le segment de base
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		// Deltas pour le C
		short noMove = 0;
		rh4WindowDeltaX = newX - rhWindowX;
		if (rh4WindowDeltaX != 0)
		{
			noMove++;
		}
		rh4WindowDeltaY = newY - rhWindowY;
		if (rh4WindowDeltaY != 0)
		{
			noMove++;
		}

		// Scan layers and check if dx/dy != 0
		if (noMove == 0)
		{
			for (int i = 0; i < rhFrame.nLayers; i++)
			{
				CLayer pLayer = rhFrame.layers[i];
				if (pLayer.dx != 0 || pLayer.dy != 0)
				{
					noMove++;
					break;
				}
			}
		}

		// Store variables for faster access in object loop
		int nOldX = rhWindowX;
		int nOldY = rhWindowY;
		int nNewX = newX;
		int nNewY = newY;
		int nDeltaX = rh4WindowDeltaX;
		int nDeltaY = rh4WindowDeltaY;

		// Coordonnees limites de gestion
		rhWindowX = newX;									// Minimum gestion droite
		rh3XMinimum = newX - COLMASK_XMARGIN;
		if (rh3XMinimum < 0)
		{
			rh3XMinimum = rh3XMinimumKill;
		}

		rhWindowY = newY;									// Minimum gestion haut
		rh3YMinimum = newY - COLMASK_YMARGIN;
		if (rh3YMinimum < 0)
		{
			rh3YMinimum = rh3YMinimumKill;
		}

		rh3XMaximum = newX + rh3WindowSx + COLMASK_XMARGIN;	// Maximum gestion droite
		if (rh3XMaximum > rhLevelSx)
		{
			rh3XMaximum = rh3XMaximumKill;
		}

		rh3YMaximum = newY + rh3WindowSy + COLMASK_YMARGIN;	// Maximum gestion bas
		if (rh3YMaximum > rhLevelSy)
		{
			rh3YMaximum = rh3YMaximumKill;
		}

		// Reaffiche tous les objets du terrain en changeant eventuellement leurs coordonnees
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		// rh4FirstQuickDisplay = (short) 0x8000;
		// rh4LastQuickDisplay = (short) 0x8000;

		int count = rhNObjects;
		CObject[] localObjectList = rhObjectList;
		for(CObject pHo : localObjectList) {
			if(pHo != null) {
				/*  if (pHo.hoNextQuickDisplay != -2)
	            {
	                pHo.hoNextQuickDisplay = -1;
	            } */
				--count;
				if (noMove != 0)
				{
					if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_SCROLLINGINDEPENDANT) != 0)
					{
						//Log.Log(" Object independant: "+pHo.hoOiList.oilName);
						int x = nDeltaX;
						int y = nDeltaY;

						if (pHo.rom == null)
						{
							pHo.hoX += x;
							pHo.hoY += y;
						}
						else
						{
							x += pHo.hoX;
							y += pHo.hoY;
							pHo.rom.rmMovement.setXPosition(x);
							pHo.rom.rmMovement.setYPosition(y);
						}
					}
					else
					{
						//Log.Log(" Object with non independant: "+pHo.hoOiList.oilName);
						int nLayer = pHo.hoLayer;
						if (nLayer < rhFrame.nLayers)
						{
							int oldLayerDx = nOldX;
							int oldLayerDy = nOldY;
							int newLayerDx = nNewX;
							int newLayerDy = nNewY;

							CLayer pLayer = rhFrame.layers[nLayer];
							if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
							{
								oldLayerDx = (int) (pLayer.xCoef * oldLayerDx);
								newLayerDx = (int) (pLayer.xCoef * newLayerDx);
							}
							if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
							{
								oldLayerDy = (int) (pLayer.yCoef * oldLayerDy);
								newLayerDy = (int) (pLayer.yCoef * newLayerDy);
							}

							// Equivalent
							int nX = (pHo.hoX + oldLayerDx) - newLayerDx + nDeltaX - pLayer.dx;
							int nY = (pHo.hoY + oldLayerDy) - newLayerDy + nDeltaY - pLayer.dy;

							if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) == 0)
							{
								pHo.hoX = nX;
								pHo.hoY = nY;
							}
							else
							{
								pHo.rom.rmMovement.setXPosition(nX);
								pHo.rom.rmMovement.setYPosition(nY);
							}
						}
					}
					if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0)					// protection ajoute build 96: crash si Q/A object + scrolling
					{
						pHo.modif();
					}
				}
				else if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0)
				{
					//Log.Log(" Object with background: "+pHo.hoOiList.oilName);
					pHo.display();
				}
				if(count  == 0)
					break;
			}
		}
		// Efface les sprites et les objets decor
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		if(spriteGen != null)
			spriteGen.spriteClear();
		//erase_QuickDisplay(renderer);
	}

	/** End of scrolling update.
	 */
	public void f_UpdateWindowPosEnd()
	{
		spriteGen.spriteUpdate();
		spriteGen.spriteDraw();
	}

	/** Shows all the objects.
	 */
	public void f_ShowAllObjects(int nLayer, boolean bShow)
	{
		int nObject=rhNObjects;
		CObject[] localObjectList = rhObjectList;
		for(CObject hoPtr : localObjectList) {
			if(hoPtr != null) {
				--nObject;
				if (nLayer == hoPtr.hoLayer || nLayer == CSpriteGen.LAYER_ALL)
				{
					if (hoPtr.ros != null)
					{
						if (hoPtr.roc.rcSprite != null)
						{
							spriteGen.activeSprite(hoPtr.roc.rcSprite, CSpriteGen.AS_ACTIVATE, null);
						}

						if (bShow)
						{
							if ((hoPtr.ros.rsFlags & CRSpr.RSFLAG_VISIBLE) != 0)
							{
								CLayer pLayer = rhFrame.layers[hoPtr.hoLayer];
								int dwOpt = pLayer.dwOptions;
								pLayer.dwOptions = ((pLayer.dwOptions & ~(CLayer.FLOPT_TOSHOW | CLayer.FLOPT_TOHIDE)) | CLayer.FLOPT_VISIBLE);

								hoPtr.ros.obShow();

								pLayer.dwOptions = dwOpt;
							}
						}
						else
						{
							hoPtr.ros.obHide();
						}
						hoPtr.ros.rsFlash = 0;
					}
				}
				if(nObject == 0)
					break;
			}
		}
	}

	// Centre le display en respectant les bords
	// -----------------------------------------
	/** Centers the display, respecting the borders.
	 */
	public void setDisplay(int x, int y, int nLayer, int flags)
	{
		x -= rh3WindowSx / 2;				//; Taille de la fenetre d'affichage
		y -= rh3WindowSy / 2;


		float xf = x;
		float yf = y;

		if (nLayer != -1 && nLayer < rhFrame.nLayers)
		{
			CLayer pLayer = rhFrame.layers[nLayer];
			if (pLayer.xCoef > 1.0f)
			{
				float dxf = (xf - rhWindowX);
				dxf /= pLayer.xCoef;
				xf = rhWindowX + dxf;
			}
			if (pLayer.yCoef > 1.0f)
			{
				float dyf = (yf - rhWindowY);
				dyf /= pLayer.yCoef;
				yf = rhWindowY + dyf;
			}
		}

		x = (int) xf;
		y = (int) yf;

		// En mode jeu, on limite aux bordures du terrain...
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (x < 0)
		{
			x = 0;					// Sort ��� haut/gauche?
		}
		if (y < 0)
		{
			y = 0;					// Sort ��� haut/gauche?
		}

		int x2 = x + rh3WindowSx;	// Sort a droite/bas?
		int y2 = y + rh3WindowSy;
		if (x2 > rhLevelSx)
		{
			x2 = rhLevelSx - rh3WindowSx;
			if (x2 < 0)
			{
				x2 = 0;
			}
			x = x2;
		}
		if (y2 > rhLevelSy)
		{
			y2 = rhLevelSy - rh3WindowSy;
			if (y2 < 0)
			{
				y2 = 0;
			}
			y = y2;
		}

		// Pour la fin de la boucle...
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if ((flags & 1) != 0)
		{
			if (x != rhWindowX)
			{
				rh3DisplayX = x;
				rh3Scrolling |= RH3SCROLLING_SCROLL;
			}
		}
		if ((flags & 2) != 0)
		{
			if (y != rhWindowY)
			{
				rh3DisplayY = y;
				rh3Scrolling |= RH3SCROLLING_SCROLL;
			}
		}
		if((rh3Scrolling & RH3SCROLLING_SCROLL) != 0) {
			++rhApp.nWindowUpdate;
		}
	}
	// Force un redessin du fond
	// -------------------------
	public void ohRedrawLevel(boolean bRedrawTotalColMask)
	{
		rh3Scrolling |= RH3SCROLLING_REDRAWALL;
		if (bRedrawTotalColMask)
		{
			rh3Scrolling |= RH3SCROLLING_REDRAWTOTALCOLMASK;
		}
	}

	// -------------------------------------------------------------------------
	// GESTION DES ECHELLES
	// -------------------------------------------------------------------------

	// Remove ladders
	public void y_Ladder_Reset(int nLayer)
	{
		if (nLayer >= 0 && nLayer < rhFrame.nLayers)
		{
			CLayer pLayer = rhFrame.layers[nLayer];
			pLayer.pLadders = null;
		}
	}

	// Add Ladder
	public void y_Ladder_Add(int nLayer, int x1, int y1, int x2, int y2)
	{
		if (nLayer >= 0 && nLayer < rhFrame.nLayers)
		{
			CLayer pLayer = rhFrame.layers[nLayer];

			CRect rc = new CRect();
			rc.left = Math.min(x1, x2);
			rc.top = Math.min(y1, y2);
			rc.right = Math.max(x1, x2);
			rc.bottom = Math.max(y1, y2);

			if (pLayer.pLadders == null)
			{
				pLayer.pLadders = new ArrayList<CRect>();
			}
			pLayer.pLadders.add(rc);
		}
	}

	// Remove ladder
	public void y_Ladder_Sub(int nLayer, int x1, int y1, int x2, int y2)
	{
		if (nLayer >= 0 && nLayer < rhFrame.nLayers)
		{
			CLayer pLayer = rhFrame.layers[nLayer];
			if (pLayer.pLadders != null)
			{
				CRect rc = new CRect();
				rc.left = Math.min(x1, x2);
				rc.top = Math.min(y1, y2);
				rc.right = Math.max(x1, x2);
				rc.bottom = Math.max(y1, y2);

				int i;
				CRect rcDst;
				int pLayer_pLadders_size = pLayer.pLadders.size();
				for (i = 0; i < pLayer_pLadders_size; i++)
				{
					rcDst = pLayer.pLadders.get(i);
					if (rcDst.intersectRect(rc) == true)
					{
						pLayer.pLadders.remove(i);
						pLayer_pLadders_size = pLayer.pLadders.size();
						i--;
					}
				}
			}
		}
	}

	public CRect y_GetLadderAt(int nLayer, int x, int y)
	{
		int nl, nLayers;

		if (nLayer == -1)
		{
			nl = 0;
			nLayers = rhFrame.nLayers;
		}
		else
		{
			nl = nLayer;
			nLayers = (nLayer + 1);
		}

		for (; nl < nLayers; nl++)
		{
			CLayer pLayer = rhFrame.layers[nl];

			if (pLayer.pLadders != null)
			{
				int i;
				CRect rc;
				int pLayer_pLadders_size = pLayer.pLadders.size();
				for (i = 0; i < pLayer_pLadders_size; i++)
				{
					rc = pLayer.pLadders.get(i);
					if (x >= rc.left)
					{
						if (y >= rc.top)
						{
							if (x < rc.right)
							{
								if (y < rc.bottom)
								{
									return rc;
								}
							}
						}
					}
				}
			}
		}
		return null;
	}

	public CRect y_GetLadderAt_Absolute(int nLayer, int x, int y)
	{
		x -= rhFrame.leX;
		y -= rhFrame.leY;

		return y_GetLadderAt(nLayer, x, y);
	}

	// -------------------------------------------------------------------------
	// COLLISION DETECTION ACCELERATION
	// -------------------------------------------------------------------------

	public CArrayList<CArrayList<Integer>> getLayerZones(int nLayer)
	{
		//ASSERT(nLayer >= 0 && nLayer < (int)pCurFrame.m_nLayers);

		CLayer pLayer = rhFrame.layers[nLayer];
		CArrayList<CArrayList<Integer>> pZones = pLayer.m_loZones;

		if ( pZones == null && (rhFrame.leWidth >= OBJZONE_WIDTH * 2 || rhFrame.leHeight >= OBJZONE_HEIGHT * 2) )
		{
			int nLOs = pLayer.nBkdLOs;
			CLO plo1 = rhFrame.LOList.getLOFromIndex((short) pLayer.nFirstLOIndex);

			// Get number of zones
			int nxz = ((rhFrame.leWidth + OBJZONE_WIDTH - 1)/ OBJZONE_WIDTH) + 2;
			int nyz = ((rhFrame.leHeight + OBJZONE_HEIGHT - 1)/ OBJZONE_HEIGHT) + 2;
			int nz = nxz * nyz;

			pLayer.m_loZones = pZones = new CArrayList<CArrayList<Integer>>();
			pZones.ensureCapacity(nz);

			for (int i=0; i<nLOs; i++, plo1 = rhFrame.LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex+i)))
			{
				CObject hoPtr;
				CRect rc = new CRect();

				//ASSERT(plo1.loLayer == nLayer);

				COI poi = rhApp.OIList.getOIFromHandle(plo1.loOiHandle);

				if ( poi==null || poi.oiOC==null )
				{
					//ASSERT(FALSE);
					continue;
				}

				COC poc = poi.oiOC;
				int typeObj = poi.oiType;

				// Get object position
				rc.left = plo1.loX;	// - dxLayer;
				rc.top = plo1.loY;	// - dyLayer;

				// Get object rectangle
				if ( typeObj < COI.OBJ_SPR )
				{
					// Ladder or no obstacle? continue
					short v = poc.ocObstacleType;
					if ( v == 0 || v == OBSTACLE_LADDER || v == OBSTACLE_TRANSPARENT )
						continue;

					//ASSERT(((LPStatic_OC)poc).ocCx >= 0 && ((LPStatic_OC)poc).ocCy >= 0); // New MMF2
					rc.right = rc.left + poc.ocCx;
					rc.bottom = rc.top + poc.ocCy;
				}
				else
				{
					CObjectCommon ocPtr= (CObjectCommon)poc;

					// Dynamic item => must be a background object
					if ( (ocPtr.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0 || (hoPtr = find_HeaderObject(plo1.loHandle)) == null )
						continue;
					int v = ((ocPtr.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT);
					// Ladder or no obstacle? continue
					if ( v == 0 || v == OBSTACLE_LADDER || v == OBSTACLE_TRANSPARENT )
						continue;
					rc.left = hoPtr.hoX - hoPtr.hoImgXSpot;
					rc.top = hoPtr.hoY - hoPtr.hoImgYSpot;
					rc.right = rc.left + hoPtr.hoImgWidth;
					rc.bottom = rc.top + hoPtr.hoImgHeight;
				}

				// Add object zones to pZones
				int minzy = 0;
				if ( rc.top >= 0 )
					minzy = Math.min(rc.top / OBJZONE_HEIGHT + 1, nyz-1);
				int maxzy = 0;
				if ( rc.bottom >= 0 )
					maxzy = Math.min(rc.bottom / OBJZONE_HEIGHT + 1, nyz-1);
				for (int zy=minzy; zy<=maxzy; zy++)
				{
					int minzx = 0;
					if ( rc.left >= 0 )
						minzx = Math.min(rc.left / OBJZONE_WIDTH + 1, nxz-1);
					int maxzx = 0;
					if ( rc.right >= 0 )
						maxzx = Math.min(rc.right / OBJZONE_WIDTH + 1, nxz-1);
					for (int zx=minzx; zx<=maxzx; zx++)
					{
						// Add object to zone list
						int z = zy * nxz + zx;
						CArrayList<Integer> pZone = pZones.get(z);
						if ( pZone == null )
						{
							//ASSERT(z < nz);
							pZone = new CArrayList<Integer>();
							pZones.setAtGrow(z, pZone);
						}
						//NSLog(@"Adding bkd to zone %i", z);
						pZone.add(i+pLayer.nFirstLOIndex);
					}
				}
			}
		}
		return pZones;
	}

	// -------------------------------------------------------------------------
	// DESSIN DES OBJETS DANS LE DECOR
	// -------------------------------------------------------------------------
	public void activeToBackdrop(CObject hoPtr, int nTypeObst, boolean bTrueObject)
	{
		CBkd2 toadd = new CBkd2();
		toadd.img = hoPtr.roc.rcImage;
		CImage ifo = rhApp.imageBank.getImageFromHandle(toadd.img);
		toadd.loHnd = 0;
		toadd.oiHnd = 0;
		toadd.x = hoPtr.hoX;
		toadd.y = hoPtr.hoY;
		toadd.width = 1;
		toadd.height = 1;
		if(ifo != null) {
			toadd.x -= ifo.getXSpot();
			toadd.y -= ifo.getYSpot();
			toadd.width = ifo.getWidth();
			toadd.height = ifo.getHeight();
			toadd.antialias = ifo.getResampling();
		}

		toadd.spriteFlag = CSprite.SF_NOHOTSPOT;
		toadd.nLayer = (short) hoPtr.hoLayer;
		//toadd.nLayer = (short) 0;
		toadd.body = null;

		toadd.obstacleType = (short) nTypeObst;	// a voir
		toadd.colMode = CSpriteGen.CM_BITMAP;

		if ((hoPtr.ros.rsCreaFlags & CSprite.SF_COLBOX) != 0)
		{
			toadd.colMode = CSpriteGen.CM_BOX;
		}

		//for (int ns = 0; ns < 4; ns++)
		//{
		//	toadd.pSpr[ns] = null;
		//}
		
		Arrays.fill(toadd.pSpr, null);
		
		toadd.inkEffect = hoPtr.ros.rsEffect;
		toadd.inkEffectParam = hoPtr.ros.rsEffectParam;

		addBackdrop2(toadd);

		// Build 284.11: if "not an obstacle" and layer 0, paste collision mask
		if ( nTypeObst == 0 && toadd.nLayer == 0 && rhFrame.colMask != null && ifo != null )
		{
			if (toadd.colMode==CSpriteGen.CM_BOX)
			{
				rhFrame.colMask.fillRectangle(toadd.x, toadd.y, toadd.x+toadd.width, toadd.y+toadd.height, nTypeObst);
			}
			else
			{
				CMask mask;
				mask = ifo.getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
				if ( mask != null )
					rhFrame.colMask.orMask(mask, toadd.x, toadd.y, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, nTypeObst);
			}
		}
		// End of build 284.11 change
	}

	public void addBackdrop2(CBkd2 toadd)
	{
		CBkd2 pbkd;
		int i;

		if (toadd.nLayer < 0 || toadd.nLayer >= rhFrame.nLayers)
		{
			return;
		}
		CLayer pLayer = rhFrame.layers[toadd.nLayer];

		// Rechercher backdrop
		if (pLayer.pBkd2 != null)
		{
			int pLayer_pBkd2_size = pLayer.pBkd2.size();
			for (i = 0; i < pLayer_pBkd2_size; i++)
			{
				pbkd = pLayer.pBkd2.get(i);
				if (pbkd.x == toadd.x && pbkd.y == toadd.y 
						&& pbkd.nLayer == toadd.nLayer && pbkd.img == toadd.img 
						&& (pbkd.inkEffect & CSpriteGen.EFFECT_MASK) == CSpriteGen.EFFECT_NONE)
				{
					if (i != pLayer_pBkd2_size - 1)
					{
						for (int j = 0; j < 4; j++)
						{
							if (pbkd.pSpr[j] != null)
							{
								spriteGen.moveSpriteToFront(pbkd.pSpr[j]);
							}
						}
						pLayer.pBkd2.remove(i);
						pLayer.pBkd2.add(pbkd);
						pLayer_pBkd2_size = pLayer.pBkd2.size();
					}
					pbkd.colMode = toadd.colMode;
					pbkd.obstacleType = toadd.obstacleType;
					//pbkd.spriteFlag = toadd.spriteFlag;

					if (pbkd.inkEffect != toadd.inkEffect || pbkd.inkEffectParam != toadd.inkEffectParam)
					{
						pbkd.inkEffect = toadd.inkEffect;
						pbkd.inkEffectParam = toadd.inkEffectParam;
						for (int j = 0; j < 4; j++)
						{
							if (pbkd.pSpr[j] != null)
							{
								spriteGen.modifSpriteEffect(pbkd.pSpr[j], pbkd.inkEffect, pbkd.inkEffectParam);
							}
						}
					}
					toadd = null;
					return;
				}
			}

			// Maxi
			if (pLayer.pBkd2.size() >= rhFrame.maxObjects)
			{
				toadd = null;
				return;
			}
		}
		// Allouer m_pBkd2
		else
		{
			pLayer.pBkd2 = new ArrayList<CBkd2>();
		}

		int nIdx = pLayer.pBkd2.size();
		pLayer.pBkd2.add(toadd);

		// Add backdrop to physical world
		if (rh4Box2DObject && rh4Box2DBase != null)
		{
			if (toadd.obstacleType == COC.OBSTACLE_SOLID || toadd.obstacleType == COC.OBSTACLE_PLATFORM)
			{
				toadd.body = rh4Box2DBase.rAddABackdrop(toadd.x, toadd.y, toadd.img, toadd.obstacleType);
			}
		}

		// TODO: add sprite si layer > 0 ? (attention si layer invisible)
		pbkd = toadd;
		int v;
		CRect rc = new CRect();

		short img;
		int dxLayer, dyLayer;

		boolean bTotalColMask = ((rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0);

		boolean flgColMaskEmpty;

		// Layer offset
		dxLayer = rhFrame.leX;
		dyLayer = rhFrame.leY;

		boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
		boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
		boolean bWrap = false;
		if (bWrapHorz || bWrapVert)
		{
			bWrap = true;
		}

		int nPlayfieldWidth = rhFrame.leWidth;
		int nPlayfieldHeight = rhFrame.leHeight;

		if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
		{
			if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
			{
				dxLayer = (int) (dxLayer * pLayer.xCoef);
			}
			if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
			{
				dyLayer = (int) (dyLayer * pLayer.yCoef);
			}
		}

		// Add layer offset
		dxLayer += pLayer.x;
		dyLayer += pLayer.y;

		// Wrap? limit dxLayer/dyLayer to playfield width/height
		if (bWrapHorz)
		{
			dxLayer %= nPlayfieldWidth;
		}
		if (bWrapVert)
		{
			dyLayer %= nPlayfieldHeight;
		}

		if ((pLayer.dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) == CLayer.FLOPT_VISIBLE)
		{
			boolean bSaveBkd = ((pLayer.dwOptions & CLayer.FLOPT_NOSAVEBKD) == 0);
			int dwWrapFlags = 0;
			int nSprite = 0;
			do
			{
				int nCurSprite = nSprite;

				rc.left = pbkd.x - dxLayer;
				rc.top = pbkd.y - dyLayer;

				int x2edit = rhFrame.leEditWinWidth - 1;
				int y2edit = rhFrame.leEditWinHeight - 1;

				// Calculer rectangle objet
				img = pbkd.img;
				rc.right = rc.left + pbkd.width;
				rc.bottom = rc.top + pbkd.height;
				// Wrap
				if (bWrap)
				{
					switch (nSprite)
					{
					// Normal sprite: test if other sprites should be displayed
					case 0:
						// Wrap horizontally?
						if (bWrapHorz && (rc.left < 0 || rc.right > nPlayfieldWidth))
						{
							// Wrap horizontally and vertically?
							if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
							{
								nSprite = 3;
								dwWrapFlags |= (WRAP_X | WRAP_Y | WRAP_XY);
							}
							// Wrap horizontally only
							else
							{
								nSprite = 1;
								dwWrapFlags |= (WRAP_X);
							}
						}

						// Wrap vertically?
						else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
						{
							nSprite = 2;
							dwWrapFlags |= (WRAP_Y);
						}

						// Delete other sprites
						if ((dwWrapFlags & WRAP_X) == 0 && pbkd.pSpr[1] != null)
						{
							spriteGen.delSprite(pbkd.pSpr[1]);
							pbkd.pSpr[1] = null;
						}
						if ((dwWrapFlags & WRAP_Y) == 0 && pbkd.pSpr[2] != null)
						{
							spriteGen.delSprite(pbkd.pSpr[2]);
							pbkd.pSpr[2] = null;
						}
						if ((dwWrapFlags & WRAP_XY) == 0 && pbkd.pSpr[3] != null)
						{
							spriteGen.delSprite(pbkd.pSpr[3]);
							pbkd.pSpr[3] = null;
						}
						break;

						// Other sprite instance: wrap horizontally
					case 1:
						// Wrap
						if (rc.left < 0)				// (rc.right + curFrame.m_leX) <= 0
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left += dx;
							rc.right += dx;
						}
						else if (rc.right > nPlayfieldWidth)	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left -= dx;
							rc.right -= dx;
						}

						// Remove flag
						dwWrapFlags &= ~WRAP_X;

						// Calculate next sprite to display
						nSprite = 0;
						if ((dwWrapFlags & WRAP_Y) != 0)
						{
							nSprite = 2;
						}
						break;

						// Other sprite instance: wrap vertically
					case 2:
						// Wrap
						if (rc.top < 0)				// (rc.bottom + curFrame.m_leY) <= 0
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top += dy;
							rc.bottom += dy;
						}
						else if (rc.bottom > nPlayfieldHeight)		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top -= dy;
							rc.bottom -= dy;
						}

						// Remove flag
						dwWrapFlags &= ~WRAP_Y;

						// Calculate next sprite to display
						nSprite = 0;
						if ((dwWrapFlags & WRAP_X) != 0)
						{
							nSprite = 1;
						}
						break;

						// Other sprite instance: wrap horizontally and vertically
					case 3:
						// Wrap
						if (rc.left < 0)				// (rc.right + curFrame.m_leX) <= 0
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left += dx;
							rc.right += dx;
						}
						else if (rc.right > nPlayfieldWidth)	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left -= dx;
							rc.right -= dx;
						}
						if (rc.top < 0)				// (rc.bottom + curFrame.m_leY) <= 0
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top += dy;
							rc.bottom += dy;
						}
						else if (rc.bottom > nPlayfieldHeight)		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top -= dy;
							rc.bottom -= dy;
						}

						// Remove flag
						dwWrapFlags &= ~WRAP_XY;

						// Calculate next sprite to display
						nSprite = 2;
						break;
					}
				}

				// On the right of the display? next object (only if no total colmask)
				if (rc.left < x2edit  + COLMASK_XMARGIN + 32 && rc.top < y2edit + COLMASK_YMARGIN)
				{
					// Obstacle?
					v = pbkd.obstacleType;

					// Ladder?
					if (v == OBSTACLE_LADDER)
					{
						y_Ladder_Add(pbkd.nLayer, rc.left, rc.top, rc.right, rc.bottom);
					}

					//Log.Log("addBackdrop Left "+rc.left+" top "+rc.top+" x2edit "+x2edit+" y2edit "+y2edit + " and type: "+(v == OBSTACLE_LADDER ? "ladder": (v == OBSTACLE_SOLID?"obstacle":"transparent")));
					// Display object?
					if (rc.left <= x2edit && rc.top <= y2edit && rc.right >= 0 && rc.bottom >= 0)
					{
						////////////////////////////////////////
						// Non-background layer => create sprite
						//Log.Log("yes entered");
						int dwFlags = CSprite.SF_BACKGROUND | CSprite.SF_INACTIF; 
						if (!bSaveBkd)
						{
							dwFlags |= CSprite.SF_NOSAVE;
						}
						if (v == OBSTACLE_SOLID)
						{
							dwFlags |= (CSprite.SF_OBSTACLE | CSprite.SF_RAMBO);
						}
						if (v == OBSTACLE_PLATFORM)
						{
							dwFlags |= (CSprite.SF_PLATFORM | CSprite.SF_RAMBO);
						}

						dwFlags |= toadd.spriteFlag;
						
						if(pbkd.antialias)
								dwFlags |= CSprite.EFFECTFLAG_ANTIALIAS;
						
						pbkd.pSpr[nCurSprite] = spriteGen.addSprite(rc.left, rc.top, img, pbkd.nLayer, 0x10000000 + nIdx * 4 + nCurSprite, 0, dwFlags, null);
						// voir si fixer variable interne a pbkd
						spriteGen.modifSpriteEffect(pbkd.pSpr[nCurSprite], pbkd.inkEffect, pbkd.inkEffectParam);
					}

					// Obstacle in layer 0?
					if (toadd.nLayer == 0 && v != OBSTACLE_TRANSPARENT)
					{
						// Retablir coords absolues si TOTALCOLMASK
						if (bTotalColMask)
						{
							//OffsetRect (&rc, dxLayer, dyLayer);  // pCurFrame.m_leX, pCurFrame.m_leY);
							rc.left += dxLayer;
							rc.top += dyLayer;
							rc.right += dxLayer;
							rc.bottom += dyLayer;
						}

						// Update collisions bitmap (meme si objet en dehors)
						int obst = 0;
						flgColMaskEmpty=true;
						if (v == OBSTACLE_SOLID)
						{
							obst = CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM;
							flgColMaskEmpty = false;
						}

						// Ajouter le masque "obstacle"
						CMask mask;
						if (flgColMaskEmpty == false)
						{
							if (toadd.colMode==CSpriteGen.CM_BOX)
							{
								rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right, rc.bottom, obst);
							}
							else
							{
								mask = rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
								rhFrame.colMask.orMask(mask, rc.left, rc.top, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, obst);
							}
						}

						// Ajouter le masque plateforme
						if (v == OBSTACLE_PLATFORM)
						{
							flgColMaskEmpty = false;
							if (toadd.colMode==CSpriteGen.CM_BOX)
							{
								rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right,
										Math.min(rc.top + CColMask.HEIGHT_PLATFORM, rc.bottom), CColMask.CM_PLATFORM);
							}
							else
							{
								mask = rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
								rhFrame.colMask.orPlatformMask(mask, rc.left, rc.top);
							}
						}
					}
				}

			} while (dwWrapFlags != 0);
		}
		toadd = null;
	}

	/** Delete all created backdrop objects from a layer
	 */
	public void deleteAllBackdrop2(int nLayer)
	{
		int i;
		CBkd2 pbkd;

		if (nLayer < 0 || nLayer >= rhFrame.nLayers)
		{
			return;
		}

		CLayer pLayer = rhFrame.layers[nLayer];
		if (pLayer.pBkd2 != null)
		{
			// Delete sprites
			int pLayer_pBkd2_size = pLayer.pBkd2.size();
			for (i = 0; i < pLayer_pBkd2_size; i++)
			{
				pbkd = pLayer.pBkd2.get(i);
				for (int ns = 0; ns < 4; ns++)
				{
					if (pbkd.pSpr[ns] != null)
					{
						spriteGen.delSprite(pbkd.pSpr[ns]);
						pbkd.pSpr[ns] = null;
					}
				}
				if (rh4Box2DObject && rh4Box2DBase != null && pbkd.body != null)
				{
					rh4Box2DBase.rSubABackdrop(pbkd.body);
				}

			}
			pLayer.pBkd2.clear();
			pLayer.pBkd2 = null;

			// Force redraw
			pLayer.dwOptions |= CLayer.FLOPT_REDRAW;
			rh3Scrolling |= RH3SCROLLING_REDRAWLAYERS;
		}
	}

	/** Delete created backdrop object at given coordinates
	 */
	public void deleteBackdrop2At(int nLayer, int x, int y, boolean bFineDetection)
	{
		if (nLayer < 0 || nLayer >= rhFrame.nLayers)
		{
			return;
		}
		CLayer pLayer = rhFrame.layers[nLayer];

		// Rechercher backdrop
		if (pLayer.pBkd2 != null)
		{
			int i;
			CBkd2 pbkd;

			boolean bSomethingDeleted = false;

			boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
			boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
			boolean bWrap = (bWrapHorz | bWrapVert);

			int nPlayfieldWidth = rhFrame.leWidth;
			int nPlayfieldHeight = rhFrame.leHeight;

			// Layer offset
			int dxLayer = rhFrame.leX;
			int dyLayer = rhFrame.leY;

			//Log.Log("About to start object research ...");
			if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
			{
				if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
				{
					dxLayer = (int) (dxLayer * pLayer.xCoef);
				}
				if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
				{
					dyLayer = (int) (dyLayer * pLayer.yCoef);
				}
			}

			// Add layer offset
			dxLayer += pLayer.x;
			dyLayer += pLayer.y;

			// Limit dxLayer/dyLayer to playfield width/height
			if (bWrapHorz)
			{
				dxLayer %= nPlayfieldWidth;
			}
			if (bWrapVert)
			{
				dyLayer %= nPlayfieldHeight;
			}

			int dwWrapFlags = 0;
			int nSprite = 0;

			int pLayer_pBkd2_size = pLayer.pBkd2.size();

			for (i = 0; i < pLayer_pBkd2_size; i++)
			{
				pbkd = pLayer.pBkd2.get(i);

				boolean bFound = false;

				if (pbkd.nLayer == nLayer)		// Heu ... c'est la peine de faire ce test?
				{
					// Box2D world
					if (rh4Box2DObject && rh4Box2DBase != null && pbkd.body != null)
					{
						rh4Box2DBase.rSubABackdrop(pbkd.body);
					}
					// Get object position
					CRect rc = new CRect();
					boolean cm_box = (pbkd.colMode == CSpriteGen.CM_BOX);
					rc.left = pbkd.x - dxLayer;
					rc.top = pbkd.y - dyLayer;

					// Get object rectangle
					//CImage pImg = rhApp.imageBank.getImageFromHandle(pbkd.img);
					//if (pImg != null)
					//{
					//	rc.right = rc.left + pImg.getWidth ();
					//	rc.bottom = rc.top + pImg.getHeight ();
					//}
					//else
					//{
					//	rc.right = rc.left + 1;
					//	rc.bottom = rc.top + 1;
					//}
					rc.right = rc.left + pbkd.width;
					rc.bottom = rc.top + pbkd.height;

					// Wrap
					if (bWrap)
					{
						switch (nSprite)
						{
						// Normal sprite: test if other sprites should be displayed
						case 0:
							// Wrap horizontally?
							if (bWrapHorz && (rc.left < 0 || rc.right > nPlayfieldWidth))
							{
								// Wrap horizontally and vertically?
								if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
								{
									nSprite = 3;
									dwWrapFlags |= (WRAP_X | WRAP_Y | WRAP_XY);
								}

								// Wrap horizontally only
								else
								{
									nSprite = 1;
									dwWrapFlags |= (WRAP_X);
								}
							}

							// Wrap vertically?
							else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
							{
								nSprite = 2;
								dwWrapFlags |= (WRAP_Y);
							}
							break;

							// Other sprite instance: wrap horizontally
						case 1:
							// Wrap
							if (rc.left < 0)				// (rc.right + curFrame.m_leX) <= 0
							{
								int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
								rc.left += dx;
								rc.right += dx;
							}
							else if (rc.right > nPlayfieldWidth)	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
							{
								int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
								rc.left -= dx;
								rc.right -= dx;
							}

							// Remove flag
							dwWrapFlags &= ~WRAP_X;

							// Calculate next sprite to display
							nSprite = 0;
							if ((dwWrapFlags & WRAP_Y) != 0)
							{
								nSprite = 2;
							}
							break;

							// Other sprite instance: wrap vertically
						case 2:
							// Wrap
							if (rc.top < 0)				// (rc.bottom + curFrame.m_leY) <= 0
							{
								int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
								rc.top += dy;
								rc.bottom += dy;
							}
							else if (rc.bottom > nPlayfieldHeight)		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
							{
								int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
								rc.top -= dy;
								rc.bottom -= dy;
							}

							// Remove flag
							dwWrapFlags &= ~WRAP_Y;

							// Calculate next sprite to display
							nSprite = 0;
							if ((dwWrapFlags & WRAP_X) != 0)
							{
								nSprite = 1;
							}
							break;

							// Other sprite instance: wrap horizontally and vertically
						case 3:
							// Wrap
							if (rc.left < 0)				// (rc.right + curFrame.m_leX) <= 0
							{
								int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
								rc.left += dx;
								rc.right += dx;
							}
							else if (rc.right > nPlayfieldWidth)	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
							{
								int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
								rc.left -= dx;
								rc.right -= dx;
							}
							if (rc.top < 0)				// (rc.bottom + curFrame.m_leY) <= 0
							{
								int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
								rc.top += dy;
								rc.bottom += dy;
							}
							else if (rc.bottom > nPlayfieldHeight)		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
							{
								int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
								rc.top -= dy;
								rc.bottom -= dy;
							}

							// Remove flag
							dwWrapFlags &= ~WRAP_XY;

							// Calculate next sprite to display
							nSprite = 2;
							break;
						}
					}

					bFound = false;

					do
					{
						// Point in rectangle?
						if (x < rc.left || y < rc.top)
						{
							break;
						}

						if (x >= rc.right || y >= rc.bottom)
						{
							break;
						}

						// Point in box
						if (!bFineDetection || cm_box)
						{
							//Log.Log("Found ...");
							bFound = true;
							break;
						}

						// Test if point into image mask
						CMask pMask = rhApp.imageBank.getImageFromHandle(pbkd.img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
						if (pMask != null)
						{
							if (pMask.testPoint(x - rc.left, y - rc.top))
							{
								bFound = true;
								break;
							}
						}

					} while (false);

					// Backdrop object found? remove it
					if (bFound)
					{
						// Set flag for redraw
						bSomethingDeleted = true;

						// Delete sprites
						for (int ns = 0; ns < 4; ns++)
						{
							if (pbkd.pSpr[ns] != null)
							{
								spriteGen.delSprite(pbkd.pSpr[ns]);
								pbkd.pSpr[ns] = null;
							}
						}

						// Overwrite bkd2 structure
						pLayer.pBkd2.remove(i);

						// decrement image count and remove it from temp image table if doesn't exist anymore
						// not urgent...

						// Do not exit loop, this routine deletes all the created backdrop objects at this location
						// break;

						// Reset wrap flags
						dwWrapFlags = 0;

						// Decrement loop index
						i--;

						// Resize the For
						pLayer_pBkd2_size = pLayer.pBkd2.size();
					}

					// Wrapped?
					if (dwWrapFlags != 0)
					{
						i--;
					}
				}
			}

			// Force redraw
			if (bSomethingDeleted)
			{
				pLayer.dwOptions |= CLayer.FLOPT_REDRAW;
				rh3Scrolling |= RH3SCROLLING_REDRAWLAYERS;
			}

			return;
		}
	}

	public void displayBkd2Layer(CLayer pLayer, int nLayer, int flags, int x2edit, int y2edit, boolean flgColMaskEmpty)
	{
		CBkd2 pbkd;
		int i, obst, v;
		CRect rc = new CRect();
		short img;
		int dxLayer, dyLayer;
		boolean bTotalColMask = ((rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0);
		boolean bUpdateColMask = ((flags & DLF_DONTUPDATECOLMASK) == 0);

		if(rhFrame.colMask == null)
			bUpdateColMask = false;

		// Log.Log(" UpdateColMask "+bUpdateColMask+" flags "+flags);
		// Layer offset
		dxLayer = rhFrame.leX;
		dyLayer = rhFrame.leY;

		boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
		boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
		boolean bWrap = (bWrapHorz || bWrapVert);

		int nPlayfieldWidth = rhFrame.leWidth;
		int nPlayfieldHeight = rhFrame.leHeight;

		if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
		{
			if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
			{
				dxLayer = (int) (dxLayer * pLayer.xCoef);
			}
			if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
			{
				dyLayer = (int) (dyLayer * pLayer.yCoef);
			}
		}

		// Add layer offset
		dxLayer += pLayer.x;
		dyLayer += pLayer.y;

		// Wrap? limit dxLayer/dyLayer to playfield width/height
		if (bWrapHorz)
		{
			dxLayer %= nPlayfieldWidth;
		}
		if (bWrapVert)
		{
			dyLayer %= nPlayfieldHeight;
		}

		// Hide layer?
		if ((pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0)
		{
			int pLayer_pBkd2_size = pLayer.pBkd2.size();
			for (i = 0; i < pLayer_pBkd2_size; i++)
			{
				pbkd = pLayer.pBkd2.get(i);

				// Delete sprite
				for (int ns = 0; ns < 4; ns++)
				{
					if (pbkd.pSpr[ns] != null)
					{
						spriteGen.delSprite(pbkd.pSpr[ns]);
						pbkd.pSpr[ns] = null;
					}
				}
			}
		}

		// Display layer
		if ((pLayer.dwOptions & CLayer.FLOPT_TOHIDE) == 0)
		{
			boolean bSaveBkd = ((pLayer.dwOptions & CLayer.FLOPT_NOSAVEBKD) == 0);
			int dwWrapFlags = 0;
			int nSprite = 0;

			int pLayer_pBkd2_size = pLayer.pBkd2.size();
			for (i = 0; i < pLayer_pBkd2_size; i++)
			{
				pbkd = pLayer.pBkd2.get(i);

				int nCurSprite = nSprite;

				//Actual sprite position (hotspot position)
				int screenPosX = pbkd.x - dxLayer;
				int screenPosY = pbkd.y - dyLayer;

				//Sprite is initially located at the backdrop's position.
				rc.left = screenPosX;
				rc.top = screenPosY;

				img = pbkd.img;
				CImage pImg = rhApp.imageBank.getImageFromHandle(img);
				if(pImg != null)
				{
					//Adjust bounding rectangle for hotspot
					if((pbkd.spriteFlag & CSprite.SF_NOHOTSPOT) == 0)
					{
						rc.left -= pImg.getXSpot();
						rc.top -= pImg.getYSpot();
					}

					// Calculer rectangle objet
					//rc.right = rc.left + pImg.getWidth();
					//rc.bottom = rc.top + pImg.getHeight();
				}
				//else
				//{
				//	rc.right = rc.left + 1;
				//	rc.bottom = rc.top + 1;
				//}
				rc.right = rc.left + pbkd.width;
				rc.bottom = rc.top + pbkd.height;

				// On the left or right of the display? next object (only if no total colmask)
				if (!bTotalColMask && !bWrap && 
						(rc.left >= x2edit + COLMASK_XMARGIN + 32 || rc.top >= y2edit + COLMASK_YMARGIN ||
						rc.right <= -COLMASK_XMARGIN - 32 || rc.bottom <= - COLMASK_YMARGIN))
				{
					// Out of visible area: delete sprite
					if (pbkd.pSpr[nCurSprite] != null)
					{
						spriteGen.delSprite(pbkd.pSpr[nCurSprite]);
						pbkd.pSpr[nCurSprite] = null;
					}
					continue;
				}

				// Wrap
				if (bWrap)
				{
					switch (nSprite)
					{
					// Normal sprite: test if other sprites should be displayed
					case 0:
						// Wrap horizontally?
						if (bWrapHorz && (rc.left < 0 || rc.right > nPlayfieldWidth))
						{
							// Wrap horizontally and vertically?
							if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
							{
								nSprite = 3;
								dwWrapFlags |= (WRAP_X | WRAP_Y | WRAP_XY);
							}

							// Wrap horizontally only
							else
							{
								nSprite = 1;
								dwWrapFlags |= (WRAP_X);
							}
						}

						// Wrap vertically?
						else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
						{
							nSprite = 2;
							dwWrapFlags |= (WRAP_Y);
						}

						// Delete other sprites
						if ((dwWrapFlags & WRAP_X) == 0 && pbkd.pSpr[1] != null)
						{
							spriteGen.delSprite(pbkd.pSpr[1]);
							pbkd.pSpr[1] = null;
						}
						if ((dwWrapFlags & WRAP_Y) == 0 && pbkd.pSpr[2] != null)
						{
							spriteGen.delSprite(pbkd.pSpr[2]);
							pbkd.pSpr[2] = null;
						}
						if ((dwWrapFlags & WRAP_XY) == 0 && pbkd.pSpr[3] != null)
						{
							spriteGen.delSprite(pbkd.pSpr[3]);
							pbkd.pSpr[3] = null;
						}
						break;

						// Other sprite instance: wrap horizontally
					case 1:
						// Wrap
						if (rc.left < 0)				// (rc.right + curFrame.m_leX) <= 0
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left += dx;
							rc.right += dx;
						}
						else if (rc.right > nPlayfieldWidth)	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left -= dx;
							rc.right -= dx;
						}

						// Remove flag
						dwWrapFlags &= ~WRAP_X;

						// Calculate next sprite to display
						nSprite = 0;
						if ((dwWrapFlags & WRAP_Y) != 0)
						{
							nSprite = 2;
						}
						break;

						// Other sprite instance: wrap vertically
					case 2:
						// Wrap
						if (rc.top < 0)				// (rc.bottom + curFrame.m_leY) <= 0
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top += dy;
							rc.bottom += dy;
						}
						else if (rc.bottom > nPlayfieldHeight)		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top -= dy;
							rc.bottom -= dy;
						}

						// Remove flag
						dwWrapFlags &= ~WRAP_Y;

						// Calculate next sprite to display
						nSprite = 0;
						if ((dwWrapFlags & WRAP_X) != 0)
						{
							nSprite = 1;
						}
						break;

						// Other sprite instance: wrap horizontally and vertically
					case 3:
						// Wrap
						if (rc.left < 0)				// (rc.right + curFrame.m_leX) <= 0
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left += dx;
							rc.right += dx;
						}
						else if (rc.right > nPlayfieldWidth)	// (rc.left + curFrame.m_leX) >= nPlayfieldWidth
						{
							int dx = nPlayfieldWidth;	// + (rc.right - rc.left)
							rc.left -= dx;
							rc.right -= dx;
						}
						if (rc.top < 0)				// (rc.bottom + curFrame.m_leY) <= 0
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top += dy;
							rc.bottom += dy;
						}
						else if (rc.bottom > nPlayfieldHeight)		// (rc.top + curFrame.m_leY) >= nPlayfieldHeight
						{
							int dy = nPlayfieldHeight;	// + (rc.bottom - rc.top)
							rc.top -= dy;
							rc.bottom -= dy;
						}

						// Remove flag
						dwWrapFlags &= ~WRAP_XY;

						// Calculate next sprite to display
						nSprite = 2;
						break;
					}
				}

				screenPosX = rc.left;		// Fixes bug 2658
				screenPosY = rc.top;
				v = pbkd.obstacleType;
				boolean cm_box = (pbkd.colMode == CSpriteGen.CM_BOX);

				// Ladder?
				if (v == OBSTACLE_LADDER)
				{
					y_Ladder_Add(nLayer, rc.left, rc.top, rc.right, rc.bottom);
					cm_box = true;		// Fill rectangle in collision masque
				}

				// Obstacle in layer 0?
				if (nLayer == 0 && bUpdateColMask && v != OBSTACLE_TRANSPARENT &&
						(bTotalColMask || 
								(rc.right >= -COLMASK_XMARGIN - 32 && rc.bottom >= -COLMASK_YMARGIN) ||
								(rc.left <= x2edit + COLMASK_XMARGIN + 32 && rc.top <= y2edit + COLMASK_YMARGIN)))
				{
					// Retablir coords absolues si TOTALCOLMASK
					if (bTotalColMask)
					{
						//OffsetRect (&rc, dxLayer, dyLayer);	// pCurFrame.m_leX, pCurFrame.m_leY);
						rc.left += dxLayer;
						rc.top += dyLayer;
						rc.right += dxLayer;
						rc.bottom += dyLayer;
					}

					// Update collisions bitmap (meme si objet en dehors)
					obst = 0;
					if (v == OBSTACLE_SOLID)
					{
						obst = CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM;
						flgColMaskEmpty = false;
					}

					// Ajouter le masque "obstacle"
					CMask mask = null;
					if (flgColMaskEmpty == false)
					{
						if (!cm_box)
							mask = rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
						if (mask == null)
						{
							rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right, rc.bottom, obst);
						}
						else
						{
							rhFrame.colMask.orMask(mask, rc.left, rc.top, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, obst);
						}
					}

					// Ajouter le masque plateforme
					if (v == OBSTACLE_PLATFORM)
					{
						flgColMaskEmpty = false;
						if (!cm_box)
							mask = rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
						if (mask == null)
						{
							rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right, (Math.min(rc.top + CColMask.HEIGHT_PLATFORM, rc.bottom)), CColMask.CM_PLATFORM);
						}
						else
						{
							rhFrame.colMask.orPlatformMask(mask, rc.left, rc.top);
						}
					}

					// Retablir coords absolues si TOTALCOLMASK
					if (bTotalColMask)
					{
						//OffsetRect (&rc, -dxLayer, -dyLayer);	// -pCurFrame.m_leX, -pCurFrame.m_leY);
						rc.left -= dxLayer;
						rc.top -= dyLayer;
						rc.right -= dxLayer;
						rc.bottom -= dyLayer;
					}
				}

				// Display object?
				if (rc.left <= x2edit && rc.top <= y2edit && rc.right >= 0 && rc.bottom >= 0)
				{
					////////////////////////////////////////
					// Non-background layer => create sprite
					
					int dwFlags = CSprite.SF_BACKGROUND | CSprite.SF_INACTIF;
					if (!bSaveBkd)
					{
						dwFlags |= CSprite.SF_NOSAVE;
					}
					if (v == OBSTACLE_SOLID)
					{
						dwFlags |= (CSprite.SF_OBSTACLE | CSprite.SF_RAMBO);
					}
					if (v == OBSTACLE_PLATFORM)
					{
						dwFlags |= (CSprite.SF_PLATFORM | CSprite.SF_RAMBO);
					}

					dwFlags |= pbkd.spriteFlag;
					// Create sprite only if not already created
					if (pbkd.pSpr[nCurSprite] == null)
					{
						if(pbkd.antialias)
								dwFlags |= CSprite.EFFECTFLAG_ANTIALIAS;
						
						pbkd.pSpr[nCurSprite] = spriteGen.addSprite(screenPosX, screenPosY, img, pbkd.nLayer, 0x10000000 + i * 4 + nCurSprite, 0, dwFlags, null);
						// mettre spriteextra (DWORD)pbkd
						spriteGen.modifSpriteEffect(pbkd.pSpr[nCurSprite], pbkd.inkEffect, pbkd.inkEffectParam);
					}

					// Otherwise update sprite coordinates
					else
					{
						spriteGen.modifSprite(pbkd.pSpr[nCurSprite], screenPosX, screenPosY, img);
					}
				}

				// Object out of visible area: delete sprite
				else
				{
					// Delete sprite
					if (pbkd.pSpr[nCurSprite] != null)
					{
						spriteGen.delSprite(pbkd.pSpr[nCurSprite]);
						pbkd.pSpr[nCurSprite] = null;
					}
				}

				// Wrapped? re-display the same object
				if (dwWrapFlags != 0)
				{
					i--;
				}
			}
		}
	}

	// -------------------------------------------------------------------------
	// BACKDRAW ROUTINES
	// -------------------------------------------------------------------------
	public void addBackDrawRoutine(CBackDraw routine)
	{
		if (rh4BackDrawRoutines == null)
		{
			rh4BackDrawRoutines = new ArrayList<CBackDraw>();
		}
		rh4BackDrawRoutines.add(routine);
	}

	public void callBackDrawRoutines()
	{
		if (rh4BackDrawRoutines != null)
		{
			// for performance reasons
			for (CBackDraw routine : rh4BackDrawRoutines)
			{
				routine.execute(this);
			}
			rh4BackDrawRoutines.clear();
		}
	}

	public void razBackDrawRoutines()
	{
		if (rh4BackDrawRoutines != null)
		{
			rh4BackDrawRoutines.clear();

		}
	}

	// -------------------------------------------------------------------------
	// MAIN LOOP
	// -------------------------------------------------------------------------
	public void updateFrameDimensions(int width, int height)
	{
		if (width>0)
		{
			rhLevelSx=width;
			rh3WindowSx=width;
			rh3XMaximumKill = rhLevelSx + GAME_XBORDER;

			int wx = rhWindowX;			// Maximum gestion droite
			wx += rh3WindowSx + COLMASK_XMARGIN;
			if (wx > rhLevelSx)
			{
				wx = rh3XMaximumKill;
			}
			rh3XMaximum = wx;
		}
		if (height>0)
		{
			rhLevelSy=height;
			rh3WindowSy = height;
			rh3YMaximumKill = rhLevelSy + GAME_YBORDER;

			int wy = rhWindowY;			// Maximum gestion bas
			wy += rh3WindowSy + COLMASK_YMARGIN;
			if (wy > rhLevelSy)
			{
				wy = rh3YMaximumKill;
			}
			rh3YMaximum = wy;
		}
	}
	
	public void f_InitLoop()
	{
		long tick = System.currentTimeMillis();
		rhTimerOld = tick;
		rhTimer = 0;

		pauseEOLCount = -1;
		rhLoopCount = 0;
		rh4LoopTheoric = 0;
		//rh2PushedEvents=0;

		rhVBLOld = rhApp.newGetCptVbl() - 1;	    // Force un premier tour effectif!
		rh4VBLDelta = 0;

		rhQuit = 0;						// On ne sort pas!
		rhQuitBis = 0;
		rhDestroyPos = 0;				// Destroy list

		for (int n = 0; n < (rhMaxObjects + 31) / 32; n++)		// Yves: ajout du +31: il faut aussi modifier la routine d'allocation
		{
			rhDestroyList[n] = 0;
		}

		// Taille de la fenetre
		rh3WindowSx = rhFrame.leEditWinWidth;
		rh3WindowSy = rhFrame.leEditWinHeight;

		// Position de KILL des objets loin du terrain
		rh3XMinimumKill = -GAME_XBORDER;		// Les bordures externes
		rh3YMinimumKill = -GAME_YBORDER;
		rh3XMaximumKill = rhLevelSx + GAME_XBORDER;
		rh3YMaximumKill = rhLevelSy + GAME_YBORDER;

		// Coordonnees limites de gestion
		int dx = rhWindowX;
		rh3DisplayX = dx;				// Minimum gestion gauche
		dx -= COLMASK_XMARGIN;
		if (dx < 0)
		{
			dx = rh3XMinimumKill;
		}
		rh3XMinimum = dx;

		int dy = rhWindowY;			// Minimum gestion haute
		rh3DisplayY = dy;
		dy -= COLMASK_YMARGIN;
		if (dy < 0)
		{
			dy = rh3YMinimumKill;
		}
		rh3YMinimum = dy;

		int wx = rhWindowX;			// Maximum gestion droite
		wx += rh3WindowSx + COLMASK_XMARGIN;
		if (wx > rhLevelSx)
		{
			wx = rh3XMaximumKill;
		}
		rh3XMaximum = wx;

		int wy = rhWindowY;			// Maximum gestion bas
		wy += rh3WindowSy + COLMASK_YMARGIN;
		if (wy > rhLevelSy)
		{
			wy = rh3YMaximumKill;
		}
		rh3YMaximum = wy;

		// Inits diverses
		rh3Scrolling = 0;				// Pas de scrolling
		rh4DoUpdate = 0;				// Premier tour de jeu non affiche...
		rh4EventCount = 0;			// Compteur pour les evenements
		rh4TimeOut = 0;				// Time Out a zero

		// Init compteur de sauvegarde des PAUSES
		rh2PauseCompteur = 0;

		// Toutes les entrees sont selectionnees
		rh4FakeKey = 0;
		int n;
		//for (n = 0; n < 4; n++)
		//{
		//	rhPlayer[n] = 0;
		//	rh2OldPlayer[n] = 0;
		//	rh2InputMask[n] = (byte) 0xFF;
		//}
		Arrays.fill(rhPlayer, (byte) 0);
		Arrays.fill(rh2OldPlayer, (byte) 0);
		Arrays.fill(rh2InputMask, (byte) 0xFF);

		rh2MouseKeys = 0;


		// RAZ flags actions
		rhEvtProg.rh2ActionEndRoutine = false;
		rh4OnCloseCount = -1;
		rh4EndOfPause = -1;
		rh4LoadCount = -1;
		rhEvtProg.rh4CheckDoneInstart = false;
		rh4PauseKey = 0;
		bodiesCreated = false;
		rh4Box2DSearched = false;
		rh4Box2DBase = null;
		rh4TimerEvents=null;

		this.rh4ForEachs=null;
		this.rh4CurrentForEach=null;
		this.rh4CurrentForEach2=null;

		this.rh4CurrentFastLoop = null;

		// RAZ du buffer de calcul du framerate
		for (n = 0; n < MAX_FRAMERATE; n++)
		{
			rh4FrameRateArray[n] = 20;				// initialisation ��� 1/50eme de seconde
		}
		rh4FrameRatePos = 0;

		// Pas de routine de draw dans le decor
		razBackDrawRoutines();

		// Joystick creation
		if (rhApp.parentApp==null)
		{
			if (rhFrame.joystick==CRunFrame.JOYSTICK_EXT)
			{
				rhApp.createJoystick(false, 0);
				rhApp.createJoystickAcc(false);
			}
			else
			{
				Log.Log("iPhoneOptions are " + rhFrame.iPhoneOptions);

				int flags=0;
				if ((rhFrame.iPhoneOptions&CRunFrame.IPHONEOPT_JOYSTICK_FIRE1)!=0)
				{
					Log.Log("Setting JFLAG_FIRE1");
					flags=CJoystick.JFLAG_FIRE1;
				}
				if ((rhFrame.iPhoneOptions&CRunFrame.IPHONEOPT_JOYSTICK_FIRE2)!=0)
				{
					flags|=CJoystick.JFLAG_FIRE2;
				}
				if ((rhFrame.iPhoneOptions&CRunFrame.IPHONEOPT_JOYSTICK_LEFTHAND)!=0)
				{
					flags|=CJoystick.JFLAG_LEFTHANDED;
				}
				if ((rhFrame.iPhoneOptions&CRunFrame.ANDROIDFOPT_JOYSTICK_DPAD)!=0)
				{
					flags|=CJoystick.JFLAG_DPAD;
				}
				if (rhFrame.joystick==CRunFrame.JOYSTICK_TOUCH)
				{
					flags|=CJoystick.JFLAG_JOYSTICK;
				}
				if ((flags&(CJoystick.JFLAG_FIRE1|CJoystick.JFLAG_FIRE2|CJoystick.JFLAG_JOYSTICK))!=0)
				{
					rhApp.createJoystick(true, flags);
				}
				else
				{
					rhApp.createJoystick(false, 0);
				}

				// Accelerometer joystick
				if (rhFrame.joystick==CRunFrame.JOYSTICK_ACCELEROMETER)
				{
					rhApp.createJoystickAcc(true);
				}
				else
				{
					rhApp.createJoystickAcc(false);
				}

			}
		}
	}

	public int f_GameLoop()
	{
		//long timer = System.currentTimeMillis();
		//Log.Log("Initial Game Loop time: "+timer);
		if(rhFrame == null )
			return 0;

		if(rhApp.modalSubApp != null) {
			rhApp.modalSubApp.handle();
			screen_Update();
			return 0;
		}
		// On est en pause?
		// ~~~~~~~~~~~~~~~~
		if (rh2PauseCompteur != 0)
		{
			screen_Update();
			return 0;
		}

		// Delayed Resume when returning from background
		if(returnPausedEvent != 0 && (rh4EventCount-returnPausedEvent) > 0) {
			returnPausedEvent = 0;
			rh4PauseKey=-1;
			rhQuit=LOOPEXIT_PAUSEGAME;
			bPaused = true;
		}

		// Create Box2D Bodies
		//long xtimer = System.currentTimeMillis() - timer;
		if (rh4Box2DObject && !bodiesCreated)
		{
			//Log.Log("Before create Bodies time: "+xtimer);

			CreateBodies();
			bodiesCreated = true;
		}
		//xtimer = System.currentTimeMillis() - timer;
		//Log.Log("After Create Bodies time: "+xtimer);

		// Delayed Pause

		// Mode VBL Independant?
		// ~~~~~~~~~~~~~~~~~~~~~
		if (rhApp.parentApp == null)
		{
			if ((rhGameFlags & GAMEFLAGS_VBLINDEP) != 0)
			{
				if (rh4VBLDelta <= 0)				// Encore des boucles ??? faire vite?
				{
					int vbl = rhApp.newGetCptVbl();
					int delta_eax = vbl - rhVBLOld;
					if (delta_eax == 0)
					{
						if (rhQuit == 0)
						{
							return rhQuitBis;
						}

						// Appel eventuel des evenements fin de niveau
						// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
						if (rhQuit == LOOPEXIT_NEXTLEVEL ||
								rhQuit == LOOPEXIT_PREVLEVEL ||
								rhQuit == LOOPEXIT_ENDGAME ||
								rhQuit == LOOPEXIT_GOTOLEVEL ||
								rhQuit == LOOPEXIT_QUIT ||
								rhQuit == LOOPEXIT_NEWGAME)
						{
							rhEvtProg.handle_GlobalEvents((-2 << 16) | 0xFFFD);
						}
						return rhQuit;
					}

					rhVBLOld = vbl;

					delta_eax += rh4LoopTheoric;
					int delta_ebx = delta_eax;
					delta_eax -= rhLoopCount;
					if (delta_eax > 3)					// Protection sur le rattrapage
					{
						delta_eax = 3;
						delta_ebx = rhLoopCount + delta_eax;
					}
					rh4LoopTheoric = delta_ebx;
					rh4VBLDelta = delta_eax;
				}
			}
			// Mode dependant du VBL
			// ~~~~~~~~~~~~~~~~~~~~~
			else
			{
				int wVbl = rhApp.newGetCptVbl();

				/* TODO: Temporary workaround for the subapp frame skip issue */

				if (false && wVbl == rhVBLOld)
				{
					if (rhQuit == 0)
					{
						return rhQuitBis;
					}

					// Appel eventuel des evenements fin de niveau
					// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
					if (rhQuit == LOOPEXIT_NEXTLEVEL ||
							rhQuit == LOOPEXIT_PREVLEVEL ||
							rhQuit == LOOPEXIT_ENDGAME ||
							rhQuit == LOOPEXIT_GOTOLEVEL ||
							rhQuit == LOOPEXIT_QUIT ||
							rhQuit == LOOPEXIT_NEWGAME)
					{
						rhEvtProg.handle_GlobalEvents((-2 << 16) | 0xFFFD);
					}
					/* return rhQuit; */
				}
				rhVBLOld = wVbl;
				rh4VBLDelta = 1;
			}
		}

		// Recupere l'horloge, le nombre de loops
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		long timerBase = System.currentTimeMillis();
		long delta = timerBase - rhTimerOld;
		long oldtimer = rhTimer;
		rhTimer = delta;
		delta -= oldtimer;
		rhTimerDelta = (int) delta;		// Delta a la boucle precedente
		rh4TimeOut += delta;			// Compteur time-out.
		rhLoopCount += 1;
		rh4MvtTimerCoef = ((double) rhTimerDelta) * ((double) rhFrame.m_dwMvtTimerBase) / 1000.0;

		// Gestion du framerate
		rh4FrameRateArray[rh4FrameRatePos] = (int) delta;
		rh4FrameRatePos++;
		if (rh4FrameRatePos >= MAX_FRAMERATE)
		{
			rh4FrameRatePos = 0;
		}

		// Le joystick
		// ~~~~~~~~~~~
		CRunApp app = rhApp;
		while (app.parentApp != null && (app.parentOptions & CCCA.CCAF_SHARE_PLAYERCTRLS) != 0)
		{
			app = app.parentApp;
		}

		int n;
		//for (n = 0; n < 4; n++)
		//{
		//	rh2OldPlayer[n] = rhPlayer[n];
		//}			
		// Appel des messages JOYSTICK PRESSED pour chaque joueur
		// rh2OldPlayer=rhPlayer;

		rh2OldPlayer = rhPlayer.clone();
		Arrays.fill(rhPlayer, (byte)0x00);
		
		// No Joystick Test if an OUYA console is running
		if(!MMFRuntime.OUYA && !MMFRuntime.FIRETV  && !MMFRuntime.NEXUSTV && !MMFRuntime.joystickEmul) {		
			joyTest(app);
		}

		// La souris, si un mouvement souris est defini
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (rhMouseUsed != 0)
		{
			// Retrouve les touches de la souris
			rh2MouseKeys = 0;

			if (app.getKeyState(0))
			{
				rh2MouseKeys |= 0x10;				//00010000B;
			}

			// Force les touches souris comme joystick
			byte mouseUsed = rhMouseUsed;
			if ((mouseUsed & 1) != 0)
			{
				byte key = (byte) (rhPlayer[0] & 0xCF);		//11001111B;
				key |= rh2MouseKeys;
				rhPlayer[0] = key;
			}
		}


		if(!MMFRuntime.OUYA && !MMFRuntime.FIRETV  && !MMFRuntime.NEXUSTV && !MMFRuntime.joystickEmul)
		{
			rhPlayer[0]=0;
			if (rhMouseUsed!=0)
			{
				rhPlayer[0]=rh2MouseKeys;
			}
			if (app.joystick!=null)
			{
				rhPlayer[0] = (byte)app.joystick.joystick;
			}
			if (app.joystickAcc!=null)
			{
				rhPlayer[0] = (byte) ((rhPlayer[0]&0xF0)|app.joystickAcc.joystick);
			}

			rhPlayer[0]&=rhJoystickMask;


		}

		// Each Device have only one holder for joystick values
		// these values are read as static
		if (MMFRuntime.joystickEmul) {
			rhPlayer = CJoystickEmulated.rhPlayer.clone();
		}

		if(MMFRuntime.OUYA) {
			rhPlayer = CJoystickOUYA.rhPlayer.clone();
		}

		if(MMFRuntime.FIRETV) {
			rhPlayer = CJoystickFIRETV.rhPlayer.clone();
		}

		if(MMFRuntime.NEXUSTV) {
			rhPlayer = CJoystickNEXUSTV.rhPlayer.clone();
		}

		byte b;
		//Log.Log(" NPlayers: "+rhNPlayers+" CRun: "+this );
		for (n = 0; n < 4; n++)
		{
			//Log.Log(" Joystick Id: "+n+" Value: "+rhPlayer[n]+" Mask: "+rh2InputMask[n]);
			rhPlayer[n]&=plMasks[rhNPlayers][n];
		    rhPlayer[n]&=rh2InputMask[n];

			// Only alter value according the amount of player
			// to avoid reset values that correspond to other frame
			// device joystick is one object per application
			
		    b=rhPlayer[n];
			b &= rh2InputMask[n];
			b ^= rh2OldPlayer[n];

			rh2NewPlayer[n] = b;
			
			if (b != 0)
			{
				b &= rhPlayer[n];
				// Mask to detect buttons and direction
				if ((b & 0xF0) != 0)
				{
					// Message bloquant pour les touches FEU seules
					rhEvtProg.rhCurOi = (short) n;
					b = rh2NewPlayer[n];
					if ((b & 0xF0) != 0)
					{
						rhEvtProg.rhCurParam0 = b;
						rhEvtProg.handle_GlobalEvents(((-4 << 16) | 0xFFF9));	// CNDL_JOYPRESSED);
					}
					// Les autres touches...
					if ((b & 0x0F) != 0)
					{
						rhEvtProg.rhCurParam0 = b;
						rhEvtProg.handle_GlobalEvents(((-4 << 16) | 0xFFF9));	// CNDL_JOYPRESSED);
					}
				}
				else
				{
					// Le reste des touches
					int num = rhEvtProg.listPointers[rhEvtProg.rhEvents[-COI.OBJ_PLAYER] + 4];		// -NUM_JOYPRESSEZD
					if (num != 0)
					{
						rhEvtProg.rhCurParam0 = b;
						rhEvtProg.computeEventList(num, null);
					}
				}
			}
		}


		// Boucle de gestion des objets
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (rhNObjects != 0)					// Nombre d'objets
		{
			int cptObject = rhNObjects;
			int count = 0;
			do
			{
				rh4ObjectAddCreate = 0;
				while (rhObjectList[count] == null)
				{
					count++;
				}
				CObject pObject = rhObjectList[count];

				pObject.hoPrevNoRepeat = pObject.hoBaseNoRepeat;	// Echange les buffers no repeat
				pObject.hoBaseNoRepeat = null;							// RAZ du buffer actuel
				if (pObject.hoCallRoutine)
				{
					rh4ObjectCurCreate = count;		// En cas de create object
					pObject.handle();
				}

				cptObject += rh4ObjectAddCreate;	 	// Un objet nouveau DEVANT le courant
				count++;
				cptObject--;
			} while (cptObject != 0);
		}
		rh3CollisionCount++; 			// Pour la gestion des evenements: on est plus dans la boucle!

		//xtimer = System.currentTimeMillis() - timer;
		//Log.Log("Before Handle Events time: "+xtimer);

		// Appel de tous les evenements
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		rhEvtProg.compute_TimerEvents();			// Evenements timer normaux
		rhEvtProg.handle_TimerEvents();			    // On Timer

		if (rhEvtProg.rhEventAlways)				// Les evenements ALWAYS
		{
			if ((rhGameFlags & GAMEFLAGS_FIRSTLOOPFADEIN) == 0)
			{
				rhEvtProg.computeEventList(0, null);
			}
		}

		// Effectue les evenements pousses
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		rhEvtProg.handle_PushedEvents();

		//xtimer = System.currentTimeMillis() - timer;
		//Log.Log("After Handle Events time: "+xtimer);

		// Modif les objets bouges par les evenements
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		modif_ChangedObjects();

		//xtimer = System.currentTimeMillis() - timer;
		//Log.Log("After Change Objects time: "+xtimer);
		// Call "endOfGameLoop" for extensions
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		eoglList_call();

		// Detruit les objets marques
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~
		destroy_List();

		//xtimer = System.currentTimeMillis() - timer;
		//Log.Log("After Destroy list time: "+xtimer);
		// RAZ du click
		// ~~~~~~~~~~~~
		rhEvtProg.rh2CurrentClick = -1;
		rhEvtProg.rh3CurrentMenu = 0;
		rh4EventCount++;
		rh4FakeKey = 0;
		//	rh4DroppedFlag=0;

		// Affiche l'ecran eventuellement
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		// TODO: iOS doesn't do this (?)
		rh4VBLDelta -= 1;					// Seulement au dernier tour

		if((rhGameFlags & GAMEFLAGS_FIRSTLOOPFADEIN) == 0 && 
				rhApp != null && rhApp.parentApp == null)
		{
			screen_Update();
		}

		//xtimer = System.currentTimeMillis() - timer;
		//Log.Log("After Screen Update time: "+xtimer);

		// Attente 1/50ieme de seconde
		// wait(1);

		// C'est fini!
		// ~~~~~~~~~~~
		if (rhQuit == 0)
		{
			return rhQuitBis;
		}

		// Appel eventuel des evenements fin de niveau
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (rhQuit == LOOPEXIT_NEXTLEVEL ||
				rhQuit == LOOPEXIT_PREVLEVEL ||
				rhQuit == LOOPEXIT_ENDGAME ||
				rhQuit == LOOPEXIT_GOTOLEVEL ||
				rhQuit == LOOPEXIT_QUIT)
		{
			rhEvtProg.handle_GlobalEvents((-2 << 16) | 0xFFFD);
		}
		
		if(pauseEOLCount != -1 && rh4EventCount - pauseEOLCount < 2)
		{
			pauseEOLCount = -1;
			pause();
		}
		
		return rhQuit;
	}

	// Bouge les objets changes par les evenements
	// -------------------------------------------
	public void modif_ChangedObjects()
	{
		int count = 0;
		CObject[] localObjectList = rhObjectList;
		for(CObject pHo : localObjectList) {// Des objets ��� voir?
			if (pHo == null)
				continue;
			count++;
			if((pHo.hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0)
			{
				if (pHo.roc.rcChanged)
				{
					pHo.modif();
					pHo.roc.rcChanged = false;
				}
			}
			if(count >= rhNObjects)
				break;
		}
	}

	//---------------------------------------------------------//
	//	Lecture lecture et clavier pour les 4 joueurs          //
	//---------------------------------------------------------//
	void joyTest(CRunApp app)
	{
		int i;

		int ctrlKeys[][] = app.getCtrlKeys();

		// Raz des entrees des joueurs
		for (i = 0; i < 4; i++)
		{
			rhPlayer[i] = 0;
		}

		// Pour chaque joueur, lire clavier ou joystick
		for (int k = 0; k < CRunApp.MAX_KEY; k++)
		{
			if (app.getKeyState(ctrlKeys[0][k]))
			{

				rhPlayer[0] |= (byte) (1 << k);
			}
		}
	}

	public int getMouseFrameX ()
	{
		return rhApp.mouseX + rhWindowX;
	}

	public int getMouseFrameY ()
	{
		return rhApp.mouseY + rhWindowY;
	}

	// -----------------------------------------------------------------------
	// DETECTION DE COLLISIONS
	// -----------------------------------------------------------------------
	public void newHandle_Collisions(CObject pHo)
	{
		// Raz des flags pour le mouvement
		pHo.rom.rmMoveFlag = false;
		pHo.rom.rmEventFlags = 0;

		// ENTREE /SORTIE DU TERRAIN?
		// ---------------------------------------------------------------------------
		if ((pHo.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKBORDER) != 0)
		{
			// Regarde si on ENTRE dans le terrain
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			int cadran1 = quadran_In(pHo.roc.rcOldX1, pHo.roc.rcOldY1, pHo.roc.rcOldX2, pHo.roc.rcOldY2);
			if (cadran1 != 0)		// Si deja dedans, on ne teste pas
			{
				int cadran2 = quadran_In(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot,
						pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);
				if (cadran2 == 0)		// Teste si entre!
				{
					int chgDir = (cadran1 ^ cadran2);	// Les directions qui ont change!
					if (chgDir != 0)
					{
						pHo.rom.rmEventFlags |= CRMvt.EF_GOESINPLAYFIELD;
						rhEvtProg.rhCurParam0 = chgDir;
						rhEvtProg.handle_Event(pHo, (-11 << 16) | ((pHo.hoType) & 0xFFFF));  // CNDL_EXTINPLAYFIELD
					}
				}
			}

			// Gestion des flags WRAP
			// ~~~~~~~~~~~~~~~~~~~~~~
			int cadran = quadran_In(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot,
					pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);
			if ((cadran & pHo.rom.rmWrapping) != 0)
			{
				boolean oldMoveFlag = pHo.rom.rmMoveFlag;

				if ((cadran & BORDER_LEFT) != 0)
				{
					pHo.rom.rmMovement.setXPosition(pHo.hoX + rhLevelSx);	// Sort a gauche
				}
				else if ((cadran & BORDER_RIGHT) != 0)
				{
					pHo.rom.rmMovement.setXPosition(pHo.hoX - rhLevelSx);	// Sort a droite
				}
				if ((cadran & BORDER_TOP) != 0)
				{
					pHo.rom.rmMovement.setYPosition(pHo.hoY + rhLevelSy);		// Sort en haut
				}
				else if ((cadran & BORDER_BOTTOM) != 0)
				{
					pHo.rom.rmMovement.setYPosition(pHo.hoY - rhLevelSy);		// Sort en bas
				}

				// Fix for bug 3468: rmMoveFlag must not be forced when wrapping (see specific SetXPos / SetYPos in Windows runtime)
				if (pHo.roc.rcMovementType != CMoveDef.MVTYPE_PLATFORM && pHo.roc.rcMovementType != CMoveDef.MVTYPE_EXT)
					pHo.rom.rmMoveFlag = oldMoveFlag;
			}

			// Regarde si on SORT du le terrain
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			cadran1 = quadran_Out(pHo.roc.rcOldX1, pHo.roc.rcOldY1, pHo.roc.rcOldX2, pHo.roc.rcOldY2);
			if (cadran1 != BORDER_ALL)		// Si deja completement dehors, on ne teste pas
			{
				int cadran2 = quadran_Out(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot,
						pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);

				int chgDir = (~cadran1 & cadran2);
				if (chgDir != 0)
				{
					pHo.rom.rmEventFlags |= CRMvt.EF_GOESOUTPLAYFIELD;
					rhEvtProg.rhCurParam0 = chgDir;		// ou LOWORD?
					rhEvtProg.handle_Event(pHo, (-12 << 16) | ((pHo.hoType) & 0xFFFF));  // CNDL_EXTOUTPLAYFIELD
				}
			}
		}

		// COLLISION AVEC DES ELEMENTS DE DECOR
		// ---------------------------------------------------------------------------
		if ((pHo.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKBACK) != 0)
		{
			// Si movement platforme, YVES!
			if (pHo.roc.rcMovementType == CMoveDef.MVTYPE_PLATFORM)
			{
				CMovePlatform platform = (CMovePlatform) pHo.rom.rmMovement;
				platform.mpHandle_Background();
			}
			// Autres mouvements
			// ~~~~~~~~~~~~~~~~~
			else
			{
				int cond = colMask_TestObject_IXY(pHo, pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY, pHo.hoX, pHo.hoY, 0, CColMask.CM_TEST_PLATFORM); // FRAROT
				if (cond != 0)
				{
					rhEvtProg.handle_Event(pHo, cond);
				}
			}
		}

		// COLLISION AVEC DES AUTRES SPRITES
		// ---------------------------------------------------------------------------
		if ((pHo.hoLimitFlags & CObjInfo.OILIMITFLAGS_ONCOLLIDE) != 0)
		{
			short[] pOiColList = null;
			if ( pHo.hoOiList != null )
				pOiColList = pHo.hoOiList.oilColList;
			ArrayList<CObject> cnt = objectAllCol_IXY(pHo, pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY, pHo.hoX, pHo.hoY, pOiColList);
			if (cnt != null)
			{
				int obj;
				int cnt_size = cnt.size();
				for (obj = 0; obj < cnt_size; obj++)
				{
					CObject pHox = cnt.get(obj);
					if ((pHox.hoFlags & CObject.HOF_DESTROYED) == 0)	// Detruit au cycle precedent?
					{
						// Genere la collision, TOUJOURS sur le type de l'objet inferieur
						short type = pHo.hoType;
						CObject pHo_esi = pHo;
						CObject pHo_ebx = pHox;
						if (pHo_esi.hoType > pHo_ebx.hoType)
						{
							pHo_esi = pHox;
							pHo_ebx = pHo;
							type = pHo_esi.hoType;
						}
						rhEvtProg.rhCurParam0 = pHo_ebx.hoOi;
						rhEvtProg.rh1stObjectNumber = pHo_ebx.hoNumber;
						rhEvtProg.handle_Event(pHo_esi, (-14 << 16) | ((type) & 0xFFFF));	// CNDL_EXTCOLLISION
					}
				}
			}
		}

	}

	// ----------------------------------------------
	// Teste les collisions de tous les objets
	// ----------------------------------------------
	// Renvoie le nombre de collisions
	/** Tests the collision for all the objects.
	 */
	public ArrayList<CObject> objectAllCol_IXY(CObject pHo, short newImg, float newAngle, float newScaleX, float newScaleY, int newX, int newY, short oilColList[])
	{
		short nOIL;
		ArrayList<CObject> list = null;

		// Sprite object?
		// --------------
		//CObject pHox;
		CObject[] localObjectList = rhObjectList;
		int count, i;
		int rectX1, rectX2, rectY1, rectY2;
		if ((pHo.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW)) != 0)
		{
			// Collisions with sprites
			if (pHo.roc.rcSprite != null)
			{
				// List of objects to test?
				if ( oilColList != null )
				{
					if ( rhTempSprColList == null )
						rhTempSprColList = new ArrayList<CSprite>();
					for (i=1; i<oilColList.length; i+=2)		// while(oilColList[i] >= 0)
					{
						nOIL = oilColList[i];
						CObjInfo poil = rhOiList[nOIL];
						if ( poil.oilNObjects != 0 )
						{
							short nObl = poil.oilObject;
							do {
								CObject pHo2 = rhObjectList[nObl];
								if ( (pHo2.hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0 &&
									 (pHo2.hoFlags & CObject.HOF_DESTROYED) == 0 && pHo2.hoLayer == pHo.hoLayer )
								{
									if ( pHo2.roc.rcSprite != null && (pHo2.hoFlags & CObject.HOF_DESTROYED) == 0 )
										rhTempSprColList.add(pHo2.roc.rcSprite);
								}

								nObl = pHo2.hoNumNext;
							} while (nObl >= 0);
						}
					}
				}

				// Full list or non empty list? test sprites
				if ( oilColList == null || rhTempSprColList.size() != 0  )
				{
					list = spriteGen.spriteCol_TestSprite_All(pHo.roc.rcSprite, newImg, newX - rhWindowX, newY - rhWindowY, newAngle, newScaleX, newScaleY, 0, (oilColList == null)?null:rhTempSprColList);
					if ( rhTempSprColList != null )
						rhTempSprColList.clear();
				}
			}

			// Les collisions avec des objets d'extension non sprites
			// ------------------------------------------------------
			if ((pHo.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKEXT) != 0)
			{
				short oldHoFlags = pHo.hoFlags;
				pHo.hoFlags |= CObject.HOF_NOCOLLISION;

				rectX1 = newX - pHo.hoImgXSpot;
				rectX2 = rectX1 + pHo.hoImgWidth;
				rectY1 = newY - pHo.hoImgYSpot;
				rectY2 = rectY1 + pHo.hoImgHeight;

				if ( oilColList != null )
				{
					for (i=1; i<oilColList.length; i+=2)		// while(oilColList[i] >= 0)
					{
						nOIL = oilColList[i];
						CObjInfo poil = rhOiList[nOIL];
						if ( poil.oilNObjects != 0 )
						{
							short nObl = poil.oilObject;
							do {
								CObject pHox = rhObjectList[nObl];
								if ( (pHox.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW | CObject.HOF_NOCOLLISION | CObject.HOF_DESTROYED)) == 0 )
								{
									if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
											pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
											pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
											pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
									{
										if (list == null)
											list = new ArrayList<CObject>();
										list.add(pHox);
									}
								}

								nObl = pHox.hoNumNext;
							} while (nObl >= 0);
						}
					}
				}
				else
				{
					count = rhNObjects;
					for(CObject pHox : localObjectList) {
						if(pHox != null) {
						--count;
						// YVES: ajout HOF_OWNERDRAW car les sprites ownerdraw sont geres dans les collisions
						if ((pHox.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW | CObject.HOF_NOCOLLISION | CObject.HOF_DESTROYED)) == 0)
						{
							if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
									pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
									pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
									pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
							{
								if (list == null)
								{
									list = new ArrayList<CObject>();
								}
								list.add(pHox);
							}
						}
						if(count == 0)
							break;
						}
					}
				}

				// Remettre anciens flags
				pHo.hoFlags = oldHoFlags;
			}
		}

		// Non-sprite object
		// -----------------
		else
		{
			if ((pHo.hoFlags & CObject.HOF_NOCOLLISION) == 0)
			{
				short oldHoFlags = pHo.hoFlags;
				pHo.hoFlags |= CObject.HOF_NOCOLLISION;

				rectX1 = newX - pHo.hoImgXSpot;
				rectX2 = rectX1 + pHo.hoImgWidth;
				rectY1 = newY - pHo.hoImgYSpot;
				rectY2 = rectY1 + pHo.hoImgHeight;

				if ( oilColList != null )
				{
					for (i=1; i<oilColList.length; i+=2)		// while(oilColList[i] >= 0)
					{
						nOIL = oilColList[i];
						CObjInfo poil = rhOiList[nOIL];
						if ( poil.oilNObjects != 0 )
						{
							short nObl = poil.oilObject;
							do {
								CObject pHox = rhObjectList[nObl];
								if ((pHox.hoFlags & (CObject.HOF_NOCOLLISION | CObject.HOF_DESTROYED)) == 0)
								{
									if ((pHox.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKCOL) != 0)
									{
										if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
												pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
												pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
												pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
										{
											if (list == null)
											{
												list = new ArrayList<CObject>();
											}
											list.add(pHox);
										}
									}
								}

								nObl = pHox.hoNumNext;
							} while (nObl >= 0);
						}
					}
				}
				else
				{
					count = rhNObjects;
					for(CObject pHox : localObjectList) {
						if(pHox != null) {
							--count;
							if ((pHox.hoFlags & (CObject.HOF_NOCOLLISION | CObject.HOF_DESTROYED)) == 0)
							{
								if ((pHox.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKCOL) != 0)
								{
									if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
											pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
											pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
											pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
									{
										if (list == null)
										{
											list = new ArrayList<CObject>();
										}
										list.add(pHox);
									}
								}
							}
							if(count == 0)
								break;
						}
					}
				}
				// Remettre anciens flags
				pHo.hoFlags = oldHoFlags;
			}
		}
		return list;
	}

	// ----------------------------------------------
	// Teste les collisions d'un objet avec le decor
	// Si collision, retourne la condition COLBACK
	// ----------------------------------------------
	public int colMask_TestObject_IXY(CObject pHo, short newImg, float newAngle, float newScaleX, float newScaleY, int newX, int newY, int htfoot, int plan)
	{
		int res = 0;
		int x = newX - rhWindowX;
		int y = newY - rhWindowY;

		//Log.Log("colMask Test Object x: "+x+" y: "+y+" newX: "+newX+" newY: "+newY+" Wx: "+rhWindowX+" Wy: "+rhWindowY);

		boolean bSprite = false;

		if ((pHo.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW)) != 0)
		{
			if ((pHo.ros.rsCreaFlags & CSprite.SF_COLBOX) == 0)
			{
				bSprite = true;
			}
		}

		if (bSprite)
		{
			// UN OBJET SPRITE
			CSprite pSpr = pHo.roc.rcSprite;
			if (pSpr != null)
			{
				if (rhFrame.bkdCol_TestSprite(pSpr, newImg, x, y, newAngle, newScaleX, newScaleY, htfoot, plan))
				{
					res = ((-13 << 16) | ((pHo.hoType) & 0xFFFF));	    // CNDL_EXTCOLBACK
				}
			}
		}
		else
		{
			x -= pHo.hoImgXSpot;
			y -= pHo.hoImgYSpot;

			// UN OBJET EXTENSION
			if (htfoot != 0)
			{
				y += pHo.hoImgHeight;
				y -= htfoot;
				if (rhFrame.bkdCol_TestRect(x, y, pHo.hoImgWidth, htfoot, pHo.hoLayer, plan))
				{
					res = ((-13 << 16) | ((pHo.hoType) & 0xFFFF));	    // CNDL_EXTCOLBACK
				}
			}
			else
			{
				if (rhFrame.bkdCol_TestRect(x, y, pHo.hoImgWidth, pHo.hoImgHeight, pHo.hoLayer, plan))
				{
					res = ((-13 << 16) | ((pHo.hoType) & 0xFFFF));	    // CNDL_EXTCOLBACK
				}
			}
		}
		return res;
	}


	// ---------------------------------------------------------------------------
	// Routine: retourne le quadran pointe par AX/BX lors de la sortie d'un sprite
	// ---------------------------------------------------------------------------
	public int quadran_Out(int x1, int y1, int x2, int y2)
	{
		int cadran = 0;
		if (x1 < 0)
		{
			cadran |= BORDER_LEFT;
		}
		if (y1 < 0)
		{
			cadran |= BORDER_TOP;
		}
		if (x2 > rhLevelSx)
		{
			cadran |= BORDER_RIGHT;
		}
		if (y2 > rhLevelSy)
		{
			cadran |= BORDER_BOTTOM;
		}
		return Table_InOut[cadran];
	}

	// ---------------------------------------------------------------------------
	// Routine: retourne le quadran pointe par AX/BX lors de l'entree d'un sprite
	// ---------------------------------------------------------------------------
	public int quadran_In(int x1, int y1, int x2, int y2)
	{
		int cadran = 15;
		if (x1 < rhLevelSx)
		{
			cadran &= ~BORDER_RIGHT;
		}
		if (y1 < rhLevelSy)
		{
			cadran &= ~BORDER_BOTTOM;
		}
		if (x2 > 0)
		{
			cadran &= ~BORDER_LEFT;
		}
		if (y2 > 0)
		{
			cadran &= ~BORDER_TOP;
		}
		return Table_InOut[cadran];
	}

	// ---------------------------------------------------------------------------
	// Generateur aleatoire, entree AX= chiffre maxi
	// ---------------------------------------------------------------------------
	// change from short to int to fit correctly the value
	public int random(short wMax)
	{
		int iMax = (int)wMax & 0xFFFF;
		int calcul = rh3Graine * 31415 + 1;
		calcul &= 0x0000FFFF;
		rh3Graine = calcul;
		return (int)(((calcul * iMax) >>> 16) & 0xFFFF);
	}    

	// ----------------------------------
	// Interprete un parametre DIRECTION
	// ----------------------------------
	public int get_Direction(int dir)
	{
		if (dir == 0 || dir == -1)
		{
			// Au hasard parmi les 32
			// ~~~~~~~~~~~~~~~~~~~~~~
			return random((short) 32);
		}

		// Compte le nombre de directions demandees
		int loop;
		int found = 0;
		int count = 0;
		int dirShift = dir;
		for (loop = 0; loop < 32; loop++)
		{
			if ((dirShift & 1) != 0)
			{
				count++;
				found = loop;
			}
			dirShift >>>= 1;
		}

		// Une direction?
		// ~~~~~~~~~~~~~~
		if (count == 1)
		{
			return found;
		}

		// Au hasard
		// ~~~~~~~~~
		count = random((short) count);
		dirShift = dir;
		for (loop = 0; loop < 32; loop++)
		{
			if ((dirShift & 1) != 0)
			{
				count--;
				if (count < 0)
				{
					return loop;
				}
			}
			dirShift >>>= 1;
		}
		return 0;
	}

	// -----------------------------------------------------------------------
	// EVALUATION D'EXPRESSION
	// -----------------------------------------------------------------------

	// Note: if possible do not use this one as it creates a CValue
	public CValue get_EventExpressionAny(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return new CValue(getExpression());
	}

	public CValue get_EventExpressionAny_WithoutNewValue(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression();
	}

	public int get_EventExpressionInt(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression().getInt();
	}

	public double get_EventExpressionDouble(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression().getDouble();
	}

	public String get_EventExpressionString(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression().getString();
	}

	public String get_EventExpressionStringLowercase (CParamExpression pExp)
	{
		if (pExp.tokens.length == 2 && pExp.tokens [0] instanceof EXP_STRING)
			return ((EXP_STRING) pExp.tokens [0]).getLowercase ();

		return get_EventExpressionString (pExp).toLowerCase();
	}

	public int get_ExpressionInt()
	{
		return getExpression().getInt();
	}

	public double get_ExpressionDouble()
	{
		return getExpression().getDouble();
	}

	public String get_ExpressionString()
	{
		return getExpression().getString();
	}

	// Note: if possible do not use this one as it creates a CValue
	public CValue get_ExpressionAny()
	{
		CValue ret=new CValue(getExpression());
		return ret;
	}

	public void evaluateAsLowercase (CExp token)
	{
		if (token instanceof EXP_STRING)
			((EXP_STRING) token).evaluateAsLowercase(this);
		else
		{
			token.evaluate (this);

			CValue currentResult = rh4Results[rh4PosPile];

			if (currentResult.type == CValue.TYPE_STRING)
				currentResult.forceString (currentResult.getString ().toLowerCase ());
		}
	}

	public String get_ExpressionStringLowercase()
	{
		CExp ope;
		int pileStart = rh4PosPile;
		rh4Operators[rh4PosPile] = rh4OpeNull;
		do
		{
			rh4PosPile++;
			bOperande=true;
			evaluateAsLowercase (rh4Tokens[rh4CurToken]);
			bOperande=false;
			rh4CurToken++;

			// Regarde l'operateur
			do
			{
				ope = rh4Tokens[rh4CurToken];
				if (ope.code > 0 && ope.code < 0x00140000)	// (OPERATOR_START && ope<OPERATOR_END)
				{
					if (ope.code > rh4Operators[rh4PosPile - 1].code)
					{
						rh4Operators[rh4PosPile] = ope;
						rh4CurToken++;

						rh4PosPile++;
						bOperande=true;

						evaluateAsLowercase (rh4Tokens[rh4CurToken]);

						bOperande=false;
						rh4CurToken++;
					}
					else
					{
						rh4PosPile--;
						evaluateAsLowercase (rh4Operators[rh4PosPile]);
					}
				}
				else
				{
					rh4PosPile--;
					if (rh4PosPile == pileStart)
					{
						break;
					}
					evaluateAsLowercase (rh4Operators[rh4PosPile]);
				}
			} while (true);
		} while (rh4PosPile > pileStart + 1);

		return rh4Results[pileStart + 1].getString();
	}
	public CValue getExpression()
	{
		CExp ope;
		int pileStart = rh4PosPile;
		rh4Operators[rh4PosPile] = rh4OpeNull;
		do
		{
			rh4PosPile++;
			bOperande=true;
			rh4Tokens[rh4CurToken].evaluate(this);
			bOperande=false;
			rh4CurToken++;

			// Regarde l'operateur
			do
			{
				ope = rh4Tokens[rh4CurToken];
				if (ope.code > 0 && ope.code < 0x00140000)	// (OPERATOR_START && ope<OPERATOR_END)
				{
					if (ope.code > rh4Operators[rh4PosPile - 1].code)
					{
						rh4Operators[rh4PosPile] = ope;
						rh4CurToken++;

						rh4PosPile++;
						bOperande=true;
						rh4Tokens[rh4CurToken].evaluate(this);
						bOperande=false;
						rh4CurToken++;
					}
					else
					{
						rh4PosPile--;
						rh4Operators[rh4PosPile].evaluate(this);
					}
				}
				else
				{
					rh4PosPile--;
					if (rh4PosPile == pileStart)
					{
						break;
					}
					rh4Operators[rh4PosPile].evaluate(this);
				}
			} while (true);
		} while (rh4PosPile > pileStart + 1);

		return rh4Results[pileStart + 1];
	}

	public CValue getPreviousResult()
	{
		return rh4Results[rh4PosPile - 1];
	}

	public CValue getNextResult()
	{
		return rh4Results[rh4PosPile + 1];
	}


	// ROUTINE DE COMPARAISON GENERALE
	// -------------------------------
	public static boolean compareTo(CValue pValue1, CValue pValue2, short comp)
	{
		switch (comp)
		{
		case 0:	// COMPARE_EQ:
			return pValue1.equal(pValue2);
		case 1:	// COMPARE_NE:
			return pValue1.notEqual(pValue2);
		case 2:	// COMPARE_LE:
			return pValue1.lower(pValue2);
		case 3:	// COMPARE_LT:
			return pValue1.lowerThan(pValue2);
		case 4:	// COMPARE_GE:
			return pValue1.greater(pValue2);
		case 5:	// COMPARE_GT:
			return pValue1.greaterThan(pValue2);
		}
		return false;
	}

	// Comparaison de deux entiers
	// ---------------------------
	public static boolean compareTer(int value1, int value2, short comparaison)
	{
		switch (comparaison)
		{
		case 0:	// COMPARE_EQ:
			return (value1 == value2);
		case 1:	// COMPARE_NE:
			return (value1 != value2);
		case 2:	// COMPARE_LE:
			return (value1 <= value2);
		case 3:	// COMPARE_LT:
			return (value1 < value2);
		case 4:	// COMPARE_GE:
			return (value1 >= value2);
		case 5:	// COMPARE_GT:
			return (value1 > value2);
		}
		return false;
	}

	// Compare integer with value
	public static boolean compareIntTo(int value, CValue pValue2, short comp)
	{
		switch (comp)
		{
		case 0:	// COMPARE_EQ:
			return pValue2.equal(value);
		case 1:	// COMPARE_NE:
			return pValue2.notEqual(value);
		case 2:	// COMPARE_LE:
			return pValue2.greater(value);		// opposed operators as values are compared in the opposed direction
		case 3:	// COMPARE_LT:
			return pValue2.greaterThan(value);
		case 4:	// COMPARE_GE:
			return pValue2.lower(value);
		case 5:	// COMPARE_GT:
			return pValue2.lowerThan(value);
		}
		return false;
	}

	public static boolean compareToInt(CValue pValue1, int value2, short comp)
	{
		switch (comp)
		{
		case 0:	// COMPARE_EQ:
			return pValue1.equal(value2);
		case 1:	// COMPARE_NE:
			return pValue1.notEqual(value2);
		case 2:	// COMPARE_LE:
			return pValue1.lower(value2);
		case 3:	// COMPARE_LT:
			return pValue1.lowerThan(value2);
		case 4:	// COMPARE_GE:
			return pValue1.greater(value2);
		case 5:	// COMPARE_GT:
			return pValue1.greaterThan(value2);
		}
		return false;
	}

	public static boolean compareToDouble(CValue pValue1, double value2, short comp)
	{
		switch (comp)
		{
		case 0:	// COMPARE_EQ:
			return pValue1.equal(value2);
		case 1:	// COMPARE_NE:
			return pValue1.notEqual(value2);
		case 2:	// COMPARE_LE:
			return pValue1.lower(value2);
		case 3:	// COMPARE_LT:
			return pValue1.lowerThan(value2);
		case 4:	// COMPARE_GE:
			return pValue1.greater(value2);
		case 5:	// COMPARE_GT:
			return pValue1.greaterThan(value2);
		}
		return false;
	}

	// Compare string with string
	public static boolean compareStringToString(String str1, String str2, short comp)
	{
		int cmpResult = str1.compareTo(str2);
		switch(comp) {
		case 0:		// COMPARE_EQ:
			return cmpResult==0;
		case 1:		// COMPARE_NE:
			return cmpResult!=0;
		case 2:		// VALUE_COMPARE_LE:
			return cmpResult<=0;
		case 3:		// VALUE_COMPARE_LT:
			return cmpResult<0;
		case 4:		// VALUE_COMPARE_GE:
			return cmpResult>=0;
		case 5:		// VALUE_COMPARE_GT:
			return cmpResult>0;
		}
		return false;
	}

	// ------------------------------------------------------------------------
	// MISE A JOUR DES OBJETS PLAYER
	// ------------------------------------------------------------------------
	public void update_PlayerObjects(int joueur, short type, int value)
	{
		joueur++;

		int count = 0;
		for (int no = 0; no < rhNObjects; no++)
		{
			while (rhObjectList[count] == null)
			{
				count++;
			}
			CObject pHo = rhObjectList[count];
			if (pHo.hoType == type)
			{
				switch (type)
				{
				case 5:	// OBJ_SCORE
					CScore pScore = (CScore) pHo;
					if (pScore.rsPlayer == joueur)
					{
						pScore.rsValue.forceInt(value);
					}
					break;
				case 6:	// OBJ_LIVES
					CLives pLife = (CLives) pHo;
					if (pLife.rsPlayer == joueur)
					{
						pLife.rsValue.forceInt(value);
					}
					break;
				}
				pHo.roc.rcChanged = true;
				pHo.modif();
			}
			count++;
		}
	}
	// Termine les vies, genere les evenements PLUS DE VIE
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void actPla_FinishLives(int joueur, int live)
	{
		int lives[] = rhApp.getLives();
		if (live == lives[joueur])
		{
			return;
		}
		
		// Nouvelle vie=0?
		if (live == 0)
		{
			if (lives[joueur] != 0)
			{
				rhEvtProg.push_Event(0, ((-5 << 16) | 0xFFF9), 0, null, (short) joueur);
			}
		}

		// Change les objets...
		lives[joueur] = live;
		update_PlayerObjects(joueur, COI.OBJ_LIVES, live);
	}

	// -----------------------------------
	// CONDITION: mouse lays on objects
	// -----------------------------------
	public boolean getMouseOnObjectsEDX(short oiList, boolean nega)
	{
		// Des objets a voir?
		// ------------------
		CObject pHo = rhEvtProg.evt_FirstObject(oiList);
		if (pHo == null)
		{
			if (nega)
			{
				return true;
			}
			return false;
		}
		int cpt = rhEvtProg.evtNSelectedObjects;

		// Demande les collisions des sprites
		// ----------------------------------
		int x = rhApp.mouseX;
		int y = rhApp.mouseY;
		ArrayList<CObject> list = new ArrayList<CObject>();
		CSprite curSpr = spriteGen.spriteCol_TestPoint(null, CSpriteGen.LAYER_ALL, x, y, 0);
		CObject pHoFound;
		while (curSpr != null)
		{
			pHoFound = curSpr.sprExtraInfo;
			if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)		//; Detruit au cycle precedent?
			{
				list.add(pHoFound);
			}
			curSpr = spriteGen.spriteCol_TestPoint(curSpr, CSpriteGen.LAYER_ALL, x, y, 0);
		}

		// Demande les collisions des autres objets
		// ----------------------------------------
		int count = 0;
		int no;
		for (no = 0; no < rhNObjects; no++)		// Des objets ��� voir?
		{
			while (rhObjectList[count] == null)
			{
				count++;
			}
			pHoFound = rhObjectList[count];
			count++;
			if ((pHoFound.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_NOCOLLISION)) == 0)
			{
				int x1 = pHoFound.hoX - rhWindowX - pHoFound.hoImgXSpot;
				int x2 = x1 + pHoFound.hoImgWidth;
				int y1 = pHoFound.hoY - rhWindowY - pHoFound.hoImgYSpot;
				int y2 = y1 + pHoFound.hoImgHeight;
				if (x >= x1 && x < x2 && y >= y1 && y < y2)
				{
					if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)		//; Detruit au cycle precedent?
					{
						list.add(pHoFound);
					}
				}
			}
		}

		// Demande les objets selectionnes
		// -------------------------------
		if (list.size() == 0)
		{
			if (nega)
			{
				return true;
			}
			return false;
		}

		if (nega == false)
		{
			do
			{
				int list_size = list.size();
				for (count = 0; count < list_size; count++)
				{
					pHoFound = list.get(count);
					if (pHoFound == pHo)
					{
						break;
					}
				}
				if (count == list.size())
				{
					cpt--;						//; Pas trouve dans la liste. on le vire
					rhEvtProg.evt_DeleteCurrentObject();
				}
				pHo = rhEvtProg.evt_NextObject();
			} while (pHo != null);
			return cpt != 0;
		}
		else
		{
			// Avec negation
			do
			{
				int list_size = list.size();
				for (count = 0; count < list_size; count++)
				{
					pHoFound = list.get(count);
					if (pHoFound == pHo)
					{
						return false;
					}
				}
				pHo = rhEvtProg.evt_NextObject();
			} while (pHo != null);
			return true;
		}
	}

	// ------------------------------------------------------------------------
	// ROUTINES OBJETS TEXTE
	// ------------------------------------------------------------------------

	// PROCEDURE D'AFFICHAGE DE UN TEXTE DONT AUQUEL C'EST ALORS
	public int txtDisplay(CEvent pe, short oi, int txtNumber)
	{
		// Cherche la position de creation
		PARAM_CREATE pEvp = (PARAM_CREATE) pe.evtParams[0];
		CPositionInfo pInfo = new CPositionInfo();
		if (pEvp.read_Position(this, 0x10, pInfo))
		{
			// Regarde si le meme text n'existe pas deja
			int count = 0;
			CObject[] localObjectList = rhObjectList;
			for(CObject pHo : localObjectList) {
				if(pHo == null)
					continue;

				count++;
				if (pHo.hoType == COI.OBJ_TEXT && pHo.hoOi == oi && pHo.hoX == pInfo.x && pHo.hoY == pInfo.y)
				{
					// Le texte existe deja a la meme position, SECURITE, on fait un SET TEXT
					pHo.ros.obShow();					// On le montre!
					pHo.hoFlags &= ~CObject.HOF_NOCOLLISION;		//; Des collisions de nouveau
					CText pText = (CText) pHo;
					pText.rsMini = -2;					// Force la copie
					pText.txtChange(txtNumber);
					pHo.roc.rcChanged = true;
					pHo.display();
					pHo.ros.rsFlash = 0;							//; Arrete le flash!
					pHo.ros.rsFlags |= CRSpr.RSFLAG_VISIBLE;
					return pHo.hoNumber;
				}
				if(count >= rhNObjects)
					break;
			}
			// Cree l'objet
			int num = f_CreateObject((short) -1, oi, pInfo.x, pInfo.y, 0, (short) 0, rhFrame.nLayers - 1, -1);
			if (num >= 0)
			{
				// Poke le numero du texte dans la structure
				((CText) rhObjectList[num]).txtChange(txtNumber);
				return num;
			}
		}
		return -1;
	}

	// APPEL DE LA CREATION DE TOUS LES TEXTE
	public int txtDoDisplay(CEvent pe, int txtNumber)
	{
		if (pe.evtOiList >= 0)
		{
			return txtDisplay(pe, pe.evtOi, txtNumber);
		}

		// Un qualifier: on explore les listes
		if (pe.evtOiList == -1)
		{
			return -1;
		}
		int qoi = pe.evtOiList & 0x7FFF;
		CQualToOiList qoil = rhEvtProg.qualToOiList[qoi];
		int count = 0;
		while (count < qoil.qoiList.length)
		{
			txtDisplay(pe, qoil.qoiList[count], txtNumber);
			count += 2;
		}
		;
		return -1;
	}

	// Gestion generique fontes / objets
	public static CFontInfo getObjectFont(CObject hoPtr)
	{
		CFontInfo info = null;

		if (hoPtr.hoType >= COI.KPX_BASE)
		{
			CExtension e = (CExtension) hoPtr;
			info = e.ext.getRunObjectFont();
		}
		else
		{
			switch (hoPtr.hoType)
			{
			case 3:		// OBJ_TEXT:
				CText pText = (CText) hoPtr;
				info = pText.getFont();
				break;
			case 5:		// OBJ_SCORE:
				CScore pScore = (CScore) hoPtr;
				info = pScore.getFont();
				break;
			case 6:		// OBJ_LIVES:
				CLives pLives = (CLives) hoPtr;
				info = pLives.getFont();
				break;
			case 7:		// OBJ_COUNTER:
				CCounter pCounter = (CCounter) hoPtr;
				info = pCounter.getFont();
				break;
			}
		}
		if (info == null)
		{
			info = new CFontInfo();
		}
		return info;
	}

	public static void setObjectFont(CObject hoPtr, CFontInfo pLf, CRect pNewSize)
	{
		if (hoPtr.hoType >= COI.KPX_BASE)
		{
			CExtension e = (CExtension) hoPtr;
			e.ext.setRunObjectFont(pLf, pNewSize);
		}
		else
		{
			switch (hoPtr.hoType)
			{
			case 3:		// OBJ_TEXT:
				CText pText = (CText) hoPtr;
				pText.setFont(pLf, pNewSize);
				break;
			case 5:		// OBJ_SCORE:
				CScore pScore = (CScore) hoPtr;
				pScore.setFont(pLf, pNewSize);
				break;
			case 6:		// OBJ_LIVES:
				CLives pLives = (CLives) hoPtr;
				pLives.setFont(pLf, pNewSize);
				break;
			case 7:		// OBJ_COUNTER:
				CCounter pCounter = (CCounter) hoPtr;
				pCounter.setFont(pLf, pNewSize);
				break;
			}
		}
	}

	public static int getObjectTextColor(CObject hoPtr)
	{
		if (hoPtr.hoType >= COI.KPX_BASE)
		{
			CExtension e = (CExtension) hoPtr;
			return e.ext.getRunObjectTextColor();
		}
		switch (hoPtr.hoType)
		{
		case 3:		// OBJ_TEXT:
			CText pText = (CText) hoPtr;
			return pText.getFontColor();
		case 5:		// OBJ_SCORE:
			CScore pScore = (CScore) hoPtr;
			return pScore.getFontColor();
		case 6:		// OBJ_LIVES:
			CLives pLives = (CLives) hoPtr;
			return pLives.getFontColor();
		case 7:		// OBJ_COUNTER:
			CCounter pCounter = (CCounter) hoPtr;
			return pCounter.getFontColor();
		}
		return 0;
	}

	public static void setObjectTextColor(CObject hoPtr, int rgb)
	{
		if (hoPtr.hoType >= COI.KPX_BASE)
		{
			CExtension e = (CExtension) hoPtr;
			e.ext.setRunObjectTextColor(rgb);
		}
		else
		{
			switch (hoPtr.hoType)
			{
			case 3:		// OBJ_TEXT:
				CText pText = (CText) hoPtr;
				pText.setFontColor(rgb);
				break;
			case 5:		// OBJ_SCORE:
				CScore pScore = (CScore) hoPtr;
				pScore.setFontColor(rgb);
				break;
			case 6:		// OBJ_LIVES:
				CLives pLives = (CLives) hoPtr;
				pLives.setFontColor(rgb);
				break;
			case 7:		// OBJ_COUNTER:
				CCounter pCounter = (CCounter) hoPtr;
				pCounter.setFontColor(rgb);
				break;
			}
		}
	}

	// SET POSITION OF OBJECTS
	// -----------------------------------------------------------------------
	public static void setXPosition(CObject hoPtr, int x)
	{
		if (hoPtr.rom != null)
		{
			hoPtr.rom.rmMovement.setXPosition(x);
		}
		else
		{
			if (hoPtr.hoX != x)
			{
				hoPtr.hoX = x;
				if (hoPtr.roc != null)
				{
					hoPtr.roc.rcChanged = true;
					hoPtr.roc.rcCheckCollides = true;
				}
			}
		}
	}

	public static void setYPosition(CObject hoPtr, int y)
	{
		if (hoPtr.rom != null)
		{
			hoPtr.rom.rmMovement.setYPosition(y);
		}
		else
		{
			if (hoPtr.hoY != y)
			{
				hoPtr.hoY = y;
				if (hoPtr.roc != null)
				{
					hoPtr.roc.rcChanged = true;
					hoPtr.roc.rcCheckCollides = true;
				}
			}
		}
	}

	//; Trouve une direction ��� partir d'une pente AX/BX . AX
	// -----------------------------------------------------
	public static int get_DirFromPente(int x, int y)
	{
		if (x == 0)							// Si nul en X
		{
			if (y >= 0)
			{
				return 24;	    // DIRID_S;
			}
			return 8;			    // DIRID_N;
		}
		if (y == 0)							// Si nul en Y
		{
			if (x >= 0)
			{
				return 0;		    // DIRID_E;
			}
			return 16;			    // DIRID_W;
		}

		int dir;
		boolean flagX = false, flagY = false;		// Flags de signe
		if (x < 0)							// DX negatif?
		{
			flagX = true;
			x = -x;
		}
		if (y < 0)							// DX negatif?
		{
			flagY = true;
			y = -y;
		}

		int d = (x * 256) / y;					// Calcul de la pente *256 pour plus de precision
		int index;
		for (index = 0;; index += 2)
		{
			if (d >= CMove.CosSurSin32[index])
			{
				break;
			}
		}
		dir = CMove.CosSurSin32[index + 1];			//; Charge la direction

		if (flagY)
		{
			dir = -dir + 32;						//; R���tablir en Y
			dir &= 31;
		}
		if (flagX)
		{
			dir -= 8;								//; Retablir en X
			dir &= 31;
			dir = -dir;
			dir &= 31;
			dir += 8;
			dir &= 31;
		}
		return dir;
	}

	//  --------------------------------------------------------------------------
	//	ANIMATION DISAPPEAR
	//  --------------------------------------------------------------------------
	public void init_Disappear(CObject hoPtr)
	{
		if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_ANIMATIONS) != 0)
		{
			if (hoPtr.roa.anim_Exist(CAnim.ANIMID_DISAPPEAR))
			{
				// Branche la fausse routine de mouvement
				if (hoPtr.roc.rcSprite != null)
				{
					hoPtr.roc.rcSprite.setSpriteColFlag(0);
				}
				if (hoPtr.rom != null)
				{
					hoPtr.rom.initSimple(hoPtr, CMoveDef.MVTYPE_DISAPPEAR, false);
					hoPtr.roc.rcSpeed = 0;
				}
				hoPtr.roa.animation_Force(CAnim.ANIMID_DISAPPEAR);
				hoPtr.roa.animation_OneLoop();
				return;
			}
		}

		// Rien du tout. on detruit le sprite!
		hoPtr.hoCallRoutine = false;
		destroy_Add(hoPtr.hoNumber);
	}

	// -------------------------------------------------------------------------
	// STOCKAGE GLOBAL POUR LES EXTENSIONS
	// -------------------------------------------------------------------------
	public CExtStorage getStorage(int id)
	{
		if (rhApp.extensionStorage != null)
		{
			int n;
			int rhApp_extensionStorage_size = rhApp.extensionStorage.size();
			for (n = 0; n < rhApp_extensionStorage_size; n++)
			{
				CExtStorage e = rhApp.extensionStorage.get(n);
				if (e.id == id)
				{
					return e;
				}
			}
		}
		return null;
	}

	public void delStorage(int id)
	{
		if (rhApp.extensionStorage != null)
		{
			int n;
			int rhApp_extensionStorage_size = rhApp.extensionStorage.size();
			for (n = 0; n < rhApp_extensionStorage_size; n++)
			{
				CExtStorage e = rhApp.extensionStorage.get(n);
				if (e.id == id)
				{
					rhApp.extensionStorage.remove(n);
					rhApp_extensionStorage_size = rhApp.extensionStorage.size();
					n--;
				}
			}
		}
	}

	public void addStorage(CExtStorage data, int id)
	{
		CExtStorage e = getStorage(id);
		if (e == null)
		{
			if (rhApp.extensionStorage == null)
			{
				rhApp.extensionStorage = new ArrayList<CExtStorage>();
			}
			data.id = id;
			rhApp.extensionStorage.add(data);
		}
	}

	// -------------------------------------------------------------------------
	// Conditions en +
	// -------------------------------------------------------------------------
	public boolean isMouseOn()
	{
		return true;
	}

	public static void objectHide(CObject pHo)
	{
		if (pHo.ros != null)
		{
			pHo.ros.obHide();
			pHo.ros.rsFlags &= ~CRSpr.RSFLAG_VISIBLE;
			pHo.ros.rsFlash = 0;
		}
	}

	public static void objectShow(CObject pHo)
	{
		if (pHo.ros != null)
		{
			pHo.ros.obShow();
			pHo.ros.rsFlags |= CRSpr.RSFLAG_VISIBLE;
			pHo.ros.rsFlash = 0;
		}
	}

	public void setFrameRate(int value)
	{
		//Log.Log("... setting Frame Rate ...");
		if (value >= 1 && value <= 1000)
		{
			// Get top-level application
			CRunApp app = this.rhApp;
			while (app.parentApp != null)
			{
				app = app.parentApp;
			}

			// Set new frame rate
			app.gaFrameRate = value;

			// Update reference time
			app.refTime = System.currentTimeMillis() - (int) (((double) rhVBLOld * 1000.0f) / app.gaFrameRate);

			MMFRuntime.inst.setFrameRate(value);
		}
	}

	public int getRGBAt(CObject hoPtr, int x, int y)
	{
		int rgb = 0;
		if (hoPtr.roc.rcImage != -1)
		{
			CImage image = rhApp.imageBank.getImageFromHandle(hoPtr.roc.rcImage);
			if(image != null) {
				rgb = image.getPixel (x, y);
			}
		}
		return rgb;
	}

	public String getTempPath()
	{
		return MMFRuntime.inst.getCacheDir().getAbsolutePath();
	}

	public void reinitDisplay ()
	{
		if (rhApp.joystick != null)
			rhApp.joystick.loadImages ();

		synchronized (rhApp.imageBank) {
             rhApp.imageBank.load();

			int cntImage = 0;
			while(!rhApp.imageBank.loaded() || cntImage > 10000)
			{
				SystemClock.sleep(10);
				cntImage++;
			}
		}

		if (rhNObjects != 0)
		{
			int cptObject = rhNObjects;
			for(CObject hoPtr : rhObjectList) {
				if(hoPtr != null) {
					hoPtr.reinitDisplay();
					-- cptObject;
					if(cptObject == 0)
						break;
				}
			}
		}
	}

	public int getDir(CObject hoPtr)
	{
		if (hoPtr.rom != null)
			return hoPtr.rom.rmMovement.getDir();
		return hoPtr.roc.rcDir;
	}

	public void CreateBodies()
	{
		CRunBaseParent pBase = this.GetBase();
		if (pBase == null)
			return;

		int nObjects=rhNObjects;
		CObject[] localObjectList =rhObjectList;
		//Log.Log("Create Physics Objects");
		//long timer = System.currentTimeMillis();
		for (CObject pHo : localObjectList)
		{
			if (pHo != null) {

				--nObjects;
				if(pHo.hoType>=32)
				{
					if (pHo.hoCommon.ocIdentifier==CRun.FANIDENTIFIER
							|| pHo.hoCommon.ocIdentifier==CRun.TREADMILLIDENTIFIER
							|| pHo.hoCommon.ocIdentifier == CRun.PARTICULESIDENTIFIER
							|| pHo.hoCommon.ocIdentifier == CRun.ROPEANDCHAINIDENTIFIER
							|| pHo.hoCommon.ocIdentifier == CRun.MAGNETIDENTIFIER
							|| pHo.hoCommon.ocIdentifier==CRun.BASEIDENTIFIER
							)
					{
						CRunBaseParent pBaseParent = (CRunBaseParent)((CExtension)pHo).ext;
						pBaseParent.rStartObject();
					}
				}
				if(nObjects == 0)
					break;
			}
		}
		//Log.Log("time for make it: "+(System.currentTimeMillis()-timer));

		nObjects=rhNObjects;
		//Log.Log("Create Movements");
		//timer = System.currentTimeMillis();
		for (CObject pHo : localObjectList)	
		{
			if (pHo != null) {
				--nObjects;
				if((pHo.hoOEFlags&CObjectCommon.OEFLAG_MOVEMENTS)!=0)
				{
					Boolean flag = false;
					if (pHo.roc.rcMovementType==CMoveDef.MVTYPE_EXT)
					{
						CMoveDefExtension mvPtr= (CMoveDefExtension)pHo.hoCommon.ocMovements.moveList[pHo.rom.rmMvtNum];
						if (mvPtr.moduleName.equalsIgnoreCase("box2d8directions")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dspring")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dspaceship")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dstatic")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dracecar")
								|| mvPtr.moduleName.equalsIgnoreCase("box2daxial")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dplatform")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dbouncingball")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dbackground")
								)
						{
							CMoveExtension pMoveExtension = (CMoveExtension)pHo.rom.rmMovement;
							CRunMBase pRunMBase = (CRunMBase)pMoveExtension.movement;
							pRunMBase.CreateBody();
							flag = true;
						}
					}
					if (flag == false && pHo.hoType == 2)
					{
						//Log.Log("Adding object: "+pHo.hoOiList.oilName);
						//timer = System.currentTimeMillis();
						pBase.rAddNormalObject(pHo);
						//Log.Log("time for make it: "+(System.currentTimeMillis()-timer));
					}
				}
				if(nObjects == 0)
					break;
			}
		}
		//Log.Log("time for make it: "+(System.currentTimeMillis()-timer));

		nObjects=rhNObjects;	
		//Log.Log("Create Movements Joint");
		//timer = System.currentTimeMillis();
		for (CObject pHo : localObjectList)	
		{
			if (pHo != null) {
				nObjects--;
				if((pHo.hoOEFlags&CObjectCommon.OEFLAG_MOVEMENTS)!=0)
				{
					//Boolean flag = false;
					if (pHo.roc.rcMovementType==CMoveDef.MVTYPE_EXT)
					{
						CMoveDefExtension mvPtr= (CMoveDefExtension)pHo.hoCommon.ocMovements.moveList[pHo.rom.rmMvtNum];
						if (mvPtr.moduleName.equalsIgnoreCase("box2d8directions")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dspring")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dspaceship")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dstatic")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dracecar")
								|| mvPtr.moduleName.equalsIgnoreCase("box2daxial")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dplatform")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dbouncingball")
								|| mvPtr.moduleName.equalsIgnoreCase("box2dbackground")
								)
						{
							CMoveExtension pMoveExtension = (CMoveExtension)pHo.rom.rmMovement;
							CRunMBase pRunMBase = (CRunMBase)pMoveExtension.movement;
							pRunMBase.CreateJoint();
						}
					}
				}
				if(nObjects == 0)
					break;
			}
		}
		//Log.Log("time for make it: "+(System.currentTimeMillis()-timer));

	}

	public CRunBaseParent GetBase()
	{
		if (rh4Box2DSearched == false)
		{
			rh4Box2DSearched = true;
			rh4Box2DBase = null;

			int nObjects=rhNObjects;
			CObject[] localObjectList = rhObjectList;
			for (CObject pHo : localObjectList)
			{
				if(pHo != null) {

					nObjects--;
					if (pHo.hoType >= 32)
					{
						if (pHo.hoCommon.ocIdentifier == CRun.BASEIDENTIFIER)
						{
							rh4Box2DBase = (CRunBaseParent)((CExtension)pHo).ext;
							break;
						}
					}
					if(nObjects == 0)
						break;
				}
			}
		}
		return rh4Box2DBase;
	}

	public CRunMBase GetMBase(CObject pHo)
	{
		if (rh4Box2DObject && pHo != null && (pHo.hoFlags & CObject.HOF_DESTROYED) == 0)
		{
			if ((pHo.hoOEFlags&CObjectCommon.OEFLAG_MOVEMENTS)!=0)
			{
				if (pHo.roc.rcMovementType==CMoveDef.MVTYPE_EXT)
				{
					CMoveDefExtension mvPtr= (CMoveDefExtension)pHo.hoCommon.ocMovements.moveList[pHo.rom.rmMvtNum];
					if (mvPtr.moduleName.equalsIgnoreCase("box2d8directions")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dspring")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dspaceship")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dstatic")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dracecar")
							|| mvPtr.moduleName.equalsIgnoreCase("box2daxial")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dplatform")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dbouncingball")
							|| mvPtr.moduleName.equalsIgnoreCase("box2dbackground")
							)
					{
						return (CRunMBase)((CMoveExtension)pHo.rom.rmMovement).movement;
					}
				}
			}
		}
		return null;
	}
}
