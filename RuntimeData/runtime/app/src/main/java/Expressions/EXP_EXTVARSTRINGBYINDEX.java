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
// STRING BY INDEX
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Objects.*;
import Values.*;

public class EXP_EXTVARSTRINGBYINDEX extends CExpOi
{
    public void evaluate(CRun rhPtr)
    {        
     	CObject pHo=rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
        rhPtr.rh4CurToken++;
        int number=rhPtr.get_ExpressionInt();
        if (pHo!=null && pHo.rov!=null)
        {
            if (number>=0 && number<pHo.rov.rvNumberOfStrings)
            {
                rhPtr.rh4Results[rhPtr.rh4PosPile].forceString(pHo.rov.getString(number));
                return;
            }
        }
        rhPtr.rh4Results[rhPtr.rh4PosPile].forceString("");
    }
}
