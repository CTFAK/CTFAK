// -----------------------------------------------------------------------------
//
// SET ALPHA COEF
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Actions
{
    class ACT_EXTSETALPHACOEF : CAct
    {
        public override void execute(CRun rhPtr)
        {
            CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
            if (pHo == null)
                return;
            if (pHo.ros == null)
                return;

            byte alpha = (byte)CServices.clamp(255-rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]), 0, 255);
            bool wasSemi = ((pHo.ros.rsEffect & CSpriteGen.BOP_RGBAFILTER) == 0);
            pHo.ros.rsEffect = (pHo.ros.rsEffect & CSpriteGen.BOP_MASK) | CSpriteGen.BOP_RGBAFILTER;
            
            int rgbaCoeff = 0x00FFFFFF;
            
            if (!wasSemi)
                rgbaCoeff = pHo.ros.rsEffectParam;

            int alphaPart = alpha << 24;
            int rgbPart = (rgbaCoeff & 0x00FFFFFF);
            pHo.ros.rsEffectParam = alphaPart | rgbPart;

            pHo.roc.rcChanged = true;
            if (pHo.roc.rcSprite != null)
            {
                pHo.hoAdRunHeader.rhApp.spriteGen.modifSpriteEffect(pHo.roc.rcSprite, pHo.ros.rsEffect, pHo.ros.rsEffectParam);
            }
        }
    }
}
