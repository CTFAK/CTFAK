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
// CCCA : Objet sub-application
//
//----------------------------------------------------------------------------------
package Objects;

import Application.CRunApp;
import Banks.CImageBank;
import Expressions.CValue;
import OI.CDefCCA;
import OI.CObjectCommon;
import OpenGL.GLRenderer;
import RunLoop.CCreateObjectInfo;
import RunLoop.CRun;
import Runtime.Log;
import Sprites.CMask;
import Sprites.CSprite;
import android.view.View;

public class CCCA extends CObject
{
    public static final int CCAF_SHARE_GLOBALVALUES = 0x00000001;
    public static final int CCAF_SHARE_LIVES = 0x00000002;
    public static final int CCAF_SHARE_SCORES = 0x00000004;
    public static final int CCAF_SHARE_WINATTRIB = 0x00000008;
    public static final int CCAF_STRETCH = 0x00000010;
    public static final int CCAF_POPUP = 0x00000020;
    public static final int CCAF_CAPTION = 0x00000040;
    public static final int CCAF_TOOLCAPTION = 0x00000080;
    public static final int CCAF_BORDER = 0x00000100;
    public static final int CCAF_WINRESIZE = 0x00000200;
    public static final int CCAF_SYSMENU = 0x00000400;
    public static final int CCAF_DISABLECLOSE = 0x00000800;
    public static final int CCAF_MODAL = 0x00001000;
    public static final int CCAF_DIALOGFRAME = 0x00002000;
    public static final int CCAF_INTERNAL = 0x00004000;
    public static final int CCAF_HIDEONCLOSE = 0x00008000;
    public static final int CCAF_CUSTOMSIZE = 0x00010000;
    public static final int CCAF_INTERNALABOUTBOX = 0x00020000;
    public static final int CCAF_CLIPSIBLINGS = 0x00040000;
    public static final int CCAF_SHARE_PLAYERCTRLS = 0x00080000;
    public static final int CCAF_MDICHILD = 0x00100000;
    public static final int CCAF_DOCKED = 0x00200000;
    public static final int CCAF_DOCKING_AREA = 0x00C00000;
    public static final int CCAF_DOCKED_LEFT = 0x00000000;
    public static final int CCAF_DOCKED_TOP = 0x00400000;
    public static final int CCAF_DOCKED_RIGHT = 0x00800000;
    public static final int CCAF_DOCKED_BOTTOM = 0x00C00000;
    public static final int CCAF_REOPEN = 0x01000000;
    public static final int CCAF_MDIRUNEVENIFNOTACTIVE = 0x02000000;
    public static final int CCAF_HIDDENATSTART = 0x04000000;
    int flags = 0;
    int odOptions = 0;
    public CRunApp subApp = null;
    int oldX;
    int oldY;
    int oldWidth;
    int oldHeight;
    int level = 0;
    int oldLevel = 0;
    boolean bVisible;
    
    public CCCA()
    {
    }

    void startCCA(String filename, CObjectCommon ocPtr, boolean bInit, int nStartFrame)
    {
    	Log.Log("Start subapp");

        CDefCCA defCCA = (CDefCCA) ocPtr.ocObject;

        hoImgWidth = defCCA.odCx;
        hoImgHeight = defCCA.odCy;
        odOptions = defCCA.odOptions;

        //odOptions &= ~ CCAF_MODAL;  // currently ignored
        
        // Stretch? force custom size option
        if ((odOptions & CCAF_STRETCH) != 0)
        {
            odOptions |= CCAF_CUSTOMSIZE;
        }

        // Popup? transform coordinates to screen coordinates
        if (bInit == true && (odOptions & CCAF_POPUP) != 0)
        {
            if (hoAdRunHeader.rhApp != null)
            {
                hoX += hoAdRunHeader.rhWindowX;
                hoY += hoAdRunHeader.rhWindowY;
            }
        }

        // Get start frame
        if ( nStartFrame == -1 )
        {
            nStartFrame = 0;
            if ( (odOptions&CCAF_INTERNAL)!=0 )
            {
            nStartFrame = defCCA.odNStartFrame;
            }
        }
        
         // Empty fileName? do not create the object, unless it's an internal sub-application
        if (filename==null)
        {
            filename=defCCA.odName;
        }
        // Change l'extension
        filename=setCCJ(filename);
        if ( filename==null || (filename!=null && filename.length()==0) )
        {
            if ( (odOptions&CCAF_INTERNAL)==0 )
            {
                return;
            }
            // Internal frame, check it exists and is different from the current one
            if ( nStartFrame >= hoAdRunHeader.rhApp.gaNbFrames )
            {
                return;
            }
            if ( nStartFrame==hoAdRunHeader.rhApp.currentFrame )
            {
                return;
            }
        }

        bVisible = true;
        
        odOptions &= ~CCCA.CCAF_HIDDENATSTART;
        if ((ocPtr.ocFlags2 & CObjectCommon.OCFLAGS2_VISIBLEATSTART) == 0)
        {
            odOptions |= CCCA.CCAF_HIDDENATSTART;
            bVisible = false;
        }

        if ((odOptions&CCAF_MODAL)!=0)
        {
            if(hoAdRunHeader.rhApp.modalSubApp == null)
                hoAdRunHeader.rhApp.modalSubApp = this;
         }

        subApp = new CRunApp();
        subApp.setParentApp(hoAdRunHeader.rhApp, nStartFrame, odOptions, this);
        subApp.createControlView();

        subApp.gaCxWin = hoImgWidth;
        subApp.gaCyWin = hoImgHeight;
        
        subApp.updateWindowDimensions(hoImgWidth, hoImgHeight);
        
        oldX = hoX;
        oldY = hoY;
        oldWidth = -1;
        oldHeight = -1;

        // Chargement de l'application

        if ( (odOptions&CCAF_INTERNAL)==0 )
        {
            subApp.load(filename);
        }
        else
        {
            subApp.load (null);
        }

        if (subApp.startApplication() == false)
        {
            subApp.endApplication();
            subApp = null;
            return;
        }
        if (subApp.playApplication(true) == false)
        {
            subApp.endApplication();
            subApp = null;
            return;
        }
    }

