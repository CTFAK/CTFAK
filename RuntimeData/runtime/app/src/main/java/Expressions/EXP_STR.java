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
// STR$
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import java.lang.String;

public class EXP_STR extends CExp
{    
	public void evaluate(CRun rhPtr)
	{        
		rhPtr.rh4CurToken++;
		CValue pValue=rhPtr.getExpression();
		switch(pValue.getType())
		{
			case 0:	    // TYPE_LONG:
				rhPtr.rh4Results[rhPtr.rh4PosPile].forceStringFromInt(pValue.getInt());
				break;
			case 1:	    // TYPE_DOUBLE:
				rhPtr.rh4Results[rhPtr.rh4PosPile].forceStringFromDouble(pValue.getDouble());
				break;
			default:
				rhPtr.rh4Results[rhPtr.rh4PosPile].forceString("");
				break;
		}
	}
}
