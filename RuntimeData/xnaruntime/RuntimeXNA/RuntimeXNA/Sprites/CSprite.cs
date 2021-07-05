//----------------------------------------------------------------------------------
//
// CSPRITE : Un sprite
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
using Microsoft.Xna.Framework;


namespace RuntimeXNA.Sprites
{
    public class CSprite
    {
        // Defines
        public const uint SF_RAMBO=0x00000001;		// flag "rentre dans tout le monde"
        public const uint SF_RECALCSURF=0x00000002;		// Recalc surface (if rotation or stretch)
        public const uint SF_PRIVATE=0x00000004;		// flag privé utilisé par le runtime pour la destruction des fade
        public const uint SF_INACTIF=0x00000008;		// flag "inactif" = reaffichage ssi intersection avec un autre
        public const uint SF_TOHIDE=0x00000010;		// flag "a cacher"
        public const uint SF_TOKILL=0x00000020;		// flag "a detruire"
        public const uint SF_REAF=0x00000040;		    // flag "a reafficher"
        public const uint SF_HIDDEN=0x00000080;		// flag "cache"
        public const uint SF_COLBOX=0x00000100;		// flag "collisions en mode box"
        public const uint SF_NOSAVE=0x00000200;		// flag "do not save background"
        public const uint SF_FILLBACK=0x00000400;		// flag "fill background using a solid colour (sprAdrBack)"
        public const uint SF_DISABLED=0x00000800;
        public const uint SF_REAFINT=0x00001000;		// Internal
        public const uint SF_OWNERDRAW=0x00002000;		// flag "owner draw"
        public const uint SF_OWNERSAVE=0x00004000;		// flag "owner save"
        public const uint SF_FADE=0x00008000;			// Private
        public const uint SF_OBSTACLE=0x00010000;		// Obstacle
        public const uint SF_PLATFORM=0x00020000;		// Platform
        public const uint SF_BACKGROUND=0x00080000;		// Backdrop object
        public const uint SF_SCALE_RESAMPLE=0x00100000;	// Resample when stretching
        public const uint SF_ROTATE_ANTIA=0x00200000;		// Antialiasing for rotations
        public const uint SF_NOHOTSPOT=0x00400000;		// No hot spot
        public const uint SF_OWNERCOLMASK=0x00800000;		// Owner-draw sprite supports collision masks
        public const uint SF_UPDATECOLLIST=0x10000000;
        public const uint SF_TRUEOBJECT = 0x20000000;
        public const int EFFECTFLAG_TRANSPARENT = 0x10000000;
        public const int EFFECTFLAG_ANTIALIAS=0x20000000;
        public const int EFFECT_SEMITRANSP=1;

        // Données 
        public CSprite objPrev=null;
        public CSprite objNext=null;
        public CImageBank bank;
        
        public uint sprFlags=0;			// Flags
        public short sprLayer=0;			// Sprite plane (layer)
        public short sprAngle = 0;			// Angle
        public short sprAnglenew = 0;			// Angle
        public int sprZOrder = 0;			// Z-order value

        // Coordinates
        public int sprX=0;
        public int sprY=0;

        // Bounding box
        public int sprX1=0;
        public int sprY1=0;
        public int sprX2=0;
        public int sprY2=0;

        // New coordinates
        public int sprXnew=0;
        public int sprYnew=0;

        // New bounding box
        public int sprX1new=0;
        public int sprY1new=0;
        public int sprX2new=0;
        public int sprY2new=0;

        // Background bounding box
        public int sprX1z=0;
        public int sprY1z=0;
        public int sprX2z=0;
        public int sprY2z=0;

        // Scale & Angle
        public float sprScaleX = 0;
        public float sprScaleY = 0;
        public float sprScaleXnew = 0;
        public float sprScaleYnew = 0;

        // Image or owner-draw routine
        public short sprImg=0;		        // Numero d'image
        public short sprImgNew=0;			// Nouvelle image
        public IDrawing sprRout=null;			// Ownerdraw callback routine

        // Ink effect
        public int sprEffect=0;			// 0=normal, 1=semi-transparent, > 16 = routine
        public int sprEffectParam=0;			// parametre effet (coef transparence, etc...)
        public Color rgb = Color.White;

        // Fill color (wipe with color mode)
        public int sprBackColor=0;

        // User data
        public CObject sprExtraInfo=null;

        public CSprite()
        {
        }
        public CSprite(CImageBank b)
        {
            bank = b;
        }

        public int getSpriteLayer()
        {
            return sprLayer / 2;
        }

        ////////////////////////////////////////////////
        //
        // Get Sprite Flags
        //
        public uint getSpriteFlags()
        {
            return sprFlags;
        }

        public uint setSpriteFlags(uint dwNewFlags)
        {
            uint dwOldFlags;
            dwOldFlags = sprFlags;
            sprFlags = dwNewFlags;
            return dwOldFlags;
        }

        public uint setSpriteColFlag(uint colMode)
        {
            uint om;
            om = (sprFlags & SF_RAMBO);
            sprFlags = (sprFlags & ~SF_RAMBO) | colMode;
            return om;
        }

