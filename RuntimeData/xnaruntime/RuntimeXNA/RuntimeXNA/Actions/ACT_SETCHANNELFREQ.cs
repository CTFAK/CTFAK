// -----------------------------------------------------------------------------
//
// SET CHANNEL FREQUENCY
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{

    public class ACT_SETCHANNELFREQ : CAct
    {
        public override void execute(CRun rhPtr)
        {
            int channel = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            int frequency = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
            rhPtr.rhApp.soundPlayer.setFrequencyChannel(channel - 1, frequency);
        }
    }
}