//----------------------------------------------------------------------------------
//
// ONCE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
namespace RuntimeXNA.Conditions
{
	
	public class CND_ONCE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			CEventGroup pEvg = rhPtr.rhEvtProg.rhEventGroup;
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_ONCE) != 0)
				return false; // Deja evalue?
			pEvg.evgFlags |= CEventGroup.EVGFLAGS_ONCE; //; Marque pour le prochain!
			return true;
		}
	}
}