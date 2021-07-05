//----------------------------------------------------------------------------------
//
// GET FONT NAME
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTGETFONTNAME:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceString("");
				return ;
			}
			CFontInfo info = CRun.getObjectFont(pHo);
            rhPtr.getCurrentResult().forceString(info.lfFaceName);
		}
	}
}