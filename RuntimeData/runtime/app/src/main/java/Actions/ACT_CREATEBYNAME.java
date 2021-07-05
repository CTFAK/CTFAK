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
// CREATE OBJECT BY NAME
//
// -----------------------------------------------------------------------------
package Actions;

import Frame.CLayer;
import OI.COI;
import OI.CObjectCommon;
import Objects.CObject;
import Params.CParamExpression;
import Params.CPositionInfo;
import Params.PARAM_POSITION;
import RunLoop.CRun;
import RunLoop.CRunMBase;

public class ACT_CREATEBYNAME extends CAct
{
    @Override
	public void execute(CRun rhPtr)
    {
        String pName = rhPtr.get_EventExpressionString((CParamExpression)this.evtParams[0]);
        CPositionInfo pInfo = new CPositionInfo();
        if (((PARAM_POSITION)this.evtParams[1]).read_Position(rhPtr, 0x11, pInfo))
        {
            if (pInfo.bRepeat)
            {
                this.evtFlags |= CAct.ACTFLAGS_REPEAT;
                rhPtr.rhEvtProg.rh2ActionLoop = true;
            }
            else
                this.evtFlags &= ~CAct.ACTFLAGS_REPEAT;

            COI oiPtr;
            for (oiPtr=rhPtr.rhApp.OIList.getFirstOI(); oiPtr!=null; oiPtr=rhPtr.rhApp.OIList.getNextOI())
            {
                if ( oiPtr.oiType>=2 )
                {
                    if (oiPtr.oiName.equalsIgnoreCase(pName))
                        break;
                }
            }

            if(oiPtr == null)
            	return;
            
            int number = rhPtr.f_CreateObject((short)-1, oiPtr.oiHandle, pInfo.x, pInfo.y, pInfo.dir, (short)0, pInfo.layer, -1);
            if (number >= 0)
            {
                CObject pHo = rhPtr.rhObjectList[number];
                rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
                
				if(rhPtr.rh4Box2DObject) {
					
					if (pHo.hoType>=32 && 
	            			( pHo.hoCommon.ocIdentifier==CRun.FANIDENTIFIER || 
	            			  pHo.hoCommon.ocIdentifier==CRun.TREADMILLIDENTIFIER || 
	            			  pHo.hoCommon.ocIdentifier==CRun.MAGNETIDENTIFIER ||
	            			  pHo.hoCommon.ocIdentifier==CRun.ROPEANDCHAINIDENTIFIER ))
					{
						rhPtr.rh4Box2DBase.AddPhysicsAttractor(pHo);
					}

	                CRunMBase mBase = null;
	                if(rhPtr.rh4Box2DObject)
	     					mBase = rhPtr.GetMBase(pHo);
	                if (mBase != null)
	                    mBase.CreateBody();
	                else
	                {
	                    if (rhPtr.rh4Box2DBase != null)
	                    {
	                        rhPtr.rh4Box2DBase.rAddNormalObject(pHo);
	                    }
	                }
                }
                if (pInfo.layer != -1)
                {
                    if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0)
                    {
                        // Hide object if layer hidden
                        CLayer pLayer = rhPtr.rhFrame.layers[pInfo.layer];
                        if ((pLayer.dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) != CLayer.FLOPT_VISIBLE)
                        {
                            pHo.ros.obHide();
                        }
                    }
                }
            }
        }
    }
}
