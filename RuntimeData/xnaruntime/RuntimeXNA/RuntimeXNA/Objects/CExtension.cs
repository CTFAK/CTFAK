//----------------------------------------------------------------------------------
//
// CEXTENSION: Objets d'extension
//
//----------------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Extensions;
using RuntimeXNA.Services;
using RuntimeXNA.Conditions;
using RuntimeXNA.Actions;
using RuntimeXNA.Expressions;
using RuntimeXNA.Events;
using RuntimeXNA.Params;
using RuntimeXNA.Frame;
using RuntimeXNA.Movements;
using RuntimeXNA.Application;
namespace RuntimeXNA.Objects
{
	
	public class CExtension:CObject, IDrawing
	{
		public CRunExtension ext;
		bool noHandle = false;
		public int privateData = 0;
		public int objectCount;
		public int objectNumber;
		
		public CExtension(int type, CRun rhPtr)
		{
			ext = rhPtr.rhApp.extLoader.loadRunObject(type);
		}
		
		public override void  init(CObjectCommon ocPtr, CCreateObjectInfo cob)
		{
			// Initialisation des pointeurs
			ext.init(this);
			
			// Initialisation de l'objet
			CFile file = null;
			if (ocPtr.ocExtension != null)
			{
				file = new CFile(ocPtr.ocExtension);
                file.setUnicode(hoAdRunHeader.rhApp.bUnicode);
			}
			privateData = ocPtr.ocPrivate;
			ext.createRunObject(file, cob, ocPtr.ocVersion);
		}
		
		public override void  handle()
		{
			// Routines standard;
			if ((hoOEFlags & 0x0200) != 0)
			// OEFLAG_SPRITE
			{
				ros.handle();
			}
			else if ((hoOEFlags & 0x0030) == 0x0010 || (hoOEFlags & 0x0030) == 0x0030)
			// OEFLAG_MOVEMENTS / OEFLAG_ANIMATIONS|OEFLAG_MOVEMENTS
			{
				rom.move();
			}
			else if ((hoOEFlags & 0x0030) == 0x0020)
			// OEFLAG_ANIMATION
			{
				roa.animate();
			}
			
			// Handle de l'objet
			int ret = 0;
			if (noHandle == false)
			{
				ret = ext.handleRunObject();
			}
			
			if ((ret & CRunExtension.REFLAG_ONESHOT) != 0)
			{
				noHandle = true;
			}
			if (roc != null)
			{
				if (roc.rcChanged)
				{
					ret |= CRunExtension.REFLAG_DISPLAY;
					roc.rcChanged = false;
				}
			}
			if ((ret & CRunExtension.REFLAG_DISPLAY) != 0)
			{
				modif();
			}
		}
		
		public override void  modif()
		{
			if (ros != null)
			{
				ros.modifRoutine();
			}
            else if ((hoOEFlags & CObjectCommon.OEFLAG_BACKGROUND) != 0)
			{
                hoAdRunHeader.redrawLevel(CRun.DLF_DONTUPDATE);
			}
			else
			{
				ext.displayRunObject(null);
			}
		}
		
		public override void  display()
		{
		}
		
		public override void  kill(bool bFast)
		{
			ext.destroyRunObject(bFast);
		}
		
		public override void  getZoneInfos()
		{
			ext.getZoneInfos();
			hoRect.left = hoX - hoAdRunHeader.rhWindowX - hoImgXSpot; // Calcul des coordonnees
			hoRect.right = hoRect.left + hoImgWidth;
			hoRect.top = hoY - hoAdRunHeader.rhWindowY - hoImgYSpot;
			hoRect.bottom = hoRect.top + hoImgHeight;
		}
		
		public override CMask getCollisionMask(int flags)
		{
			return ext.getRunObjectCollisionMask(flags);
		}
		
		public override void draw(SpriteBatchEffect batch)
		{
			Texture2D img = ext.getRunObjectSurface();
			if (img != null)
			{
                Rectangle tempRect = new Rectangle();
                tempRect.X = hoRect.left + hoAdRunHeader.rhApp.xOffset;
                tempRect.Y = hoRect.top + hoAdRunHeader.rhApp.yOffset;
                tempRect.Width = hoRect.right-hoRect.left;
                tempRect.Height = hoRect.bottom-hoRect.top;
                batch.Draw(img, tempRect, null, Color.White);
			}
			else
			{
				ext.displayRunObject(batch);
			}
		}

