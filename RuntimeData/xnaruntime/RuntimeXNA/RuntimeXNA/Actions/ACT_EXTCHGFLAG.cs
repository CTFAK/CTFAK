// -----------------------------------------------------------------------------
//
// TOGGLE FLAG
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTCHGFLAG:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			if (pHo.rov != null)
			{
				int number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				int mask = 1 << number;
				if ((pHo.rov.rvValueFlags & mask) != 0)
					pHo.rov.rvValueFlags &= ~ mask;
				else
					pHo.rov.rvValueFlags |= mask;
			}
		}
	}
}