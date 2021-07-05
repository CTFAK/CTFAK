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
// IS STOPPED?
// 
// ------------------------------------------------------------------------------
package Conditions;

import Movements.CMoveDef;
import Movements.CMoveExtension;
import OI.CObjectCommon;
import Objects.CObject;
import RunLoop.CRun;

public class CND_EXTSTOPPED extends CCnd implements IEvaObject
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	return evaObject(rhPtr, this);
    }
    public boolean eva2(CRun rhPtr)
    {
	return evaObject(rhPtr, this);
    }
    public boolean evaObjectRoutine(CObject hoPtr)
    {
    if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0)
    {
        if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
        {
            CMoveExtension mvPtr = (CMoveExtension) hoPtr.rom.rmMovement;
            return mvPtr.stopped() ? negaTRUE () : negaFALSE ();
        }
    }

	if (hoPtr.roc.rcSpeed==0) 
	    return negaTRUE();
	return negaFALSE();
    }
}
