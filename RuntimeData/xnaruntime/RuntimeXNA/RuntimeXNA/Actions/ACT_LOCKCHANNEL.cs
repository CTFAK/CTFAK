// -----------------------------------------------------------------------------
//
// LOCK CHANNEL
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_LOCKCHANNEL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int channel = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			//rhPtr.rhApp.soundPlayer.lockChannel(channel - 1);
		}
	}
}