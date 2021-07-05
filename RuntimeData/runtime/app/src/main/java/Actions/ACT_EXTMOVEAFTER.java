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
// MOVE AFTER
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.PARAM_OBJECT;
import RunLoop.CRun;
import Sprites.CSprite;

public class ACT_EXTMOVEAFTER extends CAct
{
    public void execute(CRun rhPtr)
    {
 	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo==null) return;

	if (pHo.ros!=null)
	{
	    CObject pHo2=rhPtr.rhEvtProg.get_ParamActionObjects(((PARAM_OBJECT)evtParams[0]).oiList, this);
	    if ( pHo2 == null )
		    return;

	    CSprite pSpr = pHo.roc.rcSprite;
	    CSprite pSpr2 = pHo2.roc.rcSprite;
	    if ( pSpr != null && pSpr2 != null )
	    {
		rhPtr.spriteGen.moveSpriteAfter(pSpr, pSpr2);
	    }
	}
    }
}
