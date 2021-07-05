//----------------------------------------------------------------------------------
//
// GLOBAL STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_CCAGETGLOBALSTRING:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			rhPtr.rh4CurToken++; // Saute le token
			int num = rhPtr.get_ExpressionInt(); // Le numero du flag
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceString("");
				return ;
			}
			//rhPtr.CurrentResult.forceString(((CCCA) pHo).getGlobalString(num));
		}
	}
}