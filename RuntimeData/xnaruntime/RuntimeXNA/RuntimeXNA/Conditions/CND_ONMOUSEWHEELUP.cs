// ------------------------------------------------------------------------------
// 
// ON MOUSE WHEEL UP
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_ONMOUSEWHEELUP:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			if (rhPtr.rh4OnMouseWheel + 1 != rhPtr.rhLoopCount)
				return false;
			if (rhPtr.rh4MouseWheelDelta >= 0)
				return false;
			return true;
		}
	}
}