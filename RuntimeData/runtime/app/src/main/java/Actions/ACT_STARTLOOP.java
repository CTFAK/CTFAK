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
// START LOOP
//
// -----------------------------------------------------------------------------
package Actions;

import Events.CEventGroup;
import Events.CPosOnLoop;
import Params.CParamExpression;
import RunLoop.CRun;

public class ACT_STARTLOOP extends CAct
{
    public void execute(CRun rhPtr)
    {
        String name;
        int number;

        // Fast handling
        CParamExpression pExp = (CParamExpression)evtParams[0];
        if (rhPtr.rhEvtProg.complexOnLoop == false && pExp.comparaison > 0)
        {
            CPosOnLoop posOnLoop = rhPtr.rh4PosOnLoop.get(pExp.comparaison - 1);
            if (posOnLoop != null && posOnLoop.m_bOR == false)
            {
                name = posOnLoop.m_name;
                number=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);

                boolean bInfinite=false;
                CLoop pLoop = rhPtr.addFastLoop(name);
                pLoop.flags&=~CLoop.FLFLAG_STOP;
                bInfinite=false;
                if (number<0)
                {
                    bInfinite=true;
                    number=10;
                }
                String save=rhPtr.rh4CurrentFastLoop;
                boolean actionLoop=rhPtr.rhEvtProg.rh2ActionLoop;				// Flag boucle
                int actionLoopCount=rhPtr.rhEvtProg.rh2ActionLoopCount;			// Numero de boucle d'actions
                CEventGroup eventGroup=rhPtr.rhEvtProg.rhEventGroup;
                for (pLoop.index=0; pLoop.index<number; pLoop.index++)
                {
                    rhPtr.rh4CurrentFastLoop=name;
                    rhPtr.rhEvtProg.rh2ActionOn=false;
                    rhPtr.rhEvtProg.computeEventFastLoopList(posOnLoop.m_pointers);
                    if ((pLoop.flags&CLoop.FLFLAG_STOP)!=0)
                        break;
                    if (bInfinite)
                        number=pLoop.index+10;
                }
                rhPtr.rhEvtProg.rhEventGroup=eventGroup;
                rhPtr.rhEvtProg.rh2ActionLoopCount=actionLoopCount;			// Numero de boucle d'actions
                rhPtr.rhEvtProg.rh2ActionLoop=actionLoop;					// Flag boucle
                rhPtr.rh4CurrentFastLoop=save;
                rhPtr.rhEvtProg.rh2ActionOn=true;
                return;
            }
        }

        // Normal handling
        name=rhPtr.get_EventExpressionStringLowercase((CParamExpression)evtParams[0]);
        if (name.length()==0)
            return;
        number=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
        if (number == 0)
            return;

        boolean bInfinite=false;

        CLoop pLoop = rhPtr.addFastLoop (name);
        pLoop.flags&=~CLoop.FLFLAG_STOP;

        bInfinite=false;
        if (number<0)
        {
            bInfinite=true;
            number=10;
        }
        String save=rhPtr.rh4CurrentFastLoop;
        boolean actionLoop=rhPtr.rhEvtProg.rh2ActionLoop;				// Flag boucle
        int actionLoopCount=rhPtr.rhEvtProg.rh2ActionLoopCount;			// Numero de boucle d'actions
        CEventGroup eventGroup=rhPtr.rhEvtProg.rhEventGroup;
        for (pLoop.index=0; pLoop.index<number; pLoop.index++)
        {
            rhPtr.rh4CurrentFastLoop=name;
            rhPtr.rhEvtProg.rh2ActionOn=false;
            rhPtr.rhEvtProg.handle_GlobalEvents(((-16<<16)|65535));	// CNDL_ONLOOP;
            if ((pLoop.flags&CLoop.FLFLAG_STOP)!=0)
            break;
            if (bInfinite)
            number=pLoop.index+10;
        }
        rhPtr.rhEvtProg.rhEventGroup=eventGroup;
        rhPtr.rhEvtProg.rh2ActionLoopCount=actionLoopCount;			// Numero de boucle d'actions
        rhPtr.rhEvtProg.rh2ActionLoop=actionLoop;					// Flag boucle
        rhPtr.rh4CurrentFastLoop=save;
        rhPtr.rhEvtProg.rh2ActionOn=true;
    }
}
