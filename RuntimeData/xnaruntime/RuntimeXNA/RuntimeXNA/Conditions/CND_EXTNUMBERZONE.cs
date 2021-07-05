// ------------------------------------------------------------------------------
// 
// COMPARE TO NUMBER OF OBJECTS IN ZONE
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTNUMBERZONE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int count = rhPtr.rhEvtProg.count_ZoneOneObject(evtOiList, (PARAM_ZONE) evtParams[0]);
			int number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			return CRun.compareTer(count, number, ((CParamExpression) evtParams[1]).comparaison);
		}
	}
}