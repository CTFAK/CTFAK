// -----------------------------------------------------------------------------
//
// FORCE SPEED
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTFORCESPEED:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int speed;
			speed = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			pHo.roa.animSpeed_Force(speed);
		}
	}
}