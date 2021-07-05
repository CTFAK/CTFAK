// ------------------------------------------------------------------------------
// 
// COMPARE TO DEC
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTCMPDEC:CCnd, IEvaExpObject
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
			return CRun.compareTer(hoPtr.rom.rmMovement.rmDec, value_Renamed, comp);
		}
	}
}