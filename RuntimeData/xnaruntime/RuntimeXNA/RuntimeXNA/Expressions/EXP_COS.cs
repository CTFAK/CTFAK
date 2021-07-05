//----------------------------------------------------------------------------------
//
// COSINUS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_COS:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			double value = rhPtr.get_ExpressionDouble();
			double temp = System.Math.Cos(value / 57.295779513082320876798154814105);
            rhPtr.getCurrentResult().forceDouble(temp);
		}
	}
}