// -----------------------------------------------------------------------------
//
// HIDE CURSOR
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_HIDECURSOR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			if (rhPtr.rhMouseUsed == 0)
			{
				rhPtr.hideMouse();
			}
		}
	}
}