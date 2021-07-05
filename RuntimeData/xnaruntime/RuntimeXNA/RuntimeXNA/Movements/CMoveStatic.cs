//----------------------------------------------------------------------------------
//
// CMOVESTATIC : Mouvement statique
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;
using RuntimeXNA.Application;
using RuntimeXNA.Animations;

namespace RuntimeXNA.Movements
{
    class CMoveStatic : CMove
    {
        public override void init(CObject ho, CMoveDef mvPtr)
        {
            hoPtr = ho;
            hoPtr.roc.rcSpeed = 0;
            hoPtr.roc.rcCheckCollides = true;			//; Force la detection de collision
            hoPtr.roc.rcChanged = true;
        }
        public override void move()
        {
            if (hoPtr.roa != null)
            {
                hoPtr.roa.animate();
            }
            if (hoPtr.roc.rcCheckCollides)			//; Faut-il tester les collisions?
            {
                hoPtr.roc.rcCheckCollides = false;		//; Va tester une fois!
                hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
            }
        }
        public override void setXPosition(int x)
        {
            if (hoPtr.hoX != x)
            {
                hoPtr.hoX = x;
                hoPtr.rom.rmMoveFlag = true;
                hoPtr.roc.rcChanged = true;
            }
            hoPtr.roc.rcCheckCollides = true;					//; Force la detection de collision
        }

        public override void setYPosition(int y)
        {
            if (hoPtr.hoY != y)
            {
                hoPtr.hoY = y;
                hoPtr.rom.rmMoveFlag = true;
                hoPtr.roc.rcChanged = true;
            }
            hoPtr.roc.rcCheckCollides = true;					//; Force la detection de collision
        }


    }
}
