// ------------------------------------------------------------------------------
// 
// INTERFACE CNDEVAL pour l'exploration des objets d'une condition
// 
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;

namespace RuntimeXNA.Conditions
{
    public interface ICndEval
    {
        bool eval(CRun rhPtr, CObject hoPtr);
    }
}
