//----------------------------------------------------------------------------------
//
// CDEFTEXT : un element de texte
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
    public class CDefText
    {
        public short tsFont;					// Font 
        public short tsFlags;				// Flags
        public int tsColor;				// Color
        public string tsText;

        public const short TSF_LEFT=0x0000;
        public const short TSF_HCENTER=0x0001;
        public const short TSF_RIGHT=0x0002;
        public const short TSF_VCENTER=0x0004;
        public const short TSF_HALIGN=0x000F;
        public const short TSF_CORRECT=0x0100;
        public const short TSF_RELIEF=0x0200;

        public void load(CFile file)
        {
            tsFont=file.readAShort();
            tsFlags=file.readAShort();
            tsColor=file.readAColor();
            tsText=file.readAString();
        }

        public void enumElements(IEnum enumImages, IEnum enumFonts)
        {
            if (enumFonts != null)
            {
                short num = enumFonts.enumerate(tsFont);
                if (num != -1)
                {
                    tsFont = num;
                }
            }
        }    
    }
}
