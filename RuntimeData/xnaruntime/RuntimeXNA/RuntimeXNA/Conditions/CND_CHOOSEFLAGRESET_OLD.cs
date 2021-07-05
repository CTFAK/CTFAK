// ------------------------------------------------------------------------------
// 
// CND_CHOOSEFLAGRESET_OLD
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEFLAGRESET_OLD:CCnd, IChooseValue
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaChooseValueOld(rhPtr, this);
		}
		public virtual bool evaluate(CObject pHo, int value_Renamed)
		{
			if (pHo.rov != null)
			{
				if ((pHo.rov.rvValueFlags & (1 << value_Renamed)) == 0)
					return true;
			}
			return false;
		}
	}
}