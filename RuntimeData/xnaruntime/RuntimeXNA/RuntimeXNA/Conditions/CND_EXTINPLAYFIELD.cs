// ------------------------------------------------------------------------------
// 
// IN PLAYFIELD?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
using RuntimeXNA.Movements;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTINPLAYFIELD:CCnd, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			PARAM_SHORT evpPtr = (PARAM_SHORT) evtParams[0];
			if ((evpPtr.value & ((short) rhPtr.rhEvtProg.rhCurParam0)) == 0)
			//; Prend le deuxieme parametre (directions)
				return false;
			
			if (compute_NoRepeat(hoPtr))
			{
				rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr); // Stocke l'objet courant
				return true;
			}
			
			// Si une action STOP dans le groupe, il faut la faire!!!
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			CEventGroup pEvg = rhPtr.rhEvtProg.rhEventGroup;
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_STOPINGROUP) == 0)
				return false;
			rhPtr.rhEvtProg.rh3DoStop = true;
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaObject(rhPtr, this);
		}
		public virtual bool evaObjectRoutine(CObject pHo)
		{
			if ((pHo.rom.rmEventFlags & CRMvt.EF_GOESOUTPLAYFIELD) != 0)
				return negaTRUE();
			return negaFALSE();
		}
	}
}