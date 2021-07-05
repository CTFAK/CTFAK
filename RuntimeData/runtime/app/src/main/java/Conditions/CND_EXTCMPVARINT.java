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
// COMPARE TO ALTERABLE VALUE
// 
// ------------------------------------------------------------------------------
package Conditions;

import Expressions.CValue;
import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_SHORT;
import RunLoop.CRun;
import Values.CRVal;

public class CND_EXTCMPVARINT extends CCnd
{
	private final CValue valueZero = new CValue(0);

	@Override
	public boolean eva1(CRun rhPtr, CObject hoPtr)
	{
		return eva2(rhPtr);        
	}
	@Override
	public boolean eva2(CRun rhPtr)
	{
		// Boucle d'exploration
		CObject pHo=rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		if (pHo==null) return false;

		int cpt=rhPtr.rhEvtProg.evtNSelectedObjects;
		CValue value1;
		int num=((PARAM_SHORT)evtParams[0]).value;
		CParamExpression p = (CParamExpression)evtParams[1];
		int value2=p.tokens[0].getInt();
		if ( num >= 0 )
		{
			do {
				if (pHo.rov!=null)
				{
					if ( num<pHo.rov.rvNumberOfValues )
						value1 = pHo.rov.getValue(num);
					else
						value1 = valueZero;

					if (CRun.compareToInt(value1, value2, p.comparaison)==false)
					{
						cpt--;
						rhPtr.rhEvtProg.evt_DeleteCurrentObject();
					}
				}
				else
				{
					cpt--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
				pHo=rhPtr.rhEvtProg.evt_NextObject();
			}while(pHo!=null);
		}
		else
		{
			do {
				cpt--;
				rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				pHo=rhPtr.rhEvtProg.evt_NextObject();
			}while(pHo!=null);
		}

		return (cpt!=0);
	}
}
