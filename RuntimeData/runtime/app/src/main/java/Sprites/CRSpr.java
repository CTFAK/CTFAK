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
// CRSPR : Gestion des objets sprites
//
//----------------------------------------------------------------------------------
package Sprites;

import Application.CRunApp;
import Frame.CLayer;
import Movements.CMoveDef;
import OI.COI;
import OI.CObjectCommon;
import Objects.CObject;
import RunLoop.CCreateObjectInfo;
import RunLoop.CObjInfo;
import RunLoop.CRun;
import Runtime.MMFRuntime;
import Transitions.CTrans;
import android.util.Log;

public class CRSpr 
{
    public final static short RSFLAG_HIDDEN=0x0001;
    public final static short RSFLAG_INACTIVE=0x0002;
    public final static short RSFLAG_SLEEPING=0x0004;
    public final static short RSFLAG_SCALE_RESAMPLE=0x0008;
    public final static short RSFLAG_ROTATE_ANTIA=0x0010;
    public final static short RSFLAG_VISIBLE=0x0020;
    public final static short SPRTYPE_TRUESPRITE=0;
    public final static short SPRTYPE_OWNERDRAW=1;
    public final static short SPRTYPE_QUICKDISPLAY=2;
    
    public CObject hoPtr;
    public CSpriteGen spriteGen;
    public int rsFlash;				/// Flash objets
    public int rsFlashCpt;
    public short rsLayer;				/// Layer
    public int rsZOrder;			/// Z-order value
    public int rsCreaFlags;			/// Creation flags
    public int rsBackColor;			/// background saving color
    public int rsEffect;			/// Sprite effects
    public int rsEffectParam;
    public short rsFlags;			/// Handling flags
    public short rsFadeCreaFlags;		/// Saved during a fadein
    public short rsSpriteType;
    public CTrans rsTrans=null;
    
    public CRSpr() 
    {
    }

