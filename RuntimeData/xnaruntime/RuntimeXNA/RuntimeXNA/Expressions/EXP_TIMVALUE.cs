//----------------------------------------------------------------------------------
//
// TIMER 1/1000
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_TIMVALUE:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt((int)rhPtr.rhTimer);
		}
	}
}