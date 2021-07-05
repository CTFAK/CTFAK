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
// CLEAR ZONE
//
// -----------------------------------------------------------------------------
package Actions;

import Params.CParamExpression;
import Params.PARAM_COLOUR;
import Params.PARAM_ZONE;
import RunLoop.CBackDrawClsZone;
import RunLoop.CRun;
import Services.CServices;

public class ACT_CLEARZONE extends CAct
{
    public void execute(CRun rhPtr)
    {
	int color;
	if (evtParams[1].code==24)	// PARAM_COLOUR
	    color=((PARAM_COLOUR)evtParams[1]).color;
	else
    {
	    color=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
        color=CServices.swapRGB(color);
    }
	PARAM_ZONE p=(PARAM_ZONE)evtParams[0];
	
	CBackDrawClsZone routine=new CBackDrawClsZone();
	routine.color=color;
	routine.x1=p.x1;
	routine.x2=p.x2;
	routine.y1=p.y1;
	routine.y2=p.y2;
	rhPtr.addBackDrawRoutine(routine);		
    }
}
