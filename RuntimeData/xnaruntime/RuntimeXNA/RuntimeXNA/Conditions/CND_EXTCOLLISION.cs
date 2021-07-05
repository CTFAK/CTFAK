// ------------------------------------------------------------------------------
// 
// COLLISION
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTCOLLISION:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject pHo)
		{
			CObject pHo1 = rhPtr.rhObjectList[rhPtr.rhEvtProg.rh1stObjectNumber];
			short oiEvent = evtOi;
			PARAM_OBJECT p = (PARAM_OBJECT) evtParams[0];
			short oiParam = p.oi;
			
			while (true)
			{
				if (oiEvent == pHo.hoOi)
				// Event== courant	
				{
					// 1er=courant
					if (oiParam == pHo1.hoOi)
						break;
					if (oiParam >= 0)
						return false; // Un qualifier?
					if (colGetList(rhPtr, p.oiList, pHo1.hoOi))
						break;
					return false;
				}
				if (oiParam == pHo.hoOi)
				// parametre== courant
				{
					// 2eme=courant
					if (oiEvent == pHo1.hoOi)
						break;
					if (oiEvent >= 0)
						return false;
					if (colGetList(rhPtr, evtOiList, pHo1.hoOi))
						break;
					return false;
				}
				if (oiEvent < 0)
				{
					// 1er=liste
					if (oiParam < 0)
					{
						// 1er=liste, 2eme=liste
						if (colGetList(rhPtr, evtOiList, pHo.hoOi))
						// Le courant fait-il partie de la liste 1
						{
							if (colGetList(rhPtr, p.oiList, pHo1.hoOi))
							//; Courant dans liste 1, collision dans liste 2?
								break;
							if (colGetList(rhPtr, p.oiList, pHo.hoOi) == false)
							//; Derniere chance, courant dans liste 2?
								return false;
							if (colGetList(rhPtr, evtOiList, pHo1.hoOi))
								break;
							return false;
						}
						else
						{
							if (colGetList(rhPtr, evtOiList, pHo1.hoOi))
							//; Courant dans liste 2, collision dans liste 1?
								break;
							return false;
						}
					}
					else
					{
						if (oiParam == pHo1.hoOi)
							break;
						return false;
					}
				}
				if (oiParam >= 0)
					return false;
				// 1er=oi, 2eme=qualif
				if (oiEvent != pHo1.hoOi)
					return false;
				break;
			}
			
			// Collision detectee, on ne veut pas de repeat
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			int id = (((int) pHo1.hoCreationId) << 16) | (((int) evtIdentifier) & 0x0000FFFF); //; Prend le numero de l'objet en collision
			if (compute_NoRepeatCol(id, pHo) == false)
			{
				// Si une action STOP dans le groupe, il faut la faire!!!
				if ((rhPtr.rhEvtProg.rhEventGroup.evgFlags & CEventGroup.EVGFLAGS_STOPINGROUP) == 0)
					return false;
				rhPtr.rhEvtProg.rh3DoStop = true;
			}
			id = (((int) pHo.hoCreationId) << 16) | (((int) evtIdentifier) & 0x0000FFFF); //; Prend le numero de l'objet en collision
			if (compute_NoRepeatCol(id, pHo1) == false)
			// Deja fait B et A?
			{
				// Si une action STOP dans le groupe, il faut la faire!!!
				if ((rhPtr.rhEvtProg.rhEventGroup.evgFlags & CEventGroup.EVGFLAGS_STOPINGROUP) == 0)
					return false;
				rhPtr.rhEvtProg.rh3DoStop = true;
			}
			
			// Stocke le deuxieme sprite dans la list courante
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
			rhPtr.rhEvtProg.evt_AddCurrentObject(pHo1);
			
			if (pHo1.rom.rmMovement.rmCollisionCount == rhPtr.rh3CollisionCount)
				pHo.rom.rmMovement.rmCollisionCount = rhPtr.rh3CollisionCount;
			else if (pHo.rom.rmMovement.rmCollisionCount == rhPtr.rh3CollisionCount)
				pHo1.rom.rmMovement.rmCollisionCount = rhPtr.rh3CollisionCount;
			
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			return isColliding(rhPtr);
		}
		
		// Procedure d'exploration d'un qualifier
		internal virtual bool colGetList(CRun rhPtr, short oiList, short lookFor)
		{
			if (oiList == - 1)
				return false;
			CQualToOiList qoil = rhPtr.rhEvtProg.qualToOiList[oiList & 0x7FFF];
			int index;
			for (index = 0; index < qoil.qoiList.Length; index += 2)
			{
				if (qoil.qoiList[index] == lookFor)
					return true;
			}
			return false;
		}
	}
}