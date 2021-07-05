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
// DISPATCH VAR
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_INT;
import Params.PARAM_SHORT;
import RunLoop.CRun;
import Values.CRVal;

public class ACT_EXTDISPATCHVAR extends CAct
{
    @Override
	public void execute(CRun rhPtr)
    {
	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo==null) return;

	int num;
	if (evtParams[0].code==53)
	    num=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	else
	    num=((PARAM_SHORT)evtParams[0]).value;

	PARAM_INT pBuffer=(PARAM_INT)evtParams[2];
	if (rhPtr.rhEvtProg.rh2ActionLoopCount==0)
	{
	    pBuffer.value=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
	}
	else
	{
	    pBuffer.value++;
	}
	if (pHo.rov!=null)
	{
	    if (num>=pHo.rov.rvNumberOfValues)
	    {
			if ( !pHo.rov.extendValues(num+10) )  // C++
				return;
		}
	    pHo.rov.getValue(num).forceInt(pBuffer.value);
	}        
    }
}
