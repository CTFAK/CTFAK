//----------------------------------------------------------------------------------
//
// OPERATEUR MOINS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_MINUS:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			if (rhPtr.bOperande)
			{
				rhPtr.rh4CurToken++;
				rhPtr.rh4Tokens[rhPtr.rh4CurToken].evaluate(rhPtr);
                rhPtr.getCurrentResult().negate();
			}
			else
			{
                rhPtr.getCurrentResult().sub(rhPtr.getNextResult());
			}
		}
	}
}