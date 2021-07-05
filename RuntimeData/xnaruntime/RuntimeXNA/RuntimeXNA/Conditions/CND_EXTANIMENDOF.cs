// ------------------------------------------------------------------------------
// 
// END OF ANIMATION
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTANIMENDOF:CCnd, IEvaExpObject, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			int ani;
			if (evtParams[0].code == 10)
			// PARAM_ANIMATION)
			{
				ani = ((PARAM_SHORT) evtParams[0]).value; //; Comparee au parametre animation
			}
			else
			{
				ani = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			}
			
			if (ani != rhPtr.rhEvtProg.rhCurParam0)
				return false; // L'animation courante
			rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr); // Stocke l'objet courant
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			if (evtParams[0].code == 10)
			// PARAM_ANIMATION)					// Le parametre direction?
				return evaObject(rhPtr, this);
			
			return evaExpObject(rhPtr, this); // Une expression
		}
		public virtual bool evaExpRoutine(CObject hoPtr, int value_Renamed, short comp)
		{
			if (value_Renamed != hoPtr.roa.raAnimOn)
				return false;
			if (hoPtr.roa.raAnimNumberOfFrame == 0)
				return true;
			return false;
		}
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			short anim = ((PARAM_SHORT) evtParams[0]).value;
			if (anim != hoPtr.roa.raAnimOn)
				return false;
			if (hoPtr.roa.raAnimNumberOfFrame == 0)
				return true;
			return false;
		}
	}
}