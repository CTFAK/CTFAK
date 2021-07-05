//----------------------------------------------------------------------------------
//
// FRAME BACK COLOR
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETFRAMEBKDCOLOR:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(CServices.swapRGB(rhPtr.rhFrame.leBackground));
		}
	}
}