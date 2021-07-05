//----------------------------------------------------------------------------------
//
// CMOVEDEFGENERIC : données du mouvement generique
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    class CMoveDefGeneric : CMoveDef
    {
        public short mgSpeed;
        public short mgAcc;
        public short mgDec;
        public short mgBounceMult;
        public int mgDir;

        public override void load(CFile file, int length)
        {
            mgSpeed=file.readAShort();
            mgAcc=file.readAShort();
            mgDec=file.readAShort();
            mgBounceMult=file.readAShort();
            mgDir=file.readAInt();        
        }
    }
}
