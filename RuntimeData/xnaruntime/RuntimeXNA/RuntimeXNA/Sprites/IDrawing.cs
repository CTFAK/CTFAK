//----------------------------------------------------------------------------------
//
// IDRAWING : Interface pour les sprites owner draw
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Sprites;
using RuntimeXNA.Banks;

namespace RuntimeXNA.Sprites
{
    public interface IDrawing
    {
        void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y);
        void drawableKill();
        CMask drawableGetMask(int flags);
    }
}
