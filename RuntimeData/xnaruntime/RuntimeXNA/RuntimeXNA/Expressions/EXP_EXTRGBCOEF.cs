//----------------------------------------------------------------------------------
//
// RGB COEF
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions
{
    class EXP_EXTRGBCOEF : CExpOi
    {
        public override void evaluate(CRun rhPtr)
        {
            CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
            if (pHo == null || pHo.ros==null)
            {
                rhPtr.getCurrentResult().forceInt(0);
                return;
            }

            int effect = pHo.ros.rsEffect;
            int effectParam = pHo.ros.rsEffectParam;
            int rgb = 0;
            int rgbaCoeff = effectParam;

            if ((effect & CSpriteGen.BOP_MASK) == CSpriteGen.BOP_EFFECTEX || (effect & CSpriteGen.BOP_RGBAFILTER) != 0)
                rgb = CServices.swapRGB((rgbaCoeff & 0x00FFFFFF));
            else
                rgb = 0x00FFFFFF;

            rhPtr.getCurrentResult().forceInt(rgb);
        }
    }
}
