// -----------------------------------------------------------------------------
//
// COUNTER ADD VALUE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Expressions;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CADDVALUE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[0]);
			((CCounter) pHo).cpt_Add(pValue);
		}
	}
}