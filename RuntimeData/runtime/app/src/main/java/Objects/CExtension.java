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
// CEXTENSION: Objets d'extension
//
//----------------------------------------------------------------------------------
package Objects;

import Actions.CActExtension;
import Application.CRunApp;
import Banks.CImageBank;
import Conditions.CCndExtension;
import Events.CEventProgram;
import Expressions.CNativeExpInstance;
import Expressions.CValue;
import Extensions.CRunExtension;
import Extensions.CRunNativeExtension;
import Extensions.CRunViewExtension;
import Movements.CMoveDef;
import Movements.CMoveExtension;
import OI.CObjectCommon;
import Params.PARAM_PROGRAM;
import RunLoop.CCreateObjectInfo;
import RunLoop.CRun;
import Runtime.CrashReporter;
import Runtime.MMFRuntime;
import Services.CBinaryFile;
import Sprites.CMask;
import Sprites.CSprite;
import android.content.Context;

public class CExtension extends CObject
{
    public CRunExtension ext;
    boolean noHandle = false;
    public int privateData = 0;
    public int objectCount;
    public int objectNumber;
    public boolean bAntialias;    

    public CExtension(int type, CRun rhPtr)
    {  
    	ext = rhPtr.rhApp.extLoader.loadRunObject(type);

        if (ext != null)
            CrashReporter.addInfo("Extension", ext.getClass().getName());
        
    }

    public Context getControlsContext()
    {
    	return hoAdRunHeader.rhApp.controlView.getContext();
    }
    
    @Override
	public void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
    {
        // Initialisation des pointeurs
        ext.init(this);

        // Initialisation de l'objet
        CBinaryFile file = null;
        if (ocPtr.ocExtension != null)
        {
            file = new CBinaryFile(ocPtr.ocExtension, hoAdRunHeader.rhApp.bUnicode);
        }
        privateData = ocPtr.ocPrivate;

        ext.createRunObject(file, cob, ocPtr.ocVersion);
    }

