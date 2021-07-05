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
// ACT_EXTSETLINEARVELOCITY
//
// -----------------------------------------------------------------------------
package Actions;

import Movements.CMoveExtension;
import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;

public class ACT_EXTSETLINEARVELOCITY extends CAct
{
    public void execute(CRun rhPtr)
    {
        CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo==null) return;

        if (rhPtr.rh4Box2DObject && rhPtr.GetMBase(pHo) != null)
        {
            double param1 = rhPtr.get_EventExpressionDouble((CParamExpression)evtParams[0]);
            double param2 = rhPtr.get_EventExpressionDouble((CParamExpression)evtParams[1]);
            ((CMoveExtension)pHo.rom.rmMovement).callMovement2(CAct.NACT_EXTSETLINEARVELOCITY, param1, param2);
        }
    }
}
