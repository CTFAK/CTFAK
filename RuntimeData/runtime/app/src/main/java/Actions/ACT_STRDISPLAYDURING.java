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
// DISPLAY STRING DURING
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_SHORT;
import Params.PARAM_TIME;
import RunLoop.CRun;

public class ACT_STRDISPLAYDURING extends CAct
{
    @Override
	public void execute(CRun rhPtr)
    {
    	int numPar;

    	if(evtParams[1].code == 31)
       		numPar = ((PARAM_SHORT)evtParams[1]).value;
     	else
       		numPar=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);	//Expression
 
        int num=rhPtr.txtDoDisplay(this, numPar);			// trouve le numero du texte
        if (num>=0)
        {
            CObject hoPtr=rhPtr.rhObjectList[num];
            if (evtParams[2].code == 2)     // PARAM_TIME
            {
                PARAM_TIME p2=(PARAM_TIME)evtParams[2];
                hoPtr.ros.rsFlash=p2.timer;
            }
            else
            {
                hoPtr.ros.rsFlash=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[2]);
            }
            hoPtr.ros.rsFlashCpt=hoPtr.ros.rsFlash;
        }
    }
}
