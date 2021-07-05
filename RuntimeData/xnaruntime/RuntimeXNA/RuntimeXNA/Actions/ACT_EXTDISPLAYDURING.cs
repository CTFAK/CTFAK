// -----------------------------------------------------------------------------
//
// DISPLAY DURING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTDISPLAYDURING:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			if (pHo.ros != null)
			{
				pHo.ros.obHide();
				pHo.ros.rsFlags &= ~ CRSpr.RSFLAG_VISIBLE;
				
				pHo.ros.rsFlash = ((PARAM_TIME) evtParams[0]).timer;
				pHo.ros.rsFlashCpt = ((PARAM_TIME) evtParams[0]).timer;
			}
		}
	}
}