        public float getSpriteScaleX()
        {
            return sprScaleX;
        }
        public float getSpriteScaleY()
        {
            return sprScaleY;
        }
        public bool getSpriteScaleResample()
        {
            return (sprFlags & SF_SCALE_RESAMPLE) != 0;
        }
        public int getSpriteAngle()
        {
            return sprAngle;
        }
        public bool getSpriteAngleAntiA()
        {
            return ((sprFlags & SF_ROTATE_ANTIA) != 0);
        }

        public CRect getSpriteRect()
        {
            CRect prc = new CRect();
            prc.left = sprX1new;
            prc.right = sprX2new;
            prc.top = sprY1new;
            prc.bottom = sprY2new;
            return prc;
        }

        // Update bounding box
        public void updateBoundingBox()
        {
            // Get image size & hot spot
            CImage ptei = bank.getImageFromHandle(sprImgNew);
            if (ptei == null)
            {
                sprX1new = sprXnew;
                sprX2new = sprXnew + 1;
                sprY1new = sprYnew;
                sprY2new = sprYnew + 1;
                return;
            }

            int cx = ptei.width;
            int cy = ptei.height;
            int hsx = 0;
            int hsy = 0;
            if ((sprFlags & SF_NOHOTSPOT) == 0)
            {
                hsy = ptei.ySpot;
                hsx = ptei.xSpot;
            }

            // No rotation
            if (sprAngle == 0)
            {
                // Pas de stretch en X
                if (sprScaleXnew == 1.0f)
                {
                    sprX1new = sprXnew - hsx;
                    sprX2new = sprX1new + cx;
                }

                // Stretch en X
                else
                {
                    sprX1new = sprXnew - (int)(hsx * sprScaleXnew);
                    sprX2new = sprX1new + (int)(cx * sprScaleXnew);
                }

                // Pas de stretch en Y
                if (sprScaleYnew == 1.0f)
                {
                    sprY1new = sprYnew - hsy;
                    sprY2new = sprY1new + cy;
                }

                // Stretch en Y
                else
                {
                    sprY1new = sprYnew - (int)(hsy * sprScaleYnew);
                    sprY2new = sprY1new + (int)(cy * sprScaleYnew);
                }
            }

            // Rotation
            else
            {
                // Calculate dimensions
                //int x1, y1, x2, y2;

                if (sprScaleXnew != 1.0f)
                {
                    hsx = (int)(hsx * sprScaleXnew);
                    cx = (int)(cx * sprScaleXnew);
                }
                //x1 = ptSpr.sprXnew - hsx;
                //x2 = ptSpr.sprX1new + cx;

                if (sprScaleYnew != 1.0f)
                {
                    hsy = (int)(hsy * sprScaleYnew);
                    cy = (int)(cy * sprScaleYnew);
                }
                //y1 = ptSpr.sprYnew - hsy;
                //y2 = ptSpr.sprY1new + cy;

                // Rotate
                int nhsx;
                int nhsy;

                int nx1;
                int ny1;

                int nx2;
                int ny2;

                int nx4;
                int ny4;

                cx--;	// new
                cy--;	// new

                if (sprAnglenew == 90)
                {
                    nx2 = cy;
                    ny4 = -cx;

                    ny2 = 0;
                    nx4 = 0;

                    nhsx = hsy;
                    nhsy = -hsx;
                }
                else if (sprAnglenew == 180)
                {
                    nx2 = 0;
                    ny4 = 0;

                    ny2 = -cy;
                    nx4 = -cx;

                    nhsx = -hsx;
                    nhsy = -hsy;
                }
                else if (sprAnglenew == 270)
                {
                    nx2 = -cy;
                    ny4 = cx;

                    ny2 = 0;
                    nx4 = 0;

                    nhsx = -hsy;
                    nhsy = hsx;
                }
                else
                {
                    double alpha = (double)sprAnglenew * Math.PI / 180.0;
                    float cosa = (float)Math.Cos(alpha);
                    float sina = (float)Math.Sin(alpha);

                    nhsx = (int)(hsx * cosa + hsy * sina);
                    nhsy = (int)(hsy * cosa - hsx * sina);
                    nx2 = (int)(cy * sina);	// new
                    ny4 = -(int)(cx * sina);	// new
                    ny2 = (int)(cy * cosa);	// new
                    nx4 = (int)(cx * cosa);	// new
                }

                int nx3 = nx2 + nx4;
                int ny3 = ny2 + ny4;

                // Faire translation par rapport au hotspot
                nx1 = sprXnew - nhsx;
                ny1 = sprYnew - nhsy;
                nx2 += sprXnew - nhsx;
                ny2 += sprYnew - nhsy;
                nx3 += sprXnew - nhsx;
                ny3 += sprYnew - nhsy;
                nx4 += sprXnew - nhsx;
                ny4 += sprYnew - nhsy;

                // Calculer la nouvelle bounding box (à optimiser éventuellement)
                sprX1new = Math.Min(nx1, nx2);
                sprX1new = Math.Min(sprX1new, nx3);
                sprX1new = Math.Min(sprX1new, nx4);

                sprX2new = Math.Max(nx1, nx2);
                sprX2new = Math.Max(sprX2new, nx3);
                sprX2new = Math.Max(sprX2new, nx4);

                sprX2new++;	// new

                sprY1new = Math.Min(ny1, ny2);
                sprY1new = Math.Min(sprY1new, ny3);
                sprY1new = Math.Min(sprY1new, ny4);

                sprY2new = Math.Max(ny1, ny2);
                sprY2new = Math.Max(sprY2new, ny3);
                sprY2new = Math.Max(sprY2new, ny4);

                sprY2new++; // new
            }
        }

