// -----------------------------------------------------------------------------
//
// MOVE BEFORE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTMOVEBEFORE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			if (pHo.ros != null)
			{
				CObject pHo2 = rhPtr.rhEvtProg.get_ParamActionObjects(((PARAM_OBJECT) evtParams[0]).oiList, this);
				if (pHo2 == null)
					return ;
				
				CSprite pSpr = pHo.roc.rcSprite;
				CSprite pSpr2 = pHo2.roc.rcSprite;
				if (pSpr != null && pSpr2 != null)
				{
					rhPtr.rhApp.spriteGen.moveSpriteBefore(pSpr, pSpr2);
				}
			}
		}
	}
}