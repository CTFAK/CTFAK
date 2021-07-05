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
// CREATE OBJECT
//
// -----------------------------------------------------------------------------
package Actions;


import Frame.CLayer;
import OI.CObjectCommon;
import Objects.CObject;
import Params.CPositionInfo;
import Params.PARAM_CREATE;
import RunLoop.CRun;
import RunLoop.CRunMBase;

public class ACT_CREATE extends CAct
{
    public ACT_CREATE()
    {
    }

    @Override
	public void execute(CRun rhPtr)
    {
        // Cherche la position de creation
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        PARAM_CREATE pEvp = (PARAM_CREATE) evtParams[0];
        CPositionInfo pInfo = new CPositionInfo();
        if (pEvp.read_Position(rhPtr, 0x11, pInfo)==false)
        {
            return;
        }
        if (pInfo.bRepeat)
        {
            evtFlags |= ACTFLAGS_REPEAT;					// Refaire cette action
            rhPtr.rhEvtProg.rh2ActionLoop = true;				// Refaire un tour d'actions
        }
        else
        {
            evtFlags &= ~ACTFLAGS_REPEAT;					// Ne pas refaire cette action
        }

        // Cree l'objet
        // ~~~~~~~~~~~~
        int number = rhPtr.f_CreateObject(pEvp.cdpHFII, pEvp.cdpOi, pInfo.x, pInfo.y, pInfo.dir, (short) 0, pInfo.layer, -1);

        // Met l'objet dans la liste des objets selectionnes
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        if (number >= 0)
        {
            CObject pHo = rhPtr.rhObjectList[number];
            rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);

            if(rhPtr.rh4Box2DObject) {
                
            	if (pHo.hoType>=32 && 
            			( pHo.hoCommon.ocIdentifier==CRun.FANIDENTIFIER || 
            			  pHo.hoCommon.ocIdentifier==CRun.TREADMILLIDENTIFIER || 
            			  pHo.hoCommon.ocIdentifier==CRun.MAGNETIDENTIFIER ||
            			  pHo.hoCommon.ocIdentifier==CRun.ROPEANDCHAINIDENTIFIER))
            	{
            		rhPtr.rh4Box2DBase.AddPhysicsAttractor(pHo);
             	}
            	
            	CRunMBase mBase = null;            	
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
