// -----------------------------------------------------------------------------
//
// PLAY AND LOOP CHANNEL
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_PLAYLOOPCHANNEL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_SAMPLE p = (PARAM_SAMPLE) evtParams[0];
			bool bPrio = p.sndFlags != 0;
			int channel = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			int nLoops = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[2]);
			rhPtr.rhApp.soundPlayer.play(p.sndHandle, nLoops, channel - 1, bPrio);
		}
	}
}