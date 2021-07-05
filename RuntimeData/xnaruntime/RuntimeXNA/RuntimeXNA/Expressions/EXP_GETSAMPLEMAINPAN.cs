//----------------------------------------------------------------------------------
//
// SAMPLE MAIN PAN
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETSAMPLEMAINPAN:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getMainPan());
		}
	}
}