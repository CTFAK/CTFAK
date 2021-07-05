// -----------------------------------------------------------------------------
//
// SET Y POSITION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int y = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			CRun.setYPosition(pHo, y);
		}
	}
}