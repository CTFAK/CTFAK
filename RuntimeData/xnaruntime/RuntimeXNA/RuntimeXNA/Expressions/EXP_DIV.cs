//----------------------------------------------------------------------------------
//
// OPERATEUR DIVIDE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_DIV:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().div(rhPtr.getNextResult());
		}
	}
}