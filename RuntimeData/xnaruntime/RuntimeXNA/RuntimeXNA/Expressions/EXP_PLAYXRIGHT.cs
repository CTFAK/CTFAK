//----------------------------------------------------------------------------------
//
// LIMITE A DROITE TERRAIN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLAYXRIGHT:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int r = rhPtr.rhWindowX;
			if ((rhPtr.rh3Scrolling & CRun.RH3SCROLLING_SCROLL) != 0)
				r = rhPtr.rh3DisplayX;
			r += rhPtr.rh3WindowSx;
			if (r > rhPtr.rhLevelSx)
				r = rhPtr.rhLevelSx;

            rhPtr.getCurrentResult().forceInt(r);
		}
	}
}