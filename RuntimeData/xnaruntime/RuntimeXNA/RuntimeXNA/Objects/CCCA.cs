//----------------------------------------------------------------------------------
//
// CCCA : Objet sub-application
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Expressions;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects
{
    class CCCA : CObject
    {
        public const int CCAF_SHARE_GLOBALVALUES = 0x00000001;
        public const int CCAF_SHARE_LIVES = 0x00000002;
        public const int CCAF_SHARE_SCORES = 0x00000004;
        public const int CCAF_SHARE_WINATTRIB = 0x00000008;
        public const int CCAF_STRETCH = 0x00000010;
        public const int CCAF_POPUP = 0x00000020;
        public const int CCAF_CAPTION = 0x00000040;
        public const int CCAF_TOOLCAPTION = 0x00000080;
        public const int CCAF_BORDER = 0x00000100;
        public const int CCAF_WINRESIZE = 0x00000200;
        public const int CCAF_SYSMENU = 0x00000400;
        public const int CCAF_DISABLECLOSE = 0x00000800;
        public const int CCAF_MODAL = 0x00001000;
        public const int CCAF_DIALOGFRAME = 0x00002000;
        public const int CCAF_INTERNAL = 0x00004000;
        public const int CCAF_HIDEONCLOSE = 0x00008000;
        public const int CCAF_CUSTOMSIZE = 0x00010000;
        public const int CCAF_INTERNALABOUTBOX = 0x00020000;
        public const int CCAF_CLIPSIBLINGS = 0x00040000;
        public const int CCAF_SHARE_PLAYERCTRLS = 0x00080000;
        public const int CCAF_MDICHILD = 0x00100000;
        public const int CCAF_DOCKED = 0x00200000;
        public const int CCAF_DOCKING_AREA = 0x00C00000;
        public const int CCAF_DOCKED_LEFT = 0x00000000;
        public const int CCAF_DOCKED_TOP = 0x00400000;
        public const int CCAF_DOCKED_RIGHT = 0x00800000;
        public const int CCAF_DOCKED_BOTTOM = 0x00C00000;
        public const int CCAF_REOPEN = 0x01000000;
        public const int CCAF_MDIRUNEVENIFNOTACTIVE = 0x02000000;
        public const int CCAF_HIDDENATSTART = 0x04000000;

        internal int flags = 0;
        internal int odOptions = 0;
        internal CRunApp subApp = null;
        internal int level = 0;
        internal int oldLevel = 0;
        bool bPaused = false;

	    public void startCCA(CObjectCommon ocPtr, bool bInit, int nStartFrame)
	    {
	        CDefCCA defCCA= (CDefCCA)ocPtr.ocObject;
	
	        hoImgWidth = defCCA.odCx;
	        hoImgHeight = defCCA.odCy;
	        odOptions = defCCA.odOptions;
	
	        // Stretch? force custom size option
	        if ((odOptions & CCAF_STRETCH) != 0)
	        {
	            odOptions |= CCAF_CUSTOMSIZE;
	        }
	
	        // Get start frame
	        if (nStartFrame == -1)
	        {
	            nStartFrame = 0;
	            if ((odOptions & CCAF_INTERNAL) != 0)
	            {
	                nStartFrame = defCCA.odNStartFrame;
	            }
	        }
	
	        // Change l'extension
	        if (defCCA.odName==null || defCCA.odName.Length!=0)
	        {
	        	return;
	        }
            if ((odOptions & CCAF_INTERNAL) == 0)
            {
                return;
            }
            // Internal frame, check it exists and is different from the current one
            if (nStartFrame >= hoAdRunHeader.rhApp.gaNbFrames)
            {
                return;
            }
            if (nStartFrame == hoAdRunHeader.rhApp.currentFrame)
            {
                return;
            }
	
	        // Starts the application
            CFile file = new CFile(hoAdRunHeader.rhApp.file);
	        subApp = new CRunApp(hoAdRunHeader.rhApp.game, file);
            subApp.setParentApp(hoAdRunHeader.rhApp, nStartFrame, odOptions, hoX - hoAdRunHeader.rhWindowX, hoY - hoAdRunHeader.rhWindowY, hoImgWidth, hoImgHeight);
            subApp.showSubApp((ocPtr.ocFlags2 & CObjectCommon.OCFLAGS2_VISIBLEATSTART) != 0);

            // Chargement de l'application
            subApp.load();
            subApp.startApplication();
            subApp.playApplication(true, hoAdRunHeader.rhApp.timeDouble);
            hoAdRunHeader.nSubApps++;
	    }

	    public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	    {
            startCCA(ocPtr, true, -1);
	    }

	    public override void handle()
	    {
	        rom.move();
	        if (subApp != null)
	        {
                subApp.setOffsets(hoX-hoAdRunHeader.rhWindowX, hoY-hoAdRunHeader.rhWindowY);
                if (subApp.playApplication(false, hoAdRunHeader.rhApp.timeDouble) == false)
                {
                    subApp.endApplication();
                    subApp = null;
                    return;
                }
                oldLevel = level;
                level = subApp.currentFrame;
	        }
	    }

	    public override void kill(bool bFast)
	    {
	        if (subApp != null)
	        {
	            // End of current frame
	            switch (subApp.appRunningState)
	            {
	                // Frame loop
	                case 3:	    // SL_FRAMELOOP:
	                    subApp.endFrame();
	                    break;
	            }
	
	            // End of application
	            subApp.endApplication();
	            subApp = null;
                hoAdRunHeader.nSubApps--;
            }
	    }

        public virtual void restartApp()
        {
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_NEWGAME;
                    return;
                }
                kill(true);
                hoAdRunHeader.nSubApps--;
            }
            startCCA(hoCommon, false, -1);
        }

        public virtual void endApp()
        {
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_ENDGAME;
                }
            }
        }

        public virtual void hide()
        {
            if (subApp != null)
            {
                subApp.showSubApp(false);
            }
        }

        public virtual void show()
        {
            if (subApp != null)
            {
                subApp.showSubApp(true);
            }
        }

        public virtual void jumpFrame(int frame)
        {
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_GOTOLEVEL;
                    subApp.run.rhQuitParam = 0x8000 | frame;
                }
            }
        }
        public virtual void nextFrame()
        {
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_NEXTLEVEL;
                }
            }
        }
        public virtual void previousFrame()
        {
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_PREVLEVEL;
                }
            }
        }
        public virtual void restartFrame()
        {
            if (subApp != null)
            {
                if (subApp.run != null)
                {
                    subApp.run.rhQuit = CRun.LOOPEXIT_RESTART;
                }
            }
        }

        public virtual void pause()
        {
            if (subApp != null)
            {
                bPaused = true;
                if (subApp.run != null)
                {
                    subApp.run.pause();
                }
            }
        }
        public virtual void resume()
        {
            if (subApp != null)
            {
                bPaused = false;
                if (subApp.run != null)
                {
                    subApp.run.resume();
                }
            }
        }
        public virtual void setGlobalValue(int number, CValue value_Renamed)
        {
            if (subApp != null)
            {
                subApp.setGlobalValueAt(number, value_Renamed);
            }
        }
        public virtual void setGlobalString(int number, System.String value_Renamed)
        {
            if (subApp != null)
            {
                subApp.setGlobalStringAt(number, value_Renamed);
            }
        }
        public virtual bool appFinished()
        {
            return subApp == null;
        }
        public virtual bool frameChanged()
        {
            return level != oldLevel;
        }
        public virtual System.String getGlobalString(int num)
        {
            if (subApp != null)
            {
                return subApp.getGlobalStringAt(num);
            }
            return "";
        }
        public virtual CValue getGlobalValue(int num)
        {
            if (subApp != null)
            {
                return subApp.getGlobalValueAt(num);
            }
            return new CValue(0);
        }
        public bool isVisible()
        {
            if (subApp != null)
            {
                return subApp.bSubAppShown;
            }
            return false;
        }
        public bool isPaused()
        {
            return bPaused;
        }

        public override void draw(SpriteBatchEffect batch)
        {
            if (subApp != null && subApp.run!=null)
            {
                subApp.run.screen_Update();
            }
        }
    }
}
