// -----------------------------------------------------------------------------
//
// FORCE DIRECTION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTFORCEDIR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int ani;
			if (evtParams[0].code == 29)
			// PARAM_NEWDIRECTION)
				ani = rhPtr.get_Direction(((PARAM_INT) evtParams[0]).value_Renamed);
			else
				ani = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			pHo.roa.animDir_Force(ani);
			pHo.roc.rcChanged = true; // Build 243	
		}
	}
}