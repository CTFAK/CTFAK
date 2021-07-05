//----------------------------------------------------------------------------------
//
// COCQBackdrop
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;
using RuntimeXNA.Application;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.OI
{
    class COCQBackdrop : COC, IDrawing
    {
        public const short FILLTYPE_NONE=0;
        public const short FILLTYPE_SOLID=1;
        public const short FILLTYPE_GRADIENT=2;
        public const short FILLTYPE_MOTIF=3;
        public const short SHAPE_NONE=0;
        public const short SHAPE_LINE=1;
        public const short SHAPE_RECTANGLE=2;
        public const short SHAPE_ELLIPSE=3;
        public const short LINEF_INVX = 0x0001;
        public const short LINEF_INVY = 0x0002;

        public short ocBorderSize;			// Border
        public int ocBorderColor;
        public short ocShape;			// Shape
        public short ocFillType;
        public short ocLineFlags;			// Only for lines in non filled mode
        public int ocColor1;			// Gradient
        public int ocColor2;
        public int ocGradientFlags;
        public short ocImage;				// Image
        public CRunApp app;
        public Texture2D texture=null;

        public COCQBackdrop()
        {
        }
        public COCQBackdrop(CRunApp a)
        {
            app = a;
        }

        public override void load(CFile file, short type)
        {
            file.skipBytes(4);		// ocDWSize
            ocObstacleType = file.readAShort();
            ocColMode = file.readAShort();
            ocCx = file.readAInt();
            ocCy = file.readAInt();
            ocBorderSize = file.readAShort();
            ocBorderColor = file.readAColor();
            ocShape = file.readAShort();

            ocFillType = file.readAShort();
            if (ocShape == 1)		// SHAPE_LINE
            {
                ocLineFlags = file.readAShort();
            }
            else
            {
                switch (ocFillType)
                {
                    case 1:			    // FILLTYPE_SOLID
                        ocColor1 = file.readAColor();
                        break;
                    case 2:			    // FILLTYPE_GRADIENT
                        ocColor1 = file.readAColor();
                        ocColor2 = file.readAColor();
                        ocGradientFlags = file.readAInt();
                        break;
                    case 3:			    // FILLTYPE_IMAGE
                        ocImage = file.readAShort();
                        break;
                }
            }
        }

        public override void enumElements(IEnum enumImages, IEnum enumFonts)
        {
            if (ocFillType == 3)		    // FILLTYPE_IMAGE
            {
                if (enumImages != null)
                {
                    short num = enumImages.enumerate(ocImage);
                    if (num != -1)
                    {
                        ocImage = num;
                    }
                }
            }
        }

        public void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
        {
            CImage image;
            int borderWidth = ocBorderSize;
            int cx = ocCx;
            int cy = ocCy;
            bool bVert = false;
            if (ocGradientFlags != 0)
            {
                bVert = true;
            }

            switch (ocShape)
            {
                case 1:     // Line
                    break;
                case 2:     // Box
                    switch (ocFillType)
                    {
                        case 0:
                            break;
                        case 1:			    // FILLTYPE_SOLID
                            app.services.drawFilledRectangle(app, x, y, cx, cy, ocColor1, borderWidth, ocBorderColor, oi.oiInkEffect&CSpriteGen.BOP_MASK, oi.oiInkEffectParam);
                            break;
                        case 2:			    // FILLTYPE_GRADIENT
                            if (texture == null)
                            {
                                texture = CServices.createGradientRectangle(app, cx, cy, ocColor1, ocColor2, bVert, borderWidth, ocBorderColor);
                            }
                            break;
                        case 3:			    // FILLTYPE_IMAGE
                            image= app.imageBank.getImageFromHandle(ocImage);
                            app.services.drawPatternRectangle(app.spriteBatch, image, x, y, cx, cy, borderWidth, ocBorderColor, oi.oiInkEffect&CSpriteGen.BOP_MASK, oi.oiInkEffectParam);
                            cx = (int)((cx + image.width - 1) / image.width)*image.width;
                            cy = (int)((cy + image.height - 1) / image.height)*image.width;
                            break;
                    }
                    break;
                case 3:     // Ellipse
                    switch (ocFillType)
                    {
                        case 0:
                            break;
                        case 1:			    // FILLTYPE_SOLID
                            if (texture == null)
                            {
                                texture = CServices.createFilledEllipse(app, cx, cy, ocColor1, borderWidth, ocBorderColor);
                            }
                            break;
                        case 2:			    // FILLTYPE_GRADIENT
                            if (texture == null)
                            {
                                texture = CServices.createGradientEllipse(app, cx, cy, ocColor1, ocColor2, bVert, borderWidth, ocBorderColor);
                            }
                            break;
                        case 3:			    // FILLTYPE_IMAGE
                            image = app.imageBank.getImageFromHandle(ocImage);
                            app.services.drawPatternRectangle(app.spriteBatch, image, x, y, cx, cy, borderWidth, ocBorderColor, oi.oiInkEffect & CSpriteGen.BOP_MASK, oi.oiInkEffectParam);
                            cx = (int)((cx + image.width - 1) / image.width) * image.width;
                            cy = (int)((cy + image.height - 1) / image.height) * image.width;
                            break;
                    }
                    break;
            }

            // Dessine le tour
            if (ocShape==1 && borderWidth > 0)
            {
                if ((ocLineFlags & LINEF_INVX) != 0)
                {
                    x += cx;
                    cx = -cx;
                }
                if ((ocLineFlags & LINEF_INVY) != 0)
                {
                    y += cy;
                    cy = -cy;
                }
                app.services.drawLine(app.spriteBatch, x, y, x + cx, y + cy, ocBorderColor, borderWidth, oi.oiInkEffect & CSpriteGen.BOP_MASK, oi.oiInkEffectParam);
            }

            if (texture != null)
            {
                app.tempRect.X = x;
                app.tempRect.Y = y;
                app.tempRect.Width = texture.Width;
                app.tempRect.Height = texture.Height;
                batch.Draw(texture, app.tempRect, null, Color.White, oi.oiInkEffect & CSpriteGen.BOP_MASK, oi.oiInkEffectParam);
            }
        }
        public void drawableKill()
        {
        }
        public CMask drawableGetMask(int flags)
        {
            return null;
        }

    }
}
