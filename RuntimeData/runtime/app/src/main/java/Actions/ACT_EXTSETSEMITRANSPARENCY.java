/* Copyright (c) 1996-2013 Clickteam
 *
 * This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
 * 
 * Permission is hereby granted to any person obtaining a legal copy 
 * of Clickteam Multimedia Fusion 2 to use or modify this source code for 
 * debugging, optimizing, or customizing applications created with 
 * Clickteam Multimedia Fusion 2.  Any other use of this source code is prohibited.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
// -----------------------------------------------------------------------------
//
// SET SEMI TRANSPARENCY
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import OpenGL.GLRenderer;
import Params.CParamExpression;
import RunLoop.CRun;
import Sprites.CSprite;

public class ACT_EXTSETSEMITRANSPARENCY extends CAct
{
    public void execute(CRun rhPtr)
    {
        CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo==null) return;

        int val=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);

        //Change semitransparency or alpha value?
        if((pHo.ros.rsEffect & GLRenderer.BOP_RGBAFILTER) != 0)
        {
            val = 255 - val * 2;

            if (val > 255)
                val = 255;

            if (val < 0)
                val = 0;

            pHo.ros.rsEffect = (pHo.ros.rsEffect & GLRenderer.BOP_MASK) | GLRenderer.BOP_RGBAFILTER;

            int rgbaCoeff = pHo.ros.rsEffectParam;
            int alphaPart = (int)val << 24;
            int rgbPart = (rgbaCoeff & 0x00FFFFFF);
            pHo.ros.rsEffectParam = alphaPart | rgbPart;
        }
        else
        {
            if (val > 128)
                val = 128;

            if (val < 0)
                val = 0;

            pHo.ros.rsEffect&=~GLRenderer.EFFECT_MASK;
            pHo.ros.rsEffect|=CSprite.EFFECT_SEMITRANSP;
            pHo.ros.rsEffectParam=val;
        }

        pHo.roc.rcChanged=true;
        if (pHo.roc.rcSprite!=null)
        {
            rhPtr.rhApp.run.spriteGen
                    .modifSpriteEffect (pHo.roc.rcSprite, pHo.ros.rsEffect, pHo.ros.rsEffectParam);
        }

    }
}
