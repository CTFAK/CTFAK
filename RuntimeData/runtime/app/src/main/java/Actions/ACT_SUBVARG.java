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
// SUB FROM GLOBAL VARIABLE
//
// -----------------------------------------------------------------------------
package Actions;

import Expressions.CValue;
import Params.CParamExpression;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class ACT_SUBVARG extends CAct
{    
    public void execute(CRun rhPtr)
    {
	int num;
	if (evtParams[0].code==52)	    // PARAM_VARGLOBAL_EXP 
	    num=(rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0])-1);	// &15; YVES: enlev
	else
	    num=((PARAM_SHORT)evtParams[0]).value;
	   
	CValue value=rhPtr.get_EventExpressionAny_WithoutNewValue((CParamExpression)evtParams[1]);
	rhPtr.rhApp.getGlobalValueAt(num).sub(value);
    }
}
