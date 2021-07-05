//----------------------------------------------------------------------------------
//
// CMOVEDEFPLATFORM : données du mouvement platforme
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    class CMoveDefPlatform : CMoveDef
    {
        public short mpSpeed;
        public short mpAcc;
        public short mpDec;
        public short mpJumpControl;
        public short mpGravity;
        public short mpJump;

        public override void load(CFile file, int length)
        {
            mpSpeed=file.readAShort();
            mpAcc=file.readAShort();	
            mpDec=file.readAShort();	
            mpJumpControl=file.readAShort();
            mpGravity=file.readAShort();
            mpJump=file.readAShort();        
        }
    }
}
