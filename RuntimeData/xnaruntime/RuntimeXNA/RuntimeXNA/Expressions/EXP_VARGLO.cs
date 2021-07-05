//----------------------------------------------------------------------------------
//
// VARIABLE GLOBALE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_VARGLO:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++; // Saute le token
			
			int num = (rhPtr.get_ExpressionInt() - 1);
			rhPtr.getCurrentResult().forceValue(rhPtr.rhApp.getGlobalValueAt(num));
		}
	}
}