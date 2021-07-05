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
// CRUNAPP : Classe Application
//
//----------------------------------------------------------------------------------
package Application;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Set;

import Banks.CFontBank;
import Banks.CImageBank;
import Banks.CMusicBank;
import Banks.CSoundBank;
import Banks.CImageShape;
import Expressions.CValue;
import Extensions.CExtLoader;
import Extensions.CExtStorage;
import Extensions.CRunAndroid;
import Extensions.CRunFIRETV;
import Extensions.CRunNEXUSTV;
import Extensions.CRunOUYA;
import OI.COI;
import OI.COIList;
import Objects.CCCA;
import RunLoop.CRun;
import RunLoop.CSaveGlobal;
import Runtime.CTouchManagerAPI5;
import Runtime.ControlView;
import Runtime.ITouchAware;
import Runtime.Log;
import Runtime.MMFRuntime;
import Runtime.MainView;
import Runtime.SurfaceView;
import Services.CChunk;
import Services.CFile;
import Services.CServices;
import Sprites.CColMask;
import Sprites.CMask;
import Transitions.CTransitionManager;
import android.content.ClipData;
import android.content.ClipDescription;
import android.content.Context;
import android.content.res.AssetManager;
import android.content.ClipboardManager;
import android.net.Uri;
import android.os.AsyncTask;
import android.view.KeyEvent;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup.LayoutParams;

public class CRunApp implements ITouchAware
{
	public static final short RUNTIME_VERSION = 0x0302;
	public static final int RUNTIME_CM = 1;
	public static final short MAX_PLAYER = 4;
	public static final short MAX_KEY = 8;
	//    public static final short GA_BORDERMAX=0x0001;
	public static final short GA_NOHEADING = 0x0002;
	//    public static final short GA_PANIC=0x0004;
	public static final short GA_SPEEDINDEPENDANT = 0x0008;
	//public static final short GA_STRETCH = 0x0010;
	public static final short GA_MENUHIDDEN = 0x0080;
	public static final short GA_MENUBAR = 0x0100;
	public static final short GA_MAXIMISE = 0x0200;
	public static final short GA_MIX = 0x0400;
	public static final short GA_FULLSCREENATSTART = 0x0800;
	//    public static final short GA_FULLSCREENSWITCH=0x1000;
	//    public static final short GA_PROTECTED=0x2000;
	//    public static final short GA_COPYRIGHT=0x4000;
	//    public static final short GA_ONEFILE=(short)0x8000;
	public static final short GANF_SAMPLESOVERFRAMES = 0x0001;
	//    public static final short GANF_RELOCFILES=0x0002;
	public static final short GANF_RUNFRAME = 0x0004;
	public static final short GANF_SAMPLESEVENIFNOTFOCUS=0x0008;
	//    public static final short GANF_NOMINIMIZEBOX=0x0010;
	//    public static final short GANF_NOMAXIMIZEBOX=0x0020;
	public static final short GANF_NOTHICKFRAME = 0x0040;
	public static final short GANF_DONOTCENTERFRAME = 0x0080;
	//    public static final short GANF_SCREENSAVER_NOAUTOSTOP=0x0100;
	public static final short GANF_DISABLE_CLOSE = 0x0200;
	public static final short GANF_HIDDENATSTART = 0x0400;
	//    public static final short GANF_XPVISUALTHEMESUPPORT=0x0800;
	//    public static final short GANF_VSYNC=0x1000;
	//    public static final short GANF_RUNWHENMINIMIZED=0x2000;
	public static final short GANF_MDI = 0x4000;
	//    public static final short GANF_RUNWHILERESIZING=(short)0x8000;
	//    public static final short GAOF_DEBUGGERSHORTCUTS=0x0001;
	//    public static final short GAOF_DDRAW=0x0002;
	//    public static final short GAOF_DDRAWVRAM=0x0004;
	//    public static final short GAOF_OBSOLETE=0x0008;
	//    public static final short GAOF_AUTOIMGFLT=0x0010;
	//    public static final short GAOF_AUTOSNDFLT=0x0020;
	//    public static final short GAOF_ALLINONE=0x0040;
	//    public static final short GAOF_SHOWDEBUGGER=0x0080;
	public static final short GAOF_JAVASWING = 0x1000;
	public static final short GAOF_JAVAAPPLET = 0x2000;
	public static final short SL_RESTART = 0;
	public static final short SL_STARTFRAME = 1;
	public static final short SL_FRAMEFADEINLOOP = 2;
	public static final short SL_FRAMELOOP = 3;
	public static final short SL_FRAMEFADEOUTLOOP = 4;
	public static final short SL_ENDFRAME = 5;
	public static final short SL_QUIT = 6;
	public static final short CTRLTYPE_MOUSE = 0;
	public static final short CTRLTYPE_JOY1 = 1;
	public static final short CTRLTYPE_JOY2 = 2;
	public static final short CTRLTYPE_JOY3 = 3;
	public static final short CTRLTYPE_JOY4 = 4;
	public static final short CTRLTYPE_KEYBOARD = 5;
	//    public static final short ARF_MENUINIT=0x0001;
	//    public static final short ARF_MENUIMAGESLOADED=0x0002;		// menu images have been loaded into memory
	public static final short ARF_INGAMELOOP = 0x0004;
	//    public static final short ARF_PAUSEDBEFOREMODALLOOP=0x0008;
	public static final int FILEINFO_DRIVE = 0;
	public static final int FILEINFO_DIR = 1;
	public static final int FILEINFO_TEMPPATH = 2;
	public static final int FILEINFO_PATH = 3;
	public static final int FILEINFO_APPNAME = 4;
	public static final int DISPLAY_WINDOW = 0;
	public static final int DISPLAY_SWING = 1;
	public static final int DISPLAY_FULLSCREEN = 2;
	public static final int DISPLAY_PANEL = 3;
	public static final int DISPLAY_APPLET = 4;

	public static final int AH2OPT_KEEPSCREENRATIO=0x0001;
	public static final int AH2OPT_GLOBALREFRESH=0x0008;		// (Mobile) force global refresh
	public static final int AH2OPT_STATUSLINE=0x0040;
	public static final int AH2OPT_AUTOEND=0x0400;
	public static final int AH2OPT_DISABLEBACKBUTTON=0x0800;
	public static final int AH2OPT_CRASHREPORTING=0x2000;
	public static final int AH2OPT_ENABLEAD = 0x0100;
	public static final int AH2OPT_REQUIREGPU = 0x4000;
	public static final int AH2OPT_ANTIALIASED = 0x1000;
	public static final int AH2OPT_OPENGL1 = 0x10000;
	public static final int AH2OPT_OPENGL30 = 0x20000;
	public static final int AH2OPT_OPENGL31 = 0x40000;
	public static final int AH2OPT_SYSTEMFONT = 0x80000;
	public static final int AH2OPT_KEYBOVERAPPWINDOW = 0x200000;
	public static final int AH2OPT_DESTROYIFNOINACTIVATE = 0x4000000;

	public static final int AH2OPT2_ANDROID_ENABLELOADONCALL = 0x0001;

	public static final int DOUBLECLICKTIME = 200;

	public Set <CRunAndroid> androidObjects = null;
	public Set <CRunOUYA> ouyaObjects = null;
	public Set <CRunFIRETV> firetvObjects = null;
	public Set <CRunNEXUSTV> nexustvObjects = null;

	public CCCA modalSubApp=null;
	
	public class MenuEntry
	{	
		public String title;
		public String id;

		public MenuItem item;

		public boolean disabled;
	};

	public MenuEntry[] androidMenu;

	public int counter=0;

