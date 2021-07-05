//----------------------------------------------------------------------------------
//
// RIGHT$
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_RIGHT:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			System.String pString = rhPtr.get_ExpressionString();
			rhPtr.rh4CurToken++;
			int pos = rhPtr.get_ExpressionInt();
			
			if (pos < 0)
				pos = 0;
			if (pos > pString.Length)
				pos = pString.Length;
            rhPtr.getCurrentResult().forceString(pString.Substring(pString.Length - pos, (pString.Length) - (pString.Length - pos)));
		}
	}
}