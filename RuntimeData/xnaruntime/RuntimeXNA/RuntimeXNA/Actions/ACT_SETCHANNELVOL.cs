// -----------------------------------------------------------------------------
//
// SET CHANNEL VOLUME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETCHANNELVOL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
            int channel = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            int volume = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
            rhPtr.rhApp.soundPlayer.setVolumeChannel(channel - 1, volume);
        }
	}
}