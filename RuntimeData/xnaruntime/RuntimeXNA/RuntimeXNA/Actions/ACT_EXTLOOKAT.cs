// -----------------------------------------------------------------------------
//
// LOOK AT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTLOOKAT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
			{
				return ;
			}
			
			CPosition position = (CPosition) evtParams[0];
			CPositionInfo pInfo = new CPositionInfo();
			if (position.read_Position(rhPtr, 0, pInfo))
			{
				int x = pInfo.x;
				int y = pInfo.y;
				x -= pHo.hoX;
				y -= pHo.hoY;
				int dir = CRun.get_DirFromPente(x, y);
				dir &= 31;
				if (pHo.roc.rcDir != dir)
				{
					pHo.roc.rcDir = dir;
					pHo.roc.rcChanged = true;
					pHo.rom.rmMovement.setDir(dir);
				}
			}
		}
	}
}