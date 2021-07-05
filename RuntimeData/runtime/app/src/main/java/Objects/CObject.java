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
// COBJECT : Classe de base d'un objet'
//
//----------------------------------------------------------------------------------

// 12/07/2016 added set BoundingBoxFromWidth ported from iOS , used in: activepicture

package Objects;

import java.util.ArrayList;

import Animations.CAnim;
import Animations.CRAni;
import Banks.CImageBank;
import Banks.CImageInfo;
import Frame.CLayer;
import Movements.CMoveBullet;
import Movements.CMoveDef;
import Movements.CRMvt;
import OI.CObjectCommon;
import Params.PARAM_SHOOT;
import RunLoop.CCreateObjectInfo;
import RunLoop.CObjInfo;
import RunLoop.CRun;
import Services.CRect;
import Sprites.CMask;
import Sprites.CRSpr;
import Sprites.CSprite;
import Sprites.IDrawable;
import Values.CRVal;

public abstract class CObject implements IDrawable
{
	public final static short HOF_DESTROYED=0x0001;
	public final static short HOF_TRUEEVENT=0x0002;
	public final static short HOF_REALSPRITE=0x0004;
	public final static short HOF_FADEIN=0x0008;
	public final static short HOF_FADEOUT=0x0010;
	public final static short HOF_OWNERDRAW=0x0020;
	public final static short HOF_NOCOLLISION=0x2000;
	public final static short HOF_FLOAT=0x4000;
	public final static short HOF_STRING=(short)0x8000;

	// HeaderObject
	public short hoNumber;					/// Number of the object
	public short hoNextSelected;				/// Selected object list!!! DO NOT CHANGE POSITION!!!

	public CRun hoAdRunHeader;                                  /// Run-header address
	public short hoHFII;					/// Number of LevObj
	public short hoOi;						/// Number of OI
	public short hoNumPrev;					/// Same OI previous object
	public short hoNumNext;					/// ... next
	public short hoType;					/// Type of the object
	public short hoCreationId;                                  /// Number of creation
	public CObjInfo hoOiList;                                   /// Pointer to OILIST information
	public int hoEvents;					/// Pointer to specific events
	public ArrayList<Integer> hoPrevNoRepeat=null;                       /// One-shot event handling
	public ArrayList<Integer> hoBaseNoRepeat=null;

	public int hoMark1;                                         /// #of loop marker for the events
	public int hoMark2;
	public String hoMT_NodeName;				/// Name fo the current node for path movements

	public int hoEventNumber;                                   /// Number of the event called (for extensions)
	public CObjectCommon hoCommon;				/// Common structure address

	public int hoCalculX;					/// Low weight value
	public int hoX;                                             /// X coordinate
	public int hoCalculY;					/// Low weight value
	public int hoY;						/// Y coordinate
	public int hoImgXSpot;					/// Hot spot of the current image
	public int hoImgYSpot;
	public int hoImgWidth;					/// Width of the current picture
	public int hoImgHeight;
	public CRect hoRect=new CRect();        				/// Display rectangle

	public int hoImgXAP;
	public int hoImgYAP;

	public int hoOEFlags;					/// Objects flags
	public short hoFlags;					/// Flags
	public byte hoSelectedInOR;                                 /// Selection lors d'un evenement OR
	public int hoOffsetValue;                                   /// Values structure offset
	public int hoLayer;                                         /// Layer

	public short hoLimitFlags;                                  /// Collision limitation flags
	public short hoNextQuickDisplay;                            /// Quickdraw list

	public int hoCurrentParam;                                  /// Address of the current parameter

	public int hoIdentifier;                                    /// ASCII identifier of the object
	public boolean hoCallRoutine;
	
	public boolean isPaused;									/// added 290.2 to check if object is paused by physics engine

	// Classes de gestion communes
	public CRCom roc;                   // The CRCom object
	public CRMvt rom;                   // The CRMvt object
	public CRAni roa;                   // The CRAni object
	public CRVal rov;                   // The CRVal object
	public CRSpr ros;                   // The CRSpr object

