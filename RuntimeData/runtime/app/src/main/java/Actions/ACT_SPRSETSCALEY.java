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

package Actions;

import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;

public class ACT_SPRSETSCALEY extends CAct
{
	public boolean bResampleDetermined = false;
	public boolean bResample;

    public void execute(CRun rhPtr)
    {
		CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
		if (pHo==null) 
			return;
	
		// Recupere parametres
		float fScale = (float) rhPtr.get_EventExpressionDouble((CParamExpression)evtParams[0]);
		if (fScale<0)
		{
			fScale=0;
		}
		if (!bResampleDetermined )
		{
			bResample = false;
			if ( rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]) != 0 )
				bResample = true;
		}
		pHo.setScale(pHo.roc.rcScaleX, fScale, bResample);
    }
}
