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
// X CHANCES OUT OF Y
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;

public class CND_CHANCE extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
        return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {
	int param1=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	int param2=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
	if (param1 >= param2)
		return true;
	if (param2>=1 && param1>0 && param1<=param2)
	{
	    int rnd=rhPtr.random((short)param2);
	    if (rnd<param1)
	    {
		return true;
	    }
	}
	return false;
    }
}
