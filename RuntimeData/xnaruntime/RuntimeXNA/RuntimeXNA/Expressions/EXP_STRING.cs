//----------------------------------------------------------------------------------
//
// STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_STRING:CExp
	{
		public System.String pString;
		
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceString(pString);
		}
	}
}