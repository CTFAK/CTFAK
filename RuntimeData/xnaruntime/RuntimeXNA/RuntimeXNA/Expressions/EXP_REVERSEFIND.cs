//----------------------------------------------------------------------------------
//
// REVERSE FIND
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_REVERSEFIND:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			System.String pMainString = rhPtr.get_ExpressionString();
			rhPtr.rh4CurToken++;
			System.String pSubString = rhPtr.get_ExpressionString();
			rhPtr.rh4CurToken++;
			int firstChar = rhPtr.get_ExpressionInt();
			
			if (firstChar > pMainString.Length)
			{
				firstChar = pMainString.Length;
			}
			
			int oldPos;
			int pos = - 1;
			while (true)
			{
				oldPos = pos;
				//UPGRADE_WARNING: Method 'java.lang.String.indexOf' was converted to 'System.String.IndexOf' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
				int pFound = pMainString.IndexOf(pSubString, pos + 1);
				if (pFound == - 1)
					break;
				pos = pFound;
				if (pos > firstChar)
					break;
			}
            rhPtr.getCurrentResult().forceInt(oldPos);
		}
	}
}