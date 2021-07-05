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
// GET X ACTION POINT
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Objects.*;

public class EXP_EXTLOOPINDEX extends CExpOi
{
    public void evaluate(CRun rhPtr)
    {
        int x=0;
        CObject pHo=rhPtr.rhEvtProg.get_ExpressionObjects(this.oiList);
        if (pHo!=null)
        {
            if (rhPtr.rh4CurrentForEach != null)
                if (this.oiList == rhPtr.rh4CurrentForEach.oi)
                    x = rhPtr.rh4CurrentForEach.index;

            if (rhPtr.rh4CurrentForEach2 != null)
                if (this.oiList == rhPtr.rh4CurrentForEach2.oi)
                    x = rhPtr.rh4CurrentForEach2.index;
        }
        rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(x);
    }
}
