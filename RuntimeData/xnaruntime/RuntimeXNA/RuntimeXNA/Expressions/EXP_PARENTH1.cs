//----------------------------------------------------------------------------------
//
// OUVERTURE PARENTHESE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PARENTH1:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			CValue value = rhPtr.getExpression();
            rhPtr.getCurrentResult().forceValue(value);
		}
	}
}