    @Override
	public void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
    {
     //   if (hoAdRunHeader.rhApp.m_bLoading == false)
     //   {
            startCCA(null, ocPtr, true, -1);
      //  }
    }

    String setCCJ(String filename)
    {
        // Si le nom de fichier se termine par .ccn, remplace par .ccj
        String name2=filename.toLowerCase();
        if (name2.endsWith(".ccn"))
        {
            name2=filename.substring(0, filename.length()-3)+"ccj";
            filename=name2;
        }
        return filename;
    }

    int nWindowUpdate = -1;
    		
    @Override
	public void handle()
    {
        rom.move();

        if (subApp != null)
        {
            if (oldX != hoX || oldY != hoY || oldWidth != hoImgWidth || oldHeight != hoImgHeight
            			|| nWindowUpdate != hoAdRunHeader.rhApp.nWindowUpdate)
            {
                subApp.updateWindowDimensions(hoImgWidth, hoImgHeight);
                
                if (hoImgWidth!=oldWidth || hoImgHeight!=oldHeight)
                {
                    //subApp.updateWindowDimensions(hoImgWidth, hoImgHeight);
                    subApp.changeWindowDimensions(hoImgWidth, hoImgHeight);
                    oldWidth = hoImgWidth;
                    oldHeight = hoImgHeight;
                }
                subApp.updateWindowPos();
                nWindowUpdate = hoAdRunHeader.rhApp.nWindowUpdate;
                
                // subApp.setPosition(hoX, hoY);
                oldX = hoX;
                oldY = hoY;
            }
            //if ((odOptions & CCAF_MODAL) == 0)
            //{
                if (subApp.playApplication(false) == false)
                {
                    subApp.endApplication();
                    subApp = null;

                    if ( (odOptions&CCAF_MODAL)!=0 )
                    {
                        if(hoAdRunHeader.rhApp.modalSubApp == this)
                            hoAdRunHeader.rhApp.modalSubApp = null;
                    }
                    return;
                }

                oldLevel = level;
                level = subApp.currentFrame;
            //}
            /*
            else
            {
            	
                if (subApp.bVisible == true)
                {
                    //hoAdRunHeader.pause();
                    boolean bContinue;

                    do
                    {
                        bContinue = subApp.playApplication(false);
                    } while (bContinue == true && subApp.bVisible == true);

                    if (!bContinue)
                    {
                        subApp.endApplication();
                        subApp = null;
                    }
                    hoAdRunHeader.resume();
                }
                
            }*/
        }
        else
        	nWindowUpdate = hoAdRunHeader.rhApp.nWindowUpdate  = 0;
        
    }

    public void setDockedPosition()
    {
        if (subApp != null)
        {
            if ((subApp.parentOptions & CCAF_DOCKED) != 0)
            {
             //   subApp.setPosition(0, 0);
            }
        }
    }

    // Envoie une evenement clavier ou souris   la sous application
  /*  public void sendKey(int keyCode, boolean bState)
    {
        if (subApp != null)
        {
            if (subApp.displayType == 3)	    // DISPLAY_PANEL
            {
                subApp.receiveKey(keyCode, bState);
            }
        }
    } */

    @Override
	public void kill(boolean bFast)
    {
        if (subApp != null)
        {
            // End of current frame
            switch (subApp.appRunningState)
            {
                // Frame fade-in loop
                case 2:	    // SL_FRAMEFADEINLOOP:
                    if (subApp.loopFrameFadeIn() == false)
                    {
                        subApp.endFrameFadeIn();
                        subApp.endFrame();
                    }
                    break;

                // Frame loop
                case 3:	    // SL_FRAMELOOP:
                    subApp.endFrame();
                    break;

                // Frame fade-out loop
                case 4:	    // SL_FRAMEFADEOUTLOOP:
                    subApp.endFrameFadeOut();
                    break;
            }

             // End of application
            subApp.endApplication();
            subApp = null;

        }
        if ( (odOptions&CCAF_MODAL)!=0 )
        {
            if(hoAdRunHeader.rhApp.modalSubApp == this)
                hoAdRunHeader.rhApp.modalSubApp = null;
        }

    }

