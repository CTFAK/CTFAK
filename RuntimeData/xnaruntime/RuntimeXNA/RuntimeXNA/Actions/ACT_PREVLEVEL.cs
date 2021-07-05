// -----------------------------------------------------------------------------
//
// PREVIOUS LEVEL
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_PREVLEVEL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			rhPtr.rhQuit = CRun.LOOPEXIT_PREVLEVEL;
		}
	}
}