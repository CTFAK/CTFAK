//----------------------------------------------------------------------------------
//
// CDEFSTRINGS : definition des alterable strings
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Values
{
    public class CDefStrings
    {
        public short nStrings;
        public string[] strings;

        public void load(CFile file)
        {
            nStrings=file.readAShort();
            strings=new String[nStrings];
            int n;
            for (n=0; n<nStrings; n++)
            {
                strings[n]=file.readAString();
            }
        }
    }
}
