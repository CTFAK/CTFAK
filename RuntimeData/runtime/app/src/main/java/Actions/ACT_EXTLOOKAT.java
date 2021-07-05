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
// LOOK AT
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.CPosition;
import Params.CPositionInfo;
import RunLoop.CRun;
import RunLoop.CRunMBase;

public class ACT_EXTLOOKAT extends CAct
{
    public void execute(CRun rhPtr)
    {
		CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
		if (pHo==null) 
	            return;
	
		CPosition position=(CPosition)evtParams[0];
		CPositionInfo pInfo=new CPositionInfo();
		if (position.read_Position(rhPtr, 0, pInfo))
		{
			int x=pInfo.x - pHo.hoX;
			int y=pInfo.y - pHo.hoY;

	        CRunMBase pMovement=null;
			if(rhPtr.rh4Box2DObject)
				pMovement=rhPtr.GetMBase(pHo);
            if (pMovement == null)
            {
                int dir=CRun.get_DirFromPente(x, y);
                dir&=31;
                if (rhPtr.getDir(pHo)!=dir)
                {
                    pHo.roc.rcDir=dir;
                    pHo.roc.rcChanged=true;
                    pHo.rom.rmMovement.setDir(dir);
                }
            }
            else
            {
                float angle=(float)(Math.atan2(-y, x)*180.0f/3.141592653589f);
                if (angle<0)
                    angle=360+angle;
                pMovement.setAngle(angle);
            }
		}
    }
}