	public CImageInfo	ifoTemp;		// Temporary image info

	public CObject() 
	{
	}

	// Routines diverses
	public void setScale(float fScaleX, float fScaleY, boolean bResample)
	{
		boolean bOldResample=false;
		if ((ros.rsFlags&CRSpr.RSFLAG_SCALE_RESAMPLE)!=0)
			bOldResample=true;

		if ( (roc.rcScaleX != fScaleX || roc.rcScaleY != fScaleY) || bOldResample != bResample )
		{
			roc.rcScaleX = fScaleX;
			roc.rcScaleY = fScaleY;
			ros.rsFlags &= ~CRSpr.RSFLAG_SCALE_RESAMPLE;
			if ( bResample )
				ros.rsFlags |= CRSpr.RSFLAG_SCALE_RESAMPLE;
			roc.rcChanged = true;
			updateImageInfo();
		}
	}        

	public boolean updateImageInfo()
	{
		if ( ifoTemp == null )
			ifoTemp = new CImageInfo();
		if ( hoAdRunHeader.rhApp.imageBank.getImageInfoEx2(ifoTemp, roc.rcImage, roc.rcAngle, roc.rcScaleX, roc.rcScaleY) )
		{
			hoImgWidth=ifoTemp.width;
			hoImgHeight=ifoTemp.height;
			hoImgXSpot=ifoTemp.xSpot;
			hoImgYSpot=ifoTemp.ySpot;
			hoImgXAP = ifoTemp.xAP;
			hoImgYAP = ifoTemp.yAP;
			return true;
		}
		return false;
	}

