//----------------------------------------------------------------------------------
//
// LIMITE A GAUCHE TERRAIN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLAYXLEFT:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int r = rhPtr.rhWindowX;
			if ((rhPtr.rh3Scrolling & CRun.RH3SCROLLING_SCROLL) != 0)
				r = rhPtr.rh3DisplayX;
			if (r < 0)
				r = 0;

            rhPtr.getCurrentResult().forceInt(r);
		}
	}
}