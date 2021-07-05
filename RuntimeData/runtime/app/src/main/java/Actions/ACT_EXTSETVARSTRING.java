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
// SET ALTERABLE STRING
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_SHORT;
import RunLoop.CRun;
import Values.CRVal;

public class ACT_EXTSETVARSTRING extends CAct
{
    public void execute(CRun rhPtr)
    {
		CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
		if (pHo==null)
			return;
	
		int num;
		if (evtParams[0].code==62)	// PARAM_ALTSTRING_EXP
			num=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
		else
			num=((PARAM_SHORT)evtParams[0]).value;

		if (num>=0 && pHo.rov!=null)
		{
			if (num>=pHo.rov.rvNumberOfStrings)
			{
				if ( !pHo.rov.extendStrings(num+10) )
					return;
			}

			if (num<pHo.rov.rvNumberOfStrings)
			{
				String s=rhPtr.get_EventExpressionString((CParamExpression)evtParams[1]);
				pHo.rov.setString(num, s);
			}        
		}
	}
}
