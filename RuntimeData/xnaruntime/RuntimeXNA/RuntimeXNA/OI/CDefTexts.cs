//----------------------------------------------------------------------------------
//
// CDEFTEXTS : liste de textes
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
    public class CDefTexts : CDefObject
    {
        public int otCx;
        public int otCy;
        public int otNumberOfText;
        public CDefText[] otTexts;

        public override void load(CFile file)
        {
            int debut=file.getFilePointer();
            file.skipBytes(4);          // Size
            otCx=file.readAInt();
            otCy=file.readAInt();
            otNumberOfText=file.readAInt();
            
            otTexts=new CDefText[otNumberOfText];
            int[] offsets=new int[otNumberOfText];
            int n;
            for (n=0; n<otNumberOfText; n++)
            {
                offsets[n]=file.readAInt();
            }
            for (n=0; n<otNumberOfText; n++)
            {
                otTexts[n]=new CDefText();
                file.seek(debut+offsets[n]);
                otTexts[n].load(file);
            }
        }

        public override void enumElements(IEnum enumImages, IEnum enumFonts)
        {
            int n;
            for (n = 0; n < otNumberOfText; n++)
            {
                otTexts[n].enumElements(enumImages, enumFonts);
            }
        }
    }
}
