// ------------------------------------------------------------------------------
// 
// QUESTION EQUALS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_QEQUAL:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			// Le parametre
			int num = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			// Compare
			if (rhPtr.rhEvtProg.rhCurParam0 == num)
				return true;
			return false;
		}
		public override bool eva2(CRun rhPtr)
		{
			return false;
		}
	}
}