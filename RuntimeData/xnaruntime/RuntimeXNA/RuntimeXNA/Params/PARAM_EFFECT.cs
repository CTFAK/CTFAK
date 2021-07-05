//----------------------------------------------------------------------------------
//
// PARAM_STRING : une chaine
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Params
{
    public class PARAM_EFFECT : CParam
    {
        public System.String pEffect;

        public override void load(CRunApp app)
        {
            pEffect = app.file.readAString();
        }
    }
}