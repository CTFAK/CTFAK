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
// SET SAMPLE PAN
//
// -----------------------------------------------------------------------------
package Actions;

import Params.CParam;
import Params.CParamExpression;
import Params.PARAM_SAMPLE;
import RunLoop.CRun;

public class ACT_SETSAMPLEPAN extends CAct
{
    public void execute(CRun rhPtr)
    {
		CParam p = evtParams[0];
		float pan = ((float)rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1])) / 100.0f;
		int nSound = -1;
	    // PARAM_EXPSTRING?
		if (p.code == 45) {
		    String name = rhPtr.get_EventExpressionString((CParamExpression)p);
		    nSound = rhPtr.rhApp.soundBank.getSoundHandleFromName(name);
		}
		else {
		    nSound = ((PARAM_SAMPLE)p).sndHandle;
		}
		if (nSound >= 0)
			rhPtr.rhApp.soundPlayer.setPan((short)nSound, pan);
    }
}
