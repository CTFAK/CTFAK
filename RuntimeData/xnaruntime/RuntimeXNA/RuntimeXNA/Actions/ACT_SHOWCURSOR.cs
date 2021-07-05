// -----------------------------------------------------------------------------
//
// SHOW CURSOR
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SHOWCURSOR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			if (rhPtr.rhMouseUsed == 0)
			{
				rhPtr.showMouse();
			}
		}
	}
}