//----------------------------------------------------------------------------------
//
// FRAME RATE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_FRAMERATE:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int n, total = 0;
			for (n = 0; n < CRun.MAX_FRAMERATE; n++)
				total += rhPtr.rh4FrameRateArray[n];
            if (total != 0)
            {
                rhPtr.getCurrentResult().forceInt((1000 * CRun.MAX_FRAMERATE) / total);
                return;
            }
            rhPtr.getCurrentResult().forceInt(0);
        }
	}
}