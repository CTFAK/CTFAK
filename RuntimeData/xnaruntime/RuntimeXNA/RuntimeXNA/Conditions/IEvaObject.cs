// ------------------------------------------------------------------------------
// 
// IEVAOBJECT : interface pour les conditions
// 
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Objects;

namespace RuntimeXNA.Conditions
{
    public interface IEvaObject
    {
        bool evaObjectRoutine(CObject hoPtr);
    }
}
