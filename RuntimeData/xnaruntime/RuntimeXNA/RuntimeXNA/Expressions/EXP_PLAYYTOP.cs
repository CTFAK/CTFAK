//----------------------------------------------------------------------------------
//
// LIMITE EN HAUT TERRAIN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLAYYTOP:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int r = rhPtr.rhWindowY;
			if ((rhPtr.rh3Scrolling & CRun.RH3SCROLLING_SCROLL) != 0)
				r = rhPtr.rh3DisplayY;
			if (r < 0)
				r = 0;

            rhPtr.getCurrentResult().forceInt(r);
		}
	}
}