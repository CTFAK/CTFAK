//----------------------------------------------------------------------------------
//
// GET GREEN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETGREEN:CExp
	{
		
		public EXP_GETGREEN()
		{
		}
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			int rgb = rhPtr.get_ExpressionInt();
            rhPtr.getCurrentResult().forceInt((rgb>>8) & 255);
		}
	}
}