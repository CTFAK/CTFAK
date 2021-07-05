//----------------------------------------------------------------------------------
//
// CRUNAPP : Classe Application
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Expressions;
using RuntimeXNA.OI;
using RuntimeXNA.Banks;
using RuntimeXNA.Frame;
using RuntimeXNA.Sprites;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Extensions;
using RuntimeXNA.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if !WINDOWS_PHONE
using Microsoft.Xna.Framework.Storage;
#else
using Microsoft.Xna.Framework.Input.Touch;
//using System.Device.Location;
#endif

namespace RuntimeXNA.Application
{
    public class CRunApp
    {
        public const short RUNTIME_VERSION = 0x0302;
        public const short MAX_PLAYER = 4;
        public const short MAX_KEY = 8;
        //    public const short GA_BORDERMAX=0x0001;
        public const short GA_NOHEADING = 0x0002;
        //    public const short GA_PANIC=0x0004;
        public const short GA_SPEEDINDEPENDANT = 0x0008;
        public const short GA_STRETCH = 0x0010;
        public const short GA_MENUHIDDEN = 0x0080;
        public const short GA_MENUBAR = 0x0100;
        public const short GA_MAXIMISE = 0x0200;
        public const short GA_MIX = 0x0400;
        public const short GA_FULLSCREENATSTART = 0x0800;
        //    public const short GA_FULLSCREENSWITCH=0x1000;
        //    public const short GA_PROTECTED=0x2000;
        //    public const short GA_COPYRIGHT=0x4000;
        //    public const short GA_ONEFILE=(short)0x8000;
        public const short GANF_SAMPLESOVERFRAMES = 0x0001;
        //    public const short GANF_RELOCFILES=0x0002;
        public const short GANF_RUNFRAME = 0x0004;
        //    public const short GANF_SAMPLESEVENIFNOTFOCUS=0x0008;
        //    public const short GANF_NOMINIMIZEBOX=0x0010;
        //    public const short GANF_NOMAXIMIZEBOX=0x0020;
        public const short GANF_NOTHICKFRAME = 0x0040;
        public const short GANF_DONOTCENTERFRAME = 0x0080;
        //    public const short GANF_SCREENSAVER_NOAUTOSTOP=0x0100;
        public const short GANF_DISABLE_CLOSE = 0x0200;
        public const short GANF_HIDDENATSTART = 0x0400;
        //    public const short GANF_XPVISUALTHEMESUPPORT=0x0800;
        //    public const short GANF_VSYNC=0x1000;
        //    public const short GANF_RUNWHENMINIMIZED=0x2000;
        public const short GANF_MDI = 0x4000;
        //    public const short GANF_RUNWHILERESIZING=(short)0x8000;
        //    public const short GAOF_DEBUGGERSHORTCUTS=0x0001;
        //    public const short GAOF_DDRAW=0x0002;
        //    public const short GAOF_DDRAWVRAM=0x0004;
        //    public const short GAOF_OBSOLETE=0x0008;
        //    public const short GAOF_AUTOIMGFLT=0x0010;
        //    public const short GAOF_AUTOSNDFLT=0x0020;
        //    public const short GAOF_ALLINONE=0x0040;
        //    public const short GAOF_SHOWDEBUGGER=0x0080;
        public const short GAOF_JAVASWING = 0x1000;
        public const short GAOF_JAVAAPPLET = 0x2000;
        public const short SL_RESTART = 0;
        public const short SL_STARTFRAME = 1;
        public const short SL_FRAMEFADEINLOOP = 2;
        public const short SL_FRAMELOOP = 3;
        public const short SL_FRAMEFADEOUTLOOP = 4;
        public const short SL_ENDFRAME = 5;
        public const short SL_QUIT = 6;
        public const int MAX_VK = 523;
        public const short CTRLTYPE_MOUSE = 0;
        public const short CTRLTYPE_JOY1 = 1;
        public const short CTRLTYPE_JOY2 = 2;
        public const short CTRLTYPE_JOY3 = 3;
        public const short CTRLTYPE_JOY4 = 4;
        public const short CTRLTYPE_KEYBOARD = 5;
        //    public const short ARF_MENUINIT=0x0001;
        //    public const short ARF_MENUIMAGESLOADED=0x0002;		// menu images have been loaded into memory
        public const short ARF_INGAMELOOP = 0x0004;
        //    public const short ARF_PAUSEDBEFOREMODALLOOP=0x0008;
        public const int AH2OPT_STATUSLINE = 0x0040;
        public const int AH2OPT_EDITPRESENT = 0x0400;

        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteBatchEffect spriteBatch;
        public GraphicsDevice graphicsDevice; 
        public ContentManager content;
        public int displayType;
        public int[] frameOffsets;
        public int frameMaxIndex = 0;
        public string[] framePasswords;
        public string appName = null;
        public string appCopyright = null;
        public string appAboutText = null;
        public string appDoc = null;
        public short nGlobalValuesInit = 0;
        public byte[] globalValuesInitTypes;
        public int[] globalValuesInit;
        public short nGlobalStringsInit = 0;
        public string[] globalStringsInit;
        public COIList OIList;
        public CImageBank imageBank;
        public CFontBank fontBank;
        public CSoundBank soundBank;
        public CSoundPlayer soundPlayer;
        public int appRunningState = 0;
        public int[] lives = null;
        public int[] scores;
        public string[] playerNames;
        public CArrayList gValues;
        public CArrayList gStrings;
        public CValue tempGValue;
        public int startFrame = 0;
        public int nextFrame;
        public int currentFrame;
        public CRunFrame frame = null;
        public CFile file = null;
        public CRunApp parentApp = null;
        public int parentOptions;
        public int parentX;
        public int parentY;
        public int parentWidth;
        public int parentHeight;
        //        public long refTime;
        //        public CRun run = null;
        public bool redrawBack = true;
        // Application header
        public short gaFlags;				// Flags
        public short gaNewFlags;				// New flags
        public short gaMode;				// graphic mode
        public short gaOtherFlags;				// Other Flags	
        public int gaCxWin;				// Window x-size
        public int gaCyWin;				// Window y-size
        public int gaScoreInit;				// Initial score
        public int gaLivesInit;				// Initial number of lives
        public int gaBorderColour;				// Border colour
        public int gaNbFrames;				// Number of frames
        public int gaFrameRate;				// Number of frames per second
        public short[] pcCtrlType = new short[4];		// Control type per player (0=mouse,1=joy1, 2=joy2, 3=keyb)
        public Keys[] pcCtrlKeys = new Keys[4 * 8];	// Control keys per player
        public short[] frameHandleToIndex;
        public short frameMaxHandle;
        public short appRunFlags = 0;
        public CArrayList adGO = null;
        public CArrayList sysEvents = null;
        bool quit = false;
        public CExtLoader extLoader = null;
        public bool m_bLoading = false;
        public bool bVisible = false;
        public bool bPositionWindow = false;
        public bool bResizeWindow = false;
        public int debug = 0;
        public CArrayList extensionStorage = null;
        public CEmbeddedFile[] embeddedFiles = null;
        //        public CTransitionManager transitionManager = null;
        public bool internalPaintFlag = false;
        public bool bUnicode = false;
        public int VBL;
        public long timer=0;
        public double timeDouble = 0;
        public CSpriteGen spriteGen;
        public CRun run = null;
        public CServices services;
        public Rectangle tempRect;
        public Game1 game;
        public int xOffset = 0;
        public int yOffset=0;
        public bool bSubAppShown = false;
        public int numberOfTouches;
        public int hdr2Options;
        public int hdr2Orientation;
        public bool bSignedIn;
        public CArrayList advertisements = null;

#if !WINDOWS_PHONE
        public StorageDevice storageDevice=null;
#else
        Microsoft.Devices.Sensors.Accelerometer accel;
        int nAccel=0;
        public static float accX;
        public static float accY;
        public static float accZ;
        public static float filteredAccX;
        public static float filteredAccY;
        public static float filteredAccZ;
        public static float instantAccX;
        public static float instantAccY;
        public static float instantAccZ;
        const float kFilteringFactor = (float)0.1;
        public static DisplayOrientation orientation;
        public CExtension XNAObject = null;
#endif
        public CRunApp()
        {
        }

