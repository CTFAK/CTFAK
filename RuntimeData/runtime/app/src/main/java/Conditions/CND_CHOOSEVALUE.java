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
// CHOOSE OBJECTS FROM VALUE
// 
// ------------------------------------------------------------------------------
package Conditions;

import Expressions.CValue;
import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class CND_CHOOSEVALUE extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {
	int cpt=0;
	
	// Boucle d'exploration
	CObject pHo=rhPtr.rhEvtProg.evt_FirstObjectFromType((short)-1);
	while(pHo!=null)
	{
	    cpt++;

	    int number;
	    if (evtParams[0].code==53)	    // pEvp->evpCode==PARAM_ALTVALUE_EXP
		number=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	    else
		number=((PARAM_SHORT)evtParams[0]).value;
	    CValue value2=rhPtr.get_EventExpressionAny_WithoutNewValue((CParamExpression)evtParams[1]);

	    if (pHo.rov!=null)
	    {
		CValue value=new CValue(pHo.rov.getValue(number));
		short comp=((CParamExpression)evtParams[1]).comparaison;
		if (CRun.compareTo(value, value2, comp)==false)
		{
		    rhPtr.rhEvtProg.evt_DeleteCurrentObject();
		    cpt--;
		}
	    }
	    pHo=rhPtr.rhEvtProg.evt_NextObjectFromType();
	};
	// Vrai / Faux?
	if (cpt!=0) 
	    return true;
	return false;        
    }
}
