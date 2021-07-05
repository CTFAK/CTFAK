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
// EXTENSION conditions
// 
// ------------------------------------------------------------------------------
package Conditions;

import Events.CEventProgram;
import Expressions.CValue;
import Objects.CExtension;
import Objects.CObject;
import Params.CParamExpression;
import Params.CPosition;
import Params.CPositionInfo;
import Params.PARAM_CMPTIME;
import Params.PARAM_COLOUR;
import Params.PARAM_EVERY;
import Params.PARAM_KEY;
import Params.PARAM_OBJECT;
import Params.PARAM_POSITION;
import Params.PARAM_SHORT;
import Params.PARAM_STRING;
import Params.PARAM_TIME;
import RunLoop.CRun;

public class CCndExtension extends CCnd
{
	int paramIndex;
	
    public boolean eva1(CRun rhPtr, CObject pHo)
    {
		if (pHo==null)
		{
		    return eva2(rhPtr);
		}
		CExtension extPtr=(CExtension)pHo;
		pHo.hoFlags|=CObject.HOF_TRUEEVENT;
		int cond=-(int)(short)(evtCode>>>16)-CEventProgram.EVENTS_EXTBASE-1;				// Vire le type

		paramIndex = 0;
		
		if (extPtr.condition(cond, this))
		{
		    rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
		    return true;
		}
        return false;
    }
    public boolean eva2(CRun rhPtr)
    {
		// Boucle d'exploration
		CObject pHo=rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		int cpt=rhPtr.rhEvtProg.evtNSelectedObjects;
		int cond=-(int)(short)(evtCode>>>16)-CEventProgram.EVENTS_EXTBASE-1;				// Vire le type
	
		while(pHo!=null)
		{
		    CExtension pExt=(CExtension)pHo;
		    pHo.hoFlags&=~CObject.HOF_TRUEEVENT;
		    
		    paramIndex = 0;
		    
		    if (pExt.condition(cond, this))
		    {
				if ((evtFlags2&EVFLAG2_NOT)!=0)
				{
				    cpt--;
				    rhPtr.rhEvtProg.evt_DeleteCurrentObject();			// On le vire!
				}
		    }
		    else
		    {
				if ((evtFlags2&EVFLAG2_NOT)==0)
				{
				    cpt--;
				    rhPtr.rhEvtProg.evt_DeleteCurrentObject();			// On le vire!
				}
		    }
		    pHo=rhPtr.rhEvtProg.evt_NextObject();
		}
		// Vrai / Faux?
		if (cpt!=0) 
		    return true;
		return false;
    }        
    
    // Recolte des parametres
    // ----------------------
    public PARAM_OBJECT getParamObject(CRun rhPtr, int _num)
    {
    	return (PARAM_OBJECT)evtParams[_num == -1 ? paramIndex ++ : _num];
    }
    public int getParamTime(CRun rhPtr, int _num)
    {	
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
		if (evtParams[num].code==2)	    // PARAM_TIME
		{
		    return ((PARAM_TIME)evtParams[num]).timer;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
    }
    public short getParamBorder(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public short getParamAltValue(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public short getParamDirection(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public int getParamAnimation(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
		if (evtParams[num].code==10)	    // PARAM_TIME
		{
		    return ((PARAM_SHORT)evtParams[num]).value;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
    }
    public short getParamPlayer(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public PARAM_EVERY getParamEvery(CRun rhPtr, int _num)
    {
    	return (PARAM_EVERY)evtParams[_num == -1 ? paramIndex ++ : _num];
    }
    public int getParamKey(CRun rhPtr, int _num)
    {
    	return ((PARAM_KEY)evtParams[_num == -1 ? paramIndex ++ : _num]).key;
    }
    public int getParamSpeed(CRun rhPtr, int _num)
    {
    	return rhPtr.get_EventExpressionInt((CParamExpression)
    			evtParams[_num == -1 ? paramIndex ++ : _num]);
    }
    public CPositionInfo getParamPosition(CRun rhPtr, int _num)
    {
    	//return (PARAM_POSITION)evtParams[_num == -1 ? paramIndex ++ : _num];

    	int num = (_num == -1 ? paramIndex ++ : _num);
        CPosition position = (CPosition) evtParams[num];
        CPositionInfo pInfo = new CPositionInfo();
        position.read_Position(rhPtr, 0, pInfo);
        return pInfo;
    }
    public short getParamJoyDirection(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public int getParamExpression(CRun rhPtr, int _num)
    {
    	return rhPtr.get_EventExpressionInt((CParamExpression)
    			evtParams[_num == -1 ? paramIndex ++ : _num]);
    }
    public int getParamColour(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
		if (evtParams[num].code==24)	    // PARAM_COLOUR
		{
		    return ((PARAM_COLOUR)evtParams[num]).color;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
    }
    public short getParamFrame(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public int getParamNewDirection(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
		if (evtParams[num].code==29)	    // PARAM_NEWDIRECTION
		{
		    return ((PARAM_SHORT)evtParams[num]).value;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
    }
    public short getParamClick(CRun rhPtr, int _num)
    {
    	return ((PARAM_SHORT)evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }
    public String getParamFilename(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
		if (evtParams[num].code==40)	    // PARAM_FILENAME
		{
		    return ((PARAM_STRING)evtParams[num]).string;
		}
		return rhPtr.get_EventExpressionString((CParamExpression)evtParams[num]);
    }
    public String getParamExpString(CRun rhPtr, int _num)
    {
    	return rhPtr.get_EventExpressionString((CParamExpression)
    			evtParams[_num == -1 ? paramIndex ++ : _num]);
    }
    public double getParamExpDouble(CRun rhPtr, int _num)
    {
		CValue value=rhPtr.get_EventExpressionAny_WithoutNewValue
			((CParamExpression)evtParams[_num == -1 ? paramIndex ++ : _num]);
		return value.getDouble();
    }
    public float getParamExpFloat(CRun rhPtr, int _num)
    {
    	return (float) getParamExpDouble (rhPtr, _num);
    }
    public String getParamFilename2(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
		if (evtParams[num].code==63)	    // PARAM_FILENAME2
		{
		    return ((PARAM_STRING)evtParams[num]).string;
		}
		return rhPtr.get_EventExpressionString((CParamExpression)evtParams[num]);
    }
    public boolean compareValues(CRun rhPtr, int _num, CValue value)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
		CValue value2=rhPtr.get_EventExpressionAny_WithoutNewValue((CParamExpression)evtParams[num]);
		short comp=((CParamExpression)evtParams[num]).comparaison;
		return CRun.compareTo(value, value2, comp);		
    }
    public boolean compareTime(CRun rhPtr, int _num, int t)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
		PARAM_CMPTIME p=(PARAM_CMPTIME)evtParams[num];
		CValue value2 =new CValue(p.timer);
		short comp=p.comparaison;
		CValue value=new CValue(t);
		return CRun.compareTo(value, value2, comp);	
    }
    
}
