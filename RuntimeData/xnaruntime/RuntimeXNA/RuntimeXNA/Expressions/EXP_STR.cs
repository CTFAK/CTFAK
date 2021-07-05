//----------------------------------------------------------------------------------
//
// STR$
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_STR:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			CValue pValue = rhPtr.getExpression();
			System.String s = "";
			switch (pValue.getType())
			{
				
				case 0:  // TYPE_LONG:
					s =  pValue.getInt().ToString();
					break;
				
				case 1:  // TYPE_DOUBLE:
                    s = pValue.getDouble().ToString();
					break;
			}
            rhPtr.getCurrentResult().forceString(s);
		}
	}
}