// -----------------------------------------------------------------------------
//
// SET SAMPLE PAN
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETSAMPLEPAN:CAct
	{
		public override void  execute(CRun rhPtr)
		{
            PARAM_SAMPLE p = (PARAM_SAMPLE)evtParams[0];
            int pan = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
            rhPtr.rhApp.soundPlayer.setPanSample(p.sndHandle, pan);			
		}
	}
}