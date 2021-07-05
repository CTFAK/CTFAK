//----------------------------------------------------------------------------------
//
// SINUS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_SIN:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			double value_Renamed = rhPtr.get_ExpressionDouble();
			double temp = System.Math.Sin(value_Renamed / 57.295779513082320876798154814105);
            rhPtr.getCurrentResult().forceDouble(temp);
		}
	}
}