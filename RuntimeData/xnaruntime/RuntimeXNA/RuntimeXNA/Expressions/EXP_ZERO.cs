//----------------------------------------------------------------------------------
//
// ZERO
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions
{
    class EXP_ZERO:CExp
    {
        public override void evaluate(CRun rhPtr)
        {
            rhPtr.getCurrentResult().forceInt(0);
        }
    }
}
