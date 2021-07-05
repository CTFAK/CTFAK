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
// NODE NAME REACHED?
// 
// ------------------------------------------------------------------------------
package Conditions;

import Movements.CMoveDef;
import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;

public class CND_EXTPATHNODENAME extends CCnd implements IEvaObject
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	String pName=rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
	if (hoPtr.hoMT_NodeName!=null)
	{
	    if (hoPtr.hoMT_NodeName.compareTo(pName)==0)
	    {
		return true;
	    }
	}
	return false;
    }
    public boolean eva2(CRun rhPtr)
    {
	return evaObject(rhPtr, this);        
    }
    public boolean evaObjectRoutine(CObject hoPtr)
    {
	if (hoPtr.roc.rcMovementType!=CMoveDef.MVTYPE_TAPED) 
	    return false;
	if (checkMark(hoPtr.hoAdRunHeader, hoPtr.hoMark1))
	{
	    String pName=hoPtr.hoAdRunHeader.get_EventExpressionString((CParamExpression)evtParams[0]);
	    if (hoPtr.hoMT_NodeName!=null)
	    {
		if (hoPtr.hoMT_NodeName.compareTo(pName)==0)
		{
		    return true;
		}
	    }
	}
	return false;
    }
}
