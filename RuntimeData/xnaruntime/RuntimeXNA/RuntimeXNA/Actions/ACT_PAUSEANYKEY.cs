// -----------------------------------------------------------------------------
//
// PAUSE ANY TOUCHE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_PAUSEANYKEY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			rhPtr.rh4PauseKey = 0;
            rhPtr.bAnyKeyDown = true;
            rhPtr.bCheckResume = true;
            rhPtr.pause();
		}
	}
}