// -----------------------------------------------------------------------------
//
// CLEAR FLAG
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTCLRFLAG:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			if (pHo.rov != null)
			{
				int number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				pHo.rov.rvValueFlags &= ~ (1 << number);
			}
		}
	}
}