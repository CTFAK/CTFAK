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
// COILIST : liste des OI de l'application
//
//----------------------------------------------------------------------------------
package OI;

import Application.CRunApp;
import Banks.CImage;
import Banks.IEnum;
import RunLoop.CObjInfo;
import Runtime.MMFRuntime;
import Services.CFile;

public class COI 
{
    // Flags
    public static final short OILF_OCLOADED=0x0001;
    public static final short OILF_ELTLOADED=0x0002;
    public static final short OILF_TOLOAD=0x0004;
    public static final short OILF_TODELETE=0x0008;
    public static final short OILF_CURFRAME=0x0010;
    public static final short OILF_TORELOAD=0x0020;
    public static final short OILF_IGNORELOADONCALL=0x0040;
    public static final short OIF_LOADONCALL=0x0001;
    public static final short OIF_DISCARDABLE=0x0002;
    public static final short OIF_GLOBAL=0x0004;

    public static final short NUMBEROF_SYSTEMTYPES=7;
    public static final short OBJ_PLAYER=-7;
    public static final short OBJ_KEYBOARD=-6;
    public static final short OBJ_CREATE=-5;
    public static final short OBJ_TIMER=-4;
    public static final short OBJ_GAME=-3;
    public static final short OBJ_SPEAKER=-2;
    public static final short OBJ_SYSTEM=-1;
    public static final short OBJ_BOX=0;
    public static final short OBJ_BKD=1;
    public static final short OBJ_SPR=2;
    public static final short OBJ_TEXT=3;
    public static final short OBJ_QUEST=4;
    public static final short OBJ_SCORE=5;
    public static final short OBJ_LIVES=6;
    public static final short OBJ_COUNTER=7;
    public static final short OBJ_RTF=8;
    public static final short OBJ_CCA=9;
    public static final short NB_SYSOBJ=10;
    public static final short OBJ_LAST=10;
    public static final int KPX_BASE=32;
    public static final short OIFLAG_QUALIFIER=(short)0x8000;
    
    // objInfoHeader
    public short oiHandle=0;
    public short oiType=0;
    public short oiFlags=0;			/// Memory flags
//  public short oiReserved=0;			/// No longer used
    public int oiInkEffect=0;			/// Ink effect
    public int oiInkEffectParam=0;	        /// Ink effect param

    // OI
    public String oiName=null;			/// Name
    public COC oiOC=null;			/// ObjectsCommon
    public int oiFileOffset=0;
    public int oiLoadFlags=0;
    public short oiLoadCount=0;
    public short oiCount=0;
    
    //public boolean oiAntialias;
    
    public COI() 
    {
    }
    
    public void loadHeader(CFile file)
    {
		oiHandle=file.readAShort();
		oiType=file.readAShort();
		oiFlags=file.readAShort();
		file.skipBytes(2);
		oiInkEffect=file.readAInt();
		oiInkEffectParam=file.readAInt();
		
		// Handle load on call only for active objects
		if ( oiType != OBJ_SPR )
			oiFlags &= ~COI.OIF_LOADONCALL;
			
			
    }
    public void load(CFile file) 
    {
		// Positionne au debut
		file.seek(oiFileOffset);
		
		// En fonction du type
		switch (oiType)
		{
		    case 0:		// Quick background
			oiOC=new COCQBackdrop();
			break;
		    case 1:
			oiOC=new COCBackground();
			break;
		    default:
			oiOC=new CObjectCommon();
			break;
		}
		oiOC.load(file, oiType, this);
		oiLoadFlags=0;
    }
    public void unLoad()
    {
    	oiOC=null;
    	oiLoadFlags=0;
    }
    public void enumElements(IEnum enumImages, IEnum enumFonts)
    {
		if ( (oiFlags & OIF_LOADONCALL) == 0 )
		{
    		oiOC.enumElements(enumImages, enumFonts);
			oiLoadFlags |= OILF_ELTLOADED;
		}
    }
    public void loadOnCall(CRunApp app) 
    {
		if ( (oiLoadFlags & OILF_ELTLOADED) == 0 )
		{
			// Increment usecount of object's images and fonts (fonts is just a placeholder as only active objects may have the loadoncall option in this exporter)
			app.imageBank.copyUseCount();
			oiOC.enumElements(app.imageBank, app.fontBank);

			// Load unloaded images and fonts
			app.imageBank.loadUnloaded();

			// Set "elements are loaded" flag
			oiLoadFlags |= OILF_ELTLOADED;
		}
	}
    public void discard(CRunApp app) 
    {
		if ( (oiLoadFlags & OILF_ELTLOADED) != 0 )
		{
			// Increment usecount of object's images
			app.imageBank.copyUseCount();
			oiOC.enumElements(app.imageBank, null);

			// Unload loaded images
			app.imageBank.unloadLoaded();

			// Clear "elements are loaded" flag
			oiLoadFlags &= ~OILF_ELTLOADED;
		}
	}
}
