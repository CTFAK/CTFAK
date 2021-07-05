//----------------------------------------------------------------------------------
//
// LARGEUR TERRAIN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLAYWIDTH:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhFrame.leWidth);
		}
	}
}