        public CRunApp(Game1 gam, CFile f)
        {
            game = gam;
            file = f;
            content = gam.Content;
            graphicsDeviceManager=gam.graphics;
            graphicsDevice=gam.GraphicsDevice;
            spriteBatch = gam.spriteBatch;
        }

        public void setParentApp(CRunApp pApp, int sFrame, int options, int x, int y, int width, int height)
        {
            parentApp = pApp;
            parentOptions = options;
            startFrame = sFrame;
            xOffset = x;
            yOffset = y;
            parentWidth = width;
            parentHeight = height;
        }

        public void setOffsets(int x, int y)
        {
            xOffset=x;
            yOffset=y;
            spriteGen.setOffsets(x, y);
        }

        public void showSubApp(bool bShown)
        {
            bSubAppShown = bShown;
        }
        public bool load()
        {
            // Charge le mini-header
            byte[] name = new byte[4];
            file.read(name);		    // gaType
            bool bOK=false;
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
            int prdVersion=file.readAInt();		    // gaPrdVersion
            int prdBuild=file.readAInt();		    // gaPrdBuild
            if (prdBuild<249)
            {
                return false;
            }

            // Reserve les objets
            OIList = new COIList();
            imageBank = new CImageBank(this);
            fontBank = new CFontBank(this);
            soundBank = new CSoundBank();
            soundPlayer = new CSoundPlayer(this);
//            eventLoader = new CEventLoader(this);

            // Lis les chunks
            CChunk chk = new CChunk();
            int posEnd;
            int nbPass = 0, n;
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
                        loadAppHeader(file);
                        // Buffer pour les offsets frame
                        frameOffsets = new int[gaNbFrames];
                        // Pour les password
                        framePasswords = new string[gaNbFrames];
                        for (n = 0; n < gaNbFrames; n++)
                        {
                            framePasswords[n] = null;
                        }
                        break;
                    // CHUNK_APPHEADER2
                    case 0x2245:
                        hdr2Options = file.readAInt();
                        file.skipBytes(10);
                        hdr2Orientation = file.readAShort();
                        break;
                    // CHUNK_APPNAME
                    case 0x2224:
                        appName = file.readAString();
                        break;
                    // CHUNK_COPYRIGHT
                    case 0x223B:
                        appCopyright = file.readAString();
                        break;
                    // CHUNK_ABOUTTEXT
                    case 0x223A:
                        appAboutText = file.readAString();
                        break;
                    // CHUNK_APPEDITORFILENAME:
                    case 0x222E:
//                        appEditorFilename = file.readAString();
//                        appEditorFilename = getParent(appEditorFilename);
                        break;
                    // CHUNK_APPTARGETFILENAME:
                    case 0x222F:
//                        appTargetFilename = file.readAString();
//                        appTargetFilename = getParent(appTargetFilename);
                        break;
                    // CHUNK_APPDOC
                    case 0x2230:
                        appDoc = file.readAString();
                        break;
                    // CHUNK_GLOBALVALUES
                    case 0x2232:
                        loadGlobalValues(file);
                        break;
                    // CHUNK_GLOBALSTRINGS
                    case 0x2233:
                        loadGlobalStrings(file);
                        break;
                    // CHUNK_FRAMEITEMS
                    // CHUNK_FRAMEITEMS_2
                    case 0x2229:
                    case 0x223F:
                        OIList.preLoad(file);
                        break;
                    // CHUNK_FRAMEHANDLES
                    case 0x222B:
                        loadFrameHandles(file, chk.chSize);
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
                            int frPosEnd = file.getFilePointer() + frChk.chSize;

                            switch (frChk.chID)
                            {
                                // CHUNK_FRAMEHEADER
                                case 0x3334:
                                    break;
                                // CHUNK_FRAMEPASSWORD
                                case 0x3336:
                                    string pass = file.readAString();
                                    framePasswords[frameMaxIndex] = pass;
                                    nbPass++;
                                    break;
                            }
                            file.seek(frPosEnd);
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
                        for (n = 0; n < nFiles; n++)
                        {
                            embeddedFiles[n] = new CEmbeddedFile(this);
                            embeddedFiles[n].preLoad();
                        }
                        break;
                    // CHUNK_IMAGESOFFSETS
                    case 0x5555:
                        break;
                    // CHUNK_FONTSOFFSETS
                    case 0x5556:
                        break;
                    // CHUNK_SOUNDSOFFSETS
                    case 0x5557:
                        break;
                    // CHUNK_MUSICSOFFSETS
                    case 0x5558:
                        break;
                    // CHUNK_IMAGE
                    case 0x6666:
                        imageBank.preLoad();
                        break;
                    // CHUNK_FONT
                    case 0x6667:
                        fontBank.preLoad();
                        break;
                    // CHUNK_SOUNDS
                    case 0x6668:
                        soundBank.preLoad(this);
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
        public bool startApplication()
        {
            // Initialisation des events
            sysEvents = new CArrayList();

            // Taille de la fenetre
            graphicsDeviceManager.PreferredBackBufferWidth = gaCxWin;
            graphicsDeviceManager.PreferredBackBufferHeight = gaCyWin;
#if!WINDOWS_PHONE
            if ((gaFlags & GA_FULLSCREENATSTART) != 0)
            {
                setFullScreen(true);
            }
#else
            if ((hdr2Options & AH2OPT_STATUSLINE) == 0)
            {
                graphicsDeviceManager.IsFullScreen = true;
            }
            else
            {
                graphicsDeviceManager.IsFullScreen = false;
            }
            switch(hdr2Orientation)
            {
                case 0:
                    graphicsDeviceManager.SupportedOrientations = DisplayOrientation.Portrait;
                    break;
                case 1:
                    graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft;
                    break;
                case 2:
                    graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeRight;
                    break;
                case 3:
                    graphicsDeviceManager.SupportedOrientations = DisplayOrientation.Default;
                    break;
            }
#endif
            graphicsDeviceManager.ApplyChanges();


            spriteGen = new CSpriteGen();
            spriteGen.setOffsets(xOffset, yOffset);
            run = new CRun(this);
            services = new CServices();
            tempRect = new Rectangle();
            setFrameRate(gaFrameRate);

            numberOfTouches = 0;
#if WINDOWS_PHONE
            TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
            if (tc.IsConnected)
            {
                numberOfTouches=tc.MaximumTouchCount;
            }
            startOrientationHandler();
#endif
            // Fixer le path de l'appli
            displayType = -1;		    // Force l'ouverture de la fenetre
            appRunningState = 0;	    // SL_RESTART
            currentFrame = -2;

            return true;
        }

        // Fait fonctionner l'application
        public bool playApplication(bool bOnlyRestartApp, double time)
        {
            int error = 0;
            bool bLoop = true;
            bool bContinue = true;

            VBL++;
            timeDouble = time;
            timer = (long)time;

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
                        goto case 1;
                    // SL_STARTFRAME
                    case 1:
                        error = startTheFrame();
                        break;
                    // SL_FRAMELOOP
                    case 3:
                        if (loopFrame() == false)
                        {
                            endFrame();
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

            // Continue?
            return bContinue;
        }

        // End application
        public void endApplication()
        {
            // Remove temp files
/*            if (embeddedFiles != null)
            {
                int n;
                for (n = 0; n < embeddedFiles.length; n++)
                {
                    embeddedFiles[n].releaseFileAlways();
                }
                embeddedFiles = null;
            }
*/
            // Stop sounds
/*            if (soundPlayer != null)
            {
                soundPlayer.stopAllSounds();
            }
            if (musicPlayer != null)
            {
                musicPlayer.stop();
            }
 */
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
                    frame = new CRunFrame(this);
                    if (frame.loadFullFrame(nextFrame) == false)
                    {
                        error = -1;
                        break;
                    }
 
                    currentFrame = nextFrame;
                }

                // Init runtime variables
                frame.leX = frame.leY = 0;
                frame.leLastScrlX = frame.leLastScrlY = 0;
                frame.rhOK = false;
                frame.levelQuit = 0;

                // Creates logical screen
                int cxLog;
                int cyLog;
                cxLog = Math.Min(gaCxWin, frame.leWidth);
                cyLog = Math.Min(gaCyWin, frame.leHeight);
                frame.leEditWinWidth = cxLog;
                frame.leEditWinHeight = cyLog;

                // Calculate maximum number of sprites (add max. number of bkd sprites)
/*                int nMaxSprite = frame.maxObjects * 2;		    // * 2 for background objects created as sprites
                for (short i = 0; i < frame.LOList.nIndex; i++)
                {
                    CLO lo = frame.LOList.getLOFromIndex(i);
                    COI oi = OIList.getOIFromHandle(lo.loOiHandle);
                    if (oi.oiType < COI.OBJ_SPR)
                    {
                        nMaxSprite++;
                    }
                }
*/
                // Create collision mask
                int flags = frame.evtProg.getCollisionFlags();
                flags |= frame.getMaskBits();
                frame.leFlags |= CRunFrame.LEF_TOTALCOLMASK;
                frame.colMask = null;
                if ((flags & (CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM)) != 0)
                {
                    frame.colMask = CColMask.create(-CColMask.COLMASK_XMARGIN, -CColMask.COLMASK_YMARGIN, frame.leWidth + CColMask.COLMASK_XMARGIN, frame.leHeight + CColMask.COLMASK_YMARGIN, flags);
                }

                // Met le titre de la fenetre
                setLevelTitle();

                // Reset CPT VBL
                newResetCptVbl();

            } while (false);

            // Flush du clavier
//            flushKeyboard();

            // Idle timer
#if WINDOWS_PHONE
            if ((frame.iPhoneOptions & CRunFrame.IPHONEOPT_SCREENLOCKING) != 0)
            {
                Microsoft.Phone.Shell.PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            }
            else
            {
                Microsoft.Phone.Shell.PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
            }
#endif
            // Init runloop
            bResizeWindow = true;
            run.setFrame(frame);
            run.initRunLoop();
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
        public bool loopFrame()
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
                        nextFrame = Math.Max(0, currentFrame - 1);
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
//            flushKeyboard();

            // Unload current frame if frame change
            if (appRunningState != SL_STARTFRAME || nextFrame != currentFrame)
            {
                // Reset current frame
                currentFrame = -1;
            }
        }

