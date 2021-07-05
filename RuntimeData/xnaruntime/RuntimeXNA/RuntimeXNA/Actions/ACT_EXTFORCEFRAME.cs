// -----------------------------------------------------------------------------
//
// FORCE ANIMATION FRAME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTFORCEFRAME:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int frame = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			pHo.roa.animFrame_Force(frame);
			pHo.roc.rcChanged = true;
		}
	}
}