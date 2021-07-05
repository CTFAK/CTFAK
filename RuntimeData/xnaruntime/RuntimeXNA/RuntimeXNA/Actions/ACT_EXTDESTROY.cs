// -----------------------------------------------------------------------------
//
// DESTROY
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.OI;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTDESTROY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			if (pHo.hoType == 3)
			// OBJ_TEXT)
			{
				CText pText = (CText) pHo;
				if ((pText.rsHidden & CRun.COF_FIRSTTEXT) != 0)
				//; Le dernier objet texte?
				{
					pHo.ros.obHide(); //; Cache pour le moment
					pHo.ros.rsFlags &= ~ CRSpr.RSFLAG_VISIBLE;
					pHo.hoFlags |= CObject.HOF_NOCOLLISION;
				}
				else
				{
					pHo.hoFlags |= CObject.HOF_DESTROYED; //; NON: on le detruit!
					rhPtr.destroy_Add(pHo.hoNumber);
				}
				return ;
			}
			if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)
			{
				pHo.hoFlags |= CObject.HOF_DESTROYED;
				if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_ANIMATIONS) != 0 || (pHo.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0)
				{
					// Jouer l'anim disappear
					rhPtr.init_Disappear(pHo);
				}
				else
				{
					// Pas un objet avec animation : destroy
					pHo.hoCallRoutine = false;
					rhPtr.destroy_Add(pHo.hoNumber);
				}
			}
		}
	}
}