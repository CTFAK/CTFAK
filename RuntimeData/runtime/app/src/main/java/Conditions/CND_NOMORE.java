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
//----------------------------------------------------------------------------------
//
// NO MORE
//
//----------------------------------------------------------------------------------
package Conditions;

import Events.CEventGroup;
import Objects.CObject;
import Params.CParam;
import Params.CParamExpression;
import Params.PARAM_TIME;
import RunLoop.CRun;

public class CND_NOMORE extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
        return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {
    	CEventGroup pEvg=rhPtr.rhEvtProg.rhEventGroup;
    	if ((pEvg.evgFlags&CEventGroup.EVGFLAGS_NOMORE)!=0) 
    		return true;				//; Deja evalu?
    	if ((pEvg.evgFlags&(CEventGroup.EVGFLAGS_REPEAT|CEventGroup.EVGFLAGS_NOTALWAYS))!=0) 
    		return false;	//; Verification, valide?

		// Va evaluer l'expression
        if (evtParams[0].code == CParam.PARAM_EXPRESSION)
        {
            pEvg.evgInhibit = (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0])/10);
        }
        else
        {
            pEvg.evgInhibit = (((PARAM_TIME) evtParams[0]).timer / 10);				//; Valeur du timer /10
        }
		pEvg.evgInhibitCpt=0;									// Init du compteur
		pEvg.evgFlags|=CEventGroup.EVGFLAGS_NOMORE;						// NOMORE valide!
		return true;
    }    
}
