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
// GOTO LEVEL
//
// -----------------------------------------------------------------------------
package Actions;

import Params.CParamExpression;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class ACT_GOLEVEL extends CAct
{
    public void execute(CRun rhPtr)
    {
        short level;
        if (evtParams[0].code == 26)  // PARAM_FRAME
        {
            level = ((PARAM_SHORT) evtParams[0]).value;
            // Verifie la validite du level
            if (rhPtr.rhApp.HCellToNCell(level) == -1)
            {
                return;
            }
        }
        else
        {
            // Avec un calcul
            level = (short) (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]) - 1);		// Une expression
            if (level < 0 || level >= 4096)
            {
                return;			// Entre 0 et 4096
            }
            level += rhPtr.rhApp.frameOffset;
            level |= 0x8000;
        }
        rhPtr.rhQuit = CRun.LOOPEXIT_GOTOLEVEL;
        rhPtr.rhQuitParam = level;
    }
}
