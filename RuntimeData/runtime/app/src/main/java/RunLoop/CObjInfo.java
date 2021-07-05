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

import OI.COI;
import OI.CObjectCommon;

public class CObjInfo 
{
    public static final short OILIMITFLAGS_BORDERS=0x000F;
    public static final short OILIMITFLAGS_BACKDROPS=0x0010;
    public static final short OILIMITFLAGS_ONCOLLIDE=0x0080;
    public static final short OILIMITFLAGS_QUICKCOL=0x0100;
    public static final short OILIMITFLAGS_QUICKBACK=0x0200;
    public static final short OILIMITFLAGS_QUICKBORDER=0x0400;
    public static final short OILIMITFLAGS_QUICKSPR=0x0800;
    public static final short OILIMITFLAGS_QUICKEXT=0x1000;
    public static final short OILIMITFLAGS_ALL=(short)0xFFFF;

    public static final int OIEFFECT_ANTIALIASED = 0x20000000;

    public short oilOi;  			/// THE oi
    public short oilListSelected;               /// First selection !!! DO NOT CHANGE POSITION !!!
    public short oilType;			/// Type of the object
    public short oilObject;			/// First objects in the game
    public int oilEvents;			/// Events
    public byte oilWrap;			/// WRAP flags
    public boolean oilNextFlag=false;
    public int oilNObjects;                     /// Current number
    public int oilActionCount;			/// Action loop counter
    public int oilActionLoopCount;              /// Action loop counter
    public int oilCurrentRoutine;               /// Current routine for the actions
    public int oilCurrentOi;			/// Current object
    public int oilNext;				/// Pointer on the next
    public int oilEventCount;			/// When the event list is done
    public int oilNumOfSelected;                /// Number of selected objects
    public int oilOEFlags;			/// Object's flags
    public short oilLimitFlags;			/// Movement limitation flags
    public int oilLimitList;                    /// Pointer to limitation list
    public short oilOIFlags;			/// Objects preferences
    public short oilOCFlags2;			/// Objects preferences II
    public int oilInkEffect;			/// Ink effect
    public int oilEffectParam;			/// Ink effect param
    public short oilHFII;			/// First available frameitem
    public int oilBackColor;			/// Background erasing color
    public short oilQualifiers[];               /// Qualifiers for this object
    public String oilName=null;                 /// Name	
    public int oilEventCountOR;                 /// Selection in a list of events with OR
    public short oilColList[];          /// List of objets that collide with this one in On Collide conditions (not in Is Overlapping conditions) (built at the end of assembleProgram)
    public boolean oilAntialias;		/// Added 291.4
    
    public CObjInfo() 
    {
    }
    public void copyData(COI oiPtr)
    {
        // Met dans l'OiList
    	oilOi=oiPtr.oiHandle;			
    	oilType=oiPtr.oiType;			

    	oilOIFlags=oiPtr.oiFlags;			
        CObjectCommon ocPtr=(CObjectCommon)oiPtr.oiOC;
        oilOCFlags2=ocPtr.ocFlags2;		
    	oilInkEffect=oiPtr.oiInkEffect;		
    	oilEffectParam=oiPtr.oiInkEffectParam;	
    	oilOEFlags=ocPtr.ocOEFlags;
    	oilBackColor=ocPtr.ocBackColor;			
    	oilEventCount=0;
    	oilObject=-1;
    	oilLimitFlags=OILIMITFLAGS_ALL;	
     	if ( oiPtr.oiName!=null )
        {
            oilName=new String(oiPtr.oiName);
        }
        int q;
        oilQualifiers=new short[8];
        for (q=0; q<8 && (ocPtr.ocQualifiers[q]&0x8000)==0; q++)
        {
            oilQualifiers[q]=ocPtr.ocQualifiers[q];
        }
        for (; q<8; q++)
        {
            oilQualifiers[q]=-1;
        }
		oilColList = null;
    }
}
