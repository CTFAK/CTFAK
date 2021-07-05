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
// EXTENSION
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Objects.*;
import Events.*;

/** Extension expression.
 */
public class CExpExtension extends CExpOi
{    
    /** Evalutate the extension expression.
     * Calls the extension expression method, and returns the value.
     */
    public void evaluate(CRun rhPtr)
    {        
	CObject pHo=rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
	if (pHo==null)
	{
	    rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(0);
	    return;
	}
	CExtension pExt=(CExtension)pHo;
	int exp=(int)(short)(code>>>16)-CEventProgram.EVENTS_EXTBASE;				// Vire le type
	CValue result=pExt.expression(exp);
	rhPtr.rh4Results[rhPtr.rh4PosPile].forceValue(result);
    }        
}
