//----------------------------------------------------------------------------------
//
// FLOAT TO STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_ATAN2:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			double value1 = rhPtr.get_ExpressionDouble();
			rhPtr.rh4CurToken++;
			double value2 = rhPtr.get_ExpressionDouble();

            rhPtr.getCurrentResult().forceDouble(System.Math.Atan2(value1, value2)*180/Math.PI);
		}
	}
}