        public override void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
        {
            draw(batch);
        }
        public override void drawableKill() 
        { 
        }
        
        public override CMask drawableGetMask(int flags) 
        {
            return ext.getRunObjectCollisionMask(flags);
        }

        public virtual bool condition(int num, CCndExtension cnd)
		{
			return ext.condition(num, cnd);
		}
		
		public virtual void  action(int num, CActExtension act)
		{
			ext.action(num, act);
		}
		
		public virtual CValue expression(int num)
		{
			return ext.expression(num);
		}

        public int getX()
        {
            return hoX;
        }

        public int getY()
        {
            return hoY;
        }

        public int getWidth()
        {
            return hoImgWidth;
        }

        public int getHeight()
        {
            return hoImgHeight;
        }

        public void setX(int x)
        {
            if (rom != null)
            {
                rom.rmMovement.setXPosition(x);
            }
            else
            {
                hoX = x;
                if (roc != null)
                {
                    roc.rcChanged = true;
                    roc.rcCheckCollides = true;
                }
            }
        }

        public void setY(int y)
        {
            if (rom != null)
            {
                rom.rmMovement.setYPosition(y);
            }
            else
            {
                hoY = y;
                if (roc != null)
                {
                    roc.rcChanged = true;
                    roc.rcCheckCollides = true;
                }
            }
        }

        public void setWidth(int width)
        {
            hoImgWidth = width;
            hoRect.right = hoRect.left + width;
        }

        public void setHeight(int height)
        {
            hoImgHeight = height;
            hoRect.bottom = hoRect.top + height;
        }

		public virtual void  loadImageList(short[] list)
		{
			hoAdRunHeader.rhApp.imageBank.loadImageList(list);
		}
		
		public virtual CImage getImage(short handle)
		{
			return hoAdRunHeader.rhApp.imageBank.getImageFromHandle(handle);
		}
/* FRANCOIS		
		public virtual int scaleX(int x)
		{
			return hoAdRunHeader.scaleX(x);
		}
		
		public virtual int scaleY(int y)
		{
			return hoAdRunHeader.scaleY(y);
		}
*/		
		public virtual void  reHandle()
		{
			noHandle = false;
		}
		
		public virtual void  generateEvent(int code, int param)
		{
			if (hoAdRunHeader.rh2PauseCompteur == 0)
			{
				int p0 = hoAdRunHeader.rhEvtProg.rhCurParam0;
				hoAdRunHeader.rhEvtProg.rhCurParam0 = param;
				
				code = (- (code + CEventProgram.EVENTS_EXTBASE + 1) << 16);
				code |= (((int) hoType) & 0xFFFF);
				hoAdRunHeader.rhEvtProg.handle_Event(this, code);
				
				hoAdRunHeader.rhEvtProg.rhCurParam0 = p0;
			}
		}
		
		public virtual void  pushEvent(int code, int param)
		{
			if (hoAdRunHeader.rh2PauseCompteur == 0)
			{
				code = (- (code + CEventProgram.EVENTS_EXTBASE + 1) << 16);
				code |= (((int) hoType) & 0xFFFF);
				hoAdRunHeader.rhEvtProg.push_Event(1, code, param, this, hoOi);
			}
		}
		
		public virtual void  pause()
		{
			hoAdRunHeader.pause();
		}
		
		public virtual void  resume()
		{
			hoAdRunHeader.resume();
		}
		
		public virtual void  redisplay()
		{
			hoAdRunHeader.ohRedrawLevel(true);
		}
		
		public virtual void  redraw()
		{
			modif();
			if ((hoOEFlags & (CObjectCommon.OEFLAG_ANIMATIONS | CObjectCommon.OEFLAG_MOVEMENTS | CObjectCommon.OEFLAG_SPRITES)) != 0)
			{
				roc.rcChanged = true;
			}
		}
		
		public virtual void  destroy()
		{
			hoAdRunHeader.destroy_Add(hoNumber);
		}
		
		public virtual void  setPosition(int x, int y)
		{
			if (rom != null)
			{
				rom.rmMovement.setXPosition(x);
				rom.rmMovement.setYPosition(y);
			}
			else
			{
				hoX = x;
				hoY = y;
				if (roc != null)
				{
					roc.rcChanged = true;
					roc.rcCheckCollides = true;
				}
			}
		}
		
