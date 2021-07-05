//----------------------------------------------------------------------------------
//
// CPATHSTEP : un pas de mouvement path
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    class CPathStep
    {
        //  public byte mdPrevious;
        //  public byte mdNext;
        public byte mdSpeed;
        public byte mdDir;
        public short mdDx;
        public short mdDy;
        public short mdCosinus;
        public short mdSinus;
        public short mdLength;
        public short mdPause;
        public string mdName = null;

        public void load(CFile file)
        {
            mdSpeed=file.readByte();
            mdDir=file.readByte();
            mdDx=file.readAShort();
            mdDy=file.readAShort();
            mdCosinus=file.readAShort();
            mdSinus=file.readAShort();
            mdLength=file.readAShort();
            mdPause=file.readAShort();
            string name=file.readAString();
            if (name.Length > 0)
            {
                mdName = name;
            }
        }       

    }
}
