// -----------------------------------------------------------------------------
//
// PLAY AND LOOP SAMPLE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_PLAYLOOPSAMPLE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_SAMPLE p = (PARAM_SAMPLE) evtParams[0];
			int nLoops = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			bool bPrio = p.sndFlags != 0;
			rhPtr.rhApp.soundPlayer.play(p.sndHandle, nLoops, - 1, bPrio);
		}
	}
}