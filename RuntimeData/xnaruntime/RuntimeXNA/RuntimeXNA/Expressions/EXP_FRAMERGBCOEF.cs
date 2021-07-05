//----------------------------------------------------------------------------------
//
// ZERO
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions
{
    class EXP_FRAMERGBCOEF:CExp
    {
        public override void evaluate(CRun rhPtr)
        {
            rhPtr.getCurrentResult().forceInt(0xFFFFFF);
        }
    }
}
