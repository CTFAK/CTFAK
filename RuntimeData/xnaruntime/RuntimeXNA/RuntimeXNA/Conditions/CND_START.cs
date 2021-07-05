// ------------------------------------------------------------------------------
// 
// START OF FRAME
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_START:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			if (rhPtr.rhLoopCount > 2)
				return false;
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			if (rhPtr.rhLoopCount > 2)
				return false;
			return true;
		}
	}
}