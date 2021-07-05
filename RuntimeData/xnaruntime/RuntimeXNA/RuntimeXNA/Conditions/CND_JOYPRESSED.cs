// ------------------------------------------------------------------------------
// 
// JOYSTICK PRESSED
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_JOYPRESSED:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			int joueur = evtOi; //; Le numero du player
			if (joueur != rhPtr.rhEvtProg.rhCurOi)
				return false;
			
			short j = (short) rhPtr.rhEvtProg.rhCurParam0;
			PARAM_SHORT p = (PARAM_SHORT) evtParams[0];
			j &= p.value;
			if (j != p.value)
				return false;
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			int joueur = evtOi; //; Le numero du player
			sbyte b = (sbyte) (rhPtr.rh2NewPlayer[joueur] & rhPtr.rhPlayer[joueur]);
			
			short s = (short) b;
			PARAM_SHORT p = (PARAM_SHORT) evtParams[0];
			s &= p.value;
			if (p.value != s)
				return false;
			return true;
		}
	}
}