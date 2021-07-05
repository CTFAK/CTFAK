//----------------------------------------------------------------------------------
//
// CLAYER
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Frame
{
    public class CLayer
    {
        public const int FLOPT_XCOEF=0x0001;
        public const int FLOPT_YCOEF=0x0002;
        public const int FLOPT_NOSAVEBKD=0x0004;
    //    public const int FLOPT_WRAP_OBSOLETE=0x0008;
        public const int FLOPT_VISIBLE=0x0010;
        public const int FLOPT_WRAP_HORZ=0x0020;
        public const int FLOPT_WRAP_VERT=0x0040;
        public const int FLOPT_REDRAW=0x000010000;
        public const int FLOPT_TOHIDE=0x000020000;
        public const int FLOPT_TOSHOW = 0x000040000;
        
        public string pName;			// Name

        // Offset
        public int x=0;				// Current offset
        public int y=0;
        public int dx=0;				// Offset to apply to the next refresh
        public int dy=0;

        public CArrayList pBkd2=null;

        // Ladders
        public CArrayList pLadders=null;

        // Z-order max index for dynamic objects
        public int nZOrderMax=0;

        // Permanent data (EditFrameLayer)
        public int dwOptions;			// Options
        public float xCoef;
        public float yCoef;
        public int nBkdLOs;				// Number of backdrop objects
        public int nFirstLOIndex;			// Index of first backdrop object in LO table

        // Backup for restart
        public int backUp_dwOptions;
        public float backUp_xCoef;
        public float backUp_yCoef;
        public int backUp_nBkdLOs;
        public int backUp_nFirstLOIndex;

        public void load(CFile file)
        {
	        dwOptions=file.readAInt();
	        xCoef=file.readAFloat();
	        yCoef=file.readAFloat();
	        nBkdLOs=file.readAInt();
	        nFirstLOIndex=file.readAInt();
	        pName=file.readAString();

	        backUp_dwOptions=dwOptions;
	        backUp_xCoef=xCoef;
	        backUp_yCoef=yCoef;
	        backUp_nBkdLOs=nBkdLOs;
	        backUp_nFirstLOIndex=nFirstLOIndex;
        }

    }
}
