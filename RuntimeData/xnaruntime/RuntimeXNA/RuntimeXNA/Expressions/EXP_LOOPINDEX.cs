//----------------------------------------------------------------------------------
//
// LOOP INDEX
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Actions;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_LOOPINDEX:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			System.String pName = rhPtr.get_ExpressionString();
			
			CLoop pLoop;
			int n;
			for (n = 0; n < rhPtr.rh4FastLoops.size(); n++)
			{
				pLoop = (CLoop)rhPtr.rh4FastLoops.get(n);
                if (System.String.Compare(pLoop.name, pName, StringComparison.OrdinalIgnoreCase) == 0)
				{
                    rhPtr.getCurrentResult().forceInt(pLoop.index);
					return ;
				}
			}
            rhPtr.getCurrentResult().forceInt(0);
		}
	}
}