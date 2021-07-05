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
// MINIMUM
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;

public class EXP_MIN extends CExp
{
	// Build 284: new version that doesn't use get_ExpressionAny() to avoid creating a new CValue
    public void evaluate(CRun rhPtr)
    {
        rhPtr.rh4CurToken++;
		CValue aValue = rhPtr.getExpression();
        rhPtr.rh4CurToken++;

		if ( aValue.getType()==CValue.TYPE_DOUBLE )
		{
			double d1 = aValue.getDouble();
			CValue bValue = rhPtr.getExpression();
			double d2 = bValue.getDouble();
			rhPtr.rh4Results[rhPtr.rh4PosPile].forceDouble(Math.min(d1, d2));
		}
		else
		{
			int a = aValue.getInt();
			CValue bValue = rhPtr.getExpression();
			if (  bValue.getType() == CValue.TYPE_DOUBLE )
			{
				rhPtr.rh4Results[rhPtr.rh4PosPile].forceDouble(Math.min((double)a, bValue.getDouble()));
			}
			else
			{
				int b = bValue.getInt();
				if (a<b)
					rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(a);
				else
					rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(b);
			}
		}
    }
}
