//----------------------------------------------------------------------------------
//
// COBJINFO: information sur un type d'objet
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.OI;

namespace RuntimeXNA.RunLoop
{
    public class CObjInfo
    {
        public const short OILIMITFLAGS_BORDERS = 0x000F;
        public const short OILIMITFLAGS_BACKDROPS = 0x0010;
        public const short OILIMITFLAGS_ONCOLLIDE = 0x0080;
        public const short OILIMITFLAGS_QUICKCOL = 0x0100;
        public const short OILIMITFLAGS_QUICKBACK = 0x0200;
        public const short OILIMITFLAGS_QUICKBORDER = 0x0400;
        public const short OILIMITFLAGS_QUICKSPR = 0x0800;
        public const short OILIMITFLAGS_QUICKEXT = 0x1000;
        public const short OILIMITFLAGS_ALL = unchecked((short)0xFFFF);
        public short oilOi=0;  			// THE oi
        public short oilListSelected=0;               // First selection !!! DO NOT CHANGE POSITION !!!
        public short oilType=0;			// Type of the object
        public short oilObject=0;			// First objects in the game
        public int oilEvents=0;			// Events
        public byte oilWrap=0;			// WRAP flags
        public bool oilNextFlag = false;
        public int oilNObjects=0;                     // Current number
        public int oilActionCount=0;			// Action loop counter
        public int oilActionLoopCount=0;              // Action loop counter
        public int oilCurrentRoutine=0;               // Current routine for the actions
        public int oilCurrentOi=0;			// Current object
        public int oilNext=0;				// Pointer on the next
        public int oilEventCount=0;			// When the event list is done
        public int oilNumOfSelected=0;                // Number of selected objects
        public int oilOEFlags=0;			// Object's flags
        public short oilLimitFlags=0;			// Movement limitation flags
        public int oilLimitList=0;                    // Pointer to limitation list
        public short oilOIFlags=0;			// Objects preferences
        public short oilOCFlags2=0;			// Objects preferences II
        public int oilInkEffect=0;			// Ink effect
        public int oilEffectParam=0;			// Ink effect param
        public short oilHFII=0;			// First available frameitem
        public int oilBackColor=0;			// Background erasing color
        public short[] oilQualifiers=new short[8];               // Qualifiers for this object
        public string oilName = null;                 // Name
        public int oilEventCountOR=0;                 // Selection in a list of events with OR
        public short[] oilColList=null;
        public int oilColCount;

        public void copyData(COI oiPtr)
        {
            // Met dans l'OiList
            oilOi = oiPtr.oiHandle;
            oilType = oiPtr.oiType;

            oilOIFlags = oiPtr.oiFlags;
            CObjectCommon ocPtr = (CObjectCommon) oiPtr.oiOC;
            oilOCFlags2 = ocPtr.ocFlags2;
            oilInkEffect = oiPtr.oiInkEffect;
            oilEffectParam = oiPtr.oiInkEffectParam;
            oilOEFlags = ocPtr.ocOEFlags;
            oilBackColor = ocPtr.ocBackColor;
            oilEventCount = 0;
            oilObject = -1;
            oilLimitFlags = (short) OILIMITFLAGS_ALL;
            if (oiPtr.oiName != null)
            {
                oilName = oiPtr.oiName;
            }
            int q;
            for (q = 0; q < 8; q++)
            {
                oilQualifiers[q] = ocPtr.ocQualifiers[q];
            }
        } 
    }


}
