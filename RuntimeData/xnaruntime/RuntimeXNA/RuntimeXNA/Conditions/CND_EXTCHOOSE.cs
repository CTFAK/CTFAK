// ------------------------------------------------------------------------------
// 
// PICK AN OBJECT
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{	
	public class CND_EXTCHOOSE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			rhPtr.rhEvtProg.count_ObjectsFromOiList(evtOiList, - 1); // Combien d'objets?
			if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
				return false;
			short rnd = rhPtr.random((short) rhPtr.rhEvtProg.evtNSelectedObjects);
			CObject pHo = rhPtr.rhEvtProg.count_ObjectsFromOiList(evtOiList, rnd); // Va choisir
			rhPtr.rhEvtProg.evt_ForceOneObject(pHo);
			return true;
		}
	}
}