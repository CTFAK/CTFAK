// ------------------------------------------------------------------------------
// 
// IS FLAG SET?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTFLAGSET:CCnd, IEvaExpObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return evaExpObject(rhPtr, this);
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaExpObject(rhPtr, this);
		}
		public virtual bool evaExpRoutine(CObject hoPtr, int value_Renamed, short comp)
		{
			value_Renamed &= 31;
			if (hoPtr.rov != null)
			{
				if ((hoPtr.rov.rvValueFlags & (1 << value_Renamed)) != 0)
					return true;
			}
			return false;
		}
	}
}