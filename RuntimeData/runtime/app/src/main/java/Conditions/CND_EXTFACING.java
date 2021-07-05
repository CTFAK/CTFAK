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
// FACING A DIRECTION?
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.PARAM_INT;
import RunLoop.CRun;

public class CND_EXTFACING extends CCnd implements IEvaExpObject, IEvaObject
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	return eva2(rhPtr);
   }
    public boolean eva2(CRun rhPtr)
    {
	if (evtParams[0].code==29)		// PARAM_NEWDIRECTION)					// Le parametre direction?
	    return evaObject(rhPtr, this);
	return evaExpObject(rhPtr, this);					// Une expression
    }
    public boolean evaObjectRoutine(CObject hoPtr)
    {
	int mask=((PARAM_INT)evtParams[0]).value;
	int dir;
	for (dir=0; dir<32; dir++)
	{
	    if ( ((1<<dir)&mask)!=0 )
	    {
		if (hoPtr.hoAdRunHeader.getDir(hoPtr)==dir)
		{
		    return negaTRUE();
		}
	    }
	}
	return negaFALSE();
    }    
    public boolean evaExpRoutine(CObject hoPtr, int value, short comp)
    {
	value&=31;
	if (hoPtr.hoAdRunHeader.getDir(hoPtr)==value)
	{
	    return negaTRUE();
	}
	return negaFALSE();
    }
}
