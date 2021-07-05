// -----------------------------------------------------------------------------
//
// CREATE OBJECT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Objects;
using RuntimeXNA.OI;
using RuntimeXNA.Frame;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CREATE:CAct
	{
		public ACT_CREATE()
		{
		}
		
		public override void  execute(CRun rhPtr)
		{
			// Cherche la position de creation
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			PARAM_CREATE pEvp = (PARAM_CREATE) evtParams[0];
			CPositionInfo pInfo = new CPositionInfo();
			if (pEvp.read_Position(rhPtr, 0x11, pInfo))
			{
				if (pInfo.bRepeat)
				{
					evtFlags |= ACTFLAGS_REPEAT; // Refaire cette action
					rhPtr.rhEvtProg.rh2ActionLoop = true; // Refaire un tour d'actions
				}
				else
				{
					evtFlags &= unchecked ((byte)~ ACTFLAGS_REPEAT); // Ne pas refaire cette action
				}
			}
			
			// Cree l'objet
			// ~~~~~~~~~~~~
			int number = rhPtr.f_CreateObject(pEvp.cdpHFII, pEvp.cdpOi, pInfo.x, pInfo.y, pInfo.dir, (short) 0, pInfo.layer, - 1);
			
			// Met l'objet dans la liste des objets selectionnes
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (number >= 0)
			{
				CObject pHo = rhPtr.rhObjectList[number];
				rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
				if (pInfo.layer != - 1)
				{
					if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0)
					{
						// Hide object if layer hidden
						CLayer pLayer = rhPtr.rhFrame.layers[pInfo.layer];
						if ((pLayer.dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) != CLayer.FLOPT_VISIBLE)
						{
							pHo.ros.obHide();
						}
					}
				}
			}
		}
	}
}