//----------------------------------------------------------------------------------
//
// CSERVICES : Routines utiles diverses
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;
using RuntimeXNA.Application;
using Microsoft.Xna.Framework;


namespace RuntimeXNA.Services
{
    public class CServices
    {
        public const short DT_LEFT = 0x0000;
        public const short DT_TOP = 0x0000;
        public const short DT_CENTER = 0x0001;
        public const short DT_RIGHT = 0x0002;
        public const short DT_BOTTOM = 0x0008;
        public const short DT_VCENTER = 0x0004;
        public const short DT_SINGLELINE = 0x0020;
        public const short DT_CALCRECT = 0x0400;
        public const short DT_VALIGN = 0x0800;
    	public const int CPTDISPFLAG_INTNDIGITS=0x000F;		
    	public const int CPTDISPFLAG_FLOATNDIGITS=0x00F0;		
		public const int CPTDISPFLAG_FLOATNDIGITS_SHIFT=4;
    	public const int CPTDISPFLAG_FLOATNDECIMALS=0xF000;	
    	public const int CPTDISPFLAG_FLOATNDECIMALS_SHIFT=12;
    	public const int CPTDISPFLAG_FLOAT_FORMAT=0x0200;		
    	public const int CPTDISPFLAG_FLOAT_USENDECIMALS=0x0400;
    	public const int CPTDISPFLAG_FLOAT_PADD=0x0800;		

    
        private Texture2D pixel=null;
        private Rectangle tempRect;
        private static int[] xPos=null;
        private static Vector2 vector;

        public static int HIWORD(int ul)
        {
            return ul >> 16;
        }

        public static int LOWORD(int ul)
        {
            return ul & 0x0000FFFF;
        }

        public static int MAKELONG(int lo, int hi)
        {
            return (hi << 16) | (lo & 0xFFFF);
        }

        public static int getRValueJava(int rgb)
        {
            return (rgb >> 16)&0xFF;
        }

        public static int getGValueJava(int rgb)
        {
            return (rgb >> 8) & 0xFF;
        }

        public static int getBValueJava(int rgb)
        {
            return rgb & 0xFF;
        }

        public static int RGBJava(int r, int g, int b)
        {
            return (r & 0xFF) << 16 | (g & 0xFF) << 8 | (b & 0xFF);
        }

