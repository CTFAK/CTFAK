//----------------------------------------------------------------------------------
//
// VARIABLE GLOBALE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_VARGLONAMED:CExp
	{
		internal short number;
		
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceValue(rhPtr.rhApp.getGlobalValueAt(number));
		}
	}
}