    /** First half of the initialisation.
     */
    public void init1(CObject ho, CObjectCommon ocPtr, CCreateObjectInfo cobPtr)
    {
        hoPtr=ho;
        spriteGen=ho.hoAdRunHeader.spriteGen;
        
		rsLayer = (short)cobPtr.cobLayer;					// Layer
		rsZOrder = cobPtr.cobZOrder;				// Creation z-order
	
		rsFlags=0;
		
		rsCreaFlags=CSprite.SF_RAMBO;
		if ((hoPtr.hoLimitFlags&CObjInfo.OILIMITFLAGS_QUICKCOL)==0)
            rsCreaFlags&=~CSprite.SF_RAMBO;

		rsBackColor=0;							// Couleur de sauvegarde du fond
		if ( (hoPtr.hoOEFlags&CObjectCommon.OEFLAG_BACKSAVE)==0 || (hoPtr.hoOiList.oilOCFlags2&CObjectCommon.OCFLAGS2_DONTSAVEBKD)!=0)
		{
            hoPtr.hoOEFlags&=~CObjectCommon.OEFLAG_BACKSAVE;
            rsCreaFlags|=CSprite.SF_NOSAVE;				//; pas de sauvegarde
            if ((hoPtr.hoOiList.oilOCFlags2&CObjectCommon.OCFLAGS2_SOLIDBKD)!=0)
            {
				rsBackColor=hoPtr.hoOiList.oilBackColor;
				rsCreaFlags|=CSprite.SF_FILLBACK;		//; Effacement avec couleur pleine
            }
		}
		if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_INTERNALBACKSAVE)!=0)
            rsCreaFlags|=CSprite.SF_OWNERSAVE;
		if ((hoPtr.hoOiList.oilOCFlags2&CObjectCommon.OCFLAGS2_COLBOX)!=0)		//; Collision en mode box?
            rsCreaFlags|=CSprite.SF_COLBOX;

		if ((cobPtr.cobFlags&CRun.COF_HIDDEN)!=0)				//; Faut-il le cacher a l'ouverture?
		{
            rsCreaFlags|=CSprite.SF_HIDDEN;
            rsFlags |= RSFLAG_HIDDEN;
            if (hoPtr.hoType==COI.OBJ_TEXT)
            	hoPtr.hoFlags|=CObject.HOF_NOCOLLISION;		//; Cas particulier pour cette merde d'objet texte
		}
		else
		{
			rsFlags|=RSFLAG_VISIBLE;
		}
		rsEffect=hoPtr.hoOiList.oilInkEffect;
		rsEffectParam=hoPtr.hoOiList.oilEffectParam;	//; Le parametre de l'ink effect
	
	//	cmp	   	[esi].RunObject.roc.rcNMovement,MVTYPE_STATIC*4	
		if (hoPtr.roc.rcMovementType==CMoveDef.MVTYPE_STATIC)		// Sprite inactif, si pas de mouvement
		{
			rsFlags|=RSFLAG_INACTIVE;
			rsCreaFlags|=CSprite.SF_INACTIF;
		}
    	if(hoPtr.hoOiList.oilAntialias)	//Add anti-alias effect
    		rsCreaFlags |= CSprite.EFFECTFLAG_ANTIALIAS;
			
		rsFadeCreaFlags=(short)rsCreaFlags;		//; Correction bug collision absentes quand fadein + fade sprite	
    }

    // -------------------------------------------------
    // Initialisation sprite deuxieme partie
    // -------------------------------------------------
    public void init2()
    {
    	createSprite(null);
    }

    // Routine de display 
    // ------------------
    public void displayRoutine()
    {
        switch(rsSpriteType)
        {
            case 0:         // SPRTYPE_TRUESPRITE
                if (hoPtr.roc.rcSprite!=null)
                {
                    spriteGen.modifSpriteEx(hoPtr.roc.rcSprite, 
                                            hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX, hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY, hoPtr.roc.rcImage, 
                                            hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, (hoPtr.ros.rsFlags & RSFLAG_SCALE_RESAMPLE) != 0,
                                            hoPtr.roc.rcAngle, (hoPtr.ros.rsFlags & RSFLAG_ROTATE_ANTIA) != 0);
                }
                break;
                
            case 1:         // SPRTYPE_OWNERDRAW
            case 2:         // SPRTYPE_QUICKDISPLAY (not used in Android HWA)
            	
                if (hoPtr.roc.rcSprite!=null)
                {
                    spriteGen.activeSprite(hoPtr.roc.rcSprite, CSpriteGen.AS_REDRAW, null);
                }
                break;
          /*  case 2:         // SPRTYPE_QUICKDISPLAY
                hoPtr.hoAdRunHeader.display_OwnerQuickDisplay(hoPtr);
                break; */
        }
    }

    // -------------------------------------------------------------------
    // GESTION D'UN OBJET SPRITE
    // -------------------------------------------------------------------
    public void handle()
    {
        CRun rhPtr=hoPtr.hoAdRunHeader;

        // En marche ou pas?
		// -----------------
		if ((rsFlags&RSFLAG_SLEEPING)==0)
		{
            // Gestion du flash
            // ----------------
            if (rsFlash!=0)
            {
                rsFlashCpt-=rhPtr.rhTimerDelta;
                if (rsFlashCpt<0)
                {
                    rsFlashCpt=rsFlash;
                    if ((rsFlags&RSFLAG_VISIBLE)==0)
                    {
                        rsFlags|=RSFLAG_VISIBLE;
                        obShow();
                    }
                    else
                    {
                        rsFlags&=~RSFLAG_VISIBLE;
                        obHide();
                    }
                }
            }

            // Appel de la routine de mouvement	
            // --------------------------------
            if (hoPtr.rom!=null)
                hoPtr.rom.move();

            // Verifie que l'objet n'est pas trop en dehors du terrain
            // -------------------------------------------------------
            if (hoPtr.roc.rcPlayer!=0) 
                return;			//; Seulement les objets de l'ordinateur
            if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_NEVERSLEEP)!=0) 
			{
				// Build 285.1: handle "Destroy if too far" even if "Inactivate if too far" is set to No
				if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_NEVERKILL)==0 && (rhPtr.rhApp.hdr2Options & CRunApp.AH2OPT_DESTROYIFNOINACTIVATE) != 0)
				{
					int x1=hoPtr.hoX-hoPtr.hoImgXSpot;
					int y1=hoPtr.hoY-hoPtr.hoImgYSpot;
					int x2=x1+hoPtr.hoImgWidth;
					int y2=y1+hoPtr.hoImgHeight;

					if (x2<rhPtr.rh3XMinimumKill || x1>rhPtr.rh3XMaximumKill || y2<rhPtr.rh3YMinimumKill || y1>rhPtr.rh3YMaximumKill)
						rhPtr.destroy_Add(hoPtr.hoNumber);
				}
				return;
			}

            int x1=hoPtr.hoX-hoPtr.hoImgXSpot;
            int y1=hoPtr.hoY-hoPtr.hoImgYSpot;
			
            int x2=x1+hoPtr.hoImgWidth;
            int y2=y1+hoPtr.hoImgHeight;
                       
            // Faire disparaitre le sprite?
            if (x2>=rhPtr.rh3XMinimum && x1<=rhPtr.rh3XMaximum && y2>=rhPtr.rh3YMinimum && y1<=rhPtr.rh3YMaximum) 
                return;

            // Detruit/Faire disparaitre l'objet
            // ---------------------------------
            if (x2>=rhPtr.rh3XMinimumKill && x1<=rhPtr.rh3XMaximumKill && y2>=rhPtr.rh3YMinimumKill && y1<=rhPtr.rh3YMaximumKill)
            {
                // Simplement faire disparaitre
                rsFlags|=RSFLAG_SLEEPING;
                if (hoPtr.roc.rcSprite!=null)
                {
                    // Save Z-order value before deleting sprite
                    rsZOrder = hoPtr.roc.rcSprite.sprZOrder;

                    rhPtr.spriteGen.delSpriteFast(hoPtr.roc.rcSprite);
                    hoPtr.roc.rcSprite=null;
                    return;
                }
                else
                {
                //    hoPtr.killBack();
                    return;
                }
            }
            else
            {
                // Detruire l'objet, si son flag NEVER KILL est a zero
                if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_NEVERKILL)==0)
                {
                    rhPtr.destroy_Add(hoPtr.hoNumber);
                }
                return;
            }
		}
		else 
		{
            // Un objet qui dort, le faire reapparaitre ?
            // ------------------------------------------
            int x1=hoPtr.hoX-hoPtr.hoImgXSpot;
            int y1=hoPtr.hoY-hoPtr.hoImgYSpot;
            int x2=x1+hoPtr.hoImgWidth;
            int y2=y1+hoPtr.hoImgHeight;
            if (x2>=rhPtr.rh3XMinimum && x1<=rhPtr.rh3XMaximum && y2>=rhPtr.rh3YMinimum && y1<=rhPtr.rh3YMaximum)
            {
                rsFlags&=~RSFLAG_SLEEPING;
                init2();
            }
		}
    }

    
    // Routine de modif
    // ----------------
    public void modifRoutine()
    {
        switch(rsSpriteType)
        {
            case 0:         // SPRTYPE_TRUESPRITE
                if (hoPtr.roc.rcSprite!=null)
                {
                    spriteGen.modifSpriteEx(hoPtr.roc.rcSprite, 
					  hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX, hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY, hoPtr.roc.rcImage, 
					  hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, (hoPtr.ros.rsFlags & RSFLAG_SCALE_RESAMPLE) != 0,
					  hoPtr.roc.rcAngle, (hoPtr.ros.rsFlags & RSFLAG_ROTATE_ANTIA) != 0);
                 }
                break;
            
            case 1:         // SPRTYPE_OWNERDRAW
            case 2:         // SPRTYPE_QUICKDISPLAY
            	
                objGetZoneInfos();
                if (hoPtr.roc.rcSprite!=null)
                {
                    spriteGen.modifOwnerDrawSprite(hoPtr.roc.rcSprite,	hoPtr.hoRect.left, hoPtr.hoRect.top, hoPtr.hoRect.right, hoPtr.hoRect.bottom);
                }
                break;
           /* case 2:         // SPRTYPE_QUICKDISPLAY
                hoPtr.hoAdRunHeader.display_OwnerQuickDisplay(hoPtr);
                break; */
        }
    }

    // CREATION D'UN VRAI SPRITE SIMPLE
    // --------------------------------
    public boolean createSprite(CSprite pSprBefore)
    {
	// Un vrai sprite
	// --------------
		if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_ANIMATIONS)!=0)
		{
            CSprite pSpr=spriteGen.addSprite(hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX, hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY,
					hoPtr.roc.rcImage, rsLayer, rsZOrder, rsBackColor, rsCreaFlags, hoPtr);

            if (pSpr!=null)
            {
                hoPtr.roc.rcSprite=pSpr;						//; Stocke le Sprite
                hoPtr.hoFlags|=CObject.HOF_REALSPRITE;
                spriteGen.modifSpriteEffect(pSpr, rsEffect, rsEffectParam);

                if ( pSprBefore != null )
                    spriteGen.moveSpriteBefore(pSpr, pSprBefore);
                rsSpriteType=SPRTYPE_TRUESPRITE;
           }
            return true;
		}
		// Un faux sprite, gere en owner-draw
		// ----------------------------------
	//	if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_QUICKDISPLAY)==0)
	//	{
            rsCreaFlags|=CSprite.SF_OWNERDRAW|CSprite.SF_INACTIF;
            if ( (rsCreaFlags & CSprite.SF_COLBOX) == 0 )
            	rsCreaFlags |= CSprite.SF_OWNERCOLMASK;
            rsFlags|=RSFLAG_INACTIVE;
            hoPtr.hoFlags|=CObject.HOF_OWNERDRAW;
            hoPtr.hoRect.left=hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX-hoPtr.hoImgXSpot;
            hoPtr.hoRect.top=hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY-hoPtr.hoImgYSpot;
            hoPtr.hoRect.right=hoPtr.hoRect.left+hoPtr.hoImgWidth;
            hoPtr.hoRect.bottom=hoPtr.hoRect.top+hoPtr.hoImgHeight;

            CSprite spr=spriteGen.addOwnerDrawSprite(hoPtr.hoRect.left, hoPtr.hoRect.top, hoPtr.hoRect.right, hoPtr.hoRect.bottom,
							rsLayer, rsZOrder, rsBackColor, rsCreaFlags, hoPtr, hoPtr);
            
            if (spr==null) 
            	return false;
            
			hoPtr.roc.rcSprite=spr;
            if ( pSprBefore != null )
            	spriteGen.moveSpriteBefore(spr, pSprBefore);
            rsSpriteType=SPRTYPE_OWNERDRAW;
            return true;
	/*	}
		else
		{
            // Un sprite quick-display
            // -----------------------
            if ((rsCreaFlags&CSprite.SF_HIDDEN)==0)			//; Si cache?
                hoPtr.hoNextQuickDisplay=-1;
            else
                hoPtr.hoNextQuickDisplay=-2;				//; Bloque le sprite
            rsSpriteType=SPRTYPE_QUICKDISPLAY;
            return true;
		} */
    }

    // DESTRUCTION D'UN SPRITE
    // -----------------------
    public void kill(boolean fast)
    {
		if (hoPtr.roc.rcSprite!=null)							//; Est-il active?
		{
            // Save Z-order value before deleting sprite
            rsZOrder = hoPtr.roc.rcSprite.sprZOrder;

            // Un sprite normal
            if (fast==false)								//; Mode fast?
            {
                spriteGen.delSprite(hoPtr.roc.rcSprite);
            }
            else
            {
                spriteGen.delSpriteFast(hoPtr.roc.rcSprite);
            }
            hoPtr.roc.rcSprite=null;
		}				
    }

    // RECREE UN SPRITE EN FIN DE PREMIERE BOUCLE FADE-IN   
    // --------------------------------------------------
    public void reInit_Spr(boolean fast)
    {
		if (hoPtr.roc.rcSprite!=null)
		{
            init2();
		}
		displayRoutine();
    }


    // Demande la taille du rectangle
    // ------------------------------
    public void objGetZoneInfos()
    {
		hoPtr.getZoneInfos();
		hoPtr.hoRect.left=hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX-hoPtr.hoImgXSpot;			// Calcul des coordonnees
		hoPtr.hoRect.right=hoPtr.hoRect.left+hoPtr.hoImgWidth;
		hoPtr.hoRect.top=hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY-hoPtr.hoImgYSpot;
		hoPtr.hoRect.bottom=hoPtr.hoRect.top+hoPtr.hoImgHeight;
    }
    
    // CACHE/MONTRE UN SPRITE
    // ----------------------
    public void obHide()
    {
		if ((rsFlags&RSFLAG_HIDDEN)==0)
		{
            rsFlags|=RSFLAG_HIDDEN;
            rsCreaFlags|=CSprite.SF_HIDDEN;
            rsFadeCreaFlags|=CSprite.SF_HIDDEN;
            hoPtr.roc.rcChanged=true;
            if (hoPtr.roc.rcSprite!=null)
            {
                hoPtr.hoAdRunHeader.spriteGen.showSprite(hoPtr.roc.rcSprite, false);
            }
          /*  else
            {
                if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_QUICKDISPLAY)!=0)
                {
                    hoPtr.hoAdRunHeader.display_OwnerQuickDisplay(hoPtr);
                }
            } */
		}
    }

    public void obShow()
    {
		if ((rsFlags&RSFLAG_HIDDEN)!=0)
		{
            // Test if layer shown
            CLayer pLayer = hoPtr.hoAdRunHeader.rhFrame.layers[hoPtr.hoLayer];
            if ( (pLayer.dwOptions & (CLayer.FLOPT_TOHIDE|CLayer.FLOPT_VISIBLE)) == CLayer.FLOPT_VISIBLE )
            {
                rsCreaFlags&=~CSprite.SF_HIDDEN;
                rsFadeCreaFlags&=~CSprite.SF_HIDDEN;
                rsFlags&=~RSFLAG_HIDDEN;
                hoPtr.hoFlags&=~CObject.HOF_NOCOLLISION;				//; Des collisions de nouveau (objet texte)
                hoPtr.roc.rcChanged=true;
                if (hoPtr.roc.rcSprite!=null)
                {
                    hoPtr.hoAdRunHeader.spriteGen.showSprite(hoPtr.roc.rcSprite, true);
                }
               /* else
                {
                    if ((hoPtr.hoOEFlags&CObjectCommon.OEFLAG_QUICKDISPLAY)!=0)
                    {
                        if (hoPtr.hoNextQuickDisplay==-2)		//; Debloque le quick display
                            hoPtr.hoNextQuickDisplay=-1;

                        hoPtr.hoAdRunHeader.display_OwnerQuickDisplay(hoPtr);
                    }
                } */
            }
        }
    }
}
