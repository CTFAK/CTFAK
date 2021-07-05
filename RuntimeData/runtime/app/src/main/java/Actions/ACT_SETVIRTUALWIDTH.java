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
// SET VIRTUAL WIDTH
//
// -----------------------------------------------------------------------------
package Actions;

import Params.CParamExpression;
import RunLoop.CRun;

public class ACT_SETVIRTUALWIDTH extends CAct
{
    public void execute(CRun rhPtr)
    {
	int newWidth = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);

	if ( newWidth < rhPtr.rhFrame.leWidth )
	    newWidth = rhPtr.rhFrame.leWidth;
	if ( newWidth>0x7FFFF000 )
	    newWidth = 0x7FFFF000;

	if ( rhPtr.rhFrame.leVirtualRect.right != newWidth )
	    rhPtr.rhFrame.leVirtualRect.right = rhPtr.rhLevelSx = newWidth;
    }
}
