// -----------------------------------------------------------------------------
//
// RESUME ALL CHANNELS
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_RESUMEALLCHANNELS:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			rhPtr.rhApp.soundPlayer.resume();
		}
	}
}