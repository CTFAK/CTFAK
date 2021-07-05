//----------------------------------------------------------------------------------
//
// HEXADECIMAL
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_HEX:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			int a = rhPtr.get_ExpressionInt();
			System.String s = a.ToString("X");
            rhPtr.getCurrentResult().forceString(s);
		}
	}
}