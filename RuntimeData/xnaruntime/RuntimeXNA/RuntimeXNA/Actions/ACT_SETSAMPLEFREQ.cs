// -----------------------------------------------------------------------------
//
// SET SAMPLE VOLUME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{

    public class ACT_SETSAMPLEFREQ : CAct
    {
        public override void execute(CRun rhPtr)
        {
            PARAM_SAMPLE p = (PARAM_SAMPLE)evtParams[0];
            int frequency= rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
            rhPtr.rhApp.soundPlayer.setFrequencySample(p.sndHandle, frequency);
        }
    }
}