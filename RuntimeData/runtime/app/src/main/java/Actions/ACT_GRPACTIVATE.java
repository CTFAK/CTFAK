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
// ACTIVATE GROUP
//
// -----------------------------------------------------------------------------
package Actions;

import Events.CEvent;
import Events.CEventGroup;
import Params.PARAM_GROUP;
import Params.PARAM_GROUPOINTER;
import RunLoop.CRun;

public class ACT_GRPACTIVATE extends CAct
{
    public void execute(CRun rhPtr)
    {
	PARAM_GROUPOINTER p=(PARAM_GROUPOINTER)evtParams[0];
	int evg=p.pointer;
	CEventGroup evgPtr=rhPtr.rhEvtProg.events[evg];
	CEvent evtPtr=evgPtr.evgEvents[0];

	PARAM_GROUP grpPtr=(PARAM_GROUP)evtPtr.evtParams[0];
	boolean bFlag=(grpPtr.grpFlags&PARAM_GROUP.GRPFLAGS_GROUPINACTIVE)!=0;
	grpPtr.grpFlags&=~PARAM_GROUP.GRPFLAGS_GROUPINACTIVE;

	if (bFlag)
	{
	    grpActivate(rhPtr, evg);
	}        
    }
    int grpActivate(CRun rhPtr, int evg)
    {
	CEventGroup evgPtr=rhPtr.rhEvtProg.events[evg];
	CEvent evtPtr=evgPtr.evgEvents[0];
	PARAM_GROUP grpPtr=(PARAM_GROUP)evtPtr.evtParams[0];
	int cpt;
	boolean bQuit=false;

	if ((grpPtr.grpFlags&PARAM_GROUP.GRPFLAGS_PARENTINACTIVE)==0)
	{
	    evgPtr.evgFlags&=~CEventGroup.EVGFLAGS_INACTIVE;

	    for (evg++, bQuit=false, cpt=1; ;)
	    {
		evgPtr=rhPtr.rhEvtProg.events[evg];
		evtPtr=evgPtr.evgEvents[0];
		switch (evtPtr.evtCode)
		{
		    case ((-10<<16)|65535):	    // CNDL_GROUP:
			grpPtr=(PARAM_GROUP)evtPtr.evtParams[0];
			if (cpt==1)
			{
			    grpPtr.grpFlags&=~PARAM_GROUP.GRPFLAGS_PARENTINACTIVE;
			}
			if ((grpPtr.grpFlags&PARAM_GROUP.GRPFLAGS_GROUPINACTIVE)==0)
			{
			    evg=grpActivate(rhPtr, evg);
			    continue;
			}
			else
			{
			    cpt++;
			}
			break;
		    case ((-11<<16)|65535):	    // CNDL_ENDGROUP:
			cpt--;
			if (cpt==0)
			{
			    evgPtr.evgFlags&=~CEventGroup.EVGFLAGS_INACTIVE;
			    bQuit=true;
			    evg++;
			}
			break;
		    case ((-23<<16)|65535):	    // CNDL_GROUPSTART:
			if (cpt==1)
			{
			    evgPtr.evgFlags&=~CEventGroup.EVGFLAGS_INACTIVE;
			    evgPtr.evgFlags&=~CEventGroup.EVGFLAGS_ONCE;
			}
			break;
			case ((-42 << 16) | 65535):
				evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;
				break;
		    default:
			if (cpt==1)
			{
			    evgPtr.evgFlags&=~CEventGroup.EVGFLAGS_INACTIVE;
			}
			break;
		}
		if (bQuit)
		    break;
		evg++;
	    }
	}
	else
	{
	    // Saute le groupe et les sous-groupes
	    for (evg++, bQuit=false, cpt=1; ; evg++)
	    {
		evgPtr=rhPtr.rhEvtProg.events[evg];
		evtPtr=evgPtr.evgEvents[0];
		switch (evtPtr.evtCode)
		{
		    case ((-10<<16)|65535):	    // CNDL_GROUP:
			cpt++;
			break;
		    case ((-11<<16)|65535):	    // CNDL_ENDGROUP:
			cpt--;
			if (cpt==0)
			{
			    bQuit=true;
			    evg++;
			}
			break;
			case ((-42 << 16) | 65535):
				evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;
				break;
		}
		if (bQuit)
		    break;
	    }
	}
	return evg;
    }
    
}
