// ------------------------------------------------------------------------------
// 
// OVERLAPPING A BACKGROUND?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTISCOLBACK:CCnd, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		
		public override bool eva2(CRun rhPtr)
		{
			return evaObject(rhPtr, this);
		}
		
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, 0, CColMask.CM_TEST_PLATFORM) != 0)
			{
				return negaTRUE();
			}
			return negaFALSE();
		}
	}
}