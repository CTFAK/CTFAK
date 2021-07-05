// ------------------------------------------------------------------------------
// 
// CND_CHOOSEZONE_OLD
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.OI;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEZONE_OLD:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			PARAM_ZONE p = (PARAM_ZONE) evtParams[0];
			rhPtr.rhEvtProg.count_ZoneTypeObjects(p, - 1, COI.OBJ_SPR); // Compte le objets	
			if (rhPtr.rhEvtProg.evtNSelectedObjects == 0)
				return false;
			
			int rnd = (int) rhPtr.random((short) rhPtr.rhEvtProg.evtNSelectedObjects);
			CObject pHo = rhPtr.rhEvtProg.count_ZoneTypeObjects(p, rnd, COI.OBJ_SPR); // Pointe le bon objet
			rhPtr.rhEvtProg.evt_DeleteCurrent();
			rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
			return true;
		}
	}
}