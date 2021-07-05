//----------------------------------------------------------------------------------
//
// SAMPLE MAIN VOLUME
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETSAMPLEMAINVOL:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getMainVolume());
		}
	}
}