//----------------------------------------------------------------------------------
//
// ICONTROL : Interface pour les controles
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Sprites;
using RuntimeXNA.Banks;

namespace RuntimeXNA.RunLoop
{
    public interface IControl
    {
        void drawControl(SpriteBatchEffect batch);
        int getX();
        int getY();
        void setFocus(bool bFlag);
        void click(int nClicks);
        void setMouseControlled(bool bFlag);
    }
}
