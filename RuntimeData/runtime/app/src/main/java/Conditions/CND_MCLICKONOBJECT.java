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
// CLICK ON OBJECT
// 
// ------------------------------------------------------------------------------

package Conditions;

import Events.CQualToOiList;
import Objects.CObject;
import Params.PARAM_OBJECT;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class CND_MCLICKONOBJECT extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	PARAM_SHORT p=(PARAM_SHORT)evtParams[0];
	if (rhPtr.rhEvtProg.rhCurParam0!=p.value) 
	    return false;		    	// La touche

	short oi=(short)rhPtr.rhEvtProg.rhCurParam1;							//; L'objet qui clique
	PARAM_OBJECT po=(PARAM_OBJECT)evtParams[1];
	if (oi==po.oi)					//; L'oi sur lequel on clique
	{
	    rhPtr.rhEvtProg.evt_AddCurrentObject(rhPtr.rhEvtProg.rh4_2ndObject);
	    return true;
	}

	short oil=po.oiList;
	if (oil>=0) 
	    return false;							// Un Qualifier?
	CQualToOiList qoil=rhPtr.rhEvtProg.qualToOiList[oil&0x7FFF];
	int qoi;
	for (qoi=0; qoi<qoil.qoiList.length; qoi+=2)
	{
	    if (qoil.qoiList[qoi]==oi)
	    {
		rhPtr.rhEvtProg.evt_AddCurrentQualifier(oil);
		rhPtr.rhEvtProg.evt_AddCurrentObject(rhPtr.rhEvtProg.rh4_2ndObject);
		return true;
	    }
	};
	return false;
    }
    public boolean eva2(CRun rhPtr)
    {
	PARAM_SHORT p=(PARAM_SHORT)evtParams[0];
	if (rhPtr.rhEvtProg.rh2CurrentClick!=p.value) 
	    return false;		    	// La touche

	PARAM_OBJECT po=(PARAM_OBJECT)evtParams[1];
	return rhPtr.getMouseOnObjectsEDX(po.oiList, false);
    }
}
