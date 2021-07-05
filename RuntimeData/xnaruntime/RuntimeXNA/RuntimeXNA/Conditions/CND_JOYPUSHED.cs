// ------------------------------------------------------------------------------
// 
// JOYSTICK PUSHSED
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_JOYPUSHED:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			short s = (short) rhPtr.rhPlayer[evtOi];
			PARAM_SHORT p = (PARAM_SHORT) evtParams[0];
			s &= p.value;
			if (s != p.value)
				return negaFALSE();
			return negaTRUE();
		}
	}
}