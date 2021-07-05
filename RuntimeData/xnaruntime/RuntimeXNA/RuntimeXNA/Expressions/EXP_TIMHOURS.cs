//----------------------------------------------------------------------------------
//
// TIMER HOURS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_TIMHOURS:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int s = (int) (rhPtr.rhTimer / 3600000);
            rhPtr.getCurrentResult().forceInt(s);
		}
	}
}