        public void calcBoundingBox(short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, CRect prc)
        {
            CImage ptei;

            // Empty rect
            prc.left = prc.top = prc.right = prc.bottom = 0;

            // Get image size & hot spot
            ptei = bank.getImageFromHandle(newImg);
            if (ptei == null)
                return;

            int cx = ptei.width;
            int cy = ptei.height;
            int hsx = 0;
            int hsy = 0;
            if ((sprFlags & SF_NOHOTSPOT) == 0)
            {
                hsy = ptei.ySpot;
                hsx = ptei.xSpot;
            }

            // No rotation
            if (newAngle == 0)
            {
                // Pas de stretch en X
                if (newScaleX == 1.0f)
                {
                    prc.left = newX - hsx;
                    prc.right = prc.left + cx;
                }

                // Stretch en X
                else
                {
                    prc.left = newX - (int)(hsx * newScaleX);
                    prc.right = prc.left + (int)(cx * newScaleX);
                }

                // Pas de stretch en Y
                if (newScaleY == 1.0f)
                {
                    prc.top = newY - hsy;
                    prc.bottom = prc.top + cy;
                }

                // Stretch en Y
                else
                {
                    prc.top = newY - (int)(hsy * newScaleY);
                    prc.bottom = prc.top + (int)(cy * newScaleY);
                }
            }

            // Rotation
            else
            {
                // Calculate dimensions
                if (newScaleX != 1.0f)
                {
                    hsx = (int)(hsx * newScaleX);
                    cx = (int)(cx * newScaleX);
                }

                if (newScaleY != 1.0f)
                {
                    hsy = (int)(hsy * newScaleY);
                    cy = (int)(cy * newScaleY);
                }

                // Rotate
                int nhsx;
                int nhsy;

                int nx1;
                int ny1;

                int nx2;
                int ny2;

                int nx4;
                int ny4;

                cx--;	// new
                cy--;	// new

                if (newAngle == 90)
                {
                    nx2 = cy;
                    ny4 = -cx;

                    ny2 = 0;
                    nx4 = 0;

                    nhsx = hsy;
                    nhsy = -hsx;
                }
                else if (newAngle == 180)
                {
                    nx2 = 0;
                    ny4 = 0;

                    ny2 = -cy;
                    nx4 = -cx;

                    nhsx = -hsx;
                    nhsy = -hsy;
                }
                else if (newAngle == 270)
                {
                    nx2 = -cy;
                    ny4 = cx;

                    ny2 = 0;
                    nx4 = 0;

                    nhsx = -hsy;
                    nhsy = hsx;
                }
                else
                {
                    double alpha = (double)newAngle * Math.PI / 180;
                    float cosa = (float)Math.Cos(alpha);
                    float sina = (float)Math.Sin(alpha);

                    nhsx = (int)(hsx * cosa + hsy * sina);
                    nhsy = (int)(hsy * cosa - hsx * sina);

                    nx2 = (int)(cy * sina);	// new
                    ny4 = -(int)(cx * sina);	// new
                    ny2 = (int)(cy * cosa);	// new
                    nx4 = (int)(cx * cosa);	// new
                }

                int nx3 = nx2 + nx4;
                int ny3 = ny2 + ny4;

                // Faire translation par rapport au hotspot
                nx1 = newX - nhsx;
                ny1 = newY - nhsy;
                nx2 += newX - nhsx;
                ny2 += newY - nhsy;
                nx3 += newX - nhsx;
                ny3 += newY - nhsy;
                nx4 += newX - nhsx;
                ny4 += newY - nhsy;

                // Calculer la nouvelle bounding box (à optimiser éventuellement)
                prc.left = Math.Min(nx1, nx2);
                prc.left = Math.Min(prc.left, nx3);
                prc.left = Math.Min(prc.left, nx4);

                prc.right = Math.Max(nx1, nx2);
                prc.right = Math.Max(prc.right, nx3);
                prc.right = Math.Max(prc.right, nx4);

                prc.right++;	// new

                prc.top = Math.Min(ny1, ny2);
                prc.top = Math.Min(prc.top, ny3);
                prc.top = Math.Min(prc.top, ny4);

                prc.bottom = Math.Max(ny1, ny2);
                prc.bottom = Math.Max(prc.bottom, ny3);
                prc.bottom = Math.Max(prc.bottom, ny4);

                prc.bottom++;	// new
            }
        }
        void draw(SpriteBatchEffect batch)
        {

        }

    }
}
