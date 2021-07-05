// -----------------------------------------------------------------------------
//
// FORCE ANIMATION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTFORCEANIM:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int ani;
			if (evtParams[0].code == 10)
			// PARAM_ANIMATION)
				ani = ((PARAM_SHORT) evtParams[0]).value;
			else
				ani = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			pHo.roa.animation_Force(ani);
			pHo.roc.rcChanged = true; // Build 243	
		}
	}
}