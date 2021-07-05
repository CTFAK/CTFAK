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
// CRUNFRAME : contenu d'une frame
//
//----------------------------------------------------------------------------------
package Application;

import Banks.CImage;
import Events.CEventProgram;
import Frame.CLO;
import Frame.CLOList;
import Frame.CLayer;
import OI.COC;
import OI.COCBackground;
import OI.COI;
import OI.CObjectCommon;
import Objects.CObject;
import RunLoop.CBkd2;
import RunLoop.CRun;
import Services.CArrayList;
import Services.CChunk;
import Services.CRect;
import Sprites.CColMask;
import Sprites.CMask;
import Sprites.CSprite;
import Sprites.CSpriteGen;
import Transitions.CTrans;
import Transitions.CTransitionData;
import android.os.SystemClock;
import Runtime.Log;

public class CRunFrame 
{
    CRunApp app;
    public CRun rhPtr;
    
    // Flags
    public static final int LEF_DISPLAYNAME=0x0001;
    public static final int LEF_GRABDESKTOP=0x0002;
    public static final int LEF_KEEPDISPLAY=0x0004;
    public static final int LEF_TOTALCOLMASK=0x0020;
//    public static final int LEF_PASSWORD=0x0040;
    public static final int LEF_RESIZEATSTART=0x0100;
//    public static final int LEF_DONOTCENTER=0x0200;
//    public static final int LEF_FORCE_LOADONCALL=0x0400;
    public static final int LEF_NOSURFACE=0x0800;
//    public static final int LEF_RESERVED_1=0x1000;
//    public static final int LEF_RESERVED_2=0x2000;
    public static final int LEF_TIMEDMVTS=0x8000;

    public static final int  IPHONEOPT_JOYSTICK_FIRE1 = 0x0001;
    public static final int  IPHONEOPT_JOYSTICK_FIRE2 = 0x0002;
    public static final int  IPHONEOPT_JOYSTICK_LEFTHAND = 0x0004;
    public static final int  IPHONEOPT_MULTITOUCH = 0x0008;
    public static final int  IPHONEOPT_SCREENLOCKING = 0x0010;
    public static final int  IPHONEOPT_IPHONEFRAMEIAD = 0x0020;
    public static final int  ANDROIDFOPT_FRAMEAD = 0x0040;
    public static final int  ANDROIDFOPT_ADBOTTOM = 0x0080;
    public static final int  ANDROIDFOPT_ADOVERFRAME = 0x0100;
	public static final int  ANDROIDFOPT_JOYSTICK_DPAD = 0x0200;
    public static final int  JOYSTICK_NONE = 0x0000;
    public static final int  JOYSTICK_TOUCH = 0x0001;
    public static final int  JOYSTICK_ACCELEROMETER = 0x0002;
    public static final int  JOYSTICK_EXT = 0x0003;


    // Frameheader
    public int leWidth;			/// Playfield width in pixels
    public int leHeight;		/// Playfield height in pixels
    public int leBackground;
    public int leFlags;
    
    public CRect leVirtualRect;
    public int leEditWinWidth;
    public int leEditWinHeight;
    public String frameName=null;
    public int nLayers;
    public CLayer layers[];             /// Array of layers
    public CLOList LOList;              /// List of Level Objects
    public CEventProgram evtProg;       /// Program of events
    public short maxObjects;
    
    // Coordinates of top-level pixel in edit window
    public int leX;
    public int leY;
    public int leLastScrlX;
    public int leLastScrlY;

    public short joystick;
    public short iPhoneOptions;
    
    // Exit code
    public int levelQuit;

    // Events
    public boolean rhOK;				// TRUE when the events are initialized

    // public int nPlayers;
    //	int				m_nPlayersReal;
    //	int				m_level_loop_state;
    public int startLeX;
    public int startLeY;
    //	short			m_maxObjects;
    //	short			m_maxOI;
    //	LPOBL			m_oblEnum;
    //	int				m_oblEnumCpt;
    //	BOOL			m_eventsBranched;
    public boolean fade=false;
    public long fadeTimerDelta;
    public int fadeVblDelta;
    //	DWORD			m_pasteMask;

