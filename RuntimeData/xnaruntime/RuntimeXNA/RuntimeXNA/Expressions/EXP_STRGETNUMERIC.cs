//----------------------------------------------------------------------------------
//
// VALEUR NUMERIQUE DE LA CHAINE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_STRGETNUMERIC:CExpOi
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
			if (pText.rsTextBuffer != null)
			{
				CFuncVal val = new CFuncVal();
				switch (val.parse(pText.rsTextBuffer))
				{					
					case 0:
                        rhPtr.getCurrentResult().forceInt(val.intValue);
						return ;
					
					case 1:
                        rhPtr.getCurrentResult().forceDouble(val.doubleValue);
						return ;
					}
			}
            rhPtr.getCurrentResult().forceInt(0);
		}
	}
}