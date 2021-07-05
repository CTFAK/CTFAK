// ------------------------------------------------------------------------------
// 
// CND_NUMOFALLOBJECT_OLD
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.OI;
namespace RuntimeXNA.Conditions
{
	
	public class CND_NUMOFALLOBJECT_OLD:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			rhPtr.rhEvtProg.count_ObjectsFromType(COI.OBJ_SPR, - 1);
			return compareCondition(rhPtr, 0, rhPtr.rhEvtProg.evtNSelectedObjects);
		}
	}
}