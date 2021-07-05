// ------------------------------------------------------------------------------
// 
// KEY PRESSED
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_KBPRESSKEY:CCnd
	{
		
		public CND_KBPRESSKEY()
		{
		}
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
/*			if (rhPtr.rh4DemoMode == CDemoRecord.DEMOPLAY)
			{
				if (rhPtr.rh4Demo.getKeyState(((PARAM_KEY) evtParams[0]).key) == false)
					return negaFALSE();
			}
			else
 */ 
			{
				if (rhPtr.isKeyDown(((PARAM_KEY) evtParams[0]).key) == false)
					return negaFALSE();
			}
			if (compute_GlobalNoRepeat(rhPtr))
				return negaTRUE();
			else
				return negaFALSE();
		}
	}
}