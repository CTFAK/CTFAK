//----------------------------------------------------------------------------------
//
// GET Y ACTION POINT
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Banks;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTYAP:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			int y = 0;
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo != null)
			{
				y = pHo.hoY;
				if (pHo.roa != null)
				{
                    CImage img = rhPtr.rhApp.imageBank.getImageInfoEx(pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY);
                    if (img != null)
					{
						y += img.yAP - img.ySpot;
					}
				}
			}
            rhPtr.getCurrentResult().forceInt(y);
		}
	}
}