// -----------------------------------------------------------------------------
//
// STOP SAMPLE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STOPSAMPLE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			rhPtr.rhApp.soundPlayer.stopAllSounds();
		}
	}
}