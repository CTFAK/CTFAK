// ------------------------------------------------------------------------------
// 
// PICK ALL OBJECTS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEALL:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			rhPtr.rhEvtProg.count_ObjectsFromType((short) 0, - 1); //; Trouve l'objet a choisir
			if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
				return false;
			int rnd = rhPtr.random((short) rhPtr.rhEvtProg.evtNSelectedObjects);
			CObject pHo = rhPtr.rhEvtProg.count_ObjectsFromType((short) 0, rnd);
			rhPtr.rhEvtProg.evt_DeleteCurrent(); // Vire tout
			rhPtr.rhEvtProg.evt_AddCurrentObject(pHo); // Met le seul
			return true;
		}
	}
}