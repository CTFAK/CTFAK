//----------------------------------------------------------------------------------
//
// NOT ALWAYS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
namespace RuntimeXNA.Conditions
{
	
	public class CND_NOTALWAYS:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			CEventGroup pEvg = rhPtr.rhEvtProg.rhEventGroup;
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_NOTALWAYS) != 0)
				return true; // Deja evalue?
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_NOMORE) != 0)
				return false; //; Verification, valide?
			pEvg.evgInhibit = unchecked((ushort)-2); // Premier coup!
			pEvg.evgFlags |= CEventGroup.EVGFLAGS_NOTALWAYS;
			return true;
		}
	}
}