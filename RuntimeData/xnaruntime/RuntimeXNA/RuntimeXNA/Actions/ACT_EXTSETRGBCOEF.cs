// -----------------------------------------------------------------------------
//
// SET DIRECTION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Sprites;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions
{
    class ACT_EXTSETRGBCOEF : CAct
    {
        public override void execute(CRun rhPtr)
        {
            CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
            if (pHo == null)
                return;
            if (pHo.ros == null)
                return;

            uint argb=(uint)rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            bool wasSemi = ((pHo.ros.rsEffect & CSpriteGen.BOP_RGBAFILTER) == 0);
            pHo.ros.rsEffect = (pHo.ros.rsEffect & CSpriteGen.BOP_MASK) | CSpriteGen.BOP_RGBAFILTER;

            uint rgbaCoeff = (uint)pHo.ros.rsEffectParam;
            uint alphaPart;
            if (wasSemi)
            {
                if (pHo.ros.rsEffectParam == -1)
                {
                    alphaPart = 0xFF000000;
                }
                else
                {
                    alphaPart = (uint)(255 - (pHo.ros.rsEffectParam*2))<<24;
                }
            }
            else
            {
                alphaPart = rgbaCoeff & 0xFF000000;
            }

            uint rgbPart = (uint)CServices.swapRGB((int)(argb & 0x00FFFFFF));
            uint filter = alphaPart | rgbPart;
            pHo.ros.rsEffectParam = (int)filter;

            pHo.roc.rcChanged = true;
            if (pHo.roc.rcSprite != null)
            {
                pHo.hoAdRunHeader.rhApp.spriteGen.modifSpriteEffect(pHo.roc.rcSprite, pHo.ros.rsEffect, pHo.ros.rsEffectParam);
            }
        }
    }
}
