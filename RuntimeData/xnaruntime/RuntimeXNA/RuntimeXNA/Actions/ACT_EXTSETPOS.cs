// -----------------------------------------------------------------------------
//
// SET POSITION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETPOS:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			CPosition position = (CPosition) evtParams[0];
			CPositionInfo pInfo = new CPositionInfo();
            if (position.read_Position(rhPtr, 0, pInfo))
            {
                CRun.setXPosition(pHo, pInfo.x);
                CRun.setYPosition(pHo, pInfo.y);
            }
		}
	}
}