	public long frameOffsets[];         /// Offsets to the frames in the application.ccj
	public int frameMaxIndex = 0;
	public short nGlobalValuesInit;
	public byte globalValuesInitTypes[];
	public int globalValuesInit[];
	public short nGlobalStringsInit;
	public String globalStringsInit[];
	public COIList OIList;              /// OIList
	public CImageBank imageBank;        /// The image bank
	public CFontBank fontBank;          /// The font bank
	public CSoundBank soundBank;        /// The sound bank
	public CSoundPlayer soundPlayer;    /// The sound player
	public CMusicBank musicBank;        /// The sound bank
	public CMusicPlayer musicPlayer;    /// The sound player
    public ArrayList<CImageShape> imageShapes;
	public int appRunningState;
	public int lives[];                 /// Values of lives
	public int scores[];                /// Values of scores
	public String playerNames[];        /// Names of players
	public ArrayList<CValue> gValues;           /// GLobal values
	public ArrayList<String> gStrings;          /// Global strings
	public CValue tempGValue;
	public int startFrame = 0;
	public int nextFrame;
	public int currentFrame;
	public CRunFrame frame = null;        /// The current frame
	public CFile file = null;
	public CRunApp parentApp = null;
	public int parentOptions;
	public long refTime;
	public CRun run = null;               /// The CRun object
	// Application header
	public short gaFlags;				/// Flags
	public short gaNewFlags;				/// New flags
	public short gaMode;				/// graphic mode
	public short gaOtherFlags;				/// Other Flags	
	public int gaCxWin;				/// Window x-size
	public int gaCyWin;				/// Window y-size
	public int gaScoreInit;				/// Initial score
	public int gaLivesInit;				/// Initial number of lives
	public int gaBorderColour;				/// Border colour
	public int gaAppBorderColour;
	public int gaNbFrames;				/// Number of frames
	public int gaFrameRate;				/// Number of frames per second
	public byte gaMDIWindowMenu;			/// Index of Window menu for MDI applications
	public short pcCtrlType[] = new short[4];		/// Control type per player (0=mouse,1=joy1, 2=joy2, 3=keyb)
	public int pcCtrlKeys[][] = new int[4][8];	/// Control keys per player
	public short frameHandleToIndex[];
	public short frameMaxHandle;
	public int cx;
	public int cy;
	public int mouseX;
	public int mouseY;
	public byte keyBuffer[] = new byte[CKeyConvert.MAX_VK];                          /// The key buffer
	public int keyGame;
	public short appRunFlags = 0;
	public ArrayList<CSaveGlobal> adGO = null;
	boolean quit = false;
	public CExtLoader extLoader = null;                   /// The extension loader object
	public boolean m_bLoading = false;
	public boolean bVisible = true;
	public int xOffset = 0;
	public int yOffset = 0;
	public int frameOffset = 0;
	public String pLoadFilename = null;
	public ArrayList<CExtStorage> extensionStorage = null;
	public CEmbeddedFile embeddedFiles[] = null;
	public CTransitionManager transitionManager = null;
	public int hdr2Options=0;
	public int hdr2Options2=0;
	public long mouseTime=0;
	public boolean bUnicode=false;
	public CCCA container = null;
	public String appName;
	public int codePage;
	long doubleClickTimer=0;

	public short viewMode;
	public static final short VIEWMODE_CENTER = 0;
	public static final short VIEWMODE_ADJUSTWINDOW = 1;
	public static final short VIEWMODE_FITINSIDE_BORDERS = 2;
	public static final short VIEWMODE_FITINSIDE_ADJUSTWINDOW = 3;
	public static final short VIEWMODE_FITOUTSIDE = 4;
	public static final short VIEWMODE_STRETCH = 5;

	public CMask firstMask;
	public CMask secondMask;

	public CJoystick joystick;
	public CJoystickAcc joystickAcc;
	
	public ControlView controlView;

	// JAMES: ScreenZoom
	public boolean bZoom;
	public float scScaleX;
	public float scScaleY;
	public float scScale;
	public float scAngle;
	public int scXSpot;
	public int scYSpot;
	public int scXDest;
	public int scYDest;

	public CRunApp()
	{
		androidObjects = new HashSet <CRunAndroid> ();
		ouyaObjects = new HashSet <CRunOUYA> ();
		firetvObjects = new HashSet <CRunFIRETV> ();
		nexustvObjects = new HashSet <CRunNEXUSTV> ();
		imageShapes = new ArrayList<CImageShape>();

		surfaceEnabled = true;
	}

	public void createControlView ()
	{
		controlView = new ControlView (this);

		MMFRuntime.inst.mainView.addView (controlView);
		MMFRuntime.inst.mainView.bringChildToFront (controlView);
	}

	public int nWindowUpdate = 0;

	public void updateWindowDimensions(int width, int height)
	{
		++ nWindowUpdate;

		Log.Log("Updating window dimensions to " + width + "x" + height
				+ " (scaled " + MMFRuntime.inst.scaleX + ", " + MMFRuntime.inst.scaleY + ")");

		int x = MMFRuntime.inst.viewportX;
		int y = MMFRuntime.inst.viewportY;

		for (CCCA c = container; c != null;
				c = c.hoAdRunHeader.rhApp.container)
		{
			x += (c.hoX - c.hoAdRunHeader.rhWindowX) * MMFRuntime.inst.scaleX;
			y += (c.hoY - c.hoAdRunHeader.rhWindowY) * MMFRuntime.inst.scaleY;
		}

		int controlViewWidth = (int) Math.round(gaCxWin * MMFRuntime.inst.scaleX);
		int controlViewHeight = (int) Math.round(gaCyWin * MMFRuntime.inst.scaleY);

		controlView.setLayoutParams (new MainView.LayoutParams (controlViewWidth, controlViewHeight, x, y));

		int widthMeasureSpec = View.MeasureSpec.makeMeasureSpec (controlViewWidth, View.MeasureSpec.EXACTLY),
				heightMeasureSpec = View.MeasureSpec.makeMeasureSpec (controlViewHeight, View.MeasureSpec.EXACTLY);

		controlView.measure (widthMeasureSpec, heightMeasureSpec);
		controlView.invalidate();

		controlView.getParent().requestLayout();
	}


	public void updateWindowPos ()
	{
		if (run != null)
		{
			absoluteX = - run.rhWindowX;
			absoluteY = - run.rhWindowY;
		}
		else
		{
			absoluteX = 0;
			absoluteY = 0;
		}

		if (container != null)
		{
			absoluteX += (parentApp.absoluteX + container.hoX) - parentApp.run.rhWindowX;
			absoluteY += (parentApp.absoluteY + container.hoY) - parentApp.run.rhWindowY;
		}
	}

	boolean surfaceEnabled;
	boolean init = true;

	public Thread thread;
	public boolean threadNeedsStarting;

	public void setSurfaceEnabled (boolean enabled)
	{
		if (container != null)
		{
			/* Only the top level CRunApp is in charge of the surface */
			return;
		}

		if (enabled)
		{
			if ((!init) && surfaceEnabled)
				return;

			init = false;

			surfaceEnabled = true;
			thread = null;

			SurfaceView surfaceView = new SurfaceView
					(MMFRuntime.inst.getApplicationContext());

			MMFRuntime.inst.mainView.addView (surfaceView, 0);

			surfaceView.setLayoutParams (new MainView.LayoutParams
					(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT,
							0, 0));

			surfaceView.setFocusable(true); // make sure we get key events
			surfaceView.setFocusableInTouchMode(true);

			surfaceView.requestFocus();
			
			surfaceView.setPreserveContextOnPause(true);
		
		}
		else
		{
			/*   if ((!init) && !surfaceEnabled)
                return;

            init = false;

            surfaceEnabled = false;

            int count = MMFRuntime.inst.mainView.getChildCount();

            for (int i = 0; i < count; ++ i)
            {
                View view = MMFRuntime.inst.mainView.getChildAt (i);

                if (view instanceof SurfaceView)
                {
                    MMFRuntime.inst.mainView.removeView (view);
                }
            }

            GLRenderer.inst = SurfaceView.ES2 ? new ES2Renderer() : new ES1Renderer();  */
		}

		if (init)
			init = false;

	}

