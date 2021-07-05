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
// PICK AN OBJECT
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import RunLoop.CRun;
import Params.CParam;
import Params.PARAM_MULTIPLEVAR;

public class CND_EXTCHOOSE extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
		return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {
		rhPtr.rhEvtProg.count_ObjectsFromOiList(evtOiList, -1);
		if (rhPtr.rhEvtProg.evtNSelectedObjects==0)
			return false;
		int rnd=rhPtr.random((short)rhPtr.rhEvtProg.evtNSelectedObjects);
		CObject pHo=rhPtr.rhEvtProg.count_ObjectsFromOiList(evtOiList, rnd);
		if (evtNParams > 0)
		{
			CParam p = evtParams[0];
			if (p.code == 68) {       // PARAM_MULTIPLEVAR)
				if (((PARAM_MULTIPLEVAR)p).evaluate(pHo) == false) {
					return false;
				}
			}
		}
		rhPtr.rhEvtProg.evt_ForceOneObject(evtOiList, pHo);
		return true;
    }
}
