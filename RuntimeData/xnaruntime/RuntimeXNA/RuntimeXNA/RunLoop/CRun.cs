//----------------------------------------------------------------------------------
//
// CRUNLOOP : BOucle principale
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;
using RuntimeXNA.OI;
using RuntimeXNA.Frame;
using RuntimeXNA.Application;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
using RuntimeXNA.Movements;
using RuntimeXNA.Animations;
using RuntimeXNA.Values;
using RuntimeXNA.Events;
using RuntimeXNA.Expressions;
using RuntimeXNA.Params;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
 
namespace RuntimeXNA.RunLoop
{
    public class CRun
    {
        // Flags
        public const short GAMEFLAGS_VBLINDEP = 0x0002;
        public const short GAMEFLAGS_LIMITEDSCROLL = 0x0004;
        public const short GAMEFLAGS_FIRSTLOOPFADEIN = 0x0010;
        public const short GAMEFLAGS_LOADONCALL = 0x0020;
        public const short GAMEFLAGS_REALGAME = 0x0040;
        public const short GAMEFLAGS_PLAY = 0x0080;
        public const short GAMEFLAGS_INITIALISING = 0x0200;

        // Flags pour DrawLevel
        public const short DLF_DONTUPDATE = 0x0002;
        public const short DLF_DRAWOBJECTS = 0x0004;
        public const short DLF_RESTARTLEVEL = 0x0008;
        public const short DLF_DONTUPDATECOLMASK = 0x0010;
        public const short DLF_COLMASKCLIPPED = 0x0020;
        public const short DLF_SKIPLAYER0 = 0x0040;
        public const short DLF_REDRAWLAYER = 0x0080;
        public const short DLF_STARTLEVEL = 0x0100;
        public const short GAME_XBORDER = 480;
        public const short GAME_YBORDER = 300;
        public const short COLMASK_XMARGIN = 64;
        public const short COLMASK_YMARGIN = 16;
        public const uint WRAP_X = 1;
        public const uint WRAP_Y = 2;
        public const uint WRAP_XY = 4;
        public  byte[] plMasks =
        {
            (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
            (byte) 0xFF, (byte) 0x00, (byte) 0x00, (byte) 0x00,
            (byte) 0xFF, (byte) 0xFF, (byte) 0x00, (byte) 0x00,
            (byte) 0xFF, (byte) 0xFF, (byte) 0xFF, (byte) 0x00,
            (byte) 0xFF, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF
        };

        // Flags pour rh3Scrolling
        public const int RH3SCROLLING_SCROLL = 0x0001;
        public const int RH3SCROLLING_REDRAWLAYERS = 0x0002;
        public const int RH3SCROLLING_REDRAWALL = 0x0004;
        public const int RH3SCROLLING_REDRAWTOTALCOLMASK = 0x0008;

        // Types d'obstacles
        public const int OBSTACLE_NONE = 0;
        public const int OBSTACLE_SOLID = 1;
        public const int OBSTACLE_PLATFORM = 2;
        public const int OBSTACLE_LADDER = 3;
        public const int OBSTACLE_TRANSPARENT = 4;		// for Add Backdrop

        //Flags pour createobject
        public const short COF_NOMOVEMENT = 0x0001;
        public const short COF_HIDDEN = 0x0002;
        public const short COF_FIRSTTEXT = 0x0004;
        public const short MAX_FRAMERATE = 10;

        // Main loop exit codes
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public const short LOOPEXIT_NEXTLEVEL = 1;
        public const short LOOPEXIT_PREVLEVEL = 2;
        public const short LOOPEXIT_GOTOLEVEL = 3;
        public const short LOOPEXIT_NEWGAME = 4;
        public const short LOOPEXIT_PAUSEGAME = 5;
        public const short LOOPEXIT_SAVEAPPLICATION = 6;
        public const short LOOPEXIT_LOADAPPLICATION = 7;
        public const short LOOPEXIT_SAVEFRAME = 8;
        public const short LOOPEXIT_LOADFRAME = 9;
        public const short LOOPEXIT_ENDGAME = -2;
        public const short LOOPEXIT_QUIT = 100;
        public const short LOOPEXIT_RESTART = 101;
        public const short LOOPEXIT_APPLETPAUSE = 102;
        public const short BORDER_LEFT = 1;
        public const short BORDER_RIGHT = 2;
        public const short BORDER_TOP = 4;
        public const short BORDER_BOTTOM = 8;
        public const short BORDER_ALL = 15;

        // Table d'elimination des entrees/sorties impossibles
        // ---------------------------------------------------
        short[] Table_InOut =
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
   		public static bool bMoveChanged;

        public CRunApp rhApp;						// Application info
        public CRunFrame rhFrame;					// Frame info
        public int rhMaxOI;
        public byte rhStopFlag;					// Current movement needs to be stopped
        public byte rhEvFlag=0; 					// Event evaluation flag
        public int rhNPlayers;					// Number of players
        public byte rhMouseUsed;					// Players using the mouse
        public short rhGameFlags;					// Game flags
        public byte[] rhPlayer = new byte[4];					// Current players entry
        public short rhQuit;
        public short rhQuitBis; 					// Secondary quit (scrollings)
        public int rhReturn=0;					// Value to return to the editor
        public int rhQuitParam;
        public int rhNObjects;
        public int rhMaxObjects;
        public CObjInfo[] rhOiList;                     		// ObjectInfo list
        public CEventProgram rhEvtProg = null;
        public int rhLevelSx;				// Window size
        public int rhLevelSy;
        public int rhWindowX;   				// Start of window in X/Y
        public int rhWindowY;
        public int rhVBLDeltaOld=0;				// Number of VBL
        public int rhVBLObjet;				// For the objects
        public int rhVBLOld;				// For the counter
        public short rhMT_VBLStep;   			// Path movement variables
        public short rhMT_VBLCount;
        public int rhMT_MoveStep;
        public int rhLoopCount;				// Number of loops since start of level
        public long rhTimer;				// Timer in 1/50 since start of level
        public long rhTimerOld;				// For delta calculation
        public long rhTimerFPSOld;				// For delta calculation
        public int rhTimerDelta;				// For delta calculation
        public int rhOiListPtr=0;				// OI list enumeration
        public short rhObListNext=0;				// Branch label
        public short rhDestroyPos;
        public byte[] rh2OldPlayer = new byte[4];				// Previous player entries
        public byte[] rh2NewPlayer = new byte[4];				// Modified player entries
        public byte[] rh2InputMask = new byte[4];				// Inhibated players entries
        public byte rh2MouseKeys;				// Mousekey entries
        public short rh2CreationCount;			// Number of objects created since beginning of frame
        public int rh2MouseX;				// Mouse coordinate
        public int rh2MouseY;				// Mouse coordinate
        public int oldMouseKey;
        public int mouseKey;
        public int toucheID;
        public long mouseKeyTime;
        public int rh2MouseSaveX;				// Mouse saving when pause
        public int rh2MouseSaveY;				// Mouse saving when pause
        public int rh2PauseState;
        public int rh2PauseCompteur;
        public int rh2PauseTimer;
        public int rh2PauseFPSTimer;
        public int rh2PauseVbl = 0;
        public int rh3DisplayX;				// To scroll
        public int rh3DisplayY;
        public int rh3WindowSx;   				// Window size
        public int rh3WindowSy;
        public short rh3CollisionCount;			// Collision counter
        public byte rh3Scrolling;				// Flag: we need to scroll
        public int rh3Panic;
        public int rh3XMinimum;   				// Object inactivation coordinates
        public int rh3YMinimum;
        public int rh3XMaximum;
        public int rh3YMaximum;
        public int rh3XMinimumKill;			// Object destruction coordinates
        public int rh3YMinimumKill;
        public int rh3XMaximumKill;
        public int rh3YMaximumKill;
        public short rh3Graine;
//        public short rh4DemoMode;
//        public CDemoRecord rh4Demo;
        public Keys rh4PauseKey;
        public bool bCheckResume;
        public string rh4CurrentFastLoop;
        public int rh4EndOfPause;
        public short rh4MouseWheelDelta=0;
        public int rh4OnMouseWheel;
//        public string rh4PSaveFilename;
//        public int rh4SaveVersion;
//        public int rh4MusicHandle;
//        public int rh4MusicFlags;
//        public int rh4MusicLoops;
//        public int rh4LoadCount;

    //	LPDWORD		rh4TimerEventsBase;				// Timer events base
    //	short		rh4DroppedFlag;
    //	short		rh4NDroppedFiles;
    //	LPSTR		rh4DroppedFiles;
        public CArrayList rh4FastLoops = null;
    //	LPSTR		rh4CreationErrorMessages;
        public CValue rh4ExpValue1=null;				// New V2
        public CValue rh4ExpValue2=null;
        public int rh4KpxReturn=0;				// WindowProc return
        public int rh4ObjectCurCreate;
        public int rh4ObjectAddCreate;
        public short rh4FakeKey;				// For step through : fake key pressed
        public byte rh4DoUpdate;				// Flag for screen update on first loop
        public bool rh4MenuEaten = false;			// Menu handled in an event?
        public int rh4OnCloseCount;			// For OnClose event
        public bool rh4CursorShown;				// Mouse counter
        public short rh4ScrMode=0;				// Current screen mode
        public int rh4VBLDelta;				// Number of VBL
        public int rh4LoopTheoric;				// Theorical VBL counter
        public int rh4EventCount;
        public CArrayList rh4BackDrawRoutines = null;
        public short rh4LastQuickDisplay;			// Quick - display list
        public short rh4FirstQuickDisplay;			// Quick-display object list
        public int rh4WindowDeltaX;			// For scrolling
        public int rh4WindowDeltaY;
        public long rh4TimeOut;				// For time-out!
        public int rh4MouseXCenter;
        public int rh4MouseYCenter;
        public int rh4PosPile=0;				// Expression evaluation pile position
        public CValue[] rh4Results;				// Result pile
        public CExp[] rh4Operators;				// Operators pile
        public CExp rh4OpeNull;
        public int rh4CurToken=0;
        public CExp[] rh4Tokens=null;
        public const int MAX_INTERMEDIATERESULTS = 128;
        public int[] rh4FrameRateArray = new int[MAX_FRAMERATE];             // Framerate calculation buffer
        public int rh4FrameRatePos;						// Position in buffer
        public int rh4FrameRatePrevious;					// Previous time
        public int[] rhDestroyList;			// Destroy list address
        public int rh4SaveFrame;
        public int rh4SaveFrameCount;
        public double rh4MvtTimerCoef;
        public CObject[] rhObjectList = null;			// Object list address
//        int xScrolling = 320;
        public bool bOperande=false;
        public KeyboardState keyboardState;
        public byte rhJoystickMask;
        public short[] isColArray = new short[2];

#if !WINDOWS_PHONE
        public MouseState mouseState;
#endif
        public bool bAnyKeyDown;
        public CQuestion questionObjectOn=null;
        public int nSubApps;
        public int nControls;
        public CArrayList controls;
        public IControl currentControl;        
        public bool bMouseControlled;
        public int mouseX;
        public int mouseY;
        public PlayerIndex deviceSelectorPlayer=PlayerIndex.One;
#if WINDOWS_PHONE
        public ITouches touches;
        public CJoystick joystick;
        public CJoystickAcc joystickAcc;
        public int joystickAccCount;
        public int[] cancelledTouches;
#endif

        public CRun()
        {
        }

        public CRun(CRunApp app)
        {
            rhApp = app;
        }

        public void setFrame(CRunFrame f)
        {
            rhFrame=f;
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
            int n;
            for (n = 0; n < rhMaxOI; n++)
            {
                rhOiList[n] = null;
            }

            // Random generator
            if (rhFrame.m_wRandomSeed == -1)
            {
                Random rnd = new Random();
                rh3Graine = (short)rnd.Next(32000);				// Fait un randomize
            }
            else
            {
                rh3Graine = rhFrame.m_wRandomSeed;			// Fixe la valeur donnée
            }

            // Le generateur de sprites
            rhApp.spriteGen.setData(rhApp.imageBank, rhApp, rhFrame);

            // La destroy-list
            rhDestroyList = new int[(rhFrame.maxObjects / 32) + 1];

            // Les fast loops
            rh4FastLoops = new CArrayList();
            rh4CurrentFastLoop = "";

            // Le buffer d'objets
            rhMaxObjects = rhFrame.maxObjects;

            // INITIALISATION DU RESTE DES DONNEES
            rhNPlayers = rhEvtProg.nPlayers;
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
            rh4FirstQuickDisplay = -1;
            rh4LastQuickDisplay = -1;
            rh4MouseXCenter = rhFrame.leEditWinWidth / 2;
            rh4MouseYCenter = rhFrame.leEditWinHeight / 2;
            rh4FrameRatePos = 0;
            rh4FrameRatePrevious = 0;
            //	CreateCoxHandleRoutines();
            rh4BackDrawRoutines = null;
            rh4SaveFrame = 0;
            rh4SaveFrameCount = -3;
            nSubApps = 0;

            rhGameFlags |= GAMEFLAGS_REALGAME;

            // Rempli les CValue pour l'evaluation d'expression
            rh4Results = new CValue[MAX_INTERMEDIATERESULTS];
            rh4Operators = new CExp[MAX_INTERMEDIATERESULTS];
            for (n = 0; n < MAX_INTERMEDIATERESULTS; n++)
            {
                rh4Results[n] = new CValue();
            }
            rh4OpeNull = new EXP_END();
            rh4OpeNull.code = 0;
            rhEvtProg.rh2CurrentClick = -1;

            nControls = 0;
            currentControl = null;
            controls = null;
            bMouseControlled=true;
            mouseKey = -1;

#if XBOX
            bMouseControlled=false;
#endif
#if WINDOWS_PHONE
            cancelledTouches=new int[rhApp.numberOfTouches];
#endif

            // Header valide!
            rhFrame.rhOK = true;
            return 0;
        }

        public void freeRunHeader()
        {
            rhFrame.rhOK = false;

            // Si demo
//            rh4Demo = null;
            rhObjectList = null;
            rhOiList = null;
            rhDestroyList = null;
//            rh4PSaveFilename = null;
            rh4CurrentFastLoop = null;
            rh4FastLoops = null;
            rh4BackDrawRoutines = null;

            // Vire les CValues
            for (int n = 0; n < MAX_INTERMEDIATERESULTS; n++)
            {
                rh4Results[n] = null;
            }
            rh4OpeNull = null;
        }

        public int initRunLoop()
        {
            int error = 0;

            // Si chargement d'une frame
/*            if (rhApp.pLoadFilename != null)
            {
                if (loadFramePosition(null))
                {
                    if (bFade)
                    {
                        rhGameFlags |= GAMEFLAGS_FIRSTLOOPFADEIN;
                    }
                    return 0;
                }

                // Erreur: effacer la demi frame
                f_StopSamples();
                f_StopMusics();
                killFrameObjects();
                y_KillLevel(false);
                rhEvtProg.unBranchPrograms();
                freeMouse();
                freeRunHeader();
            }
*/
            error = allocRunHeader();
            if (error != 0)
            {
                return error;
            }

            initAsmLoop();

            y_InitLevel();
            //      f_InitLoop();

            error = prepareFrame();
            if (error != 0)
            {
                return error;
            }

            error = createFrameObjects();
            if (error != 0)
            {
                return error;
            }

            redrawLevel((short)(DLF_DONTUPDATE | DLF_STARTLEVEL));

            loadGlobalObjectsData();

            rhEvtProg.prepareProgram();
            rhEvtProg.assemblePrograms(this);

            captureMouse();
            rhQuitParam = 0;
            f_InitLoop();

            return 0;
        }

#if XBOX
        bool getPauseKeys()
        {
            int i;
            bool bUnPause = false;
            GamePadState[] states = new GamePadState[4];
            for (i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        states[i] = GamePad.GetState(PlayerIndex.One);
                        break;
                    case 1:
                        states[i] = GamePad.GetState(PlayerIndex.Two);
                        break;
                    case 2:
                        states[i] = GamePad.GetState(PlayerIndex.Three);
                        break;
                    case 3:
                        states[i] = GamePad.GetState(PlayerIndex.Four);
                        break;
                }
                int n;
                for (n = 0; n < 4; n++)
                {
                    if (states[n].DPad.Left == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].DPad.Right == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].DPad.Up == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].DPad.Down == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].ThumbSticks.Left.X<-0.5)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].ThumbSticks.Left.X > 0.5)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].ThumbSticks.Left.Y > 0.5)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].ThumbSticks.Left.Y < -0.5)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].Buttons.A == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].Buttons.B == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].Buttons.X == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].Buttons.Y == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].Buttons.Start == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                    if (states[n].Buttons.Back == ButtonState.Pressed)
                    {
                        bUnPause = true;
                        break;
                    }
                }
                if (bUnPause)
                {
                    break;
                }
            }
            return bUnPause;
        }
