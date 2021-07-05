//----------------------------------------------------------------------------------
//
// CSPRITEGEN : Generateur de sprites
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Objects;
using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Sprites
{
    public class CSpriteGen
    {
        // ActiveSprite
        public const int AS_DEACTIVATE = 0x0000;		// Desactive un sprite actif
        public const int AS_REDRAW = 0x0001;			// Reaffiche un sprite inactif
        public const int AS_ACTIVATE = 0x0002;			// Active un sprite inactif
        public const int AS_ENABLE = 0x0004;
        public const int AS_DISABLE = 0x0008;
    //    public const int AS_REDRAW_NOBKD=0x0011;
        public const int AS_REDRAW_RECT = 0x0020;
        public const int GS_BACKGROUND = 0x0001;
        public const int GS_SAMELAYER = 0x0002;
        public const short CM_BOX = 0;
        public const short CM_BITMAP = 1;
        public const short PSCF_CURRENTSURFACE = 0x0001;
        public const short PSCF_TEMPSURFACE = 0x0002;
        public const short LAYER_ALL = -1;
//        public const int EFFECT_NONE = 0;
//        public const int EFFECT_SEMITRANSP = 1;
        public const int BOP_COPY=0; // None
        public const int BOP_BLEND=1; // dest = ((dest * coef) + (src * (128-coef)))/128
        public const int BOP_INVERT=2; // dest = src XOR 0xFFFFFF
        public const int BOP_XOR=3; // dest = src XOR dest
        public const int BOP_AND=4; // dest = src AND dest
        public const int BOP_OR=5; // dest = src OR dest
        public const int BOP_BLEND_REPLACETRANSP=6; // dest = ((dest * coef) + ((src==transp)?replace:src * (128-coef)))/128
        public const int BOP_DWROP=7;
        public const int BOP_ANDNOT=8;
        public const int BOP_ADD=9;
        public const int BOP_MONO=10;
        public const int BOP_SUB=11;
        public const int BOP_BLEND_DONTREPLACECOLOR=12;
        public const int BOP_EFFECTEX=13;
        public const int BOP_MAX = 14;
        public const int EFFECTFLAG_TRANSPARENT = 0x10000000;
        public const int EFFECTFLAG_ANTIALIAS = 0x20000000;
//        public const int EFFECT_MASK = 0xFFFF;
        public const int BOP_MASK = 0xFFF;
        public const int BOP_RGBAFILTER = 0x1000;

        public const int SCF_OBSTACLE=1;
        public const int SCF_PLATFORM=2;
        public const int SCF_EVENNOCOL=4;
        public const int SCF_BACKGROUND = 8;

        public CSprite firstSprite;
        public CSprite lastSprite;
        CRunFrame frame;
        CRunApp app;
        CImageBank bank;
        public short colMode;
        public Rectangle tempRect = new Rectangle();
        public int xOffset=0;
        public int yOffset = 0;
        Vector2 vector = new Vector2();
        Color bColor = new Color(255,255,255,255);

        public CSpriteGen()
        {
            firstSprite = null;
            lastSprite = null;
        }

        public void setData(CImageBank b, CRunApp a, CRunFrame f)
        {
            bank = b;
            frame = f;
            app = a;
        }
        public void setOffsets(int x, int y)
        {
            xOffset = x;
            yOffset = y;
        }
        public CSprite addSprite(int xSpr, int ySpr, short iSpr, short wLayer, int nZOrder, int backSpr, uint sFlags, CObject extraInfo)
        {
            // Verifie validite fenetre et image
            CSprite ptSpr = null;

            // Alloue de la place pour l'objet
            ptSpr = winAllocSprite();

            // Store info
            ptSpr.bank = bank;
            ptSpr.sprFlags = (sFlags | CSprite.SF_REAF);
            ptSpr.sprFlags &= ~(CSprite.SF_TOKILL | CSprite.SF_REAFINT | CSprite.SF_RECALCSURF | CSprite.SF_OWNERDRAW | CSprite.SF_OWNERSAVE);
            ptSpr.sprLayer = (short)(wLayer * 2);
            if ((sFlags & CSprite.SF_BACKGROUND) == 0)
            {
                ptSpr.sprLayer++;
            }
            ptSpr.sprZOrder = nZOrder;
            ptSpr.sprX = ptSpr.sprXnew = xSpr;
            ptSpr.sprY = ptSpr.sprYnew = ySpr;
            ptSpr.sprImg = ptSpr.sprImgNew = (short)iSpr;
            ptSpr.sprExtraInfo = extraInfo;
            ptSpr.sprEffect = CSprite.EFFECTFLAG_TRANSPARENT;
            ptSpr.sprEffectParam = 0;
            ptSpr.sprScaleX = ptSpr.sprScaleY = ptSpr.sprScaleXnew = ptSpr.sprScaleYnew = 1.0f;
            ptSpr.sprAngle = ptSpr.sprAnglenew = 0;
            ptSpr.sprX1z = ptSpr.sprY1z = -1;

            // Background color
            ptSpr.sprBackColor = 0;
            if ((sFlags & CSprite.SF_FILLBACK) != 0)
            {
                ptSpr.sprBackColor = backSpr;
            }

            // Update bounding box
            ptSpr.updateBoundingBox();

            // Copy new bounding box to old
            ptSpr.sprX1 = ptSpr.sprX1new;
            ptSpr.sprY1 = ptSpr.sprY1new;
            ptSpr.sprX2 = ptSpr.sprX2new;
            ptSpr.sprY2 = ptSpr.sprY2new;

            // Sort sprite
            sortLastSprite(ptSpr);

            return ptSpr;
        }

        public CSprite addOwnerDrawSprite(int x1, int y1, int x2, int y2, short wLayer, int nZOrder, int backSpr, uint sFlags, CObject extraInfo, IDrawing sprProc)
        {
            CSprite ptSpr;
            ptSpr = winAllocSprite();

            // Init coord sprite
            ptSpr.sprX = ptSpr.sprXnew = x1;
            ptSpr.sprY = ptSpr.sprYnew = y1;
            ptSpr.sprX1new = ptSpr.sprX1 = x1;
            ptSpr.sprY1new = ptSpr.sprY1 = y1;
            ptSpr.sprX2new = ptSpr.sprX2 = x2;
            ptSpr.sprY2new = ptSpr.sprY2 = y2;
            ptSpr.sprX1z = ptSpr.sprY1z = -1;
            ptSpr.sprLayer = (short)(wLayer * 2);
            if ((sFlags & CSprite.SF_BACKGROUND) == 0)
            {
                ptSpr.sprLayer++;
            }
            ptSpr.sprZOrder = nZOrder;
            ptSpr.sprExtraInfo = extraInfo;
            ptSpr.sprRout = sprProc;
            ptSpr.sprFlags = (sFlags | CSprite.SF_OWNERDRAW);
            ptSpr.sprFlags &= ~(CSprite.SF_TOKILL | CSprite.SF_REAFINT | CSprite.SF_RECALCSURF);
            ptSpr.sprEffect = CSprite.EFFECTFLAG_TRANSPARENT;
            ptSpr.sprEffectParam = 0;
            ptSpr.sprScaleX = ptSpr.sprScaleY = ptSpr.sprScaleXnew = ptSpr.sprScaleYnew = 1.0f;
            ptSpr.sprAngle = ptSpr.sprAnglenew = 0;

            // Background color
            ptSpr.sprBackColor = 0;
            if ((sFlags & CSprite.SF_FILLBACK) != 0)
            {
                ptSpr.sprBackColor = backSpr;
            }

            // Trier le sprite avec ses predecesseurs
            sortLastSprite(ptSpr);

            return ptSpr;
        }

        public CSprite modifSprite(CSprite ptSpr, int xSpr, int ySpr, short iSpr)
        {
            // Pointer sur le sprite et son image
            do
            {
                if (ptSpr == null)
                {
                    break;
                }

                // Comparison
                if (ptSpr.sprXnew != xSpr || ptSpr.sprYnew != ySpr || ptSpr.sprImgNew != iSpr)
                {
                    // Change
                    ptSpr.sprXnew = xSpr;
                    ptSpr.sprYnew = ySpr;
                    ptSpr.sprImgNew = iSpr;

                    // Update bounding box
                    ptSpr.updateBoundingBox();

                    // Set redraw flag
                    ptSpr.sprFlags |= CSprite.SF_REAF;
                }
            } while (false);
            return ptSpr;
        }

        public CSprite modifSpriteEx(CSprite ptSpr, int xSpr, int ySpr, short iSpr, float fScaleX, float fScaleY, bool bResample, int nAngle, bool bAntiA)
        {
            // Pointer sur le sprite et son image
            do
            {
                if (ptSpr == null)
                {
                    break;
                }

                // Plus tard: autoriser les valeurs négatives et faire ReverseX / ReverseY
                if (fScaleX < 0.0f)
                {
                    fScaleX = 0.0f;
                }
                if (fScaleY < 0.0f)
                {
                    fScaleY = 0.0f;
                }
                nAngle %= 360;
                if (nAngle < 0)
                {
                    nAngle += 360;
                }

                // Comparison
                if (ptSpr.sprXnew != xSpr || ptSpr.sprYnew != ySpr || ptSpr.sprImgNew != iSpr || fScaleX != ptSpr.sprScaleX || fScaleY != ptSpr.sprScaleY || nAngle != ptSpr.sprAngle)
                {
                    // Change
                    ptSpr.sprXnew = xSpr;
                    ptSpr.sprYnew = ySpr;
                    ptSpr.sprImgNew = iSpr;

                    ptSpr.sprScaleXnew = fScaleX;
                    ptSpr.sprScaleYnew = fScaleY;
                    ptSpr.sprAnglenew = (short)nAngle;

                    // Update bounding box
                    ptSpr.updateBoundingBox();

                    // Set redraw flag
                    ptSpr.sprFlags |= CSprite.SF_REAF;
                }
            } while (false);
            return ptSpr;
        }

        public CSprite modifSpriteEffect(CSprite ptSpr, int eff, int effectParam)
        {
            // Pointer sur le sprite
            do
            {
                if (ptSpr == null)
                {
                    break;
                }

                ptSpr.sprEffect = eff&BOP_MASK;
                ptSpr.sprEffectParam = effectParam;

                ptSpr.rgb = Color.White;
                float coef = 1.0F;
                if ((eff & BOP_RGBAFILTER) != 0)
                {
                    ptSpr.rgb = CServices.getColorAlpha(effectParam & 0xFFFFFF);
                    coef = (float)(((effectParam >> 24) & 0xFF) / 255.0);
                }
                else if (ptSpr.sprEffect == BOP_BLEND)
                {
                    coef = (float)((128 - ptSpr.sprEffectParam) / 128.0);
                }
                ptSpr.rgb = ptSpr.rgb * coef;
                
                // reafficher sprite
                ptSpr.sprFlags |= CSprite.SF_REAF;

            } while (false);
            return ptSpr;
        }

        public CSprite modifOwnerDrawSprite(CSprite ptSprModif, int x1, int y1, int x2, int y2)
        {
            // Pointer sur le sprite
            do
            {
                if (ptSprModif == null)
                {
                    break;
                }

                ptSprModif.sprX1new = x1;
                ptSprModif.sprY1new = y1;
                ptSprModif.sprX2new = x2;
                ptSprModif.sprY2new = y2;

                // Reafficher sprite
                ptSprModif.sprFlags |= CSprite.SF_REAF;

            } while (false);
            return ptSprModif;
        }

        public void setSpriteLayer(CSprite ptSpr, int nLayer)
        {
            if (ptSpr == null)
            {
                return;
            }

            int nNewLayer = nLayer * 2;
            if ((ptSpr.sprFlags & CSprite.SF_BACKGROUND) == 0)
            {
                nNewLayer++;
            }

            CSprite pSprPrev;
            CSprite pSprNext;
            if (ptSpr.sprLayer != (short)nNewLayer)
            {
                int nOldLayer = ptSpr.sprLayer;
                ptSpr.sprLayer = (short)nNewLayer;

                if (nOldLayer < nNewLayer)
                {
                    if (lastSprite != null)
                    {
                        // Exchange the sprite with the next one until the end of the list or the next layer
                        while (ptSpr != lastSprite)
                        {
                            pSprNext = ptSpr.objNext;
                            if (pSprNext == null)
                            {
                                break;
                            }
                            if (pSprNext.sprLayer > (short)nNewLayer)
                            {
                                break;
                            }

                            int nzo1 = ptSpr.sprZOrder;
                            int nzo2 = pSprNext.sprZOrder;

                            swapSprites(ptSpr, pSprNext);

                            // Restore z-order values
                            ptSpr.sprZOrder = nzo1;
                            pSprNext.sprZOrder = nzo2;
                        }
                    }
                }
                else
                {
                    if (firstSprite != null)
                    {
                        // Exchange the sprite with the previous one until the beginning of the list or the previous layer
                        while (ptSpr != firstSprite)
                        {
                            pSprPrev = ptSpr.objPrev;
                            if (pSprPrev == null)
                            {
                                break;
                            }
                            if (pSprPrev.sprLayer <= (short)nNewLayer)
                            {
                                break;
                            }

                            int nzo1 = ptSpr.sprZOrder;
                            int nzo2 = pSprPrev.sprZOrder;

                            swapSprites(pSprPrev, ptSpr);

                            // Restore z-order values
                            ptSpr.sprZOrder = nzo1;
                            pSprPrev.sprZOrder = nzo2;
                        }
                    }
                }

                // Take the last zorder value plus one (but the caller must update this value after calling SetSpriteLayer)
                pSprPrev = ptSpr.objPrev;
                if (pSprPrev == null || pSprPrev.sprLayer != ptSpr.sprLayer)
                {
                    ptSpr.sprZOrder = 1;
                }
                else
                {
                    ptSpr.sprZOrder = pSprPrev.sprZOrder + 1;
                }
            }
        }

        public void setSpriteScale(CSprite ptSpr, float fScaleX, float fScaleY, bool bResample)
        {
            if (ptSpr != null)
            {
                // Autoriser les valeurs négatives et faire ReverseX / ReverseY
                if (fScaleX < (float)0.0)
                {
                    fScaleX = (float)0.0;
                }
                if (fScaleY < (float)0.0)
                {
                    fScaleY = (float)0.0;
                }
                bool bOldResample = ((ptSpr.sprFlags & CSprite.SF_SCALE_RESAMPLE) != 0);

                if (ptSpr.sprScaleX != fScaleX || ptSpr.sprScaleY != fScaleY || bResample != bOldResample)
                {
                    ptSpr.sprScaleXnew = fScaleX;
                    ptSpr.sprScaleYnew = fScaleY;
                    ptSpr.sprFlags |= CSprite.SF_REAF;
                    ptSpr.sprFlags &= ~CSprite.SF_SCALE_RESAMPLE;
                    ptSpr.updateBoundingBox();
                }
            }
        }

        public void setSpriteAngle(CSprite ptSpr, int nAngle, bool bAntiA)
        {
            if (ptSpr != null)
            {
                nAngle %= 360;
                if (nAngle < 0)
                {
                    nAngle += 360;
                }
                if (ptSpr.sprAngle != nAngle)
                {
                    ptSpr.sprAnglenew = (short)nAngle;
                    ptSpr.sprFlags &= ~CSprite.SF_ROTATE_ANTIA;
                    ptSpr.sprFlags |= CSprite.SF_REAF;
                    ptSpr.updateBoundingBox();
                }
            }
        }

        public void sortLastSprite(CSprite ptSprOrg)
        {
            CSprite ptSpr = ptSprOrg;
            CSprite ptSpr1;
            CSprite ptSprPrev;
            CSprite ptSprNext;
            short wLayer;

            //==================================================;
            //  	Tri sur les numeros de plan uniquement		;
            //==================================================;
            // On part du principe que les autres sprites sont deja tries
            //
            // On peut mieux optimiser!!! (= parcours tant que plan < et 1 seul echange a la fin...)

            // Look for sprite layer
            wLayer = ptSpr.sprLayer;
            ptSpr1 = ptSpr.objPrev;
            while (ptSpr1 != null)
            {
                if (wLayer >= ptSpr1.sprLayer)			// On arrete des qu'on trouve un plan <
                {
                    break;
                }

                // Si plan trouve >, alors on echange les sprites
                ptSprPrev = ptSpr1.objPrev;
                if (ptSprPrev == null)
                {
                    firstSprite = ptSpr;
                }
                else
                {
                    ptSprPrev.objNext = ptSpr;		// Next ( Prev ( spr1 ) ) = spr
                }
                ptSprNext = ptSpr.objNext;
                if (ptSprNext == null)
                {
                    lastSprite = ptSpr1;
                }
                else
                {
                    ptSprNext.objPrev = ptSpr1;		// Prev ( Next ( spr ) ) = spr1
                }
                ptSpr.objPrev = ptSpr1.objPrev;	// Prev ( spr ) = Prev ( spr1 )
                ptSpr1.objPrev = ptSpr;

                ptSpr1.objNext = ptSpr.objNext;	// Next ( spr1 ) = Next ( spr )
                ptSpr.objNext = ptSpr1;
                ptSpr1 = ptSpr;

                ptSpr1 = ptSpr1.objPrev;			// sprite precedent
            }

            // Same layer? sort by z-order value
            if (ptSpr1 != null && wLayer == ptSpr1.sprLayer)
            {
                int nZOrder = ptSpr.sprZOrder;

                while (ptSpr1 != null && wLayer == ptSpr1.sprLayer)
                {
                    if (nZOrder >= ptSpr1.sprZOrder)			// On arrete des qu'on trouve un plan <
                    {
                        break;
                    }

                    // Si plan trouve >, alors on echange les sprites
                    ptSprPrev = ptSpr1.objPrev;
                    if (ptSprPrev == null)
                    {
                        firstSprite = ptSpr;
                    }
                    else
                    {
                        ptSprPrev.objNext = ptSpr;		// Next ( Prev ( spr1 ) ) = spr
                    }
                    ptSprNext = ptSpr.objNext;
                    if (ptSprNext == null)
                    {
                        lastSprite = ptSpr1;
                    }
                    else
                    {
                        ptSprNext.objPrev = ptSpr1;		// Prev ( Next ( spr ) ) = spr1
                    }
                    ptSpr.objPrev = ptSpr1.objPrev;			// Prev ( spr ) = Prev ( spr1 )
                    ptSpr1.objPrev = ptSpr;

                    ptSpr1.objNext = ptSpr.objNext;			// Next ( spr1 ) = Next ( spr )
                    ptSpr.objNext = ptSpr1;
                    ptSpr1 = ptSpr;

                    ptSpr1 = ptSpr1.objPrev;			// sprite precedent
                }
            }
        }

        public void swapSprites(CSprite sp1, CSprite sp2)
        {
            // Security
            if (sp1 == sp2)
            {
                return;
            }

            CSprite pPrev1 = sp1.objPrev;
            CSprite pNext1 = sp1.objNext;

            CSprite pPrev2 = sp2.objPrev;
            CSprite pNext2 = sp2.objNext;

            // Exchange layers . non !
            //	WORD holdLayer = sp1.sprLayer;
            //	sp1.sprLayer = sp2.sprLayer;
            //	sp2.sprLayer = holdLayer;

            // Exchange z-order values
            int nZOrder = sp1.sprZOrder;
            sp1.sprZOrder = sp2.sprZOrder;
            sp2.sprZOrder = nZOrder;

            // Exchange sprites

            // Several cases
            //
            // 1. pPrev1, sp1, sp2, pNext2
            //
            //    pPrev1.next = sp2
            //	  sp2.prev = pPrev1;
            //	  sp2.next = sp1;
            //	  sp1.prev = sp2;
            //	  sp1.next = pNext2
            //	  pNext2.prev = sp1
            //
            if (pNext1 == sp2)
            {
                if (pPrev1 != null)
                {
                    pPrev1.objNext = sp2;
                }
                sp2.objPrev = pPrev1;
                sp2.objNext = sp1;
                sp1.objPrev = sp2;
                sp1.objNext = pNext2;
                if (pNext2 != null)
                {
                    pNext2.objPrev = sp1;
                }

                // Update first & last sprites
                if (pPrev1 == null)
                {
                    firstSprite = sp2;
                }
                if (pNext2 == null)
                {
                    lastSprite = sp1;
                }
            }

            // 2. pPrev2, sp2, sp1, pNext1
            //
            //    pPrev2.next = sp1
            //	  sp1.prev = pPrev2;
            //	  sp1.next = sp2;
            //	  sp2.prev = sp1;
            //	  sp2.next = pNext1
            //	  pNext1.prev = sp2
            //
            else if (pNext2 == sp1)
            {
                if (pPrev2 != null)
                {
                    pPrev2.objNext = sp1;
                }
                sp1.objPrev = pPrev2;
                sp1.objNext = sp2;
                sp2.objPrev = sp1;
                sp2.objNext = pNext1;
                if (pNext1 != null)
                {
                    pNext1.objPrev = sp2;
                }

                // Update first & last sprites
                if (pPrev2 == null)
                {
                    firstSprite = sp1;	//	*ptPtsObj = (UINT)sp1;
                }
                if (pNext1 == null)
                {
                    lastSprite = sp2;	//	*(ptPtsObj+1) = (UINT)sp2;
                }
            }
            else
            {
                if (pPrev1 != null)
                {
                    pPrev1.objNext = sp2;
                }
                if (pNext1 != null)
                {
                    pNext1.objPrev = sp2;
                }
                sp1.objPrev = pPrev2;
                sp1.objNext = pNext2;
                if (pPrev2 != null)
                {
                    pPrev2.objNext = sp1;
                }
                if (pNext2 != null)
                {
                    pNext2.objPrev = sp1;
                }
                sp2.objPrev = pPrev1;
                sp2.objNext = pNext1;

                // Update first & last sprites
                if (pPrev1 == null)
                {
                    firstSprite = sp2;
                }
                if (pPrev2 == null)
                {
                    firstSprite = sp1;
                }
                if (pNext1 == null)
                {
                    lastSprite = sp2;
                }
                if (pNext2 == null)
                {
                    lastSprite = sp1;
                }
            }
        }

        public void moveSpriteToFront(CSprite pSpr)
        {
            if (lastSprite != null)
            {
                int nLayer = pSpr.sprLayer;

                // Exchange the sprite with the next one until the end of the list
                while (pSpr != lastSprite)
                {
                    CSprite pSprNext = pSpr.objNext;
                    if (pSprNext == null)
                    {
                        break;
                    }

                    if (pSprNext.sprLayer > nLayer)
                    {
                        break;
                    }

                    swapSprites(pSpr, pSprNext);
                }
            }
        }

        public void moveSpriteToBack(CSprite pSpr)
        {
            if (lastSprite != null)
            {
                int nLayer = pSpr.sprLayer;

                // Exchange the sprite with the previous one until the end of the list
                while (pSpr != firstSprite)
                {
                    CSprite pSprPrev = pSpr.objPrev;
                    if (pSprPrev == null)
                    {
                        break;
                    }

                    if (pSprPrev.sprLayer < nLayer)
                    {
                        break;
                    }

                    swapSprites(pSprPrev, pSpr);
                }
            }
        }

        public void moveSpriteBefore(CSprite pSprToMove, CSprite pSprDest)
        {
            if (pSprToMove.sprLayer == pSprDest.sprLayer)
            {
                CSprite pSpr = pSprToMove.objPrev;
                while (pSpr != null && pSpr != pSprDest)
                {
                    pSpr = pSpr.objPrev;
                }
                if (pSpr != null)
                {
                    // Exchange the sprite with the previous one until the second one is reached
                    // TODO: could be optimized (no need to loop, we only need to update 6 sprites)
                    CSprite pPrevSpr = pSprToMove;
                    do
                    {
                        pPrevSpr = pSprToMove.objPrev;
                        if (pPrevSpr == null)
                        {
                            break;
                        }

                        swapSprites(pSprToMove, pPrevSpr);

                    } while (pPrevSpr != pSprDest);
                }
            }
        }

        public void moveSpriteAfter(CSprite pSprToMove, CSprite pSprDest)
        {
            if (pSprToMove.sprLayer == pSprDest.sprLayer)
            {
                CSprite pSpr = pSprToMove.objNext;
                while (pSpr != null && pSpr != pSprDest)
                {
                    pSpr = pSpr.objNext;
                }
                if (pSpr != null)
                {
                    // Exchange the sprite with the next one until the second one is reached
                    // TODO: could be optimized (no need to loop, we only need to update 6 sprites)
                    CSprite pNextSpr;
                    do
                    {
                        pNextSpr = pSprToMove.objNext;
                        if (pNextSpr == null)
                        {
                            break;
                        }
                        swapSprites(pSprToMove, pNextSpr);
                    } while (pNextSpr != pSprDest);
                }
            }
        }

        public bool isSpriteBefore(CSprite pSpr, CSprite pSprDest)
        {
            if (pSpr.sprLayer < pSprDest.sprLayer)
            {
                return true;
            }
            if (pSpr.sprLayer > pSprDest.sprLayer)
            {
                return false;
            }
            if (pSpr.sprZOrder < pSprDest.sprZOrder)
            {
                return true;
            }
            return false;
        }

        public bool isSpriteAfter(CSprite pSpr, CSprite pSprDest)
        {
            if (pSpr.sprLayer > pSprDest.sprLayer)
            {
                return true;
            }
            if (pSpr.sprLayer < pSprDest.sprLayer)
            {
                return false;
            }
            if (pSpr.sprZOrder > pSprDest.sprZOrder)
            {
                return true;
            }
            return false;
        }

        public CSprite getFirstSprite(int nLayer, int dwFlags)
        {
            CSprite pSpr = null;
            pSpr = firstSprite;

            // Get internal layer number
            int nIntLayer = nLayer;
            if (nLayer != -1)
            {
                nIntLayer *= 2;
                if ((dwFlags & GS_BACKGROUND) == 0)
                {
                    nIntLayer++;
                }
            }

            // Search for first sprite in this layer
            while (pSpr != null)
            {
                // Correct layer?
                if (nIntLayer == -1 || pSpr.sprLayer == nIntLayer)
                {
                    break;
                }

                // Break if a greater layer is reached (means there is no sprite in the layer)
                if (pSpr.sprLayer > nIntLayer)
                {
                    pSpr = null;
                    break;
                }

                // Next sprite
                pSpr = pSpr.objNext;
            }
            return pSpr;
        }

        public CSprite getNextSprite(CSprite pSpr, int dwFlags)
        {
            if (pSpr != null)
            {
                int nLayer = pSpr.sprLayer;

                if ((dwFlags & GS_BACKGROUND) != 0)
                {
                    // Look for next background sprite
                    while ((pSpr = pSpr.objNext) != null)
                    {
                        // Active
                        if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) == 0)
                        {
                            // If only same layer, stop
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                pSpr = null;
                                break;
                            }
                        }
                        // Background
                        else
                        {
                            // If only same layer
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                // Different layer? end
                                if (pSpr.sprLayer != nLayer)
                                {
                                    pSpr = null;
                                }
                            }

                            // Stop
                            break;
                        }
                    }
                }
                else
                {
                    // Look for next active sprite
                    while ((pSpr = pSpr.objNext) != null)
                    {
                        // Background
                        if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) != 0)
                        {
                            // If only same layer, stop
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                pSpr = null;
                                break;
                            }
                        }
                        // Active
                        else
                        {
                            // If only same layer
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                // Different layer? end
                                if (pSpr.sprLayer != nLayer)
                                {
                                    pSpr = null;
                                }
                            }

                            // Stop
                            break;
                        }
                    }
                }
            }
            return pSpr;
        }

        public CSprite getPrevSprite(CSprite pSpr, int dwFlags)
        {
            if (pSpr != null)
            {
                int nLayer = pSpr.sprLayer;

                if ((dwFlags & GS_BACKGROUND) != 0)
                {
                    // Look for previous background sprite
                    while ((pSpr = pSpr.objPrev) != null)
                    {
                        // Active
                        if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) == 0)
                        {
                            // If only same layer, stop
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                pSpr = null;
                                break;
                            }
                        }
                        // Background
                        else
                        {
                            // If only same layer
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                // Different layer? end
                                if (pSpr.sprLayer != nLayer)
                                {
                                    pSpr = null;
                                }
                            }
                            // Stop
                            break;
                        }
                    }
                }
                else
                {
                    // Look for next active sprite
                    while ((pSpr = pSpr.objPrev) != null)
                    {
                        // Background
                        if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) != 0)
                        {
                            // If only same layer, stop
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                pSpr = null;
                                break;
                            }
                        }
                        // Active
                        else
                        {
                            // If only same layer
                            if ((dwFlags & GS_SAMELAYER) != 0)
                            {
                                // Different layer? end
                                if (pSpr.sprLayer != nLayer)
                                {
                                    pSpr = null;
                                }
                            }
                            // Stop
                            break;
                        }
                    }
                }
            }
            return pSpr;
        }

        public void showSprite(CSprite ptSpr, bool showFlag)
        {
            if (ptSpr != null)
            {
                // Show sprite
                if (showFlag)
                {
                    if ((ptSpr.sprFlags & CSprite.SF_HIDDEN)!=0)
                    {
                        ptSpr.sprFlags &= ~CSprite.SF_HIDDEN;
                        ptSpr.sprFlags |= CSprite.SF_REAF;
                    }
                }
                // Hide sprite (next loop)
                else
                {
                    if ((ptSpr.sprFlags & CSprite.SF_HIDDEN) == 0)
                    {
                        ptSpr.sprFlags |= CSprite.SF_HIDDEN;
                        ptSpr.sprFlags |= CSprite.SF_REAF;
                    }
                }
            }
        }

        public void killSprite(CSprite ptSprToKill)
        {
            CSprite ptSpr = ptSprToKill;

            // Si sprite OwnerDraw, appeler routine
            if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) != 0)
            {
                ptSpr.sprRout.drawableKill();
            }

            // Free object
            winFreeSprite(ptSpr);
        }

        public void activeSprite(CSprite ptSpr, int activeFlag, CRect reafRect)
        {
	        // Active only one
	        if (ptSpr != null)
	        {
		        switch (activeFlag)
		        {
                        // Deactivate
			        case 0x0000:					// AS_DEACTIVATE
				        ptSpr.sprFlags |= CSprite.SF_INACTIF|CSprite.SF_REAF;	// Warning: no break
                        break;
        				
                        // Redraw (= activate only for next loop)
			        case 0x0001:					// AS_REDRAW:
                        ptSpr.sprFlags |= CSprite.SF_REAF;
				        break;
        				
                        // Activate
			        case 0x0002:					// AS_ACTIVATE:
                        ptSpr.sprFlags &= ~CSprite.SF_INACTIF;
				        break;
        				
                        // Enable
			        case 0x0004:					// AS_ENABLE:
                        ptSpr.sprFlags &= ~CSprite.SF_DISABLED;
				        break;
        				
                        // Disable
			        case 0x0008:					// AS_DISABLE:
                        ptSpr.sprFlags |= CSprite.SF_DISABLED;
				        break;
		        }
	        }
	        // Active all
	        else
	        {
		        ptSpr = firstSprite;
		        while (ptSpr != null)
		        {
			        switch (activeFlag)
			        {
                            // Deactivate
				        case 0x0000:			    //AS_DEACTIVATE:
                            ptSpr.sprFlags |= CSprite.SF_INACTIF | CSprite.SF_REAF;
                            break;
	
                            // Redraw (= activate only for next loop)
				        case 0x0001:			    // AS_REDRAW:
                            ptSpr.sprFlags |= CSprite.SF_REAF;
					        break;
        					
                            // Redraw (= activate only for next loop) - all sprites except background sprites
				        case 0x0011:			    // AS_REDRAW_NOBKD:
                            if ((ptSpr.sprFlags & (CSprite.SF_HIDDEN | CSprite.SF_BACKGROUND)) == 0)
					        {
                                ptSpr.sprFlags |= CSprite.SF_REAF;
					        }
					        break;
        					
                            // Activate
				        case 0x0002:			    // AS_ACTIVATE:
                            ptSpr.sprFlags &= ~CSprite.SF_INACTIF;
					        break;
        					
                            // Enable
				        case 0x0004:			    // AS_ENABLE:
                            ptSpr.sprFlags &= ~CSprite.SF_DISABLED;
					        break;
        					
                            // Disable
				        case 0x0008:			    // AS_DISABLE:
                            ptSpr.sprFlags |= CSprite.SF_DISABLED;
					        break;
        					
				        default:
                            ptSpr.sprFlags &= ~CSprite.SF_INACTIF;
					        break;
			        }
			        ptSpr = ptSpr.objNext;
		        }
	        }
        }

        public void delSprite(CSprite ptSprToDel)
        {
            killSprite(ptSprToDel);
        }

        public void delSpriteFast(CSprite ptSpr)
        {
            killSprite(ptSpr);
        }

        public CMask getSpriteMask(CSprite ptSpr, short newImg, int nFlags, int newAngle, float newScaleX, float newScaleY)
        {
	        if (ptSpr != null)
	        {
		        if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) == 0)
		        {
			        short nImg = newImg;
			        if (nImg == -1)
			        {
				        nImg = ptSpr.sprImg;
			        }
			        if (nImg != -1)
			        {
				        CImage pImage = bank.getImageFromHandle(nImg);
				        return pImage.getMask(nFlags, newAngle, newScaleX, newScaleY);
			        }
		        }
		        else
		        {
			        return ptSpr.sprRout.drawableGetMask(nFlags);
		        }
	        }
	        return null;        	
        }

        public void spriteUpdate()
        {
	        CSprite ptSpr;
        	
	        // Parcours table des sprites
	        ptSpr = firstSprite;
	        while (ptSpr != null)
	        {
		        // Mise a jour nouvelles caracteristiques sprite
		        if ((ptSpr.sprFlags & CSprite.SF_REAF) != 0)
		        {
			        ptSpr.sprX = ptSpr.sprXnew;
			        ptSpr.sprY = ptSpr.sprYnew;
			        ptSpr.sprX1 = ptSpr.sprX1new;
			        ptSpr.sprY1 = ptSpr.sprY1new;
			        ptSpr.sprX2 = ptSpr.sprX2new;
			        ptSpr.sprY2 = ptSpr.sprY2new;
                    ptSpr.sprScaleX=ptSpr.sprScaleXnew;
                    ptSpr.sprScaleY=ptSpr.sprScaleYnew;
                    ptSpr.sprAngle=ptSpr.sprAnglenew;
                    if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) == 0)
			        {
				        ptSpr.sprImg = ptSpr.sprImgNew;
			        }
		        }
       		
		        // Next sprite
		        ptSpr = ptSpr.objNext;
	        }
        }

        public const int PSF_HOTSPOT = 0x0001;		// Take hot spot into account
        public const int PSF_NOTRANSP = 0x0002;		// Non transparent image... ignored in PasteSpriteEffect

        public void pasteSpriteEffect(SpriteBatchEffect batch, short iNum, int iX, int iY, int flags, int effect, int effectParam)
        {
            int x1, y1;
            CImage ptei;

            do
            {
                // Calcul adresse image et coordonnees
                ptei = bank.getImageFromHandle((short) iNum);
                if (ptei == null)
                {
                    break;
                }

                x1 = iX;
                if ((flags & PSF_HOTSPOT) != 0)
                {
                    x1 -= ptei.xSpot;
                }

                y1 = iY;
                if ((flags & PSF_HOTSPOT) != 0)
                {
                    y1 -= ptei.ySpot;
                }

                // Blit
                tempRect.X = x1+xOffset;
                tempRect.Y = y1+yOffset;
                tempRect.Width = ptei.width;
                tempRect.Height = ptei.height;

                Color color = Color.White;
                float coef = 1.0F;
                if ((effect & BOP_RGBAFILTER) != 0)
                {
                    color = CServices.getColorAlpha(effectParam & 0xFFFFFF);
                    coef = (float)(((effectParam >> 24) & 0xFF) / 255.0);
                }
                else if ((effect&BOP_MASK) == BOP_BLEND)
                {
                    coef = (float)((128 - effectParam) / 128.0);
                }
                color= color* coef;
                Texture2D texture = ptei.image;
                Nullable<Rectangle> sourceRect = null;
                if (ptei.mosaic != 0)
                {
                    texture = app.imageBank.mosaics[ptei.mosaic];
                    sourceRect = ptei.mosaicRectangle;
                }
                batch.Draw(texture, tempRect, sourceRect, color, effect & BOP_MASK, effectParam);

            } while (false);
        }

        public void spriteDraw(SpriteBatchEffect batch)
        {
	        CSprite ptSpr;
	        CSprite ptFirstSpr;
        	
	        ptFirstSpr = firstSprite;
	        if (ptFirstSpr == null)
	        {
                app.run.draw_QuickDisplay(batch);
                return;
	        }
        	
	        // Inkeffects: rajouter blitmodes
	        // Scan sprite table
            bool bQuickDisplay = true;
	        for (ptSpr = ptFirstSpr; ptSpr != null; ptSpr = ptSpr.objNext)
	        {
                if (bQuickDisplay)
                {
                    if ((ptSpr.sprFlags & CSprite.SF_TRUEOBJECT) != 0)
                    {
                        app.run.draw_QuickDisplay(batch);
                        bQuickDisplay = false;
                    }
                }

		        // Si sprite inactif et pas SF_REAF, pas d'affichage
                if ((ptSpr.sprFlags & (CSprite.SF_HIDDEN | CSprite.SF_DISABLED)) != 0)
		        {
			        continue;
		        }
        		
		        // Sprite ownerdraw
                if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) != 0 && ptSpr.sprRout != null)
		        {
			        ptSpr.sprRout.drawableDraw(batch, ptSpr, bank, ptSpr.sprX1+xOffset, ptSpr.sprY1+yOffset);
		        }       		
		        // Normal sprite
		        else
		        {
                    CImage ptei = bank.getImageFromHandle(ptSpr.sprImg);
                    if (ptei != null)
                    {
                        int hsx = 0;
                        int hsy = 0;
                        if ((ptSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
                        {
                            hsx = ptei.xSpot;
                            hsy = ptei.ySpot;
                        }
                        tempRect.X = ptSpr.sprX + xOffset;
                        tempRect.Y = ptSpr.sprY + yOffset;
                        tempRect.Width = (int)(ptei.width*ptSpr.sprScaleX);
                        tempRect.Height = (int)(ptei.height*ptSpr.sprScaleY);
                        vector.X = hsx;
                        vector.Y = hsy;
                        Texture2D texture = ptei.image;
                        Nullable<Rectangle> sourceRect = null;
                        if (ptei.mosaic != 0)
                        {
                            texture = app.imageBank.mosaics[ptei.mosaic];
                            sourceRect = ptei.mosaicRectangle;
                        }
                        batch.Draw(texture, tempRect, sourceRect, ptSpr.rgb, (float)((-ptSpr.sprAngle*Math.PI)/180), vector, ptSpr.sprEffect);
                    }
		        }
                ptSpr.sprFlags &= ~(CSprite.SF_REAF | CSprite.SF_REAFINT);
	        }
            if (bQuickDisplay)
            {
                app.run.draw_QuickDisplay(batch);
            }
        }

        public CSprite getLastSprite(int nLayer, int dwFlags)
        {
            CSprite pSpr = lastSprite;

            // Get internal layer number
            int nIntLayer = nLayer;
            if (nLayer != -1)
            {
                nIntLayer *= 2;
                if ((dwFlags & GS_BACKGROUND) == 0)
                {
                    nIntLayer++;
                }
            }

            // Search for first sprite in this layer
            while (pSpr != null)
            {
                // Correct layer?
                if (nIntLayer == -1 || pSpr.sprLayer == nIntLayer)
                {
                    break;
                }

                // Break if a greater layer is reached (means there is no sprite in the layer)
                if (pSpr.sprLayer < nIntLayer)
                {
                    pSpr = null;
                    break;
                }

                // Next sprite
                pSpr = pSpr.objPrev;
            }
            return pSpr;
        }

        public CSprite winAllocSprite()
        {
            CSprite spr = new CSprite(bank);
            if (firstSprite == null)
            {
                firstSprite = spr;
                lastSprite = spr;
                spr.objPrev = null;
                spr.objNext = null;
                return spr;
            }
            CSprite previous = lastSprite;
            previous.objNext = spr;
            spr.objPrev = previous;
            spr.objNext = null;
            lastSprite = spr;
            return spr;
        }

        public void winFreeSprite(CSprite spr)
        {
            if (spr.objPrev == null)
            {
                firstSprite = spr.objNext;
            }
            else
            {
                spr.objPrev.objNext = spr.objNext;
            }
            if (spr.objNext != null)
            {
                spr.objNext.objPrev = spr.objPrev;
            }
            else
            {
                lastSprite = spr.objPrev;
            }
        }

        public void winSetColMode(short c)
        {
            colMode = c;
        }


        public CSprite spriteCol_TestPoint(CSprite firstSpr, short nLayer, int xp, int yp, int dwFlags)
        {
	        CSprite ptSpr = firstSpr;
	        if (ptSpr == null)
	        {
		        ptSpr = firstSprite;
	        }
	        else
	        {
		        ptSpr = ptSpr.objNext;
	        }
        	
	        bool bAllLayers = (nLayer == LAYER_ALL);
            bool bEvenNoCol = ((dwFlags & SCF_EVENNOCOL) != 0);
	        short wAllLayerBit;
	        if ((dwFlags & SCF_BACKGROUND) != 0)
	        {
		        wAllLayerBit = 0;
		        if (nLayer != LAYER_ALL)
		        {
			        nLayer = (short) (nLayer * 2);
		        }
	        }
	        else
	        {
		        wAllLayerBit = 1;
		        if (nLayer != LAYER_ALL)
		        {
			        nLayer = (short) (nLayer * 2 + 1);
		        }
	        }
        	
	        // Recherche des autres sprites
	        for (; ptSpr != null; ptSpr = ptSpr.objNext)
	        {
		        if (!bAllLayers)	
		        {
			        if (ptSpr.sprLayer < nLayer)
			        {
				        continue;
			        }
			        if (ptSpr.sprLayer > nLayer)
			        {
				        break;
			        }
		        }
		        else if ((ptSpr.sprLayer & 1) != wAllLayerBit)
		        {
			        continue;
		        }
        		
		        // Can test collision with this one?
		        if (bEvenNoCol || (ptSpr.sprFlags & CSprite.SF_RAMBO) != 0)
		        {
			        if (xp >= ptSpr.sprX1 && xp < ptSpr.sprX2 && yp >= ptSpr.sprY1 && yp < ptSpr.sprY2)
			        {
				        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
        				
				        // Collides => test background flags
				        if ((dwFlags & SCF_BACKGROUND) != 0)
				        {
					        // Platform? no collision if we check collisions with obstacles
                            if ((ptSpr.sprFlags & CSprite.SF_PLATFORM) != 0)
					        {
						        if ((dwFlags & SCF_OBSTACLE) != 0)
						        {
							        continue;
						        }
        						
						        // Platform and check collisions with platforms => get platform collision mask
						        nGetColMaskFlag = CMask.GCMF_PLATFORM;
					        }
				        }
        			
				        // Box collision mode
                        if (colMode == CM_BOX || (ptSpr.sprFlags & CSprite.SF_COLBOX) != 0)
				        {
					        return ptSpr;
				        }
        					
				        // Fine collision mode, test bit image
				        CMask pMask = getSpriteMask(ptSpr, (short)-1, nGetColMaskFlag, ptSpr.sprAngle, ptSpr.sprScaleX, ptSpr.sprScaleY);
				        if (pMask != null)
				        {
					        int dy = yp - ptSpr.sprY1;
					        if (dy < pMask.height)
					        {
						        int offset = dy * pMask.lineWidth;
						        int dx = xp - ptSpr.sprX1;
						        if (dx < pMask.width)
						        {
							        offset += dx / 16;
							        short m = (short) (0x8000 >> (dx & 15));							
							        if ((pMask.mask[offset] & m) != 0)
							        {
								        return ptSpr;
							        }
						        }
					        }
				        }
			        }
		        }
	        }
	        return null;
        }

        public CSprite spriteCol_TestPointOne(CSprite firstSpr, short nLayer, int xp, int yp, int dwFlags)
        {
            CSprite ptSpr = firstSpr;

            bool bAllLayers = (nLayer == LAYER_ALL);
            bool bEvenNoCol = true;
            short wAllLayerBit;
            if ((dwFlags & SCF_BACKGROUND) != 0)
            {
                wAllLayerBit = 0;
                if (nLayer != LAYER_ALL)
                {
                    nLayer = (short)(nLayer * 2);
                }
            }
            else
            {
                wAllLayerBit = 1;
                if (nLayer != LAYER_ALL)
                {
                    nLayer = (short)(nLayer * 2 + 1);
                }
            }

            // Recherche des autres sprites
            for (; ptSpr != null; ptSpr = ptSpr.objNext)
            {
                if (!bAllLayers)
                {
                    if (ptSpr.sprLayer < nLayer)
                    {
                        continue;
                    }
                    if (ptSpr.sprLayer > nLayer)
                    {
                        break;
                    }
                }
                else if ((ptSpr.sprLayer & 1) != wAllLayerBit)
                {
                    continue;
                }

                // Can test collision with this one?
                if (bEvenNoCol || (ptSpr.sprFlags & CSprite.SF_RAMBO) != 0)
                {
                    if (xp >= ptSpr.sprX1 && xp < ptSpr.sprX2 && yp >= ptSpr.sprY1 && yp < ptSpr.sprY2)
                    {
                        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;

                        // Box collision mode
                        if (colMode == CM_BOX || (ptSpr.sprFlags & CSprite.SF_COLBOX) != 0)
                        {
                            return ptSpr;
                        }

                        // Fine collision mode, test bit image
                        CMask pMask = getSpriteMask(ptSpr, (short)-1, nGetColMaskFlag, ptSpr.sprAngle, ptSpr.sprScaleX, ptSpr.sprScaleY);
                        if (pMask != null)
                        {
                            int dy = yp - ptSpr.sprY1;
                            if (dy < pMask.height)
                            {
                                int offset = dy * pMask.lineWidth;
                                int dx = xp - ptSpr.sprX1;
                                if (dx < pMask.width)
                                {
                                    offset += dx / 16;
                                    short m = (short)(0x8000 >> (dx & 15));
                                    if ((pMask.mask[offset] & m) != 0)
                                    {
                                        return ptSpr;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public CArrayList spriteCol_TestSprite_All(CSprite ptSpr, short newImg, int newX,  int newY, int newAngle, float newScaleX, float newScaleY, int dwFlags)
        {
	        int nLayer;
	        int cm = colMode;
	        CArrayList list = null;
        	
	        if (ptSpr == null || newImg<0)
	        {
		        return null;
	        }
            if ((ptSpr.sprFlags & CSprite.SF_COLBOX) != 0)
	        {
		        cm = CM_BOX;
	        }
        	
	        nLayer = ptSpr.sprLayer;		// collisions always in the same layer
        	
	        // Flags to test
	        if ((dwFlags & SCF_BACKGROUND) != 0)
	        {
		        // Test with background sprites => even layer
		        nLayer &= ~1;
	        }
	        else
	        {
		        // Test with active sprites => collisions must be enabled
                if ((ptSpr.sprFlags & CSprite.SF_RAMBO) == 0)
		        {
			        return null;
		        }
        		
		        // Test with active sprites => odd layer
		        nLayer |= 1;
	        }
        	
	        int x1 = newX;
	        int y1 = newY;
	        int x2 = x1;
	        int y2 = y1;
        	
	        CMask pMask = null;

            // Get sprite mask
            if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW)!=0)
	        {
		        x2+=ptSpr.sprX2-ptSpr.sprX1;
		        y2+=ptSpr.sprY2-ptSpr.sprY1;
	        }
	        else
	        {
		        CImage ifo=bank.getImageInfoEx(newImg, newAngle, newScaleX, newScaleY);
                if ((ptSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
		        {
			        x1 -= ifo.xSpot;
			        y1 -= ifo.ySpot;
		        }
		        x2 = x1 + ifo.width;
		        y2 = y1 + ifo.height;	
	        }
	        CSprite ptSpr1;
	        CMask pMask2;
	        for (ptSpr1 = firstSprite; ptSpr1 != null; ptSpr1 = ptSpr1.objNext)
	        {
		        // Same layer?
		        if (ptSpr1.sprLayer < nLayer)
		        {
			        continue;
		        }
		        if (ptSpr1.sprLayer > nLayer)
		        {
			        break;
		        }
        		
		        // Collision flags?
                if ((ptSpr1.sprFlags & CSprite.SF_RAMBO) == 0)
		        {
			        continue;
		        }
        		
		        // Collides?
		        if (x1 < ptSpr1.sprX2 && x2 > ptSpr1.sprX1 && y1 < ptSpr1.sprY2 && y2 > ptSpr1.sprY1)
		        {
			        if (ptSpr1 == ptSpr)
			        {
				        continue;
			        }
        			
			        // The sprite must not be deleted
			        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
        			
			        // Collides => test background flags
			        if ((dwFlags & SCF_BACKGROUND) != 0)
			        {
				        // Platform? no collision if we check collisions with obstacles
                        if ((ptSpr1.sprFlags & CSprite.SF_PLATFORM) != 0)
				        {
					        if ((dwFlags & SCF_OBSTACLE) != 0)
					        {
						        continue;
					        }
					        // Platform and check collisions with platforms => get platform collision mask
					        nGetColMaskFlag = CMask.GCMF_PLATFORM;
				        }
			        }
        			
			        // Box mode?
                    if (cm == CM_BOX || (ptSpr1.sprFlags & CSprite.SF_COLBOX) != 0)
			        {
				        if (list == null)
				        {
					        list = new CArrayList();
				        }
				        list.add(ptSpr1.sprExtraInfo);
			        }		
			        // Fine collision mode
			        else
			        {
				        // Calculate mask?
				        if (pMask == null)
				        {
					        pMask = getSpriteMask(ptSpr, newImg, CMask.GCMF_OBSTACLE, newAngle, newScaleX, newScaleY);
					        if (pMask==null)
					        {
						        if (list == null)
						        {
							        list = new CArrayList();
						        }
						        list.add(ptSpr1.sprExtraInfo);
						        continue;
					        }
				        }
				        // Test collision with second sprite
				        pMask2 = getSpriteMask(ptSpr1, -1, nGetColMaskFlag, ptSpr1.sprAngle, ptSpr1.sprScaleX, ptSpr1.sprScaleY);
				        if (pMask2 != null)
				        {
					        if (pMask.testMask(0, x1, y1, pMask2, 0, ptSpr1.sprX1, ptSpr1.sprY1))
					        {
						        if (list == null)
						        {
							        list = new CArrayList();
						        }
						        list.add(ptSpr1.sprExtraInfo);
					        }
				        }
			        }
		        }
	        }
	        return list;
        }

        public CSprite spriteCol_TestSprite(CSprite ptSpr, short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, uint dwFlags)
        {
	        int nLayer;
        	
	        if (ptSpr == null)
	        {
		        return null;
	        }
            if ((ptSpr.sprFlags & CSprite.SF_COLBOX) != 0)
	        {
		        colMode = CM_BOX;
	        }
        	
	        nLayer = ptSpr.sprLayer;		// collisions always in the same layer
        	
	        // Flags to test
	        if ((dwFlags & SCF_BACKGROUND) != 0)
	        {
		        // Test with background sprites => even layer
		        nLayer &= ~1;
	        }
	        else
	        {
		        // Test with active sprites => collisions must be enabled
                if ((ptSpr.sprFlags & CSprite.SF_RAMBO) == 0)
		        {
			        return null;
		        }
        		
		        // Test with active sprites => odd layer
		        nLayer |= 1;
	        }
        	
	        int x1 = newX;
	        int y1 = newY;
	        int x2 = x1;
	        int y2 = y1;
        	
	        CMask pMask = null;
        	
	        // Si angle = 0 et coefs = 1.0f, ou si ownerdraw
	        //		=> m�thode normale
	        //
	        // Si angle != 0 ou coefs != 1.0f
	        //		=> appeler PrepareSpriteColMask pour calculer la bounding box et r�cup�rer le masque si d�j� calcul�
	        //		=> puis r�cup�rer le masque avec CompleteSpriteColMask quand on en a besoin
        	
	        // Image sprite not stretched and not rotated, or owner draw sprite?
	        // Get sprite mask
            if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW)!=0)
	        {
		        x2+=ptSpr.sprX2-ptSpr.sprX1;
		        y2+=ptSpr.sprY2-ptSpr.sprY1;
	        }
	        else
	        {		
		        CImage ifo=bank.getImageInfoEx(newImg, newAngle, newScaleX, newScaleY);
                if ((ptSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
		        {
			        x1 -= ifo.xSpot;
			        y1 -= ifo.ySpot;
		        }
		        x2 = x1 + ifo.width;
		        y2 = y1 + ifo.height;
	        }
        	
	        // Take subHt into account
	        if (subHt != 0)
	        {
		        int nHeight = (y2 - y1);
		        if (subHt > nHeight)
		        {
			        subHt = nHeight;
		        }
		        y1 += nHeight - subHt;
	        }
        	
	        // Compare sprite box with current box
	        CSprite ptSpr1;
	        for (ptSpr1 = firstSprite; ptSpr1 != null; ptSpr1 = ptSpr1.objNext)
	        {
		        // Same layer?
		        if (ptSpr1.sprLayer < nLayer)
		        {
			        continue;
		        }
		        if (ptSpr1.sprLayer > nLayer)
		        {
			        break;
		        }
        		
		        // Collision flags?
                if ((ptSpr1.sprFlags & CSprite.SF_RAMBO) == 0)
		        {
			        continue;
		        }
        		
		        // Collides?
		        if (x1 < ptSpr1.sprX2 && x2 > ptSpr1.sprX1 && y1 < ptSpr1.sprY2 && y2 > ptSpr1.sprY1)
		        {
			        if (ptSpr1 == ptSpr)
			        {
				        continue;
			        }
        			
			        // The sprite must not be deleted
                    if ((ptSpr1.sprFlags & CSprite.SF_TOKILL) == 0)
			        {
				        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
        				
				        // Collides => test background flags
				        if ((dwFlags & SCF_BACKGROUND) != 0)
				        {
					        // Platform? no collision if we check collisions with obstacles
                            if ((ptSpr1.sprFlags & CSprite.SF_PLATFORM) != 0)
					        {
						        if ((dwFlags & SCF_OBSTACLE) != 0)
						        {
							        continue;
						        }
        						
						        // Platform and check collisions with platforms => get platform collision mask
						        nGetColMaskFlag = CMask.GCMF_PLATFORM;
					        }
				        }
        				
				        // Box mode?
                        if (colMode == CM_BOX || (ptSpr1.sprFlags & CSprite.SF_COLBOX) != 0)
				        {
					        return ptSpr1;
				        }
				        // Fine collision mode
				        else
				        {
					        // Calculate mask?
					        if (pMask == null)
					        {
						        pMask = getSpriteMask(ptSpr, newImg, CMask.GCMF_OBSTACLE, newAngle, newScaleX, newScaleY);
						        if (pMask==null)
						        {							
							        return ptSpr1;
						        }
					        }
        					
					        // Take subHt into account
					        int yMaskBits = 0;
					        int nMaskHeight = pMask.height;
					        if (subHt != 0)
					        {
						        if (subHt > nMaskHeight)
						        {
							        subHt = nMaskHeight;
						        }
						        yMaskBits = nMaskHeight - subHt;
						        nMaskHeight = subHt;
					        }
        					
					        // Test collision with second sprite
					        CMask pMask2 = getSpriteMask(ptSpr1, -1, nGetColMaskFlag, ptSpr1.sprAngle, ptSpr1.sprScaleX, ptSpr1.sprScaleY);
					        if (pMask2 != null)
					        {
						        if (pMask.testMask(yMaskBits, x1, y1, pMask2, 0, ptSpr1.sprX1, ptSpr1.sprY1))
						        {
							        return ptSpr1;
						        }
					        }
				        }
			        }
		        }
	        }
	        return null;
        }

        public CSprite spriteCol_TestRect(CSprite firstSpr, int nLayer, int xp, int yp, int wp, int hp, int dwFlags)
        {
	        CSprite ptSpr = firstSpr;
	        if (ptSpr == null)
	        {
		        ptSpr = firstSprite;
	        }
	        else
	        {
		        ptSpr = ptSpr.objNext;
	        }
        	
	        bool bAllLayers = (nLayer == LAYER_ALL);
	        bool bEvenNoCol = ((dwFlags & SCF_EVENNOCOL) != 0);
	        short wAllLayerBit;
	        if ((dwFlags & SCF_BACKGROUND) != 0)
	        {
		        wAllLayerBit = 0;
		        if (nLayer != LAYER_ALL)
		        {
			        nLayer = nLayer * 2;
		        }
	        }
	        else
	        {
		        wAllLayerBit = 1;
		        if (nLayer != LAYER_ALL)
		        {
			        nLayer = nLayer * 2 + 1;
		        }
	        }
        	
	        // Recherche des autres sprites
	        for (; ptSpr != null; ptSpr = ptSpr.objNext)
	        {
		        if (!bAllLayers)	// todo: optimisation: faire une boucle diff�rente pour le mode all layers et une autre pour skipper les 1ers layers
		        {
			        if (ptSpr.sprLayer < nLayer)
			        {
				        continue;
			        }
			        if (ptSpr.sprLayer > nLayer)
			        {
				        break;
			        }
		        }
		        else if ((ptSpr.sprLayer & 1) != wAllLayerBit)
		        {
			        continue;
		        }
        		
		        // Can test collision with this one?
                if (bEvenNoCol || (ptSpr.sprFlags & CSprite.SF_RAMBO) != 0)
		        {
			        if (xp <= ptSpr.sprX2 && xp + wp > ptSpr.sprX1 && yp <= ptSpr.sprY2 && yp + hp > ptSpr.sprY1)
			        {
                        if ((ptSpr.sprFlags & CSprite.SF_TOKILL) == 0)	// should never happen
				        {
					        int nGetColMaskFlag = CMask.GCMF_OBSTACLE;
        					
					        // Collides => test background flags
					        if ((dwFlags & SCF_BACKGROUND) != 0)
					        {
						        // Platform? no collision if we check collisions with obstacles
                                if ((ptSpr.sprFlags & CSprite.SF_PLATFORM) != 0)
						        {
							        if ((dwFlags & SCF_OBSTACLE) != 0)
							        {
								        continue;
							        }
							        // Platform and check collisions with platforms => get platform collision mask
							        nGetColMaskFlag = CMask.GCMF_PLATFORM;
						        }
					        }
        					
					        // Box collision mode
                            if (colMode == CM_BOX || (ptSpr.sprFlags & CSprite.SF_COLBOX) != 0)
					        {
						        return ptSpr;
					        }
        					
					        // Fine collision mode, test bit image
					        CMask pMask = getSpriteMask(ptSpr, -1, nGetColMaskFlag, ptSpr.sprAngle, ptSpr.sprScaleX, ptSpr.sprScaleY);
					        if (pMask != null)
					        {
						        if (pMask.testRect(0, xp - ptSpr.sprX1, yp - ptSpr.sprY1, wp, hp))
						        {
							        return ptSpr;
						        }
					        }
				        }
			        }
		        }
	        }
	        return null;
        }

    }
}