    //	int				m_nCurTempString;
    //	LPSTR			m_pTempString[MAX_TEMPSTRING];
    public int dwColMaskBits;
    public CColMask colMask=null;           /// Collision mask
    public short m_wRandomSeed;
    public int frameAlpha=0;
    public int keyTimeOut=500;
    public int m_dwMvtTimerBase;

    // Transitions
    public CTransitionData fadeIn=null;
    public CTransitionData fadeOut=null;
    public CTrans pTrans=null;

    public boolean haveUpdatedViewport = false;
    public int frame_number;

    public CRunFrame() 
    {
    }
    public CRunFrame(CRunApp pApp) 
    {
    	app=pApp;
    }
    
    // Charge la frame
    public boolean loadFullFrame(int index)
    {
        frame_number = index + 1;
        Log.Log("loading frame #:"+frame_number);
		// Positionne le fichier
        app.file.seek((int) app.frameOffsets[index]);

		// Charge la frame
		evtProg=new CEventProgram();
		LOList=new CLOList();
		leVirtualRect=new CRect();
		
		CChunk chk=new CChunk();
		long posEnd;
		int nOldFrameWidth=0;
		int nOldFrameHeight=0;
		m_wRandomSeed=-1;
		while (chk.chID!=0x7F7F)	// CHUNK_LAST
		{
		    chk.readHeader(app.file);
		    if (chk.chSize==0)
			continue;
		    posEnd=app.file.getFilePointer()+chk.chSize;
		    switch(chk.chID)
		    {
			// CHUNK_FRAMEHEADER
			case 0x3334:
			    loadHeader();

			    if ( (leFlags & LEF_RESIZEATSTART)!=0 )
			    {
				nOldFrameWidth = leWidth;
				nOldFrameHeight = leHeight;
	
				leWidth = app.gaCxWin;
				leHeight = app.gaCyWin;
	
				// To keep compatibility with previous versions without virtual rectangle (necessaire ?)
				leVirtualRect.left = leVirtualRect.top = 0;
				leVirtualRect.right = leWidth;
				leVirtualRect.bottom = leHeight;
			    }

	            updateSize();

			    break;
			    
			// CHUNK_FRAMEVIRTUALRECT
			case 0x3342:
			    leVirtualRect.load(app.file);
			    if ( (leFlags & LEF_RESIZEATSTART)!=0 )
			    {
			    	if ( leVirtualRect.right - leVirtualRect.left == nOldFrameWidth || leVirtualRect.right - leVirtualRect.left < leWidth )
			    		leVirtualRect.right = leVirtualRect.left + leWidth;
			    	if ( leVirtualRect.bottom - leVirtualRect.top == nOldFrameHeight || leVirtualRect.bottom - leVirtualRect.top < leHeight )
			    		leVirtualRect.bottom = leVirtualRect.top + leHeight;
			    }
			    break;
	
			// CHUNK_RANDOMSEED
			case 0x3344:
			    m_wRandomSeed=app.file.readAShort();
			    break;
	
	        // CHUNK_MVTTIMERBASE
	        case 0x3347:
	            m_dwMvtTimerBase=app.file.readAInt();
	            break;
			    		    
			// CHUNK_FRAMENAME
			case 0x3335:
			    frameName=app.file.readAString();
			    break;
			    
			// CHUNK_FRAMEPALETTE
			case 0x3337:
			    break;
			    
			// CHUNK_FRAMELAYERS
			case 0x3341:
			    loadLayers();
			    break;
			
			// CHUNK_FRAMELAYEREFFECTS
			case 0x3345:
				loadLayerEffects();
				break;
			// CHUNK_FRAMEITEMINSTANCES
			case 0x3338:
			    LOList.load(app);
			    break;
	
			// CHUNK_ADDITIONAL_FRAMEITEMINSTANCE
			case 0x3340:
			    break;
			    
			// CHUNK_FRAMEEVENTS
			case 0x333D:
			    evtProg.load(app);
			    maxObjects=evtProg.maxObjects;
			    break;
			    
			// CHUNK_FRAMEOPTIONS1
			case  0x3346:
			    frameAlpha=app.file.readAInt();
			    keyTimeOut=app.file.readAInt();
			    break;

            // CHUNK_FRAME_IPHONE_OPTIONS
			case 0x334A:
				joystick=app.file.readAShort();
				iPhoneOptions=app.file.readAShort();
				break;

            // CHUNK_FRAMEFADEIN
			case 0x333B:
			    //fadeIn=new CTransitionData();
                //fadeIn.load(app.file);     
			    break;
			    
			// CHUNK_FRAMEFADEOUT
			case 0x333C:
                //fadeOut=new CTransitionData();
                //fadeOut.load(app.file);
                break;
			    
		    }
		    // Positionne a la fin du chunk
		    app.file.seek((int) posEnd);
		}
	
		// Marque les OI a charger
		app.OIList.resetToLoad();
		int n;
		for (n=0; n<LOList.nIndex; n++)
		{
		    CLO lo=LOList.getLOFromIndex((short)n);
		    app.OIList.setToLoad(lo.loOiHandle);
		}
		
		// Charge les OI et les elements des banques
		app.imageBank.resetToLoad();
		app.fontBank.resetToLoad();
		app.OIList.load(app.file);
		app.OIList.enumElements(app.imageBank, app.fontBank);
	    app.imageBank.load();
		app.fontBank.load(app.file);
		evtProg.enumSounds(app.musicBank, app.soundBank);
		app.soundBank.load(app);
		app.musicBank.load(app);
		
		// Marque les OI de la frame
		app.OIList.resetOICurrent();
		for (n=0; n<LOList.nIndex; n++)
		{
		    CLO lo=LOList.list[n];
		    if (lo.loType>=COI.OBJ_SPR)
            {
                app.OIList.setOICurrent(lo.loOiHandle);
            }
        }
		
		int cntImage = 0;
		while(!app.imageBank.loaded() || cntImage > 10000)
		{
			SystemClock.sleep(10);
			cntImage++;
		}
		
        return true;
    }

