// -----------------------------------------------------------------------------
//
// UNLOCK CHANNEL
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_UNLOCKCHANNEL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int channel = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			//rhPtr.rhApp.soundPlayer.unlockChannel(channel - 1);
		}
	}
}