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
// END OF ANIMATION
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class CND_EXTANIMENDOF extends CCnd implements IEvaExpObject, IEvaObject
{
    @Override
	public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	int ani;
	if (evtParams[0].code==10)	// PARAM_ANIMATION)
	{
	    ani=((PARAM_SHORT)evtParams[0]).value;						//; Comparee au parametre animation
	}	
	else
	{
	    ani=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	}

	if (ani!=rhPtr.rhEvtProg.rhCurParam0) 
	    return false;				// L'animation courante
	rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr);	// Stocke l'objet courant
	return true;
    }
    @Override
	public boolean eva2(CRun rhPtr)
    {
	if (evtParams[0].code==10)		// PARAM_ANIMATION)					// Le parametre direction?
	    return evaObject(rhPtr, this);

	return evaExpObject(rhPtr, this);					// Une expression
    }
    @Override
	public boolean evaExpRoutine(CObject hoPtr, int value, short comp)
    {
	if (value!=hoPtr.roa.raAnimOn) 
	    return false;
	if (hoPtr.roa.raAnimNumberOfFrame==0) 
	    return true;
	return false;
    }
    @Override
	public boolean evaObjectRoutine(CObject hoPtr)
    {
	short anim=((PARAM_SHORT)evtParams[0]).value;
	if (anim!=hoPtr.roa.raAnimOn) 
	    return false;
	if (hoPtr.roa.raAnimNumberOfFrame==0) 
	    return true;
	return false;
    }
}
