//----------------------------------------------------------------------------------
//
// ALPHA COEF
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Expressions
{
    class EXP_EXTALPHACOEF : CExpOi
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
            int alpha = 0;
            int rgbaCoeff = effectParam;

            if ((effect & CSpriteGen.BOP_MASK) == CSpriteGen.BOP_EFFECTEX || (effect & CSpriteGen.BOP_RGBAFILTER) != 0)
            {
                alpha = 255 - ((rgbaCoeff >> 24)&0xFF);
            }
            else
            {
                if (effectParam == -1)
                    alpha = 0;
                else
                    alpha = effectParam * 2;
            }
            rhPtr.getCurrentResult().forceInt(alpha);
        }
    }
}
