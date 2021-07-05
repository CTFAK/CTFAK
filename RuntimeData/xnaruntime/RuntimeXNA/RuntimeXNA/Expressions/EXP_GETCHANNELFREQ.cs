//----------------------------------------------------------------------------------
//
// CHANNEL FREQUENCY
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{

    public class EXP_GETCHANNELFREQ : CExp
    {
        public override void evaluate(CRun rhPtr)
        {
            rhPtr.rh4CurToken++;
            int channel = rhPtr.get_ExpressionInt();
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getFrequencyChannel(channel - 1));
        }
    }
}