        public void draw()
        {
//          spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, effect);
            spriteBatch.Begin();
            run.draw();
            spriteBatch.End();
        }

        // RAZ des donnes objets globaux
        public void killGlobalData()
        {
            adGO = null;
        }

        // Gestion du fadein
        public bool startFrameFadeIn()
        {
            return false;
        }

        public bool loopFrameFadeIn()
        {
            return false;
        }

        public bool endFrameFadeIn()
        {
            return true;
        }

        // Gestion du fadeout
        public bool startFrameFadeOut()
        {
            return false;
        }

        public bool loopFrameFadeOut()
        {
            return false;
        }

        public bool endFrameFadeOut()
        {
            return true;
        }

        // Initialise les variables globales
        public void initGlobal()
        {
            int n;

            // Vies et score
            if (parentApp == null || (parentApp != null /*&& (parentOptions & CCCA.CCAF_SHARE_LIVES) == 0*/))
            {
                lives = new int[MAX_PLAYER];
                for (n = 0; n < MAX_PLAYER; n++)
                {
                    lives[n] = gaLivesInit ^ -1;
                }
            }
            else
            {
                lives = null;
            }
            if (parentApp == null || (parentApp != null /*&& (parentOptions & CCCA.CCAF_SHARE_SCORES) == 0*/))
            {
                scores = new int[MAX_PLAYER];
                for (n = 0; n < MAX_PLAYER; n++)
                {
                    scores[n] = gaScoreInit ^ -1;
                }
            }
            else
            {
                scores = null;
            }
            playerNames = new string[MAX_PLAYER];
            for (n = 0; n < MAX_PLAYER; n++)
            {
                playerNames[n] = "";
            }

            // Global values
            if (parentApp == null || (parentApp != null /*&& (parentOptions & CCCA.CCAF_SHARE_GLOBALVALUES) == 0*/))
            {
                gValues = new CArrayList();
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
            if (parentApp == null || (parentApp != null /*&& (parentOptions & CCCA.CCAF_SHARE_GLOBALVALUES) == 0*/))
            {
                gStrings = new CArrayList();
                for (n = 0; n < nGlobalStringsInit; n++)
                {
                    gStrings.add(globalStringsInit[n]);
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
            while (app.parentApp != null /*&& (app.parentOptions & CCCA.CCAF_SHARE_PLAYERCTRLS) != 0*/)
            {
                app = app.parentApp;
            }
            return app.pcCtrlType;
        }

        public Keys[] getCtrlKeys()
        {
            CRunApp app = this;
            while (app.parentApp != null /*&& (app.parentOptions & CCCA.CCAF_SHARE_PLAYERCTRLS) != 0*/)
            {
                app = app.parentApp;
            }
            return app.pcCtrlKeys;
        }

        // Recherche les global values dans les parents
        public CArrayList getGlobalValues()
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

        public CArrayList getGlobalStrings()
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

        public CArrayList checkGlobalValue(int num)
        {
            CArrayList values = getGlobalValues();

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
            CArrayList values = checkGlobalValue(num);
            if (values != null)
            {
                return (CValue)values.get(num);
            }
            return tempGValue;
        }

        public void setGlobalValueAt(int num, CValue value)
        {
            CArrayList values = checkGlobalValue(num);
            if (values != null)
            {
                ((CValue)values.get(num)).forceValue(value);
            }
        }

        public CArrayList checkGlobalString(int num)
        {
            CArrayList strings = getGlobalStrings();

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
                    strings.add("");
                }
            }
            return strings;
        }

        public string getGlobalStringAt(int num)
        {
            CArrayList strings = checkGlobalString(num);
            if (strings != null)
            {
                return (string)strings.get(num);
            }
            return "";
        }

        public void setGlobalStringAt(int num, string value)
        {
            CArrayList strings = checkGlobalString(num);
            if (strings != null)
            {
                strings.set(num, String.Concat(value));
            }
        }

        // Charge le header de l'application
        public void loadAppHeader(CFile file)
        {
            file.skipBytes(4);			// Structure size
            gaFlags = file.readAShort();   		// Flags
            gaNewFlags = file.readAShort();		// New flags
            gaMode = file.readAShort();		// graphic mode
            gaOtherFlags = file.readAShort();		// Other Flags
            gaCxWin = file.readAShort();		// Window x-size
            gaCyWin = file.readAShort();		// Window y-size
            gaScoreInit = file.readAInt();		// Initial score
            gaLivesInit = file.readAInt();		// Initial number of lives
            int n, m;
            for (n = 0; n < MAX_PLAYER; n++)
            {
                short type = file.readAShort();
                if (type==0)
                    type=5;
                if (type<5)
                {
                    type=(short)((1<<(type-1))|0x80);
                }
                pcCtrlType[n] = type;	// Control type per player (0=mouse,1=joy1, 2=joy2, 3=keyb)
            }
            for (n = 0; n < MAX_PLAYER; n++)
            {
                for (m = 0; m < MAX_KEY; m++)
                {
                    pcCtrlKeys[n * MAX_KEY + m] = CKeyConvert.getXnaKey(file.readAShort());     // CKeyConvert.getJavaKey(file.readAShort());
                }
            }
#if XBOX
            for (n = 0; n < 4; n++)
            {
                pcCtrlType[n] = (short)(1<<n);
            }
#endif
            gaBorderColour = file.readAColor();	// Border colour
            gaNbFrames = file.readAInt();		// Number of frames
            gaFrameRate = file.readAInt();		// Number of frames per second
            file.skipBytes(1);	// Index of Window menu for MDI applications
            file.skipBytes(3);
        }

        // Charge le chunk GlobalValues
        public void loadGlobalValues(CFile file)
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
        public void loadGlobalStrings(CFile file)
        {
            nGlobalStringsInit = (short) file.readAInt();
            globalStringsInit = new string[nGlobalStringsInit];
            int n;
            for (n = 0; n < nGlobalStringsInit; n++)
            {
                globalStringsInit[n] = file.readAString();
            }
        }

        // Charge le chunk Frame handles
        public void loadFrameHandles(CFile file, int size)
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

        // Cache / montre la souris
        public void showCursor(bool bShown)
        {
            game.IsMouseVisible = bShown;
        }
        public int newGetCptVbl()
        {
            return VBL;
        }
        public void newResetCptVbl()
        {
            VBL = 0;
        }
        public void setFrameRate(int fps)
        {
            gaFrameRate = fps;
            double delta = 1000.0 / fps;
            TimeSpan ts = TimeSpan.FromMilliseconds(delta);
            game.TargetElapsedTime = ts;
        }
        public void setFullScreen(bool flag)
        {
            try
            {
                if (flag)
                {
                    if (graphicsDeviceManager.IsFullScreen == false)
                    {
                        graphicsDeviceManager.ToggleFullScreen();
                        graphicsDeviceManager.ApplyChanges();
                    }
                }
                else
                {
                    if (graphicsDeviceManager.IsFullScreen == true)
                    {
                        graphicsDeviceManager.ToggleFullScreen();
                        graphicsDeviceManager.ApplyChanges();
                    }
                }
            }
            catch (NoSuitableGraphicsDeviceException e)
            {
                e.GetType();
            }
            catch (InvalidOperationException e)
            {
                e.GetType();
            }
            catch (ArgumentException e)
            {
                e.GetType();
            }
        }
        void setLevelTitle()
        {
#if WINDOWS
            String temp;
            temp = appName ;
            if ((frame.leFlags & CRunFrame.LEF_DISPLAYNAME) != 0 && frame.frameName != null && frame.frameName.Length != 0)
            {
                temp += " - " + frame.frameName;
            }
            game.Window.Title = temp;
#endif
        }

#if WINDOWS_PHONE
        // Screen orientation
        void startOrientationHandler()
        {
            orientation = game.Window.CurrentOrientation;
            game.Window.OrientationChanged += new EventHandler<EventArgs>(orientationChanged);
        }
        private void orientationChanged(object sender, EventArgs e)
        {
            orientation = game.Window.CurrentOrientation;
        }

        // Joystick accelerometre
        public void startAccelerometer()
        {
            nAccel++;
            if (nAccel==1)
            {
                if (accel == null)
                {
                    accel = new Microsoft.Devices.Sensors.Accelerometer();
                    accel.ReadingChanged += new EventHandler<Microsoft.Devices.Sensors.AccelerometerReadingEventArgs>(accelerometerChanged);
                }
                accel.Start();
            }
        }
        public void stopAccelerometer()
        {
            nAccel--;
            if (nAccel == 0)
            {
                accel.Stop();
            }
        }

        static void accelerometerChanged(object sender, Microsoft.Devices.Sensors.AccelerometerReadingEventArgs e)
        {
            if (orientation==DisplayOrientation.LandscapeLeft)
            {
                accX = -(float)e.Y;
                accY = -(float)e.X;
                accZ = (float)e.Z;
            }
            else if (orientation == DisplayOrientation.LandscapeRight)
            {
                accX = (float)e.Y;
                accY = (float)e.X;
                accZ = (float)e.Z;
            }
            else
            {
                accX = (float)e.X;
                accY = -(float)e.Y;
                accZ = (float)e.Z;
            }
            filteredAccX = (float)((accX * kFilteringFactor) + (filteredAccX * (1.0 - kFilteringFactor)));
            filteredAccY = (float)((accY * kFilteringFactor) + (filteredAccY * (1.0 - kFilteringFactor)));
            filteredAccZ = (float)((accZ * kFilteringFactor) + (filteredAccZ * (1.0 - kFilteringFactor)));
            instantAccX = (float)(accX - ((accX * kFilteringFactor) + (instantAccX * (1.0 - kFilteringFactor))));
            instantAccY = (float)(accX - ((accY * kFilteringFactor) + (instantAccY * (1.0 - kFilteringFactor))));
            instantAccZ = (float)(accZ - ((accZ * kFilteringFactor) + (instantAccZ * (1.0 - kFilteringFactor))));
        }
#endif
        // ---------------------------------------------------------------------------------
        // EMBEDDED FILES
        // ---------------------------------------------------------------------------------
        public CEmbeddedFile getEmbeddedFile(string path)
        {
            int n;
            if (embeddedFiles != null)
            {
                for (n = 0; n < embeddedFiles.Length; n++)
                {
                    if (string.Compare(embeddedFiles[n].path, path) == 0)
                    {
                        return embeddedFiles[n];
                    }
                }
            }
            return null;
        }


    }
}
