//----------------------------------------------------------------------------------
//
// STR$
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{

    public class EXP_PATH : CExp
    {
        public override void evaluate(CRun rhPtr)
        {
            rhPtr.getCurrentResult().forceString("");
        }
    }
}