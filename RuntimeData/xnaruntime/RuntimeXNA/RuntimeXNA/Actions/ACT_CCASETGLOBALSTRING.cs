// -----------------------------------------------------------------------------
//
// SET GLOBAL STRING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CCASETGLOBALSTRING:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			System.String s = rhPtr.get_EventExpressionString((CParamExpression) evtParams[1]);
			
			((CCCA) pHo).setGlobalString(number, s);
		}
	}
}