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
// ------------------------------------------------------------------------------
// 
// CHOOSE ALL OBJECT IN A LINE
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;

public class CND_CHOOSEALLINLINE extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	return eva2(rhPtr);        
    }
    public boolean eva2(CRun rhPtr)
    {
	int x1=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	int y1=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
	int x2=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[2]);
	int y2=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[3]);

	if (rhPtr.rhEvtProg.select_LineOfSight(x1, y1, x2, y2)!=0)
	    return true;
	return false;        
    }
}
