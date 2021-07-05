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
// ADD TO ALTERABLE VALUE
//
// -----------------------------------------------------------------------------
package Actions;

import Expressions.CValue;
import Objects.CObject;
import RunLoop.CRun;
import Values.CRVal;

public class ACT_EXTADDVARDBL extends CAct
{
	int		varNum;
	double	value;

    @Override
	public void execute(CRun rhPtr)
    {
		CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
		if (pHo==null) return;
	
		if (pHo.rov!=null)
		{
		    if (varNum>=pHo.rov.rvNumberOfValues)
		    {
				if ( !pHo.rov.extendValues(varNum+10) )
					return;
			}
			pHo.rov.getValue(varNum).add(value);
		}        
    }
}
