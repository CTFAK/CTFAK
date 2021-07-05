//----------------------------------------------------------------------------------
//
// OPERATEUR XOR
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_XOR:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().xorLog(rhPtr.getNextResult());
		}
	}
}