   @Override
public void handle()
    {
        bAntialias = (MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0;
        // Routines standard;
        if ((hoOEFlags & 0x0200) != 0)	// OEFLAG_SPRITE
        {
            ros.handle();
        }
        else if ((hoOEFlags & 0x0030) == 0x0010 || (hoOEFlags & 0x0030) == 0x0030) // OEFLAG_MOVEMENTS / OEFLAG_ANIMATIONS|OEFLAG_MOVEMENTS
        {
            rom.move();
        }
        else if ((hoOEFlags & 0x0030) == 0x0020)	// OEFLAG_ANIMATION
        {
            roa.animate();
        }

        // Handle de l'objet
        int ret = 0;
        if (noHandle == false)
        {
            ret = ext.handleRunObject();
        }

        if ((ret & CRunExtension.REFLAG_ONESHOT) != 0)
        {
            noHandle = true;
        }
        if (roc != null)
        {
            if (roc.rcChanged)
            {
                ret |= CRunExtension.REFLAG_DISPLAY;
                roc.rcChanged = false;
            }
        }
        if ((ret & CRunExtension.REFLAG_DISPLAY) != 0)
        {
            modif();
        }
    }

    @Override
	public void modif()
    {
        if (ros != null)
        {
            ros.modifRoutine();
        }
        else if ((hoOEFlags & CObjectCommon.OEFLAG_BACKGROUND) != 0)
        {
            hoAdRunHeader.modif_RedrawLevel(this);
        }
        else
        {
        	bAntialias = (MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0;
            ext.displayRunObject();
        }
    }

    @Override
	public void display()
    {
    }

    @Override
	public void kill(boolean bFast)
    {
         ext.destroyRunObject(bFast);
         ext = null;
    }

    @Override
	public void getZoneInfos()
    {
    	ext.getZoneInfos();

        hoRect.left = hoX - hoAdRunHeader.rhWindowX - hoImgXSpot;			// Calcul des coordonnees
        hoRect.right = hoRect.left + hoImgWidth;
        hoRect.top = hoY - hoAdRunHeader.rhWindowY - hoImgYSpot;
        hoRect.bottom = hoRect.top + hoImgHeight;
    }

    @Override
	public CMask getCollisionMask(int flags)
    {
        return ext.getRunObjectCollisionMask(flags);
    }

    @Override
	public void draw()
    {
      /*  Bitmap img = ext.getRunObjectSurface();
        if (img != null)
        {
            c.drawBitmap(img, hoRect.top, hoRect.left, p);
        }
        else
        {
            ext.displayRunObject(renderer);
        } */
    	bAntialias = (MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0;
        ext.displayRunObject();
    }

    @Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
    {
        ext.displayRunObject();
    }

    @Override
	public CMask spriteGetMask()
    {
    	return ext.getRunObjectCollisionMask(CMask.GCMF_OBSTACLE);
    }

    public boolean condition(int num, CCndExtension cnd)
    {
    	return ext.condition(num, cnd);
    }

    public void action(int num, CActExtension act)
    {
    	ext.action(num, act);
    }

    public CValue expression(int num)
    {
		if (ext instanceof CRunNativeExtension)
		{
			CNativeExpInstance inst = new CNativeExpInstance(this);
			
			((CRunNativeExtension) ext).nativeExpression (num, inst);
			
			return inst.result;
		}
		else
		{
	    	return ext.expression(num);
		}
    }

    @Override
	public void reinitDisplay ()
    {
        ext.reinitDisplay ();
    }


    ////////////////////////////////////////////////////////////////////////
    // CALL BACKS
    ////////////////////////////////////////////////////////////////////////
    /* Returns the application object.
     */
    public CRunApp getApplication()
    {
        return hoAdRunHeader.rhApp;
    }
    /* Loads a list of images.
     */
    public void loadImageList(short[] list, boolean bAntialias)
    {
        hoAdRunHeader.rhApp.imageBank.loadImageList(list, bAntialias);
    }

    public void loadImageList(short[] list)
    {
		//Reading anti-aliased flag from object
        hoAdRunHeader.rhApp.imageBank.loadImageList(list, hoOiList.oilAntialias);
    }

    public CImageBank getImageBank ()
    {
        return hoAdRunHeader.rhApp.imageBank;
    }

    public int getX()
    {
        return hoX;
    }

    public int getY()
    {
        return hoY;
    }

    public int getWidth()
    {
        return hoImgWidth;
    }

    public int getHeight()
    {
        return hoImgHeight;
    }

    public void setX(int x)
    {
        if (rom != null)
        {
            rom.rmMovement.setXPosition(x);
        }
        else
        {
            hoX = x;
            if (roc != null)
            {
                roc.rcChanged = true;
                roc.rcCheckCollides = true;
            }
        }

        if (ext instanceof CRunViewExtension)
            ((CRunViewExtension) ext).setViewX (x);
    }

    public void setY(int y)
    {
        if (rom != null)
        {
            rom.rmMovement.setYPosition(y);
        }
        else
        {
            hoY = y;
            if (roc != null)
            {
                roc.rcChanged = true;
                roc.rcCheckCollides = true;
            }
        }

        if (ext instanceof CRunViewExtension)
            ((CRunViewExtension) ext).setViewY (y);
    }

    public void setWidth(int width)
    {
        hoImgWidth = width;
        hoRect.right = hoRect.left + width;

        if (ext instanceof CRunViewExtension)
            ((CRunViewExtension) ext).setViewWidth (hoImgWidth);
    }

    public void setHeight(int height)
    {
        hoImgHeight = height;
        hoRect.bottom = hoRect.top + height;

        if (ext instanceof CRunViewExtension)
            ((CRunViewExtension) ext).setViewHeight (hoImgHeight);
    }

    public void setSize(int width, int height)
    {
        hoImgWidth = width;
        hoImgHeight = height;

        hoRect.right = hoRect.left + width;
        hoRect.bottom = hoRect.top + height;

        if (ext instanceof CRunViewExtension)
            ((CRunViewExtension) ext).setViewSize (hoImgWidth, hoImgHeight);
    }

    // added 285.2 to avoid comparing using 4 jump e.g: boxA or boxB
    public boolean isInBounds(int left, int top, int right, int bottom) {
    	if ((left >= hoX) && 
    			(right <= hoX + hoImgWidth) && 
    			(top >= hoY) && 
    			(bottom <= hoY + hoImgHeight))
    	{
    		return true;
    	}
    	return false;
    }

    // added 285.2 to avoid comparing using 4 jump e.g: boxA or boxB
    public boolean isPointInBoundsRelative(int x, int y) {
    	if ((x >= hoX - hoAdRunHeader.rhWindowX) &&
    			(x <= (hoX - hoAdRunHeader.rhWindowX + hoImgWidth)) &&
    			(y >= (hoY - hoAdRunHeader.rhWindowY)) &&
    			(y <= (hoY - hoAdRunHeader.rhWindowY + hoImgHeight)))
    	{
    		return true;
    	}
    	return false;
    }
    
    public int scaleX(int x)
    {
        return hoAdRunHeader.scaleX(x);
    }

    public int scaleY(int y)
    {
        return hoAdRunHeader.scaleY(y);
    }

    public void reHandle()
    {
        noHandle = false;
    }

    public void generateEvent(int code, int param)
    {
    	synchronized(hoAdRunHeader)
	    {
	        if (hoAdRunHeader.rh2PauseCompteur == 0)
	        {
	            int p0 = hoAdRunHeader.rhEvtProg.rhCurParam0;
	            hoAdRunHeader.rhEvtProg.rhCurParam0 = param;
	
	            code = (-(code + CEventProgram.EVENTS_EXTBASE + 1) << 16);
	            code |= ((hoType) & 0xFFFF);
	            hoAdRunHeader.rhEvtProg.handle_Event(this, code);
	
	            hoAdRunHeader.rhEvtProg.rhCurParam0 = p0;
	        }
    	}
    }

    public void pushEvent(int code, int param)
    {
        if (hoAdRunHeader.rh2PauseCompteur == 0)
        {
            code = (-(code + CEventProgram.EVENTS_EXTBASE + 1) << 16);
            code |= ((hoType) & 0xFFFF);
            hoAdRunHeader.rhEvtProg.push_Event(1, code, param, this, hoOi);
        }
    }

    //
    //  Add Event during pause and for activity Result
    //  285.2
    //
    public void activityEvent(int code, int param)
    {
        if (hoAdRunHeader.rh2PauseCompteur != 0)
        {
            code = (-(code + CEventProgram.EVENTS_EXTBASE + 1) << 16);
            code |= ((hoType) & 0xFFFF);
            hoAdRunHeader.rhEvtProg.push_Event(1, code, param, this, hoOi);
        }
    }

    public void pause()
    {
        hoAdRunHeader.pause();
    }

    public void resume()
    {
        hoAdRunHeader.resume();
    }

    public void redisplay()
    {
        hoAdRunHeader.ohRedrawLevel(true);
    }

    public void redraw()
    {
        modif();
        if ((hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0)
        {
            roc.rcChanged = true;
        }
    }

    public void destroy()
    {
        hoAdRunHeader.destroy_Add(hoNumber);
    }

    public void execProgram(String filename, String commandLine, short flags)
    {
        String command[] = new String[2];
        command[0] = filename;
        command[1] = commandLine;
        try
        {
            Process process = Runtime.getRuntime().exec(command);
            if ((flags & PARAM_PROGRAM.PRGFLAGS_WAIT) != 0)
            {
                hoAdRunHeader.pause();

                if ((flags & PARAM_PROGRAM.PRGFLAGS_HIDE) != 0)
                {
                    // hoAdRunHeader.rhApp.window.setVisible(false);
                }
                try
                {
                    process.waitFor();
                }
                catch (InterruptedException e)
                {
                }
                if ((flags & PARAM_PROGRAM.PRGFLAGS_HIDE) != 0)
                {
                    // hoAdRunHeader.rhApp.window.setVisible(true);
                }

                hoAdRunHeader.resume();
            }
        }
        catch (Throwable e)
        {
        }
    }

    public void setPosition(int x, int y)
    {
        if (rom != null)
        {
            rom.rmMovement.setXPosition(x);
            rom.rmMovement.setYPosition(y);
        }
        else
        {
            hoX = x;
            hoY = y;
            if (roc != null)
            {
                roc.rcChanged = true;
                roc.rcCheckCollides = true;
            }
        }

        if (ext instanceof CRunViewExtension)
            ((CRunViewExtension) ext).setViewPosition (x, y);
    }

    public int getExtUserData()
    {
        return privateData;
    }

    public void setExtUserData(int data)
    {
        privateData = data;
    }

/*    public void addBackdrop(Bitmap img, int x, int y, int dwEffect, int dwEffectParam, int typeObst, int nLayer)
    {
        // Duplique et ajoute l'image
        int width = img.getWidth();
        int height = img.getHeight();
        Bitmap newImg = Bitmap.createBitmap(img);
        short handle = hoAdRunHeader.rhApp.imageBank.addImageCompare(newImg, (short) 0, (short) 0, (short) 0, (short) 0);

        // Ajoute a la liste
        CBkd2 toadd = new CBkd2();
        toadd.img = handle;
        toadd.loHnd = 0;
        toadd.oiHnd = 0;
        toadd.x = x;
        toadd.y = y;
        toadd.width = todo;
        toadd.height = todo;
        toadd.nLayer = (short) nLayer;
        toadd.inkEffect = dwEffect;
        toadd.inkEffectParam = dwEffectParam;
        toadd.colMode = CSpriteGen.CM_BITMAP;
        toadd.obstacleType = (short) typeObst;	// a voir
        for (int ns = 0; ns < 4; ns++)
        {
            toadd.pSpr[ns] = null;
        }
        hoAdRunHeader.addBackdrop2(toadd);

        // Add paste routine (pour �viter d'avoir � r�afficher tout le d�cor)
        if (nLayer == 0 && (hoAdRunHeader.rhFrame.layers[0].dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) == CLayer.FLOPT_VISIBLE)
        {
            CBackDrawPaste paste;
            paste = new CBackDrawPaste();
            paste.img = handle;
            paste.x = x;
            paste.y = y;
            paste.typeObst = (short) typeObst;
            paste.inkEffect = dwEffect;
            paste.inkEffectParam = dwEffectParam;
            hoAdRunHeader.addBackDrawRoutine(paste);

            // Redraw sprites that intersect with the rectangle
            CRect rc = new CRect();
            rc.left = x - hoAdRunHeader.rhWindowX;
            rc.top = y - hoAdRunHeader.rhWindowY;
            rc.right = rc.left + width;
            rc.bottom = rc.top + height;
            hoAdRunHeader.spriteGen.activeSprite(null, CSpriteGen.AS_REDRAW_RECT, rc);
        }
    }   */

    public int getEventCount()
    {
        return hoAdRunHeader.rh4EventCount;
    }

    public CValue getExpParam()
    {
        hoAdRunHeader.rh4CurToken++;		// Saute la fonction
        return hoAdRunHeader.getExpression();
    }

    public int getEventParam()
    {
        return hoAdRunHeader.rhEvtProg.rhCurParam0;
    }

    public double callMovement(CObject hoPtr, int action, double param)
    {
        if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0)
        {
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvPtr = (CMoveExtension) hoPtr.rom.rmMovement;
                return mvPtr.callMovement(action, param);
            }
        }
        return 0;
    }

    public CValue callExpression(CObject hoPtr, int action, int param)
    {
        CExtension pExtension=(CExtension)hoPtr;
        pExtension.privateData=param;
        return pExtension.expression(action);
    }

    public int getExpressionParam()
    {
        return privateData;
    }

    public CObject getFirstObject()
    {
        objectCount = 0;
        objectNumber = 0;
        return getNextObject();
    }

    public CObject getNextObject()
    {
        if (objectNumber < hoAdRunHeader.rhNObjects)
        {
            while (hoAdRunHeader.rhObjectList[objectCount] == null)
            {
                objectCount++;
            }
            CObject hoPtr = hoAdRunHeader.rhObjectList[objectCount];
            objectCount++;
			objectNumber++;		// Build 284.2, bug from crash reports, fixed after comparing IOS/SWF source code
            return hoPtr;
        }
        return null;
    }

    public CObject getObjectFromFixed(int fixed)
    {
        int count = 0;
        int number;
        for (number = 0; number < hoAdRunHeader.rhNObjects; number++)
        {
            while (hoAdRunHeader.rhObjectList[count] == null)
            {
                count++;
            }
            CObject hoPtr = hoAdRunHeader.rhObjectList[count];
            count++;
            int id = (hoPtr.hoCreationId << 16) | ((hoPtr.hoNumber) & 0xFFFF);
            if (id == fixed)
            {
                return hoPtr;
            }
        }
        return null;
    }

    public CRunApp.HFile openHFile(String path, boolean enabled_perms)
    {
        return hoAdRunHeader.rhApp.openHFile(path, enabled_perms);
    }

    public void retrieveHFile(String path, CRunApp.FileRetrievedHandler handler)
    {
        hoAdRunHeader.rhApp.retrieveHFile(path, handler);
    }

    public void closeHFile (CRunApp.HFile file)
    {
    	file.close();
    }

    @Override
	public void spriteKill (CSprite spr)
    {
    }
        
}
