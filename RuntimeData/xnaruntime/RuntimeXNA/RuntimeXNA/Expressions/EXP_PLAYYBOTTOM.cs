//----------------------------------------------------------------------------------
//
// LIMITE EN BAS TERRAIN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLAYYBOTTOM:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int r = rhPtr.rhWindowY;
			if ((rhPtr.rh3Scrolling & CRun.RH3SCROLLING_SCROLL) != 0)
				r = rhPtr.rh3DisplayY;
			r += rhPtr.rh3WindowSy;
			if (r > rhPtr.rhLevelSy)
				r = rhPtr.rhLevelSy;

            rhPtr.getCurrentResult().forceInt(r);
		}
	}
}