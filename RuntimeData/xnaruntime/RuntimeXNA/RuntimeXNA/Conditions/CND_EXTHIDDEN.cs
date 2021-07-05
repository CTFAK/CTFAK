// ------------------------------------------------------------------------------
// 
// CACHE?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTHIDDEN:CCnd, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return evaObject(rhPtr, this);
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaObject(rhPtr, this);
		}
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			if ((hoPtr.ros.rsFlags & CRSpr.RSFLAG_HIDDEN) != 0)
				return true;
			return false;
		}
	}
}