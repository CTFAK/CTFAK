//----------------------------------------------------------------------------------
//
// TOTAL NUMBER OF OBJECTS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_CRENUMBERALL:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhNObjects);
		}
	}
}