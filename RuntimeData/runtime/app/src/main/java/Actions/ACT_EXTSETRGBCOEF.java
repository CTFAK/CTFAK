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
import Params.CParamExpression;
import RunLoop.CRun;
import Services.CServices;

public class ACT_EXTSETRGBCOEF extends CAct
{
    public void execute(CRun rhPtr)
    {
        CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo==null)
            return;

        int argb=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);

        boolean wasSemi = ((pHo.ros.rsEffect & GLRenderer.BOP_RGBAFILTER) == 0);
        pHo.ros.rsEffect = (pHo.ros.rsEffect & GLRenderer.BOP_MASK) | GLRenderer.BOP_RGBAFILTER;

        int rgbaCoeff = pHo.ros.rsEffectParam;
        int alphaPart;
        if(wasSemi)
        {
            if(pHo.ros.rsEffectParam == -1)
                alphaPart = 0xFF000000;
            else
                alphaPart = (255-pHo.ros.rsEffectParam*2)<<24;
        }
        else
            alphaPart = (rgbaCoeff & 0xFF000000);

        int rgbPart = CServices.swapRGB((argb & 0x00FFFFFF));
        int filter = alphaPart | rgbPart;
        pHo.ros.rsEffectParam = filter;

        pHo.roc.rcChanged=true;
        if (pHo.roc.rcSprite!=null)
            rhPtr.rhApp.run.spriteGen.modifSpriteEffect
                (pHo.roc.rcSprite, pHo.ros.rsEffect, pHo.ros.rsEffectParam);
    }
}