    public void updateSize()
    {
        // B243
        if ( (leFlags & LEF_RESIZEATSTART)!=0 )
        {
            leEditWinWidth = app.gaCxWin;
            leEditWinHeight = app.gaCyWin;
        }
        else
        {
            leEditWinWidth=Math.min(app.gaCxWin, leWidth);
            leEditWinHeight=Math.min(app.gaCyWin, leHeight);
        }
    }

    public void loadLayers()
    {
		nLayers=app.file.readAInt();
		layers=new CLayer[nLayers];
		
		int n;
		for (n=0; n<nLayers; n++)
		{
		    layers[n]=new CLayer();
		    layers[n].load(app.file);
		}
    }
    
	public void loadLayerEffects()
	{
		/*
		int n;
		for (n=0; n<nLayers; n++)
		{
			layers[n].effect=app.file.readAShort();
			layers[n].effectParam=app.file.readAShort();
			app.file.skipBytes(12);				
		}
		*/
	}

    public void loadHeader()
    {
		leWidth=app.file.readAInt();
		leHeight=app.file.readAInt();
		leBackground=app.file.readAColor();
		leFlags=app.file.readAInt();
    }

    // Get obstacle mask bits
    public int getMaskBits ()    
    {
		int flgs=0;
		
		int n;
		for (n=0; n<LOList.nIndex; n++)
		{
		    CLO lo=LOList.getLOFromIndex((short)n);
		    if ( lo.loLayer > 0 )
		    	break;
	
		    COI poi=app.OIList.getOIFromHandle(lo.loOiHandle);
		    if ( poi.oiType < COI.OBJ_SPR )
		    {
				COC poc=poi.oiOC;
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
			CObjectCommon pCommon=(CObjectCommon)poi.oiOC;
			if ( (pCommon.ocOEFlags & CObjectCommon.OEFLAG_BACKGROUND) != 0 )
			{
			    switch ((pCommon.ocFlags2&CObjectCommon.OCFLAGS2_OBSTACLEMASK) >> CObjectCommon.OCFLAGS2_OBSTACLESHIFT) 
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
    public boolean bkdLevObjCol_TestPoint(int x, int y, int nTestLayer, int nPlane)
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

            boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
            boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
            boolean bWrap = (bWrapHorz | bWrapVert);
            int i;

            // Layer offset
            int dxLayer = leX;
            int dyLayer = leY;
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

            int nLOs = pLayer.nBkdLOs;

            // Optimization (only if no wrap)
            int nxz, nyz;
            //int nz;
            CArrayList<CArrayList<Integer>> pZones = null;
            CArrayList<Integer> pZone = null;
            nxz = nyz = 0;
            //nz = 0;
            if ( !bWrap )
            {
                // Get (or calculate) LO lists per zone (zone width = 512 x 512)
                pZones = app.run.getLayerZones(nLayer);

                // Get number of zones
                nxz = ((app.frame.leWidth + CRun.OBJZONE_WIDTH - 1)/ CRun.OBJZONE_WIDTH) + 2;
                nyz = ((app.frame.leHeight + CRun.OBJZONE_HEIGHT - 1)/ CRun.OBJZONE_HEIGHT) + 2;
                //nz = nxz * nyz;
            }

            if ( pZones != null )
            {
                int zy = 0;
                if ( (y + dyLayer) >= 0 )
                    zy = Math.min((y + dyLayer) / CRun.OBJZONE_HEIGHT + 1, nyz-1);
                int zx = 0;
                if ( (x + dxLayer) >= 0 )
                    zx = Math.min((x + dxLayer) / CRun.OBJZONE_WIDTH + 1, nxz-1);
                int z = zy * nxz + zx;
                //ASSERT(z < nz);
                pZone = pZones.get(z);
                if ( pZone != null )
                    nLOs = pZone.size();
                else
                    nLOs = 0;
            }

            for (i = 0; i < nLOs; i++)
            {
                CLO plo = LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex + i));
                CObject hoPtr = null;

                if (pZone != null)
                    plo = app.frame.LOList.getLOFromIndex(pZone.get(i).shortValue());

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
                    CObjectCommon pCommon = (CObjectCommon) poc;
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
                    if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est crit dans les 2 masques...
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
				    //if ( (poi.oiLoadFlags & COI.OILF_ELTLOADED) == 0 )
					//    poi.loadOnCall(app);

                    int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                    if (v == COC.OBSTACLE_PLATFORM)
                    {
                        nGetColMaskFlag = CMask.GCMF_PLATFORM;
                    }

                    // Test if point into image mask
                    pMask = null;
                    if (typeObj < COI.OBJ_SPR)
                    {
                        image = app.imageBank.getImageFromHandle(((COCBackground) poc).ocImage);
                        if(image != null)
                        	pMask = image.getMask(nGetColMaskFlag, 0, 1.0, 1.0);
                    }
                    else
                    {
                        pMask = hoPtr.getCollisionMask(nGetColMaskFlag);
                    }
                    if (pMask == null)		// No mask? collision
                    {
                        return true;
                    }

                    if (pMask != null && pMask.testPoint(x - rc.left, y - rc.top))
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
                
                int pLayer_pBkd2_size = pLayer.pBkd2.size();
                for (i = 0; i < pLayer_pBkd2_size; i++)
                {
                    pbkd = pLayer.pBkd2.get(i);

                    if(pbkd == null)
                    	continue;
                    
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
                        if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est crit dans les 2 masques...
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
                        pMask = image.getMask(nGetColMaskFlag, 0, 1.0, 1.0);
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
            }
        }
        return false;
    }

    /** Tests a rectangle in the background collision.
     * Depending on the number of the layer, explores the obstacle mask, or calls 
     * the sprites collision detection.
     */
    public boolean bkdLevObjCol_TestRect(int x, int y, int nWidth, int nHeight, int nTestLayer, int nPlane)
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

            boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
            boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
            boolean bWrap = (bWrapHorz | bWrapVert);
            int i;

            // Layer offset
            int dxLayer = leX;
            int dyLayer = leY;
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
            int nLOs = pLayer.nBkdLOs;

            // Optimization (only if no wrap)
            int nxz, nyz;
            //int nz;
            CArrayList<CArrayList<Integer>> pZones = null;
            CArrayList<Integer> pZone = null;
            nxz = nyz = 0;
            //nz = 0;
            if ( !bWrap )
            {
                // Get (or calculate) LO lists per zone (zone width = 512 x 512)
                pZones = app.run.getLayerZones(nLayer);

                // Get number of zones
                nxz = ((app.frame.leWidth + CRun.OBJZONE_WIDTH - 1)/ CRun.OBJZONE_WIDTH) + 2;
                nyz = ((app.frame.leHeight + CRun.OBJZONE_HEIGHT - 1)/ CRun.OBJZONE_HEIGHT) + 2;
                //nz = nxz * nyz;
            }

            int minzy, maxzy;
            minzy = maxzy = 0;
            if ( pZones != null )
            {
                if ( (y + dyLayer) >= 0 )
                    minzy = Math.min((y + dyLayer) / CRun.OBJZONE_HEIGHT + 1, nyz-1);
                if ( ((y+nHeight-1) + dyLayer) >= 0 )
                    maxzy = Math.min(((y+nHeight-1) + dyLayer) / CRun.OBJZONE_HEIGHT + 1, nyz-1);
            }
            for (int zy=minzy; zy<=maxzy; zy++)
            {
                int minzx, maxzx;
                minzx = maxzx = 0;
                if ( pZones != null )
                {
                    if ( (x + dxLayer) >= 0 )
                        minzx = Math.min((x + dxLayer) / CRun.OBJZONE_WIDTH + 1, nxz-1);
                    if ( ((x+nWidth-1) + dxLayer) >= 0 )
                        maxzx = Math.min(((x+nWidth-1) + dxLayer) / CRun.OBJZONE_WIDTH + 1, nxz-1);
                }
                for (int zx=minzx; zx<=maxzx; zx++)
                {
                    if ( pZones != null )
                    {
                        int z = zy * nxz + zx;
                        //ASSERT(z < nz);
                        pZone = pZones.get(z);
                        if ( pZone == null )
                            continue;
                        nLOs = pZone.size();
                    }

                    for (i = 0; i < nLOs; i++)
                    {
                        CLO plo = LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex + i));
                        CObject hoPtr = null;

                        if (pZone != null)
                            plo = app.frame.LOList.getLOFromIndex(pZone.get(i).shortValue());

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
                            CObjectCommon pCommon = (CObjectCommon) poc;
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
                            if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est crit dans les 2 masques...
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
        					//if ( (poi.oiLoadFlags & COI.OILF_ELTLOADED) == 0 )
        					//	poi.loadOnCall(app);

                            // Get background mask
                            int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                            if (v == COC.OBSTACLE_PLATFORM)
                            {
                                nGetColMaskFlag = CMask.GCMF_PLATFORM;
                            }

                            if (typeObj < COI.OBJ_SPR)
                            {
                                image = app.imageBank.getImageFromHandle(((COCBackground) poc).ocImage);
                                if(image != null)
                                	pMask = image.getMask(nGetColMaskFlag, 0, 1.0, 1.0);
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
                            if (pMask != null && pMask.testRect(0, x - rc.left, y - rc.top, nWidth, nHeight))
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
                }
            }

            // Scan Bkd2s
            if (pLayer.pBkd2 != null)
            {
                CBkd2 pbkd;

                dwWrapFlags = 0;
                nSprite = 0;
                int pLayer_pBkd2_size = pLayer.pBkd2.size();
                for (i = 0; i < pLayer_pBkd2_size; i++)
                {
                    pbkd = pLayer.pBkd2.get(i);

                    if(pbkd == null)
                    	continue;
                    
                    rc.left = pbkd.x - dxLayer;
                    rc.top = pbkd.y - dyLayer;

                    v = pbkd.obstacleType;
                    if (v == 0 || v == COC.OBSTACLE_LADDER || v == COC.OBSTACLE_TRANSPARENT)
                    {
                        continue;
                    }
                    cm_box = (pbkd.colMode == CSpriteGen.CM_BOX) ? 1 : 0;

                    // Get object rectangle
                    //image = app.imageBank.getImageFromHandle(pbkd.img);
                    //if (image != null)
                    //{
                    //    rc.right = rc.left + image.getWidth ();
                    //    rc.bottom = rc.top + image.getHeight ();
                    //}
                    //else
                    //{
                    //    rc.right = rc.left + 1;
                    //    rc.bottom = rc.top + 1;
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
                        if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est crit dans les 2 masques...
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
                        if(image != null)
                        	pMask = image.getMask(nGetColMaskFlag, 0, 1.0, 1.0);
						if (pMask == null)		// No mask? collision
						{
							return true;
						}
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
            }
        }
        return false;
    }

