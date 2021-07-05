// ------------------------------------------------------------------------------
// 
// INTERFACE IEVAEXPOBJECT pour l'exploration des objets d'une condition
// 
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Objects;

namespace RuntimeXNA.Conditions
{
    public interface IEvaExpObject
    {
        bool evaExpRoutine(CObject hoPtr, int v, short comp);
    }
}
