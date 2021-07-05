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
// MOVE TO LAYER
//
// -----------------------------------------------------------------------------
package Actions;

import Frame.CLayer;
import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;
import Sprites.CRSpr;
import Sprites.CSprite;
import Sprites.CSpriteGen;

public class ACT_EXTMOVETOLAYER extends CAct
{
    public void execute(CRun rhPtr)
    {
 	CObject hoPtr=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (hoPtr==null) return;

	if (hoPtr.ros!=null)
	{
	    int nLayer = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);

        nLayer -= 1;

	    if ( nLayer >= 0 && nLayer < rhPtr.rhFrame.nLayers && hoPtr.hoLayer!=nLayer)
	    {
				// Set new layer
				hoPtr.hoLayer = (short)nLayer;
                if ( hoPtr.ros!=null )
					hoPtr.ros.rsLayer = (short)nLayer;

                CLayer pLayer=rhPtr.rhFrame.layers[nLayer];
                pLayer.nZOrderMax++;      // B248
                
		// Show / hide sprite and update z-order index
                CSprite pSpr = hoPtr.roc.rcSprite;
                if ( pSpr != null )
                {
                    rhPtr.spriteGen.setSpriteLayer(pSpr, nLayer);

                    // Increments the maximum z-order value and use it
                    pSpr.sprZOrder = pLayer.nZOrderMax;

                    // Update z-order
                    if ( hoPtr.ros!=null )
                    {
                        // Update the zorder value in the yourapplication structure (not mandatory, done before DeleteSprite)
                        hoPtr.ros.rsZOrder = pSpr.sprZOrder;

                        // Hide object if new layer is hidden
                        if ( (pLayer.dwOptions & (CLayer.FLOPT_TOHIDE|CLayer.FLOPT_VISIBLE)) != CLayer.FLOPT_VISIBLE )
                        {
			    rhPtr.spriteGen.activeSprite(pSpr, CSpriteGen.AS_REDRAW, null);
			    hoPtr.ros.obHide();
                        }
                        else
                        {
			    // Show object if new layer is visible
			    if ( (hoPtr.ros.rsFlags&CRSpr.RSFLAG_VISIBLE) != 0 && (hoPtr.ros.rsFlags&CRSpr.RSFLAG_HIDDEN) != 0 &&
				     (pLayer.dwOptions & (CLayer.FLOPT_TOHIDE|CLayer.FLOPT_VISIBLE)) == CLayer.FLOPT_VISIBLE )
			    {
				hoPtr.ros.obShow();
			    }
                        }
                    }
                }
	    }
	}
    }
}
