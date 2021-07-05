//----------------------------------------------------------------------------------
//
// OPERATEUR NOT
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_NOT:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			int value_Renamed = rhPtr.get_ExpressionInt();
            rhPtr.getCurrentResult().forceInt(value_Renamed ^ unchecked((int)0xFFFFFFFF));
		}
	}
}