//----------------------------------------------------------------------------------
//
// GLOBAL STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_STRINGGLONAMED:CExp
	{
		internal short number;
		
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceString(rhPtr.rhApp.getGlobalStringAt(number));
		}
	}
}