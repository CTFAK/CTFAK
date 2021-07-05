//----------------------------------------------------------------------------------
//
// ON LOOP
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Conditions
{
	
	public class CND_ONLOOP:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			CParamExpression pExp = (CParamExpression) evtParams[0];
			if (pExp.tokens.Length == 2 && pExp.tokens[0].code == ((3 << 16) | 65535) && pExp.tokens[1].code == 0)
			{
                if (string.Compare(rhPtr.rh4CurrentFastLoop, ((EXP_STRING) pExp.tokens[0]).pString, StringComparison.CurrentCultureIgnoreCase)==0)
				{
					return true;
				}
				return false;
			}
			
			System.String pName = rhPtr.get_EventExpressionString(pExp);
            if (string.Compare(rhPtr.rh4CurrentFastLoop, pName, StringComparison.CurrentCultureIgnoreCase) != 0)
				return false;
			rhPtr.rhEvtProg.rh2ActionOn = false;
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			return false;
		}
	}
}