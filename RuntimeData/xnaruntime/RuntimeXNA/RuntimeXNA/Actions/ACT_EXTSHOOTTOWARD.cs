// -----------------------------------------------------------------------------
//
// SHOOT TOWARD
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Animations;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSHOOTTOWARD:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			// Peut-on tirer?
			// ~~~~~~~~~~~~~~
//			if (pHo.roa.raAnimOn == CAnim.ANIMID_SHOOT)
//				return ; //; Deja en train de tirer?
			
			// Cherche la position de creation
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			PARAM_SHOOT pEvp = (PARAM_SHOOT) evtParams[0];
			CPositionInfo pInfo = new CPositionInfo();
			if (pEvp.read_Position(rhPtr, 0x11, pInfo))
			{
				CPositionInfo pInfoDest = new CPositionInfo();
				if (((CPosition) evtParams[1]).read_Position(rhPtr, 0, pInfoDest))
				{
					// Trouve la bonne direction
					int x2 = pInfoDest.x;
					int y2 = pInfoDest.y;
					int dir = CRun.get_DirFromPente(x2 - pInfo.x, y2 - pInfo.y); // Calcul des pentes
					
					// Va creer la balle
					pHo.shtCreate(pEvp, pInfo.x, pInfo.y, dir);
				}
			}
		}
	}
}