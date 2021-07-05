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
// INPUT KEY
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;

public class EXP_GETINPUTKEY extends CExpOi
{
    public void evaluate(CRun rhPtr)
    {        
	int joueur=oi;

	rhPtr.rh4CurToken++;
	int key=rhPtr.get_ExpressionInt();			// Numero de la touche
/*	if (key<CRunApp.MAX_KEY)
	{
	    String s=CKeyConvert.getKeyText(rhPtr.rhApp.pcCtrlKeys[joueur][key]);
	    rhPtr.rh4Results[rhPtr.rh4PosPile].forceString(s);
	} */
	rhPtr.rh4Results[rhPtr.rh4PosPile].forceString("");
    }
}