		public virtual void  addBackdrop(CImage img, int x, int y, int dwEffect, int dwEffectParam, int typeObst, int nLayer)
		{
/*			// Duplique et ajoute l'image
			int width = img.Width;
			int height = img.Height;
			System.Drawing.Bitmap newImg = new System.Drawing.Bitmap(width, height, (System.Drawing.Imaging.PixelFormat) System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Graphics g2 = System.Drawing.Graphics.FromImage(newImg);
			//UPGRADE_WARNING: Method 'java.awt.Graphics.drawImage' was converted to 'System.Drawing.Graphics.drawImage' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
			g2.DrawImage(img, 0, 0);
			g2.Dispose();
			short handle = hoAdRunHeader.rhApp.imageBank.addImageCompare(newImg, (short) 0, (short) 0, (short) 0, (short) 0);
			
			// Ajoute a la liste
			CBkd2 toadd = new CBkd2();
			toadd.img = handle;
			toadd.loHnd = 0;
			toadd.oiHnd = 0;
			toadd.x = x;
			toadd.y = y;
			toadd.nLayer = (short) nLayer;
			toadd.inkEffect = dwEffect;
			toadd.inkEffectParam = dwEffectParam;
			toadd.colMode = CSpriteGen.CM_BITMAP;
			toadd.obstacleType = (short) typeObst; // a voir
			for (int ns = 0; ns < 4; ns++)
			{
				toadd.pSpr[ns] = null;
			}
			hoAdRunHeader.addBackdrop2(toadd);
			
			// Add paste routine (pour éviter d'avoir à réafficher tout le décor)
			if (nLayer == 0 && (hoAdRunHeader.rhFrame.layers[0].dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) == CLayer.FLOPT_VISIBLE)
			{
				CBackDrawPaste paste;
				paste = new CBackDrawPaste();
				paste.img = handle;
				paste.x = x;
				paste.y = y;
				paste.typeObst = (short) typeObst;
				paste.inkEffect = dwEffect;
				paste.inkEffectParam = dwEffectParam;
				hoAdRunHeader.addBackDrawRoutine(paste);
				
				// Redraw sprites that intersect with the rectangle
				CRect rc = new CRect();
				rc.left = x - hoAdRunHeader.rhWindowX;
				rc.top = y - hoAdRunHeader.rhWindowY;
				rc.right = rc.left + width;
				rc.bottom = rc.top + height;
				hoAdRunHeader.spriteGen.activeSprite(null, CSpriteGen.AS_REDRAW_RECT, rc);
			}
 */
		}

        public int getEventCount()
        {
            return hoAdRunHeader.rh4EventCount;
        }

        public CValue getExpParam()
        {
            hoAdRunHeader.rh4CurToken++;		// Saute la fonction
            return hoAdRunHeader.getExpression();
        }

        public int getEventParam()
        {
            return hoAdRunHeader.rhEvtProg.rhCurParam0;
        }

        public virtual double callMovement(CObject hoPtr, int action, double param)
		{
			if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0)
			{
				if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
				{
					CMoveExtension mvPtr = (CMoveExtension) hoPtr.rom.rmMovement;
					return mvPtr.callMovement(action, param);
				}
			}
			return 0;
		}
		
		public virtual CValue callExpression(CObject hoPtr, int action, int param)
		{
			CExtension pExtension = (CExtension) hoPtr;
			pExtension.privateData = param;
			return pExtension.expression(action);
		}
		
		public virtual CObject getObjectFromFixed(int fixed_Renamed)
		{
			int count = 0;
			int number;
			for (number = 0; number < hoAdRunHeader.rhNObjects; number++)
			{
				while (hoAdRunHeader.rhObjectList[count] == null)
				{
					count++;
				}
				CObject hoPtr = hoAdRunHeader.rhObjectList[count];
				count++;
				int id = (hoPtr.hoCreationId << 16) | (((int) hoPtr.hoNumber) & 0xFFFF);
				if (id == fixed_Renamed)
				{
					return hoPtr;
				}
			}
			return null;
		}

        public CObject getFirstObject()
        {
            objectCount = 0;
            objectNumber = 0;
            return getNextObject();
        }

        public CObject getNextObject()
        {
            if (objectNumber < hoAdRunHeader.rhNObjects)
            {
                while (hoAdRunHeader.rhObjectList[objectCount] == null)
                {
                    objectCount++;
                }
                CObject hoPtr = hoAdRunHeader.rhObjectList[objectCount];
                objectNumber++;
                objectCount++;
                return hoPtr;
            }
            return null;
        }

		
	}
}