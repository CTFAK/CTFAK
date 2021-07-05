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
// JOYSTICK PRESSED
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class CND_JOYPRESSED extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	int joueur=evtOi;						//; Le numero du player
	if (joueur!=rhPtr.rhEvtProg.rhCurOi) 
	    return false;

	short j=(short)rhPtr.rhEvtProg.rhCurParam0;
	PARAM_SHORT p=(PARAM_SHORT)evtParams[0];

    if (p.value == 0) /* "No joystick action" */
        return j == 0 ? negaTRUE() : negaFALSE();

	j&=p.value;
	if (j!=p.value) 
	    return false;
	return true;
    }
    public boolean eva2(CRun rhPtr)
    {
	int joueur=evtOi;						//; Le numero du player
	byte b=(byte)(rhPtr.rh2NewPlayer[joueur]&rhPtr.rhPlayer[joueur]);

	short s=(short)b;
	PARAM_SHORT p=(PARAM_SHORT)evtParams[0];

    if (p.value == 0) /* "No joystick action" */
        return s == 0 ? negaTRUE() : negaFALSE();

	s&=p.value;
	if (p.value!=s) 
	    return false;
	return true;
    }
}
