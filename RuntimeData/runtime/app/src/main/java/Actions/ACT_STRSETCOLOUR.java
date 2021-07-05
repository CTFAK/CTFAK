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
// SET STRING COLOR
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Objects.CText;
import Params.CParamExpression;
import Params.PARAM_COLOUR;
import RunLoop.CRun;
import Services.CServices;

public class ACT_STRSETCOLOUR extends CAct
{
    public void execute(CRun rhPtr)
    {
	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo!=null)
	{
	    int color;
	    if (evtParams[0].code==24)	    // PARAM_COLOUR
		color=((PARAM_COLOUR)evtParams[0]).color;
	    else
	    {
		color=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		color=CServices.swapRGB(color);
	    }
	    CText pText=(CText)pHo;
	    pText.rsTextColor=color;
	    pHo.roc.rcChanged=true;
	    pHo.display();
	}
    }
}
