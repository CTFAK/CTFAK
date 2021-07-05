/* Copyright (c) 1996-2013 Clickteam
 *
 * This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
 * 
 * Permission is hereby granted to any person obtaining a legal copy 
 * of Clickteam Multimedia Fusion 2 to use or modify this source code for 
 * debugging, optimizing, or customizing applications created with 
 * Clickteam Multimedia Fusion 2.  Any other use of this source code is prohibited.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
//----------------------------------------------------------------------------------
//
// CSPRITEGEN : Generateur de sprites
//
//----------------------------------------------------------------------------------
package Sprites;

import java.util.ArrayList;

import Application.CRunApp;
import Application.CRunFrame;
import Banks.CImage;
import Banks.CImageBank;
import Banks.CImageInfo;
import Objects.CObject;
import OpenGL.GLRenderer;
import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CRect;
import android.graphics.Rect;

public class CSpriteGen {
	// ActiveSprite
	public static final int AS_DEACTIVATE = 0x0000; // Desactive un sprite actif
	public static final int AS_REDRAW = 0x0001; // Reaffiche un sprite inactif
	public static final int AS_ACTIVATE = 0x0002; // Active un sprite inactif
	public static final int AS_ENABLE = 0x0004;
	public static final int AS_DISABLE = 0x0008;
	// public static final int AS_REDRAW_NOBKD=0x0011;
	public static final int AS_REDRAW_RECT = 0x0020;
	public static final int GS_BACKGROUND = 0x0001;
	public static final int GS_SAMELAYER = 0x0002;
	public static final short CM_BOX = 0;
	public static final short CM_BITMAP = 1;
	public static final short PSCF_CURRENTSURFACE = 0x0001;
	public static final short PSCF_TEMPSURFACE = 0x0002;
	public static final short LAYER_ALL = -1;
	public static final int EFFECT_NONE = 0;
	public static final int EFFECT_SEMITRANSP = 1;
	// public static final int EFFECT_INVERTED=2;
	// public static final int EFFECT_XOR=3;
	// public static final int EFFECT_AND=4;
	// public static final int EFFECT_OR=5;
	public static final int EFFECTFLAG_TRANSPARENT = 0x10000000;
	public static final int EFFECTFLAG_ANTIALIAS = 0x20000000;
	public static final int EFFECT_MASK = 0xFFFF;
	public CSprite firstSprite;
	public CSprite lastSprite;
	CRunFrame frame;
	CRunApp app;
	CImageBank bank;
	public short colMode;
	Rect srcRect;
	Rect dstRect;
	
	public CSpriteGen() {
	}

	public CSpriteGen(CImageBank b, CRunApp a, CRunFrame f) {
		firstSprite = null;
		lastSprite = null;
		bank = b;
		frame = f;
		app = a;
		srcRect = new Rect();
		dstRect = new Rect();
	}

	/* public void dump()
    {
        Log.Log("*** SPRITE LIST ***");

        int i = 0;

        for(CSprite spr = firstSprite; spr != null; spr = spr.objNext)
        {
            Log.Log(i + ")  " + spr.sprX + ", " + spr.sprY);
            ++ i;
        }

        Log.Log("******");
    } */

    public void spriteClear()
    {
        CSprite ptSpr;
        CSprite ptSprNext;

        //do
        //{
                //ptSpr = firstSprite;
                //[self sprite_Intersect:frame.leEditWinWidth withHeight:frame.leEditWinHeight];

                // Parcours table des sprites
                //while (ptSpr != null)
                for(ptSpr = firstSprite; ptSpr != null;  ptSpr = ptSprNext)
                {
                        // Tester si sprite a virer ou a cacher
                        if ((ptSpr.sprFlags & CSprite.SF_TOHIDE) != 0)
                        {
                                ptSpr.killSpriteZone();
                                ptSpr.sprFlags &= ~CSprite.SF_TOHIDE;
                                ptSpr.sprFlags |= CSprite.SF_HIDDEN;
                                //ptSpr.sprX1z = ptSpr.sprY1z = -1;
                        }
                        if ((ptSpr.sprFlags & CSprite.SF_TOKILL) != 0)
                        {
                                ptSprNext = ptSpr.objNext;
                                killSprite(ptSpr, false);
                                ptSpr = ptSprNext;
                        }
                        else
                        {
                                //ptSpr = ptSpr.objNext;
                            	ptSprNext = ptSpr.objNext;
                       	
                        }
                }

        //} while (false);
    }

	// Ajout d'un sprite normal a la liste
	// -------------------------------------------------------------
	public CSprite addSprite(final int xSpr, final int ySpr, final short iSpr, final short wLayer,
			final int nZOrder, final int backSpr, final int sFlags, final CObject extraInfo) {

	    // Log.Log("Adding sprite at " + xSpr + ", " + ySpr + " (" + extraInfo + ")");
        // dump();

		// Verifie validite fenetre et image
		CSprite ptSpr = null;
		// Alloue de la place pour l'objet
		ptSpr = winAllocSprite();

		// Store info
		ptSpr.bank = bank;
		ptSpr.sprFlags = (sFlags | CSprite.SF_REAF);
		ptSpr.sprFlags &= ~(CSprite.SF_TOKILL | CSprite.SF_REAFINT
				/*| CSprite.SF_RECALCSURF*/ | CSprite.SF_OWNERDRAW | CSprite.SF_OWNERSAVE);
		ptSpr.sprLayer = (short) (wLayer * 2);
		if ((sFlags & CSprite.SF_BACKGROUND) == 0) {
			ptSpr.sprLayer++;
		}
		ptSpr.sprZOrder = nZOrder;
		ptSpr.sprX = ptSpr.sprXnew = xSpr;
		ptSpr.sprY = ptSpr.sprYnew = ySpr;
		ptSpr.sprImg = ptSpr.sprImgNew = iSpr;
		ptSpr.sprExtraInfo = extraInfo;
		ptSpr.sprEffect = CSprite.EFFECTFLAG_TRANSPARENT;
		ptSpr.sprEffectParam = 0;
		ptSpr.sprScaleX = ptSpr.sprScaleY = 1.0f;		// ptSpr.sprTempScaleX = ptSpr.sprTempScaleY = 
		ptSpr.sprAngle = 0;		// ptSpr.sprTempAngle = 
		// ptSpr.sprTempImg = 0;
		// ptSpr.sprX1z = ptSpr.sprY1z = -1;
		// ptSpr.sprSf = ptSpr.sprTempSf = NULL;
		// ptSpr.sprColMask = ptSpr.sprTempColMask = NULL;

		// Background color
		//ptSpr.sprBackColor = 0;
		//if ((sFlags & CSprite.SF_FILLBACK) != 0) {
		//	ptSpr.sprBackColor = backSpr;
		//}
		// Update bounding box
		ptSpr.updateBoundingBox();

		// Copy new bounding box to old
		ptSpr.sprX1 = ptSpr.sprX1new;
		ptSpr.sprY1 = ptSpr.sprY1new;
		ptSpr.sprX2 = ptSpr.sprX2new;
		ptSpr.sprY2 = ptSpr.sprY2new;

        // dump();
        // Log.Log("Sorting...");

		// Sort sprite
		sortLastSprite(ptSpr);

        // dump();

		return ptSpr;
	}

	// ------------------------------------------------------;
	// Ajout d'un sprite ownerdraw a la liste des sprites ;
	// ------------------------------------------------------;
	public CSprite addOwnerDrawSprite(final int x1, final int y1, final int x2, final int y2,
			final short wLayer, final int nZOrder, final int backSpr, final int sFlags,
			final CObject extraInfo, final IDrawable sprProc) {
		CSprite ptSpr = winAllocSprite();

		// Init coord sprite
		ptSpr.sprX = ptSpr.sprXnew = x1;
		ptSpr.sprY = ptSpr.sprYnew = y1;
		ptSpr.sprX1new = ptSpr.sprX1 = x1;
		ptSpr.sprY1new = ptSpr.sprY1 = y1;
		ptSpr.sprX2new = ptSpr.sprX2 = x2;
		ptSpr.sprY2new = ptSpr.sprY2 = y2;
		//ptSpr.sprX1z = ptSpr.sprY1z = -1;
		ptSpr.sprLayer = (short) (wLayer * 2);
		if ((sFlags & CSprite.SF_BACKGROUND) == 0) {
			ptSpr.sprLayer++;
		}
		ptSpr.sprZOrder = nZOrder;
		ptSpr.sprExtraInfo = extraInfo;
		ptSpr.sprRout = sprProc;
		ptSpr.sprFlags = (sFlags | CSprite.SF_REAF | CSprite.SF_OWNERDRAW);
		ptSpr.sprFlags &= ~(CSprite.SF_TOKILL | CSprite.SF_REAFINT /*| CSprite.SF_RECALCSURF*/);
		ptSpr.sprEffect = CSprite.EFFECTFLAG_TRANSPARENT;
		ptSpr.sprEffectParam = 0;
		ptSpr.sprScaleX = ptSpr.sprScaleY = 1.0f;		// ptSpr.sprTempScaleX = ptSpr.sprTempScaleY = 
		ptSpr.sprAngle = 0;		// ptSpr.sprTempAngle = 
		// ptSpr.sprTempImg = 0;
		// ptSpr.sprSf = ptSpr.sprTempSf = NULL;
		// ptSpr.sprColMask = ptSpr.sprTempColMask = 0;

		// Background color
		//ptSpr.sprBackColor = 0;
		//if ((sFlags & CSprite.SF_FILLBACK) != 0) {
		//	ptSpr.sprBackColor = backSpr;
		//}

		// Trier le sprite avec ses predecesseurs
		sortLastSprite(ptSpr);

		return ptSpr;
	}

	// --------------------------------------------------;
	// Modification des caracteristiques d'un sprite ;
	// --------------------------------------------------;
	// Out: new ID sprite (si change de fenetre) ;
	// --------------------------------------------------;
	public void modifSprite(CSprite ptSpr, final int xSpr, final int ySpr, final short iSpr) {

        if (ptSpr != null && ptSpr.sprXnew != xSpr || ptSpr.sprYnew != ySpr || ptSpr.sprImgNew != iSpr)
		{
            // Recalc surface?
            //if (ptSpr.sprImgNew != iSpr
            //        && (ptSpr.sprAngle != 0 || ptSpr.sprScaleX != 1.0f || ptSpr.sprScaleY != 1.0f)) {
            //    ptSpr.sprFlags |= CSprite.SF_RECALCSURF;
            //    ptSpr.sprColMask = null;
            //}

            // Change
            ptSpr.sprXnew = xSpr;
            ptSpr.sprYnew = ySpr;
            ptSpr.sprImgNew = iSpr;

            // Update bounding box
            ptSpr.updateBoundingBox();

            // Set redraw flag
            ptSpr.sprFlags |= CSprite.SF_REAF;
        }
	}

	public void modifSpriteEx(CSprite ptSpr, final int xSpr, final int ySpr, final short iSpr,
			final float fScaleX, final float fScaleY, final boolean bResample, float nAngle,
			final boolean bAntiA) {

		if (ptSpr == null)
			return;

		boolean bOldResample = ((ptSpr.sprFlags & CSprite.SF_SCALE_RESAMPLE) != 0);
		boolean bOldAntiA = ((ptSpr.sprFlags & CSprite.SF_ROTATE_ANTIA) != 0);

		nAngle %= 360;
		if (nAngle < 0) {
			nAngle += 360;
		}

		//boolean bRecalcSurf = false;
		//if (ptSpr.sprScaleX != fScaleX || ptSpr.sprScaleY != fScaleY
		//		|| bResample != bOldResample || ptSpr.sprAngle != nAngle
		//		|| bOldAntiA != bAntiA) {
		//	bRecalcSurf = true;
		//}

		// Comparison
		if (ptSpr.sprXnew != xSpr || ptSpr.sprYnew != ySpr || ptSpr.sprImgNew != iSpr ||
			ptSpr.sprScaleX != fScaleX || ptSpr.sprScaleY != fScaleY || ptSpr.sprAngle != nAngle || 
			bResample != bOldResample || bOldAntiA != bAntiA)
		{
			// Recalc surface?
			//if (bRecalcSurf != false
			//		|| ptSpr.sprImgNew != iSpr
			//		&& (ptSpr.sprAngle != 0 || ptSpr.sprScaleX != 1.0f || ptSpr.sprScaleY != 1.0f)) {
			//	ptSpr.sprFlags |= CSprite.SF_RECALCSURF;
			// ptSpr.sprColMask = null;
			//}

			// Change
			ptSpr.sprXnew = xSpr;
			ptSpr.sprYnew = ySpr;
			ptSpr.sprImgNew = iSpr;

			ptSpr.sprScaleX = fScaleX;
			if ( ptSpr.sprScaleX < 0.0f )
				ptSpr.sprScaleX = 0.0f;
			ptSpr.sprScaleY = fScaleY;
			if ( ptSpr.sprScaleY < 0.0f )
				ptSpr.sprScaleY = 0.0f;
			ptSpr.sprAngle = nAngle;
			ptSpr.sprFlags &= ~(CSprite.SF_SCALE_RESAMPLE | CSprite.SF_ROTATE_ANTIA);
			if (bResample != false) {
				ptSpr.sprFlags |= CSprite.SF_SCALE_RESAMPLE;
			}
			if (bAntiA != false) {
				ptSpr.sprFlags |= CSprite.SF_ROTATE_ANTIA;
			}

			// Update bounding box
			ptSpr.updateBoundingBox();

			// Set redraw flag
			ptSpr.sprFlags |= CSprite.SF_REAF;

			// Update colliding sprites
			//if ((ptSpr.sprFlags & CSprite.SF_HIDDEN) == 0
			//		&& (ptSpr.sprFlags & (CSprite.SF_INACTIF | CSprite.SF_REAF | CSprite.SF_DISABLED)) != 0) {
			//	ptSpr.sprFlags |= CSprite.SF_REAF;
			//}
		}
	}

	// ------------------------------------------;
	// Modification dd l'ink effect d'un sprite ;
	// ------------------------------------------;
	public void modifSpriteEffect(CSprite ptSpr, final int effect, final int effectParam) {

        if (ptSpr != null)
		{
			ptSpr.sprEffect = effect;
			ptSpr.sprEffectParam = effectParam;

			// reafficher sprite
			ptSpr.sprFlags |= CSprite.SF_REAF;
        }
	}

	// --------------------------------------------------;
	// Modification des caracteristiques d'un sprite ;
	// --------------------------------------------------;
	public void modifOwnerDrawSprite(CSprite ptSprModif, final int x1, final int y1,
			final int x2, final int y2) {

        if (ptSprModif != null)
		{
			ptSprModif.sprX1new = x1;
			ptSprModif.sprY1new = y1;
			ptSprModif.sprX2new = x2;
			ptSprModif.sprY2new = y2;

			// Reafficher sprite
			ptSprModif.sprFlags |= CSprite.SF_REAF;
		}
	}

	// //////////////////////////////////////////////
	//
	// Set sprite layer
	//
	public void setSpriteLayer(CSprite ptSpr, int nLayer) {
		if (ptSpr == null) {
			return;
		}

		int nNewLayer = nLayer * 2;
		if ((ptSpr.sprFlags & CSprite.SF_BACKGROUND) == 0) {
			nNewLayer++;
		}

        CSprite pSprNext;
        CSprite pSprPrev;
		if (ptSpr.sprLayer != (short) nNewLayer) {
			int nOldLayer = ptSpr.sprLayer;
			ptSpr.sprLayer = (short) nNewLayer;

			if (nOldLayer < nNewLayer) {
				if (lastSprite != null) {
					// Exchange the sprite with the next one until the end of
					// the list or the next layer
					while (ptSpr != lastSprite) {
						pSprNext = ptSpr.objNext;
						if (pSprNext == null) {
							break;
						}
						if (pSprNext.sprLayer > (short) nNewLayer) {
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
			} else {
				if (firstSprite != null) {
					// Exchange the sprite with the previous one until the
					// beginning of the list or the previous layer
					while (ptSpr != firstSprite) {
						pSprPrev = ptSpr.objPrev;
						if (pSprPrev == null) {
							break;
						}
						if (pSprPrev.sprLayer <= (short) nNewLayer) {
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

			// Take the last zorder value plus one (but the caller must update
			// this value after calling SetSpriteLayer)
			pSprPrev = ptSpr.objPrev;
			if (pSprPrev == null || pSprPrev.sprLayer != ptSpr.sprLayer) {
				ptSpr.sprZOrder = 1;
			} else {
				ptSpr.sprZOrder = pSprPrev.sprZOrder + 1;
			}
		}

        ptSpr.sprFlags |= CSprite.SF_REAF;

		// Force redraw
		if ((ptSpr.sprFlags & CSprite.SF_HIDDEN) == 0) {
			activeSprite(ptSpr, AS_REDRAW, null);
		}
	}

	// //////////////////////////////////////////////
	//
	// Set sprite scale
	//
	public void setSpriteScale(CSprite ptSpr, float fScaleX, float fScaleY,
			boolean bResample) {
		if (ptSpr != null) {
			// Autoriser les valeurs n�gatives et faire ReverseX / ReverseY
			if (fScaleX < (float) 0.0) {
				fScaleX = (float) 0.0;
			}
			if (fScaleY < (float) 0.0) {
				fScaleY = (float) 0.0;
			}
			boolean bOldResample = ((ptSpr.sprFlags & CSprite.SF_SCALE_RESAMPLE) != 0);

			if (ptSpr.sprScaleX != fScaleX || ptSpr.sprScaleY != fScaleY
					|| bResample != bOldResample) {
				ptSpr.sprScaleX = fScaleX;
				ptSpr.sprScaleY = fScaleY;
				ptSpr.sprFlags |= (CSprite.SF_REAF /*| CSprite.SF_RECALCSURF*/);
				ptSpr.sprFlags &= ~CSprite.SF_SCALE_RESAMPLE;
				if (bResample) {
					ptSpr.sprFlags |= CSprite.SF_SCALE_RESAMPLE;
				}
				// ptSpr.sprColMask = null;
				ptSpr.updateBoundingBox();
			}
		}
	}

	// //////////////////////////////////////////////
	//
	// Set sprite angle
	//
	public void setSpriteAngle(CSprite ptSpr, float nAngle, boolean bAntiA) {
		if (ptSpr != null) {
			boolean bOldAntiA = ((ptSpr.sprFlags & CSprite.SF_ROTATE_ANTIA) != 0);
			nAngle %= 360;
			if (nAngle < 0) {
				nAngle += 360;
			}
			if (ptSpr.sprAngle != nAngle || bOldAntiA != bAntiA) {
				ptSpr.sprAngle = nAngle;
				ptSpr.sprFlags &= ~CSprite.SF_ROTATE_ANTIA;
				if (bAntiA) {
					ptSpr.sprFlags |= CSprite.SF_ROTATE_ANTIA;
				}
				ptSpr.sprFlags |= (CSprite.SF_REAF /*| CSprite.SF_RECALCSURF*/);
				// ptSpr.sprColMask = null;
				ptSpr.updateBoundingBox();
			}
		}
	}

	// --------------------------------------------------;
	// Trier le dernier sprite avec ses predecesseurs ;
	// --------------------------------------------------;
	public void sortLastSprite(CSprite ptSprOrg) {

		CSprite ptSpr = ptSprOrg;
		CSprite ptSpr1;
		CSprite ptSprPrev;
		CSprite ptSprNext;
		short wLayer;

		// ==================================================;
		// Tri sur les numeros de plan uniquement ;
		// ==================================================;
		// On part du principe que les autres sprites sont deja tries
		//
		// On peut mieux optimiser!!! (= parcours tant que plan < et 1 seul
		// echange a la fin...)

		// Look for sprite layer
		wLayer = ptSpr.sprLayer;
		ptSpr1 = ptSpr.objPrev;
		while (ptSpr1 != null) {
			if (wLayer >= ptSpr1.sprLayer) // On arrete des qu'on trouve un plan
			// <
			{
				break;
			}

			// Si plan trouve >, alors on echange les sprites
			ptSprPrev = ptSpr1.objPrev;
			if (ptSprPrev == null) {
				firstSprite = ptSpr;
			} else {
				ptSprPrev.objNext = ptSpr; // Next ( Prev ( spr1 ) ) = spr
			}
			ptSprNext = ptSpr.objNext;
			if (ptSprNext == null) {
				lastSprite = ptSpr1;
			} else {
				ptSprNext.objPrev = ptSpr1; // Prev ( Next ( spr ) ) = spr1
			}
			ptSpr.objPrev = ptSpr1.objPrev; // Prev ( spr ) = Prev ( spr1 )
			ptSpr1.objPrev = ptSpr;

			ptSpr1.objNext = ptSpr.objNext; // Next ( spr1 ) = Next ( spr )
			ptSpr.objNext = ptSpr1;
			ptSpr1 = ptSpr;

			ptSpr1 = ptSpr1.objPrev; // sprite precedent
		}

		// Same layer? sort by z-order value
		if (ptSpr1 != null && wLayer == ptSpr1.sprLayer) {
			int nZOrder = ptSpr.sprZOrder;

			while (ptSpr1 != null && wLayer == ptSpr1.sprLayer) {
				if (nZOrder >= ptSpr1.sprZOrder) // On arrete des qu'on trouve
				// un plan <
				{
					break;
				}

				// Si plan trouve >, alors on echange les sprites
				ptSprPrev = ptSpr1.objPrev;
				if (ptSprPrev == null) {
					firstSprite = ptSpr;
				} else {
					ptSprPrev.objNext = ptSpr; // Next ( Prev ( spr1 ) ) = spr
				}
				ptSprNext = ptSpr.objNext;
				if (ptSprNext == null) {
					lastSprite = ptSpr1;
				} else {
					ptSprNext.objPrev = ptSpr1; // Prev ( Next ( spr ) ) = spr1
				}
				ptSpr.objPrev = ptSpr1.objPrev; // Prev ( spr ) = Prev ( spr1 )
				ptSpr1.objPrev = ptSpr;

				ptSpr1.objNext = ptSpr.objNext; // Next ( spr1 ) = Next ( spr )
				ptSpr.objNext = ptSpr1;
				ptSpr1 = ptSpr;

				ptSpr1 = ptSpr1.objPrev; // sprite precedent
			}
		}
	}

	public void swapSprites(CSprite sp1, CSprite sp2) {
		// Security
		if (sp1 == sp2) {
			return;
		}

		CSprite pPrev1 = sp1.objPrev;
		CSprite pNext1 = sp1.objNext;

		CSprite pPrev2 = sp2.objPrev;
		CSprite pNext2 = sp2.objNext;

		// Exchange layers . non !
		// WORD holdLayer = sp1.sprLayer;
		// sp1.sprLayer = sp2.sprLayer;
		// sp2.sprLayer = holdLayer;

		// Exchange z-order values
		int nZOrder = sp1.sprZOrder;
		sp1.sprZOrder = sp2.sprZOrder;
		sp2.sprZOrder = nZOrder;

		// Exchange sprites

		// Several cases
		//
		// 1. pPrev1, sp1, sp2, pNext2
		//
		// pPrev1.next = sp2
		// sp2.prev = pPrev1;
		// sp2.next = sp1;
		// sp1.prev = sp2;
		// sp1.next = pNext2
		// pNext2.prev = sp1
		//
		if (pNext1 == sp2) {
			if (pPrev1 != null) {
				pPrev1.objNext = sp2;
			}
			sp2.objPrev = pPrev1;
			sp2.objNext = sp1;
			sp1.objPrev = sp2;
			sp1.objNext = pNext2;
			if (pNext2 != null) {
				pNext2.objPrev = sp1;
			}

			// Update first & last sprites
			if (pPrev1 == null) {
				firstSprite = sp2;
			}
			if (pNext2 == null) {
				lastSprite = sp1;
			}
		}

		// 2. pPrev2, sp2, sp1, pNext1
		//
		// pPrev2.next = sp1
		// sp1.prev = pPrev2;
		// sp1.next = sp2;
		// sp2.prev = sp1;
		// sp2.next = pNext1
		// pNext1.prev = sp2
		//
		else if (pNext2 == sp1) {
			if (pPrev2 != null) {
				pPrev2.objNext = sp1;
			}
			sp1.objPrev = pPrev2;
			sp1.objNext = sp2;
			sp2.objPrev = sp1;
			sp2.objNext = pNext1;
			if (pNext1 != null) {
				pNext1.objPrev = sp2;
			}

			// Update first & last sprites
			if (pPrev2 == null) {
				firstSprite = sp1; // *ptPtsObj = (UINT)sp1;
			}
			if (pNext1 == null) {
				lastSprite = sp2; // *(ptPtsObj+1) = (UINT)sp2;
			}
		} else {
			if (pPrev1 != null) {
				pPrev1.objNext = sp2;
			}
			if (pNext1 != null) {
				pNext1.objPrev = sp2;
			}
			sp1.objPrev = pPrev2;
			sp1.objNext = pNext2;
			if (pPrev2 != null) {
				pPrev2.objNext = sp1;
			}
			if (pNext2 != null) {
				pNext2.objPrev = sp1;
			}
			sp2.objPrev = pPrev1;
			sp2.objNext = pNext1;

			// Update first & last sprites
			if (pPrev1 == null) {
				firstSprite = sp2;
			}
			if (pPrev2 == null) {
				firstSprite = sp1;
			}
			if (pNext1 == null) {
				lastSprite = sp2;
			}
			if (pNext2 == null) {
				lastSprite = sp1;
			}
		}
	}

	public void moveSpriteToFront(CSprite pSpr) {
		if (lastSprite != null) {
			int nLayer = pSpr.sprLayer;

			// Exchange the sprite with the next one until the end of the list
			while (pSpr != lastSprite) {
				CSprite pSprNext = pSpr.objNext;
				if (pSprNext == null) {
					break;
				}

				if (pSprNext.sprLayer > nLayer) {
					break;
				}

				swapSprites(pSpr, pSprNext);
			}

			// Force redraw
			if ((pSpr.sprFlags & CSprite.SF_HIDDEN) == 0) {
				activeSprite(pSpr, AS_REDRAW, null);
			}
		}
	}

	public void moveSpriteToBack(CSprite pSpr) {
		if (lastSprite != null) {
			int nLayer = pSpr.sprLayer;

			// Exchange the sprite with the previous one until the end of the
			// list
			while (pSpr != firstSprite) {
				CSprite pSprPrev = pSpr.objPrev;
				if (pSprPrev == null) {
					break;
				}

				if (pSprPrev.sprLayer < nLayer) {
					break;
				}

				swapSprites(pSprPrev, pSpr);
			}

			// Force redraw
			if ((pSpr.sprFlags & CSprite.SF_HIDDEN) == 0) {
				activeSprite(pSpr, AS_REDRAW, null);
			}
		}
	}

	public void moveSpriteBefore(CSprite pSprToMove, CSprite pSprDest) {
		if (pSprToMove.sprLayer == pSprDest.sprLayer) {
			CSprite pSpr = pSprToMove.objPrev;
			while (pSpr != null && pSpr != pSprDest) {
				pSpr = pSpr.objPrev;
			}
			if (pSpr != null) {
				// Exchange the sprite with the previous one until the second
				// one is reached
				CSprite pPrevSpr = pSprToMove;
				do {
					pPrevSpr = pSprToMove.objPrev;
					if (pPrevSpr == null) {
						break;
					}

					swapSprites(pSprToMove, pPrevSpr);

				} while (pPrevSpr != pSprDest);

				// Force redraw
				if ((pSprToMove.sprFlags & CSprite.SF_HIDDEN) == 0) {
					activeSprite(pSprToMove, AS_REDRAW, null);
				}
			}
		}
	}

	public void moveSpriteAfter(CSprite pSprToMove, CSprite pSprDest) {
		if (pSprToMove.sprLayer == pSprDest.sprLayer) {
			CSprite pSpr = pSprToMove.objNext;
			while (pSpr != null && pSpr != pSprDest) {
				pSpr = pSpr.objNext;
			}
			if (pSpr != null) {
				// Exchange the sprite with the next one until the second one is
				// reached
				CSprite pNextSpr;
				do {
					pNextSpr = pSprToMove.objNext;
					if (pNextSpr == null) {
						break;
					}
					swapSprites(pSprToMove, pNextSpr);
				} while (pNextSpr != pSprDest);

				// Force redraw
				if ((pSprToMove.sprFlags & CSprite.SF_HIDDEN) == 0) {
					activeSprite(pSprToMove, AS_REDRAW, null);
				}
			}
		}
	}

	public boolean isSpriteBefore(CSprite pSpr, CSprite pSprDest) {
		if (pSpr.sprLayer < pSprDest.sprLayer) {
			return true;
		}
		if (pSpr.sprLayer > pSprDest.sprLayer) {
			return false;
		}
		if (pSpr.sprZOrder < pSprDest.sprZOrder) {
			return true;
		}
		return false;
	}

	public boolean isSpriteAfter(CSprite pSpr, CSprite pSprDest) {
		if (pSpr.sprLayer > pSprDest.sprLayer) {
			return true;
		}
		if (pSpr.sprLayer < pSprDest.sprLayer) {
			return false;
		}
		if (pSpr.sprZOrder > pSprDest.sprZOrder) {
			return true;
		}
		return false;
	}

	public CSprite getFirstSprite(int nLayer, int dwFlags) {
		CSprite pSpr = null;
		pSpr = firstSprite;

		// Get internal layer number
		int nIntLayer = nLayer;
		if (nLayer != -1) {
			nIntLayer *= 2;
			if ((dwFlags & GS_BACKGROUND) == 0) {
				nIntLayer++;
			}
		}

		// Search for first sprite in this layer
		while (pSpr != null) {
			// Correct layer?
			if (nIntLayer == -1 || pSpr.sprLayer == nIntLayer) {
				break;
			}

			// Break if a greater layer is reached (means there is no sprite in
			// the layer)
			if (pSpr.sprLayer > nIntLayer) {
				pSpr = null;
				break;
			}

			// Next sprite
			pSpr = pSpr.objNext;
		}
		return pSpr;
	}

	public CSprite getNextSprite(CSprite pSpr, int dwFlags) {
		if (pSpr != null) {
			int nLayer = pSpr.sprLayer;

			if ((dwFlags & GS_BACKGROUND) != 0) {
				// Look for next background sprite
				while ((pSpr = pSpr.objNext) != null) {
					// Active
					if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) == 0) {
						// If only same layer, stop
						if ((dwFlags & GS_SAMELAYER) != 0) {
							pSpr = null;
							break;
						}
					}
					// Background
					else {
						// If only same layer
						if ((dwFlags & GS_SAMELAYER) != 0) {
							// Different layer? end
							if (pSpr.sprLayer != nLayer) {
								pSpr = null;
							}
						}

						// Stop
						break;
					}
				}
			} else {
				// Look for next active sprite
				while ((pSpr = pSpr.objNext) != null) {
					// Background
					if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) != 0) {
						// If only same layer, stop
						if ((dwFlags & GS_SAMELAYER) != 0) {
							pSpr = null;
							break;
						}
					}
					// Active
					else {
						// If only same layer
						if ((dwFlags & GS_SAMELAYER) != 0) {
							// Different layer? end
							if (pSpr.sprLayer != nLayer) {
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

	public CSprite getPrevSprite(CSprite pSpr, int dwFlags) {
		if (pSpr != null) {
			int nLayer = pSpr.sprLayer;

			if ((dwFlags & GS_BACKGROUND) != 0) {
				// Look for previous background sprite
				while ((pSpr = pSpr.objPrev) != null) {
					// Active
					if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) == 0) {
						// If only same layer, stop
						if ((dwFlags & GS_SAMELAYER) != 0) {
							pSpr = null;
							break;
						}
					}
					// Background
					else {
						// If only same layer
						if ((dwFlags & GS_SAMELAYER) != 0) {
							// Different layer? end
							if (pSpr.sprLayer != nLayer) {
								pSpr = null;
							}
						}
						// Stop
						break;
					}
				}
			} else {
				// Look for next active sprite
				while ((pSpr = pSpr.objPrev) != null) {
					// Background
					if ((pSpr.sprFlags & CSprite.SF_BACKGROUND) != 0) {
						// If only same layer, stop
						if ((dwFlags & GS_SAMELAYER) != 0) {
							pSpr = null;
							break;
						}
					}
					// Active
					else {
						// If only same layer
						if ((dwFlags & GS_SAMELAYER) != 0) {
							// Different layer? end
							if (pSpr.sprLayer != nLayer) {
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

	// ------------------------------------------;
	// Montrer / cacher un sprite ;
	// ------------------------------------------;
	public void showSprite(CSprite ptSpr, boolean showFlag) {
		if (ptSpr != null) {
			// Show sprite
			if (showFlag) {
				ptSpr.sprFlags &= ~(CSprite.SF_HIDDEN | CSprite.SF_TOHIDE);
				if ((ptSpr.sprFlags & CSprite.SF_INACTIF) != 0) {
					ptSpr.sprFlags |= CSprite.SF_REAF;
				}
			}

			// Hide sprite (next loop)
			else {
				if ((ptSpr.sprFlags & CSprite.SF_HIDDEN) == 0)
				{
					//if (ptSpr.sprX1z == -1 && ptSpr.sprY1z == -1)
					//{
						ptSpr.sprFlags |= CSprite.SF_HIDDEN;
					//} else {
					//	ptSpr.sprFlags |= CSprite.SF_TOHIDE;
					//	if ((ptSpr.sprFlags & CSprite.SF_INACTIF) != 0) {
					//		ptSpr.sprFlags |= CSprite.SF_REAF;
					//	}
					//}
				}
			}
		}
	}

	// ------------------------------------------;
	// Changement d'etat d'un sprite ;
	// ------------------------------------------;
	public void activeSprite(CSprite ptSpr, int activeFlag, CRect reafRect) {
		// Active only one
		if (ptSpr != null) {
			switch (activeFlag) {
			// Deactivate
			case 0x0000: // AS_DEACTIVATE
				ptSpr.sprFlags |= CSprite.SF_INACTIF; // Warning: no break

            // Redraw (= activate only for next loop)
			case 0x0001: // AS_REDRAW:
                ptSpr.sprFlags |= CSprite.SF_REAF;
				break;

			// Activate
			case 0x0002: // AS_ACTIVATE:
				ptSpr.sprFlags &= ~CSprite.SF_INACTIF;
				break;

			// Enable
			case 0x0004: // AS_ENABLE:
				ptSpr.sprFlags &= ~CSprite.SF_DISABLED;
				break;

			// Disable
			case 0x0008: // AS_DISABLE:
				ptSpr.sprFlags |= CSprite.SF_DISABLED;
				break;
			}
		}
		// Active all
		else {
			ptSpr = firstSprite;
			while (ptSpr != null) {
				switch (activeFlag) {
				// Deactivate
				case 0x0000: // AS_DEACTIVATE:
					ptSpr.sprFlags |= CSprite.SF_INACTIF;

					// Redraw (= activate only for next loop)
				case 0x0001: // AS_REDRAW:
					if ((ptSpr.sprFlags & CSprite.SF_HIDDEN) == 0) {
						ptSpr.sprFlags |= CSprite.SF_REAF;
					}
					break;

				// Redraw (= activate only for next loop) - all sprites except
				// background sprites
				case 0x0011: // AS_REDRAW_NOBKD:
					if ((ptSpr.sprFlags & (CSprite.SF_HIDDEN | CSprite.SF_BACKGROUND)) == 0) {
						ptSpr.sprFlags |= CSprite.SF_REAF;
					}
					break;

				// Activate
				case 0x0002: // AS_ACTIVATE:
					ptSpr.sprFlags &= ~CSprite.SF_INACTIF;
					break;

				// Enable
				case 0x0004: // AS_ENABLE:
					ptSpr.sprFlags &= ~CSprite.SF_DISABLED;
					break;

				// Disable
				case 0x0008: // AS_DISABLE:
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

	// ----------------------;
	// Virer un sprite ;
	// ----------------------;
	public void killSprite(CSprite ptSprToKill, boolean bFast) {
		CSprite ptSpr = ptSprToKill;

		// Log.Log ("Killing " + ptSprToKill.sprX + ", " + ptSprToKill.sprY);
		// dump();

		ptSpr.killSpriteZone();

		if (bFast == false)
        {
            if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) != 0)
                ptSpr.sprRout.spriteKill (ptSpr);
        }

		// Free object
		winFreeSprite(ptSpr);

	    // dump();
	}

	// ----------------------------------------------;
	// Enlever un sprite de la liste des sprites ;
	// ----------------------------------------------;
	public void delSprite(CSprite ptSprToDel) {
		if (ptSprToDel != null) {
			CSprite ptSpr = ptSprToDel;

			ptSpr.sprFlags &= ~CSprite.SF_RAMBO;
			ptSpr.sprFlags |= CSprite.SF_TOKILL | CSprite.SF_REAF;
			if ((ptSpr.sprFlags & CSprite.SF_HIDDEN) == 0
					&& (ptSpr.sprFlags & CSprite.SF_INACTIF) != 0) {
				ptSpr.sprFlags &= ~CSprite.SF_INACTIF;
			}
		}
	}

	// ----------------------------------------------------------;
	// Enlever un sprite immediatement de la liste des sprites ;
	// ----------------------------------------------------------;
	public void delSpriteFast(CSprite ptSpr) {
		killSprite(ptSpr, true);
	}

	// //////////////////////////////////////////////
	//
	// Recalc sprite surface (rotation or stretch)
	//
	/*
	public void recalcSpriteSurface(CSprite ptSpr) {
		// Free collision mask
		ptSpr.sprColMask = null;

		// Original image?
		if (ptSpr.sprAngle == 0 && ptSpr.sprScaleX == 1.0f
				&& ptSpr.sprScaleY == 1.0f) {
		}
		// Stretched or rotated image
		else {
			// Already calculated?
			if (ptSpr.sprTempColMask != null && ptSpr.sprImgNew == ptSpr.sprTempImg
					&& ptSpr.sprAngle == ptSpr.sprTempAngle
					&& ptSpr.sprScaleX == ptSpr.sprTempScaleX
					&& ptSpr.sprScaleY == ptSpr.sprTempScaleY) {

				ptSpr.sprColMask = ptSpr.sprTempColMask;
				ptSpr.sprTempColMask = null;
				return;
			}

			// Get image surface
			CImage ptei;
			ptei = bank.getImageFromHandle(ptSpr.sprImgNew);
			if (ptei == null) {
				return;
			}

			// Create or resize surface
			int w = ptSpr.sprX2new - ptSpr.sprX1new;
			int h = ptSpr.sprY2new - ptSpr.sprY1new;
			if (w <= 0) {
				w = 1;
			}
			if (h <= 0) {
				h = 1;
			}
		}
	}
	*/

	// //////////////////////////////////////////////
	//
	// Get Sprite Mask
	//

	public CMask getSpriteMask(CSprite ptSpr, short newImg, int nFlags, float newAngle, double newScaleX, double newScaleY) {
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
                    if(pImage != null)
                    	return pImage.getMask(nFlags, Math.round(newAngle), newScaleX, newScaleY);
                    else
                    	return null;
                }
            }
            else
            {
                return ptSpr.sprRout.spriteGetMask();
            }
        }
		return null;
	}

	// ------------------------------;
	// Mise a jour des sprites ;
	// ------------------------------;
	public void spriteUpdate() {
		CSprite ptSpr;

		// Parcours table des sprites
		/*
		ptSpr = firstSprite;
		while (ptSpr != null) {
			// Mise a jour nouvelles caracteristiques sprite
			if ((ptSpr.sprFlags & CSprite.SF_REAF) != 0) {
				ptSpr.sprX = ptSpr.sprXnew;
				ptSpr.sprY = ptSpr.sprYnew;
				ptSpr.sprX1 = ptSpr.sprX1new;
				ptSpr.sprY1 = ptSpr.sprY1new;
				ptSpr.sprX2 = ptSpr.sprX2new;
				ptSpr.sprY2 = ptSpr.sprY2new;
				if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) == 0) {
					ptSpr.sprImg = ptSpr.sprImgNew;
				}
			}

			// Recalculate surface
			//if ((ptSpr.sprFlags & CSprite.SF_RECALCSURF) != 0) {
			//	ptSpr.sprFlags &= ~CSprite.SF_RECALCSURF;
			//	recalcSpriteSurface(ptSpr);
			//}

			// Next sprite
			ptSpr = ptSpr.objNext;
		}
		*/
		for (ptSpr = firstSprite; ptSpr != null; ptSpr = ptSpr.objNext) {
			// Mise a jour nouvelles caracteristiques sprite
			if ((ptSpr.sprFlags & CSprite.SF_REAF) != 0) {
				ptSpr.sprX = ptSpr.sprXnew;
				ptSpr.sprY = ptSpr.sprYnew;
				ptSpr.sprX1 = ptSpr.sprX1new;
				ptSpr.sprY1 = ptSpr.sprY1new;
				ptSpr.sprX2 = ptSpr.sprX2new;
				ptSpr.sprY2 = ptSpr.sprY2new;
				if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) == 0) {
					ptSpr.sprImg = ptSpr.sprImgNew;
				}
			}

			// Recalculate surface
			//if ((ptSpr.sprFlags & CSprite.SF_RECALCSURF) != 0) {
			//	ptSpr.sprFlags &= ~CSprite.SF_RECALCSURF;
			//	recalcSpriteSurface(ptSpr);
			//}
			// Next sprite --- INTERNALLY IN FOR
		}
	}



	// ------------------------------------------------------;
	// Affichage d'une image en sprite dans une fenetre ;
	// ------------------------------------------------------;
	public static final int PSF_HOTSPOT = 0x0001; // Take hot spot into account
	public static final int PSF_NOTRANSP = 0x0002; // Non transparent image...

	// ignored in
	// PasteSpriteEffect

	public void pasteSpriteEffect(short iNum, int iX, int iY, int flags, int effect, int effectParam) {
		int x1, y1;
		CImage ptei;

		do {
			// Calcul adresse image et coordonnees
			ptei = bank.getImageFromHandle (iNum);
			if (ptei == null) {
				break;
			}

			x1 = iX;
			if ((flags & PSF_HOTSPOT) != 0) {
				x1 -= ptei.getXSpot();
			}

			y1 = iY;
			if ((flags & PSF_HOTSPOT) != 0) {
				y1 -= ptei.getYSpot();
			}

			ptei.setResampling((MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0);
			//Log.Log("paste alias from texture of:"+(ptei.getAntialias()?"yes":"no"));
			GLRenderer.inst.renderImage(ptei, x1, y1, -1, -1, effect, effectParam);
                        
		} while (false);
	}

	// --------------------------------------------------------------;
	// Affichage des sprites d'une fenetre, sans intersection ;
	// --------------------------------------------------------------;
	public void winDrawSprites() {

		CSprite ptSpr;
		CSprite ptFirstSpr;

		// BlitMode bm;
		// BlitOp bo;

		ptFirstSpr = firstSprite;
		if (ptFirstSpr == null) {
			return;
		}

		
		// Inkeffects: rajouter blitmodes
		// Scan sprite table
		for (ptSpr = ptFirstSpr; ptSpr != null; ptSpr = ptSpr.objNext) {
			//++ s;
            // Si sprite inactif et pas SF_REAF, pas d'affichage
			if ((ptSpr.sprFlags & (CSprite.SF_HIDDEN | CSprite.SF_DISABLED)) != 0) {
				continue;
			}
            
			// Sprite ownerdraw
			if ((ptSpr.sprFlags & CSprite.SF_OWNERDRAW) != 0) {
				if (ptSpr.sprRout != null) {
					ptSpr.sprRout.spriteDraw(ptSpr, bank, ptSpr.sprX1,
							ptSpr.sprY1);
				}
			}

			// Normal sprite
			else {
				ptSpr.draw();
			}
			ptSpr.sprFlags &= ~(CSprite.SF_REAF | CSprite.SF_REAFINT);

		}
	}

	// ------------------------------;
	// Affichage des sprites ;
	// ------------------------------;
	public void spriteDraw() {
		winDrawSprites();
	}

	public CSprite getLastSprite(int nLayer, int dwFlags) {
		CSprite pSpr = lastSprite;

		// Get internal layer number
		int nIntLayer = nLayer;
		if (nLayer != -1) {
			nIntLayer *= 2;
			if ((dwFlags & GS_BACKGROUND) == 0) {
				nIntLayer++;
			}
		}

		// Search for first sprite in this layer
		while (pSpr != null) {
			// Correct layer?
			if (nIntLayer == -1 || pSpr.sprLayer == nIntLayer) {
				break;
			}

			// Break if a greater layer is reached (means there is no sprite in
			// the layer)
			if (pSpr.sprLayer < nIntLayer) {
				pSpr = null;
				break;
			}

			// Next sprite
			pSpr = pSpr.objPrev;
		}
		return pSpr;
	}

	// Ajoute/retrait d'un sprite de la liste
	// --------------------------------------
	/**
	 * Adds a new sprite to the list.
	 */
	public CSprite winAllocSprite() {
		CSprite spr = new CSprite(bank);
		if (firstSprite == null) {
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

	public void winFreeSprite(CSprite spr) {
		if (spr.objPrev == null) {
			firstSprite = spr.objNext;
		} else {
			spr.objPrev.objNext = spr.objNext;
		}
		if (spr.objNext != null) {
			spr.objNext.objPrev = spr.objPrev;
		} else {
			lastSprite = spr.objPrev;
		}
	}

	public void winSetColMode(short c) {
		colMode = c;
	}

	// --------------------------------------------------------------//
	// Test collision entre 1 point et les sprites sauf 1 //
	// --------------------------------------------------------------//
	public static final int SCF_OBSTACLE = 1;
	public static final int SCF_PLATFORM = 2;
	public static final int SCF_EVENNOCOL = 4;
	public static final int SCF_BACKGROUND = 8;

	public CSprite spriteCol_TestPoint(CSprite firstSpr, short nLayer, int xp,
			int yp, int dwFlags) {
		CSprite ptSpr = firstSpr;
		if (ptSpr == null) {
			ptSpr = firstSprite;
		} else {
			ptSpr = ptSpr.objNext;
		}

		boolean bAllLayers = (nLayer == LAYER_ALL);
		boolean bEvenNoCol = ((dwFlags & SCF_EVENNOCOL) != 0);
		short wAllLayerBit;
		if ((dwFlags & SCF_BACKGROUND) != 0) {
			wAllLayerBit = 0;
			if (nLayer != LAYER_ALL) {
				nLayer = (short) (nLayer * 2);
			}
		} else {
			wAllLayerBit = 1;
			if (nLayer != LAYER_ALL) {
				nLayer = (short) (nLayer * 2 + 1);
			}
		}

		// Recherche des autres sprites
		for (; ptSpr != null; ptSpr = ptSpr.objNext) {
			if (!bAllLayers) // todo: optimisation: faire une boucle
			// diff�rente pour le mode all layers et une
			// autre pour skipper les 1ers layers
			{
				if (ptSpr.sprLayer < nLayer) {
					continue;
				}
				if (ptSpr.sprLayer > nLayer) {
					break;
				}
			} else if ((ptSpr.sprLayer & 1) != wAllLayerBit) {
				continue;
			}

			// Can test collision with this one?
			if (bEvenNoCol || (ptSpr.sprFlags & CSprite.SF_RAMBO) != 0) {
				if (xp >= ptSpr.sprX1 && xp < ptSpr.sprX2 && yp >= ptSpr.sprY1
						&& yp < ptSpr.sprY2) {
					if ((ptSpr.sprFlags & CSprite.SF_TOKILL) == 0) // should
					// never
					// happen
					{
						int nGetColMaskFlag = CMask.GCMF_OBSTACLE;

						// Collides => test background flags
						if ((dwFlags & SCF_BACKGROUND) != 0) {
							// Platform? no collision if we check collisions
							// with obstacles
							if ((ptSpr.sprFlags & CSprite.SF_PLATFORM) != 0) {
								if ((dwFlags & SCF_OBSTACLE) != 0) {
									continue;
								}

								// Platform and check collisions with platforms
								// => get platform collision mask
								nGetColMaskFlag = CMask.GCMF_PLATFORM;
							}
						}

						// Box collision mode
						if (colMode == CM_BOX
								|| (ptSpr.sprFlags & CSprite.SF_COLBOX) != 0) {
							return ptSpr;
						}

						// Fine collision mode, test bit image
						CMask pMask = getSpriteMask(ptSpr, (short) -1,
								nGetColMaskFlag, ptSpr.sprAngle, ptSpr.sprScaleX, ptSpr.sprScaleY);
						if (pMask == null)
						{
							return ptSpr;
						}
						int dy = yp - ptSpr.sprY1;
						if (dy >= 0 && dy < pMask.getHeight ()) {
							int offset = dy * pMask.getLineWidth ();
							int dx = xp - ptSpr.sprX1;
							if (dx >=0 && dx < pMask.getWidth ()) {
								offset += dx / 16;
								short m = (short) (0x8000 >> (dx & 15));
								if ((pMask.getRawValue (offset) & m) != 0) {
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

    	if(ptSpr == null)
    		return null;
    	
        boolean bAllLayers = (nLayer == LAYER_ALL);
        boolean bEvenNoCol = true;
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
        //for (; ptSpr != null; ptSpr = ptSpr.objNext)
        //{
            if (!bAllLayers)
            {
                if (ptSpr.sprLayer < nLayer)
                {
                    //continue;
                    return null;
                }
                if (ptSpr.sprLayer > nLayer)
                {
                    //break;
                	return null;
                }
            }
            else if ((ptSpr.sprLayer & 1) != wAllLayerBit)
            {
                //continue;
            	return null;
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
					if (pMask == null)
						return ptSpr;
                    int dy = yp - ptSpr.sprY1;
                    if (dy >= 0 && dy < pMask.getHeight ()) {
                        int offset = dy * pMask.getLineWidth ();
                        int dx = xp - ptSpr.sprX1;
                        if (dx >= 0 && dx < pMask.getWidth ()) {
                            offset += dx / 16;
                            short m = (short) (0x8000 >> (dx & 15));
                            if ((pMask.getRawValue (offset) & m) != 0) {
                                return ptSpr;
                            }
                        }
                    }
                }
            }
        //}
        return null;
    }

    public CSprite spriteCol_TestRectOne(CSprite firstSpr, short nLayer, int xp, int yp, int wp, int hp, int dwFlags)
    {
    	CSprite ptSpr = firstSpr;

    	if(ptSpr == null)
    		return null;
    	
    	boolean bAllLayers = (nLayer == LAYER_ALL);
    	boolean bEvenNoCol = true;
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

    	if (!bAllLayers)
    	{
    		if (ptSpr.sprLayer < nLayer)
    		{
    			return null;
    		}
    		if (ptSpr.sprLayer > nLayer)
    		{
    			return null;
    		}
    	}
    	else if ((ptSpr.sprLayer & 1) != wAllLayerBit)
    	{
    		return null;
    	}

    	// Can test collision with this one?
    	if (bEvenNoCol || (ptSpr.sprFlags & CSprite.SF_RAMBO) != 0)
    	{
    		if(xp+wp/2 < ptSpr.sprX1 || ptSpr.sprX2 < xp-wp/2 || yp+hp/2 < ptSpr.sprY1 || ptSpr.sprY2 < yp-hp/2 )
    		{
    			return null;
    		}
    		else
    		{
    			int nGetColMaskFlag = CMask.GCMF_OBSTACLE;

    			// Box collision mode
    			if (colMode == CM_BOX || (ptSpr.sprFlags & CSprite.SF_COLBOX) != 0)
    			{
    				return ptSpr;
    			}

    			// Fine collision mode, test bit image
    			CMask pMask = getSpriteMask(ptSpr, (short)-1, nGetColMaskFlag, ptSpr.sprAngle, ptSpr.sprScaleX, ptSpr.sprScaleY);
				if (pMask == null)
					return ptSpr;
    			if(checkMask(ptSpr, pMask, xp, yp) 
    					|| checkMask(ptSpr, pMask, xp-wp/2, yp-hp/2) || checkMask(ptSpr, pMask, xp+wp/2, yp-hp/2)
    					|| checkMask(ptSpr, pMask, xp-wp/2, yp+hp/2) || checkMask(ptSpr, pMask, xp+wp/2, yp+hp/2))
    			{
    				return ptSpr;
    			}
    		}
    	}
    	return null;
    }

    private boolean checkMask(CSprite ptSpr, CMask pMask, int x, int y)
    {
    	if (pMask != null && ptSpr != null) {
    		int dy = y - ptSpr.sprY1;
    		if (dy >= 0 && dy < pMask.getHeight ()) {
    			int offset = dy * pMask.getLineWidth ();
    			int dx = x - ptSpr.sprX1;
    			if (dx >= 0 && dx < pMask.getWidth ()) {
    				offset += dx / 16;
    				short m = (short) (0x8000 >> (dx & 15));
    				if ((pMask.getRawValue (offset) & m) != 0) {
    					return true;
    				}
    			}
    		}
    	}
    	return false;
    }
    
    // --------------------------------------------------------------------------------------//
	// Test collision entre 1 sprite et les autres en changeant les coords du
	// 1er //
	// --------------------------------------------------------------------------------------//
	// Out: nombre de sprites collisionnes //
	// tabCols remplie pour chaque collision: idSpr(DW), extraInfo(DW) //
	// --------------------------------------------------------------------------------------//
	public ArrayList<CObject> spriteCol_TestSprite_All(CSprite ptSpr,
			short newImg, int newX, int newY, float newAngle, float newScaleX,
			float newScaleY, int dwFlags, ArrayList<CSprite> sprlist) {
		int nLayer;
		int cm = colMode;
		ArrayList<CObject> list = null;

		if (ptSpr == null) {
			return null;
		}
		if ((ptSpr.sprFlags & CSprite.SF_COLBOX) != 0) {
			cm = CM_BOX;
		}

		nLayer = ptSpr.sprLayer; // collisions always in the same layer

		// Flags to test
		if ((dwFlags & SCF_BACKGROUND) != 0) {
			// Test with background sprites => even layer
			nLayer &= ~1;
		} else {
			// Test with active sprites => collisions must be enabled
			if ((ptSpr.sprFlags & CSprite.SF_RAMBO) == 0) {
				return null;
			}

			// Test with active sprites => odd layer
			nLayer |= 1;
		}

		int x1 = newX;
		int y1 = newY;
		int x2 = x1;
		int y2 = y1;

		//int dwPSCFlags = 0;
		CMask pMask = null;

        // Si angle = 0 et coefs = 1.0f, ou si ownerdraw
        //		=> m�thode normale
        //
        // Si angle != 0 ou coefs != 1.0f
        //		=> appeler PrepareSpriteColMask pour calculer la bounding box et r�cup�rer le masque si d�j� calcul�
        //		=> puis r�cup�rer le masque avec CompleteSpriteColMask quand on en a besoin

        // Get sprite mask
        if ((ptSpr.sprFlags&CSprite.SF_OWNERDRAW) != 0)
        {
            x2+=ptSpr.sprX2-ptSpr.sprX1;
            y2+=ptSpr.sprY2-ptSpr.sprY1;
        }
        else
        {
            CImageInfo ifo=bank.getImageInfoEx(newImg, newAngle, newScaleX, newScaleY);
            if (ifo==null)
                return null;

            if ((ptSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
            {
                x1 -= ifo.xSpot;
                y1 -= ifo.ySpot;
            }
            x2 = x1 + ifo.width;
            y2 = y1 + ifo.height;

            ifo = null;
        }
        
        CSprite ptSpr1;
        CMask pMask2;

		if ( sprlist != null )
		{
			int ispr;
			for (ispr=0; ispr<sprlist.size(); ispr++)
			{
				ptSpr1 = sprlist.get(ispr);

				// In list mode:
				// - the sprites are on the same layer
				// - the sprites have the collision flags
				// - no test with background sprites

				// Collides?
				if (x1 < ptSpr1.sprX2 && x2 > ptSpr1.sprX1 && y1 < ptSpr1.sprY2 && y2 > ptSpr1.sprY1)
				{
					if (ptSpr1 == ptSpr)
						continue;

					// Box mode?
					if (cm == CM_BOX || (ptSpr1.sprFlags & CSprite.SF_COLBOX) != 0)
					{
						if (list == null)
						{
							list = new ArrayList<CObject>();
						}
						list.add(ptSpr1.sprExtraInfo);
					}

					// Fine collision mode
					else
					{
						// Calculate mask?
						if (pMask == null)
						{
							pMask = getSpriteMask(ptSpr, newImg, CMask.GCMF_OBSTACLE | CMask.GCMF_FORCEMASK, newAngle, newScaleX, newScaleY);
							if (pMask==null)
							{
								if (list == null)
								{
									list = new ArrayList<CObject>();
								}
								list.add(ptSpr1.sprExtraInfo);
								continue;
							}
						}

						// Test collision with second sprite
						pMask2 = getSpriteMask(ptSpr1, (short) -1, CMask.GCMF_OBSTACLE | CMask.GCMF_FORCEMASK, ptSpr1.sprAngle, ptSpr1.sprScaleX, ptSpr1.sprScaleY);
						if (pMask2==null)
						{
							if (list == null)
							{
								list = new ArrayList<CObject>();
							}
							list.add(ptSpr1.sprExtraInfo);
							continue;
						}
						if (pMask.testMask(0, x1, y1, pMask2, 0, ptSpr1.sprX1, ptSpr1.sprY1))
						{
							if (list == null)
							{
								list = new ArrayList<CObject>();
							}
							list.add(ptSpr1.sprExtraInfo);
						}
					}
				}
			}
		}
		else
		{
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
							list = new ArrayList<CObject>();
						}
						list.add(ptSpr1.sprExtraInfo);
					}
					// Fine collision mode
					else
					{
						// Calculate mask?
						if (pMask == null)
						{
							pMask = getSpriteMask(ptSpr, newImg, CMask.GCMF_OBSTACLE | CMask.GCMF_FORCEMASK, newAngle, newScaleX, newScaleY);
							if (pMask==null)
							{
								if (list == null)
								{
									list = new ArrayList<CObject>();
								}
								list.add(ptSpr1.sprExtraInfo);
								continue;
							}
						}
						// Test collision with second sprite
						pMask2 = getSpriteMask(ptSpr1, (short) -1, nGetColMaskFlag | CMask.GCMF_FORCEMASK, ptSpr1.sprAngle, ptSpr1.sprScaleX, ptSpr1.sprScaleY);
						if (pMask2==null)
						{
							if (list == null)
							{
								list = new ArrayList<CObject>();
							}
							list.add(ptSpr1.sprExtraInfo);
							continue;
						}
						if (pMask.testMask(0, x1, y1, pMask2, 0, ptSpr1.sprX1, ptSpr1.sprY1))
						{
							if (list == null)
							{
								list = new ArrayList<CObject>();
							}
							list.add(ptSpr1.sprExtraInfo);
						}
					}
				}
			}
		}
        return list;
	}

	// /////////////////////////////////////////////
	//
	// Get first sprite colliding with another one
	//
	public CSprite spriteCol_TestSprite(CSprite ptSpr, short newImg, int newX,
			int newY, float newAngle, float newScaleX, float newScaleY,
			int subHt, int dwFlags) {
		int nLayer;

		if (ptSpr == null) {
			return null;
		}
		if ((ptSpr.sprFlags & CSprite.SF_COLBOX) != 0) {
			colMode = CM_BOX;
		}

		nLayer = ptSpr.sprLayer; // collisions always in the same layer

		// Flags to test
		if ((dwFlags & SCF_BACKGROUND) != 0) {
			// Test with background sprites => even layer
			nLayer &= ~1;
		} else {
			// Test with active sprites => collisions must be enabled
			if ((ptSpr.sprFlags & CSprite.SF_RAMBO) == 0) {
				return null;
			}

			// Test with active sprites => odd layer
			nLayer |= 1;
		}

		int x1 = newX;
		int y1 = newY;
		int x2 = x1;
		int y2 = y1;

		//int dwPSCFlags = 0;
		CMask pMask = null;

        // Si angle = 0 et coefs = 1.0f, ou si ownerdraw
        //		=> m�thode normale
        //
        // Si angle != 0 ou coefs != 1.0f
        //		=> appeler PrepareSpriteColMask pour calculer la bounding box et r�cup�rer le masque si d�j� calcul�
        //		=> puis r�cup�rer le masque avec CompleteSpriteColMask quand on en a besoin

        // Image sprite not stretched and not rotated, or owner draw sprite?
        // Get sprite mask
        if ((ptSpr.sprFlags&CSprite.SF_OWNERDRAW) != 0)
        {
            x2+=ptSpr.sprX2-ptSpr.sprX1;
            y2+=ptSpr.sprY2-ptSpr.sprY1;
        }
        else
        {
            CImageInfo ifo=bank.getImageInfoEx(newImg, newAngle, newScaleX, newScaleY);
            if ((ptSpr.sprFlags & CSprite.SF_NOHOTSPOT) == 0)
            {
                x1 -= ifo.xSpot;
                y1 -= ifo.ySpot;
            }
            x2 = x1 + ifo.width;
            y2 = y1 + ifo.height;

            ifo = null;
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
                if ((ptSpr1.sprFlags & CSprite.SF_TOKILL) == 0)	// Securit�?
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
                            pMask = getSpriteMask(ptSpr, newImg, CMask.GCMF_OBSTACLE | CMask.GCMF_FORCEMASK, newAngle, newScaleX, newScaleY);
                            if (pMask==null)
                            {
                                return ptSpr1;
                            }
                        }

                        // Take subHt into account
                        int yMaskBits = 0;
                        int nMaskHeight = pMask.getHeight ();
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
                        CMask pMask2 = getSpriteMask(ptSpr1, (short) -1, nGetColMaskFlag | CMask.GCMF_FORCEMASK, ptSpr1.sprAngle, ptSpr1.sprScaleX, ptSpr1.sprScaleY);
                        if (pMask2==null)
                        {
                            return ptSpr1;
                        }
                        if (pMask.testMask(yMaskBits, x1, y1, pMask2, 0, ptSpr1.sprX1, ptSpr1.sprY1))
                        {
                            return ptSpr1;
                        }
                    }
                }
            }
        }
        return null;
	}

	// --------------------------------------------------------------//
	// Test collision entre 1 rectangle et les sprites sauf 1 //
	// --------------------------------------------------------------//
	public CSprite spriteCol_TestRect(CSprite firstSpr, int nLayer, int xp,
			int yp, int wp, int hp, int dwFlags) {
		CSprite ptSpr = firstSpr;
		if (ptSpr == null) {
			ptSpr = firstSprite;
		} else {
			ptSpr = ptSpr.objNext;
		}

		boolean bAllLayers = (nLayer == LAYER_ALL);
		boolean bEvenNoCol = ((dwFlags & SCF_EVENNOCOL) != 0);
		short wAllLayerBit;
		if ((dwFlags & SCF_BACKGROUND) != 0) {
			wAllLayerBit = 0;
			if (nLayer != LAYER_ALL) {
				nLayer = nLayer * 2;
			}
		} else {
			wAllLayerBit = 1;
			if (nLayer != LAYER_ALL) {
				nLayer = nLayer * 2 + 1;
			}
		}

		// Recherche des autres sprites
		for (; ptSpr != null; ptSpr = ptSpr.objNext) {
			if (!bAllLayers) // todo: optimisation: faire une boucle
			// diff�rente pour le mode all layers et une
			// autre pour skipper les 1ers layers
			{
				if (ptSpr.sprLayer < nLayer) {
					continue;
				}
				if (ptSpr.sprLayer > nLayer) {
					break;
				}
			} else if ((ptSpr.sprLayer & 1) != wAllLayerBit) {
				continue;
			}

			// Can test collision with this one?
			if (bEvenNoCol || (ptSpr.sprFlags & CSprite.SF_RAMBO) != 0) {
				if (xp <= ptSpr.sprX2 && xp + wp > ptSpr.sprX1
						&& yp <= ptSpr.sprY2 && yp + hp > ptSpr.sprY1) {
					if ((ptSpr.sprFlags & CSprite.SF_TOKILL) == 0) // should
					// never
					// happen
					{
						int nGetColMaskFlag = CMask.GCMF_OBSTACLE;

						// Collides => test background flags
						if ((dwFlags & SCF_BACKGROUND) != 0) {
							// Platform? no collision if we check collisions
							// with obstacles
							if ((ptSpr.sprFlags & CSprite.SF_PLATFORM) != 0) {
								if ((dwFlags & SCF_OBSTACLE) != 0) {
									continue;
								}
								// Platform and check collisions with platforms
								// => get platform collision mask
								nGetColMaskFlag = CMask.GCMF_PLATFORM;
							}
						}

						// Box collision mode
						if (colMode == CM_BOX
								|| (ptSpr.sprFlags & CSprite.SF_COLBOX) != 0) {
							return ptSpr;
						}

						// Fine collision mode, test bit image
						CMask pMask = getSpriteMask(ptSpr, (short) -1,
								nGetColMaskFlag | CMask.GCMF_FORCEMASK, ptSpr.sprAngle, ptSpr.sprScaleX,
                                ptSpr.sprScaleY);
						if (pMask == null)
							return ptSpr;
						if (pMask.testRect(0, xp - ptSpr.sprX1, yp - ptSpr.sprY1, wp, hp)) {
							return ptSpr;
						}
					}
				}
			}
		}
		return null;
	}
}