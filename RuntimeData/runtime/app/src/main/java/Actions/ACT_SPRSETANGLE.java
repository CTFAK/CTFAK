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

import Banks.CImageInfo;
import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CRun;
import RunLoop.CRunMBase;
import Sprites.CRSpr;

public class ACT_SPRSETANGLE extends CAct
{
	public boolean bAntiADetermined = false;
	public boolean bAntiA;

    @Override
	public void execute(CRun rhPtr)
    {
    	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
		if (pHo==null) 
			return;
	
		// Recupere parametres
		float nAngle = (float)rhPtr.get_EventExpressionDouble((CParamExpression)evtParams[0]);
		nAngle %= 360.0f;
		if ( nAngle < 0 )
			nAngle += 360;

        CRunMBase pMovement=null;
		if(rhPtr.rh4Box2DObject)
			pMovement=rhPtr.GetMBase(pHo);
        if (pMovement!=null)
        {
            pMovement.setAngle(nAngle);
            return;
        }

		if (!bAntiADetermined )
		{
			bAntiA = false;
			if ( rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]) != 0 )
				bAntiA = true;
		}
		
		boolean bOldAntiA = false;
		if ( (pHo.ros.rsFlags&CRSpr.RSFLAG_ROTATE_ANTIA)!= 0)
			bOldAntiA=true;
		if ( pHo.roc.rcAngle!=nAngle || bOldAntiA!=bAntiA )
		{
			pHo.roc.rcAngle=nAngle;
			pHo.ros.rsFlags &= ~CRSpr.RSFLAG_ROTATE_ANTIA;
			if ( bAntiA )
				pHo.ros.rsFlags |= CRSpr.RSFLAG_ROTATE_ANTIA;
			pHo.roc.rcChanged = true;
			pHo.updateImageInfo();
		}
    }
}
