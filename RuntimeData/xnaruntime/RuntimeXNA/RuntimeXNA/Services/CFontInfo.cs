//----------------------------------------------------------------------------------
//
// CFONTINFO : informations sur une fonte
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Services
{
    public class CFontInfo
    {
        public int lfHeight = 0;
        public int lfWeight = 0;
        public byte lfItalic = 0;
        public byte lfUnderline = 0;
        public byte lfStrikeOut = 0;
        public string lfFaceName = null;

        public void copy(CFontInfo f)
        {
            lfHeight = f.lfHeight;
            lfWeight = f.lfWeight;
            lfItalic = f.lfItalic;
            lfUnderline = f.lfUnderline;
            lfStrikeOut = f.lfStrikeOut;
            lfFaceName = f.lfFaceName;
        }

    }
}
