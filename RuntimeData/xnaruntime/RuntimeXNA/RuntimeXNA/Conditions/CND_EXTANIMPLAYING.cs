// ------------------------------------------------------------------------------
// 
// IS ANIMATION PLAYING?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTANIMPLAYING:CCnd, IEvaExpObject, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
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
				return negaFALSE();
			if (hoPtr.roa.raAnimNumberOfFrame != 0)
				return negaTRUE();
			return negaFALSE();
		}
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			short anim = ((PARAM_SHORT) evtParams[0]).value;
			if (anim != hoPtr.roa.raAnimOn)
				return negaFALSE();
			if (hoPtr.roa.raAnimNumberOfFrame != 0)
				return negaTRUE();
			return negaFALSE();
		}
	}
}