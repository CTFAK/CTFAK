// -----------------------------------------------------------------------------
//
// SET GLOBAL VALUE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CCASETGLOBALVALUE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			CValue value_Renamed = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[1]);
			
			((CCCA) pHo).setGlobalValue(number, value_Renamed);
		}
	}
}