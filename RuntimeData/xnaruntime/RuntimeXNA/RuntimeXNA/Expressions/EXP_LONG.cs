//----------------------------------------------------------------------------------
//
// LONG
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_LONG:CExp
	{
		internal int value;
		
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(value);
		}
	}
}