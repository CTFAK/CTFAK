//----------------------------------------------------------------------------------
//
// SAMPLE FREQUENCY
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{

    public class EXP_GETSAMPLEFREQ : CExp
    {
        public override void evaluate(CRun rhPtr)
        {
            rhPtr.rh4CurToken++;
            string sample = rhPtr.get_ExpressionString();
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.soundPlayer.getFrequencySample(sample));
        }
    }
}