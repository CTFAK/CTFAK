//----------------------------------------------------------------------------------
//
// EMPTY STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions
{
    class EXP_EMPTY:CExp
    {
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceString("");
		}
    }
}
