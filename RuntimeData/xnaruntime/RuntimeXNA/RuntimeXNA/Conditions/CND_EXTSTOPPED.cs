// ------------------------------------------------------------------------------
// 
// IS STOPPED?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTSTOPPED:CCnd, IEvaObject
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
			if (hoPtr.roc.rcSpeed == 0)
				return negaTRUE();
			return negaFALSE();
		}
	}
}