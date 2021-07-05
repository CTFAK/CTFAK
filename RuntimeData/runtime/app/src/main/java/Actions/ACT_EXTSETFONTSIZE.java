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
// -----------------------------------------------------------------------------
//
// SET FONT SIZE
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;
import Services.CFontInfo;
import Services.CRect;

public class ACT_EXTSETFONTSIZE extends CAct
{
    public static final int MAX_HEIGHTS=40;
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

    public void execute(CRun rhPtr)
    {
        CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo == null)
        {
            return;
        }

        int newSize = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
        int bResize = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);

        CFontInfo lf = CRun.getObjectFont(pHo);

        int oldSize = lf.lfHeight;
        lf.lfHeight = heightNormalToLF(newSize);
        newSize=lf.lfHeight;

        if (bResize == 0)
        {
            CRun.setObjectFont(pHo, lf, null);
        }
        else
        {
            CRect rc = new CRect();
            float coef = 1.0f;
            if (oldSize != 0)
            {
                coef = ((float) newSize) / ((float) oldSize);
            }
            rc.right = (int) (pHo.hoImgWidth * coef);
            rc.bottom = (int) (pHo.hoImgHeight * coef);
            rc.left = 0;
            rc.top = 0;
            CRun.setObjectFont(pHo, lf, rc);
        }
    }
    int heightNormalToLF(int height)
    {
        if (height<MAX_HEIGHTS)
        {
            return heightNormalToLF[height];
        }
        int nLogVert = 96;
        return (height*nLogVert) / 72;
    }
}
