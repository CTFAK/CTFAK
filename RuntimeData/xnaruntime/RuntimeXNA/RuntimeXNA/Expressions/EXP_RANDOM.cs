//----------------------------------------------------------------------------------
//
// RANDOM
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_RANDOM:CExp
	{
		
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++; // Saute le token
			int num = rhPtr.get_ExpressionInt(); // Le parametre
            rhPtr.getCurrentResult().forceInt(rhPtr.random((short)num)); // Genere le chiffre
		}
	}
}