    public void restartApp()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.rhQuit = CRun.LOOPEXIT_NEWGAME;
                return;
            }
            kill(true);
        }
        startCCA(null, hoCommon, false, -1);
    }

    public void endApp()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.rhQuit = CRun.LOOPEXIT_ENDGAME;
            }
        }
        
   }

    public void hide()
    {
        if (subApp != null) {
    	    subApp.bVisible = false;
    	    bVisible = false;
    	    this.ros.obHide();
    	    subApp.controlView.setVisibility(View.INVISIBLE);
    	}

    }

    public void show()
    {
        if (subApp != null) {
            subApp.bVisible = true;
    	    bVisible = true;
    	    this.ros.obShow();
            subApp.controlView.setVisibility(View.VISIBLE);
    	}

    }

    public void jumpFrame(int frame)
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                if (frame>=0 && frame<4096)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_GOTOLEVEL;
                    subApp.run.rhQuitParam = 0x8000 | frame;
                }
            }
        }
    }

    public void nextFrame()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.rhQuit = CRun.LOOPEXIT_NEXTLEVEL;
            }
        }
    }

    public void previousFrame()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.rhQuit = CRun.LOOPEXIT_PREVLEVEL;
            }
        }
    }

    public void restartFrame()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.rhQuit = CRun.LOOPEXIT_RESTART;
            }
        }
    }

    public void pause()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.pause();
            }
        }
    }

    public void resume()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                subApp.run.resume();
            }
        }
    }

    public void setGlobalValue(int number, CValue value)
    {
        if (subApp != null)
        {
            subApp.setGlobalValueAt(number, value);
        }
    }

    public void setWidth(int width)
    {
        if (subApp!=null)
        {
            hoImgWidth=width;
        }
    }

    public void setHeight(int height)
    {
        if (subApp!=null)
        {
            hoImgHeight=height;
        }
    }

    public void setGlobalString(int number, String value)
    {
        if (subApp != null)
        {
            subApp.setGlobalStringAt(number, value);
        }
    }

    public boolean isPaused()
    {
        if (subApp != null)
        {
            if (subApp.run != null)
            {
                return subApp.run.rh2PauseCompteur != 0;
            }
        }
        return false;
    }

    public boolean appFinished()
    {
        return subApp == null;
    }

    public boolean isVisible()
    {
        if (subApp != null)
        {
            return subApp.bVisible;
        }
        return false;
    }

    public boolean frameChanged()
    {
        return level != oldLevel;
    }

    public String getGlobalString(int num)
    {
        if (subApp != null)
        {
            return subApp.getGlobalStringAt(num);
        }
        return "";
    }

    public CValue getGlobalValue(int num)
    {
        if (subApp != null)
        {
            return subApp.getGlobalValueAt(num);
        }
        return new CValue(0);
    }

    public int getFrameNumber()
    {
        return level + 1;
    }

  /*  public void bringToFront()
    {
        if (subApp != null)
        {
            if (subApp.displayType == 0 || subApp.displayType == 1)
            {
                subApp.window.toFront();
            }
        }
    } */

    /*   public void writePosition(DataOutputStream s)
    {
        boolean bStarted = false;
        if (subApp != null && subApp.run != null)
        {
            bStarted = true;
        }
        s.writeBoolean(bStarted);
        if (bStarted)
        {
            if (subApp.run.saveFramePosition(s) == false)
            {
                throw new IOException()
            }
        }
    }

    public void readPosition(DataInputStream s)
    {
        boolean bStarted = s.readBoolean();
        if (bStarted)
        {
            startCCA(null, hoCommon, true, -1);
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    if (subApp.run.loadFramePosition(s) == false)
                    {
                        throw new IOException();
                    }
                }
            }
        }
    } */
    @Override
	public void getZoneInfos()
    {
    }

    @Override
	public void draw()
    {
        if (subApp != null && subApp.run!=null)
        {
            if(bVisible) {
	        	GLRenderer.inst.pushClipAndBase(hoX - hoAdRunHeader.rhWindowX,
	                    hoY - hoAdRunHeader.rhWindowY,
	                    hoImgWidth, hoImgHeight);
	
	            subApp.run.screen_Update();
	
	            GLRenderer.inst.popClipAndBase ();
            }
        }
        
    }

    @Override
	public CMask getCollisionMask(int flags)
    {
        return null;
    }

    @Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
    {
        draw();
    }
    
    @Override
	public CMask spriteGetMask()
    {
        return null;
    }
    @Override
	public void spriteKill (CSprite spr)
    {
    }
}
