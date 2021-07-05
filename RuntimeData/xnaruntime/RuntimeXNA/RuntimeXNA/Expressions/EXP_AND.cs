//----------------------------------------------------------------------------------
//
// OPERATEUR AND
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_AND:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().andLog(rhPtr.getNextResult());
		}
	}
}