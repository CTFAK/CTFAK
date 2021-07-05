// -----------------------------------------------------------------------------
//
// CENTER DISPLAY
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CDISPLAY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CPosition position = (CPosition) evtParams[0];
			CPositionInfo pInfo = new CPositionInfo();
			position.read_Position(rhPtr, 0, pInfo);
			rhPtr.setDisplay(pInfo.x, pInfo.y, pInfo.layer, 3);
		}
	}
}