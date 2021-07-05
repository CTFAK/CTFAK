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
// CPOSITION: parametre position donn�es
//
//----------------------------------------------------------------------------------
package Params;

import Application.CRunApp;
import Banks.CImageInfo;
import Movements.CMove;
import OI.CObjectCommon;
import Objects.CObject;
import RunLoop.CRun;
import RunLoop.CRunMBase;

public abstract class CPosition extends CParam
{
    public short posOINUMParent;			//0
    public short posFlags;
    public short posX;					//4
    public short posY;
    public short posSlope;				//8
    public short posAngle;
    public int posDir;					//12
    public short posTypeParent;                         //16
    public short posOiList;				//18
    public short posLayer;				//20

    public static final short CPF_DIRECTION=0x0001;
    public static final short CPF_ACTION=0x0002;
    public static final short CPF_INITIALDIR=0x0004;
    public static final short CPF_DEFAULTDIR=0x0008;
    
    public CPosition() 
    {
    }
    
    // ----------------------------------
    // Interprete une structure POSITION EAX=Structure position
    // ----------------------------------
    public boolean read_Position(CRun rhPtr, int getDir, CPositionInfo pInfo)
    {
        pInfo.layer=-1;

		if (posOINUMParent==-1)
		{
            // Pas d'objet parent
            // ~~~~~~~~~~~~~~~~~~
            if (getDir!=0)									// Tenir compte de la direction?
            {
                pInfo.dir=-1;
                if ((posFlags&CPF_DEFAULTDIR)==0)		// Garder la direction de l'objet
                {
                    pInfo.dir=rhPtr.get_Direction(posDir);		// Va chercher la direction
                }
            }
            pInfo.x=posX;
            pInfo.y=posY;
            int nLayer = posLayer;
            if ( nLayer > rhPtr.rhFrame.nLayers - 1 )
                nLayer = rhPtr.rhFrame.nLayers - 1 ;
            pInfo.layer=nLayer;
            pInfo.bRepeat=false;
		}
		else
		{
            // Trouve le parent
            rhPtr.rhEvtProg.rh2EnablePick=false;
            CObject pHo=rhPtr.rhEvtProg.get_CurrentObjects(posOiList);
            pInfo.bRepeat=rhPtr.rhEvtProg.repeatFlag;
            if (pHo==null) 
                return false;
            pInfo.x=pHo.hoX;
            pInfo.y=pHo.hoY;
            pInfo.layer=pHo.hoLayer;

            if ((posFlags&CPF_ACTION)!=0)					// Relatif au point d'action?
            {
                if ((pHo.hoOEFlags&CObjectCommon.OEFLAG_ANIMATIONS)!=0)
                {
                    if ( pHo.roc.rcImage >= 0 )
                    {
                        CImageInfo ifo;

                        float angle = pHo.roc.rcAngle;
                        CRunMBase pMBase = null;
        				if(rhPtr.rh4Box2DObject)
        					pMBase = rhPtr.GetMBase(pHo);
                        if (pMBase!=null)
                            angle=pMBase.getAngle();
                        ifo=rhPtr.rhApp.imageBank.getImageInfoEx(pHo.roc.rcImage, angle, pHo.roc.rcScaleX, pHo.roc.rcScaleY);
                        if(ifo != null) {
	                        pInfo.x+=ifo.xAP-ifo.xSpot;
	                        pInfo.y+=ifo.yAP-ifo.ySpot;
                        }
                    }
                }
            }

            if ((posFlags&CPF_DIRECTION)!=0)				// Tenir compte de la direction?
            {
                int dir=(posAngle+rhPtr.getDir(pHo))&0x1F;	// La direction courante
                int px=CMove.getDeltaX(posSlope, dir);
                int py=CMove.getDeltaY(posSlope, dir);
                pInfo.x+=px;
                pInfo.y+=py;
            }
            else
            {
                pInfo.x+=posX;								// Additionne la position relative
                pInfo.y+=posY;		
            }

            if ((getDir&0x01)!=0)
            {
                if ((posFlags&CPF_DEFAULTDIR)!=0)			// Mettre la direction par defaut?
                {
                    pInfo.dir=-1;
                }
                else if ((posFlags&CPF_INITIALDIR)!=0)		// Mettre la direction initiale?
                {
                    pInfo.dir=rhPtr.getDir(pHo);
                }
                else
                {
                    pInfo.dir=rhPtr.get_Direction(posDir);		// Va cherche la direction
                }
            }
		}

		// Verification des directions: dans le terrain!!
		if ((getDir&0x02)!=0)
		{
            if (pInfo.x<rhPtr.rh3XMinimumKill || pInfo.x>rhPtr.rh3XMaximumKill) 
                return false;
            if (pInfo.y<rhPtr.rh3YMinimumKill || pInfo.y>rhPtr.rh3YMaximumKill) 
                return false;
		}
		return true;
    }

    
    @Override
	public abstract void load(CRunApp app);
}
