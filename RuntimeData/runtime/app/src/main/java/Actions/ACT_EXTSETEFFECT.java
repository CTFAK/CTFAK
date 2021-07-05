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

package Actions;

import Objects.CObject;
import OpenGL.GLRenderer;
import Params.PARAM_STRING;
import RunLoop.CRun;

public class ACT_EXTSETEFFECT extends CAct
{
    public void execute(CRun rhPtr)
    {
        CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo==null)
            return;

        String effectName = ((PARAM_STRING) evtParams[0]).string;
        int effect = GLRenderer.BOP_COPY;
        if (effectName != null && effectName.length() > 0)
        {
            if (effectName.equalsIgnoreCase("Add"))
                effect = GLRenderer.BOP_ADD;
            else if (effectName.equalsIgnoreCase("Invert"))
                effect = GLRenderer.BOP_INVERT;
            else if (effectName.equalsIgnoreCase("Sub"))
                effect = GLRenderer.BOP_SUB;
            else if (effectName.equalsIgnoreCase("Mno") || effectName.equalsIgnoreCase("Mono"))
                effect = GLRenderer.BOP_MONO;
            else if (effectName.equalsIgnoreCase("Blend"))
                effect = GLRenderer.BOP_BLEND;
            else if (effectName.equalsIgnoreCase("XOR"))
                effect = GLRenderer.BOP_XOR;
            else if (effectName.equalsIgnoreCase("OR"))
                effect = GLRenderer.BOP_OR;
            else if (effectName.equalsIgnoreCase("AND"))
                effect = GLRenderer.BOP_AND;
        }
        pHo.ros.rsEffect&=~GLRenderer.EFFECT_MASK;
        pHo.ros.rsEffect|=effect;
        pHo.roc.rcChanged=true;
        if (pHo.roc.rcSprite!=null)
        {
            rhPtr.spriteGen.modifSpriteEffect(pHo.roc.rcSprite, pHo.ros.rsEffect, pHo.ros.rsEffectParam);
        }
    }
}
