// -----------------------------------------------------------------------------
//
// GOTO WINDOWED MODE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_WINDOWEDMODE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
            rhPtr.rhApp.setFullScreen(false);
        }
	}
}