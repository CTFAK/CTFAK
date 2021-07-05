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
// SET COLOR 1
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CCounter;
import Objects.CObject;
import Params.CParam;
import Params.CParamExpression;
import Params.PARAM_COLOUR;
import RunLoop.CRun;
import Services.CServices;

public class ACT_CSETCOLOR1 extends CAct
{
    public void execute(CRun rhPtr)
    {
	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo==null) 
            return;
	
	int color;
	if (evtParams[0].code==CParam.PARAM_EXPRESSION)
	{
	    color=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	    color=CServices.swapRGB(color);
	}
	else
	    color=((PARAM_COLOUR)evtParams[0]).color;
	
	((CCounter)pHo).cpt_SetColor1(color);        
    }
}
