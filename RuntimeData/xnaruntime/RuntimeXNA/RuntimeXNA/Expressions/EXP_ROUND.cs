//----------------------------------------------------------------------------------
//
// ROUND
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_ROUND:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			double value = rhPtr.get_ExpressionDouble();
			double v = System.Math.Floor(value);
			if (value - v > 0.5)
				v++;
            rhPtr.getCurrentResult().forceInt((int)v);
		}
	}
}