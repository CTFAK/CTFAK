//----------------------------------------------------------------------------------
//
// GET FONT COLOR
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTGETFONTCOLOR:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			int rgb = CRun.getObjectTextColor(pHo);
			rgb = CServices.swapRGB(rgb);
            rhPtr.getCurrentResult().forceInt(rgb);
		}
	}
}