#endif
        // Un tour de boucle
        public int doRunLoop()
        {
            // Appel du jeu
            rhApp.appRunFlags |= CRunApp.ARF_INGAMELOOP;
            int quit = f_GameLoop();
            rhApp.appRunFlags &= ~CRunApp.ARF_INGAMELOOP;

            // Appel des evenements click
#if !WINDOWS_PHONE
            mouseKey = -1;
            mouseState = Mouse.GetState();
            if (mouseState.MiddleButton==ButtonState.Pressed)
            {
                mouseKey=1;
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                mouseKey=2;
            }
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mouseKey=0;
            }
            mouseX = mouseState.X;
            mouseY = mouseState.Y;
#else
            getTouches();
#endif
            getMouseCoords();
            if (mouseKey!=oldMouseKey)
            {
                int nClicks=1;
                if (mouseKey>=0)
                {
                    long delta=rhApp.timer-mouseKeyTime;
                    if (delta<500)
                    {
                        nClicks=2;
                        mouseKeyTime=0;
                    }
                    else
                    {
                        mouseKeyTime=rhApp.timer;
                    }
                	rhEvtProg.onMouseButton(mouseKey, nClicks);
                    clickControls(nClicks);
                }
                oldMouseKey=mouseKey;
            }

            // Appel des evenements ANYKEY
            if (rhEvtProg.bTestAllKeys || rh2PauseCompteur>0)
            {
#if !XBOX
                int n=0;
                do
                {
                    if (keyboardState.IsKeyDown(CKeyConvert.xnaKeys[n]))
                    {
                        if (bAnyKeyDown == false)
                        {
                            bAnyKeyDown = true;
                            rhEvtProg.onKeyDown(CKeyConvert.xnaKeys[n]);
                        }
                        break;
                    }
                    n++;
                } while (CKeyConvert.pcKeys[n] >= 0);
                if (CKeyConvert.pcKeys[n] < 0)
                {
                    bAnyKeyDown = false;
                }
#endif
#if XBOX
                if (rh2PauseCompteur>0)
                {
                    if (rh2PauseState == 0)
                    {
                        if (getPauseKeys() == false)
                        {
                            rh2PauseState = 1;
                        }
                    }
                    else
                    {
                        if (getPauseKeys())
                        {
                            resume();
                            rh4EndOfPause = rhLoopCount; // Pour les evenements II
                            rhEvtProg.handle_GlobalEvents(((-8 << 16) | 0xFFFD)); // CNDL_ENDOFPAUSE
                        }
                    }
                }
#endif
            }


