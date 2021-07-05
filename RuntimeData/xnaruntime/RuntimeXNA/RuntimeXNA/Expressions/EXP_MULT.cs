//----------------------------------------------------------------------------------
//
// OPERATEUR MULTIPLY
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_MULT:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().mul(rhPtr.getNextResult());
		}
	}
}