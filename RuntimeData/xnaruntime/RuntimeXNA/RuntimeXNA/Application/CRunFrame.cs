//----------------------------------------------------------------------------------
//
// CRUNFRAME : contenu d'une frame
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Events;
using RuntimeXNA.Frame;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Application
{
    public class CRunFrame
    {
        // Flags
        public const int LEF_DISPLAYNAME = 0x0001;
        public const int LEF_GRABDESKTOP = 0x0002;
        public const int LEF_KEEPDISPLAY = 0x0004;
        public const int LEF_TOTALCOLMASK = 0x0020;
    //    public const int LEF_PASSWORD=0x0040;
        public const int LEF_RESIZEATSTART = 0x0100;
    //    public const int LEF_DONOTCENTER=0x0200;
    //    public const int LEF_FORCE_LOADONCALL=0x0400;
        public const int LEF_NOSURFACE = 0x0800;
    //    public const int LEF_RESERVED_1=0x1000;
    //    public const int LEF_RESERVED_2=0x2000;
        public const int LEF_TIMEDMVTS = 0x8000;
        public const int IPHONEOPT_JOYSTICK_FIRE1 = 0x0001;
        public const int IPHONEOPT_JOYSTICK_FIRE2 = 0x0002;
        public const int IPHONEOPT_JOYSTICK_LEFTHAND = 0x0004;
        public const int IPHONEOPT_MULTITOUCH = 0x0008;
        public const int IPHONEOPT_SCREENLOCKING = 0x0010;
        public const int IPHONEOPT_IPHONEFRAMEIAD = 0x0020;
        public const int JOYSTICK_NONE = 0x0000;
        public const int JOYSTICK_TOUCH = 0x0001;
        public const int JOYSTICK_ACCELEROMETER = 0x0002;
        public const int JOYSTICK_EXT = 0x0003;
        
        // Frameheader
        public int leWidth;			// Playfield width in pixels
        public int leHeight;		// Playfield height in pixels
        public int leBackground;
        public int leFlags;
        public CRect leVirtualRect;
        public int leEditWinWidth;
        public int leEditWinHeight;
        public string frameName = null;
        public int nLayers;
        public CLayer[] layers;
        public CLOList LOList;
        public CEventProgram evtProg;
        public short maxObjects=512;
        // Coordinates of top-level pixel in edit window
        public int leX=0;
        public int leY=0;
        public int leLastScrlX=0;
        public int leLastScrlY=0;

        // Transitions
        public CRect fadeIn = null;
        public CRect fadeOut = null;
//        public CTrans pTrans = null;
        // Exit code
        public int levelQuit=0;

        // Events
        public bool rhOK=false;				// TRUE when the events are initialized

        // public int nPlayers;
        //	int				m_nPlayersReal;
        //	int				m_level_loop_state;
        public int startLeX=0;
        public int startLeY=0;
        //	short			m_maxObjects;
        //	short			m_maxOI;
        //	LPOBL			m_oblEnum;
        //	int				m_oblEnumCpt;
        //	BOOL			m_eventsBranched;
        public bool fade = false;
        public int fadeTimerDelta=0;
        public int fadeVblDelta=0;
        //	DWORD			m_pasteMask;

        //	int				m_nCurTempString;
        //	LPSTR			m_pTempString[MAX_TEMPSTRING];
        public int dwColMaskBits=0;
        public CColMask colMask = null;
        public short m_wRandomSeed=0;
        public int m_dwMvtTimerBase=0;
        public CRunApp app=null;
        public CRun rhPtr=null;
        public short joystick;
        public short iPhoneOptions;
        public short[] mosaicHandles= null;
        public int[] mosaicX= null;
        public int[] mosaicY= null;
        public int mosaicMaxHandle=0;

        public CRunFrame()
        {
        }

        public CRunFrame(CRunApp pApp)
        {
            app = pApp;
        }

        // Charge la frame
        public bool loadFullFrame(int index)
        {
            // Positionne le fichier
            app.file.seek(app.frameOffsets[index]);

            // Charge la frame
            evtProg = new CEventProgram();
            LOList = new CLOList();
            leVirtualRect = new CRect();

            CChunk chk = new CChunk();
            int posEnd;
            int nOldFrameWidth = 0;
            int nOldFrameHeight = 0;
            m_wRandomSeed = -1;
            int n;
            while (chk.chID != 0x7F7F)	// CHUNK_LAST
            {
                chk.readHeader(app.file);
                if (chk.chSize == 0)
                {
                    continue;
                }
                posEnd = app.file.getFilePointer() + chk.chSize;
                switch (chk.chID)
                {
                    // CHUNK_FRAMEHEADER
                    case 0x3334:
                        loadHeader();
/*
                        if ((leFlags & LEF_RESIZEATSTART) != 0)
                        {
                            nOldFrameWidth = leWidth;
                            nOldFrameHeight = leHeight;

                            screen = Toolkit.getDefaultToolkit().getScreenSize();
                            leWidth = screen.width;
                            leHeight = screen.height;

                            // To keep compatibility with previous versions without virtual rectangle (nécessaire ?)
                            leVirtualRect.left = leVirtualRect.top = 0;
                            leVirtualRect.right = leWidth;
                            leVirtualRect.bottom = leHeight;
                        }
*/
                        // B243
/*                        if (app.parentApp != null && (app.parentOptions & CCCA.CCAF_DOCKED) != 0)
                        {
                            leEditWinWidth = app.cx;
                            leEditWinHeight = app.cy;
                        }
                        else if ((leFlags & LEF_RESIZEATSTART) != 0)
                        {
                            screen = Toolkit.getDefaultToolkit().getScreenSize();
                            leEditWinWidth = screen.width;
                            leEditWinHeight = screen.height;
                        }
                        else
 */ 
                        {
                            leEditWinWidth = Math.Min(app.gaCxWin, leWidth);
                            leEditWinHeight = Math.Min(app.gaCyWin, leHeight);
                        }
                        break;

                    // CHUNK_FRAMEVIRTUALRECT
                    case 0x3342:
                        leVirtualRect.load(app.file);
                        if ((leFlags & LEF_RESIZEATSTART) != 0)
                        {
                            if (leVirtualRect.right - leVirtualRect.left == nOldFrameWidth || leVirtualRect.right - leVirtualRect.left < leWidth)
                            {
                                leVirtualRect.right = leVirtualRect.left + leWidth;
                            }
                            if (leVirtualRect.bottom - leVirtualRect.top == nOldFrameHeight || leVirtualRect.bottom - leVirtualRect.top < leHeight)
                            {
                                leVirtualRect.bottom = leVirtualRect.top + leHeight;
                            }
                        }
                        break;

                    // CHUNK_RANDOMSEED
                    case 0x3344:
                        m_wRandomSeed = app.file.readAShort();
                        break;

                    // CHUNK_MVTTIMERBASE
                    case 0x3347:
                        m_dwMvtTimerBase = app.file.readAInt();
                        break;

                    // CHUNK_FRAMENAME
                    case 0x3335:
                        frameName = app.file.readAString();
                        break;

                    // CHUNK_FRAMEPALETTE
                    case 0x3337:
                        break;

                    // CHUNK_FRAMELAYERS
                    case 0x3341:
                        loadLayers();
                        break;

                    // CHUNK_FRAMEITEMINSTANCES
                    case 0x3338:
                        LOList.load(app);
                        break;

                    // CHUNK_ADDITIONAL_FRAMEITEMINSTANCE
                    case 0x3340:
                        break;

                    // CHUNK_IPHONE_OPTIONS
                    case 0x334A:
                        joystick=app.file.readAShort();
                        iPhoneOptions = app.file.readAShort();
                        break;

                    // CHUNK_FRAMEFADEIN
                    case 0x333B:
//                        fadeIn = new CTransitionData();
//                        fadeIn.load(app.file);
                        break;

                    // CHUNK_FRAMEFADEOUT
                    case 0x333C:
//                        fadeOut = new CTransitionData();
//                        fadeOut.load(app.file);
                        break;

                    // CHUNK_FRAMEEVENTS
                    case 0x333D:
                        evtProg.load(app);
                        maxObjects = evtProg.maxObjects;
//                        evtProg.relocatePath(app);
                        break;

                    // CHUNK_MOSAICIMAGETABLE
                    case 0x3348:
                        int number = chk.chSize / (3 * 2);
                        mosaicHandles = new short[number];
                        mosaicX = new int[number];
                        mosaicY = new int[number];
                        mosaicMaxHandle = 0;
                        for (n = 0; n < number; n++)
                        {
                            mosaicHandles[n] = app.file.readAShort();
                            mosaicMaxHandle = Math.Max(mosaicMaxHandle, mosaicHandles[n]);
                            mosaicX[n] = app.file.readAShort();
                            mosaicY[n] = app.file.readAShort();
                        }
                        mosaicMaxHandle++;
                        break;

                }
                // Positionne a la fin du chunk
                app.file.seek(posEnd);
            }

            // Marque les OI a charger
            app.OIList.resetToLoad();
            for (n = 0; n < LOList.nIndex; n++)
            {
                CLO lo = LOList.getLOFromIndex((short) n);
                app.OIList.setToLoad(lo.loOiHandle);
            }

            // Charge les OI et les elements des banques
            app.imageBank.resetToLoad();
            app.fontBank.resetToLoad();
            app.OIList.load(app.file, app);
            app.OIList.enumElements((IEnum) app.imageBank, (IEnum)app.fontBank);
            app.imageBank.load();
            app.fontBank.load();
            evtProg.enumSounds((IEnum) app.soundBank);
            app.soundBank.load();

            // Marque les OI de la frame
            app.OIList.resetOICurrent();
            for (n = 0; n < LOList.nIndex; n++)
            {
                CLO lo = LOList.list[n];
                if (lo.loType >= COI.OBJ_SPR)
                {
                    app.OIList.setOICurrent(lo.loOiHandle);
                }
            }
            return true;
        }

        public void loadLayers()
        {
            nLayers = app.file.readAInt();
            layers = new CLayer[nLayers];

            int n;
            for (n = 0; n < nLayers; n++)
            {
                layers[n] = new CLayer();
                layers[n].load(app.file);
            }
        }

        public void loadHeader()
        {
            leWidth = app.file.readAInt();
            leHeight = app.file.readAInt();
            leBackground = app.file.readAColor();
            leFlags = app.file.readAInt();
        }

        // Get obstacle mask bits
        public int getMaskBits()
        {
            int flgs = 0;

            int n;
            for (n = 0; n < LOList.nIndex; n++)
            {
                CLO lo = LOList.getLOFromIndex((short) n);
                if (lo.loLayer > 0)
                {
                    break;
                }

                COI poi = app.OIList.getOIFromHandle(lo.loOiHandle);
                if (poi.oiType < COI.OBJ_SPR)
                {
                    COC poc = poi.oiOC;
                    switch (poc.ocObstacleType)
                    {
                        case 1:	    // COC.OBSTACLE_SOLID:
                            flgs |= CColMask.CM_OBSTACLE;
                            break;
                        case 2:	    // COC.OBSTACLE_PLATFORM:
                            flgs |= CColMask.CM_PLATFORM;
                            break;
                    }
                }
                else
                {
                    CObjectCommon pCommon = (CObjectCommon) poi.oiOC;
                    if ((pCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) != 0)
                    {
                        switch ((pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT)
                        {
                            case 1:	    // OBSTACLE_SOLID:
                                flgs |= CColMask.CM_OBSTACLE;
                                break;
                            case 2:	    // OBSTACLE_PLATFORM:
                                flgs |= CColMask.CM_PLATFORM;
                                break;
                        }
                    }
                }
            }
            return flgs;
        }

        //////////////////////////////////////////////////////////////////////////////
        //
        // Background collision routines
        //
        public bool bkdLevObjCol_TestPoint(int x, int y, int nTestLayer, int nPlane)
        {
            int nLayer;
            int nFirstLayer;
            int nLastLayer;
            int v;
            int cm_box;
            CRect rc = new CRect();
            CImage image;
            CMask pMask;

            if (nTestLayer == CSpriteGen.LAYER_ALL)
            {
                nFirstLayer = 1;				// Layer 0 already tested by caller
                nLastLayer = nLayers - 1;
            }
            else
            {
                if (nTestLayer >= nLayers)
                {
                    return false;
                }
                nFirstLayer = nLastLayer = nTestLayer;
            }

            int nPlayfieldWidth = leWidth;
            int nPlayfieldHeight = leHeight;

            for (nLayer = nFirstLayer; nLayer <= nLastLayer; nLayer++)
            {
                CLayer pLayer = layers[nLayer];

                bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
                bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
                bool bWrap = (bWrapHorz | bWrapVert);
                int i;

                // Layer offset
                int dxLayer = leX;
                int dyLayer = leY;
                if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
                {
                    if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
                    {
                        dxLayer = (int)((float)dxLayer * pLayer.xCoef);
                    }
                    if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
                    {
                        dyLayer = (int)((float)dyLayer * pLayer.yCoef);
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

                int nLOs = pLayer.nBkdLOs;
                for (i = 0; i < nLOs; i++)
                {
                    CLO plo = LOList.getLOFromIndex((short)(pLayer.nFirstLOIndex + i));
                    CObject hoPtr = null;

                    COI poi = app.OIList.getOIFromHandle(plo.loOiHandle);
                    if (poi == null || poi.oiOC == null)
                    {
                        continue;
                    }

                    COC poc = poi.oiOC;
                    int typeObj = poi.oiType;

                    // Get object position
                    rc.left = plo.loX - dxLayer;
                    rc.top = plo.loY - dyLayer;

                    // Get object rectangle
                    if (typeObj < COI.OBJ_SPR)
                    {
                        v = poc.ocObstacleType;
                        // Ladder or no obstacle? continue
                        if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                        {
                            continue;
                        }
                        cm_box = poc.ocColMode;
                        rc.right = rc.left + poc.ocCx;
                        rc.bottom = rc.top + poc.ocCy;
                    }
                    else
                    {
                        CObjectCommon pCommon = (CObjectCommon)poc;
                        // Dynamic item => must be a background object
                        if ((pCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0 || (hoPtr = rhPtr.find_HeaderObject(plo.loHandle)) == null)
                        {
                            continue;
                        }
                        v = ((pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT);
                        // Ladder or no obstacle? continue
                        if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                        {
                            continue;
                        }
                        cm_box = ((pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_COLBOX) != 0) ? 1 : 0;
                        rc.left = hoPtr.hoX - leX - hoPtr.hoImgXSpot;
                        rc.top = hoPtr.hoY - leY - hoPtr.hoImgYSpot;
                        rc.right = rc.left + hoPtr.hoImgWidth;
                        rc.bottom = rc.top + hoPtr.hoImgHeight;
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
                                        dwWrapFlags |= (CRun.WRAP_X | CRun.WRAP_Y | CRun.WRAP_XY);
                                    }

                                    // Wrap horizontally only
                                    else
                                    {
                                        nSprite = 1;
                                        dwWrapFlags |= (CRun.WRAP_X);
                                    }
                                }
                                // Wrap vertically?
                                else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
                                {
                                    nSprite = 2;
                                    dwWrapFlags |= (CRun.WRAP_Y);
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
                                dwWrapFlags &= ~CRun.WRAP_X;

                                // Calculate next sprite to display
                                nSprite = 0;
                                if ((dwWrapFlags & CRun.WRAP_Y) != 0)
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
                                dwWrapFlags &= ~CRun.WRAP_Y;

                                // Calculate next sprite to display
                                nSprite = 0;
                                if ((dwWrapFlags & CRun.WRAP_X) != 0)
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
                                dwWrapFlags &= ~CRun.WRAP_XY;

                                // Calculate next sprite to display
                                nSprite = 2;
                                break;
                        }
                    }

                    do
                    {
                        if (x < rc.left || y < rc.top)
                        {
                            break;
                        }

                        // Point in rectangle?
                        if (x >= rc.right || y >= rc.bottom)
                        {
                            break;
                        }

                        // Obstacle and ask for platform or reciprocally? continue
                        if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est écrit dans les 2 masques...
                                (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                        {
                            break;
                        }

                        // Collision with box
                        if (cm_box != 0)
                        {
                            return true;		// collides
                        }
                        // Load image if not yet loaded
                        //FRANCOIS:	    if ( (poi.oiLoadFlags & OILF_ELTLOADED) == 0 )
                        //			    LoadOnCall(poi);

                        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                        if (v == COC.OBSTACLE_PLATFORM)
                        {
                            nGetColMaskFlag = CMask.GCMF_PLATFORM;
                        }

                        // Test if point into image mask
                        pMask = null;
                        if (typeObj < COI.OBJ_SPR)
                        {
                            image = app.imageBank.getImageFromHandle(((COCBackground)poc).ocImage);
                            pMask = image.getMask(nGetColMaskFlag, 0, (float)1.0, (float)1.0);
                        }
                        else
                        {
                            pMask = hoPtr.getCollisionMask(nGetColMaskFlag);
                        }
                        if (pMask == null)		// No mask? collision
                        {
                            return true;
                        }

                        if (pMask.testPoint(x - rc.left, y - rc.top))
                        {
                            return true;
                        }
                    } while (false);

                    // Wrapped?
                    if (dwWrapFlags != 0)
                    {
                        i--;
                    }
                }

                // Scan Bkd2s
                if (pLayer.pBkd2 != null)
                {
                    CBkd2 pbkd;

                    dwWrapFlags = 0;
                    nSprite = 0;

                    for (i = 0; i < pLayer.pBkd2.size(); i++)
                    {
                        pbkd = (CBkd2)pLayer.pBkd2.get(i);

                        // Get object position
                        rc.left = pbkd.x - dxLayer;
                        rc.top = pbkd.y - dyLayer;

                        v = pbkd.obstacleType;
                        if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                        {
                            continue;
                        }
                        cm_box = (pbkd.colMode == CSpriteGen.CM_BOX) ? 1 : 0;

                        // Get object rectangle
                        image = app.imageBank.getImageFromHandle(pbkd.img);
                        if (image != null)
                        {
                            rc.right = rc.left + image.width;
                            rc.bottom = rc.top + image.height;
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
                                            dwWrapFlags |= (CRun.WRAP_X | CRun.WRAP_Y | CRun.WRAP_XY);
                                        }

                                        // Wrap horizontally only
                                        else
                                        {
                                            nSprite = 1;
                                            dwWrapFlags |= (CRun.WRAP_X);
                                        }
                                    }
                                    // Wrap vertically?
                                    else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
                                    {
                                        nSprite = 2;
                                        dwWrapFlags |= (CRun.WRAP_Y);
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
                                    dwWrapFlags &= ~CRun.WRAP_X;

                                    // Calculate next sprite to display
                                    nSprite = 0;
                                    if ((dwWrapFlags & CRun.WRAP_Y) != 0)
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
                                    dwWrapFlags &= ~CRun.WRAP_Y;

                                    // Calculate next sprite to display
                                    nSprite = 0;
                                    if ((dwWrapFlags & CRun.WRAP_X) != 0)
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
                                    dwWrapFlags &= ~CRun.WRAP_XY;

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

                            // Obstacle and ask for platform or reciprocally? continue
                            if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est écrit dans les 2 masques...
                                    (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                            {
                                break;
                            }

                            // Collision with box
                            if (cm_box != 0)
                            {
                                return true;		// collides
                            }
                            int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                            if (v == COC.OBSTACLE_PLATFORM)
                            {
                                nGetColMaskFlag = CMask.GCMF_PLATFORM;
                            }

                            // Test if point into image mask
                            image = app.imageBank.getImageFromHandle(pbkd.img);
                            pMask = image.getMask(nGetColMaskFlag, 0, (float)1.0, (float)1.0);
                            if (pMask != null)
                            {
                                if (pMask.testPoint(x - rc.left, y - rc.top))
                                {
                                    return true;
                                }
                            }
                        } while (false);

                        // Wrapped?
                        if (dwWrapFlags != 0)
                        {
                            i--;
                        }
                    }
                }
            }
            return false;
        }

        public bool bkdLevObjCol_TestRect(int x, int y, int nWidth, int nHeight, int nTestLayer, int nPlane)
        {
            int nLayer;
            int nFirstLayer;
            int nLastLayer;
            int v;
            int cm_box;
            CRect rc = new CRect();
            CImage image;
            CMask pMask = null;

            if (nTestLayer == CSpriteGen.LAYER_ALL)
            {
                nFirstLayer = 1;				// Layer 0 already tested by caller
                nLastLayer = nLayers - 1;
            }
            else
            {
                if (nTestLayer >= nLayers)
                {
                    return false;
                }
                nFirstLayer = nLastLayer = nTestLayer;
            }

            int nPlayfieldWidth = leWidth;
            int nPlayfieldHeight = leHeight;

            for (nLayer = nFirstLayer; nLayer <= nLastLayer; nLayer++)
            {
                CLayer pLayer = layers[nLayer];

                bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
                bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
                bool bWrap = (bWrapHorz | bWrapVert);
                int i;

                // Layer offset
                int dxLayer = leX;
                int dyLayer = leY;
                if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
                {
                    if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
                    {
                        dxLayer = (int)((float)dxLayer * pLayer.xCoef);
                    }
                    if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
                    {
                        dyLayer = (int)((float)dyLayer * pLayer.yCoef);
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

                int nLOs = pLayer.nBkdLOs;
                for (i = 0; i < nLOs; i++)
                {
                    CLO plo = LOList.getLOFromIndex((short)(pLayer.nFirstLOIndex + i));
                    CObject hoPtr = null;

                    COI poi = app.OIList.getOIFromHandle(plo.loOiHandle);
                    if (poi == null || poi.oiOC == null)
                    {
                        continue;
                    }

                    COC poc = poi.oiOC;
                    int typeObj = poi.oiType;

                    // Get object position
                    rc.left = plo.loX - dxLayer;
                    rc.top = plo.loY - dyLayer;

                    // Get object rectangle
                    if (typeObj < COI.OBJ_SPR)
                    {
                        v = poc.ocObstacleType;
                        // Ladder or no obstacle? continue
                        if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                        {
                            continue;
                        }
                        cm_box = poc.ocColMode;
                        rc.right = rc.left + poc.ocCx;
                        rc.bottom = rc.top + poc.ocCy;
                    }
                    else
                    {
                        CObjectCommon pCommon = (CObjectCommon)poc;
                        // Dynamic item => must be a background object
                        if ((pCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0 || (hoPtr = rhPtr.find_HeaderObject(plo.loHandle)) == null)
                        {
                            continue;
                        }
                        v = ((pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT);
                        // Ladder or no obstacle? continue
                        if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                        {
                            continue;
                        }
                        cm_box = ((pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_COLBOX) != 0) ? 1 : 0;
                        rc.left = hoPtr.hoX - leX - hoPtr.hoImgXSpot;
                        rc.top = hoPtr.hoY - leY - hoPtr.hoImgYSpot;
                        rc.right = rc.left + hoPtr.hoImgWidth;
                        rc.bottom = rc.top + hoPtr.hoImgHeight;
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
                                        dwWrapFlags |= (CRun.WRAP_X | CRun.WRAP_Y | CRun.WRAP_XY);
                                    }

                                    // Wrap horizontally only
                                    else
                                    {
                                        nSprite = 1;
                                        dwWrapFlags |= (CRun.WRAP_X);
                                    }
                                }
                                // Wrap vertically?
                                else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
                                {
                                    nSprite = 2;
                                    dwWrapFlags |= (CRun.WRAP_Y);
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
                                dwWrapFlags &= ~CRun.WRAP_X;

                                // Calculate next sprite to display
                                nSprite = 0;
                                if ((dwWrapFlags & CRun.WRAP_Y) != 0)
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
                                dwWrapFlags &= ~CRun.WRAP_Y;

                                // Calculate next sprite to display
                                nSprite = 0;
                                if ((dwWrapFlags & CRun.WRAP_X) != 0)
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
                                dwWrapFlags &= ~CRun.WRAP_XY;

                                // Calculate next sprite to display
                                nSprite = 2;
                                break;
                        }
                    }
                    do
                    {
                        if (x + nWidth <= rc.left || y + nHeight <= rc.top)
                        {
                            break;
                        }

                        // Point in rectangle?
                        if (x >= rc.right || y >= rc.bottom)
                        {
                            break;
                        }

                        // Obstacle and ask for platform or reciprocally? continue
                        if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est écrit dans les 2 masques...
                                (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                        {
                            break;
                        }

                        // Collision with box
                        if (cm_box != 0)
                        {
                            return true;		// collides
                        }
                        // Load image if not yet loaded
                        //FRANCOIS:	    if ( (poi.oiLoadFlags & OILF_ELTLOADED) == 0 )
                        //			LoadOnCall(poi);

                        // Get background mask
                        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                        if (v == COC.OBSTACLE_PLATFORM)
                        {
                            nGetColMaskFlag = CMask.GCMF_PLATFORM;
                        }

                        if (typeObj < COI.OBJ_SPR)
                        {
                            image = app.imageBank.getImageFromHandle(((COCBackground)poc).ocImage);
                            pMask = image.getMask(nGetColMaskFlag, 0, (float)1.0, (float)1.0);
                        }
                        else
                        {
                            pMask = hoPtr.getCollisionMask(nGetColMaskFlag);
                        }
                        if (pMask == null)		// No mask? collision
                        {
                            return true;
                        }

                        // Test if rectangle intersects with background mask
                        if (pMask.testRect(0, x - rc.left, y - rc.top, nWidth, nHeight))
                        {
                            return true;
                        }

                    } while (false);

                    // Wrapped?
                    if (dwWrapFlags != 0)
                    {
                        i--;
                    }
                }

                // Scan Bkd2s
                if (pLayer.pBkd2 != null)
                {
                    CBkd2 pbkd;

                    dwWrapFlags = 0;
                    nSprite = 0;

                    for (i = 0; i < pLayer.pBkd2.size(); i++)
                    {
                        pbkd = (CBkd2)pLayer.pBkd2.get(i);

                        rc.left = pbkd.x - dxLayer;
                        rc.top = pbkd.y - dyLayer;

                        v = pbkd.obstacleType;
                        if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                        {
                            continue;
                        }
                        cm_box = (pbkd.colMode == CSpriteGen.CM_BOX) ? 1 : 0;

                        // Get object rectangle
                        image = app.imageBank.getImageFromHandle(pbkd.img);
                        if (image != null)
                        {
                            rc.right = rc.left + image.width;
                            rc.bottom = rc.top + image.height;
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
                                            dwWrapFlags |= (CRun.WRAP_X | CRun.WRAP_Y | CRun.WRAP_XY);
                                        }

                                        // Wrap horizontally only
                                        else
                                        {
                                            nSprite = 1;
                                            dwWrapFlags |= (CRun.WRAP_X);
                                        }
                                    }

                                    // Wrap vertically?
                                    else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
                                    {
                                        nSprite = 2;
                                        dwWrapFlags |= (CRun.WRAP_Y);
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
                                    dwWrapFlags &= ~CRun.WRAP_X;

                                    // Calculate next sprite to display
                                    nSprite = 0;
                                    if ((dwWrapFlags & CRun.WRAP_Y) != 0)
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
                                    dwWrapFlags &= ~CRun.WRAP_Y;

                                    // Calculate next sprite to display
                                    nSprite = 0;
                                    if ((dwWrapFlags & CRun.WRAP_X) != 0)
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
                                    dwWrapFlags &= ~CRun.WRAP_XY;

                                    // Calculate next sprite to display
                                    nSprite = 2;
                                    break;
                            }
                        }

                        do
                        {
                            // Intersection?
                            if (x + nWidth <= rc.left || y + nHeight <= rc.top)
                            {
                                break;
                            }

                            if (x >= rc.right || y >= rc.bottom)
                            {
                                break;
                            }

                            // Obstacle and ask for platform or reciprocally? continue
                            if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est écrit dans les 2 masques...
                                    (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                            {
                                break;
                            }

                            // Collision with box
                            if (cm_box != 0)
                            {
                                return true;		// collides
                            }
                            int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                            if (v == COC.OBSTACLE_PLATFORM)
                            {
                                nGetColMaskFlag = CMask.GCMF_PLATFORM;
                            }

                            // Test if point into image mask
                            image = app.imageBank.getImageFromHandle(pbkd.img);
                            pMask = image.getMask(nGetColMaskFlag, 0, (float)1.0, (float)1.0);
                            if (pMask != null)
                            {
                                if (pMask.testRect(0, x - rc.left, y - rc.top, nWidth, nHeight))
                                {
                                    return true;
                                }
                            }
                        } while (false);

                        // Wrapped?
                        if (dwWrapFlags != 0)
                        {
                            i--;
                        }
                    }
                }
            }
            return false;
        }

        public bool bkdLevObjCol_TestSprite(CSprite pSpr, short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
        {
            CObject hoPtr;
            int v;
            int cm_box;
            CRect rc = new CRect();

            // Get sprite layer
            int nLayer = pSpr.sprLayer / 2;	    // GetSpriteLayer

            CLayer pLayer = layers[nLayer];

            bool bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
            bool bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
            bool bWrap = (bWrapHorz | bWrapVert);
            int i;

            int nPlayfieldWidth = leWidth;
            int nPlayfieldHeight = leHeight;
            CImage image;

            // Layer offset
            int dxLayer = leX;
            int dyLayer = leY;
            if ((pLayer.dwOptions & (CLayer.FLOPT_XCOEF | CLayer.FLOPT_YCOEF)) != 0)
            {
                if ((pLayer.dwOptions & CLayer.FLOPT_XCOEF) != 0)
                {
                    dxLayer = (int)((float)dxLayer * pLayer.xCoef);
                }
                if ((pLayer.dwOptions & CLayer.FLOPT_YCOEF) != 0)
                {
                    dyLayer = (int)((float)dyLayer * pLayer.yCoef);
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

            // Sprite collision mode
            uint dwSprFlags = pSpr.sprFlags;
            bool bSprColBox = ((dwSprFlags & CSprite.SF_COLBOX) != 0);

            // Sprite rectangle
            CRect sprRc = new CRect();
            int nWidth=0, nHeight=0;
            int nImg = newImg;

            sprRc.left = newX;
            sprRc.top = newY;
            if (newImg == 0)
            {
                nImg = pSpr.sprImg;
            }

            CMask pSprMask = null;
            CMask pBkdMask = null;
            int yMaskBits = 0;

            // Bitmap collision?
            if (!bSprColBox)
            {
                // Image sprite not stretched and not rotated, or owner draw sprite?
//                if ((dwSprFlags & CSprite.SF_OWNERDRAW) != 0 || (newAngle == 0 && newScaleX == 1.0f && newScaleY == 1.0f))
                {
                    pSprMask = app.spriteGen.getSpriteMask(pSpr, (short)nImg, CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
                    if (pSprMask == null)
                    {
                        sprRc.left = pSpr.sprX1new;		// GetSpriteRect
                        sprRc.right = pSpr.sprX2new;
                        sprRc.top = pSpr.sprY1new;
                        sprRc.bottom = pSpr.sprY2new;
                        nWidth = sprRc.right - sprRc.left;
                        nHeight = sprRc.bottom - sprRc.top;
                        bSprColBox = true;			// no mask ? box collision
                    }
                    else
                    {
                        // Get sprite box
                        if ((pSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
                        {
                            sprRc.left -= pSprMask.xSpot;
                            sprRc.top -= pSprMask.ySpot;
                        }
                        nWidth = pSprMask.width;
                        nHeight = pSprMask.height;
                        sprRc.right = sprRc.left + nWidth;
                        sprRc.bottom = sprRc.top + nHeight;
                    }
                }
            }
            else
            {
                // Box collision: no need to calculate the mask
                if (nImg == 0 || nImg == pSpr.sprImg || (dwSprFlags & CSprite.SF_OWNERDRAW) != 0)
                {
                    sprRc.left = pSpr.sprX1new;		// GetSpriteRect
                    sprRc.right = pSpr.sprX2new;
                    sprRc.top = pSpr.sprY1new;
                    sprRc.bottom = pSpr.sprY2new;
                    nWidth = sprRc.right - sprRc.left;
                    nHeight = sprRc.bottom - sprRc.top;
                }
                else
                {
                    image = app.imageBank.getImageFromHandle((short)nImg);
                    if (image != null)
                    {
                        sprRc.left -= image.xSpot;
                        sprRc.top -= image.ySpot;
                        nWidth = image.width;
                        nHeight = image.height;
                        sprRc.right = sprRc.left + nWidth;
                        sprRc.bottom = sprRc.top + nHeight;
                    }
                    else
                    {
                        sprRc.left = pSpr.sprX1new;		// GetSpriteRect
                        sprRc.right = pSpr.sprX2new;
                        sprRc.top = pSpr.sprY1new;
                        sprRc.bottom = pSpr.sprY2new;
                        nWidth = sprRc.right - sprRc.left;
                        nHeight = sprRc.bottom - sprRc.top;
                    }
                }
            }

            // Take subHt into account
            if (subHt != 0)
            {
                if (subHt > nHeight)
                {
                    subHt = nHeight;
                }
                sprRc.top += nHeight - subHt;
                if (pSprMask != null)
                {
                    yMaskBits = nHeight - subHt;
                }
                nHeight = subHt;
            }

            // Scan LOs
            uint dwWrapFlags = 0;
            int nSprite = 0;

            int nLOs = pLayer.nBkdLOs;
            for (i = 0; i < nLOs; i++)
            {
                CLO plo = LOList.getLOFromIndex((short)(pLayer.nFirstLOIndex + i));

                COI poi = app.OIList.getOIFromHandle(plo.loOiHandle);
                if (poi == null || poi.oiOC == null)
                {
                    continue;
                }

                COC poc = poi.oiOC;
                int typeObj = poi.oiType;

                // Get object position
                rc.left = plo.loX - dxLayer;
                rc.top = plo.loY - dyLayer;

                // Get object rectangle
                hoPtr = null;
                if (typeObj < COI.OBJ_SPR)
                {
                    // Ladder or no obstacle? continue
                    v = poc.ocObstacleType;
                    if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                    {
                        continue;
                    }
                    cm_box = poc.ocColMode;
                    rc.right = rc.left + poc.ocCx;
                    rc.bottom = rc.top + poc.ocCy;
                }
                else
                {
                    // Dynamic item => must be a background object
                    CObjectCommon pCommon = (CObjectCommon)poc;
                    if ((pCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) == 0 || (hoPtr = rhPtr.find_HeaderObject(plo.loHandle)) == null)
                    {
                        continue;
                    }
                    v = ((pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT);
                    // Ladder or no obstacle? continue
                    if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                    {
                        continue;
                    }
                    cm_box = (pCommon.ocFlags2 & CObjectCommon.OCFLAGS2_COLBOX) != 0 ? 1 : 0;
                    rc.left = hoPtr.hoX - leX - hoPtr.hoImgXSpot;
                    rc.top = hoPtr.hoY - leY - hoPtr.hoImgYSpot;
                    rc.right = rc.left + hoPtr.hoImgWidth;
                    rc.bottom = rc.top + hoPtr.hoImgHeight;
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
                                    dwWrapFlags |= (CRun.WRAP_X | CRun.WRAP_Y | CRun.WRAP_XY);
                                }

                                // Wrap horizontally only
                                else
                                {
                                    nSprite = 1;
                                    dwWrapFlags |= (CRun.WRAP_X);
                                }
                            }
                            // Wrap vertically?
                            else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
                            {
                                nSprite = 2;
                                dwWrapFlags |= (CRun.WRAP_Y);
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
                            dwWrapFlags &= ~CRun.WRAP_X;

                            // Calculate next sprite to display
                            nSprite = 0;
                            if ((dwWrapFlags & CRun.WRAP_Y) != 0)
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
                            dwWrapFlags &= ~CRun.WRAP_Y;

                            // Calculate next sprite to display
                            nSprite = 0;
                            if ((dwWrapFlags & CRun.WRAP_X) != 0)
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
                            dwWrapFlags &= ~CRun.WRAP_XY;

                            // Calculate next sprite to display
                            nSprite = 2;
                            break;
                    }
                }

                do
                {
                    if (sprRc.right <= rc.left || sprRc.bottom <= rc.top)
                    {
                        break;
                    }

                    // No Intersection?
                    if (sprRc.left >= rc.right || sprRc.top >= rc.bottom)
                    {
                        break;
                    }

                    // Obstacle and ask for platform or reciprocally? continue
                    if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est écrit dans les 2 masques...
                            (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                    {
                        break;
                    }

                    // Background sprite = Box?
                    if (cm_box != 0)
                    {
                        // Collision between 2 boxes? OK
                        if (bSprColBox)
                        {
                            return true;
                        }

                        // Active sprite = bitmap
                        // => test collision between background rectangle and sprite's mask
                        if (pSprMask == null)
                        {
//                            pSprMask = rhPtr.spriteGen.completeSpriteColMask(pSpr, dwPSCFlags, nWidth, nHeight);
                            if (pSprMask == null)
                            {
                                return true;		// Can't calculate mask => box collision
                            }
                            yMaskBits = 0;
                            if (subHt != 0)
                            {
                                if (subHt > nHeight)
                                {
                                    subHt = nHeight;
                                }
                                yMaskBits = nHeight - subHt;
                            }
                        }

                        if (pSprMask.testRect(yMaskBits, rc.left - sprRc.left, rc.top - sprRc.top, rc.right - rc.left, rc.bottom - rc.top))
                        {
                            return true;
                        }
                    }
                    // Background sprite = bitmap
                    else
                    {
                        // Load image if not yet loaded
                        //FRANCOIS:	    if ( (poi.oiLoadFlags & OILF_ELTLOADED) == 0 )
                        //			LoadOnCall(poi);

                        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                        if (v == COC.OBSTACLE_PLATFORM)
                        {
                            nGetColMaskFlag = CMask.GCMF_PLATFORM;
                        }

                        // Get background mask
                        pBkdMask = null;
                        if (typeObj < COI.OBJ_SPR)
                        {
                            image = app.imageBank.getImageFromHandle(((COCBackground)poc).ocImage);
                            pBkdMask = image.getMask(nGetColMaskFlag, 0, (float)1.0, (float)1.0);
                        }
                        else
                        {
                            pBkdMask = hoPtr.getCollisionMask(nGetColMaskFlag);
                        }

                        // Active sprite = box ?
                        if (bSprColBox)
                        {
                            if (pBkdMask == null)		// No background mask? collision
                            {
                                return true;
                            }

                            // Test collision between background mask and sprite rectangle
                            if (pBkdMask.testRect(0, sprRc.left - rc.left, sprRc.top - rc.top, nWidth, nHeight))
                            {
                                return true;
                            }
                        }
                        // Active sprite = bitmap
                        else
                        {
                            // Get sprite mask
                            yMaskBits = 0;
                            if (subHt != 0)
                            {
                                if (subHt > nHeight)
                                {
                                    subHt = nHeight;
                                }
                                yMaskBits = nHeight - subHt;
                            }

                            // No background mask
                            if (pBkdMask == null)
                            {
                                // Test collision between sprite mask and background rectangle
                                if (pSprMask.testRect(yMaskBits, rc.left - sprRc.left, rc.top - sprRc.top, rc.right - rc.left, rc.bottom - rc.top))
                                {
                                    return true;
                                }
                            }
                            // Background mask
                            else
                            {
                                if (pSprMask == null)
                                {
                                    // Test collision between background mask and sprite rectangle
                                    if (pBkdMask.testRect(0, sprRc.left - rc.left, sprRc.top - rc.top, nWidth, nHeight))
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    // Test collision between background and sprite masks
                                    if (pBkdMask.testMask(0, rc.left, rc.top, pSprMask, yMaskBits, sprRc.left, sprRc.top))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                } while (false);

                // Wrapped?
                if (dwWrapFlags != 0)
                {
                    i--;
                }
            }

            // Scan Bkd2s
            if (pLayer.pBkd2 != null)
            {
                CBkd2 pbkd;

                dwWrapFlags = 0;
                nSprite = 0;

                for (i = 0; i < pLayer.pBkd2.size(); i++)
                {
                    pbkd = (CBkd2)pLayer.pBkd2.get(i);

                    // Get object position
                    rc.left = pbkd.x - dxLayer;
                    rc.top = pbkd.y - dyLayer;

                    v = pbkd.obstacleType;
                    if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                    {
                        continue;
                    }
                    cm_box = (pbkd.colMode == CSpriteGen.CM_BOX) ? 1 : 0;

                    // Get object rectangle
                    image = app.imageBank.getImageFromHandle(pbkd.img);
                    if (image != null)
                    {
                        rc.right = rc.left + image.width;
                        rc.bottom = rc.top + image.height;
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
                                        dwWrapFlags |= (CRun.WRAP_X | CRun.WRAP_Y | CRun.WRAP_XY);
                                    }

                                    // Wrap horizontally only
                                    else
                                    {
                                        nSprite = 1;
                                        dwWrapFlags |= (CRun.WRAP_X);
                                    }
                                }

                                // Wrap vertically?
                                else if (bWrapVert && (rc.top < 0 || rc.bottom > nPlayfieldHeight))
                                {
                                    nSprite = 2;
                                    dwWrapFlags |= (CRun.WRAP_Y);
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
                                dwWrapFlags &= ~CRun.WRAP_X;

                                // Calculate next sprite to display
                                nSprite = 0;
                                if ((dwWrapFlags & CRun.WRAP_Y) != 0)
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
                                dwWrapFlags &= ~CRun.WRAP_Y;

                                // Calculate next sprite to display
                                nSprite = 0;
                                if ((dwWrapFlags & CRun.WRAP_X) != 0)
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
                                dwWrapFlags &= ~CRun.WRAP_XY;

                                // Calculate next sprite to display
                                nSprite = 2;
                                break;
                        }
                    }

                    do
                    {
                        // No Intersection?
                        if (sprRc.right <= rc.left || sprRc.bottom <= rc.top)
                        {
                            break;
                        }

                        if (sprRc.left >= rc.right || sprRc.top >= rc.bottom)
                        {
                            break;
                        }

                        // Obstacle and ask for platform or reciprocally? continue
                        if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est écrit dans les 2 masques...
                                (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                        {
                            break;
                        }

                        // Background sprite = Box?
                        if (cm_box != 0)
                        {
                            // Collision between 2 boxes? OK
                            if (bSprColBox)
                            {
                                return true;
                            }

                            // Active sprite = bitmap
                            // => test collision between background rectangle and sprite's mask
					        // Get background mask
					        image = app.imageBank.getImageFromHandle(pbkd.img);
                            pBkdMask = image.getMask(CMask.GCMF_OBSTACLE, 0, (float)1.0, (float)1.0);
                            if (pSprMask == null)
                            {
                                return true;		// Can't calculate mask => box collision
                            }
                            yMaskBits = 0;
                            if (subHt != 0)
                            {
                                if (subHt > nHeight)
                                {
                                    subHt = nHeight;
                                }
                                yMaskBits = nHeight - subHt;
                            }

                            if (pSprMask.testRect(yMaskBits, rc.left - sprRc.left, rc.top - sprRc.top, rc.right - rc.left, rc.bottom - rc.top))
                            {
                                return true;
                            }
                        }
                        // Background sprite = bitmap
                        else
                        {
                            int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                            if (v == COC.OBSTACLE_PLATFORM)
                            {
                                nGetColMaskFlag = CMask.GCMF_PLATFORM;
                            }

                            // Get background mask
                            image = app.imageBank.getImageFromHandle(pbkd.img);
                            pBkdMask = image.getMask(nGetColMaskFlag, 0, (float)1.0, (float)1.0);
                            if (pBkdMask == null)
                            {
                                continue;
                            }

                            // Active sprite = box ?
                            if (bSprColBox)
                            {
                                // Test collision between background mask and sprite rectangle
                                if (pBkdMask.testRect(0, sprRc.left - rc.left, sprRc.top - rc.top, nWidth, nHeight))
                                {
                                    return true;
                                }
                            }
                            // Active sprite = bitmap
                            else
                            {
                                if (pSprMask == null)
                                {
                                    return true;		// Can't calculate mask => box collision
                                }
                                yMaskBits = 0;
                                if (subHt != 0)
                                {
                                    if (subHt > nHeight)
                                    {
                                        subHt = nHeight;
                                    }
                                    yMaskBits = nHeight - subHt;
                                }

                                // Test collision between background mask and sprite's mask
                                if (pBkdMask.testMask(0, rc.left, rc.top, pSprMask, yMaskBits, sprRc.left, sprRc.top))
                                {
                                    return true;
                                }
                            }
                        }
                    } while (false);

                    // Wrapped?
                    if (dwWrapFlags != 0)
                    {
                        i--;
                    }
                }
            }
            return false;
        }

        ////////////////////////////////////////////////
        //
        // Test collision with a specific point
        //
        public bool bkdCol_TestPoint(int x, int y, int nLayer, int nPlane)
        {
            CLayer pLayer;
            int dwFlags;

            // All layers?
            if (nLayer == CSpriteGen.LAYER_ALL)
            {
                // Test with layer 0
                ////////////////////

                // Wrap mode and full collision mask?
                pLayer = layers[0];
                if ((leFlags & LEF_TOTALCOLMASK) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_WRAP_HORZ | CLayer.FLOPT_WRAP_VERT)) != 0)
                {
                    // Handle collisions like with the other layers (detect collisions with the objects, not with the collision mask)
                    if (bkdLevObjCol_TestPoint(x, y, 0, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // Normal mode (no wrap mode, or windowed collision mask)
                else
                {
                    if (colMask != null)
                    {
                        if (colMask.testPoint(x, y, nPlane))
                        {
                            return true;
                        }
                    }
                }

                // Other layers
                ///////////////
                if (nLayers == 1)
                {
                    return false;
                }

                // Test with background objects
                if ((leFlags & LEF_TOTALCOLMASK) != 0)
                {
                    // Total colmask => test with levObjs
                    return bkdLevObjCol_TestPoint(x, y, nLayer, nPlane);
                }
                else
                {
                    // Partial colmask => test with background sprites
                    dwFlags = CSpriteGen.SCF_BACKGROUND;
                    if (nPlane == CColMask.CM_TEST_PLATFORM)
                    {
                        dwFlags |= CSpriteGen.SCF_PLATFORM;
                    }
                    else
                    {
                        dwFlags |= CSpriteGen.SCF_OBSTACLE;
                    }

                    return (app.spriteGen.spriteCol_TestPoint(null, (short)nLayer, x, y, dwFlags) != null);
                }
            }

            // Layer 0?
            if (nLayer == 0)
            {
                // Wrap mode and full collision mask?
                pLayer = layers[0];
                if ((leFlags & LEF_TOTALCOLMASK) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_WRAP_HORZ | CLayer.FLOPT_WRAP_VERT)) != 0)
                {
                    // Handle collisions like with the other layers (detect collisions with the objects, not with the collision mask)
                    if (bkdLevObjCol_TestPoint(x, y, 0, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // Normal mode (no wrap mode, or windowed collision mask)
                else
                {
                    return colMask.testPoint(x, y, nPlane);
                }
            }

            // Only one layer?
            if (nLayers == 1)
            {
                return false;
            }

            // Layer > 0, total colmask?
            if ((leFlags & LEF_TOTALCOLMASK) != 0)
            {
                // Total colmask => test with levObjs
                return bkdLevObjCol_TestPoint(x, y, nLayer, nPlane);
            }

            // Partial colmask => test with background sprites
            dwFlags = CSpriteGen.SCF_BACKGROUND;
            if (nPlane == CColMask.CM_TEST_PLATFORM)
            {
                dwFlags |= CSpriteGen.SCF_PLATFORM;
            }
            else
            {
                dwFlags |= CSpriteGen.SCF_OBSTACLE;
            }

            return (app.spriteGen.spriteCol_TestPoint(null, CSpriteGen.LAYER_ALL, x, y, dwFlags) != null);
        }

        ////////////////////////////////////////////////
        //
        // Test collision with a rectangle
        //
        public bool bkdCol_TestRect(int x, int y, int nWidth, int nHeight, int nLayer, int nPlane)
        {
            CLayer pLayer;
            int dwFlags;

            // All layers?
            if (nLayer == CSpriteGen.LAYER_ALL)
            {
                // Test with layer 0
                ////////////////////

                // Wrap mode and full collision mask?
                pLayer = layers[0];
                if ((leFlags & LEF_TOTALCOLMASK) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_WRAP_HORZ | CLayer.FLOPT_WRAP_VERT)) != 0)
                {
                    // Handle collisions like with the other layers (detect collisions with the objects, not with the collision mask)
                    if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // Normal mode (no wrap mode, or windowed collision mask)
                else
                {
                    if (colMask.testRect(x, y, nWidth, nHeight, nPlane))
                    {
                        return true;
                    }
                }

                // Other layers
                ///////////////
                if (nLayers == 1)
                {
                    return false;
                }

                // Test with background objects
                if ((leFlags & LEF_TOTALCOLMASK) != 0)
                {
                    // Total colmask => test with levObjs
                    if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Partial colmask => test with background sprites
                    dwFlags = CSpriteGen.SCF_BACKGROUND;
                    if (nPlane == CColMask.CM_TEST_PLATFORM)
                    {
                        dwFlags |= CSpriteGen.SCF_PLATFORM;
                    }
                    else
                    {
                        dwFlags |= CSpriteGen.SCF_OBSTACLE;
                    }

                    if (app.spriteGen.spriteCol_TestRect(null, nLayer, x, y, nWidth, nHeight, dwFlags) != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Layer 0?
            if (nLayer == 0)
            {
                // Wrap mode and full collision mask?
                pLayer = layers[0];
                if ((leFlags & LEF_TOTALCOLMASK) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_WRAP_HORZ | CLayer.FLOPT_WRAP_VERT)) != 0)
                {
                    // Handle collisions like with the other layers (detect collisions with the objects, not with the collision mask)
                    if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // Normal mode (no wrap mode, or windowed collision mask)
                else
                {
                    if (colMask.testRect(x, y, nWidth, nHeight, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Only one layer?
            if (nLayers == 1)
            {
                return false;
            }

            // Layer > 0, total colmask?
            if ((leFlags & LEF_TOTALCOLMASK) != 0)
            {
                // Total colmask => test with levObjs
                if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Partial colmask => test with background sprites
            dwFlags = CSpriteGen.SCF_BACKGROUND;
            if (nPlane == CColMask.CM_TEST_PLATFORM)
            {
                dwFlags |= CSpriteGen.SCF_PLATFORM;
            }
            else
            {
                dwFlags |= CSpriteGen.SCF_OBSTACLE;
            }

            return (app.spriteGen.spriteCol_TestRect(null, CSpriteGen.LAYER_ALL, x, y, nWidth, nHeight, dwFlags) != null);
        }

        public bool bkdCol_TestSprite(CSprite pSpr, int newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
        {
            // Get sprite layer
            int dwLayer = pSpr.sprLayer / 2;	// GetSpriteLayer(idEditWin, pSpr);

            // Layer 0
            CLayer pLayer;
            uint dwFlags;
            if (dwLayer == 0)
            {
                // Wrap mode and full collision mask?
                pLayer = layers[0];
                if ((leFlags & LEF_TOTALCOLMASK) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_WRAP_HORZ | CLayer.FLOPT_WRAP_VERT)) != 0)
                {
                    // Handle collisions like with the other layers (detect collisions with the objects, not with the collision mask)
                    if (bkdLevObjCol_TestSprite(pSpr, (short)newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // Normal mode (no wrap mode, or windowed collision mask)
                else
                {
                    if (colMask_TestSprite(pSpr, newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Only one layer?
            if (nLayers == 1)
            {
                return false;
            }

            // Layer > 0, total colmask?
            if ((leFlags & LEF_TOTALCOLMASK) != 0)
            {
                // Total colmask => test with levObjs
                if (bkdLevObjCol_TestSprite(pSpr, (short)newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Partial colmask => test with background sprites
            dwFlags = CSpriteGen.SCF_BACKGROUND;
            if (nPlane == CColMask.CM_TEST_PLATFORM)
            {
                dwFlags |= CSpriteGen.SCF_PLATFORM;
            }
            else
            {
                dwFlags |= CSpriteGen.SCF_OBSTACLE;
            }

            return (app.spriteGen.spriteCol_TestSprite(pSpr, (short)newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, dwFlags) != null);
        }

        //-------------------------------------------------------------------------;
        //	Tester la collision d'un sprite avec le masque du fond d'une fenetre	;
        //-------------------------------------------------------------------------;
        public bool colMask_TestSprite(CSprite pSpr, int newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
        {
            if (pSpr == null || colMask == null)
            {
                return false;
            }

            int nImg = newImg;
            int x1 = newX;
            int y1 = newY;
            int nColMode = (int)app.spriteGen.colMode;
            int nWidth, nHeight;
            CRect rc = new CRect();

            if (newImg == 0)
            {
                nImg = pSpr.sprImg;
            }

            // Bitmap collision?
            if (nColMode != CSpriteGen.CM_BOX && (pSpr.sprFlags & CSprite.SF_COLBOX) == 0)
            {
                CMask pMask = null;

		        // Image sprite not stretched and not rotated, or owner draw sprite?
		        pMask = app.spriteGen.getSpriteMask(pSpr, (short)nImg, CMask.GCMF_OBSTACLE, newAngle, newScaleX, newScaleY);
		        if (pMask == null)
		        {
			        x1 -= (pSpr.sprX - pSpr.sprX1);
			        y1 -= (pSpr.sprY - pSpr.sprY1);
			        nWidth = pSpr.sprX2 - pSpr.sprX1;
			        nHeight = pSpr.sprY2 - pSpr.sprY1;
		        }
		        else
		        {
			        // Get sprite box
			        if ((pSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
			        {
				        x1 -= pMask.xSpot;
				        y1 -= pMask.ySpot;
			        }
			        nWidth = pMask.width;
			        nHeight = pMask.height;
		        }
        		

                // Test mask collision
                if (pMask != null)
                {
                    int yMaskBits = 0;

                    // Take subHt into account
                    if (subHt != 0)
                    {
                        if (subHt > nHeight)
                        {
                            subHt = nHeight;
                        }
                        y1 += nHeight - subHt;
                        yMaskBits = nHeight - subHt;
                        nHeight = subHt;
                    }
                    return colMask.testMask(pMask, yMaskBits, x1, y1, nPlane);
                }
            }
            else
            {
                // Box collision: no need to calculate the mask
                if (nImg == 0 || nImg == pSpr.sprImg || (pSpr.sprFlags & CSprite.SF_OWNERDRAW) != 0)
                {
                    x1 -= (pSpr.sprX - pSpr.sprX1);
                    y1 -= (pSpr.sprY - pSpr.sprY1);
                    nWidth = pSpr.sprX2 - pSpr.sprX1;
                    nHeight = pSpr.sprY2 - pSpr.sprY1;
                }
                else
                {
                    CImage pei = app.imageBank.getImageFromHandle((short)nImg);
                    if (pei != null)
                    {
                        x1 -= pei.xSpot;
                        y1 -= pei.ySpot;
                        nWidth = pei.width;
                        nHeight = pei.height;
                    }
                    else
                    {
                        x1 -= (pSpr.sprX - pSpr.sprX1);
                        y1 -= (pSpr.sprY - pSpr.sprY1);
                        nWidth = pSpr.sprX2 - pSpr.sprX1;
                        nHeight = pSpr.sprY2 - pSpr.sprY1;
                    }
                }
            }

            // Take subHt into account
            if (subHt != 0)
            {
                if (subHt > nHeight)
                {
                    subHt = nHeight;
                }
                y1 += nHeight - subHt;
                nHeight = subHt;
            }

            return colMask.testRect(x1, y1, nWidth, nHeight, nPlane);
        }


    }
}
