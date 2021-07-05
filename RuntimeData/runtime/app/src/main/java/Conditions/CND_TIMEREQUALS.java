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
// EVERY 2 : with expression
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_INT;
import Params.PARAM_TIME;
import RunLoop.CRun;

public class CND_TIMEREQUALS extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
        return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {

        int time;
        if (this.evtParams[0].code == 22)
            time = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
        else
            time = ((PARAM_TIME)evtParams[0]).timer;

        PARAM_INT param2 = (PARAM_INT)evtParams[1];
        if (rhPtr.rhTimer >= time)
        {
            if (param2.value == rhPtr.rhLoopCount)
            {
                param2.value = rhPtr.rhLoopCount + 1;
                return false;
            }
            param2.value = rhPtr.rhLoopCount + 1;
            return true;
        }
        return false;
    }
}
