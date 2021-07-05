//----------------------------------------------------------------------------------
//
// CMOVEDEFBALL : données du mouvement ball
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    class CMoveDefBall : CMoveDef
    {
        public short mbSpeed;
        public short mbBounce;
        public short mbAngles;
        public short mbSecurity;
        public short mbDecelerate;

        public override void load(CFile file, int length)
        {
            mbSpeed=file.readAShort();
            mbBounce=file.readAShort();
            mbAngles=file.readAShort();
            mbSecurity=file.readAShort();
            mbDecelerate=file.readAShort();       
        }

    }
}
