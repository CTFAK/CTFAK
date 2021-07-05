//----------------------------------------------------------------------------------
//
// VAL
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_VAL:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			
			System.String s = rhPtr.get_ExpressionString();
			CFuncVal val = new CFuncVal();
			switch (val.parse(s))
			{
				
				case 0:
                    rhPtr.getCurrentResult().forceInt(val.intValue);
					return ;
				
				case 1:
                    rhPtr.getCurrentResult().forceDouble(val.doubleValue);
					return ;
				}
		}
	}
}