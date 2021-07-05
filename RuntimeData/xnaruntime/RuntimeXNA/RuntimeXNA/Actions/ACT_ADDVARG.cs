// -----------------------------------------------------------------------------
//
// ADD TO GLOBAL VARIABLE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Actions
{
	
	public class ACT_ADDVARG:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int num;
			if (evtParams[0].code == 52)
			// PARAM_VARGLOBAL_EXP 
				num = (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]) - 1);
			// &15; YVES: enlev
			else
				num = ((PARAM_SHORT) evtParams[0]).value;
			
			CValue value = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[1]);
			rhPtr.rhApp.getGlobalValueAt(num).add(value);
		}
	}
}