// -----------------------------------------------------------------------------
//
// DETROY STRING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STRDESTROY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo != null)
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
			}
		}
	}
}