	// SHOOT : Cree la balle
	// ----------------------
	public void shtCreate(PARAM_SHOOT p, int x, int y, int dir)
	{
		int nLayer = hoLayer;
		int num=hoAdRunHeader.f_CreateObject(p.cdpHFII, p.cdpOi, x, y, dir, (short)(CRun.COF_NOMOVEMENT|CRun.COF_HIDDEN), nLayer, -1);
		if (num>=0)
		{
			// Cree le movement
			// ----------------
			CObject pHo=hoAdRunHeader.rhObjectList[num];
			if (pHo.rom!=null)
			{
				pHo.rom.initSimple(pHo, CMoveDef.MVTYPE_BULLET, false);		
				pHo.roc.rcDir=dir;						// Met la direction de depart
				pHo.roc.rcSpeed=p.shtSpeed;				// Met la vitesse
				CMoveBullet mBullet=(CMoveBullet)pHo.rom.rmMovement;
				mBullet.init2(this);

				// Hide object if layer hidden
				// ---------------------------
				if (nLayer!=-1)
				{
					if ( (pHo.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0 )
					{
						// Hide object if layer hidden
						CLayer layer=hoAdRunHeader.rhFrame.layers[nLayer];
						if ( (layer.dwOptions & (CLayer.FLOPT_TOHIDE|CLayer.FLOPT_VISIBLE)) != CLayer.FLOPT_VISIBLE )
						{
							pHo.ros.obHide();
						}
					}
				}

				// Met l'objet dans la liste des objets selectionnes
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				hoAdRunHeader.rhEvtProg.evt_AddCurrentObject(pHo);

				// Force l'animation SHOOT si definie
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				if ((hoOEFlags&CObjectCommon.OEFLAG_ANIMATIONS)!=0)
				{
					if (roa.anim_Exist(CAnim.ANIMID_SHOOT))
					{
						roa.animation_Force(CAnim.ANIMID_SHOOT);
						roa.animation_OneLoop();
					}
				}		
			}
			else
			{
				hoAdRunHeader.destroy_Add(pHo.hoNumber);
			}
		}
	}

	// Fonctions de base
	public abstract void init(CObjectCommon ocPtr, CCreateObjectInfo cob);
	public abstract void handle();
	public abstract void kill(boolean bFast);
	public abstract void getZoneInfos();
	public abstract void draw();
	public abstract CMask getCollisionMask(int flags);
	@Override
	public abstract void spriteDraw(CSprite spr, CImageBank bank, int x, int y);
	@Override
	public abstract CMask spriteGetMask();

	public void modif()
	{
	}

	public void display()
	{
	}

	public void reinitDisplay()
	{
	}
	
	public int fixedValue()
	{
		return (hoCreationId << 16) | ((hoNumber) & 0xFFFF);
	}
	
	public void setBoundingBoxFromWidth(int cx, int cy, int hsx, int hsy)
	{
		float nAngle = roc.rcAngle;
		float fScaleX = roc.rcScaleX;
		float fScaleY = roc.rcScaleY;
		
		// No rotation
		if ( nAngle == 0 )
		{
			// Stretch en X
			if ( fScaleX != 1.0f )
			{
				hsx = (int)(hsx * fScaleX);
				cx = (int)(cx * fScaleX);
			}
			
			// Stretch en Y
			if ( fScaleY != 1.0f )
			{
				hsy = (int)(hsy * fScaleY);
				cy = (int)(cy * fScaleY);
			}
		}
		
		// Rotation
		else
		{
			// Calculate dimensions
			if ( fScaleX != 1.0f )
			{
				hsx = (int)(hsx * fScaleX);
				cx = (int)(cx * fScaleX);
			}
			
			if ( fScaleY != 1.0f )
			{
				hsy = (int)(hsy * fScaleY);
				cy = (int)(cy * fScaleY);
			}
			
			// Rotate
			float alpha = (float) (nAngle * Math.PI / 180.0f);
			float cosa = (float) Math.cos(alpha);
			float sina = (float) Math.sin(alpha);
			
			int nx2, ny2;
			int	nx4, ny4;
			
			if ( sina >= 0.0f )
			{
				nx2 = (int)(cy * sina + 0.5f);		// (1.0f-sina));		// 1-sina est ici pour l'arrondi ??
				ny4 = -(int)(cx * sina + 0.5f);		// (1.0f-sina));
			}
			else
			{
				nx2 = (int)(cy * sina - 0.5f);		// (1.0f-sina));
				ny4 = -(int)(cx * sina - 0.5f);		// (1.0f-sina));
			}
			
			if ( cosa == 0.0f )
			{
				ny2 = 0;
				nx4 = 0;
			}
			else if ( cosa > 0 )
			{
				ny2 = (int)(cy * cosa + 0.5f);		// (1.0f-cosa));
				nx4 = (int)(cx * cosa + 0.5f);		// (1.0f-cosa));
			}
			else
			{
				ny2 = (int)(cy * cosa - 0.5f);		// (1.0f-cosa));
				nx4 = (int)(cx * cosa - 0.5f);		// (1.0f-cosa));
			}
			
			int nx3 = nx2 + nx4;
			int ny3 = ny2 + ny4;
			int nhsx = (int)(hsx * cosa + hsy * sina);
			int nhsy = (int)(hsy * cosa - hsx * sina);
			
			// Faire translation par rapport au hotspot
			int nx1 = 0;	// -nhsx;
			int ny1 = 0;	// -nhsy;
			
			// Calculer la nouvelle bounding box (? optimiser ?ventuellement)
			int x1 = Math.min(nx1, nx2);
			x1 = Math.min(x1, nx3);
			x1 = Math.min(x1, nx4);
			
			int x2 = Math.max(nx1, nx2);
			x2 = Math.max(x2, nx3);
			x2 = Math.max(x2, nx4);
			
			int y1 = Math.min(ny1, ny2);
			y1 = Math.min(y1, ny3);
			y1 = Math.min(y1, ny4);
			
			int y2 = Math.max(ny1, ny2);
			y2 = Math.max(y2, ny3);
			y2 = Math.max(y2, ny4);
			
			cx = x2 - x1;
			cy = y2 - y1;
			
			hsx = -(x1 - nhsx);
			hsy = -(y1 - nhsy);
		}			

		hoImgWidth = cx;
		hoImgHeight = cy;
		hoImgXSpot = hsx;
		hoImgYSpot = hsy;
	}
	
}
