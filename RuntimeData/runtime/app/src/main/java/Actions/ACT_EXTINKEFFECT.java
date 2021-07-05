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
// SET INK EFFECT
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import OpenGL.GLRenderer;
import Params.PARAM_2SHORTS;
import RunLoop.CRun;
import Sprites.CSpriteGen;

public class ACT_EXTINKEFFECT extends CAct
{
    public void execute(CRun rhPtr)
    {
	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo==null) return;

	if (pHo.ros!=null)
	{
	    PARAM_2SHORTS p=(PARAM_2SHORTS)evtParams[0];

	    int mask=p.value1;
	    pHo.ros.rsEffect&=~CSpriteGen.EFFECT_MASK;
	    pHo.ros.rsEffect|=mask;

	    int param=p.value2;
	    pHo.ros.rsEffectParam=param;

        if (pHo.ros.rsEffect != GLRenderer.BOP_BLEND)
            pHo.ros.rsEffectParam = 0;

	    pHo.roc.rcChanged=true;
	    if (pHo.roc.rcSprite!=null)
	    {
		pHo.hoAdRunHeader.spriteGen.modifSpriteEffect(pHo.roc.rcSprite, pHo.ros.rsEffect, pHo.ros.rsEffectParam);
	    }        
	}
    }
}