	public void setParentApp(CRunApp pApp, int sFrame, int options, CCCA c)
	{
		parentApp = pApp;
		parentOptions = options;
		startFrame = sFrame;
		container = c;

	}


	/** Loads the main data of the application.
	 * Explores the Application.ccj different chunks, and sets the internal values accordingly.
	 * Initialise the image, sound and font banks.
	 */

	public boolean load(String ccnFilename)
	{
		Log.Log ("CRunApp/load");

		if(MMFRuntime.inst.touchManager == null)
			MMFRuntime.inst.touchManager = new CTouchManagerAPI5();

		try
		{
			if (ccnFilename == null)
			{
				String fileName = "raw/application";
				if(MMFRuntime.inst.obbAvailable)
					fileName = "res/raw/application";
				file = new CFile (MMFRuntime.inst.getResourceID (fileName), false);
				file.mmap ();
			}
			else
			{
				file = new CFile (ccnFilename);
				file.mmap ();
			}
		}
		catch (Throwable e)
		{
			Log.Log ("*** Application load failed: " + e);
			return false;
		}


		// Charge le mini-header
		byte name[] = new byte[4];
		file.read(name);		    // gaType
		boolean bOK=false;
		if (name[0]=='P' && name[1]=='A' && name[2]=='M' && name[3]=='E')
		{
			bOK=true;
			bUnicode=false;
		}
		if (name[0]=='P' && name[1]=='A' && name[2]=='M' && name[3]=='U')
		{
			bOK=true;
			bUnicode=true;
		}
		if (!bOK)
		{
			return false;
		}
		file.setUnicode(bUnicode);
		short s = file.readAShort();	    // gaVersion
		if (s != RUNTIME_VERSION)
		{
			return false;
		}
		s = file.readAShort();		    // gaSubversion
		file.skipBytes(4);		    // gaPrdVersion
		file.skipBytes(4);		    // gaPrdBuild

		// Reserve les objets
		OIList = new COIList();
		imageBank = new CImageBank(this);
		fontBank = new CFontBank();
		soundPlayer = new CSoundPlayer(this);
		soundBank = new CSoundBank(soundPlayer);
		musicPlayer = new CMusicPlayer(this);
		musicBank = new CMusicBank(musicPlayer);

		viewMode = 0;

		// Lis les chunksr
		CChunk chk = new CChunk();
		long posEnd;
		while (chk.chID != CChunk.CHUNK_LAST)
		{
			chk.readHeader(file);
			if (chk.chSize == 0)
			{
				continue;
			}
			posEnd = file.getFilePointer() + chk.chSize;
			switch (chk.chID)
			{
			// CHUNK_APPHEADER
			case 0x2223:
				Log.Log ("Got CHUNK_APPHEADER");

				loadAppHeader();
				frameOffsets = new long[gaNbFrames];
				break;
				// CHUNK_APPNAME
			case 0x2224:
				appName = file.readAString();
				break;
				// CHUNK_GLOBALVALUES
			case 0x2232:
				loadGlobalValues();
				break;
				// CHUNK_GLOBALSTRINGS
			case 0x2233:
				loadGlobalStrings();
				break;
				// CHUNK_APPHEADER2
			case 0x2245:
				loadAppHeader2();
				break;
				// CHUNK_APPCODEPAGE
			case 0x2246:
				codePage = file.readAInt();
				break;
				// CHUNK_FRAMEOFFSET
			case 0x2247:
				frameOffset = file.readAInt();
				break;
			case 0x2248:
				MMFRuntime.inst.adMobID = file.readAString ();
				MMFRuntime.inst.adMobTestDeviceID = file.readAString();
				break;

				// CHUNK_ANDROIDMENU
			case 0x224B:

				androidMenu = new MenuEntry[file.readAShort()];
				int androidMenu_length = androidMenu.length;
				for(int i = 0; i < androidMenu_length; ++ i)
				{
					MenuEntry entry = new MenuEntry();

					entry.id = file.readAString();
					entry.title = file.readAString();

					androidMenu[i] = entry;
				}

				break;

				// CHUNK_FRAMEITEMS
				// CHUNK_FRAMEITEMS_2
			case 0x2229:
			case 0x223F:
				OIList.preLoad(file);

				// Security if the CCN was built with an older version of Fusion: reset load on call flags for all objects
				if ( (hdr2Options2 & CRunApp.AH2OPT2_ANDROID_ENABLELOADONCALL) == 0 )
				{
					COI oi;
					for (oi = OIList.getFirstOI(); oi != null; oi = OIList.getNextOI())
						oi.oiFlags &= ~COI.OIF_LOADONCALL;
				}
				break;
				// CHUNK_FRAMEHANDLES
			case 0x222B:
				loadFrameHandles(chk.chSize);
				break;


				// CHUNK_FRAME
			case 0x3333:
				// Repere les positions des frames dans le fichier
				frameOffsets[frameMaxIndex] = file.getFilePointer();
				CChunk frChk = new CChunk();
				while (frChk.chID != 0x7F7F)		// CHUNK_LAST
				{
					frChk.readHeader(file);
					if (frChk.chSize == 0)
					{
						continue;
					}
					long frPosEnd = file.getFilePointer() + frChk.chSize;

					/*			switch (frChk.chID)
                        {
                        // CHUNK_FRAMEHEADER
                        case 0x3334:
                        break;
                        // CHUNK_FRAMEPASSWORD
                        case 0x3336:
                        break;
                        }
					 */ file.seek(frPosEnd);
				}
				frameMaxIndex++;
				break;
				// CHUNK_EXTENSIONS2
			case 0x2234:
				extLoader = new CExtLoader(this);
				extLoader.loadList(file);
				break;
				// CHUNK_BINARYFILES
			case 0x2238:
				int nFiles = file.readAInt();
				embeddedFiles = new CEmbeddedFile[nFiles];
				for (int n = 0; n < nFiles; n++)
				{
					embeddedFiles[n] = new CEmbeddedFile(this);
					embeddedFiles[n].preLoad();
				}
				break;
			case 0x4500:
				loadImageShapes(file);
				break;
				// CHUNK_IMAGE
			case 0x6666:
				imageBank.preLoad(file);
				break;
				// CHUNK_FONT
			case 0x6667:
				fontBank.preLoad(file);
				break;
				// CHUNK_SOUNDS
			case 0x6668:
				soundBank.preLoad(file);
				break;
				// CHUNK_MUSICS
			case 0x6669:
				musicBank.preLoad(file);
				break;
			}

			// Positionne a la fin du chunk
			file.seek(posEnd);
		}

		// Fixe le flags multiple samples
		soundPlayer.setMultipleSounds((gaFlags & GA_MIX) != 0);

		// Pas d'erreur
		return true;
	}

	// Lancement de l'application
	public boolean startApplication()
	{
		// Fixer le path de l'appli
		appRunningState = 0;	    // SL_RESTART
		currentFrame = -2;

		Log.Log ("CRunApp/addTouchAware");
		MMFRuntime.inst.touchManager.addTouchAware(this);
		return true;
	}