/*            // Si fin de FADE IN, detruit les sprites
            if ((rhGameFlags & GAMEFLAGS_FIRSTLOOPFADEIN) != 0)
            {
                f_RemoveObjects();
                rhFrame.fadeTimerDelta = System.currentTimeMillis() - rhTimerOld;
                rhFrame.fadeVblDelta = rhApp.newGetCptVbl() - rhVBLOld;
                y_KillLevel(true);
                rhEvtProg.unBranchPrograms();
            }
*/
/*            if (rh4SaveFrame == LOOPEXIT_SAVEFRAME)
            {
                rh4SaveFrame = 0;
                if (!rhFrame.fade)
                {
                    saveFramePosition(null);
                }
            }
*/
            if (quit != 0)
            {
//                int frame = 0;
                switch (quit)
                {
/*                    // Chargement de l'application
                    case 7:		// LOOPEXIT_LOADAPPLICATION:
                        frame = loadApplicationPosition(null);
                        if (frame < 0)
                        {
                            break;
                        }
                        if (frame != rhApp.currentFrame)
                        {
                            frame |= 0x8000;
                            quit = LOOPEXIT_GOTOLEVEL;
                            rhQuit = (short)quit;
                            rhQuitParam = frame;
                            break;
                        }
 
                    // Chargement d'une frame
                    case 9:		// LOOPEXIT_LOADFRAME:
                        rhQuit = 0;
                        quit = 0;
                        frame = 0;
                        if (loadFramePosition(null))
                        {
                            break;
                        }
 */ 
                    // Redémarre la frame
                    case 101:	// LOOPEXIT_RESTART:
                        if (rhFrame.fade)
                        {
                            break;
                        }

                        // Sortie du niveau preceddent
                        f_StopSamples();
                        killFrameObjects();
                        y_KillLevel(false);
                        rhEvtProg.unBranchPrograms();
                        freeMouse();
                        freeRunHeader();

                        // Redemarre la frame
                        rhFrame.leX = rhFrame.leLastScrlX = 0;
                        rhFrame.leY = rhFrame.leLastScrlY = 0;
                        if (rhFrame.colMask != null)
                        {
                            rhFrame.colMask.setOrigin(0, 0);
                        }
                        allocRunHeader();
                        initAsmLoop();
                        y_InitLevel();
                        redrawLevel((short)(DLF_DONTUPDATE | DLF_RESTARTLEVEL));
                        prepareFrame();
                        //		    f_InitLoop();
                        createFrameObjects();
                        loadGlobalObjectsData();
                        rhEvtProg.prepareProgram();
                        rhEvtProg.assemblePrograms(this);
                        f_InitLoop();
                        captureMouse();
                        quit = 0;
                        rhQuitParam = 0;
                        break;

                    case 100:	    // LOOPEXIT_QUIT:
                    case -2:	    // LOOPEXIT_ENDGAME:
                        rhEvtProg.handle_GlobalEvents(((-4 << 16) | 65533));	// CNDL_QUITAPPLICATION
                        break;

                    case 102:	    // LOOPEXIT_APPLETPAUSE
                        quit = rhQuit;
                        break;
                }
            }
            return quit;
        }

        // Sortie de la boucle
        public int killRunLoop(int quit, bool bLeaveSamples)
        {
            int quitParam;

            // Filtre les codes internes
            if (quit > 100)
            {
                quit = LOOPEXIT_ENDGAME;
            }
            quitParam = (int)rhQuitParam;
            saveGlobalObjectsData();
            killFrameObjects();

            y_KillLevel(bLeaveSamples);
            rhEvtProg.unBranchPrograms();
            freeRunHeader();
#if WINDOWS_PHONE
            if (joystickAcc != null)
            {
                stopJoystickAcc();
            }
#endif
            return (CServices.MAKELONG(quit, quitParam));
        }

        public void y_InitLevel()
        {
            resetFrameLayers(-1, false);
        }

        public void initAsmLoop()
        {
            rhApp.spriteGen.winSetColMode(CSpriteGen.CM_BITMAP);				// Collisions precises
            f_ObjMem_Init();
        }

        public void f_ObjMem_Init()
        {
            for (int i = 0; i < rhMaxObjects; i++)
            {
                rhObjectList[i] = null;
            }
        }

        // PREPARATION DE LA FRAME AU RUN
        public int prepareFrame()
        {
            COI oiPtr;
            CObjectCommon ocPtr;
            short n, type;

            // Flags de RUN
            if ((rhApp.gaFlags & CRunApp.GA_SPEEDINDEPENDANT) != 0 && rhFrame.fade == false)
            {
                rhGameFlags |= GAMEFLAGS_VBLINDEP;
            }
            else
            {
                rhGameFlags &= ~GAMEFLAGS_VBLINDEP;
            }
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
                    ocPtr = (CObjectCommon)oiPtr.oiOC;
                    if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0 && ocPtr.ocMovements != null)
                    {
                        for (n = 0; n < ocPtr.ocMovements.nMovements; n++)
                        {
                            CMoveDef mvPtr = ocPtr.ocMovements.moveList[n];
                            if (mvPtr.mvType == CMoveDef.MVTYPE_MOUSE)
                            {
                                rhMouseUsed |= (byte)(1 << (mvPtr.mvControl - 1));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < rhFrame.nLayers; i++)
            {
                CLayer layer = rhFrame.layers[i];
                layer.nZOrderMax = 1;
            }
            return 0;
        }

        // POSITIONNE LA FRAME AU DEBUT
        public int createFrameObjects()
        {
            COI oiPtr;
            CObjectCommon ocPtr;
            short type;
            int n;
            short creatFlags;
            CLO loPtr;

            int error = 0;
            for (n = 0, loPtr = rhFrame.LOList.first_LevObj(); loPtr != null; n++, loPtr = rhFrame.LOList.next_LevObj())
            {
                oiPtr = rhApp.OIList.getOIFromHandle(loPtr.loOiHandle);
                ocPtr = (CObjectCommon)oiPtr.oiOC;
                type = oiPtr.oiType;

                creatFlags = 0;

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

                // Creation de l'objet                
                if ((ocPtr.ocOEFlags & CObjectCommon.OEFLAG_DONTCREATEATSTART) == 0)
                {
                    f_CreateObject(loPtr.loHandle, loPtr.loOiHandle, 0x7FFFFFFF, 0x7FFFFFFF, -1, creatFlags, -1, -1);
                }
            }
            rhGameFlags &= ~GAMEFLAGS_INITIALISING;
            return error;
        }

        public void killFrameObjects()
        {
            // Arrete tous les sprites, en mode FAST
            short n;
            for (n = 0; n < rhMaxObjects && rhNObjects != 0; n++)
            {
                f_KillObject(n, true);
            }
            rh4FirstQuickDisplay = -1;
        }

        // End frame
        public void y_KillLevel(bool bLeaveSamples)
        {
            // Clear ladders & additional backdrops
            resetFrameLayers(-1, false);

            // ++v1.03.35: stop sounds only if not GANF_SAMPLESOVERFRAMES
            if (!bLeaveSamples)
            {
                if ((rhApp.gaNewFlags & CRunApp.GANF_SAMPLESOVERFRAMES) == 0)
                {
                    rhApp.soundPlayer.stopAllSounds();
                }
                else
                {
                    rhApp.soundPlayer.keepCurrentSounds();
                }
            }
        }

        public void resetFrameLayers(int nLayer, bool bDeleteFrame)
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
                    plo = rhFrame.LOList.getLOFromIndex((short)(pLayer.nFirstLOIndex + j));

                    // Delete sprite
                    for (int ns = 0; ns < 4; ns++)
                    {
                        if (plo.loSpr[ns] != null)
                        {
                            rhApp.spriteGen.delSpriteFast(plo.loSpr[ns]);
                            plo.loSpr[ns] = null;
                        }
                    }
                }

                if (pLayer.pBkd2 != null)
                {
                    for (j = 0; j < pLayer.pBkd2.size(); j++)
                    {
                        CBkd2 pbkd = (CBkd2)pLayer.pBkd2.get(j);
                        // Delete sprite
                        for (int ns = 0; ns < 4; ns++)
                        {
                            if (pbkd.pSpr[ns] != null)
                            {
                                rhApp.spriteGen.delSpriteFast(pbkd.pSpr[ns]);
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
            }
        }

        void f_RemoveObjects()
        {
            int count = 0;
            int no;
            for (no = 0; no < rhNObjects; no++)
            {
                while (rhObjectList[count] == null)
                {
                    count++;
                }
                CObject hoPtr = rhObjectList[count];
                count++;
                if (hoPtr.ros != null)
                {
                    if (hoPtr.roc.rcSprite != null)
                    {
                        // Save Z-order value before deleting sprite
                        hoPtr.ros.rsZOrder = hoPtr.roc.rcSprite.sprZOrder;

                        // Delete sprite
                        rhApp.spriteGen.delSpriteFast(hoPtr.roc.rcSprite);
                    }
                }
                if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_QUICKDISPLAY) != 0)
                {
                    remove_QuickDisplay(hoPtr);
                }
            }
        }

        public void captureMouse()
        {
#if !WINDOWS_PHONE
            if (rhMouseUsed != 0)
            {
                MouseState state = Mouse.GetState();
                rh2MouseSaveX = state.X;
                rh2MouseSaveY = state.Y;
                hideMouse();
            }
#endif
        }

        public void freeMouse()
        {
#if !WINDOWS_PHONE
            if (rhMouseUsed != 0)
            {
                showMouse();
                Mouse.SetPosition(rh2MouseSaveX, rh2MouseSaveY);
            }
#endif
        }

        public void showMouse()
        {
#if !WINDOWS_PHONE
            rh4CursorShown = true;
            rhApp.showCursor(true);
#endif
        }

        public void hideMouse()
        {
#if !WINDOWS_PHONE
            rh4CursorShown = false;
            rhApp.showCursor(false);
#endif
        }

        // -------------------------------------------------------------------------------
        // SAUVEGARDE / RESTAURATION DES OBJETS GLOBAUX
        // -------------------------------------------------------------------------------
        public void saveGlobalObjectsData()
        {
            CObject hoPtr;
            CObjInfo oilPtr;
            int oil, obj;
            COI oiPtr;
            String name;
            short o;

            for (oil = 0; oil < rhOiList.Length; oil++)
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
                        name = string.Format("{0:s}::{1:d}", oilPtr.oilName, oilPtr.oilType);

                        if (rhApp.adGO == null)
                        {
                            rhApp.adGO = new CArrayList();
                        }

                        // Rechercher l'objet dans les objets déjà créés
                        bool flag = false;
                        CSaveGlobal save = null;
                        for (obj = 0; obj < rhApp.adGO.size(); obj++)
                        {
                            save = (CSaveGlobal)rhApp.adGO.get(obj);
                            if (name==save.name)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag == false)
                        {
                            save = new CSaveGlobal();
                            save.name = name;
                            save.objects = new CArrayList();
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
                                CText text = (CText)hoPtr;
                                CSaveGlobalText saveText = new CSaveGlobalText();
                                saveText.text = text.rsTextBuffer;
                                saveText.rsMini = text.rsMini;
                                save.objects.add(saveText);
                            }
                            else if (oilPtr.oilType == COI.OBJ_COUNTER)
                            {
                                CCounter counter = (CCounter)hoPtr;
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
                                saveValues.flags = hoPtr.rov.rvValueFlags;
                                saveValues.values = new CValue[CRVal.VALUES_NUMBEROF_ALTERABLE];
                                for (n = 0; n < CRVal.VALUES_NUMBEROF_ALTERABLE; n++)
                                {
                                    saveValues.values[n] = null;
                                    if (hoPtr.rov.rvValues[n] != null)
                                    {
                                        saveValues.values[n] = new CValue(hoPtr.rov.rvValues[n]);
                                    }
                                }
                                saveValues.strings = new String[CRVal.STRINGS_NUMBEROF_ALTERABLE];
                                for (n = 0; n < CRVal.STRINGS_NUMBEROF_ALTERABLE; n++)
                                {
                                    saveValues.strings[n] = null;
                                    if (hoPtr.rov.rvStrings[n] != null)
                                    {
                                        saveValues.strings[n] = hoPtr.rov.rvStrings[n];
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

            for (oil = 0; oil < rhOiList.Length; oil++)
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
                        name = string.Format("{0:s}::{1:d}", oilPtr.oilName, oilPtr.oilType);

                        // Recherche dans les headers
                        for (obj = 0; obj < rhApp.adGO.size(); obj++)
                        {
                            CSaveGlobal save = (CSaveGlobal)rhApp.adGO.get(obj);
                            if (name==save.name)
                            {
                                int count = 0;
                                while (true)
                                {
                                    hoPtr = rhObjectList[o];

                                    if (oilPtr.oilType == COI.OBJ_TEXT)
                                    {
                                        CSaveGlobalText saveText = (CSaveGlobalText)save.objects.get(count);
                                        CText text = (CText)hoPtr;
                                        text.rsTextBuffer = saveText.text;
                                        text.rsMini = saveText.rsMini;
                                    }
                                    else if (oilPtr.oilType == COI.OBJ_COUNTER)
                                    {
                                        CSaveGlobalCounter saveCounter = (CSaveGlobalCounter)save.objects.get(count);
                                        CCounter counter = (CCounter)hoPtr;
                                        counter.rsValue = new CValue(saveCounter.value);
                                        counter.rsMini = saveCounter.rsMini;
                                        counter.rsMaxi = saveCounter.rsMaxi;
                                        counter.rsMiniDouble = saveCounter.rsMiniDouble;
                                        counter.rsMaxiDouble = saveCounter.rsMaxiDouble;
                                    }
                                    else
                                    {
                                        CSaveGlobalValues saveValues = (CSaveGlobalValues)save.objects.get(count);
                                        hoPtr.rov.rvValueFlags = saveValues.flags;
                                        int n;
                                        for (n = 0; n < CRVal.VALUES_NUMBEROF_ALTERABLE; n++)
                                        {
                                            if (saveValues.values[n] != null)
                                            {
                                                hoPtr.rov.rvValues[n] = new CValue(saveValues.values[n]);
                                            }
                                        }
                                        for (n = 0; n < CRVal.STRINGS_NUMBEROF_ALTERABLE; n++)
                                        {
                                            if (saveValues.strings[n] != null)
                                            {
                                                hoPtr.rov.rvStrings[n] = saveValues.strings[n];
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
                CObjectCommon ocPtr = (CObjectCommon)oiPtr.oiOC;

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
                    case 8:         // OBJ_RTF
//                        hoPtr = new CRtf();
                        break;
                    case 9:         // OBJ_CCA
                        hoPtr = new CCCA();
                        break;
                    default:        // Extensions
                        hoPtr = new CExtension(oiPtr.oiType, this);
                        if (((CExtension)hoPtr).ext == null)
                        {
                            hoPtr = null;
                        }
                        break;
                }
                if (hoPtr == null)
                {
                    break;
                }

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

                // Gestion de la boucle principale
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (numCreation > rh4ObjectCurCreate)				// Si objet DEVANT l'objet courant
                {
                    rh4ObjectAddCreate++;					// Il faut explorer encore!
                }

                // Rempli la structure headerObject
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                hoPtr.hoNumber = (short)numCreation;					  			//; Numero de l'objet
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
                //	if (rhGameFlags&GAMEFLAGS_LOADONCALL)
                //	{
                //            loadOnCall(Get_ObjInfo(hoPtr.hoOi));
                //	}

                // Adresse de l'objectsCommon
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~
                hoPtr.hoCommon = ocPtr;

                // Rempli la structure CreateObjectInfo (virer X et Y)
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                int x = coordX;									// X
                if (x == 0x7FFFFFFF)
                {
                    x = loPtr.loX;
                }
                cob.cobX = x;
                hoPtr.hoX = x;

                int y = coordY;									// Y
                if (y == 0x7FFFFFFF)
                {
                    y = loPtr.loY;
                }
                cob.cobY = y;
                hoPtr.hoY = y;

                // Set layer
                if (loPtr != null)
                {
                    if (nLayer == -1)
                    {
                        nLayer = loPtr.loLayer;
                    }
                }
                else
                {
                    nLayer = 0;
                }
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
                    hoPtr.ros.init2(true);
                }
                // Sortie sans erreur
                return numCreation;									// Retourne avec EAX=NOBJECT
            }
            return -1;
        }

        public void f_KillObject(int nObject, bool bFast)
        {
            // Pointe l'objet
            // ~~~~~~~~~~~~~~
            CObject hoPtr = rhObjectList[nObject];
            if (hoPtr == null)
            {
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

            // Appelle la routine de destruction specifique
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            hoPtr.kill(bFast);

            // Enleve le OI (pas en mode panique!)
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            oi_Delete(hoPtr);

            // Vire les sprites si LOADONCALL et DISCARD
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            hoPtr.hoCreationId = 0;
            if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_QUICKDISPLAY) != 0 && hoPtr.ros.rsLayer==0)
            {
                remove_QuickDisplay(hoPtr);
            }

            rhObjectList[nObject] = null;
            rhNObjects--;

            // Efface les appels aux routines d'affichage
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            hoPtr.hoCallRoutine = false;
        }

        // Additionne un objet a la liste de destroy
        // ------------------------------------------
        public void destroy_Add(int hoNumber)
        {
            rhDestroyList[hoNumber / 32] |= (1 << (hoNumber & 31));
            rhDestroyPos++;
        }

        // Detruit tous les sprites de la liste
        // ------------------------------------
        public void destroy_List()
        {
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
                                    int cond = (-33 << 16);	    // CNDL_EXTNOMOREOBJECT
                                    cond |= (((int)pHo.hoType) & 0xFFFF);
                                    rhEvtProg.handle_Event(pHo, cond);	    
                                }
                            }
                            // Detruit l'objet!
                            f_KillObject(nob + count, false);
                            rhDestroyPos--;
                        }
                        dw = dw >> 1;
                    }
                    rhDestroyList[nob / 32] = 0;
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
        void killShootPtr(CObject hoSource)
        {
            int count = 0;
            int nObject;
            CObject hoPtr;
            for (nObject = 0; nObject < rhNObjects; nObject++)
            {
                while (rhObjectList[count] == null)
                {
                    count++;
                }
                hoPtr = rhObjectList[count];
                count++;

                if (hoPtr.rom != null)
                {
                    if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_BULLET)
                    {
                        CMoveBullet mBullet = (CMoveBullet)hoPtr.rom.rmMovement;
                        if (mBullet.MBul_ShootObject == hoSource && mBullet.MBul_Wait == true)
                        {
                            mBullet.startBullet();
                        }
                    }
                }
            }
        }

        // Insere l'objet [esi] dans les liste d'OI
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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
                pHo.hoNumPrev = -1;
                pHo.hoNumNext = -1;
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
                        pHo2.hoNumNext = -1;
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
                    poil.oilObject = -1;
                }
            }
        }

        public void pause()
        {
            // Le compteur de sauvegarde
            // ~~~~~~~~~~~~~~~~~~~~~~~~~
            rh2PauseCompteur++;
            if (rh2PauseCompteur == 1)
            {
                // Sauve le timer
                // ~~~~~~~~~~~~~~
                rh2PauseTimer = (int)rhApp.timer;
                rh2PauseFPSTimer = (int)rhApp.timer;
                rh2PauseState = 0;

                // Sauve le VBL
                // ~~~~~~~~~~~~
                rh2PauseVbl = rhApp.newGetCptVbl() - rhVBLOld;

                // Arret de tous les objets
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                int count = 0;
                int no;
                for (no = 0; no < rhNObjects; no++)
                {
                    while (rhObjectList[count] == null)
                    {
                        count++;
                    }
                    CObject hoPtr = rhObjectList[count];
                    count++;
                    if (hoPtr.hoType == COI.OBJ_CCA)
                    {
                        ((CCCA)hoPtr).pause();
                    }
                    else if (hoPtr.hoType >= COI.KPX_BASE)
                    {
                        CExtension e = (CExtension)hoPtr;
                        e.ext.pauseRunObject();
                    }
                }

                // Arret des musiques et des sons
                // ------------------------------
                rhApp.soundPlayer.pause();

                // Remet eventuellement la souris
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                showMouse();
            }
        }

        public void resume()
        {
            // Uniquement au dernier retour
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (rh2PauseCompteur != 0)
            {
                rh2PauseCompteur=Math.Max(rh2PauseCompteur-1, 0);
                if (rh2PauseCompteur == 0)
                {
                    // Enleve eventuellement la souris
                    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if !WINDOWS_PHONE
                    if (rhMouseUsed != 0)
                    {
                        MouseState state = Mouse.GetState();
                        rh2MouseSaveX = state.X;
                        rh2MouseSaveY = state.Y;
                        hideMouse();
                        Mouse.SetPosition(rh4MouseXCenter, rh4MouseYCenter);
                    }
                    else
                    {
                        if (rh4CursorShown==false)
                        {
                            hideMouse();
                        }
                    }
#endif
                    // Remet tous les objets
                    // ~~~~~~~~~~~~~~~~~~~~~
                    int count = 0;
                    int no;
                    for (no = 0; no < rhNObjects; no++)
                    {
                        while (rhObjectList[count] == null)
                        {
                            count++;
                        }
                        CObject hoPtr = rhObjectList[count];
                        count++;
                        if (hoPtr.hoType == COI.OBJ_CCA)
                        {
                            ((CCCA)hoPtr).resume();
                        }
                        else if (hoPtr.hoType >= COI.KPX_BASE)
                        {
                            CExtension e = (CExtension)hoPtr;
                            e.ext.continueRunObject();
                        }
                    }

                    // Remet les musiques et les sons
                    // ------------------------------
                    rhApp.soundPlayer.resume();

                    // Nettoie le clavier
                    // ------------------
//                    rhApp.flushKeyboard();

                    // Remet le timer
                    // ~~~~~~~~~~~~~~
                    rhTimerOld += (int)(rhApp.timer - rh2PauseTimer);
                    rhTimerFPSOld += (int)(rhApp.timer - rh2PauseFPSTimer);

                    // Remet le VBL
                    // ~~~~~~~~~~~~
                    rhVBLOld = rhApp.newGetCptVbl() - rh2PauseVbl;
                    rh4PauseKey = 0;
                    bCheckResume = false;
                }
            }
        }

        // --------------------------------------------------------------------
        // ARRET DE LA MUSIQUE ET DES SONS
        // --------------------------------------------------------------------
        public void f_StopSamples()
        {
            rhApp.soundPlayer.stopAllSounds();
        }




        
        //////////////////////////////////////////////////////////////////////////////
        //
        // Draw background
        //
        public void redrawLevel ( int flags )
        {
	        int			lgEdit, htEdit;
            int         i, obst, v, x2edit, y2edit;
            bool        cm_box;
            bool        flgColMaskEmpty = false;
	        short      img;
	        CLO		    plo;
	        CObject		hoPtr=null;
	        int			dxLayer, dyLayer;
	        bool		bTotalColMask = ((rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0);
	        bool		bUpdateColMask = ((flags & DLF_DONTUPDATECOLMASK) == 0);
	        bool		bSkipLayer0 = ((flags & DLF_SKIPLAYER0) != 0);
	        CRect		rc=new CRect();
	        int			nLayer;
	        bool		bLayer0Invisible = false;

        //	if ( flags & DLF_RESTARTLEVEL )
        //	{
		        rc.left = rc.top = 0;
		        rc.right = rhApp.gaCxWin;
		        rc.bottom = rhApp.gaCyWin;
        //	}
        //	else
        //		GetClientRect (hEditWin, &rc);
	        lgEdit = rc.right;
	        x2edit = lgEdit - 1;
	        htEdit = rc.bottom;
	        y2edit = htEdit - 1;

	        // Hide or show layers? => hide or show objects
            if ((flags & (DLF_DRAWOBJECTS | DLF_STARTLEVEL | DLF_RESTARTLEVEL)) != 0)
	        {
		        for (nLayer=0 ; nLayer < (int)rhFrame.nLayers; nLayer++)
		        {
			        CLayer pLayer = rhFrame.layers[nLayer];
			        if ( (pLayer.dwOptions & CLayer.FLOPT_TOSHOW) != 0 )
				        f_ShowAllObjects(nLayer, true);
			        if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
				        f_ShowAllObjects(nLayer, false);
		        }
	        }

	        // If DLF_REDRAWLAYER flag, skip layer 0 if not redrawable
	        if ( !bSkipLayer0 && (flags & DLF_REDRAWLAYER) != 0 )
	        {
		        CLayer pLayer0 = rhFrame.layers[0];
		        if ( (pLayer0.dwOptions & CLayer.FLOPT_REDRAW) == 0 )
			        bSkipLayer0 = true;
	        }

	        // Hide background objects from layers to hide
	        // Build 248 : déplacé ici avant le F_UpdateWindowPos
	        for (nLayer = 0; nLayer < (int)rhFrame.nLayers; nLayer++)
	        {
		        CLayer pLayer = rhFrame.layers[nLayer];

		        // Hide layer?
		        if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
		        {
			        // Delete background sprites
			        int nLOs = (int)pLayer.nBkdLOs;

			        for (i=0; i<nLOs; i++)
			        {
                        plo = rhFrame.LOList.getLOFromIndex((short)(pLayer.nFirstLOIndex + i));

                        // Delete sprite
				        for (int ns=0; ns<4; ns++)
				        {
					        if ( plo.loSpr[ns] != null )
					        {
						        rhApp.spriteGen.delSprite(plo.loSpr[ns]);
						        plo.loSpr[ns] = null;
					        }
				        }
			        }
		        }
	        }

	        // Clear background and update objects
	        if ((flags & DLF_DRAWOBJECTS)!=0)
	        {
		        // Redraw layer? force sprites to be cleared
		        if ( (flags & DLF_REDRAWLAYER) != 0 )
		        {
			        // Force re-display of all the sprites
			        //rhApp.spriteGen.ActiveSprite (idEditWin, NULL, AS_REDRAW);

			        // Marche pas, il doit falloir rafraichir aussi les objets actifs?
        /*			for (int i=1; i < (int)pCurFrame.m_nLayers; i++)
			        {
				        RunFrameLayer* pLayer = &pCurFrame.m_pLayers[i];
				        if ( (pLayer.dwOptions & FLOPT_REDRAW) != 0 )
				        {
					        int l, ns;
					        int nLOs = (int)pLayer.nBkdLOs;
					        plo = &pCurFrame.m_los[pLayer.nFirstLOIndex];

					        for (l=0; l<nLOs; l++, NEXT_LO(plo))
					        {
						        for (ns=0; ns<4; ns++)
						        {
							        npSpr	pSpr = plo.loSpr[ns];
							        if ( pSpr != NULL )
								        ActiveSprite (idEditWin, pSpr, AS_REDRAW);
						        }
					        }
					        if ( pLayer.m_pBkd2 != NULL )
					        {
						        LPBKD2 pbkd=pLayer.m_pBkd2;
						        for (l=0; l<pLayer.m_nBkd2Count; l++, pbkd++)
						        {
							        for (ns=0; ns<4; ns++)
							        {
								        npSpr	pSpr = pbkd.pSpr[ns];
								        if ( pSpr != NULL )
									        ActiveSprite (idEditWin, pSpr, AS_REDRAW);
							        }
						        }
					        }
				        }
			        } */
		        }

		        // Update active sprites and clear background
		        f_UpdateWindowPos(rhFrame.leX, rhFrame.leY);

		        // Force re-display of all the sprites
//		        spriteGen.ActiveSprite(NULL, AS_REDRAW);
	        }

	        // Erase collisions bitmap
	        if ( rhFrame.colMask!=null && bUpdateColMask )
	        {
		        rhFrame.colMask.fillRectangle (-32767, -32767, +32767, +32767, 0);
		        flgColMaskEmpty = true;
	        }

	        int nPlayfieldWidth = rhFrame.leWidth;
	        int nPlayfieldHeight = rhFrame.leHeight;

	        nLayer = 0;
	        if ( bSkipLayer0 )
		        nLayer++;

	        // Display backdrop objects
	        for ( ; nLayer < (int)rhFrame.nLayers; nLayer++)
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
			        if ( bUpdateColMask == false )
				        continue;
			        bLayer0Invisible = true;
		        }

		        // Redraw layer?
		        if ( (flags & DLF_REDRAWLAYER) != 0 )
		        {
			        if ( (pLayer.dwOptions & CLayer.FLOPT_REDRAW) == 0 )
				        continue;
		        }
		        pLayer.dwOptions &= ~CLayer.FLOPT_REDRAW;

		        // Get layer offset
		        bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
		        bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
		        bool bWrap = (bWrapHorz | bWrapVert);
		        dxLayer = rhFrame.leX;
		        dyLayer = rhFrame.leY;

		        // Apply layer scrolling coefs
		        if ( (pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0 )
		        {
			        if ( (pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0 )
				        dxLayer = (int)((float)dxLayer * pLayer.xCoef);
			        if ( (pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0 )
				        dyLayer = (int)((float)dyLayer * pLayer.yCoef);
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
		        int nLOs = (int)pLayer.nBkdLOs;

		        // Hide layer?
		        if ( (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) != 0 )
		        {
			        // Hide all objects
			        f_ShowAllObjects(nLayer, false);

			        // Layer 0 => set invisible flag and redraw it (for collision mask)
			        if ( nLayer == 0 )
				        bLayer0Invisible = true;
		        }

		        // Display layer
		        if ( (pLayer.dwOptions&CLayer.FLOPT_VISIBLE)!=0 && (pLayer.dwOptions & CLayer.FLOPT_TOHIDE) == 0 || nLayer == 0 )
		        {
			        bool bSaveBkd = ((pLayer.dwOptions & CLayer.FLOPT_NOSAVEBKD) == 0);

			        // Show layer? show all objects
			        if ( (pLayer.dwOptions & CLayer.FLOPT_TOSHOW) != 0 )
			        {
				        pLayer.dwOptions &= ~CLayer.FLOPT_TOSHOW;
				        f_ShowAllObjects(nLayer, true);
			        }

			        // Display background objects
			        uint dwWrapFlags = 0;
			        int nSprite = 0;
                    int index;
			        for (index=0; index<nLOs; index++)
			        {
                        plo=rhFrame.LOList.getLOFromIndex((short)(index+pLayer.nFirstLOIndex));
				        bool bOut = true;

				        int nCurSprite = nSprite;
                        int ppSpr=nSprite;

				        do {
					        COI poi=null;
					        COC poc=null;
                            CObjectCommon pOCommon=null;
                            int typeObj = plo.loType;

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
						        if ( poi==null || poi.oiOC==null )
							        { dwWrapFlags = 0; nSprite = 0; break; }
						        poc = poi.oiOC;
                                pOCommon = (CObjectCommon)poc;
						        if ( (pOCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0 || (hoPtr = find_HeaderObject (plo.loHandle)) == null)
							        { dwWrapFlags = 0; nSprite = 0; break; }
						        rc.left = hoPtr.hoX - rhFrame.leX - hoPtr.hoImgXSpot;
						        rc.top = hoPtr.hoY - rhFrame.leY - hoPtr.hoImgYSpot;
                                hoPtr.getZoneInfos();
					        }

					        // On the right of the display? next object (only if no total colmask)
					        if ( !bTotalColMask && !bWrap && (rc.left >= x2edit + COLMASK_XMARGIN + 32 || rc.top >= y2edit + COLMASK_YMARGIN) )
						    { 
                                dwWrapFlags = 0; 
                                nSprite = 0; 
                                break; 
                            }

					        // Get object rectangle
					        if ( typeObj < COI.OBJ_SPR )
					        {
						        poi = rhApp.OIList.getOIFromHandle(plo.loOiHandle);
						        if ( poi==null || poi.oiOC==null )
							        { dwWrapFlags = 0; nSprite = 0; break; }
						        poc = poi.oiOC;

						        rc.right = rc.left + poc.ocCx;
						        rc.bottom = rc.top + poc.ocCy;
						        v = poc.ocObstacleType;
						        cm_box = (poc.ocColMode!=0);
					        }
					        else
					        {
						        rc.right = rc.left + hoPtr.hoImgWidth;
						        rc.bottom = rc.top + hoPtr.hoImgHeight;
                                v = ((pOCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT);
                                cm_box = ((pOCommon.ocFlags2 & CObjectCommon.OCFLAGS2_COLBOX) != 0);
					        }

					        // Wrap
					        if ( bWrap )
					        {
						        switch ( nSprite ) {

						        // Normal sprite: test if other sprites should be displayed
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
							        if ( (dwWrapFlags & WRAP_X) == 0 && plo.loSpr[1] != null)
							        {
								        rhApp.spriteGen.delSprite(plo.loSpr[1]);
								        plo.loSpr[1] = null;
							        }
							        if ( (dwWrapFlags & WRAP_Y) == 0 && plo.loSpr[2] != null )
							        {
                                        rhApp.spriteGen.delSprite(plo.loSpr[2]);
								        plo.loSpr[2] = null;
							        }
							        if ( (dwWrapFlags & WRAP_XY) == 0 && plo.loSpr[3] != null )
							        {
                                        rhApp.spriteGen.delSprite(plo.loSpr[3]);
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

					        // Ladder?
					        if ( v == OBSTACLE_LADDER )
					        {
						        y_Ladder_Add (nLayer, rc.left, rc.top, rc.right, rc.bottom);
						        cm_box = true;		// Fill rectangle in collision masque
					        }

					        // Obstacle in layer 0?
					        if ( rhFrame.colMask!=null && nLayer == 0 && bUpdateColMask && v != OBSTACLE_TRANSPARENT &&
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

						        // Ajouter le masque "obstacle"
						        if ( flgColMaskEmpty == false )
						        {
							        if ( /*typeObj==OBJ_BOX || v==OBSTACLE_LADDER ||*/ cm_box==true )
							        {
								        rhFrame.colMask.fillRectangle (rc.left, rc.top, (rc.right-1), (rc.bottom-1), obst);
							        }
							        else
							        {
//								        if ( (poi.oiLoadFlags & OILF_ELTLOADED) == 0 )
//									        LoadOnCall(poi);
								        if ( pMask == null )
								        {
									        if ( typeObj < COI.OBJ_SPR )
									        {
										        img = ((COCBackground)poc).ocImage;
                                                CImage image=rhApp.imageBank.getImageFromHandle(img);										        
                                                pMask = image.getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
									        }
									        else
									        {
									            pMask = hoPtr.getCollisionMask(CMask.GCMF_OBSTACLE);
									        }
								        }
								        if ( pMask == null )
								        {
									        rhFrame.colMask.fillRectangle(rc.left, rc.top, (rc.right-1), (rc.bottom-1), obst);
								        }
								        else
								        {
                                            rhFrame.colMask.orMask(pMask, rc.left, rc.top, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, obst);
								        }
							        }
						        }

						        // Ajouter le masque plateforme
						        if ( v == COC.OBSTACLE_PLATFORM )
						        {
							        flgColMaskEmpty = false;
							        if ( /*typeObj==OBJ_BOX ||*/ cm_box==true )
							        {
								        rhFrame.colMask.fillRectangle (rc.left, rc.top, (rc.right-1), (Math.Min((int)(rc.top+CColMask.HEIGHT_PLATFORM),(int)rc.bottom)-1), CColMask.CM_PLATFORM);
							        }
							        else
							        {
								        if ( pMask == null )
								        {
									        if ( typeObj < COI.OBJ_SPR )
									        {
										        img = ((COCBackground)poc).ocImage;
                                                CImage image=rhApp.imageBank.getImageFromHandle(img);										        
                                                pMask = image.getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
									        }
									        else
									        {
									            pMask = hoPtr.getCollisionMask(CMask.GCMF_OBSTACLE);
									        }
								        }
								        if ( pMask == null )
								        {
									        rhFrame.colMask.fillRectangle (rc.left, rc.top, (rc.right-1), (Math.Min((int)(rc.top+CColMask.HEIGHT_PLATFORM),(int)rc.bottom)-1), CColMask.CM_PLATFORM);
								        }
								        else
								        {
									        rhFrame.colMask.orPlatformMask (pMask, rc.left, rc.top);
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
					        if ( rc.left <= x2edit && rc.top <= y2edit && rc.right >= 0 && rc.bottom >= 0 )
					        {
						        // In "displayable" area
						        bOut = false;

//						        // Load on call
//						        if ( (poi.oiLoadFlags & OILF_ELTLOADED) == 0 )
//							        LoadOnCall(poi);

						        ////////////////////////////////////////
						        // Non-background layer => create sprite

						        if ( nLayer > 0 || !bLayer0Invisible )
						        {
                                    uint dwFlags = CSprite.SF_BACKGROUND | CSprite.SF_NOHOTSPOT | CSprite.SF_INACTIF;
							        if ( !bSaveBkd )
                                        dwFlags |= CSprite.SF_NOSAVE;
                                    if (nLayer > 0)
                                    {
                                        switch (v)
                                        {
                                            case OBSTACLE_SOLID:
                                                dwFlags |= (CSprite.SF_OBSTACLE | CSprite.SF_RAMBO);
                                                break;
                                            case OBSTACLE_PLATFORM:
                                                dwFlags |= (CSprite.SF_PLATFORM | CSprite.SF_RAMBO);
                                                break;
                                        }
                                    }
							        // Create sprite only if not already created
							        if ( plo.loSpr[ppSpr] == null )
							        {
								        switch ( typeObj ) {

								        // QuickBackdrop: ownerdraw sprite
								        case COI.OBJ_BOX:
									        plo.loSpr[ppSpr] = rhApp.spriteGen.addOwnerDrawSprite(rc.left, rc.top, rc.right, rc.bottom, 
																	        plo.loLayer, index*4+nCurSprite, 0, (dwFlags | CSprite.SF_COLBOX), null, (IDrawing)poc);
									        break;

								        // Backdrop: sprite
								        case COI.OBJ_BKD:
									        plo.loSpr[ppSpr] = rhApp.spriteGen.addSprite(rc.left, rc.top, ((COCBackground)poc).ocImage, 
															        plo.loLayer, index*4+nCurSprite, 0, dwFlags, null);
									        rhApp.spriteGen.modifSpriteEffect(plo.loSpr[nSprite], poi.oiInkEffect, poi.oiInkEffectParam);
									        break;

								        // Extension
								        default:
									        if ( hoPtr != null )
									        {
                                                plo.loSpr[ppSpr] = rhApp.spriteGen.addOwnerDrawSprite(rc.left, rc.top, rc.right, rc.bottom,
                                                                                plo.loLayer, index * 4 + nCurSprite, 0, (dwFlags | CSprite.SF_COLBOX), null, (IDrawing)hoPtr);
									        }
									        break;
								        }
							        }

							        // Otherwise update sprite coordinates
							        else
							        {
								        switch ( typeObj ) {

								        // QuickBackdrop: ownerdraw sprite
								        case COI.OBJ_BOX:
									        {
										        CRect rcSpr=plo.loSpr[ppSpr].getSpriteRect();
										        if ( rc.left != rcSpr.left || rc.top != rcSpr.top || rc.right != rcSpr.right || rc.bottom != rcSpr.bottom )
											        rhApp.spriteGen.modifOwnerDrawSprite(plo.loSpr[ppSpr], rc.left, rc.top, rc.right, rc.bottom);
									        }
									        break;

								        // Backdrop: sprite
								        case COI.OBJ_BKD:
									        rhApp.spriteGen.modifSprite(plo.loSpr[ppSpr], rc.left, rc.top, ((COCBackground)poc).ocImage);
									        break;

								        // Extension: TODO: force the object to be re-displayed?
								        default:
                                            if (hoPtr != null)
                                            {
                                                rhApp.spriteGen.modifOwnerDrawSprite(plo.loSpr[ppSpr], rc.left, rc.top, rc.right, rc.bottom);
                                            }
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
					        if ( plo.loSpr[ppSpr] != null )
					        {
						        rhApp.spriteGen.delSprite(plo.loSpr[ppSpr]);
						        plo.loSpr[ppSpr] = null;
					        }

					        // Discard object
	        //				#ifdef ALLOW_DISCARD
	        //					if ( poi.oiLoadCount == poi.oiCount )
	        //						DiscardWhenNoMore (poi);
	        //				#endif
				        }

				        // Re-display the same object but wrapped
				        if ( dwWrapFlags != 0 )
				        {
                            index--;
				        }
			        }
		        }

		        // Display backdrop objects created at runtime
		        if ( pLayer.pBkd2 != null )
			        displayBkd2Layer(pLayer, nLayer, flags, x2edit, y2edit, flgColMaskEmpty);

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
				        dxLayer = (int)((float)dxLayer * pLayer.xCoef);
			        if ( (pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0 )
				        dyLayer = (int)((float)dyLayer * pLayer.yCoef);
		        }

		        // Add layer offset
		        dxLayer += pLayer.x;
		        dyLayer += pLayer.y;

                if (rhFrame.colMask != null)
                {
                    rhFrame.colMask.setOrigin(dxLayer, dyLayer);
                }
	        }
        }

        public void ohRedrawLevel(bool bRedrawTotalColMask)
        {
            rh3Scrolling |= RH3SCROLLING_REDRAWALL;
            if (bRedrawTotalColMask)
            {
                rh3Scrolling |= RH3SCROLLING_REDRAWTOTALCOLMASK;
            }
        }

                // Scroll frame
        /** Scrolls the level.
         */
        void scrollLevel()
        {
	        int xSrc, ySrc, xDest, yDest;
	        bool flgScroll, flgScrollMask;
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
		        oX = (int) ((float) oX * xCoef);
		        nX = (int) ((float) nX * xCoef);
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
		        oY = (int) ((float) oY * yCoef);
		        nY = (int) ((float) nY * yCoef);
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
        	
	        // Update coordinates
	        rhFrame.leX = rh3DisplayX;
	        rhFrame.leY = rh3DisplayY;
        	
	        // Clear sprites
	        rhApp.spriteGen.activeSprite(null, CSpriteGen.AS_REDRAW, null);		// AS_DEACTIVATE
        	
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
        	
	        // Si pas eu de scrolling, reafficher tout
	        if (flgScroll == false)
	        {
                redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK);
	        }
        	
	        // Si scrolling effectue, refaire un redraw clippe
	        else
	        {
		        bool bRedrawDone = false;

		        if (xSrc != 0 || xDest != 0)
		        {
			        // Ajouter zone totale pour rafraichissement fenetre
			        if (flgScrollMask != false)
			        {
				        //		    ColMask_SetClip (idEditWin, &rcClipH);
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
			        // Ajouter zone totale pour rafraichissement fenetre
			        if (flgScrollMask !=false)
				        redrawLevel(DLF_DONTUPDATE | DLF_COLMASKCLIPPED);
			        else
				        redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK);
        			
			        bRedrawDone = true;
		        }
        		
		        // Redraw not done? redraw layers greater than 0
		        if (!bRedrawDone && rhFrame.nLayers > 0)
		        {
			        if ((rhFrame.layers[0].dwOptions & (CLayer.FLOPT_REDRAW)) != 0)
				        redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK);
			        else
				        redrawLevel(DLF_DONTUPDATE | DLF_DONTUPDATECOLMASK | DLF_SKIPLAYER0);
		        }
	        }
        }

        // Update of level coordinates in case of a scrolling but when everything has to be redrawn
        void updateScrollLevelPos()
        {
            int xSrc, ySrc, xDest, yDest;

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
                oX = (int)((float)oX * xCoef);
                nX = (int)((float)nX * xCoef);
            }

            // Add layer offset
            //	oX += pLayer.x;
            //	nX += pLayer.x;

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
                    rhFrame.leLastScrlX = rh3DisplayX;
            }

            int oY = rhFrame.leLastScrlY;
            int nY = rh3DisplayY;

            if (yCoef != 1.0f)
            {
                oY = (int)((float)oY * yCoef);
                nY = (int)((float)nY * yCoef);
            }

            // Add layer offset
            //	oY += pLayer.y;
            //	nY += pLayer.y;

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
                    rhFrame.leLastScrlY = rh3DisplayY;
            }

            // Update coordinates
            rhFrame.leX = rh3DisplayX;
            rhFrame.leY = rh3DisplayY;

        }

        public void screen_Update()
        {	
	        int background;
	        if(rhApp.frame != null)
		        background = rhApp.frame.leBackground;
	        else
		        background = rhApp.gaBorderColour;

            Color c = CServices.getColor(background);
            if (rhApp.parentApp == null)
            {
                rhApp.graphicsDevice.Clear(c);
            }
            else
            {
                if (rhApp.bSubAppShown == false)
                {
                    return;
                }
                rhApp.services.drawFilledRectangleSub(rhApp.spriteBatch, rhApp.xOffset, rhApp.yOffset, rhApp.parentWidth, rhApp.parentHeight, c, CSpriteGen.BOP_COPY, 0);
            }

            if (rh3Scrolling!=0)
	        {
		        if ((rh3Scrolling&RH3SCROLLING_REDRAWALL)!=0)
		        {
			        // Update scroll pos if scrolling
			        if (rhFrame.leX != rh3DisplayX || rhFrame.leY != rh3DisplayY)
				        updateScrollLevelPos();
        			
			        // Redraw everything 
			        int flags = DLF_DRAWOBJECTS;
			        if ((rh3Scrolling & RH3SCROLLING_REDRAWTOTALCOLMASK) == 0 && (rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0)
				        flags |= DLF_DONTUPDATECOLMASK;
			        redrawLevel(flags);
        			
			        rh3DisplayX=rhWindowX;
			        rh3DisplayY=rhWindowY;		
		        }
		        else if ((rh3Scrolling&RH3SCROLLING_SCROLL)!=0)
		        {
			        // Update scroll pos if scrolling
			        if (rhFrame.leX != rh3DisplayX || rhFrame.leY != rh3DisplayY)
			        {
				        scrollLevel();
			        }
		        }
        		else
		        {
			        redrawLevel(DLF_DONTUPDATECOLMASK|DLF_DRAWOBJECTS|DLF_REDRAWLAYER);
		        }
	        }
        	
	        rhApp.spriteGen.spriteUpdate();
	        rhApp.spriteGen.spriteDraw(rhApp.spriteBatch);
//	        draw_QuickDisplay(rhApp.spriteBatch);
	        rh3Scrolling=0;

            if (questionObjectOn != null)
            {
                questionObjectOn.draw(rhApp.spriteBatch);
            }
            if (nSubApps != 0)
            {
                int count = 0;
                int no;
                for (no = 0; no < rhNObjects; no++)
                {
                    while (rhObjectList[count] == null)
                    {
                        count++;
                    }
                    CObject hoPtr = rhObjectList[count];
                    count++;
                    if (hoPtr.hoType == COI.OBJ_CCA)
                    {
                        ((CCCA)hoPtr).draw(rhApp.spriteBatch);
                    }
                }
            }
            if (nControls != 0)
            {
                int n;
                for (n = 0; n < nControls; n++)
                {
                    IControl control = (IControl)controls.get(n);
                    control.drawControl(rhApp.spriteBatch);
                }
            }
#if WINDOWS_PHONE
            if (joystick != null)
            {
                joystick.draw(rhApp.spriteBatch);
            }
#endif
        }

        // -----------------------------------------------------------------------
        // TROUVE LE HO
        // -----------------------------------------------------------------------
        public CObject find_HeaderObject(short hlo)
        {
            int count = 0;
            for (int nObjects = 0; nObjects < rhNObjects; nObjects++)
            {
                while (rhObjectList[count] == null)
                {
                    count++;
                }
                if (hlo == rhObjectList[count].hoHFII)
                {
                    return rhObjectList[count];
                }
                count++;
            }
            return null;
        }

        // -----------------------------------------------------------------------
        // SCROLLING DU TERRAIN, UPDATE LES POSITIONS
        // -----------------------------------------------------------------------
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
            rh4FirstQuickDisplay = -1;
            rh4LastQuickDisplay = -1;

            int count = 0;
            for (int nObjects = 0; nObjects < rhNObjects; nObjects++)
            {
                while (rhObjectList[count] == null)
                {
                    count++;
                }
                CObject pHo = rhObjectList[count];
                count++;

                if (noMove != 0)
                {
                    if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_SCROLLINGINDEPENDANT) != 0)
                    {
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
                                oldLayerDx = (int) (pLayer.xCoef * (float) oldLayerDx);
                                newLayerDx = (int) (pLayer.xCoef * (float) newLayerDx);
                            }
                            if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
                            {
                                oldLayerDy = (int) (pLayer.yCoef * (float) oldLayerDy);
                                newLayerDy = (int) (pLayer.yCoef * (float) newLayerDy);
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

                    if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0)					// protection ajoutée build 96: crash si Q/A object + scrolling
                    {
                        pHo.modif();
                    }
                }
                else if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0)
                {
                    pHo.display();
                }
            }
        }

        public void f_ShowAllObjects(int nLayer, bool bShow)
        {
            int count = 0;
            int nObject;
            for (nObject = 0; nObject < rhNObjects; nObject++)
            {
                while (rhObjectList[count] == null)
                {
                    count++;
                }
                CObject hoPtr = rhObjectList[count];
                count++;

                if (nLayer == hoPtr.hoLayer || nLayer == CSpriteGen.LAYER_ALL)
                {
                    if (hoPtr.ros != null)
                    {
                        if (hoPtr.roc.rcSprite != null)
                        {
                            rhApp.spriteGen.activeSprite(hoPtr.roc.rcSprite, CSpriteGen.AS_REDRAW, null);
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
            }
        }

        // Centre le display en respectant les bords
        // -----------------------------------------
        public void setDisplay(int x, int y, int nLayer, int flags)
        {
            x -= rh3WindowSx / 2;				//; Taille de la fenetre d'affichage
            y -= rh3WindowSy / 2;

            float xf = (float) x;
            float yf = (float) y;

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
                x = 0;					// Sort à haut/gauche?
            }
            if (y < 0)
            {
                y = 0;
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
                    rh3Scrolling |= (byte)RH3SCROLLING_SCROLL;
                }
            }
            if ((flags & 2) != 0)
            {
                if (y != rhWindowY)
                {
                    rh3DisplayY = y;
                    rh3Scrolling |= (byte)RH3SCROLLING_SCROLL;
                }
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
                rc.left = Math.Min(x1, x2);
                rc.top = Math.Min(y1, y2);
                rc.right = Math.Max(x1, x2);
                rc.bottom = Math.Max(y1, y2);

                if (pLayer.pLadders == null)
                {
                    pLayer.pLadders = new CArrayList();
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
                    rc.left = Math.Min(x1, x2);
                    rc.top = Math.Min(y1, y2);
                    rc.right = Math.Max(x1, x2);
                    rc.bottom = Math.Max(y1, y2);

                    int i;
                    CRect rcDst;
                    for (i = 0; i < pLayer.pLadders.size(); i++)
                    {
                        rcDst = (CRect)pLayer.pLadders.get(i);
                        if (rcDst.intersectRect(rc) == true)
                        {
                            pLayer.pLadders.remove(i);
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
                    for (i = 0; i < pLayer.pLadders.size(); i++)
                    {
                        rc = (CRect)pLayer.pLadders.get(i);
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
        // DESSIN DES OBJETS DANS LE DECOR
        // -------------------------------------------------------------------------
        public void activeToBackdrop(CObject hoPtr, int nTypeObst, bool bTrueObject)
        {
	        CBkd2 toadd;
	        toadd = new CBkd2();
	        toadd.img = hoPtr.roc.rcImage;
	        CImage ifo = rhApp.imageBank.getImageFromHandle(toadd.img);
	        toadd.loHnd = 0;
	        toadd.oiHnd = 0;
	        toadd.x = hoPtr.hoX - ifo.xSpot;
	        toadd.y = hoPtr.hoY - ifo.ySpot;
	        toadd.nLayer = (short) hoPtr.hoLayer;
        	
	        toadd.obstacleType = (short) nTypeObst;	// a voir
	        toadd.colMode = CSpriteGen.CM_BITMAP;
        	
	        if ((hoPtr.ros.rsCreaFlags & CSprite.SF_COLBOX) != 0)
	        {
		        toadd.colMode = CSpriteGen.CM_BOX;
	        }
        	
	        for (int ns = 0; ns < 4; ns++)
	        {
		        toadd.pSpr[ns] = null;
	        }
	        toadd.inkEffect = hoPtr.ros.rsEffect;
	        toadd.inkEffectParam = hoPtr.ros.rsEffectParam;	
	        addBackdrop2(toadd);	
        }

        /** Sub method of the above.
         */
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
		        for (i = 0; i < pLayer.pBkd2.size(); i++)
		        {
			        pbkd = (CBkd2)pLayer.pBkd2.get(i);
                    if (pbkd.x == toadd.x && pbkd.y == toadd.y && pbkd.nLayer == toadd.nLayer && pbkd.img == toadd.img && (pbkd.inkEffect & CSpriteGen.BOP_MASK) == CSpriteGen.BOP_COPY)
			        {
				        if (i != pLayer.pBkd2.size() - 1)
				        {
					        for (int j = 0; j < 4; j++)
					        {
						        if (pbkd.pSpr[j] != null)
						        {
							        rhApp.spriteGen.moveSpriteToFront(pbkd.pSpr[j]);
						        }
					        }
					        pLayer.pBkd2.remove(i);
					        pLayer.pBkd2.add(pbkd);
				        }
				        pbkd.colMode = toadd.colMode;
				        pbkd.obstacleType = toadd.obstacleType;
        				
				        if (pbkd.inkEffect != toadd.inkEffect || pbkd.inkEffectParam != toadd.inkEffectParam)
				        {
					        pbkd.inkEffect = toadd.inkEffect;
					        pbkd.inkEffectParam = toadd.inkEffectParam;
					        for (int j = 0; j < 4; j++)
					        {
						        if (pbkd.pSpr[j] != null)
						        {
							        rhApp.spriteGen.modifSpriteEffect(pbkd.pSpr[j], pbkd.inkEffect, pbkd.inkEffectParam);
						        }
					        }
				        }
				        return;
			        }
		        }
        		
		        // Maxi
/*		        if (pLayer.pBkd2.size() >= rhFrame.maxObjects)
		        {
			        return;
		        }
*/	        }
	        // Allouer m_pBkd2
	        else
	        {
		        pLayer.pBkd2 = new CArrayList();
	        }
        	
	        // Ajouter le backdrop a la fin
	        int nIdx = pLayer.pBkd2.size();
	        pLayer.pBkd2.add(toadd);
        	
	        // TODO: add sprite si layer > 0 ? (attention si layer invisible)
	        pbkd = toadd;
	        int v;
	        CRect rc = new CRect();
        	
	        short img;
	        int dxLayer, dyLayer;
        	
	        // Layer offset
	        dxLayer = rhFrame.leX;
	        dyLayer = rhFrame.leY;
        	
	        bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
	        bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
	        bool bWrap = false;
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
			        dxLayer = (int) ((float) dxLayer * pLayer.xCoef);
		        }
		        if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
		        {
			        dyLayer = (int) ((float) dyLayer * pLayer.yCoef);
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
		        bool bSaveBkd = ((pLayer.dwOptions & CLayer.FLOPT_NOSAVEBKD) == 0);
		        uint dwWrapFlags = 0;
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
			        CImage pImg = rhApp.imageBank.getImageFromHandle(img);
			        if (pImg != null)
			        {
				        rc.right = rc.left + pImg.width;
				        rc.bottom = rc.top + pImg.height;
			        }
			        else
			        {
				        rc.right = rc.left + 1;
				        rc.bottom = rc.top + 1;
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
							        rhApp.spriteGen.delSprite(pbkd.pSpr[1]);
							        pbkd.pSpr[1] = null;
						        }
						        if ((dwWrapFlags & WRAP_Y) == 0 && pbkd.pSpr[2] != null)
						        {
							        rhApp.spriteGen.delSprite(pbkd.pSpr[2]);
							        pbkd.pSpr[2] = null;
						        }
						        if ((dwWrapFlags & WRAP_XY) == 0 && pbkd.pSpr[3] != null)
						        {
							        rhApp.spriteGen.delSprite(pbkd.pSpr[3]);
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
                    // Display object?
                    if (rhFrame.colMask != null && pbkd.nLayer == 0 && pbkd.colMode != OBSTACLE_TRANSPARENT &&
                         ((rc.right >= -COLMASK_XMARGIN - 32 && rc.bottom >= -COLMASK_YMARGIN)) )
                    {
                        CMask pMask = null;

                        // Retablir coords absolues si TOTALCOLMASK
                        rc.left += dxLayer;
                        rc.top += dyLayer;
                        rc.right += dxLayer;
                        rc.bottom += dyLayer;

                        // Update collisions bitmap (meme si objet en dehors)
                        int obst = 0;
                        if (pbkd.colMode == OBSTACLE_SOLID)
                        {
                            obst = CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM;
                        }

                        // Ajouter le masque "obstacle"
		                CImage ifo = rhApp.imageBank.getImageFromHandle(toadd.img);                                    
                        pMask = ifo.getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
                        if ( pbkd.obstacleType==CSpriteGen.CM_BOX)
                        {
                            rhFrame.colMask.fillRectangle(rc.left, rc.top, (rc.right - 1), (rc.bottom - 1), obst);
                        }
                        else
                        {
                            if (pMask == null)
                            {
                                rhFrame.colMask.fillRectangle(rc.left, rc.top, (rc.right - 1), (rc.bottom - 1), obst);
                            }
                            else
                            {
                                rhFrame.colMask.orMask(pMask, rc.left, rc.top, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, obst);
                            }
                        }

                        // Ajouter le masque plateforme
                        if (pbkd.colMode == COC.OBSTACLE_PLATFORM)
                        {
                            if ( pbkd.obstacleType==CSpriteGen.CM_BOX)
                            {
                                rhFrame.colMask.fillRectangle(rc.left, rc.top, (rc.right - 1), (Math.Min((int)(rc.top + CColMask.HEIGHT_PLATFORM), (int)rc.bottom) - 1), CColMask.CM_PLATFORM);
                            }
                            else
                            {
                                if (pMask == null)
                                {
                                    rhFrame.colMask.fillRectangle(rc.left, rc.top, (rc.right - 1), (Math.Min((int)(rc.top + CColMask.HEIGHT_PLATFORM), (int)rc.bottom) - 1), CColMask.CM_PLATFORM);
                                }
                                else
                                {
                                    rhFrame.colMask.orPlatformMask(pMask, rc.left, rc.top);
                                }
                            }
                        }

                        // Retablir coords absolues si TOTALCOLMASK
                        rc.left -= dxLayer;
                        rc.top -= dyLayer;
                        rc.right -= dxLayer;
                        rc.bottom -= dyLayer;
                    }

        			
			        // On the right of the display? next object (only if no total colmask)
			        if (rc.left < x2edit + COLMASK_XMARGIN + 32 && rc.top < y2edit + COLMASK_YMARGIN)
			        {
				        // Obstacle?
				        v = pbkd.obstacleType;
        				
				        // Ladder?
				        if (v == OBSTACLE_LADDER)
				        {
					        y_Ladder_Add(pbkd.nLayer, rc.left, rc.top, rc.right, rc.bottom);
				        }
        				
				        if (rc.left <= x2edit && rc.top <= y2edit && rc.right >= 0 && rc.bottom >= 0)
				        {
					        ////////////////////////////////////////
					        // Non-background layer => create sprite
        					
					        uint dwFlags = CSprite.SF_BACKGROUND  | CSprite.SF_INACTIF; //| SF_NOHOTSPOT;
                            if (!bSaveBkd)
					        {
						        dwFlags |= CSprite.SF_NOSAVE;
					        }
                            if (pbkd.nLayer > 0)
                            {
                                if (v == COC.OBSTACLE_SOLID)
                                {
                                    dwFlags |= (CSprite.SF_OBSTACLE | CSprite.SF_RAMBO);
                                }
                                if (v == COC.OBSTACLE_PLATFORM)
                                {
                                    dwFlags |= (CSprite.SF_PLATFORM | CSprite.SF_RAMBO);
                                }
                            }
					        CImage ifo = rhApp.imageBank.getImageFromHandle(toadd.img);
                            int xx = rc.left;// +ifo.xSpot;
                            int yy = rc.top;// +ifo.ySpot;
                            dwFlags |= CSprite.SF_NOHOTSPOT;
        					
					        pbkd.pSpr[nCurSprite] = rhApp.spriteGen.addSprite(xx, yy, img, pbkd.nLayer, 0x10000000 + nIdx * 4 + nCurSprite, 0, dwFlags, null);
					        // voir si fixer variable interne a pbkd
					        rhApp.spriteGen.modifSpriteEffect(pbkd.pSpr[nCurSprite], pbkd.inkEffect, pbkd.inkEffectParam);
				        }
			        }
		        } while (dwWrapFlags != 0);
	        }
        }

        // Delete all created backdrop objects from a layer
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
                for (i = 0; i < pLayer.pBkd2.size(); i++)
                {
                    pbkd = (CBkd2)pLayer.pBkd2.get(i);
                    for (int ns = 0; ns < 4; ns++)
                    {
                        if (pbkd.pSpr[ns] != null)
                        {
                            rhApp.spriteGen.delSprite(pbkd.pSpr[ns]);
                            pbkd.pSpr[ns] = null;
                        }
                    }

                }
                pLayer.pBkd2 = null;

                // Force redraw
                pLayer.dwOptions |= CLayer.FLOPT_REDRAW;
                rh3Scrolling |= (byte)RH3SCROLLING_REDRAWLAYERS;
            }
        }

        // Delete created backdrop object at given coordinates
        public void deleteBackdrop2At(int nLayer, int x, int y, bool bFineDetection)
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
                bool bSomethingDeleted = false;

                bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
                bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
                bool bWrap = (bWrapHorz | bWrapVert);

                int nPlayfieldWidth = rhFrame.leWidth;
                int nPlayfieldHeight = rhFrame.leHeight;

                // Layer offset
                int dxLayer = rhFrame.leX;
                int dyLayer = rhFrame.leY;
                if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
                {
                    if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
                    {
                        dxLayer = (int) ((float) dxLayer * pLayer.xCoef);
                    }
                    if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
                    {
                        dyLayer = (int) ((float) dyLayer * pLayer.yCoef);
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

                uint dwWrapFlags = 0;
                int nSprite = 0;

                for (i = 0; i < pLayer.pBkd2.size(); i++)
                {
                    pbkd = (CBkd2)pLayer.pBkd2.get(i);

                    if (pbkd.nLayer == nLayer)		// Heu ... c'est la peine de faire ce test?
                    {
                        bool bFound = false;

                        // Get object position
                        CRect rc = new CRect();
                        bool cm_box = (pbkd.colMode == CSpriteGen.CM_BOX);
                        rc.left = pbkd.x - dxLayer;
                        rc.top = pbkd.y - dyLayer;

                        // Get object rectangle
                        CImage pImg = rhApp.imageBank.getImageFromHandle(pbkd.img);
                        if (pImg != null)
                        {
                            rc.right = rc.left + pImg.width;
                            rc.bottom = rc.top + pImg.height;
                        }
                        else
                        {
                            rc.right = rc.left + 1;
                            rc.bottom = rc.top + 1;
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
                                bFound = true;
                                break;
                            }

                            // Test if point into image mask
                            CMask pMask = rhApp.imageBank.getImageFromHandle(pbkd.img).getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
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
                                    rhApp.spriteGen.delSprite(pbkd.pSpr[ns]);
                                    pbkd.pSpr[ns] = null;
                                }
                            }

                            // Overwrite bkd2 structure
                            pLayer.pBkd2.remove(i);

                            // TODO : decrement image count and remove it from temp image table if doesn't exist anymore
                            // not urgent...

                            // Do not exit loop, this routine deletes all the created backdrop objects at this location
                            // break;

                            // Reset wrap flags
                            dwWrapFlags = 0;

                            // Decrement loop index
                            i--;
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
                    rh3Scrolling |= (byte)RH3SCROLLING_REDRAWLAYERS;
                }
            }
        }

        /** Displays the backdrop objects created at runtime
         */
        public void displayBkd2Layer(CLayer pLayer, int nLayer, int flags, int x2edit, int y2edit, bool flgColMaskEmpty)
        {
	        CBkd2 pbkd;
	        int i, obst, v;
	        CRect rc = new CRect();
        	
	        short img;
	        int dxLayer, dyLayer;
	        bool bTotalColMask = ((rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0);
	        bool bUpdateColMask = ((flags & DLF_DONTUPDATECOLMASK) == 0);
        	
	        // Layer offset
	        dxLayer = rhFrame.leX;
	        dyLayer = rhFrame.leY;
        	
	        bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
	        bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
	        bool bWrap = (bWrapHorz | bWrapVert);
        	
	        int nPlayfieldWidth = rhFrame.leWidth;
	        int nPlayfieldHeight = rhFrame.leHeight;
        	
	        if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
	        {
		        if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
		        {
			        dxLayer = (int) ((float) dxLayer * pLayer.xCoef);
		        }
		        if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
		        {
			        dyLayer = (int) ((float) dyLayer * pLayer.yCoef);
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
		        for (i = 0; i < pLayer.pBkd2.size(); i++)
		        {
			        pbkd = (CBkd2)pLayer.pBkd2.get(i);
        			
			        // Delete sprite
			        for (int ns = 0; ns < 4; ns++)
			        {
				        if (pbkd.pSpr[ns] != null)
				        {
					        rhApp.spriteGen.delSprite(pbkd.pSpr[ns]);
					        pbkd.pSpr[ns] = null;
				        }
			        }
		        }
	        }
        	
	        // Display layer
	        if ((pLayer.dwOptions & CLayer.FLOPT_TOHIDE) == 0)
	        {
		        bool bSaveBkd = ((pLayer.dwOptions & CLayer.FLOPT_NOSAVEBKD) == 0);
		        uint dwWrapFlags = 0;
		        int nSprite = 0;
        		
		        for (i = 0; i < pLayer.pBkd2.size(); i++)
		        {
			        pbkd = (CBkd2)pLayer.pBkd2.get(i);
        			
			        int nCurSprite = nSprite;
        			
			        rc.left = pbkd.x - dxLayer;
			        rc.top = pbkd.y - dyLayer;
        			
			        // On the right of the display? next object (only if no total colmask)
			        if (!bTotalColMask && !bWrap && (rc.left >= x2edit + COLMASK_XMARGIN + 32 || rc.top >= y2edit + COLMASK_YMARGIN))
			        {
				        // Out of visible area: delete sprite
				        {
					        // Delete sprite
					        if (pbkd.pSpr[nCurSprite] != null)
					        {
						        rhApp.spriteGen.delSprite(pbkd.pSpr[nCurSprite]);
						        pbkd.pSpr[nCurSprite] = null;
					        }
				        }
				        continue;
			        }
        			
			        // Calculer rectangle objet
			        img = pbkd.img;
			        CImage pImg = rhApp.imageBank.getImageFromHandle(img);
			        if (pImg != null)
			        {
				        rc.right = rc.left + pImg.width;
				        rc.bottom = rc.top + pImg.height;
			        }
			        else
			        {
				        rc.right = rc.left + 1;
				        rc.bottom = rc.top + 1;
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
							        rhApp.spriteGen.delSprite(pbkd.pSpr[1]);
							        pbkd.pSpr[1] = null;
						        }
						        if ((dwWrapFlags & WRAP_Y) == 0 && pbkd.pSpr[2] != null)
						        {
							        rhApp.spriteGen.delSprite(pbkd.pSpr[2]);
							        pbkd.pSpr[2] = null;
						        }
						        if ((dwWrapFlags & WRAP_XY) == 0 && pbkd.pSpr[3] != null)
						        {
							        rhApp.spriteGen.delSprite(pbkd.pSpr[3]);
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
        			
			        v = pbkd.obstacleType;
			        bool cm_box = (pbkd.colMode == CSpriteGen.CM_BOX);
        			
			        // Ladder?
			        if (v == OBSTACLE_LADDER)
			        {
				        y_Ladder_Add(nLayer, rc.left, rc.top, rc.right, rc.bottom);
				        cm_box = true;		// Fill rectangle in collision masque
			        }
        			
			        // Obstacle in layer 0?
			        if (nLayer == 0 && bUpdateColMask && v != OBSTACLE_TRANSPARENT &&
				        (bTotalColMask || (rc.right >= -COLMASK_XMARGIN - 32 && rc.bottom >= -COLMASK_YMARGIN)))
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
				        CMask mask;
				        if (flgColMaskEmpty == false)
				        {
					        if (cm_box)
					        {
						        rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right, rc.bottom, obst);
					        }
					        else
					        {
						        mask = rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
						        rhFrame.colMask.orMask(mask, rc.left, rc.top, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, obst);
					        }
				        }
        				
				        // Ajouter le masque plateforme
				        if (v == OBSTACLE_PLATFORM)
				        {
					        flgColMaskEmpty = false;
					        if (cm_box)
					        {
						        rhFrame.colMask.fillRectangle(rc.left, rc.top, rc.right, Math.Min((int)(rc.top + CColMask.HEIGHT_PLATFORM), (int)rc.bottom), CColMask.CM_PLATFORM);
					        }
					        else
					        {
						        mask = rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
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
				        uint dwFlags = CSprite.SF_BACKGROUND | CSprite.SF_NOHOTSPOT | CSprite.SF_INACTIF;
				        if (!bSaveBkd)
				        {
					        dwFlags |= CSprite.SF_NOSAVE;
				        }
				        if (v == COC.OBSTACLE_SOLID)
				        {
					        dwFlags |= (CSprite.SF_OBSTACLE | CSprite.SF_RAMBO);
				        }
				        if (v == COC.OBSTACLE_PLATFORM)
				        {
					        dwFlags |= (CSprite.SF_PLATFORM | CSprite.SF_RAMBO);
				        }
        				
				        // Create sprite only if not already created
				        if (pbkd.pSpr[nCurSprite] == null)
				        {
					        pbkd.pSpr[nCurSprite] = rhApp.spriteGen.addSprite(rc.left, rc.top, img, pbkd.nLayer, 0x10000000 + i * 4 + nCurSprite, 0, dwFlags, null);
					        rhApp.spriteGen.modifSpriteEffect(pbkd.pSpr[nCurSprite], pbkd.inkEffect, pbkd.inkEffectParam);
				        }
        				
				        // Otherwise update sprite coordinates
				        else
				        {
					        rhApp.spriteGen.modifSprite(pbkd.pSpr[nCurSprite], rc.left, rc.top, img);
				        }
			        }
        			
			        // Object out of visible area: delete sprite
			        else
			        {
				        {
					        // Delete sprite
					        if (pbkd.pSpr[nCurSprite] != null)
					        {
						        rhApp.spriteGen.delSprite(pbkd.pSpr[nCurSprite]);
						        pbkd.pSpr[nCurSprite] = null;
					        }
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

        public void f_InitLoop()
        {
            int n;
            long tick = rhApp.timer;
            rhTimerOld = tick;
            rhTimerFPSOld = tick;
            rhTimer = 0;

            rhLoopCount = 0;
            rh4LoopTheoric = 0;

            rhVBLOld = rhApp.newGetCptVbl() - 1;	    // Force un premier tour effectif!
            rh4VBLDelta = 0;

            rhQuit = 0;						// On ne sort pas!
            rhQuitBis = 0;
            rhDestroyPos = 0;				// Destroy list

            for (n = 0; n < (rhMaxObjects + 31) / 32; n++)		// Yves: ajout du +31: il faut aussi modifier la routine d'allocation
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
            for (n = 0; n < 4; n++)
            {
                rhPlayer[n] = 0;
                rh2OldPlayer[n] = 0;
                rh2InputMask[n] = (byte) 0xFF;
            }
            rh2MouseKeys = 0;
            oldMouseKey = -1;
            toucheID = -1;
            mouseKeyTime = 0;
            // Recentrer la souris si necessaire
            if (rhMouseUsed != 0)
            {
                rh4MouseXCenter = rhApp.gaCxWin/ 2;
                rh4MouseYCenter = rhApp.gaCyWin/ 2;
                Mouse.SetPosition(rh4MouseXCenter, rh4MouseYCenter);
            }

            // RAZ flags actions
            rhEvtProg.rh2ActionEndRoutine = false;
            rh4OnCloseCount = -1;
            rh4EndOfPause = -1;
//            rh4OnMouseWheel = -1;
//            rh4LoadCount = -1;
            rhEvtProg.rh4CheckDoneInstart = false;
            rh4PauseKey = 0;
            bCheckResume = false;

            // Sons
            rhApp.soundPlayer.reset();

            // Pas de demo
//            rh4DemoMode = CDemoRecord.DEMONOTHING;
//            rh4Demo = null;

            // RAZ du buffer de calcul du framerate
            for (n = 0; n < MAX_FRAMERATE; n++)
            {
                rh4FrameRateArray[n] = 20;				// initialisation à 1/50eme de seconde
            }
            rh4FrameRatePos = 0;
#if WINDOWS_PHONE
            for (n=0; n<rhApp.numberOfTouches; n++)
            {
                cancelledTouches[n]=-1;
            }
#endif
            // Initialisation Any Key
            if (rhEvtProg.bTestAllKeys)
            {
                n = 0;
                do
                {
                    if (keyboardState.IsKeyDown(CKeyConvert.xnaKeys[n]))
                    {
                        bAnyKeyDown = true;
                        break;
                    }
                    n++;
                } while (CKeyConvert.pcKeys[n] >= 0);
            }
#if WINDOWS_PHONE
	        // Joystick creation
            if (rhApp.parentApp == null)
            {
                if (rhFrame.joystick == CRunFrame.JOYSTICK_EXT)
                {
                    if (joystick == null)
                    {
                        joystick = new CJoystick(rhApp);
                    }
                    else
                    {
                        joystick.reset(0);
                    }
                }
                else
                {
                    int flags = 0;
                    if ((rhFrame.iPhoneOptions & CRunFrame.IPHONEOPT_JOYSTICK_FIRE1) != 0)
                    {
                        flags = CJoystick.JFLAG_FIRE1;
                    }
                    if ((rhFrame.iPhoneOptions & CRunFrame.IPHONEOPT_JOYSTICK_FIRE2) != 0)
                    {
                        flags |= CJoystick.JFLAG_FIRE2;
                    }
                    if ((rhFrame.iPhoneOptions & CRunFrame.IPHONEOPT_JOYSTICK_LEFTHAND) != 0)
                    {
                        flags |= CJoystick.JFLAG_LEFTHANDED;
                    }
                    if (rhFrame.joystick == CRunFrame.JOYSTICK_TOUCH)
                    {
                        flags |= CJoystick.JFLAG_JOYSTICK;
                    }
                    if ((flags & (CJoystick.JFLAG_FIRE1 | CJoystick.JFLAG_FIRE2 | CJoystick.JFLAG_JOYSTICK)) != 0)
                    {
                        if (joystick == null)
                        {
                            joystick = new CJoystick(rhApp);
                        }
                        joystick.reset(flags);
                    }
                    else
                    {
                        joystick = null;
                    }

                    // Accelerometer joystick
                    if (rhFrame.joystick == CRunFrame.JOYSTICK_ACCELEROMETER)
                    {
                        startJoystickAcc();
                    }
                    else
                    {
                        stopJoystickAcc();
                    }
                }
            }
#endif
            rhJoystickMask = 0xFF;
        }

        public void handleFrameRaate()
        {
            long timerBase = rhApp.timer;
            long delta = timerBase - rhTimerFPSOld;
            rhTimerFPSOld = timerBase;

            // Gestion du framerate
            rh4FrameRateArray[rh4FrameRatePos] = (int)delta;
            rh4FrameRatePos++;
            if (rh4FrameRatePos >= MAX_FRAMERATE)
            {
                rh4FrameRatePos = 0;
            }
        }

        public int f_GameLoop()
        {

            keyboardState = Keyboard.GetState();

            // On est en pause?
            // ~~~~~~~~~~~~~~~~
            if (rh2PauseCompteur != 0)
            {
                if (questionObjectOn != null)
                {
                    questionObjectOn.handleQuestion();
                }
                return 0;
            }

            // Gestion des sons
            rhApp.soundPlayer.checkSounds();

            // Recupere l'horloge, le nombre de loops
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            long timerBase = rhApp.timer;
            long delta =timerBase - rhTimerOld;
            long oldtimer = rhTimer;
            rhTimer = delta;
            delta -= oldtimer;
            rhTimerDelta = (int) delta;				// Delta a la boucle precedente
            rh4TimeOut += delta;			// Compteur time-out.
            rhLoopCount += 1;
            rh4MvtTimerCoef = ((double) rhTimerDelta) * ((double) rhFrame.m_dwMvtTimerBase) / 1000.0;

            // Si mode demo
            // ~~~~~~~~~~~~
/*            if (rh4DemoMode == CDemoRecord.DEMOPLAY)
            {
                rh4Demo.playStep();
            }
*/
            // Le joystick
            // ~~~~~~~~~~~

            int n;
            for (n = 0; n < 4; n++)
            {
                rh2OldPlayer[n] = rhPlayer[n];
            }
#if !WINDOWS_PHONE
            joyTest();
#else
            if (joystick != null)
            {
                rhPlayer[0] = (byte)(joystick.getJoystick()&rhJoystickMask);
            }
            if (joystickAcc != null)
            {
                rhPlayer[0] |= joystickAcc.getJoystick();
            }
#endif

            // La souris, si un mouvement souris est defini
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            byte mKeys = 0;
            if (rhMouseUsed != 0)
            {
#if !WINDOWS_PHONE
                rh2MouseX = mouseState.X- rh4MouseXCenter;
                rh2MouseY = mouseState.Y - rh4MouseYCenter;
                if (rh2MouseX != 0 || rh2MouseY != 0)
                {
                    Mouse.SetPosition(rh4MouseXCenter, rh4MouseYCenter);
                }
#endif
                // Retrouve les touches de la souris 
                mKeys=0;
                if ((rh2MouseKeys&0x01)!=0)
                {
                    mKeys |= 0x10;				//00010000B;
                }

                if ((rh2MouseKeys&0x02)!=0)
                {
                    mKeys |= 0x20;				//00100000B;
                }

                // Force les touches souris comme joystick
                byte mouseUsed = rhMouseUsed;
                for (n = 0; n < rhNPlayers; n++)
                {
                    if ((mouseUsed & 1) != 0)
                    {
                        byte key = (byte) (rhPlayer[n] & 0xCF);		//11001111B;
                        key |= mKeys;
                        rhPlayer[n] = key;
                    }
                    mouseUsed >>= 1;
                }
            }

            // Appel des messages JOYSTICK PRESSED pour chaque joueur
            byte b;
            for (n = 0; n < 4; n++)
            {
                b = (byte) (rhPlayer[n] & plMasks[rhNPlayers*4+n]);
                b &= rh2InputMask[n];
                rhPlayer[n] = b;
                b ^= rh2OldPlayer[n];
                rh2NewPlayer[n] = b;
                if (b != 0)
                {
                    if (bMouseControlled == false)
                    {
                        if (n == 0)
                        {
                            newKey();
                        }
                    }
                    b &= rhPlayer[n];
                    if ((b & 0xF0) != 0)
                    {
                        // Message bloquant pour les touches FEU seules
                        rhEvtProg.rhCurOi = (short) n;
//                        b = rh2NewPlayer[n];
                        if ((b & 0xF0) != 0)
                        {
                            rhEvtProg.rhCurParam0 = b;
                            rhEvtProg.handle_GlobalEvents( unchecked((int)((-4 << 16) | 0xFFF9)));	// CNDL_JOYPRESSED);
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

            // Appel de tous les evenements
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            rhEvtProg.compute_TimerEvents();			// Evenements timer normaux

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

            // Modif les objets bouges par les evenements
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            modif_ChangedObjects();

            // Detruit les objets marques
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~
            destroy_List();

            // RAZ du click
            // ~~~~~~~~~~~~
            rhEvtProg.rh2CurrentClick = -1;
            rhEvtProg.rh3CurrentMenu = 0;
            rh4EventCount++;
            rh4FakeKey = 0;
    //	rh4DroppedFlag=0;

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
                    rhQuit == LOOPEXIT_QUIT ||
                    rhQuit == LOOPEXIT_NEWGAME)
            {
                rhEvtProg.handle_GlobalEvents((-2 << 16) | 0xFFFD);
            }
            return rhQuit;
        }

        // Bouge les objets changes par les evenements
        // -------------------------------------------
        public void modif_ChangedObjects()
        {
            int count = 0;
            for (int no = 0; no < rhNObjects; no++)		// Des objets à voir?
            {
                while (rhObjectList[count] == null)
                {
                    count++;
                }
                CObject pHo = rhObjectList[count];
                count++;

                if ((pHo.hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0)
                {
                    if (pHo.roc.rcChanged)
                    {
                        pHo.modif();
                        pHo.roc.rcChanged = false;
                    }
                }
            }
        }

        public void draw()
        {
            if ((rhGameFlags & GAMEFLAGS_FIRSTLOOPFADEIN) == 0)
            {
                if (rhApp.parentApp == null)
                {
                    screen_Update();
                }
            }
        }

        //---------------------------------------------------------//
        //	Lecture lecture et clavier pour les 4 joueurs          //
        //---------------------------------------------------------//
        void joyTest()
        {
            int i;

            // Raz des entrees des joueurs
            GamePadState[] states = new GamePadState[4];
            for (i = 0; i < 4; i++)
            {
                rhPlayer[i] = 0;
                switch (i)
                {
                    case 0:
                        states[i] = GamePad.GetState(PlayerIndex.One);
                        break;
                    case 1:
                        states[i] = GamePad.GetState(PlayerIndex.Two);
                        break;
                    case 2:
                        states[i] = GamePad.GetState(PlayerIndex.Three);
                        break;
                    case 3:
                        states[i] = GamePad.GetState(PlayerIndex.Four);
                        break;
                }
           }

            // Pour chaque joueur, lire clavier ou joystick
            short[] ctrlType = rhApp.getCtrlType();
            Keys[] ctrlKeys = rhApp.getCtrlKeys();

            for (i = 0; i < CRunApp.MAX_PLAYER; i++)
            {
                short player=ctrlType[i];
                if (player!=5)
                {
                    int n;
                    for (n = 0; n < 4; n++)
                    {
                        if ((player & (1 << n)) != 0)
                        {
                            if (states[n].DPad.Left == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x04;
                            }
                            if (states[n].DPad.Right == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x08;
                            }
                            if (states[n].DPad.Up == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x01;
                            }
                            if (states[n].DPad.Down == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x02;
                            }
                            if (states[n].ThumbSticks.Left.X<-0.5)
                            {
                                rhPlayer[i] |= 0x04;
                            }
                            if (states[n].ThumbSticks.Left.X > 0.5)
                            {
                                rhPlayer[i] |= 0x08;
                            }
                            if (states[n].ThumbSticks.Left.Y > 0.5)
                            {
                                rhPlayer[i] |= 0x01;
                            }
                            if (states[n].ThumbSticks.Left.Y < -0.5)
                            {
                                rhPlayer[i] |= 0x02;
                            }
                            if (states[n].Buttons.A == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x10;
                            }
                            if (states[n].Buttons.B == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x20;
                            }
                            if (states[n].Buttons.X == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x40;
                            }
                            if (states[n].Buttons.Y == ButtonState.Pressed)
                            {
                                rhPlayer[i] |= 0x80;
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < CRunApp.MAX_KEY; k++)
                    {
                        if (isKeyDown(ctrlKeys[i*4+k]))
                        {
                            rhPlayer[i] |= (byte) (1 << k);
                        }
                    }
                }
            }
        }

        public bool isKeyDown(Keys key)
        {
            if (keyboardState.IsKeyDown(key))
            {
                return true;
            }
            if (key == Keys.LeftShift)
            {
                if (keyboardState.IsKeyDown(Keys.RightShift))
                {
                    return true;
                }
            }
            if (key == Keys.LeftControl)
            {
                if (keyboardState.IsKeyDown(Keys.RightControl))
                {
                    return true;
                }
            }
            return false;
        }

        // --------------------------------------------------------------------------
        // Demande les coordonnees de la souris au systeme, retabli dans le playfield	  
        // --------------------------------------------------------------------------
        void getMouseCoords()
        {
/*            if (rh4DemoMode == CDemoRecord.DEMOPLAY)
            {
                rh2MouseX = rh4Demo.getMouseX();
                rh2MouseY = rh4Demo.getMouseY();
            }
            else
*/
            {
                rh2MouseX = mouseX+rhWindowX;
                rh2MouseY = mouseY+rhWindowY;
                if (rhApp.parentApp != null)
                {
                    rh2MouseX -= rhApp.xOffset;
                    rh2MouseY -= rhApp.yOffset;
                }
            }
            rh2MouseKeys=0;
#if !WINDOWS_PHONE
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                rh2MouseKeys|=0x01;
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                rh2MouseKeys|=0x02;
            }
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                rh2MouseKeys|=0x04;
            }
#else
            if (mouseKey==0)
            {
                rh2MouseKeys|=0x01;
            }
#endif
        }

        // -----------------------------------------------------------------------
        // DETECTION DE COLLISIONS
        // -----------------------------------------------------------------------
        public bool newHandle_Collisions(CObject pHo)
        {
            // Raz des flags pour le mouvement
            pHo.rom.rmMoveFlag = false;
            pHo.rom.rmEventFlags = 0;
            bMoveChanged = false;

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
                            rhEvtProg.handle_Event(pHo, (-11 << 16) | (((int) pHo.hoType) & 0xFFFF));  // CNDL_EXTINPLAYFIELD
                        }
                    }
                }

                // Gestion des flags WRAP
                // ~~~~~~~~~~~~~~~~~~~~~~
                int cadran = quadran_In(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot,
                        pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);
                if ((cadran & pHo.rom.rmWrapping) != 0)
                {
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
                        rhEvtProg.handle_Event(pHo, (-12 << 16) | (((int) pHo.hoType) & 0xFFFF));  // CNDL_EXTOUTPLAYFIELD
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
                CArrayList cnt = objectAllCol_IXY(pHo, pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY, pHo.hoX, pHo.hoY, pHo.hoOiList.oilColList);
                if (cnt != null)
                {
                    int obj;
                    for (obj = 0; obj < cnt.size(); obj++)
                    {
                        CObject pHox = (CObject)cnt.get(obj);
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
                            rhEvtProg.handle_Event(pHo_esi, (-14 << 16) | (((int) type) & 0xFFFF));	// CNDL_EXTCOLLISION
                        }
                    }
                }
            }
            return bMoveChanged;
        }

        // ----------------------------------------------
        // Teste les collisions de tous les objets
        // ----------------------------------------------
        // Renvoie le nombre de collisions
        public CArrayList objectAllCol_IXY(CObject pHo, short newImg, int newAngle, float newScaleX, float newScaleY, int newX, int newY, short[] pOiColList)
	    {
			CArrayList list=null;
			
			int rectX1= newX - pHo.hoImgXSpot;
			int rectX2= rectX1 + pHo.hoImgWidth;
			int rectY1= newY - pHo.hoImgYSpot;
			int rectY2= rectY1 + pHo.hoImgHeight;

			CMask pMask2;
			CImage image2;
			if ((pHo.hoFlags&CObject.HOF_NOCOLLISION)!=0)
			{
				return list;
			}

			bool bMask1=false;
			CMask pMask1=null;
			CImage image;
			int nLayer=-1;
            CSprite sprite1=null;
			if (pHo.hoType==COI.OBJ_SPR)
			{
                sprite1=pHo.roc.rcSprite;
                if (sprite1!=null)
                {
                    if ((sprite1.sprFlags&CSprite.SF_COLBOX)==0)
                    {
				        bMask1=true;
                    }
                }
                nLayer = pHo.ros.rsLayer;
            }
			
			short oldHoFlags = pHo.hoFlags;
			pHo.hoFlags |= CObject.HOF_NOCOLLISION;
			int count=0;
			int i;
			CObject pHox;
			int xHox, yHox;
            CSprite sprite2;
			if (pOiColList!=null)
			{
				int nOi=0;
				for (nOi=0; nOi<pOiColList.Length; nOi+=2)
				{
					CObjInfo pOil=rhOiList[pOiColList[nOi+1]];
					int nObject=pOil.oilObject;
					while(nObject>=0)
					{
						pHox=rhObjectList[nObject];
						nObject=pHox.hoNumNext;
						
						if ((pHox.hoFlags&CObject.HOF_NOCOLLISION)==0)
						{					
							xHox=pHox.hoX-pHox.hoImgXSpot;
							yHox=pHox.hoY-pHox.hoImgYSpot;
							if ( xHox < rectX2 && 
								xHox + pHox.hoImgWidth > rectX1 && 
								yHox < rectY2 && 
								yHox + pHox.hoImgHeight > rectY1 )
							{
								switch(pHox.hoType)
								{
									case COI.OBJ_SPR:
										if (nLayer<0 || (nLayer>=0 && nLayer==pHox.ros.rsLayer))
										{
                                            sprite2=pHox.roc.rcSprite;
                                            if (sprite2!=null)
                                            {
											    if ((sprite2.sprFlags&CSprite.SF_RAMBO)!=0)
											    {
												    if (bMask1==false || (sprite2.sprFlags&CSprite.SF_COLBOX)!=0)
												    {
													    if (list==null)
													    {
														    list=new CArrayList();
													    }
													    list.add(pHox);
													    break;
												    }
												    if (pMask1==null)
												    {
													    image=rhApp.imageBank.getImageFromHandle(newImg);
													    if (image!=null)
													    {
														    pMask1=image.getMask(0, newAngle, newScaleX, newScaleY);
													    }
												    }
                                                    pMask2 = null;
												    image2=rhApp.imageBank.getImageFromHandle(pHox.roc.rcImage);
												    if (image2!=null)
												    {
													    pMask2=image2.getMask(0, pHox.roc.rcAngle, pHox.roc.rcScaleX, pHox.roc.rcScaleY);
												    }
												    if (pMask1!=null && pMask2!=null)
												    {									
													    if (pMask1.testMask(0, rectX1, rectY1, pMask2, 0, xHox, yHox))
													    {
														    if (list==null)
														    {
															    list=new CArrayList();
														    }
														    list.add(pHox);
														    break;
													    }
												    }									
											    }
                                            }
										}
										break;
									case COI.OBJ_TEXT:
									case COI.OBJ_COUNTER:
									case COI.OBJ_LIVES:
									case COI.OBJ_SCORE:
									case COI.OBJ_CCA:
										if (list==null)
										{
											list=new CArrayList();
										}
										list.add(pHox);
										break;
									default:
										if (list==null)
										{
											list=new CArrayList();
										}
										list.add(pHox);
										break;
								}
							}
						}									
					}
				}
			}
			else
			{
				for (i=0; i<rhNObjects; i++)
				{
				    while(rhObjectList[count]==null)
						count++;
				    pHox=rhObjectList[count];
				    count++;
	
					if ((pHox.hoFlags&CObject.HOF_NOCOLLISION)==0)
					{					
						xHox=pHox.hoX-pHox.hoImgXSpot;
						yHox=pHox.hoY-pHox.hoImgYSpot;
						if ( xHox < rectX2 && 
						     xHox + pHox.hoImgWidth > rectX1 && 
						     yHox < rectY2 && 
						     yHox + pHox.hoImgHeight > rectY1 )
						{
							switch(pHox.hoType)
							{
								case COI.OBJ_SPR:
									if (nLayer<0 || (nLayer>=0 && nLayer==pHox.ros.rsLayer))
									{
                                        sprite2=pHox.roc.rcSprite;
                                        if (sprite2!=null)
                                        {
										    if ((sprite2.sprFlags&CSprite.SF_RAMBO)!=0)
										    {
											    if (bMask1==false || (sprite2.sprFlags&CSprite.SF_COLBOX)!=0)
											    {
											        if (list==null)
											        {
													    list=new CArrayList();
											        }
											        list.add(pHox);
											        break;
											    }
											    if (pMask1==null)
											    {
												    image=rhApp.imageBank.getImageFromHandle(newImg);
												    if (image!=null)
												    {
													    pMask1=image.getMask(0, newAngle, newScaleX, newScaleY);
												    }
											    }
											    image2=rhApp.imageBank.getImageFromHandle(pHox.roc.rcImage);
                                                pMask2 = null;
                                                if (image2 != null)
                                                {
                                                    pMask2 = image2.getMask(0, pHox.roc.rcAngle, pHox.roc.rcScaleX, pHox.roc.rcScaleY);
                                                }
											    if (pMask1!=null && pMask2!=null)
											    {									
												    if (pMask1.testMask(0, rectX1, rectY1, pMask2, 0, xHox, yHox))
												    {
												        if (list==null)
												        {
														    list=new CArrayList();
												        }
												        list.add(pHox);
												        break;
												    }
											    }									
										    }
                                        }
									}
									break;
								case COI.OBJ_TEXT:
								case COI.OBJ_COUNTER:
								case COI.OBJ_LIVES:
								case COI.OBJ_SCORE:
								case COI.OBJ_CCA:
								    if (list==null)
								    {
										list=new CArrayList();
								    }
								    list.add(pHox);
									break;
								default:
								    if (list==null)
								    {
										list=new CArrayList();
								    }
								    list.add(pHox);
									break;
							}
						}
					}									
			    }
			}
			// Remettre anciens flags
			pHo.hoFlags = oldHoFlags;    
			return list;
	    }


/*
        public CArrayList objectAllCol_IXY(CObject pHo, short newImg, int newAngle, float newScaleX, float newScaleY, int newX, int newY, short[] pOiColList)
        {
            CArrayList list = null;

            // Les collisions d'un objet sprite
            // --------------------------------
            CObject pHox;
            int count, i;
            int rectX1, rectX2, rectY1, rectY2;
            if ((pHo.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW)) != 0)
            {
                // Les collisions avec les autres sprites
                // --------------------------------------
                if (pHo.roc.rcSprite != null)
                {
                    // List sprite objects to test
                    bool bSpriteList = false;
                    CSprite[] spriteList = null;
                    int spriteListCount = 0;
                    if (pOiColList != null)
                    {
                        count = 1;
                        while (count < pOiColList.Length)
                        {
                            CObjInfo poil = rhOiList[pOiColList[count]];
                            if (poil.oilNObjects != 0)
                            {
                                short nObl = poil.oilObject;
                                do
                                {
                                    CObject pHo2 = rhObjectList[nObl];

                                    // Sprite?
                                    if ((pHo2.hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0 &&
                                         (pHo2.hoFlags & CObject.HOF_DESTROYED) == 0 && pHo2.hoLayer == pHo.hoLayer)
                                    {
                                        if (pHo2.roc.rcSprite != null)
                                        {
                                            if (spriteList == null)
                                            {
                                                spriteList = new CSprite[10];
                                            }
                                            else if (spriteListCount > spriteList.Length)
                                            {
                                                Array.Resize(ref spriteList, spriteList.Length + 10);
                                            }
                                            spriteList[spriteListCount++] = pHo2.roc.rcSprite;
                                            bSpriteList = true;
                                        }
                                    }
                                    nObl = pHo2.hoNumNext;
                                } while (nObl >= 0);
                            }
                            count += 2;
                        }
                    }
                    if (!bSpriteList || spriteList != null)
                    {
                        list = rhApp.spriteGen.spriteCol_TestSprite_All(pHo.roc.rcSprite, newImg, newX - rhWindowX, newY - rhWindowY, newAngle, newScaleX, newScaleY, 0, spriteList);
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

                    if (pOiColList != null)
                    {
                        count = 1;
                        while (count < pOiColList.Length)
                        {
                            CObjInfo poil = rhOiList[pOiColList[count]];
                            if (poil.oilNObjects != 0)
                            {
                                short nObl = poil.oilObject;
                                do
                                {
                                    pHox = rhObjectList[nObl];
                                    if ((pHox.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW | CObject.HOF_NOCOLLISION)) == 0)
                                    {
                                        if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
                                                pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
                                                pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
                                                pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
                                        {
                                            if (list == null)
                                            {
                                                list = new CArrayList();
                                            }
                                            list.add(pHox);
                                        }
                                    }
                                    nObl = pHox.hoNumNext;
                                } while (nObl >= 0);
                            }
                            count += 2;
                        }
                    }
                    else
                    {
                        count = 0;
                        for (i = 0; i < rhNObjects; i++)
                        {
                            while (rhObjectList[count] == null)
                            {
                                count++;
                            }
                            pHox = rhObjectList[count];
                            count++;

                            // YVES: ajout HOF_OWNERDRAW car les sprites ownerdraw sont gérés dans les collisions
                            if ((pHox.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW | CObject.HOF_NOCOLLISION)) == 0)
                            {
                                if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
                                        pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
                                        pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
                                        pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
                                {
                                    if (list == null)
                                    {
                                        list = new CArrayList();
                                    }
                                    list.add(pHox);
                                }
                            }
                        }
                    }
                    // Remettre anciens flags
                    pHo.hoFlags = oldHoFlags;
                }
            }
            // Les collisions d'un objet non sprite
            // ------------------------------------
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

                    if (pOiColList != null)
                    {
                        count = 1;
                        while (count < pOiColList.Length)
                        {
                            CObjInfo poil = rhOiList[pOiColList[count]];
                            if (poil.oilNObjects != 0)
                            {
                                short nObl = poil.oilObject;
                                do
                                {
                                    pHox = rhObjectList[nObl];
                                    if ((pHox.hoFlags & CObject.HOF_NOCOLLISION) == 0)
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
                                                    list = new CArrayList();
                                                }
                                                list.add(pHox);
                                            }
                                        }
                                    }
                                    nObl = pHox.hoNumNext;
                                } while (nObl >= 0);
                            }
                            count += 2;
                        }
                    }
                    else
                    {
                        count = 0;
                        for (i = 0; i < rhNObjects; i++)
                        {
                            while (rhObjectList[count] == null)
                            {
                                count++;
                            }
                            pHox = rhObjectList[count];
                            count++;

                            if ((pHox.hoFlags & CObject.HOF_NOCOLLISION) == 0)
                            {
                                // ??? Remplacement de QUICKEXT par QUICKCOL pour corriger pb collisions avec objet FLI
                                // ??? A verifier!!!!!
                                if ((pHox.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKCOL) != 0)
                                {
                                    if (pHox.hoX - pHox.hoImgXSpot <= rectX2 &&
                                            pHox.hoX - pHox.hoImgXSpot + pHox.hoImgWidth >= rectX1 &&
                                            pHox.hoY - pHox.hoImgYSpot <= rectY2 &&
                                            pHox.hoY - pHox.hoImgYSpot + pHox.hoImgHeight >= rectY1)
                                    {
                                        if (list == null)
                                        {
                                            list = new CArrayList();
                                        }
                                        list.add(pHox);
                                    }
                                }
                            }
                        }
                    }
                    // Remettre anciens flags
                    pHo.hoFlags = oldHoFlags;
                }
            }
            return list;
        }
*/
        // ----------------------------------------------
        // Teste les collisions d'un objet avec le decor
        // Si collision, retourne la condition COLBACK
        // ----------------------------------------------
        public int colMask_TestObject_IXY(CObject pHo, short newImg, int newAngle, float newScaleX, float newScaleY, int newX, int newY, int htfoot, int plan)
        {
            int res = 0;
            int x = newX - rhWindowX;
            int y = newY - rhWindowY;

            bool bSprite = false;

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
                        res = ((-13 << 16) | (((int) pHo.hoType) & 0xFFFF));	    // CNDL_EXTCOLBACK
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
                        res = ((-13 << 16) | (((int) pHo.hoType) & 0xFFFF));	    // CNDL_EXTCOLBACK
                    }
                }
                else
                {
                    if (rhFrame.bkdCol_TestRect(x, y, pHo.hoImgWidth, pHo.hoImgHeight, pHo.hoLayer, plan))
                    {
                        res = ((-13 << 16) | (((int) pHo.hoType) & 0xFFFF));	    // CNDL_EXTCOLBACK
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
        public short random(short wMax)
        {
            int calcul = (int) rh3Graine * 31415 + 1;
            rh3Graine = (short) calcul;
            calcul &= 0x0000FFFF;
            return (short) ((calcul * wMax) >> 16);
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
                dirShift=(dirShift>>1)&0x7FFFFFFF;
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
                dirShift=(dirShift>>1)&0x7FFFFFFF;
            }
            return 0;
        }

        // -----------------------------------------------------------------------
        // EVALUATION D'EXPRESSION
        // -----------------------------------------------------------------------
        public CValue get_EventExpressionAny(CParamExpression pExp)
        {
            rh4Tokens = pExp.tokens;
            rh4CurToken = 0;
            return new CValue(getExpression());
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

        public CValue get_ExpressionAny()
        {
            CValue ret=new CValue(getExpression());
            return ret;
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

                // Regarde l'opérateur
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

        public CValue getCurrentResult()
        {
            return rh4Results[rh4PosPile];
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
        public static bool compareTo(CValue pValue1, CValue pValue2, short comp)
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
        public static bool compareTer(int value1, int value2, short comparaison)
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
            int[] lives = rhApp.getLives();
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
        public bool getMouseOnObjectsEDX(short oiList, bool nega)
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
            int x = rh2MouseX - rhWindowX;
            int y = rh2MouseY - rhWindowY;
            CArrayList list = new CArrayList();
            CSprite curSpr = rhApp.spriteGen.spriteCol_TestPoint(null, CSpriteGen.LAYER_ALL, x, y, 0);
            CObject pHoFound;
            while (curSpr != null)
            {
                pHoFound = curSpr.sprExtraInfo;
                if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)		//; Detruit au cycle precedent?
                {
                    list.add(pHoFound);
                }
                curSpr = rhApp.spriteGen.spriteCol_TestPoint(curSpr, CSpriteGen.LAYER_ALL, x, y, 0);
            }

            // Demande les collisions des autres objets
            // ----------------------------------------
            int count = 0;
            int no;
            for (no = 0; no < rhNObjects; no++)		// Des objets à voir?
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
                    for (count = 0; count < list.size(); count++)
                    {
                        pHoFound = (CObject)list.get(count);
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
                    for (count = 0; count < list.size(); count++)
                    {
                        pHoFound = (CObject)list.get(count);
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
                for (int no = 0; no < rhNObjects; no++)
                {
                    while (rhObjectList[count] == null)
                    {
                        count++;
                    }
                    CObject pHo = rhObjectList[count];
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
            while (count < qoil.qoiList.Length)
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

        //; Trouve une direction à partir d'une pente AX/BX . AX
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
            bool flagX = false, flagY = false;		// Flags de signe
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
                dir = -dir + 32;						//; Rétablir en Y
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
            bool bFlag = false;
            int dw = 0;

            if ((hoPtr.hoFlags & CObject.HOF_FADEIN) == 0)				// Le sprite est-il encore en fade-in?
            {
                if (hoPtr.ros.initFadeOut())
                {
                    return;
                }
                if (hoPtr.roa != null)
                {
                    if (hoPtr.roa.anim_Exist(CAnim.ANIMID_DISAPPEAR))
                    {
                        dw = 1;								// Une animation presente?
                    }
                }
            }
            if (dw == 0)
            {
                bFlag = true;
            }

            // Rien du tout. on detruit le sprite!
            if (bFlag)
            {
                hoPtr.hoCallRoutine = false;
                destroy_Add(hoPtr.hoNumber);
                return;
            }

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
            if ((dw & 1) != 0)
            {
                hoPtr.roa.animation_Force(CAnim.ANIMID_DISAPPEAR);
                hoPtr.roa.animation_OneLoop();
            }
        }

        // -----------------------------------------------------------------------
        // SPRITES OWNER DRAW QUICK DISPLAY 
        // -----------------------------------------------------------------------

        public void add_QuickDisplay(CObject hoPtr)
        {
	        if (rh4FirstQuickDisplay<0)			
	        {
		        rh4FirstQuickDisplay=hoPtr.hoNumber;
		        hoPtr.hoPreviousQuickDisplay=-1;
	        }
	        else
	        {
		        if (rh4LastQuickDisplay>=0)
		        {
			        CObject hoLast=rhObjectList[rh4LastQuickDisplay];
			        hoLast.hoNextQuickDisplay=hoPtr.hoNumber;
			        hoPtr.hoPreviousQuickDisplay=hoLast.hoNumber;
		        }
	        }
	        rh4LastQuickDisplay=hoPtr.hoNumber;
	        hoPtr.hoNextQuickDisplay=-1;
        }

        public void draw_QuickDisplay(SpriteBatchEffect batch)
        {
	        int nObject = rh4FirstQuickDisplay;
	        while (nObject >= 0)
	        {
		        CObject hoPtr = rhObjectList[nObject];
        		
		        if ((hoPtr.ros.rsFlags & (CRSpr.RSFLAG_SLEEPING | CRSpr.RSFLAG_HIDDEN)) == 0)	// Afficher le sprite?
		        {
			        hoPtr.draw(batch);
		        }
		        nObject = hoPtr.hoNextQuickDisplay;
	        }
        }

        public void remove_QuickDisplay(CObject hoPtr)
        {
	        CObject hoPrevious;
	        CObject hoNext;
        	
	        short next=hoPtr.hoNextQuickDisplay;
	        short prev=hoPtr.hoPreviousQuickDisplay;

	        if (prev>=0)
	        {
		        hoPrevious=rhObjectList[prev];
		        hoPrevious.hoNextQuickDisplay=next;
	        }
	        else 
	        {
		        rh4FirstQuickDisplay=next;
	        }
	        if (next>=0)
	        {
		        hoNext=rhObjectList[next];
		        hoNext.hoPreviousQuickDisplay=prev;
	        }
	        else
	        {
		        rh4LastQuickDisplay=prev;		
	        }
        }

        // -------------------------------------------------------------------------
        // Conditions / Actions en +
        // -------------------------------------------------------------------------

        public bool isMouseOn()
        {
            return rh4CursorShown;
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
            if (value >= 1 && value <= 1000)
            {
                // Get top-level application
                CRunApp app = rhApp;
                while (app.parentApp != null)
                {
                    app = app.parentApp;
                }

                // Set new frame rate
                app.gaFrameRate = value;
// FRANCOIS
            }
        }

        public int getXMouse()
        {
            if (rhMouseUsed != 0)
            {
                return 0;
            }
            return rh2MouseX;
        }

        public int getYMouse()
        {
            if (rhMouseUsed != 0)
            {
                return 0;
            }
            return rh2MouseY;
        }

        public int getRGBAt(CObject hoPtr, int x, int y)
        {
/* FRANCOIS
            int rgb = 0;
            if (hoPtr.roc.rcImage != -1)
            {
                CImage image = rhApp.imageBank.getImageFromHandle(hoPtr.roc.rcImage);
                int pixels[] = new int[image.width * image.height];
                PixelGrabber pg = new PixelGrabber(image.img, 0, 0, image.width, image.height, pixels, 0, image.width);
                try
                {
                    pg.grabPixels();
                }
                catch (InterruptedException e)
                {
                }

                rgb = pixels[y * image.width + x] & 0xFFFFFF;
                rgb = CServices.swapRGB(rgb);
            }
            return rgb;
 */ 
            return 0;
        }

        // -------------------------------------------------------------------------
        // STOCKAGE GLOBAL POUR LES EXTENSIONS
        // -------------------------------------------------------------------------    
        public CExtStorage getStorage(int id)
        {
            if (rhApp.extensionStorage != null)
            {
                int n;
                for (n = 0; n < rhApp.extensionStorage.size(); n++)
                {
                    CExtStorage e = (CExtStorage)rhApp.extensionStorage.get(n);
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
                for (n = 0; n < rhApp.extensionStorage.size(); n++)
                {
                    CExtStorage e = (CExtStorage)rhApp.extensionStorage.get(n);
                    if (e.id == id)
                    {
                        rhApp.extensionStorage.remove(n);
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
                    rhApp.extensionStorage = new CArrayList();
                }
                data.id = id;
                rhApp.extensionStorage.add(data);
            }
        }

        // Gestion des touches
        // ------------------------------------------------------------------------------
#if WINDOWS_PHONE
        public void getTouches()
        {
            mouseKey = -1;
            rh2MouseKeys = 0;
            if (rhApp.numberOfTouches > 0)
            {
                TouchCollection touchCollection = TouchPanel.GetState();
                foreach (TouchLocation tl in touchCollection)
                {
                    if (toucheID<0 && tl.State == TouchLocationState.Pressed)
                    {
                        toucheID = tl.Id;
                    }

                    int n;
                    bool bFlag=false;
                    bool bFlagLocal = false;
                    switch (tl.State)
                    {
                        case TouchLocationState.Pressed:
                            if (joystick != null)
                            {
                                bFlagLocal = joystick.touchBegan(tl);
                                if (bFlagLocal)
                                {
                                    bFlag = true;
                                }
                            }
                            if (touches != null)
                            {
                                if (bFlagLocal == false)
                                {
                                    touches.touchBegan(tl);
                                }
                            }
                            if (bFlagLocal)
                            {
                                for (n = 0; n < rhApp.numberOfTouches; n++)
                                {
                                    if (cancelledTouches[n] < 0)
                                    {
                                        cancelledTouches[n] = tl.Id;
                                        break;
                                    }
                                }
                            }
                            break;
                        case TouchLocationState.Moved:
                            if (joystick != null)
                            {
                                joystick.touchMoved(tl);
                            }
                            for (n = 0; n < rhApp.numberOfTouches; n++)
                            {
                                if (cancelledTouches[n] == tl.Id)
                                {
                                    bFlagLocal = true;
                                    bFlag = true;
                                    break;
                                }
                            }
                            if (touches != null)
                            {
                                if (bFlagLocal == false)
                                {
                                    touches.touchMoved(tl);
                                }
                            }
                            break;
                        case TouchLocationState.Released:
                            if (joystick != null)
                            {
                                joystick.touchEnded(tl);
                            }
                            for (n = 0; n < rhApp.numberOfTouches; n++)
                            {
                                if (cancelledTouches[n] == tl.Id)
                                {
                                    cancelledTouches[n] = -1;
                                    bFlagLocal = true;
                                    bFlag = true;
                                    break;
                                }
                            }
                            if (touches != null)
                            {
                                if (bFlagLocal == false)
                                {
                                    touches.touchEnded(tl);
                                }
                            }
                            break;
                        case TouchLocationState.Invalid:
                            if (joystick != null)
                            {
                                joystick.touchCancelled(tl);
                            }
                            for (n = 0; n < rhApp.numberOfTouches; n++)
                            {
                                if (cancelledTouches[n] == tl.Id)
                                {
                                    cancelledTouches[n] = -1;
                                    break;
                                }
                            }
                            if (touches != null)
                            {
                                touches.touchCancelled(tl);
                            }
                            break;
                    }

                    if (tl.Id==toucheID)
                    {
                        switch (tl.State)
                        {
                            case TouchLocationState.Pressed:
                            case TouchLocationState.Moved:
                                mouseX = (int)tl.Position.X;
                                mouseY = (int)tl.Position.Y;
                                if (bFlag==false)
                                {
                                    mouseKey = 0;
                                }
                                break;
                            case TouchLocationState.Released:
                                mouseX = (int)tl.Position.X;
                                mouseY = (int)tl.Position.Y;
                                toucheID = -1;
                                break;
                            case TouchLocationState.Invalid:
                                toucheID = -1;
                                break;
                        }
                    }
                }
            }
        }
        public void startJoystickAcc()
        {
            joystickAccCount++;
            if (joystickAccCount == 1)
            {
                rhApp.startAccelerometer();
                joystickAcc = new CJoystickAcc();
            }
        }
        public void stopJoystickAcc()
        {
            joystickAccCount--;
            if (joystickAccCount == 0)
            {
                rhApp.stopAccelerometer();
                joystickAcc = null;
            }
        }
#endif
        public void callEventExtension(CExtension hoPtr, int code, int param)
        {
            if (rh2PauseCompteur == 0)
            {
                int p0 = rhEvtProg.rhCurParam0;
                rhEvtProg.rhCurParam0 = param;
                code = (-(code + CEventProgram.EVENTS_EXTBASE + 1) << 16);
                code |= (((int)hoPtr.hoType) & 0xFFFF);
                rhEvtProg.handle_Event(hoPtr, code);
                rhEvtProg.rhCurParam0 = p0;
            }
        }

        // Gestion des controles
        // ------------------------------------------------------------------------------
        public void addControl(IControl c)
        {
            nControls++;
            if (controls==null)
            {
                controls=new CArrayList();
            }
            controls.add(c);
            c.setMouseControlled(bMouseControlled);
        }
        public void delControl(IControl c)
        {
            nControls--;
            controls.remove(c);
        }

        public void clickControls(int nClicks)
        {
            int n;
            for (n = 0; n < nControls; n++)
            {
                ((IControl)controls.get(n)).click(nClicks);
            }
        }
        public void newKey()
        {
            if (nControls > 0)
            {
                int x, y, n;
                int xBase, yBase;
                int xClosest, yClosest;
                IControl control, controlClosest, controlBase;
                if ((rh2NewPlayer[0] & 4) != 0 && (rhPlayer[0] & 4) != 0)
                {
                    // Left
                    if (currentControl == null)
                    {
                        controlBase = null;
                        xBase = 1000;
                        yBase = 1000;
                    }
                    else
                    {
                        controlBase=currentControl;
                        xBase = currentControl.getX();
                        yBase = currentControl.getY();
                        controlBase.setFocus(false);
                    }
                    xClosest = -1000;
                    yClosest = -1000;
                    controlClosest=null;
                    for (n = 0; n < nControls; n++)
                    {
                        control = (IControl)controls.get(n);
                        if (control != controlBase)
                        {
                            x=control.getX();
                            y=control.getY();
                            if ((y < yBase) || (y == yBase && x < xBase))
                            {
                                if ((y>yClosest) || (y==yClosest && x>xClosest))
                                {
                                    xClosest=x;
                                    yClosest=y;
                                    controlClosest=control;
                                }                                
                            }
                        }
                    }
                    currentControl = controlClosest;
                }
                if ((rh2NewPlayer[0] & 8) != 0 && (rhPlayer[0] & 8) != 0)
                {
                    // Right
                    if (currentControl == null)
                    {
                        controlBase = null;
                        xBase = -1000;
                        yBase = -1000;
                    }
                    else
                    {
                        controlBase=currentControl;
                        xBase = currentControl.getX();
                        yBase = currentControl.getY();
                        controlBase.setFocus(false);
                    }
                    xClosest = 1000;
                    yClosest = 1000;
                    controlClosest=null;
                    for (n = 0; n < nControls; n++)
                    {
                        control = (IControl)controls.get(n);
                        if (control != controlBase)
                        {
                            x=control.getX();
                            y=control.getY();
                            if ((y > yBase) || (y == yBase && x > xBase))
                            {
                                if ((y<yClosest) || (y==yClosest && x<xClosest))
                                {
                                    xClosest=x;
                                    yClosest=y;
                                    controlClosest=control;
                                }                                
                            }
                        }
                    }
                    currentControl = controlClosest;
                }
                if (currentControl != null)
                {
                    currentControl.setFocus(true);
                }
            }
        }
    }
}
