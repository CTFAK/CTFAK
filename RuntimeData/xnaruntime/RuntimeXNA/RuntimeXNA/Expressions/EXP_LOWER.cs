//----------------------------------------------------------------------------------
//
// LOWER
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_LOWER:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			System.String pString = rhPtr.get_ExpressionString();
            rhPtr.getCurrentResult().forceString(pString.ToLower());
		}
	}
}