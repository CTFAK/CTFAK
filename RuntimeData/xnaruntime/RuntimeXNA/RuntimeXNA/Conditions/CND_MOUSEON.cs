// ------------------------------------------------------------------------------
// 
// MOUSE ON
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_MOUSEON:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			if (rhPtr.isMouseOn())
			{
				return negaTRUE();
			}
			return negaFALSE();
		}
	}
}