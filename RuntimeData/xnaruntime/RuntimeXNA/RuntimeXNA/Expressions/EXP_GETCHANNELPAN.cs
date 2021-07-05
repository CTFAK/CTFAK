//----------------------------------------------------------------------------------
//
// CHANNEL PAN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETCHANNELPAN:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.rh4CurToken++;
            int channel = rhPtr.get_ExpressionInt();
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getPanChannel(channel - 1));
        }
	}
}