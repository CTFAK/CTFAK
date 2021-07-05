//----------------------------------------------------------------------------------
//
// TIMER MINUTES
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_TIMMINITS:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int s = (int) (rhPtr.rhTimer / 60000);
            rhPtr.getCurrentResult().forceInt(s % 60);
		}
	}
}