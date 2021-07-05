// ------------------------------------------------------------------------------
// 
// CND_CHOOSEALL_OLD
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.OI;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEALL_OLD:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			rhPtr.rhEvtProg.count_ObjectsFromType(COI.OBJ_SPR, - 1); //; Trouve l'objet a choisir
			if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
				return false;
			int rnd = rhPtr.random((short) rhPtr.rhEvtProg.evtNSelectedObjects);
			CObject pHo = rhPtr.rhEvtProg.count_ObjectsFromType(COI.OBJ_SPR, rnd);
			rhPtr.rhEvtProg.evt_DeleteCurrentType(COI.OBJ_SPR); // Vire tout
			rhPtr.rhEvtProg.evt_AddCurrentObject(pHo); // Met le seul
			return true;
		}
	}
}