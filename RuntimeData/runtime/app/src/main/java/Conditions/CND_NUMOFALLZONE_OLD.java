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
// CND_NUMOFALLZONE_OLD
// 
// ------------------------------------------------------------------------------
package Conditions;

import Expressions.CValue;
import OI.COI;
import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_ZONE;
import RunLoop.CRun;

public class CND_NUMOFALLZONE_OLD extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {
	// Le nombre d'objets
	rhPtr.rhEvtProg.count_ZoneTypeObjects((PARAM_ZONE)evtParams[0], -1, COI.OBJ_SPR);

	// Le parametre
	CValue value2 =rhPtr.get_EventExpressionAny_WithoutNewValue((CParamExpression)evtParams[1]);
	short comp=((CParamExpression)evtParams[1]).comparaison;
	return CRun.compareIntTo(rhPtr.rhEvtProg.evtNSelectedObjects, value2, comp);
    }
}
