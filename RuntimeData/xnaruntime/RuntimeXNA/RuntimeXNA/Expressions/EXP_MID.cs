//----------------------------------------------------------------------------------
//
// MID$
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_MID:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			System.String pString = rhPtr.get_ExpressionString();
			rhPtr.rh4CurToken++;
			int start = rhPtr.get_ExpressionInt();
			rhPtr.rh4CurToken++;
			int len = rhPtr.get_ExpressionInt();
			
			if (start < 0)
				start = 0;
			if (start > pString.Length)
				start = pString.Length;
			if (len < 0)
				len = 0;
			if (start + len > pString.Length)
				len = pString.Length - start;
            rhPtr.getCurrentResult().forceString(pString.Substring(start, (start + len) - (start)));
		}
	}
}