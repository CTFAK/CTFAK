//----------------------------------------------------------------------------------
//
// COCBACKGROUND : un objet décor normal
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;

namespace RuntimeXNA.OI
{
    class COCBackground : COC
    {
        public short ocImage;			// Image

        public override void load(CFile file, short type)
        {
	        file.skipBytes(4);		// ocDWSize
	        ocObstacleType=file.readAShort();
	        ocColMode=file.readAShort();
	        ocCx=file.readAInt();
	        ocCy=file.readAInt();
	        ocImage=file.readAShort();
        }

        public override void enumElements(IEnum enumImages, IEnum enumFonts)
        {
            if (enumImages != null)
            {
                short num = enumImages.enumerate(ocImage);
                if (num != -1)
                {
                    ocImage = num;
                }
            }
        }

    }
}
