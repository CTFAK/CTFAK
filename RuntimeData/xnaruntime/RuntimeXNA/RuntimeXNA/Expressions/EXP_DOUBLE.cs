//----------------------------------------------------------------------------------
//
// VALEUR A VIRGULE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_DOUBLE:CExp
	{
		internal double value;
		
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceDouble(value);
		}
	}
}