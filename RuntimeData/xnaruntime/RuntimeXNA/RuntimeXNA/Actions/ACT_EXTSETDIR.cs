// -----------------------------------------------------------------------------
//
// SET DIRECTION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETDIR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int dir;
			if (evtParams[0].code == 29)
			// PARAM_NEWDIRECTION)
				dir = rhPtr.get_Direction(((PARAM_INT) evtParams[0]).value_Renamed);
			else
				dir = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			dir &= 31;
			if (pHo.roc.rcDir != dir)
			{
				pHo.roc.rcDir = dir;
				pHo.roc.rcChanged = true;
				pHo.rom.rmMovement.setDir(dir);
				
				if (pHo.hoType == 2)
				// OBJ_SPR)
				{
					pHo.roa.animIn(0);
				}
			}
		}
	}
}