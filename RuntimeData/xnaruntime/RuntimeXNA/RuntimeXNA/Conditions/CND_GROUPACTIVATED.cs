//----------------------------------------------------------------------------------
//
// ON GROUP ACTIVATED
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Events;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_GROUPACTIVATED:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			CEventGroup pEvg = rhPtr.rhEvtProg.events[((PARAM_GROUPOINTER) evtParams[0]).pointer];
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_INACTIVE) != 0)
				return negaFALSE();
			return negaTRUE();
		}
	}
}