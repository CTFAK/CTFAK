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
// SET FRAME HEIGHT
//
// -----------------------------------------------------------------------------
package Actions;

import Frame.CLayer;
import Params.CParamExpression;
import RunLoop.CRun;

public class ACT_SETFRAMEHEIGHT extends CAct
{
    @Override
	public void execute(CRun rhPtr)
    {
	int newHeight = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);

	// Set new height
	int nOldHeight = rhPtr.rhFrame.leHeight;
	rhPtr.rhFrame.leHeight = newHeight;

	// Set virtual height
	if ( nOldHeight == rhPtr.rhFrame.leVirtualRect.bottom )
	{
	    rhPtr.rhFrame.leVirtualRect.bottom = rhPtr.rhLevelSy = newHeight;
		rhPtr.rh3YMaximumKill = rhPtr.rhLevelSy + CRun.GAME_YBORDER;
	}

	for (CLayer pLayer : rhPtr.rhFrame.layers)
	{
		// Free loZones
		if(pLayer.m_loZones != null)
			pLayer.m_loZones.clear();
		pLayer.m_loZones = null;
	}

	//rhPtr.updateFrameDimensions(rhPtr.rhFrame.leWidth, newHeight);
	
	// Redraw frame
	rhPtr.ohRedrawLevel(true);    
    }
}
