//----------------------------------------------------------------------------------
//
// COULEUR POINT IMAGE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETRGBAT:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject hoPtr = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			rhPtr.rh4CurToken++;
			if (hoPtr == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			int x = rhPtr.get_ExpressionInt();
			rhPtr.rh4CurToken++;
			int y = rhPtr.get_ExpressionInt();
			
			int rgb = 0;

            if (hoPtr.roc.rcImage != - 1)
			{
                CImage image = rhPtr.rhApp.imageBank.getImageFromHandle(hoPtr.roc.rcImage);
                if (x > 0 && x < image.width && y > 0 && y < image.height)
                {
                    int[] pixels = new int[image.width * image.height];
                    image.image.GetData(pixels);
                    rgb = pixels[y * image.width + x] & 0xFFFFFF;
                    rgb = CServices.swapRGB(rgb);
                }
			}
            rhPtr.getCurrentResult().forceInt(rgb);
		}
	}
}