        public static int swapRGB(int rgb)
        {
            int r = (rgb >> 16) & 0xFF;
            int g = (rgb >> 8) & 0xFF;
            int b = rgb & 0xFF;
            return (b & 0xFF) << 16 | (g & 0xFF) << 8 | (r & 0xFF);
        }
        public static Color getColor(int rgb)
        {
            int r = (rgb >> 16) & 0xFF;
            int g = (rgb >> 8) & 0xFF;
            int b = rgb & 0xFF;
            return new Color((byte)r, (byte)g, (byte)b);
        }
        public static Color getColorAlpha(int rgb)
        {
            int r = (rgb >> 16) & 0xFF;
            int g = (rgb >> 8) & 0xFF;
            int b = rgb & 0xFF;
            return new Color(r, g, b, 255);
        }
        public static int clamp(int val, int a, int b)
        {
            return Math.Min(Math.Max(val, a), b);
        }
        public static int drawText(SpriteBatchEffect batch, string s, short flags, CRect rc, int rgb, CFont font, int effect, int effectParam)
        {
            // Une chaine nulle?
            if (s.Length == 0)
            {
                if ((flags & 0x0400) != 0)	//DT_CALCRECT
                {
                    rc.right = rc.left;
                    rc.bottom = rc.top;
                }
                return 0;
            }

            // Cree la fonte
            SpriteFont f=font.getFont();

            // Si retour chariots, coupe la ligne en bouts
            int maxHeight = 0;
            int index = s.IndexOf((char)10);
            if (index >= 0)
            {
                CRect rc2 = new CRect();
                rc2.copyRect(rc);
                string sub;
                int h;
                int prevIndex = 0;
                int maxWidth = 0;
                int index2, nextIndex;

                // Si VCENTER ou BOTTOM calcule la taille en Y
                do
                {
                    index2 = -1;
                    if (prevIndex < s.Length)
                    {
                        index2 = s.IndexOf((char)13, prevIndex);
                    }
                    nextIndex=Math.Max(index, index2);
                    if (index2 == index - 1)
                    {
                        index--;
                    }
                    sub = s.Substring(prevIndex, index-prevIndex);
                    h = drawIt(batch, f, sub, (short)(flags | DT_CALCRECT), rc2, rgb, effect, effectParam);
                    maxWidth = Math.Max(maxWidth, rc2.right - rc2.left);
                    maxHeight += h;
                    rc2.top += h;
                    rc2.bottom = rc.bottom;
                    rc2.right = rc.right;
                    prevIndex = nextIndex + 1;
                    index = -1;
                    if (prevIndex < s.Length)
                    {
                        index = s.IndexOf((char)10, prevIndex);
                    }
                } while (index >= 0);
                if (prevIndex < s.Length)
                {
                    sub = s.Substring(prevIndex);
                    h = drawIt(batch, f, sub, (short) (flags | DT_CALCRECT), rc2, rgb, effect, effectParam);
                    maxWidth = Math.Max(maxWidth, rc2.right - rc2.left);
                    maxHeight += h;
                }
                if ((flags & DT_CALCRECT) != 0)
                {
                    rc.right = rc.left + maxWidth;
                    rc.bottom = rc2.bottom;
                    return maxHeight;
                }

                // Dessine
                rc2.copyRect(rc);
                if ((flags & DT_VCENTER) != 0)
                {
                    rc2.top = rc2.top + (rc2.bottom - rc2.top) / 2 - maxHeight / 2;
                }
                else if ((flags & DT_BOTTOM) != 0)
                {
                    rc2.top = rc2.bottom - maxHeight;
                }
                maxHeight = 0;
                prevIndex = 0;
                index = s.IndexOf((char)10);
                do
                {
                    index2 = -1;
                    if (prevIndex < s.Length)
                    {
                        index2 = s.IndexOf((char)13, prevIndex);
                    }
                    nextIndex=Math.Max(index, index2);
                    if (index2 == index - 1)
                    {
                        index--;
                    }
                    sub = s.Substring(prevIndex, index-prevIndex);
                    h = drawIt(batch, f, sub, flags, rc2, rgb, effect, effectParam);
                    maxHeight += h;
                    rc2.top += h;
                    rc2.bottom = rc.bottom;
                    rc2.right = rc.right;
                    prevIndex = nextIndex + 1;
                    index = -1;
                    if (prevIndex < s.Length)
                    {
                        index = s.IndexOf((char)10, prevIndex);
                    }
                } while (index >= 0);
                if (prevIndex < s.Length)
                {
                    sub = s.Substring(prevIndex);
                    h = drawIt(batch, f, sub, flags, rc2, rgb, effect, effectParam);
                    maxHeight += h;
                }
                return maxHeight;
            }

            // Pas de retour chariot, dessin direct
            maxHeight = drawIt(batch, f, s, (short)(flags | DT_VALIGN), rc, rgb, effect, effectParam);
            return maxHeight;
        }

