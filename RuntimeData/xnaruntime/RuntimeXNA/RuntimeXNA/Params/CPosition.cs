//----------------------------------------------------------------------------------
//
// CPOSITION: parametre position donnes
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;
using RuntimeXNA.Expressions;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Movements;
using RuntimeXNA.Objects;
using RuntimeXNA.OI;
using RuntimeXNA.Banks;

namespace RuntimeXNA.Params
{	
	public abstract class CPosition:CParam
	{
		public short posOINUMParent; //0
		public short posFlags;
		public short posX; //4
		public short posY;
		public short posSlope; //8
		public short posAngle;
		public int posDir; //12
		public short posTypeParent; //16
		public short posOiList; //18
		public short posLayer; //20
		
		public const short CPF_DIRECTION = (short) (0x0001);
		public const short CPF_ACTION = (short) (0x0002);
		public const short CPF_INITIALDIR = (short) (0x0004);
		public const short CPF_DEFAULTDIR = (short) (0x0008);
		
		public CPosition()
		{
		}
		
		// ----------------------------------
		// Interprete une structure POSITION EAX=Structure position
		// ----------------------------------
		public virtual bool read_Position(CRun rhPtr, int getDir, CPositionInfo pInfo)
		{
			pInfo.layer = - 1;
			
			if (posOINUMParent == - 1)
			{
				// Pas d'objet parent
				// ~~~~~~~~~~~~~~~~~~
				if (getDir != 0)
				// Tenir compte de la direction?
				{
					pInfo.dir = - 1;
					if ((posFlags & CPF_DEFAULTDIR) == 0)
					// Garder la direction de l'objet
					{
						pInfo.dir = rhPtr.get_Direction(posDir); // Va chercher la direction
					}
				}
				pInfo.x = posX;
				pInfo.y = posY;
				int nLayer = posLayer;
				if (nLayer > rhPtr.rhFrame.nLayers - 1)
					nLayer = rhPtr.rhFrame.nLayers - 1;
				pInfo.layer = nLayer;
				pInfo.bRepeat = false;
			}
			else
			{
				// Trouve le parent
				rhPtr.rhEvtProg.rh2EnablePick = false;
				CObject pHo = rhPtr.rhEvtProg.get_CurrentObjects(posOiList);
				pInfo.bRepeat = rhPtr.rhEvtProg.repeatFlag;
				if (pHo == null)
					return false;
				pInfo.x = pHo.hoX;
				pInfo.y = pHo.hoY;
				pInfo.layer = pHo.hoLayer;
				
				if ((posFlags & CPF_ACTION) != 0)
				// Relatif au point d'action?
				{
					if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_ANIMATIONS) != 0)
					{
						if (pHo.roc.rcImage != 0)
						{
							CImage ifo;
							ifo = rhPtr.rhApp.imageBank.getImageInfoEx(pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY);
							pInfo.x += ifo.xAP - ifo.xSpot;
							pInfo.y += ifo.yAP - ifo.ySpot;
						}
					}
				}
				
				if ((posFlags & CPF_DIRECTION) != 0)
				// Tenir compte de la direction?
				{
					int dir = (posAngle + pHo.roc.rcDir) & 0x1F; // La direction courante
					int px = CMove.getDeltaX(posSlope, dir);
					int py = CMove.getDeltaY(posSlope, dir);
					pInfo.x += px;
					pInfo.y += py;
				}
				else
				{
					pInfo.x += posX; // Additionne la position relative
					pInfo.y += posY;
				}
				
				if ((getDir & 0x01) != 0)
				{
					if ((posFlags & CPF_DEFAULTDIR) != 0)
					// Mettre la direction par defaut?
					{
						pInfo.dir = - 1;
					}
					else if ((posFlags & CPF_INITIALDIR) != 0)
					// Mettre la direction initiale?
					{
						pInfo.dir = pHo.roc.rcDir;
					}
					else
					{
						pInfo.dir = rhPtr.get_Direction(posDir); // Va cherche la direction
					}
				}
			}
			
			// Verification des directions: dans le terrain!!
			if ((getDir & 0x02) != 0)
			{
				if (pInfo.x < rhPtr.rh3XMinimumKill || pInfo.x > rhPtr.rh3XMaximumKill)
					return false;
				if (pInfo.y < rhPtr.rh3YMinimumKill || pInfo.y > rhPtr.rh3YMaximumKill)
					return false;
			}
			return true;
		}
		
		
		public abstract override void  load(CRunApp app);
	}
}