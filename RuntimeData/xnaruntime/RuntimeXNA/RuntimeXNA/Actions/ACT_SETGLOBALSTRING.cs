// -----------------------------------------------------------------------------
//
// SET GLOBAL STRING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETGLOBALSTRING:CAct
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
			
			System.String string_Renamed = rhPtr.get_EventExpressionString((CParamExpression) evtParams[1]);
			rhPtr.rhApp.setGlobalStringAt(num, string_Renamed);
		}
	}
}