        public static int drawIt(SpriteBatchEffect batch, SpriteFont f, string s, short flags, CRect rc, int rgb, int effect, int effectParam)
        {
            if (s.Length == 0)
            {
                s = " ";
            }

            // Calcule la largeur de la chaine
            int hLine;
            int spaceWidth;
            hLine = f.LineSpacing;
            spaceWidth = (int)f.MeasureString(" ").X;

            int rectWidth = rc.right - rc.left;
            int startSpace = 0;
            int currentSpace = 0;
            int previousSpace;
            int firstSpace = 0;
            int x;
            int width = 0;
            int height = 0;
            int currentXPos;
            if (xPos == null)
            {
                xPos = new int[100];
            }
            int sx;
            string ss;
            bool bQuit = false;
            bool bContinue = false;

            Color color=Color.Black;
            if ((flags & 0x0400) == 0)	//DT_CALCRECT
            {
                color=getColor(rgb);
            }

            int y = rc.top;
            int hCalcul = hLine;
            if ((hCalcul & 1) != 0)
            {
                hCalcul++;
            }
            if ((flags & DT_VALIGN) != 0)
            {
                if ((flags & DT_VCENTER) != 0)
                {
                    y = rc.top + (rc.bottom - rc.top) / 2 - hCalcul / 2;
                }
                else if ((flags & DT_BOTTOM) != 0)
                {
                    y = rc.bottom - hLine;
                }
            }
            int yTop = y;

            do
            {
                firstSpace = startSpace;
                currentXPos = 0;
                x = 0;
                height += hLine;
                do
                {
                    xPos[currentXPos] = x;
                    currentXPos += 1;
                    previousSpace = currentSpace;
                    currentSpace = -1;
                    if (firstSpace < s.Length)
                    {
                        currentSpace = s.IndexOf((char)' ', firstSpace);
                    }
                    if (currentSpace == -1)
                    {
                        currentSpace = s.Length;
                    }
                    if (currentSpace < firstSpace)
                    {
                        x -= spaceWidth;
                        break;
                    }
                    ss = s.Substring(firstSpace, currentSpace-firstSpace);
                    sx = (int)f.MeasureString(ss).X;
                    if (x + sx > rectWidth)
                    {
                        currentXPos--;
                        if (currentXPos > 0)
                        {
                            sx -= spaceWidth;
                            x -= spaceWidth;
                            currentSpace = previousSpace;
                            break;
                        }
                        int c;
                        for (c = firstSpace; c < currentSpace; c++)
                        {
                            sx = (int)f.MeasureString(s.Substring(c, 1)).X;
                            if (x + sx >= rectWidth)
                            {
                                c--;
                                if (c > 0)
                                {
                                    width = Math.Max(x, width);
                                    if ((flags & 0x0400) == 0)	//DT_CALCRECT
                                    {
                                        if ((flags & 0x0001) != 0)	//DT_CENTER
                                        {
                                            x = rc.left + (rc.right - rc.left) / 2 - x / 2;
                                        }
                                        else if ((flags & 0x0002) != 0) //DT_RIGHT
                                        {
                                            x = rc.right - x;
                                        }
                                        else
                                        {
                                            x = rc.left;
                                        }
                                        ss = s.Substring(firstSpace, c-firstSpace);
                                        vector.X = x;
                                        vector.Y = y;
                                        batch.DrawString(f, ss, vector, color, effect, effectParam);
                                    }
                                }
                                currentSpace = -1;
                                if (c < s.Length)
                                {
                                    currentSpace = s.IndexOf((char)' ', c);
                                }
                                bQuit = true;
                                if (currentSpace >= 0)
                                {
                                    bContinue = true;
                                }
                                break;
                            }
                            x += sx;
                        }
                    }
                    if (bQuit)
                    {
                        break;
                    }
                    x += sx;
                    if (x + spaceWidth > rectWidth)
                    {
                        break;
                    }
                    x += spaceWidth;
                    firstSpace = currentSpace + 1;
                } while (true);
                if (bContinue == false)
                {
                    if (bQuit)
                    {
                        break;
                    }
                    width = Math.Max(x, width);
                    int n;
                    if ((flags & 0x0400) == 0)	//DT_CALCRECT
                    {
                        if ((flags & 0x0001) != 0)	//DT_CENTER
                        {
                            x = rc.left + (rc.right - rc.left) / 2 - x / 2;
                        }
                        else if ((flags & 0x0002) != 0) //DT_RIGHT
                        {
                            x = rc.right - x;
                        }
                        else
                        {
                            x = rc.left;
                        }
                        firstSpace = startSpace;
                        for (n = 0; n < currentXPos; n++)
                        {
                            currentSpace = -1;
                            if (firstSpace < s.Length)
                            {
                                currentSpace = s.IndexOf((char)' ', firstSpace);
                            }
                            if (currentSpace == -1)
                            {
                                currentSpace = s.Length;
                            }
                            if (currentSpace < firstSpace)
                            {
                                break;
                            }
                            ss = s.Substring(firstSpace, currentSpace-firstSpace);
                            vector.X=x + xPos[n];
                            vector.Y=y;
                            batch.DrawString(f, ss, vector, color, effect, effectParam);
                            firstSpace = currentSpace + 1;
                        }
                    }
                }
                bQuit = false;
                bContinue = false;
                y += hLine;
                startSpace = currentSpace + 1;
            } while (startSpace < s.Length);

            // Retourne la taille
            if ((flags & 0x0400) != 0)	//DT_CALCRECT
            {
                rc.right = rc.left + width;
                rc.bottom = yTop + height;
            }
            return height;
        }

        public static string intToString(int value, int displayFlags)
        {
            string s = String.Format("{0:D}", value);
            if ((displayFlags & CPTDISPFLAG_INTNDIGITS) != 0)
            {
                int nDigits = displayFlags & CPTDISPFLAG_INTNDIGITS;
                if (s.Length > nDigits)
                {
                    s = s.Substring(0, nDigits);
                }
                else
                {
                    while (s.Length < nDigits)
                    {
                        s = "0" + s;
                    }
                }
            }
            return s;
        }

