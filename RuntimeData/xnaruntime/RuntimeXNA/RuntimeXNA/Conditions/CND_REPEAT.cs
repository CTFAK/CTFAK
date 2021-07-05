//----------------------------------------------------------------------------------
//
// REPEAT N TIMES
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_REPEAT:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			CEventGroup pEvg = rhPtr.rhEvtProg.rhEventGroup;
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_REPEAT) != 0)
				return true; //; Deja evalue?
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_NOMORE) != 0)
				return false; //; Verification, valide?
			
			// Va evaluer l'expression
			pEvg.evgInhibitCpt = (short) rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]); //; Repeat valide!
			pEvg.evgFlags |= CEventGroup.EVGFLAGS_REPEAT;
			return true;
		}
	}
}