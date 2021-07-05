// -----------------------------------------------------------------------------
//
// SET X POSITION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETX:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int x = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			CRun.setXPosition(pHo, x);
		}
	}
}