// ------------------------------------------------------------------------------
// 
// IS BOLD?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTISBOLD:CCnd, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return evaObject(rhPtr, this);
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaObject(rhPtr, this);
		}
		public virtual bool evaObjectRoutine(CObject pHo)
		{
			CFontInfo info = CRun.getObjectFont(pHo);
			if (info.lfWeight >= 400)
				return true;
			return false;
		}
	}
}