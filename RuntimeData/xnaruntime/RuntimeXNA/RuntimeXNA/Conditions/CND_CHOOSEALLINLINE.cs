// ------------------------------------------------------------------------------
// 
// CHOOSE ALL OBJECT IN A LINE
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEALLINLINE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int x1 = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			int y1 = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			int x2 = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[2]);
			int y2 = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[3]);
			
			if (rhPtr.rhEvtProg.select_LineOfSight(x1, y1, x2, y2) != 0)
				return true;
			return false;
		}
	}
}