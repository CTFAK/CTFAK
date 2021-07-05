// -----------------------------------------------------------------------------
//
// JUMP TO FRAME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CCAJUMPFRAME:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int frame = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			((CCCA) pHo).jumpFrame(frame);
		}
	}
}