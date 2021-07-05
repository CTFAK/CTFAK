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
// SET INPUT
//
// -----------------------------------------------------------------------------
package Actions;

import Application.CRunApp;
import Params.CParamExpression;
import RunLoop.CRun;

public class ACT_SETINPUT extends CAct
{
    public void execute(CRun rhPtr)
    {
	int input=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	if (input>CRunApp.CTRLTYPE_KEYBOARD)
	    return;
	if (input==CRunApp.CTRLTYPE_MOUSE)
	    input=CRunApp.CTRLTYPE_KEYBOARD;
	int joueur=evtOi;
	if ( joueur>=4 )	// MAX_PLAYER
	    return;
	rhPtr.rhApp.getCtrlType()[joueur]=(short)input;

/*JOYSTICK
	// Ajout Yves build 242: initialize joystick if necessary
	if ( input >= CTRLTYPE_JOY1 && input <= CTRLTYPE_JOY4 )
		InitJoystick(joueur, input - CTRLTYPE_JOY1);
*/
    }
}
