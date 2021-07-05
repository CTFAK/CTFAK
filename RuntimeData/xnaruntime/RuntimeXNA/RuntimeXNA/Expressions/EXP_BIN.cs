//----------------------------------------------------------------------------------
//
// BINARY
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_BIN:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			int a = rhPtr.get_ExpressionInt();
			System.String s = "0b" + System.Convert.ToString(a, 2);
            rhPtr.getCurrentResult().forceString(s);
		}
	}
}