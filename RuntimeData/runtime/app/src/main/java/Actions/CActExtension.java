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
// SUB FROM GLOBAL VARIABLE
//
// -----------------------------------------------------------------------------
package Actions;

import Events.CEventProgram;
import Expressions.CValue;
import Objects.CExtension;
import Objects.CObject;
import Params.CParamExpression;
import Params.CPosition;
import Params.CPositionInfo;
import Params.PARAM_COLOUR;
import Params.PARAM_CREATE;
import Params.PARAM_EVERY;
import Params.PARAM_EXTENSION;
import Params.PARAM_INT;
import Params.PARAM_KEY;
import Params.PARAM_OBJECT;
import Params.PARAM_SHOOT;
import Params.PARAM_SHORT;
import Params.PARAM_STRING;
import Params.PARAM_TIME;
import Params.PARAM_ZONE;
import RunLoop.CRun;
import Services.CBinaryFile;
import Services.CServices;

public class CActExtension extends CAct
{
	int paramIndex;
	
    /** Calls the action in the extension object.
     */
    public void execute(CRun rhPtr)
    {
        CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo == null)
        {
            return;
        }

        paramIndex = 0;
        
        int act = (int) (short) (evtCode >>> 16) - CEventProgram.EVENTS_EXTBASE;				// Vire le type
        CExtension pExt = (CExtension) pHo;        
        pExt.action(act, this);
    }

    // Recolte des parametres
    // ----------------------
    /** Returns an object parameter.
     */
    public CObject getParamObject(CRun rhPtr, int _num)
    {
        return rhPtr.rhEvtProg.get_ParamActionObjects(((PARAM_OBJECT)
        		evtParams[_num == -1 ? paramIndex ++ : _num]).oiList, this);
    }

    /** Returns a time parameter.
     */
    public int getParamTime(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        if (evtParams[num].code == 2)	    // PARAM_TIME
        {
            return ((PARAM_TIME) evtParams[num]).timer;
        }
        return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
    }

    /** Returns a border parameter.
     */
    public short getParamBorder(CRun rhPtr, int _num)
    {	
        return ((PARAM_SHORT)
        		evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }

    /** Returns a short parameter
     */
    public short getParamShort(CRun rhPtr, int _num)
    {
        return ((PARAM_SHORT)
        		evtParams[_num == -1 ? paramIndex ++ : _num]).value;
    }

    /** Returns an alterable value parameter.
     */
    public short getParamAltValue(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_SHORT) evtParams[num]).value;
    }

    /** Returns a direction parameter.
     */
    public short getParamDirection(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_SHORT) evtParams[num]).value;
    }

    /** Returns a create parameter.
     */
    public PARAM_CREATE getParamCreate(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return (PARAM_CREATE) evtParams[num];
    }

    /** Returns an animation parameter.
     */
    public int getParamAnimation(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        if (evtParams[num].code == 10)	    // PARAM_TIME
        {
            return ((PARAM_SHORT) evtParams[num]).value;
        }
        return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
    }

    /** Returns a player parameter.
     */
    public short getParamPlayer(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_SHORT) evtParams[num]).value;
    }

    /** Returns an every parameter.
     */
    public PARAM_EVERY getParamEvery(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return (PARAM_EVERY) evtParams[num];
    }

    /** Returns a key parameter.
     */
    public int getParamKey(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_KEY) evtParams[num]).key;
    }

    /** Returns a speed parameter.
     */
    public int getParamSpeed(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
    }

    /** Returns a position parameter.
     */
    public CPositionInfo getParamPosition(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        CPosition position = (CPosition) evtParams[num];
        CPositionInfo pInfo = new CPositionInfo();
        position.read_Position(rhPtr, 0, pInfo);
        return pInfo;
    }

    /** Returns a joystick direction parameter.
     */
    public short getParamJoyDirection(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_SHORT) evtParams[num]).value;
    }

    /** Returns a shoot parameter.
     */
    public PARAM_SHOOT getParamShoot(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return (PARAM_SHOOT) evtParams[num];
    }

    /** Returns a zone parameter.
     */
    public PARAM_ZONE getParamZone(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return (PARAM_ZONE) evtParams[num];
    }

    /** Returns a expression.
     */
    public int getParamExpression(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
    }

    /** Returns a color parameter.
     */
    public int getParamColour(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        if (evtParams[num].code == 24)	    // PARAM_COLOUR
        {
            return ((PARAM_COLOUR) evtParams[num]).color;
        }
        return CServices.swapRGB(rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]));
    }

    /** Returns a frame parameter.
     */
    public short getParamFrame(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_SHORT) evtParams[num]).value;
    }

    /** Returns a direction parameter.
     */
    public int getParamNewDirection(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        if (evtParams[num].code == 29)	    // PARAM_NEWDIRECTION
        {
            return ((PARAM_INT) evtParams[num]).value;
        }
        return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
    }

    /** Returns a click parameter.
     */
    public short getParamClick(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return ((PARAM_SHORT) evtParams[num]).value;
    }

    /** Returns a filename parameter.
     */
    public String getParamFilename(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        if (evtParams[num].code == 40)	    // PARAM_FILENAME
        {
            return ((PARAM_STRING) evtParams[num]).string;
        }
        return rhPtr.get_EventExpressionString((CParamExpression) evtParams[num]);
    }

    /** Returns a string expression parameter.
     */
    public String getParamExpString(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        return rhPtr.get_EventExpressionString((CParamExpression) evtParams[num]);
    }

    /** Returns a double expression parameter.
     */
    public double getParamExpDouble(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        CValue value = rhPtr.get_EventExpressionAny_WithoutNewValue((CParamExpression) evtParams[num]);
        return value.getDouble();
    }
    
    public float getParamExpFloat(CRun rhPtr, int _num)
    {
    	return (float) getParamExpDouble (rhPtr, _num);
    }
    
    /** Returns a filename parameter 2.
     */
    public String getParamFilename2(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        if (evtParams[num].code == 63)	    // PARAM_FILENAME2
        {
            return ((PARAM_STRING) evtParams[num]).string;
        }
        return rhPtr.get_EventExpressionString((CParamExpression) evtParams[num]);
    }

    /** Returns an extension parameter.
     */
    public CBinaryFile getParamExtension(CRun rhPtr, int _num)
    {
    	int num = (_num == -1 ? paramIndex ++ : _num);
    	
        PARAM_EXTENSION p = (PARAM_EXTENSION) evtParams[num];
        if (p.data != null)
        {
            return new CBinaryFile(p.data, rhPtr.rhApp.bUnicode);
        }
        return null;
    }
    
}
