// ------------------------------------------------------------------------------
// 
// INTERFACE POUR CHOOSEVALUE1
// 
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Objects;

namespace RuntimeXNA.Conditions
{
    public interface IChooseValue
    {
        bool evaluate(CObject pHo, int v);
    }
}
