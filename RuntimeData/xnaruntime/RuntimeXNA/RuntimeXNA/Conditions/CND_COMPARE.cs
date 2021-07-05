//----------------------------------------------------------------------------------
//
// COMPARE TWO GENERAL VALUES
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Conditions
{
	
	public class CND_COMPARE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			CValue value1 = new CValue();
			value1.forceValue(rhPtr.get_EventExpressionAny((CParamExpression) evtParams[0]));
			
			CParamExpression pp = (CParamExpression) evtParams[1];
			CValue value2 = rhPtr.get_EventExpressionAny(pp);
			
			return CRun.compareTo(value1, value2, pp.comparaison);
		}
	}
}