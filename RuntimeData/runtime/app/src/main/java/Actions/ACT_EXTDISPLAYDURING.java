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
// DISPLAY DURING
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_TIME;
import RunLoop.CRun;
import Sprites.CRSpr;

public class ACT_EXTDISPLAYDURING extends CAct
{
    public void execute(CRun rhPtr)
    {
        CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo==null) return;

        if (pHo.ros!=null)
        {
            pHo.ros.obHide();
            pHo.ros.rsFlags&=~CRSpr.RSFLAG_VISIBLE;

            if (evtParams[0].code == 2)     // PARAM_TIME
            {
                PARAM_TIME p=(PARAM_TIME)evtParams[0];
                pHo.ros.rsFlash=p.timer;
            }
            else
            {
                pHo.ros.rsFlash=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            }
            pHo.ros.rsFlashCpt=pHo.ros.rsFlash;
        }
    }
}