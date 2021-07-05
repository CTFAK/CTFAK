//----------------------------------------------------------------------------------
//
// CACTIVE : Objets actifs
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.OI;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Objects
{
    class CActive : CObject
    {
        public override void handle()
        {
            ros.handle();
            if (roc.rcChanged)
            {
                roc.rcChanged = false;
                modif();
            }
        }
        public override void modif()
        {
            ros.modifRoutine();
        }
    }
}
