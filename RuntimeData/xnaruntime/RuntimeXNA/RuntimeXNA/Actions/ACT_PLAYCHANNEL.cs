// -----------------------------------------------------------------------------
//
// PLAY CHANNEL
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_PLAYCHANNEL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_SAMPLE p = (PARAM_SAMPLE) evtParams[0];
			bool bPrio = p.sndFlags != 0;
			int channel = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			rhPtr.rhApp.soundPlayer.play(p.sndHandle, 1, channel, bPrio);
		}
	}
}