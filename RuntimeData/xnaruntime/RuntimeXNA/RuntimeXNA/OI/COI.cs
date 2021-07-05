//----------------------------------------------------------------------------------
//
// COILIST : liste des OI de l'application
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;
using RuntimeXNA.Application;

namespace RuntimeXNA.OI
{
    public class COI
    {
        // Flags
        public const short OILF_OCLOADED=0x0001;
        public const short OILF_ELTLOADED=0x0002;
        public const short OILF_TOLOAD=0x0004;
        public const short OILF_TODELETE=0x0008;
        public const short OILF_CURFRAME=0x0010;
        public const short OILF_TORELOAD=0x0020;
        public const short OILF_IGNORELOADONCALL=0x0040;
        public const short OIF_LOADONCALL=0x0001;
        public const short OIF_DISCARDABLE=0x0002;
        public const short OIF_GLOBAL=0x0004;

        public const short NUMBEROF_SYSTEMTYPES=7;
        public const short OBJ_PLAYER=-7;
        public const short OBJ_KEYBOARD=-6;
        public const short OBJ_CREATE=-5;
        public const short OBJ_TIMER=-4;
        public const short OBJ_GAME=-3;
        public const short OBJ_SPEAKER=-2;
        public const short OBJ_SYSTEM=-1;
        public const short OBJ_BOX=0;
        public const short OBJ_BKD=1;
        public const short OBJ_SPR=2;
        public const short OBJ_TEXT=3;
        public const short OBJ_QUEST=4;
        public const short OBJ_SCORE=5;
        public const short OBJ_LIVES=6;
        public const short OBJ_COUNTER=7;
        public const short OBJ_RTF=8;
        public const short OBJ_CCA=9;
        public const short NB_SYSOBJ=10;
        public const short OBJ_LAST=10;
        public const int KPX_BASE=32;
        public const ushort OIFLAG_QUALIFIER=0x8000;

        // objInfoHeader
        public short oiHandle = 0;
        public short oiType = 0;
        public short oiFlags = 0;			// Memory flags
        //  public short oiReserved=0;			// No longer used
        public int oiInkEffect = 0;			// Ink effect
        public int oiInkEffectParam = 0;	        // Ink effect param

        // OI
        public string oiName = null;			// Name
        public COC oiOC = null;			// ObjectsCommon
        public int oiFileOffset = 0;
        public int oiLoadFlags = 0;
        public short oiLoadCount = 0;
        public short oiCount = 0;
    
        public void loadHeader(CFile file)
        {
	        oiHandle=file.readAShort();
	        oiType=file.readAShort();
	        oiFlags=file.readAShort();
	        file.skipBytes(2);
	        oiInkEffect=file.readAInt();
	        oiInkEffectParam=file.readAInt();
        }
        public void load(CFile file, CRunApp app)
        {
	        // Positionne au debut
	        file.seek(oiFileOffset);
        	
	        // En fonction du type
	        switch (oiType)
	        {
	            case 0:		// Quick background
		            oiOC=new COCQBackdrop(app);
		            break;
	            case 1:
		            oiOC=new COCBackground();
		            break;
	            default:
		            oiOC=new CObjectCommon();
		            break;
	        }
	        oiOC.load(file, oiType);
            oiOC.oi = this;
	        oiLoadFlags=0;
        }
        public void unLoad()
        {
            oiOC = null;
        }
        public void enumElements(IEnum enumImages, IEnum enumFonts)
        {
            oiOC.enumElements(enumImages, enumFonts);
        }
    }
}
