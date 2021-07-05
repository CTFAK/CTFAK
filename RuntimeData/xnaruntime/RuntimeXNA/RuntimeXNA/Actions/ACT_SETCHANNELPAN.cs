// -----------------------------------------------------------------------------
//
// SET CHANNEL PAN
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETCHANNELPAN:CAct
	{
		public override void  execute(CRun rhPtr)
		{
            int channel = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            int pan = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
            rhPtr.rhApp.soundPlayer.setPanChannel(channel - 1, pan);
        }
	}
}