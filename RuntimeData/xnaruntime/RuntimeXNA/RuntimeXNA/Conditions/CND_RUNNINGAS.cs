// ------------------------------------------------------------------------------
// 
// RUNNING AS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Values;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Conditions
{
	
	public class CND_RUNNINGAS:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
            int num;
            if (evtParams[0].code == 67) // PARAM_RUNTIME
                num = ((PARAM_SHORT)evtParams[0]).value;
            else
                num = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            if (num == 6)      // RUNTIME_XNA
                return negaTRUE();
            return negaFALSE();
        }
	}
}