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
// GET RGB
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;

public class EXP_GETRGB extends CExp
{
    
    public void evaluate(CRun rhPtr)
    {        
	rhPtr.rh4CurToken++;
	int r=rhPtr.get_ExpressionInt();
	rhPtr.rh4CurToken++;
	int g=rhPtr.get_ExpressionInt();
	rhPtr.rh4CurToken++;
	int b=rhPtr.get_ExpressionInt();

	int rgb=((b&255)<<16) + ((g&255)<<8) + (r&255);
	rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(rgb);
    }
    
}
