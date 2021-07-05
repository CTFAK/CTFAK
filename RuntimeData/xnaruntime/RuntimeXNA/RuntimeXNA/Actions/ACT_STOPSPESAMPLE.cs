// -----------------------------------------------------------------------------
//
// STOP SPECIFIC SAMPLE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STOPSPESAMPLE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_SAMPLE p = (PARAM_SAMPLE) evtParams[0];
			rhPtr.rhApp.soundPlayer.stopSample(p.sndHandle);
		}
	}
}