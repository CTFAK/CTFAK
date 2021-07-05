//----------------------------------------------------------------------------------
//
// NUMERO DU PARAGRAPHE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_STRNUMBER:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			CText pText = (CText) pHo;
            rhPtr.getCurrentResult().forceInt(pText.rsMini + 1);
		}
	}
}