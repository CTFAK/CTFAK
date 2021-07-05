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
// EVENT AFTER
//
// -----------------------------------------------------------------------------
package Actions;

import Params.CParamExpression;
import Params.PARAM_TIME;
import RunLoop.CRun;
import RunLoop.TimerEvents;

public class ACT_EVENTAFTER extends CAct
{
    public void execute(CRun rhPtr)
    {
        long timer;
        if (evtParams[0].code==22)	    // PARAM_EXPRESSION
            timer=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
        else
            timer=((PARAM_TIME)evtParams[0]).timer;

        String pName=rhPtr.get_EventExpressionString((CParamExpression)evtParams[1]);

        TimerEvents pLoop=rhPtr.rh4TimerEvents;
        TimerEvents pPrevious=null;
        while(pLoop!=null)
        {
            pPrevious=pLoop;
            pLoop=pLoop.next;
        }
        TimerEvents pEvent=new TimerEvents();
        if (pPrevious==null)
            rhPtr.rh4TimerEvents=pEvent;
        else
            pPrevious.next=pEvent;
        pEvent.type=TimerEvents.TIMEREVENTTYPE_ONESHOT;
        pEvent.timer=rhPtr.rhTimer+timer;
        pEvent.name=pName;
        pEvent.next=null;
    }
}
