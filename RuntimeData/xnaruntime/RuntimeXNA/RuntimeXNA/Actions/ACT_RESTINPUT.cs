// -----------------------------------------------------------------------------
//
// RESTORE INPUT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_RESTINPUT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			rhPtr.rh2InputMask[evtOi] = (byte)(0xFF);
		}
	}
}