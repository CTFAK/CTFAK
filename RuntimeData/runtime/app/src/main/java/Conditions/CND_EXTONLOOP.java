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
// OBJECT ONLOOP
// 
// ------------------------------------------------------------------------------
package Conditions;

import Events.CForEach;
import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;
import Runtime.Log;
import Params.PARAM_MULTIPLEVAR;

public class CND_EXTONLOOP extends CCnd
{
	boolean bDeterminedName = false;
	public String name;

    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
		if ( !bDeterminedName )
    		name = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
        if (rhPtr.rh4CurrentForEach != null)
        {
            if (name.equalsIgnoreCase(rhPtr.rh4CurrentForEach.name))
            {
				if (evtNParams > 1)
				{
					if (((PARAM_MULTIPLEVAR)evtParams[1]).evaluate(hoPtr) == false) {
						return false;
					}
				}
                rhPtr.rhEvtProg.evt_ForceOneObject(this.evtOiList, hoPtr);
                return true;
            }
        }
        if (rhPtr.rh4CurrentForEach2 != null)
        {
            if (name.equalsIgnoreCase(rhPtr.rh4CurrentForEach2.name))
            {
				if (evtNParams > 1)
				{
					if (((PARAM_MULTIPLEVAR)evtParams[1]).evaluate(hoPtr) == false) {
						return false;
					}
				}
                rhPtr.rhEvtProg.evt_ForceOneObject(this.evtOiList, hoPtr);
                return true;
            }
        }
        return false;
    }
    public boolean eva2(CRun rhPtr)
    {
		if ( !bDeterminedName )
    		name = rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
        CForEach pForEach=rhPtr.rh4CurrentForEach;
        CObject pHo2 = null;
        if (pForEach!=null)
        {
            if (pForEach.name.equalsIgnoreCase(name))
            {
                if (pForEach.oi==this.evtOiList)
                {
                    pHo2=pForEach.objects[pForEach.index%pForEach.number];
                }
            }
        }
        pForEach = rhPtr.rh4CurrentForEach2;
        if (pForEach!=null)
        {
            if (pForEach.name.equalsIgnoreCase(name))
            {
                if (pForEach.oi==this.evtOiList)
                {
                    pHo2=pForEach.objects[pForEach.index%pForEach.number];
                }
            }
        }
        if (pHo2!=null)
        {
			if (evtNParams > 1)
			{
				if (((PARAM_MULTIPLEVAR)evtParams[1]).evaluate(pHo2) == false) {
					return false;
				}
			}
            rhPtr.rhEvtProg.evt_ForceOneObject(this.evtOiList, pHo2);
            return true;
        }
        return false;
    }
}
