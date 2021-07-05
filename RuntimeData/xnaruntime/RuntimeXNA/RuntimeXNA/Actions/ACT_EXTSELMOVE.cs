// -----------------------------------------------------------------------------
//
// SELECT MOVE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSELMOVE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int n;
			if (evtParams[0].code == 22)
			// PARAM_EXPRESSION)
			{
				n = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			}
			else
			{
				n = ((PARAM_SHORT) evtParams[0]).value;
			}
			if (pHo.rom != null)
			{
				pHo.rom.selectMovement(pHo, n);
			}
		}
	}
}