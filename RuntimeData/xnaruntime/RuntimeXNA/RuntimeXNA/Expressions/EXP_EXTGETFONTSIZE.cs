//----------------------------------------------------------------------------------
//
// GET FONT SIZE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTGETFONTSIZE:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			CFontInfo info = CRun.getObjectFont(pHo);
            rhPtr.getCurrentResult().forceInt(info.lfHeight);
		}
	}
}