// -----------------------------------------------------------------------------
//
// SET TIMER
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETTIMER:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			long newTime;
			if (evtParams[0].code == 22)
			// PARAM_EXPRESSION
				newTime = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			else
				newTime = ((PARAM_TIME) evtParams[0]).timer;
			
			rhPtr.rhTimer = newTime;
			rhPtr.rhTimerOld = rhPtr.rhApp.timer - rhPtr.rhTimer;
			
			// Reactive les evenements timer...
			rhPtr.rhEvtProg.restartTimerEvents();
		}
	}
}