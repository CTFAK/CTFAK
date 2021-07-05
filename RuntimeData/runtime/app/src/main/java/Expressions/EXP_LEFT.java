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
// LEFT$
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;

public class EXP_LEFT extends CExp
{
    
    public void evaluate(CRun rhPtr)
    {        
	rhPtr.rh4CurToken++;
	String string=rhPtr.get_ExpressionString();
	rhPtr.rh4CurToken++;
	int pos=rhPtr.get_ExpressionInt();
	if (pos<0)
	    pos=0;
	if (pos>string.length())
	    pos=string.length();
	rhPtr.rh4Results[rhPtr.rh4PosPile].forceString(string.substring(0, pos));
    }
    
}
