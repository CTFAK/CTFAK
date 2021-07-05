//----------------------------------------------------------------------------------
//
// NEW LINE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_NEWLINE:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			System.String s = System.Environment.NewLine;
            rhPtr.getCurrentResult().forceString(s);
		}
	}
}