// -----------------------------------------------------------------------------
//
// SET SAMPLE MAIN VOLUME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETSAMPLEMAINVOL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
            int volume = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            rhPtr.rhApp.soundPlayer.setMainVolume(volume);
        }
	}
}