	public void centerDisplay()
	{
		// Calcul du centrage de l'image dans le canvas
		if ((gaNewFlags & CRunApp.GANF_DONOTCENTERFRAME) != 0)
		{
			xOffset = 0;
			yOffset = 0;
		}
		else
		{
			xOffset = gaCxWin / 2 - frame.leEditWinWidth / 2;
			yOffset = gaCyWin / 2 - frame.leEditWinHeight / 2;
		}
	}

	// Fait fonctionner l'application
	public boolean playApplication (boolean bOnlyRestartApp)
	{
		if (parentApp == null)
		{
			for(;;)
			{
				Runnable r;

				synchronized (MMFRuntime.inst.toRun)
				{
					r = MMFRuntime.inst.toRun.poll();
				}

				if (r == null)
					break;

				r.run();
			}
		}

		if(run != null)
		{
			run.rh3WindowSx = gaCxWin;
			run.rh3WindowSy = gaCyWin;

			if(run.rhFrame != null)
			{
				run.rhFrame.leEditWinWidth = Math.min(gaCxWin, run.rhFrame.leWidth);
				run.rhFrame.leEditWinHeight = Math.min(gaCyWin, run.rhFrame.leHeight);
			}
		}

		int error = 0;
		boolean bLoop = true;
		boolean bContinue = true;


		do
		{
			switch (appRunningState)
			{
			// SL_RESTART
			case 0:
				initGlobal();
				nextFrame = startFrame;
				appRunningState = 1;
				killGlobalData();
				// reInitMenu();
				// Build 248 : only restart application?
						if (bOnlyRestartApp)
						{
							// Used in Sub-Applications, initializes application global data and exit
							// (= don't execute the first frame loop now, it will be executed at the end
							// of the first loop of the parent frame as usual)
							bLoop = false;
							break;
						}
						break;
						// SL_STARTFRAME
			case 1:
				error = startTheFrame();

				Log.Log ("startTheFrame() called");
				bLoop = false;
				


				break;
				// SL_FRAMEFADEINLOOP
			case 2:
				if (loopFrameFadeIn() == false)
				{
					endFrameFadeIn();
					if (appRunningState == SL_QUIT || appRunningState == SL_RESTART)
					{
						endFrame();
					}
				}
				else
				{
					bLoop = false;
				}
				break;
				// SL_FRAMELOOP
			case 3:
				if (loopFrame() == false)
				{
					if (startFrameFadeOut())
					{
						appRunningState = SL_FRAMEFADEOUTLOOP;
					}
					else
					{
						endFrame();
					}
				}
				else
				{
					bLoop = false;
				}

				break;
				// SL_FRAMEFADEOUTLOOP
			case 4:
				if (loopFrameFadeOut() == false)
				{
					endFrameFadeOut();
					if (appRunningState == SL_QUIT || appRunningState == SL_RESTART)
					{
						endFrame();
					}
				}
				else
				{
					bLoop = false;
				}
				break;
				// SL_ENDFRAME
			case 5:
				endFrame();
				break;
			default:
				bLoop = false;
				break;
			}
		} while (bLoop == true && error == 0 && quit == false);

		// Error?
		if (error != 0)
		{
			appRunningState = SL_QUIT;
		}

		// Quit ?
		if (appRunningState == SL_QUIT)
		{
			bContinue = false;
		}

		if (bContinue && threadNeedsStarting)
		{
			threadNeedsStarting = false;
			thread.start();
		}

		// Continue?
				return bContinue;
	}

	// End application
	public void endApplication()
	{
		Log.Log ("CRunApp/removeTouchAware");
		MMFRuntime.inst.touchManager.removeTouchAware(this);

		// Stop sounds
		soundPlayer.stopAllSounds();
		musicPlayer.stopAllMusics();
		if (file != null)
		{
			file.close();
		}
	}

	public void changeWindowDimensions(int width, int height)
	{
		if (width>=0)
			gaCxWin=width;
		if (height>0)
			gaCyWin=height;
		if (frame!=null)
		{
			if (width>0)
			{
				frame.leEditWinWidth = width;
				frame.leWidth=width;
				frame.leVirtualRect.right = width;
			}
			if (height>0)
			{
				frame.leEditWinHeight = height;
				frame.leHeight=height;
				frame.leVirtualRect.bottom = height;
			}

			if ( run != null )
				run.updateFrameDimensions(width, height);
		}
	}

	public void initScreenZoom()
	{
		this.bZoom = false;
		this.scAngle = 0;
		this.scScale = this.scScaleX = this.scScaleY = 1;
		this.scXSpot = this.scXDest = this.gaCxWin / 2;
		this.scYSpot = this.scYDest = this.gaCyWin / 2;
	}

