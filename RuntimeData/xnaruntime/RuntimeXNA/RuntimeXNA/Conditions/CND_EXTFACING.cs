// ------------------------------------------------------------------------------
// 
// FACING A DIRECTION?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTFACING:CCnd, IEvaExpObject, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			if (evtParams[0].code == 29)
			// PARAM_NEWDIRECTION)					// Le parametre direction?
				return evaObject(rhPtr, this);
			return evaExpObject(rhPtr, this); // Une expression
		}
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			int mask = ((PARAM_INT) evtParams[0]).value_Renamed;
			int dir;
			for (dir = 0; dir < 32; dir++)
			{
				if (((1 << dir) & mask) != 0)
				{
					if (hoPtr.roc.rcDir == dir)
					{
						return negaTRUE();
					}
				}
			}
			return negaFALSE();
		}
		public virtual bool evaExpRoutine(CObject hoPtr, int value_Renamed, short comp)
		{
			value_Renamed &= 31;
			if (hoPtr.roc.rcDir == value_Renamed)
			{
				return negaTRUE();
			}
			return negaFALSE();
		}
	}
}