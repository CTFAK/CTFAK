// ------------------------------------------------------------------------------
// 
// COMPARE TO FIXED VALUE
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTCMPVARFIXED:CCnd, IEvaExpObject
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
			int fixed_Renamed = (hoPtr.hoCreationId << 16) | (((int) hoPtr.hoNumber) & 0xFFFF);
			return CRun.compareTer(fixed_Renamed, value_Renamed, comp);
		}
	}
}