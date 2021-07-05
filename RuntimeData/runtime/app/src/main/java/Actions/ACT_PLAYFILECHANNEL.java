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
// PLAY SAMPLE FILE CHANNEL
//
// -----------------------------------------------------------------------------
package Actions;

import Params.CParamExpression;
import Params.PARAM_STRING;
import RunLoop.CRun;

public class ACT_PLAYFILECHANNEL extends CAct
{
    public void execute(CRun rhPtr)
    {
	String filename;
	if (evtParams[0].code==40)	    // PARAM_FILENAME
	    filename=((PARAM_STRING)evtParams[0]).string;
	else
	    filename=rhPtr.get_EventExpressionString((CParamExpression)evtParams[0]);
	
	int channel=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
	
	rhPtr.rhApp.soundPlayer.playFile(filename, 1, channel-1, false);
    }
}