        public static string doubleToString(double value, int displayFlags)
        {
            string s;
            if ((displayFlags & CPTDISPFLAG_FLOAT_FORMAT) == 0)
            {
                s = String.Format("{0:G}", value);
            }
            else
            {
//                bool bRemoveTrailingZeros = false;
                int nDigits = ((displayFlags & CPTDISPFLAG_FLOATNDIGITS) >> CPTDISPFLAG_FLOATNDIGITS_SHIFT) + 1;
                int nDecimals = -1;
                if ((displayFlags & CPTDISPFLAG_FLOAT_USENDECIMALS) != 0)
                    nDecimals = ((displayFlags & CPTDISPFLAG_FLOATNDECIMALS) >> CPTDISPFLAG_FLOATNDECIMALS_SHIFT);
                else if (value != 0.0 && value > -1.0 && value < 1.0)
                {
                    nDecimals = nDigits;
//                    bRemoveTrailingZeros = true;
                }
                if (nDecimals < 0)
                {
                    s = String.Format("{0:G" + nDigits.ToString() + "}", value);
                }
                else
                {
                    s = String.Format("{0:F" + nDecimals.ToString() + "}", value);
                }
                int l, n;
                int ss;
                if ((displayFlags & CPTDISPFLAG_FLOAT_PADD) != 0)
                {
                    l = 0;
                    for (n = 0; n < s.Length; n++)
                    {
                        ss = s[n];
                        if (ss != '.' && ss != '+' && ss != '-' && ss != 'e' && ss != 'E')
                        {
                            l++;
                        }
                    }
                    bool bFlag = false;
                    if (s[0] == '-')
                    {
                        bFlag = true;
                        s = s.Substring(1);
                    }
                    while (l < nDigits)
                    {
                        s = "0" + s;
                        l++;
                    }
                    if (bFlag)
                    {
                        s = "-" + s;
                    }
                }
            }
            return s;
        }

        // Drawing primitives
        // ---------------------------------------------------------------------
        public static int getNextPowerOfTwo(int value)
        {
            uint x=(uint)(value-1);
            x |= x >> 1;  // handle  2 bit numbers
            x |= x >> 2;  // handle  4 bit numbers
            x |= x >> 4;  // handle  8 bit numbers
            x |= x >> 8;  // handle 16 bit numbers
            x |= x >> 16; // handle 32 bit numbers
            x++;
            return (int)x;
        }
        public void createThePixel(SpriteBatchEffect batch)
        {
   			pixel = new Texture2D(batch.GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });
        }
        public void drawFilledRectangle(CRunApp app, int x, int y, int width, int height, int rgb, int thickness, int borderColor, int effect, int effectParam)
        {
            Color color = getColor(rgb);
            drawFilledRectangleSub(app.spriteBatch, x, y, width, height, color, effect, effectParam);

            if (thickness > 0)
            {
                color = getColor(borderColor);
                drawFilledRectangleSub(app.spriteBatch, x, y, width, thickness, color, effect, effectParam);
                drawFilledRectangleSub(app.spriteBatch, x, y + height - thickness, width, thickness, color, effect, effectParam);
                drawFilledRectangleSub(app.spriteBatch, x, y, thickness, height, color, effect, effectParam);
                drawFilledRectangleSub(app.spriteBatch, x + width - thickness, y, thickness, height, color, effect, effectParam);
            }
        }
        public void drawRect(SpriteBatchEffect batch, CRect rc, int rgb, int effect, int effectParam) 
        {
            int width = rc.right - rc.left;
            int height = rc.bottom - rc.top;
            Color color = getColor(rgb);
            drawFilledRectangleSub(batch, rc.left, rc.top, width, 1, color, effect, effectParam);
            drawFilledRectangleSub(batch, rc.left, rc.bottom - 1, width, 1, color, effect, effectParam);
            drawFilledRectangleSub(batch, rc.left, rc.top, 1, height, color, effect, effectParam);
            drawFilledRectangleSub(batch, rc.right - 1, rc.top, 1, height, color, effect, effectParam);
        }
        public void drawRect(SpriteBatchEffect batch, int x1, int y1, int width, int height, int rgb, int effect, int effectParam)
        {
            Color color = getColor(rgb);
            drawFilledRectangleSub(batch, x1, y1, width, 1, color, effect, effectParam);
            drawFilledRectangleSub(batch, x1, y1 + height - 1, width, 1, color, effect, effectParam);
            drawFilledRectangleSub(batch, x1, y1, 1, height, color, effect, effectParam);
            drawFilledRectangleSub(batch, x1 + width - 1, y1, 1, height, color, effect, effectParam);
        }
        public void fillRect(SpriteBatchEffect batch, CRect rc, int rgb, int effect, int effectParam)
        {
            Color c = getColor(rgb);
            drawFilledRectangleSub(batch, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, c, effect, effectParam);
        }
        public void fillRect(SpriteBatchEffect batch, int x1, int y1, int width, int height, int rgb, int effect, int effectParam)
        {
            Color color = getColor(rgb);
            drawFilledRectangleSub(batch, x1, y1, width, height, color, effect, effectParam);
        }
        public void drawFilledRectangleSub(SpriteBatchEffect batch, int x, int y, int width, int height, Color color, int effect, int effectParam)
        {
            if (pixel == null)
            {
                createThePixel(batch);
            }
            if (tempRect == null)
            {
                tempRect = new Rectangle();
            }
            tempRect.X = x;
            tempRect.Y = y;
            tempRect.Width = width;
            tempRect.Height = height;
            batch.Draw(pixel, tempRect, null, color, effect, effectParam);
        }
        static void drawColorLine(Color[] colors, int x1, int y1, int x2, int width, Color color)
        {
            int x;
            int y = y1 * width;
            for (x = x1; x < x2; x++)
            {
                colors[x + y] = color;
            }
        }
        public static Texture2D createUpArrow(CRunApp app, int width, int height, int rgb)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            int textureWidth = getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            Color color = getColor(rgb);

