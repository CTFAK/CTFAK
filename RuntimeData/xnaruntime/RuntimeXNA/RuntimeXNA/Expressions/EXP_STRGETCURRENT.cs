//----------------------------------------------------------------------------------
//
// CHAINE COURANTE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_STRGETCURRENT:CExpOi
	{
		
		public EXP_STRGETCURRENT()
		{
		}
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceString("");
				return ;
			}
			CText pText = (CText) pHo;
			if (pText.rsTextBuffer != null)
                rhPtr.getCurrentResult().forceString(pText.rsTextBuffer);
			else
                rhPtr.getCurrentResult().forceString("");
		}
	}
}