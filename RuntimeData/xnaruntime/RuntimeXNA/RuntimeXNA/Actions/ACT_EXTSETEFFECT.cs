// -----------------------------------------------------------------------------
//
// SET EFFECT 
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Sprites;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
    class ACT_EXTSETEFFECT : CAct
    {
        public override void execute(CRun rhPtr)
        {
            CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
            if (pHo == null)
                return;

            string effectName = ((PARAM_EFFECT)evtParams[0]).pEffect;
            int effect = CSpriteGen.BOP_COPY;
            if (effectName != null && effectName.Length != 0)
            {
                if (effectName == "Add")
                    effect = CSpriteGen.BOP_ADD;
                else if (effectName == "Invert")
                    effect = CSpriteGen.BOP_INVERT;
                else if (effectName == "Sub")
                    effect = CSpriteGen.BOP_SUB;
                else if (effectName == "Mono")
                    effect = CSpriteGen.BOP_MONO;
                else if (effectName == "Blend")
                    effect = CSpriteGen.BOP_BLEND;
                else if (effectName == "XOR")
                    effect = CSpriteGen.BOP_XOR;
                else if (effectName == "OR")
                    effect = CSpriteGen.BOP_OR;
                else if (effectName == "AND")
                    effect = CSpriteGen.BOP_AND;

                pHo.ros.modifSpriteEffect(effect, pHo.ros.rsEffectParam);
            }
        }			
    }
}
