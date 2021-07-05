// ------------------------------------------------------------------------------
// 
// X CHANCES OUT OF Y
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHANCE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int param1 = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			int param2 = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			if (param2 >= 1 && param1 > 0 && param1 <= param2)
			{
				int rnd = rhPtr.random((short) param2);
				if (rnd <= param1)
				{
					return true;
				}
			}
			return false;
		}
	}
}