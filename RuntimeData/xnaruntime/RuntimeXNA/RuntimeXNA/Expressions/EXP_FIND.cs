//----------------------------------------------------------------------------------
//
// FIND
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_FIND:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			System.String pMainString = rhPtr.get_ExpressionString();
			rhPtr.rh4CurToken++;
			System.String pSubString = rhPtr.get_ExpressionString();
			rhPtr.rh4CurToken++;
			int firstChar = rhPtr.get_ExpressionInt();
			
			if (firstChar >= pMainString.Length)
			{
                rhPtr.getCurrentResult().forceInt(-1);
				return ;
			}
            rhPtr.getCurrentResult().forceInt(pMainString.IndexOf(pSubString, firstChar));
		}
	}
}