    public boolean bkdLevObjCol_TestSprite(CSprite pSpr, short newImg, int newX, int newY, float newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
    {
        CObject hoPtr;
        int v;
        int cm_box;
        CRect rc = new CRect();

        // Get sprite layer
        int nLayer = pSpr.sprLayer / 2;	    // GetSpriteLayer

        CLayer pLayer = layers[nLayer];

        boolean bWrapHorz = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_HORZ) != 0);
        boolean bWrapVert = ((pLayer.dwOptions & CLayer.FLOPT_WRAP_VERT) != 0);
        boolean bWrap = (bWrapHorz | bWrapVert);
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

        // Sprite collision mode
        int dwSprFlags = pSpr.sprFlags;
        boolean bSprColBox = ((dwSprFlags & CSprite.SF_COLBOX) != 0);

        // Sprite rectangle
        CRect sprRc = new CRect();
        int nWidth, nHeight;
        int nImg = newImg;

        sprRc.left = newX;
        sprRc.top = newY;
        if (newImg == 0)
        {
            nImg = pSpr.sprImg;
        }

        CMask pSprMask = null;
        CMask pBkdMask = null;
        //int dwPSCFlags = 0;
        int yMaskBits = 0;

        // Bitmap collision?
        if (!bSprColBox)
        {
            // Image sprite not stretched and not rotated, or owner draw sprite?
            pSprMask = rhPtr.spriteGen.getSpriteMask(pSpr, (short) nImg, CMask.GCMF_OBSTACLE | CMask.GCMF_FORCEMASK, newAngle, newScaleX, newScaleY);
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
                    sprRc.left -= pSprMask.getSpotX ();
                    sprRc.top -= pSprMask.getSpotY ();
                }
                nWidth = pSprMask.getWidth ();
                nHeight = pSprMask.getHeight ();
                sprRc.right = sprRc.left + nWidth;
                sprRc.bottom = sprRc.top + nHeight;
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
                image = app.imageBank.getImageFromHandle((short) nImg);
                if (image != null)
                {
                    sprRc.left -= image.getXSpot ();
                    sprRc.top -= image.getYSpot ();
                    nWidth = image.getWidth ();
                    nHeight = image.getHeight ();
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
        int dwWrapFlags = 0;
        int nSprite = 0;
        int nLOs = pLayer.nBkdLOs;

        // Optimization (only if no wrap)
        int nxz, nyz;
        //int nz;
        CArrayList<CArrayList<Integer>> pZones = null;
        CArrayList<Integer> pZone = null;
        nxz = nyz = 0;
        //nz = 0;
        if ( !bWrap )
        {
            // Get (or calculate) LO lists per zone (zone width = 512 x 512)
            pZones = app.run.getLayerZones(nLayer);

            // Get number of zones
            nxz = ((app.frame.leWidth + CRun.OBJZONE_WIDTH - 1)/ CRun.OBJZONE_WIDTH) + 2;
            nyz = ((app.frame.leHeight + CRun.OBJZONE_HEIGHT - 1)/ CRun.OBJZONE_HEIGHT) + 2;
            //nz = nxz * nyz;
        }

        int minzy, maxzy;
        minzy = maxzy = 0;
        if ( pZones != null )
        {
            if ( (sprRc.top + dyLayer) >= 0 )
                minzy = Math.min((sprRc.top + dyLayer) / CRun.OBJZONE_HEIGHT + 1, nyz-1);
            if ( (sprRc.bottom + dyLayer) >= 0 )
                maxzy = Math.min((sprRc.bottom + dyLayer) / CRun.OBJZONE_HEIGHT + 1, nyz-1);
        }
        for (int zy=minzy; zy<=maxzy; zy++)
        {
            int minzx, maxzx;
            minzx = maxzx = 0;
            if ( pZones != null )
            {
                if ( (sprRc.left + dxLayer) >= 0 )
                    minzx = Math.min((sprRc.left + dxLayer) / CRun.OBJZONE_WIDTH + 1, nxz-1);
                if ( (sprRc.right + dxLayer) >= 0 )
                    maxzx = Math.min((sprRc.right + dxLayer) / CRun.OBJZONE_WIDTH + 1, nxz-1);
            }
            for (int zx=minzx; zx<=maxzx; zx++)
            {
                if ( pZones != null )
                {
                    int z = zy * nxz + zx;
                    //ASSERT(z < nz);
                    pZone = pZones.get(z);
                    if ( pZone == null )
                    {
                        //NSLog(@"No zone: %i", z);
                        continue;
                    }
                    nLOs = pZone.size();
                }

                for (i = 0; i < nLOs; i++)
                {
                    CLO plo = LOList.getLOFromIndex((short) (pLayer.nFirstLOIndex + i));

                    if(pZone != null)
                        plo = app.frame.LOList.getLOFromIndex(pZone.get(i).shortValue());

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
                        CObjectCommon pCommon = (CObjectCommon) poc;
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
                        if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est crit dans les 2 masques...
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
                                // FRA: pSprMask = rhPtr.spriteGen.completeSpriteColMask(pSpr, dwPSCFlags, nWidth, nHeight);
                                if (pSprMask == null)
                                {
                                    return true;		// Can't calculate mask => box collision
                                }
                                //yMaskBits = 0;
                                //if (subHt != 0)
                                //{
                                //    if (subHt > nHeight)
                                //    {
                                //        subHt = nHeight;
                                //    }
                                //    yMaskBits = nHeight - subHt;
                                //}
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
        					//if ( (poi.oiLoadFlags & COI.OILF_ELTLOADED) == 0 )
        					//	poi.loadOnCall(app);

                            int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                            if (v == COC.OBSTACLE_PLATFORM)
                            {
                                nGetColMaskFlag = CMask.GCMF_PLATFORM;
                            }

                            // Get background mask
                            pBkdMask = null;
                            if (typeObj < COI.OBJ_SPR)
                            {
                                image = app.imageBank.getImageFromHandle(((COCBackground) poc).ocImage);
                                if(image != null)
                                	pBkdMask = image.getMask(nGetColMaskFlag, 0, 1.0, 1.0);
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
            }
        }

        // Scan Bkd2s
        if (pLayer.pBkd2 != null)
        {
            CBkd2 pbkd;

            dwWrapFlags = 0;
            nSprite = 0;
            int pLayer_pBkd2_size = pLayer.pBkd2.size();
            for (i = 0; i < pLayer_pBkd2_size; i++)
            {
                pbkd = pLayer.pBkd2.get(i);
                
                if(pbkd == null)
                	continue;
                
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
                    if ( /* (v == OBSTACLE_SOLID && nPlane == CM_TEST_PLATFORM) || */ // Non car un obstacle solide est crit dans les 2 masques...
                            (v == COC.OBSTACLE_PLATFORM && nPlane == CColMask.CM_TEST_OBSTACLE))
                    {
                        break;
                    }

                    // Get background sprite mask if no box collision
					if ( cm_box == 0 )
					{
                        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
                        if (v == COC.OBSTACLE_PLATFORM)
                        {
                            nGetColMaskFlag = CMask.GCMF_PLATFORM;
                        }

                        // Get background mask
                        image = app.imageBank.getImageFromHandle(pbkd.img);
                        if(image != null)
                        	pBkdMask = image.getMask(nGetColMaskFlag, 0, 1.0, 1.0);
					}

                    // Background sprite = Box?
                    if (cm_box != 0 || pBkdMask == null)
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
                                // FRA: pSprMask = rhPtr.spriteGen.completeSpriteColMask(pSpr, dwPSCFlags, nWidth, nHeight);
                                if (pSprMask == null)
                                {
                                    return true;		// Can't calculate mask => box collision
                                }
                                //yMaskBits = 0;
                                //if (subHt != 0)
                                //{
                                //    if (subHt > nHeight)
                                //    {
                                //        subHt = nHeight;
                                //    }
                                //    yMaskBits = nHeight - subHt;
                                //}
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
    public boolean bkdCol_TestPoint(int x, int y, int nLayer, int nPlane)
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
                //if (bkdLevObjCol_TestPoint(x, y, 0, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return bkdLevObjCol_TestPoint(x, y, 0, nPlane);
            }
            // Normal mode (no wrap mode, or windowed collision mask)
            else
            {
                if (colMask != null && colMask.testPoint(x, y, nPlane))
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

                return (rhPtr.spriteGen.spriteCol_TestPoint(null, (short) nLayer, x, y, dwFlags) != null);
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
                //if (bkdLevObjCol_TestPoint(x, y, 0, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return bkdLevObjCol_TestPoint(x, y, 0, nPlane);
            }
            // Normal mode (no wrap mode, or windowed collision mask)
            else
            {
                return colMask != null && colMask.testPoint(x, y, nPlane);
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

        return (rhPtr.spriteGen.spriteCol_TestPoint(null, CSpriteGen.LAYER_ALL, x, y, dwFlags) != null);
    }

    ////////////////////////////////////////////////
    //
    // Test collision with a rectangle
    //
    public boolean bkdCol_TestRect(int x, int y, int nWidth, int nHeight, int nLayer, int nPlane)
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
                //if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane);
            }
            // Normal mode (no wrap mode, or windowed collision mask)
            else
            {
                if (colMask != null && colMask.testRect(x, y, nWidth, nHeight, nPlane))
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
                //if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane);
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

                //if (rhPtr.spriteGen.spriteCol_TestRect(null, nLayer, x, y, nWidth, nHeight, dwFlags) != null)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return (rhPtr.spriteGen.spriteCol_TestRect(null, nLayer, x, y, nWidth, nHeight, dwFlags) != null);
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
                //if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //   return false;
                //}
                return bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane);
            }
            // Normal mode (no wrap mode, or windowed collision mask)
            else
            {
            	//if (colMask != null && colMask.testRect(x, y, nWidth, nHeight, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            	return  (colMask != null && colMask.testRect(x, y, nWidth, nHeight, nPlane));
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
            //if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane);
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

        return (rhPtr.spriteGen.spriteCol_TestRect(null, CSpriteGen.LAYER_ALL, x, y, nWidth, nHeight, dwFlags) != null);
    }

    public boolean bkdCol_TestSprite(CSprite pSpr, int newImg, int newX, int newY, float newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
    {
        // Get sprite layer
        int dwLayer = pSpr.sprLayer / 2;	// GetSpriteLayer(idEditWin, pSpr);

        // Layer 0
        CLayer pLayer;
        int dwFlags;
        if (dwLayer == 0)
        {
            // Wrap mode and full collision mask?
            pLayer = layers[0];
            if ((leFlags & LEF_TOTALCOLMASK) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_WRAP_HORZ | CLayer.FLOPT_WRAP_VERT)) != 0)
            {
                // Handle collisions like with the other layers (detect collisions with the objects, not with the collision mask)
                //if (bkdLevObjCol_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return bkdLevObjCol_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane);
            }
            // Normal mode (no wrap mode, or windowed collision mask)
            else
            {
                //if (colMask_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                return colMask_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane);
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
            //if (bkdLevObjCol_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return bkdLevObjCol_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane);
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

        return (rhPtr.spriteGen.spriteCol_TestSprite(pSpr, (short) newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, dwFlags) != null);
    }

    //-------------------------------------------------------------------------;
    //	Tester la collision d'un sprite avec le masque du fond d'une fenetre	;
    //-------------------------------------------------------------------------;
    boolean colMask_TestSprite(CSprite pSpr, int newImg, int newX, int newY, float newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
    {
        if (pSpr == null || colMask == null)
        {
            return false;
        }

        int nImg = newImg;
        int x1 = newX;
        int y1 = newY;
        int nColMode = rhPtr.spriteGen.colMode;
        int nWidth, nHeight;
        //CRect rc = new CRect();

        if (newImg == 0)
        {
            nImg = pSpr.sprImg;
        }

        // Bitmap collision?
        if (nColMode != CSpriteGen.CM_BOX && (pSpr.sprFlags & CSprite.SF_COLBOX) == 0)
        {
            // int dwPSCFlags = 0;
            CMask pMask = null;

            // Image sprite not stretched and not rotated, or owner draw sprite?

            pMask = rhPtr.spriteGen.getSpriteMask(pSpr, (short) nImg, CMask.GCMF_OBSTACLE | CMask.GCMF_FORCEMASK, newAngle, newScaleX, newScaleY);
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
	                    x1 -= pMask.getSpotX ();
	                    y1 -= pMask.getSpotY ();
	                }
	                nWidth = pMask.getWidth ();
	                nHeight = pMask.getHeight ();
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
                CImage pei = app.imageBank.getImageFromHandle((short) nImg);
                if (pei != null)
                {
                    x1 -= pei.getXSpot ();
                    y1 -= pei.getYSpot ();
                    nWidth = pei.getWidth ();
                    nHeight = pei.getHeight ();
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