	// Charge la frame
	public int startTheFrame()
	{
		int error = 0;

		do
		{
			// Charge la frame
			if (nextFrame != currentFrame)
			{
				gaBorderColour = nextFrame < frameOffset ? 0 : gaAppBorderColour;

				frame = new CRunFrame(this);
				if (frame.loadFullFrame(nextFrame) == false)
				{
					error = -1;
					break;
				}
				currentFrame = nextFrame;

				// setSurfaceEnabled ((frame.leFlags & CRunFrame.LEF_NOSURFACE) == 0);

				/* Skip an event loop to allow the transition between
                   threaded gameloop/surface gameloop  */

				return error;
			}

			Log.Log("Starting new frame");

			if(!MMFRuntime.ADMOB && (hdr2Options & AH2OPT_ENABLEAD) != 0) {
				try
				{
					Log.Log("About to make and request and Ad");
					MMFRuntime.inst.setAdMob
					((hdr2Options & AH2OPT_ENABLEAD) != 0 &&
					(frame.iPhoneOptions & CRunFrame.ANDROIDFOPT_FRAMEAD) != 0,
					(frame.iPhoneOptions & CRunFrame.ANDROIDFOPT_ADBOTTOM) != 0,
					(frame.iPhoneOptions & CRunFrame.ANDROIDFOPT_ADOVERFRAME) != 0);
				}
				catch(Exception e)
				{
					Log.Log("Problem creating AdMob, "+e.toString());
				}
			}
			if(!frame.haveUpdatedViewport)
			{
				frame.haveUpdatedViewport = true;
				MMFRuntime.inst.updateViewport();

			}

			// Init yourapplication variables
			frame.leX = frame.leY = 0;
			frame.leLastScrlX = frame.leLastScrlY = 0;
			frame.rhOK = false;
			frame.levelQuit = 0;

			initScreenZoom();
			
			// Creates logical screen
			int cxLog = Math.min(gaCxWin, frame.leWidth);
			int cyLog = Math.min(gaCyWin, frame.leHeight);
			frame.leEditWinWidth = cxLog;
			frame.leEditWinHeight = cyLog;

			centerDisplay();

			// Calculate maximum number of sprites (add max. number of bkd sprites)
			//int nMaxSprite = frame.maxObjects * 2;		    // * 2 for background objects created as sprites
			//for (short i = 0; i < frame.LOList.nIndex; i++)
			//{
			//    CLO lo = frame.LOList.getLOFromIndex(i);
			//    if (lo.loLayer > 0)
			//    {
			//        COI oi = OIList.getOIFromHandle(lo.loOiHandle);
			//if (oi.oiType < COI.OBJ_SPR)
			//{
			//    nMaxSprite++;
			//}
			//    }
			//}

			// Create collision mask
			int flags = frame.evtProg.getCollisionFlags();
			flags |= frame.getMaskBits();
			frame.leFlags |= CRunFrame.LEF_TOTALCOLMASK;
			frame.colMask = null;
			if ((frame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0)
			{
				if ((flags & (CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM)) != 0)
				{
					frame.colMask = CColMask.create(-CColMask.COLMASK_XMARGIN, -CColMask.COLMASK_YMARGIN, frame.leWidth + CColMask.COLMASK_XMARGIN, frame.leHeight + CColMask.COLMASK_YMARGIN, flags);
				}
			}
			// COLMASK:
				/*
            else
            ColMask_Create (idEditWin, COLMASK_XMARGIN, COLMASK_YMARGIN, pCurFrame->m_dwColMaskBits);
				 */

			// Reset CPT VBL
			newResetCptVbl();

			appRunningState = SL_FRAMELOOP;

		} while (false);

		// Centre l'affichage
		centerDisplay();

		// Flush du clavier
		flushKeyboard();

		// Init RUN LOOP
		run = new CRun(this);

		// Init runloop
		run.initRunLoop(frame.fadeIn != null);
		frame.rhPtr = run;

		// Set app running state
		if (frame.fadeIn != null)
		{
			// Do 1st loop
			if (loopFrame() == false)
			{
				appRunningState = SL_ENDFRAME;
			}
			else
			{
				if (startFrameFadeIn() == false)
				{
					appRunningState = SL_FRAMELOOP;
				}
			}
		}
		else
		{
			appRunningState = SL_FRAMELOOP;
		}

		if (error != 0)
		{
			appRunningState = SL_QUIT;
		}

		return error;
	}

	// Un tour de boucle
	public boolean loopFrame()
	{
		if (frame.levelQuit == 0)
		{
			// One frame loop
			frame.levelQuit = run.doRunLoop();
		}
		return (frame.levelQuit == 0);
	}

	// Sortie d'une boucle
	public void endFrame()
	{
		int ul;

		// Fin de la boucle => renvoyer code de sortie
		ul = run.killRunLoop(frame.levelQuit, false);

		// Run Frame?
		if ((gaNewFlags & GANF_RUNFRAME) != 0)
		{
			appRunningState = SL_QUIT;
		}
		// Calculer event en fonction du code de sortie
		else
		{
			switch (CServices.LOWORD(ul))
			{
			// Next frame
			case 1:				// LOOPEXIT_NEXTLEVEL
				nextFrame = currentFrame + 1;
				appRunningState = SL_STARTFRAME;
				break;

				// Previous frame
			case 2:				// LOOPEXIT_PREVLEVEL:
				nextFrame = Math.max(frameOffset, currentFrame - 1);
				appRunningState = SL_STARTFRAME;
				break;

				// Jump to frame
			case 3:				// LOOPEXIT_GOTOLEVEL:
				appRunningState = SL_STARTFRAME;
				if ((CServices.HIWORD(ul) & 0x8000) != 0)			// Si flag 0x8000, numero de cellule direct
				{
					nextFrame = CServices.HIWORD(ul) & 0x7FFF;
					if (nextFrame >= gaNbFrames)
					{
						nextFrame = gaNbFrames - 1;
					}
					if (nextFrame < 0)
					{
						nextFrame = 0;
					}
				}
				else											// Sinon, HCELL
				{
					if (CServices.HIWORD(ul) < frameMaxHandle)
					{
						nextFrame = frameHandleToIndex[CServices.HIWORD(ul)];
						if (nextFrame == -1)
						{
							nextFrame = currentFrame + 1;
						}
					}
					else
					{
						nextFrame = currentFrame + 1;
					}
				}
				break;

				// Restart application
			case 4:				// LOOPEXIT_NEWGAME:
				// Restart application
				appRunningState = SL_RESTART;
				nextFrame = startFrame;
				break;

				// Quit
			default:
				appRunningState = SL_QUIT;
				break;
			}
		}

		if (appRunningState == SL_STARTFRAME)
		{
			// If invalid frame number, quit current game
			if (nextFrame < 0 || nextFrame >= gaNbFrames)
			{
				appRunningState = SL_QUIT;
			}
		}

		// Flush keyboard
		flushKeyboard();

		// Unload current frame if frame change
		if (appRunningState != SL_STARTFRAME || nextFrame != currentFrame)
		{
			if (frame.colMask != null)
			{
				frame.colMask.destroy ();
				frame.colMask = null;
			}

			// Unload frame
			frame = null;
			run = null;

			// Reset current frame
			currentFrame = -1;
		}
	}

	// RAZ des donnes objets globaux
	public void killGlobalData()
	{
		adGO = null;
	}

	// Gestion du fadein
	public boolean startFrameFadeIn()
	{
		/*    	
        Image pSf1 = null;
        Image pSf2 = null;
        Graphics g;
        CTransitionData pData = frame.fadeIn;

        do
        {
            // V2 transitions
            if (pData != null)
            {
                // Create surfaces
                pSf1 = Image.createImage(editWin.getWidth(), editWin.getHeight());
                pSf2 = Image.createImage(editWin.getWidth(), editWin.getHeight());

                g = pSf2.getGraphics();
                g.drawImage(editWin, 0, 0, Graphics.TOP | Graphics.LEFT);

                // Fill source surface
                if ((pData.transFlags & CTransitionData.TRFLAG_COLOR) != 0)
                {
                    g = pSf1.getGraphics();
                    g.setColor(pData.transColor|0xFF000000);
                    g.fillRect(0, 0, pSf1.getWidth(), pSf2.getHeight());
                }
                else
                {
                    g = pSf1.getGraphics();
                    g.setColor(gaBorderColour|0xFF000000);
                    g.fillRect(0, 0, pSf1.getWidth(), pSf2.getHeight());
                    if (pOldSurf != null)
                    {
                        g.drawImage(pOldSurf, (pSf1.getWidth() - pOldSurf.getWidth()) / 2, (pSf1.getHeight() - pOldSurf.getHeight()) / 2, Graphics.TOP | Graphics.LEFT);
                    }
                }

                // Reset current surface
                g2EditWin.drawImage(pSf1, 0, 0, Graphics.TOP | Graphics.LEFT);

                frame.pTrans = getTransitionManager().createTransition(pData, editWin, pSf1, pSf2);
                if (frame.pTrans == null)
                {
                    break;
                }
                appRunningState = SL_FRAMEFADEINLOOP;

                return true;
            }
        } while (false);

        // En cas d'erreur, creer les objets qui n'ont pas ete crees
        run.createRemainingFrameObjects();
        endFrameFadeIn();
		 */        
		return false;
	}

	public boolean loopFrameFadeIn()
	{
		/*        // V2 transition
        if (frame.pTrans != null)
        {
            // Transition completed ?
            if (frame.pTrans.isCompleted())
            {
                endFrameFadeIn();
                return false;		// Stop
            }

            // One step
            CRect rc[];
            rc = frame.pTrans.stepDraw(CTrans.TRFLAG_FADEIN);

            // Invalidate rects
            if (rc == null)
            {
                winMan.winAddZone(null);
            }
            else
            {
                int n;
                for (n = 0; n < rc.length; n++)
                {
                    winMan.winAddZone(rc[n]);
                }
            }

            // Refresh screen
            canvas.repaint();
            return true;
        }
		 */        
		return false;
	}

	public boolean endFrameFadeIn()
	{
		/*       if (frame.pTrans != null)
        {
            frame.pTrans.end();
            frame.pTrans = null;
            if (appRunningState == SL_FRAMEFADEINLOOP)
            {
                appRunningState = SL_FRAMELOOP;
            }
            run.createRemainingFrameObjects();
        }
		 */        
		return true;
	}

	// Gestion du fadeout
	public boolean startFrameFadeOut()
	{
		/*        Image pSf1 = null;
        Image pSf2 = null;
        CTransitionData pData = frame.fadeOut;
        Graphics g;

        do
        {
            // V2 transitions
            if (pData != null)
            {
                // Create surfaces
                pSf1 = Image.createImage(editWin.getWidth(), editWin.getHeight());
                pSf2 = Image.createImage(editWin.getWidth(), editWin.getHeight());

                g = pSf1.getGraphics();
                g.drawImage(editWin, 0, 0, Graphics.TOP | Graphics.LEFT);

                // Fill destination surface
                g = pSf2.getGraphics();
                if ((pData.transFlags & CTransitionData.TRFLAG_COLOR) != 0)
                {
                    g.setColor(pData.transColor|0xFF000000);
                }
                else
                {
                    g.setColor(0xFF000000);
                }
                g.fillRect(0, 0, pSf2.getWidth(), pSf2.getHeight());

                // Start transition
                frame.pTrans = getTransitionManager().createTransition(pData, editWin, pSf1, pSf2);

                // OK
                appRunningState = SL_FRAMEFADEOUTLOOP;
                return true;
            }
        } while (false);
        endFrameFadeOut();
		 */        
		return false;
	}

	public boolean loopFrameFadeOut()
	{
		/*        // V2 transition
        if (frame.pTrans != null)
        {
            // Transition completed ?
            if (frame.pTrans.isCompleted())
            {
                endFrameFadeOut();
                return false;		// Stop
            }

            // One step
            CRect rc[];
            rc = frame.pTrans.stepDraw(CTrans.TRFLAG_FADEOUT);

            // Invalidate rects
            if (rc == null)
            {
                winMan.winAddZone(null);
            }
            else
            {
                int n;
                for (n = 0; n < rc.length; n++)
                {
                    winMan.winAddZone(rc[n]);
                }
            }
            // Refresh screen
            canvas.repaint();
        }
        // Continue
		 */
		return true;
	}

	public boolean endFrameFadeOut()
	{
		/*        if (frame.pTrans != null)
        {
            frame.pTrans.end();
            frame.pTrans = null;
            if (appRunningState == SL_FRAMEFADEOUTLOOP)
            {
                appRunningState = SL_ENDFRAME;
            }
        }
		 */        
		return true;
	}


	// RAZ du compteur de VBL
	public void newResetCptVbl()
	{
		refTime = System.currentTimeMillis();
	}

	public int newGetCptVbl()
	{
		return (int) (((double) (System.currentTimeMillis() - refTime) * (double) gaFrameRate) / 1000.0);
	}

	// Initialise les variables globales
	public void initGlobal()
	{
		int n;

		// Vies et score
		if (parentApp == null || (parentApp != null && (parentOptions & CCCA.CCAF_SHARE_LIVES) == 0))
		{
			lives = new int[MAX_PLAYER];
			for (n = 0; n < MAX_PLAYER; n++)
			{
				lives[n] = gaLivesInit ^ 0xFFFFFFFF;
			}
		}
		else
		{
			lives = null;
		}
		if (parentApp == null || (parentApp != null && (parentOptions & CCCA.CCAF_SHARE_SCORES) == 0))
		{
			scores = new int[MAX_PLAYER];
			for (n = 0; n < MAX_PLAYER; n++)
			{
				scores[n] = gaScoreInit ^ 0xFFFFFFFF;
			}
		}
		else
		{
			scores = null;
		}
		playerNames = new String[MAX_PLAYER];
		for (n = 0; n < MAX_PLAYER; n++)
		{
			//playerNames[n] = new String("");
			playerNames[n] = "";
		}

		// Global values
		if (parentApp == null || (parentApp != null && (parentOptions & CCCA.CCAF_SHARE_GLOBALVALUES) == 0))
		{
			gValues = new ArrayList<CValue>();
			for (n = 0; n < nGlobalValuesInit; n++)
			{
				gValues.add(new CValue(globalValuesInit[n]));
			}
		}
		else
		{
			gValues = null;
		}
		tempGValue = new CValue();

		// Global strings
		if (parentApp == null || (parentApp != null && (parentOptions & CCCA.CCAF_SHARE_GLOBALVALUES) == 0))
		{
			gStrings = new ArrayList<String>();
			for (n = 0; n < nGlobalStringsInit; n++)
			{
				gStrings.add(new String(globalStringsInit[n]));
			}
		}
		else
		{
			gStrings = null;
		}
	}

	// Retourne les vies et les scores
	public int[] getLives()
	{
		CRunApp app = this;
		while (app.lives == null)
		{
			app = app.parentApp;
		}
		return app.lives;
	}

	public int[] getScores()
	{
		CRunApp app = this;
		while (app.scores == null)
		{
			app = app.parentApp;
		}
		return app.scores;
	}

	// Retourne les controles joueur
	public short[] getCtrlType()
	{
		CRunApp app = this;
		while (app.parentApp != null && (app.parentOptions & CCCA.CCAF_SHARE_PLAYERCTRLS) != 0)
		{
			app = app.parentApp;
		}
		return app.pcCtrlType;
	}

	public int[][] getCtrlKeys()
	{
		CRunApp app = this;
		while (app.parentApp != null && (app.parentOptions & CCCA.CCAF_SHARE_PLAYERCTRLS) != 0)
		{
			app = app.parentApp;
		}
		return app.pcCtrlKeys;
	}

	// Recherche les global values dans les parents
	public ArrayList<CValue> getGlobalValues()
	{
		CRunApp app = this;
		while (app.gValues == null)
		{
			app = app.parentApp;
		}
		return app.gValues;
	}

	public int getNGlobalValues()
	{
		if (gValues != null)
		{
			return gValues.size();
		}
		return 0;
	}

	public ArrayList<String> getGlobalStrings()
	{
		CRunApp app = this;
		while (app.gStrings == null)
		{
			app = app.parentApp;
		}
		return app.gStrings;
	}

	public int getNGlobalStrings()
	{
		if (gStrings != null)
		{
			return gStrings.size();
		}
		return 0;
	}

	public ArrayList<CValue> checkGlobalValue(int num)
	{
		ArrayList<CValue> values = getGlobalValues();

		if (num < 0 || num > 1000)
		{
			return null;
		}
		int oldSize = values.size();
		if (num >= oldSize)
		{
			values.ensureCapacity(num);
			int n;
			for (n = oldSize; n <= num; n++)
			{
				values.add(new CValue());
			}
		}
		return values;
	}

	public CValue getGlobalValueAt(int num)
	{
		// Build 284.2: optimize usual case
		if ( gValues != null && num >= 0 && num < gValues.size() )
			return gValues.get(num);

		ArrayList<CValue> values = checkGlobalValue(num);
		if (values != null)
		{
			return values.get(num);
		}
		return tempGValue;
	}

	public void setGlobalValueAt(int num, CValue value)
	{
		// Build 284.2: optimize usual case
		if ( gValues != null && num >= 0 && num < gValues.size() )
			gValues.get(num).forceValue(value);
		else
		{
			ArrayList<CValue> values = checkGlobalValue(num);
			if(values != null)
			{
				values.get(num).forceValue(value);
			}
		}
	}

	public ArrayList<String> checkGlobalString(int num)
	{
		ArrayList<String> strings = getGlobalStrings();

		if (num < 0 || num > 1000)
		{
			return null;
		}
		int oldSize = strings.size();
		if (num >= oldSize)
		{
			strings.ensureCapacity(num);
			int n;
			for (n = oldSize; n <= num; n++)
			{
				//strings.add(new String(""));
				strings.add("");
			}
		}
		return strings;
	}

	public String getGlobalStringAt(int num)
	{
		// Build 284.2: optimize usual case
		if ( gStrings != null && num < gStrings.size() )
			return gStrings.get(num);

		ArrayList<String> strings = checkGlobalString(num);
		if (strings != null)
		{
			return strings.get(num);
		}
		return "";
	}

	public void setGlobalStringAt(int num, String value)
	{
		// Build 284.2: optimize usual case
		if ( gStrings != null && num < gStrings.size() )
			gStrings.set(num, new String(value));
		else
		{
			ArrayList<String> strings = checkGlobalString(num);
			if (strings != null)
			{
				strings.set(num, new String(value));
			}
		}
	}

	public int widthSetting, heightSetting;

	// Charge le header de l'application
	public void loadAppHeader()
	{
		file.skipBytes(4);			// Structure size
		gaFlags = file.readAShort();   		// Flags
		gaNewFlags = file.readAShort();		// New flags
		gaMode = file.readAShort();		// graphic mode
		gaOtherFlags = file.readAShort();		// Other Flags

		widthSetting = file.readAShort();		// Window x-size
		heightSetting = file.readAShort();		// Window y-size

		gaScoreInit = file.readAInt();		// Initial score
		gaLivesInit = file.readAInt();		// Initial number of lives
		int n, m;
		for (n = 0; n < MAX_PLAYER; n++)
		{
			pcCtrlType[n] = file.readAShort();	// Control type per player (0=mouse,1=joy1, 2=joy2, 3=keyb)
		}
		for (n = 0; n < MAX_PLAYER; n++)
		{
			for (m = 0; m < MAX_KEY; m++)
			{
				pcCtrlKeys[n][m] = CKeyConvert.getJavaKey(file.readAShort());
			}
		}
		gaAppBorderColour = file.readAColor(); // Border colour
		gaBorderColour = gaAppBorderColour;
		gaNbFrames = file.readAInt();		// Number of frames
		gaFrameRate = file.readAInt();		// Number of frames per second
		gaMDIWindowMenu = file.readByte();	// Index of Window menu for MDI applications
		file.skipBytes(3);
	}

	public void loadAppHeader2()
	{
		hdr2Options=file.readAInt();
		file.skipBytes(10);
		file.readAShort(); /* orientation */

		viewMode = file.readAShort();
		hdr2Options2 = file.readAShort();

		if (parentApp == null)
		{
			if (MMFRuntime.inst.enableCrashReporting == false)
				MMFRuntime.inst.enableCrashReporting = (hdr2Options & AH2OPT_CRASHREPORTING) != 0;

			if ((hdr2Options & AH2OPT_OPENGL30) != 0)
				SurfaceView.ES = 3;
			else if ((hdr2Options & AH2OPT_OPENGL1) != 0)
				SurfaceView.ES = 1;
			else
				SurfaceView.ES = 2;
		}
	}

	// Charge le chunk GlobalValues
	public void loadGlobalValues()
	{
		nGlobalValuesInit = file.readAShort();
		globalValuesInit = new int[nGlobalValuesInit];
		globalValuesInitTypes = new byte[nGlobalValuesInit];
		int n;
		for (n = 0; n < nGlobalValuesInit; n++)
		{
			globalValuesInit[n] = file.readAInt();
		}
		file.read(globalValuesInitTypes);
	}

	// Charge le chunk GlobalStrings
	public void loadGlobalStrings()
	{
		nGlobalStringsInit = (short) file.readAInt();
		globalStringsInit = new String[nGlobalStringsInit];
		int n;
		for (n = 0; n < nGlobalStringsInit; n++)
		{
			globalStringsInit[n] = file.readAString();
		}
	}

	// Charge le chunk Frame handles
	public void loadFrameHandles(int size)
	{
		frameMaxHandle = (short) (size / 2);
		frameHandleToIndex = new short[frameMaxHandle];

		int n;
		for (n = 0; n < frameMaxHandle; n++)
		{
			frameHandleToIndex[n] = file.readAShort();
		}
	}

	// Transformation d'un HCELL en numero de cellule
	public short HCellToNCell(short hCell)
	{
		if (frameHandleToIndex == null || hCell == -1 || hCell >= frameMaxHandle)
		{
			return -1;
		}
		return frameHandleToIndex[hCell];
	}

	// ---------------------------------------------------------------------------------
	// EMBEDDED FILES
	// ---------------------------------------------------------------------------------
	public CEmbeddedFile getEmbeddedFile(String path)
	{
		int n, p;
		if (embeddedFiles != null)
		{
			String pathname = path;
			p = pathname.lastIndexOf(File.separator);
			if (p < 0)
				p = pathname.lastIndexOf('\\');
			if (p >= 0)
				pathname = pathname.substring(p + 1);

			for (n = 0; n < embeddedFiles.length; n++)
			{
				String embPathname = embeddedFiles[n].path;
				p = embPathname.lastIndexOf(File.separator);
				if (p < 0)
					p = embPathname.lastIndexOf('\\');
				if (p >= 0)
					embPathname = embPathname.substring(p + 1);

				if (embPathname.compareToIgnoreCase(pathname) == 0)
				{
					return embeddedFiles[n];
				}
			}
		}
		return null;
	}

	public class HFile
	{
		public InputStream stream;
		public HttpURLConnection connection;

		public void close()
		{
			if(stream != null)
			{
				try
				{	stream.close();
				}
				catch (IOException e)
				{
				}

				stream = null;
			}

			if (connection != null)
			{
				connection.disconnect();
				connection = null;
			}
		}

		@Override
		public void finalize()
		{
			close();
		}
	}

	public HFile openHFile(String path, boolean checkpermissions)
	{
		HFile file = new HFile();

		if (path != null && path.length() > 0)
		{
			// First check is file exist
			// IF NOT check internal
			// IF Both not exist, try URL

			CEmbeddedFile embed = null;
			File lfile = new File(path);

			if(lfile == null || !lfile.exists())
				embed = getEmbeddedFile(path);

			if (embed != null)
			{
				file.stream = embed.getInputStream();
				return file;
			}
			else
			{
				try
				{
	    	    	if(!checkpermissions) {
	    				MMFRuntime.inst.askForPermissionsApi23();		
	    	    		throw new IOException();
	    	    	}
	    	    	
					if(!lfile.exists())
						throw new IOException();

					file.stream = new FileInputStream(path);
					return file;
				}
				catch (IOException e)
				{
					if(MMFRuntime.inst.obbAvailable) {
						file.stream = MMFRuntime.inst.getInputStreamFromAPK(path);
						if(file.stream != null)
							return file;
					}
					try
					{
						URL url = new URL(path);

						file.connection = (HttpURLConnection) url.openConnection();
						file.stream = file.connection.getInputStream();

						return file;
					}
					catch(Exception ue)
					{
						try
						{
							Uri uri = Uri.fromFile(lfile);
							if(uri.getScheme().contains("file"))
							{
								String assetname = uri.getLastPathSegment();
								AssetManager am = MMFRuntime.inst.getAssets();
								file.connection = null;
								file.stream = am.open(assetname);
							}

							return file;
						}
						catch(Exception ie)
						{
							Log.Log(ie.toString());
						}
					}

				}
			}
		}

		return null;
	}

	public static abstract class FileRetrievedHandler
	{
		public abstract void onRetrieved(HFile file, InputStream stream);
		public abstract void onFailure();
	}

	public void retrieveHFile(String path, final FileRetrievedHandler handler)
	{
		(new AsyncTask<String, Integer, Long>()
				{
			@Override
			public Long doInBackground(String... paths)
			{
				final HFile file = openHFile(paths[0], true);

				if(file == null)
				{
					handler.onFailure();
					return (long) 0;
				}

				final String id = "HFile-" + file.hashCode();

				if (!MMFRuntime.inst.inputStreamToFile(file.stream, id))
				{
					MMFRuntime.inst.runOnRuntimeThread(new Runnable()
					{
						@Override
						public void run()
						{
							handler.onFailure();
						}
					});

					return (long) 0;
				}

				MMFRuntime.inst.runOnRuntimeThread(new Runnable()
				{
					@Override
					public void run()
					{
						try
						{
							handler.onRetrieved(file, new FileInputStream(MMFRuntime.inst.getFilesDir() + "/" + id));
						}
						catch(IOException e)
						{
							handler.onFailure();
						}

						MMFRuntime.inst.deleteFile(id);
					}
				});

				this.cancel(true);
				return (long) 1;
			}

				}).execute(path);
	}

	public String getBinTempName(String path)
	{
		String ret = path;

		if (path != null && path.length() > 0)
		{
			CEmbeddedFile embed = getEmbeddedFile(path);
			if (embed != null)
			{
				return embed.extractedTo;
			}
		}
		return ret;
	}

	// Force la position de la souris
	// ------------------------------
	public void setCursorPos(int x, int y)
	{
		mouseX = x;
		mouseY = y;
	}

	// ---------------------------------------------------------------------------------
	// EVENT HANDLERS
	// ---------------------------------------------------------------------------------
	public void keyDown(int keyCode)
	{
		if (keyCode < 0 || keyCode >= keyBuffer.length)
			return;

		//* 
		if (keyCode == KeyEvent.KEYCODE_BACK &&
				(hdr2Options & CRunApp.AH2OPT_DISABLEBACKBUTTON) != 0)
		{
			for (Iterator <CRunAndroid> it = androidObjects.iterator ();
					it.hasNext (); )
			{
				CRunAndroid ext = it.next();

				ext.backButtonEvent = ext.ho.getEventCount();
				ext.ho.generateEvent (10, 0);
			}
		}
		else if (keyCode == KeyEvent.KEYCODE_MENU)
		{
			for (Iterator <CRunAndroid> it = androidObjects.iterator ();
					it.hasNext (); )
			{
				CRunAndroid ext = it.next();

				ext.menuButtonEvent = ext.ho.getEventCount();
				ext.ho.generateEvent (12, 0);
			}
		}

		keyBuffer[keyCode] = 1;

		if (run != null && run.rhEvtProg != null)
			run.rhEvtProg.onKeyDown(keyCode);

		if(androidObjects.size() > 0) 
		{
			for (Iterator <CRunAndroid> it = androidObjects.iterator ();
					it.hasNext (); )
			{
				CRunAndroid ext = it.next();
	
				ext.lastKeyPressed = keyCode;
			}
		}
	}
	public void keyUp(int keyCode)
	{
		if (keyCode < 0 || keyCode >= keyBuffer.length)
			return;

		keyBuffer[keyCode] = 0;
	}

	public void trackballMove(int dx, int dy)
	{
		mouseX+=dx;
		if (mouseX<0)
		{
			mouseX=0;
		}
		if (mouseX>=gaCxWin)
		{
			mouseX=gaCxWin-1;
		}
		mouseY+=dy;
		if (mouseY<0)
		{
			mouseY=0;
		}
		if (mouseY>=gaCyWin)
		{
			mouseY=gaCyWin-1;
		}
	}

	public int touchCount = 0;
	private int touchID = -1;

	@Override
	public void newTouch(int id, float x, float y)
	{		
		if(modalSubApp == null && container != null)
			container.resume();
		
		if(touchID != -1)
			return;
		
		if (++ touchCount == 1)
			keyBuffer[CKeyConvert.VK_LBUTTON] = 1;		

		touchID = id;
		
		mouseX = (int) x;
		mouseY = (int) y;
		
		if(this.container != null) {
			mouseX -= absoluteX;
			mouseY -= absoluteY;
		}

		if(mouseX < 0 || mouseY < 0 || mouseX > gaCxWin || mouseY > gaCyWin)
			return;

		if (run != null && run.rhEvtProg != null)
		{
			if (System.currentTimeMillis()-doubleClickTimer<DOUBLECLICKTIME)
			{
				run.rhEvtProg.onMouseButton(0, 2);
				doubleClickTimer=0;
			}
			else
			{
				run.rhEvtProg.onMouseButton(0, 1);
				doubleClickTimer=System.currentTimeMillis();
			}
		}
	}

	@Override
	public void touchMoved(int id, float x, float y)
	{
		if (touchCount != 0 && id == touchID) {
			mouseX = (int) x;
			mouseY = (int) y;
			if(this.container != null) {
				mouseX -= absoluteX;
				mouseY -= absoluteY;
			}
		}

	}

	@Override
	public void endTouch(int id)
	{
		if (-- touchCount <= 0 && id == touchID || id == -1) {
			keyBuffer[CKeyConvert.VK_LBUTTON] = 0;
			touchCount = 0;
			touchID = -1;
		}
				
	}

	public void flushKeyboard()
	{
		int k;
		for (k = 0; k < CKeyConvert.MAX_VK; k++)
		{
			keyBuffer[k] = 0;
		}
	}

	public boolean getKeyState(int keyCode)
	{
		if (keyCode < CKeyConvert.MAX_VK)
		{
			return keyBuffer[keyCode] != 0;
		}
		return false;
	}

	public void createJoystick(boolean bCreate, int flags)
	{
		if(bCreate)
		{
			if(joystick == null)
			{
				joystick = new CJoystick(this, flags);
				MMFRuntime.inst.touchManager.addTouchAware(joystick);
			}

			joystick.reset(flags);
		}
		else
		{
			if(joystick != null)
			{
				MMFRuntime.inst.touchManager.removeTouchAware(joystick);
				joystick = null;
			}
		}
	}

	public void createJoystickAcc(boolean bCreate)
	{
		if(bCreate)
		{
			if(joystickAcc == null)
				joystickAcc = new CJoystickAcc(this);
		}
		else
		{
			if(joystickAcc != null)
			{
				joystickAcc.deactivate();
				joystickAcc = null;
			}
		}
	}

	public void destroyJoystick()
	{
		joystick = null;
		joystickAcc = null;
	}

	public int absoluteX, absoluteY;

	private ClipboardManager Clipboard = null;

	public void clipboard(String s)
	{
		if(Clipboard == null) {
			Clipboard = ((ClipboardManager)
					MMFRuntime.inst.getSystemService(Context.CLIPBOARD_SERVICE));
		}

		if(Clipboard != null) {
			ClipData clipdata = ClipData.newPlainText("clip_text", s);
			Clipboard.setPrimaryClip(clipdata);
		}
	}

	public String clipboard()
	{
		String ret = "";
		if(Clipboard != null) {
			if (Clipboard.hasPrimaryClip()) {
				android.content.ClipDescription description = Clipboard.getPrimaryClipDescription();
				android.content.ClipData data = Clipboard.getPrimaryClip();
				if (data != null && description != null && description.hasMimeType(ClipDescription.MIMETYPE_TEXT_PLAIN))
					ret = String.valueOf(data.getItemAt(0).getText());
			}
		}

		return ret;
	}
	private void loadImageShapes(CFile f)
	{
		int nshapes = f.readAInt();		// number of shapes
		int i;
		for (i=0; i<nshapes; i++)
		{
			CImageShape shape = new CImageShape();
			shape.load(f);
			imageShapes.add(shape);
		}
		Log.Log("Number of shapes: " + nshapes);
	}
}
