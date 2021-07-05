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
// MASQUE DE COLLISIONS
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Sprites.*;

public class EXP_GETCOLLISIONMASK extends CExp
{
    public void evaluate(CRun rhPtr)
    {        
	int x, y;

	rhPtr.rh4CurToken++;
	x=rhPtr.get_ExpressionInt();
	rhPtr.rh4CurToken++;
	y=rhPtr.get_ExpressionInt();

	int result=0;
	if ( rhPtr.y_GetLadderAt_Absolute(-1, x, y) != null )
	    result=2;
	else
	{
	    if ( rhPtr.rhFrame.bkdCol_TestPoint(x - rhPtr.rhWindowX, y - rhPtr.rhWindowY, CSpriteGen.LAYER_ALL, CColMask.CM_TEST_OBSTACLE) )
		    result=1;
	}
	rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(result);
    }
    
}
