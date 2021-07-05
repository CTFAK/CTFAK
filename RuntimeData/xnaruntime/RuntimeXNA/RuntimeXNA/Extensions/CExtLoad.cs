//----------------------------------------------------------------------------------
//
// CEXTLOADER: Chargement des extensions
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions
{
    class CExtLoad
    {
        public string name;
        public string subType;
        public short handle;

        public void loadInfo(CFile file)
        {
            int debut = file.getFilePointer();

            short size = Math.Abs(file.readAShort());
            handle = file.readAShort();
            file.skipBytes(12);
            name = file.readAString();
            int pos = name.LastIndexOf('.');
            name = name.Substring(0, pos);
            subType = file.readAString();
            file.seek(debut + size);
        }

        public CRunExtension loadRunObject()
        {
	        CRunExtension pObject=null;
        	
//F01 			
	        if (string.Compare(name, "KcArray")==0)
	        {
		        pObject=new CRunKcArray();
	        }
            if (string.Compare(name, "KcDbl") == 0)
            {
                pObject = new CRunKcDbl();
            }
            if (string.Compare(name, "ActiveBackdrop")==0)
	        {
		        pObject=new CRunActiveBackdrop();
	        }
	        if (string.Compare(name, "AdvGameBoard")==0)
	        {
		        pObject=new CRunAdvGameBoard();
	        }
	        if (string.Compare(name, "AdvPathMov")==0)
	        {
		        pObject=new CRunAdvPathMov();
	        }
//	        if (string.Compare(name, "CalcRect")==0)
//	        {
//		        pObject=new CRunCalcRect();
//	        }
	        if (string.Compare(name, "IIF")==0)
	        {
		        pObject=new CRunIIF();
	        }
	        if (string.Compare(name, "InAndOutController")==0)
	        {
		        pObject=new CRunInAndOutController();
	        }
	        if (string.Compare(name, "kcdirect")==0)
	        {
		        pObject=new CRunkcdirect();
	        }
	        if (string.Compare(name, "Layer")==0)
	        {
		        pObject=new CRunLayer();
	        }
	        if (string.Compare(name, "MoveSafely2")==0)
	        {
		        pObject=new CRunMoveSafely2();
	        }
	        if (string.Compare(name, "Platform")==0)
	        {
		        pObject=new CRunPlatform();
	        }
	        if (string.Compare(name, "clickteam-movement-controller")==0)
	        {
		        pObject=new CRunclickteam_movement_controller();
	        }
	        if (string.Compare(name, "KcBoxA")==0)
	        {
		        pObject=new CRunKcBoxA();
	        }
	        if (string.Compare(name, "KcBoxB")==0)
	        {
		        pObject=new CRunKcBoxB();
	        }
//	        if (string.Compare(name, "Accelerometer")==0)
//	        {
//		        pObject=new CRunAccelerometer();
//	        }
//	        if (string.Compare(name, "Location")==0)
//	        {
//		        pObject=new CRunLocation();
//	        }
	        if (string.Compare(name, "kcini")==0)
	        {
		        pObject=new CRunkcini();
	        }
//	        if (string.Compare(name, "MultipleTouch")==0)
//	        {
//		        pObject=new CRunMultipleTouch();
//	        }
	        if (string.Compare(name, "KcButton")==0)
	        {
		        pObject=new CRunKcButton();
	        }
//	        if (string.Compare(name, "iOSSingleEdit")==0)
//	        {
//		        pObject=new CRuniOSSingleEdit();
//	        }
//	        if (string.Compare(name, "iOSMultipleEdit")==0)
//	        {
//		        pObject=new CRuniOSMultipleEdit();
//	        }
	        if (string.Compare(name, "AdvDir")==0)
	        {
		        pObject=new CRunAdvDir();
	        }
//	        if (string.Compare(name, "JoystickControl")==0)
//	        {
//		        pObject=new CRunJoystickControl();
//	        }
//	        if (string.Compare(name, "kcclock")==0)
//	        {
//		        pObject=new CRunkcclock();
//	        }
	        if (string.Compare(name, "kchisc")==0)
	        {
		        pObject=new CRunkchisc();
	        }
	        if (string.Compare(name, "kcrandom")==0)
	        {
		        pObject=new CRunkcrandom();
	        }
	        if (string.Compare(name, "ObjectMover")==0)
	        {
		        pObject=new CRunObjectMover();
	        }
	        if (string.Compare(name, "WargameMap")==0)
	        {
		        pObject=new CRunWargameMap();
	        }
	        if (string.Compare(name, "parser")==0)
	        {
		        pObject=new CRunparser();
	        }
	        if (string.Compare(name, "MTRandom")==0)
	        {
		        pObject=new CRunMTRandom();
	        }
	        if (string.Compare(name, "XNAEdit")==0)
	        {
		        pObject=new CRunXNAEdit();
	        }
//	        if (string.Compare(name, "get")==0)
//	        {
//		        pObject=new CRunGet();
//	        }
	        if (string.Compare(name, "kclist")==0)
	        {
		        pObject=new CRunkclist();
	        }
            if (string.Compare(name, "kccombo") == 0)
            {
                pObject = new CRunkccombo();
            }
            if (string.Compare(name, "kcedit") == 0)
            {
                pObject = new CRunkcedit();
            }
            //	        if (string.Compare(name, "kcinput")==0)
//	        {
//		        pObject=new CRunkcinput();
//	        }
//	        if (string.Compare(name, "MobileUtilities")==0)
//	        {
//		        pObject=new CRunMobileUtilities();
//	        }
	        if (string.Compare(name, "StringTokenizer")==0)
	        {
		        pObject=new CRunStringTokenizer();
	        }
            if (string.Compare(name, "kcclock") == 0)
            {
                pObject = new CRunkcclock();
            }
#if XBOX || WINDOWS
            if (string.Compare(name, "XBOXGamepad") == 0)
            {
                pObject = new CRunXBOXGamepad();
            }
#endif
#if WINDOWS_PHONE
            if (string.Compare(name, "MultipleTouch") == 0)
            {
                pObject = new CRunMultipleTouch();
            }
            if (string.Compare(name, "JoystickControl") == 0)
            {
                pObject = new CRunJoystickControl();
            }
            if (string.Compare(name, "Accelerometer") == 0)
            {
                pObject = new CRunAccelerometer();
            }
            if (string.Compare(name, "Location") == 0)
            {
                pObject = new CRunLocation();
            }
            if (string.Compare(name, "XNAAdControl") == 0)
            {
                pObject = new CRunXNAAdControl();
            }
#endif
            if (string.Compare(name, "XNA") == 0)
            {
                pObject = new CRunXNA();
            }
            if (string.Compare(name, "kcwctrl") == 0)
            {
                pObject = new CRunkcwctrl();
            }
            if (string.Compare(name, "XNAGamerServices") == 0)
            {
                pObject = new CRunXNAGamerServices();
            }
            if (string.Compare(name, "Inventory") == 0)
            {
                pObject = new CRunInventory();
            }

#if WINDOWS_PHONE || WINDOWS
            if (string.Compare(name, "Get") == 0)
            {
                pObject = new CRunGet();
            }
#endif
            //F01END	        	
	        return pObject;
        }

    }
}
