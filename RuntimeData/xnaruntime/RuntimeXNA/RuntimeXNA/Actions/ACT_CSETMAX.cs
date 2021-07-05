// -----------------------------------------------------------------------------
//
// SET VALUE MAX
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Expressions;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CSETMAX:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[0]);
			((CCounter) pHo).cpt_SetMax(pValue);
		}
	}
}