            double x1, x2;
            double y;
            for (y=0; y<height; y++)
            {
                x1 = width / 2 - (y / height) * (width/ 2);
                x2 = width / 2 + (y / height) * (width/ 2);
                drawColorLine(colors, (int)x1, (int)y, (int)x2, textureWidth, color);
            }
            texture.SetData(colors);
            return texture;
        }
        public static Texture2D createDownArrow(CRunApp app, int width, int height, int rgb)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            int textureWidth = getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            Color color = getColor(rgb);

            double x1, x2;
            double y;
            for (y = 0; y < height; y++)
            {
                x1 = (y / height) * (width / 2);
                x2 = width - (y / height) * (width / 2); 
                drawColorLine(colors, (int)x1, (int)y, (int)x2, textureWidth, color);
            }
            texture.SetData(colors);
            return texture;
        }
        public static Texture2D createRoundedRect(CRunApp app, int width, int height, int colorRect1, int colorRect2, int colorFill1, int colorFill2)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            int textureWidth = getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            Color color = new Color(0, 0, 0, 0);
            int x, y, yy;
            for (y = 0; y < height; y++)
            {
                yy = textureWidth * y;
                for (x = width; x < textureWidth; x++)
                {
                    colors[yy + x] = color;
                }
            }

            float r1r = (colorRect1 >> 16) & 0xFF;
            float g1r = (colorRect1 >> 8) & 0xFF;
            float b1r = colorRect1 & 0xFF;
            float r2r = (colorRect2 >> 16) & 0xFF;
            float g2r = (colorRect2 >> 8) & 0xFF;
            float b2r = colorRect2 & 0xFF;
            float r1 = (colorFill1 >> 16) & 0xFF;
            float g1 = (colorFill1 >> 8) & 0xFF;
            float b1 = colorFill1 & 0xFF;
            float r2 = (colorFill2 >> 16) & 0xFF;
            float g2 = (colorFill2 >> 8) & 0xFF;
            float b2 = colorFill2 & 0xFF;
            int radius = height / 6;
            double angle;
            double step = Math.PI / 100;

            int xLeft, xRight, yCurrent, yOld;
            float deltaR, deltaG, deltaB;
            float deltaRr, deltaGr, deltaBr;
            deltaR = (r2 - r1) / height;
            deltaG = (g2 - g1) / height;
            deltaB = (b2 - b1) / height;
            deltaRr = (r2r - r1r) / height;
            deltaGr = (g2r - g1r) / height;
            deltaBr = (b2r - b1r) / height;
            float rCurrent, gCurrent, bCurrent;
            float rCurrentR, gCurrentR, bCurrentR;
            yOld = -1;
            for (angle = 0; angle < Math.PI / 2; angle += step)
            {
                xLeft = (int)(radius - radius * Math.Cos(angle));
                xRight = (int)(width - radius + radius * Math.Cos(angle));
                yCurrent = (int)(radius-radius*Math.Sin(angle));
                if (yCurrent != yOld)
                {
                    yOld = yCurrent;
                    rCurrent = r1 + deltaR * yCurrent;
                    gCurrent = g1 + deltaG * yCurrent;
                    bCurrent = b1 + deltaB * yCurrent;
                    color=new Color((byte)rCurrent, (byte)gCurrent, (byte)bCurrent);
                    drawColorLine(colors, xLeft, yCurrent, xRight, textureWidth, color);

                    rCurrentR = r1r + deltaRr * yCurrent;
                    gCurrentR = g1r + deltaGr * yCurrent;
                    bCurrentR = b1r + deltaBr * yCurrent;
                    color=new Color((byte)rCurrentR, (byte)gCurrentR, (byte)bCurrentR);
                    if (yCurrent == 0)
                    {
                        drawColorLine(colors, xLeft, yCurrent, xRight, textureWidth, color);
                    }
                    else
                    {
                        colors[xLeft + yCurrent * textureWidth] = color;
                        colors[xRight + yCurrent * textureWidth] = color;
                    }
                }
            }
            for (yCurrent = radius; yCurrent < height - radius; yCurrent++)
            {
                rCurrent = r1 + deltaR * yCurrent;
                gCurrent = g1 + deltaG * yCurrent;
                bCurrent = b1 + deltaB * yCurrent;
                color = new Color((byte)rCurrent, (byte)gCurrent, (byte)bCurrent);
                drawColorLine(colors, 0, yCurrent, width, textureWidth, color);

                rCurrentR = r1r + deltaRr * yCurrent;
                gCurrentR = g1r + deltaGr * yCurrent;
                bCurrentR = b1r + deltaBr * yCurrent;
                color=new Color((byte)rCurrentR, (byte)gCurrentR, (byte)bCurrentR);
                colors[yCurrent*textureWidth]=color;
                colors[width-1+yCurrent*textureWidth]=color;
            }
            for (angle = step; angle < Math.PI / 2; angle += step)
            {
                xLeft = (int)(radius-radius * Math.Cos(angle));
                xRight = (int)(width - radius + radius*Math.Cos(angle));
                yCurrent = (int)(height-radius+radius*Math.Sin(angle));
                if (yCurrent != yOld)
                {
                    yOld = yCurrent;
                    rCurrent = r1 + deltaR * yCurrent;
                    gCurrent = g1 + deltaG * yCurrent;
                    bCurrent = b1 + deltaB * yCurrent;
                    color=new Color((byte)rCurrent, (byte)gCurrent, (byte)bCurrent);
                    drawColorLine(colors, xLeft, yCurrent, xRight, textureWidth, color);

                    rCurrentR = r1r + deltaRr * yCurrent;
                    gCurrentR = g1r + deltaGr * yCurrent;
                    bCurrentR = b1r + deltaBr * yCurrent;
                    color=new Color((byte)rCurrentR, (byte)gCurrentR, (byte)bCurrentR);
                    colors[xLeft + yCurrent * textureWidth] = color;
                    colors[xRight + yCurrent * textureWidth] = color;
                    if (yCurrent == height - 1)
                    {
                        drawColorLine(colors, xLeft, yCurrent, xRight, textureWidth, color);
                    }
                }
            }

            texture.SetData(colors);
            return texture;
        }
        public static Texture2D createGradientRectangle(CRunApp app, int width, int height, int color1, int color2, bool bVertical, int thickness, int borderColor)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }

            int textureWidth=getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            float r1 = (color1 >> 16) & 0xFF;
            float g1 = (color1 >> 8) & 0xFF;
            float b1 = color1 & 0xFF;
            float r2 = (color2 >> 16) & 0xFF;
            float g2 = (color2 >> 8) & 0xFF;
            float b2 = color2 & 0xFF;

            Color color = new Color(r1, g1, b1);
            float oldR = r1;
            float oldG = g1;
            float oldB = b1;
            int x, y, yy;
            float deltaR, deltaG, deltaB;
            if (bVertical)
            {
                deltaR = (r2 - r1) / height;
                deltaG = (g2 - g1) / height;
                deltaB = (b2 - b1) / height;
                for (y = 0; y < height; y++)
                {
                    yy = y * textureWidth;
                    if (r1 != oldR || g1 != oldG || b1 != oldB)
                    {
                        color = new Color((byte)r1, (byte)g1, (byte)b1);
                    }
                    for (x = 0; x < width; x++)
                    {
                        colors[yy + x] = color;
                    }
                    r1 += deltaR;
                    g1 += deltaG;
                    b1 += deltaB;
                }
            }
            else
            {
                deltaR = (r2 - r1) / width;
                deltaG = (g2 - g1) / width;
                deltaB = (b2 - b1) / width;
                for (x = 0; x < width; x++)
                {
                    if (r1 != oldR || g1 != oldG || b1 != oldB)
                    {
                        color = new Color((byte)r1, (byte)g1, (byte)b1);
                    }
                    for (y = 0; y < height; y++)
                    {
                        colors[y*textureWidth+x] = color;
                    }
                    r1 += deltaR;
                    g1 += deltaG;
                    b1 += deltaB;
                }
            }

            color = new Color(0,0,0,0);
            for (y = 0; y < height; y++)
            {
                yy = textureWidth * y;
                for (x = width; x < textureWidth; x++)
                {
                    colors[yy + x] = color;
                }
            }

            if (thickness > 0)
            {
                color = getColor(borderColor);
                fillRectangle(colors, textureWidth, 0, 0, width, thickness, color);
                fillRectangle(colors, textureWidth, 0, height-thickness, width, height, color);
                fillRectangle(colors, textureWidth, 0, 0, thickness, height, color);
                fillRectangle(colors, textureWidth, width-thickness, 0, width, height, color);
            }

            texture.SetData(colors);
            return texture;
        }

        private static void fillRectangle(Color[] colors, int textureWidth, int x1, int y1, int x2, int y2, Color color)
        {
            int x, y, yLine;

            for (y = y1; y < y2; y++)
            {
                yLine = y * textureWidth;
                for (x = x1; x < x2; x++)
                {
                    colors[yLine + x] = color;
                }
            }
        }

        public void drawPatternRectangle(SpriteBatchEffect batch, CImage image, int xx, int yy, int width, int height, int thickness, int borderColor, int effect, int effectParam)
        {
            if (tempRect == null)
            {
                tempRect = new Rectangle();
            }

            int sx = (int)((width + image.width - 1) / image.width);
            int sy = (int)((height + image.height - 1) / image.height);

            int x, y;
            tempRect.Width = image.width;
            tempRect.Height = image.height;
            Texture2D texture = image.image;
            Nullable<Rectangle> sourceRect = null;
            if (image.mosaic != 0)
            {
                texture = image.app.imageBank.mosaics[image.mosaic];
                sourceRect = image.mosaicRectangle;
            }
            for (x = 0; x < sx; x++)
            {
                for (y = 0; y < sy; y++)
                {
                    tempRect.X = xx + x * image.width;
                    tempRect.Y = yy + y * image.height;

                    batch.Draw(texture, tempRect, sourceRect, Color.White, effect, effectParam);
                }
            }

            if (thickness > 0)
            {
                sx *= image.width;
                sy *= image.height;
                Color color = getColor(borderColor);
                drawFilledRectangleSub(batch, xx, yy, sx, thickness, color, effect, effectParam);
                drawFilledRectangleSub(batch, xx, yy + sy - thickness, sx, thickness, color, effect, effectParam);
                drawFilledRectangleSub(batch, xx, yy, thickness, sy, color, effect, effectParam);
                drawFilledRectangleSub(batch, xx + sx - thickness, yy, thickness, sy, color, effect, effectParam);
            }
        }

        public static Texture2D createEllipse(CRunApp app, int width, int height, int borderWidth, int borderColor)
        {
            int textureWidth = getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            int xRadius = width / 2-1;
            int yRadius = height / 2-1;
            int x, y;

            Color color=new Color(0,0,0,0);
            y=textureWidth*height;
            for (x = 0; x < y; x++)
            {
                colors[x] = color;
            }
            color = getColor(borderColor);
            createEllipse(colors, textureWidth, width, height, borderWidth, color);
            texture.SetData(colors);
            return texture;
        }

        public static Texture2D createFilledEllipse(CRunApp app, int width, int height, int rgb, int borderWidth, int borderColor)
        {
            int textureWidth = getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            int xRadius = width / 2-1;
            int yRadius = height / 2-1;
            int x, y;

            Color color=new Color(0,0,0,0);
            y=textureWidth*height;
            for (x = 0; x < y; x++)
            {
                colors[x] = color;
            }

            double angle;
            double step = Math.PI / 1000;
            int oldYY = -1;
            int x1, x2, xx, yy, yLine;
            color = getColor(rgb);
            for (angle = 0; angle < Math.PI; angle += step)
            {
                yy = (int)(yRadius-yRadius * Math.Sin(Math.PI / 2 + angle));
                if (yy != oldYY)
                {
                    x1 = (int)(xRadius+xRadius * Math.Cos(Math.PI / 2 + angle));
                    x2 = (int)(xRadius+xRadius * Math.Cos(Math.PI / 2 - angle));
                    yLine=yy*textureWidth;
                    for (xx = x1; xx < x2; xx++)
                    {
                        colors[yLine + xx] = color;
                    }
                    oldYY = yy;
                }
            }
            if (borderWidth > 0)
            {
                color = getColor(borderColor);
                createEllipse(colors, textureWidth, width, height, borderWidth, color);
            }

            texture.SetData(colors);
            return texture;
        }

        public static Texture2D createGradientEllipse(CRunApp app, int width, int height, int color1, int color2, bool bVertical, int borderWidth, int borderColor)
        {
            int textureWidth = getNextPowerOfTwo(width);
            Texture2D texture = new Texture2D(app.spriteBatch.GraphicsDevice, textureWidth, height);
            Color[] colors = new Color[textureWidth * height];

            int xRadius = width / 2-1;
            int yRadius = height / 2-1;
            int x, y;

            Color color = new Color(0, 0, 0, 0);
            y = textureWidth * height;
            for (x = 0; x < y; x++)
            {
                colors[x] = color;
            }

            float r1 = (color1 >> 16) & 0xFF;
            float g1 = (color1 >> 8) & 0xFF;
            float b1 = color1 & 0xFF;
            float r2 = (color2 >> 16) & 0xFF;
            float g2 = (color2 >> 8) & 0xFF;
            float b2 = color2 & 0xFF;
            color = new Color(r1, g1, b1);
            float oldR = r1;
            float oldG = g1;
            float oldB = b1;
            float deltaR, deltaG, deltaB;
            double angle;
            double step = Math.PI / 1000;
            int oldXX = -1;
            int oldYY = -1;
            int x1, x2, y1, y2, xx, yy, yLine;
            if (bVertical)
            {
                deltaR = (r2 - r1) / height;
                deltaG = (g2 - g1) / height;
                deltaB = (b2 - b1) / height;
                for (angle = 0; angle < Math.PI; angle += step)
                {
                    yy = (int)(yRadius - yRadius * Math.Sin(Math.PI / 2 + angle));
                    if (yy != oldYY)
                    {
                        if (r1 != oldR || g1 != oldG || b1 != oldB)
                        {
                            color = new Color((byte)r1, (byte)g1, (byte)b1);
                        }
                        x1 = (int)(xRadius + xRadius * Math.Cos(Math.PI / 2 + angle));
                        x2 = (int)(xRadius + xRadius * Math.Cos(Math.PI / 2 - angle));
                        yLine = yy * textureWidth;
                        for (xx = x1; xx < x2; xx++)
                        {
                            colors[yLine + xx] = color;
                        }
                        oldYY = yy;
                        r1 += deltaR;
                        g1 += deltaG;
                        b1 += deltaB;
                    }
                }
            }
            else
            {
                deltaR = (r2 - r1) / width;
                deltaG = (g2 - g1) / width;
                deltaB = (b2 - b1) / width;

                for (angle = 0; angle < Math.PI; angle += step)
                {
                    xx = (int)(xRadius + xRadius * Math.Cos(Math.PI - angle));
                    if (xx != oldXX)
                    {
                        if (r1 != oldR || g1 != oldG || b1 != oldB)
                        {
                            color = new Color((byte)r1, (byte)g1, (byte)b1);
                        }
                        y1 = (int)(yRadius - yRadius * Math.Sin(Math.PI - angle));
                        y2 = (int)(yRadius - yRadius * Math.Sin(Math.PI + angle));
                        for (yy = y1; yy < y2; yy++)
                        {
                            colors[yy * textureWidth + xx] = color;
                        }
                        oldXX = xx;
                        r1 += deltaR;
                        g1 += deltaG;
                        b1 += deltaB;
                    }
                }
            }

            if (borderWidth > 0)
            {
                color=getColor(borderColor);
                createEllipse(colors, textureWidth, width, height, borderWidth, color);
            }
            texture.SetData(colors);
            return texture;
        }

        private static void createEllipse(Color[] colors, int textureWidth, int width, int height, int thickness, Color color)
        {
            int xRadius = width / 2-1;
            int yRadius = height / 2-1;
            int xCenter = xRadius;
            int yCenter = yRadius;
            int xx, yy;

            double angle;
            double step = Math.PI / 1000;
            int thick;
            for (thick=0; thick<thickness; thick++)
            {
                for (angle = 0; angle < Math.PI*2; angle += step)
                {
                    xx = (int)(xCenter + xRadius * Math.Cos(Math.PI / 2 + angle));
                    yy = (int)(yCenter - yRadius * Math.Sin(Math.PI / 2 + angle));
                    colors[yy*textureWidth + xx] = color;
                }
                xRadius--;
                yRadius--;
            }
        }

        public void drawLine(SpriteBatchEffect batch, int x1, int y1, int x2, int y2, int rgb, int thickness, int effect, int effectParam)
	    {
		    if (pixel == null) 
            { 
                createThePixel(batch); 
            }

            Vector2 vectorA = new Vector2(x1, y1);
            Vector2 vectorB = new Vector2(x2, y2);

            // calculate the distance between the two vectors
	        float distance = Vector2.Distance(vectorA, vectorB);

	        // calculate the angle between the two vectors
	        float angle = (float)Math.Atan2(vectorB.Y - vectorA.Y, vectorB.X - vectorA.X);

            Color color = getColor(rgb);

            batch.Draw(pixel, vectorA, null, color, angle, Vector2.Zero, new Vector2(distance, thickness), SpriteEffects.None, 0, effect, effectParam);
	    }
        public static void replaceColor(CRunApp app, Color[] pixels, int width, int height, int oldColor, int newColor)
        {
            Color c = CServices.getColor(newColor);
            byte r = (byte)CServices.getRValueJava(oldColor);
            byte g = (byte)CServices.getGValueJava(oldColor);
            byte b = (byte)CServices.getBValueJava(oldColor);

            int x, y;
            Color pCol;
            for (y = 0; y < height; y++)
            {
                for (x=0; x<width; x++)
                {
                    pCol = pixels[y * width + x];
                    if (pCol.R == r && pCol.G == g && pCol.B == b)
                    {
                        if (newColor != 0)
                        {
                            c.A = pCol.A;
                        }
                        else
                        {
                            c.A = 0;
                        }
                        pixels[y * width + x] = c;
                    }
                }
            }
        }
    }
}
