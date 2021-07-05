//----------------------------------------------------------------------------------
//
// TIMER 1/100
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_TIMCENT:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int c = (int) (rhPtr.rhTimer / 10);
            rhPtr.getCurrentResult().forceInt(c % 100);
		}
	}
}