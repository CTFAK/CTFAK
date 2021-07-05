// ------------------------------------------------------------------------------
// 
// TIME OUT
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_TIMEOUT:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			long time;
			if (evtParams[0].code == 22)
			// PARAM_EXPRESSION
				time = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			else
				time = ((PARAM_TIME) evtParams[0]).timer;
			
			if (rhPtr.rh4TimeOut > time)
				return true;
			return false;
		}
	}
}