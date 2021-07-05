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
// GET FONT SIZE
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Objects.*;
import Services.CFontInfo;

public class EXP_EXTGETFONTSIZE extends CExpOi
{
    final static int MAX_HEIGHTS=40;
    int heightNormalToLF[]=
    {
        0,		// 0
        1,		// 1
        2,		// 2
        3,		// 3
        5,		// 4
        7,		// 5
        8,		// 6
        9,		// 7
        11,		// 8
        12,		// 9
        13,		// 10
        15,		// 11
        16,		// 12
        17,		// 13
        19,		// 14
        20,		// 15
        21,		// 16
        23,		// 17
        24,		// 18
        25,		// 19
        27,		// 20
        28,		// 21
        29,		// 22
        31,		// 23
        32,		// 24
        33,		// 25
        35,		// 26
        36,		// 27
        37,		// 28
        39,		// 29
        40,		// 30
        41,		// 31
        43,		// 32
        44,		// 33
        45,		// 34
        47,		// 35
        48,		// 36
        49,		// 37
        51,		// 38
        52		// 39
    };
    public void evaluate(CRun rhPtr)
    {
        CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
        if (pHo == null)
        {
            rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(0);
            return;
        }
        CFontInfo info = CRun.getObjectFont(pHo);
        rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(heightLFToNormal(info.lfHeight));
    }
    int heightLFToNormal(int height)
    {
        int n;
        for (n=0; n<MAX_HEIGHTS; n++)
        {
            if (heightNormalToLF[n]==height)
            {
                return n;
            }
        }
        return (height * 72) / 96;
    }
}
