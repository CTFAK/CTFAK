// -----------------------------------------------------------------------------
//
// SET ACCELERATION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETACCELERATION:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int acc = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			pHo.rom.rmMovement.setAcc(acc);
		}
	}
}