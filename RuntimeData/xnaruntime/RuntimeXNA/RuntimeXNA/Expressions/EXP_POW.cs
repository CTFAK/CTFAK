//----------------------------------------------------------------------------------
//
// OPERATEUR MODULO
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_POW:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().pow(rhPtr.getNextResult());
		}
	}
}