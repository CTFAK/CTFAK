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
// OUT OF PLAYFIELD?
// 
// ------------------------------------------------------------------------------
package Conditions;

import Events.CEventGroup;
import Movements.CRMvt;
import Objects.CObject;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class CND_EXTOUTPLAYFIELD extends CCnd implements IEvaObject
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
        PARAM_SHORT evpPtr=(PARAM_SHORT)evtParams[0];
        if ( (evpPtr.value&((short)rhPtr.rhEvtProg.rhCurParam0))==0 )	//; Prend le deuxieme parametre (directions)
            return false;

	if (compute_NoRepeat(hoPtr))
	{
            rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr);	// Stocke l'objet courant
            return true;
	}
	
	// Si une action STOP dans le groupe, il faut la faire!!!
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CEventGroup pEvg=rhPtr.rhEvtProg.rhEventGroup;
	if ((pEvg.evgFlags&CEventGroup.EVGFLAGS_STOPINGROUP)==0) 
            return false;
	rhPtr.rhEvtProg.rh3DoStop=true;
	return true;
    }
    public boolean eva2(CRun rhPtr)
    {
        return evaObject(rhPtr, this);
    }
    public boolean evaObjectRoutine(CObject pHo)
    {
	if ( (pHo.rom.rmEventFlags&CRMvt.EF_GOESOUTPLAYFIELD)!=0 )
            return negaTRUE();
	return negaFALSE();
    }
}
