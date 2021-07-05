//----------------------------------------------------------------------------------
//
// OPERATEUR PLUS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLUS:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().add(rhPtr.getNextResult());
		}
	}
}