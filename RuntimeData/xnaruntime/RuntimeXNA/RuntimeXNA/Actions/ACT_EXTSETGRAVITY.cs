// -----------------------------------------------------------------------------
//
// SET GRAVITY
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETGRAVITY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int grav = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			pHo.rom.rmMovement.setGravity(grav);
		}
	}
}