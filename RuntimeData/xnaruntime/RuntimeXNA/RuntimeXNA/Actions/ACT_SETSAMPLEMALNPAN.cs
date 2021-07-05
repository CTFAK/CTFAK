// -----------------------------------------------------------------------------
//
// SET SAMPLE MAIN PAN
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETSAMPLEMALNPAN:CAct
	{
		public override void  execute(CRun rhPtr)
		{
            int pan = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            rhPtr.rhApp.soundPlayer.setMainPan(pan);
        }
	}
}