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
// SHOOT TOWARD
//
// -----------------------------------------------------------------------------
package Actions;

import Animations.CAnim;
import Objects.CObject;
import Params.CPosition;
import Params.CPositionInfo;
import Params.PARAM_SHOOT;
import RunLoop.CRun;

public class ACT_EXTSHOOTTOWARD extends CAct
{
	public void execute(CRun rhPtr)
	{
		CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
		if (pHo==null) return;

		// Peut-on tirer?
		// ~~~~~~~~~~~~~~
		//if (pHo.roa.raAnimOn==CAnim.ANIMID_SHOOT) return;			//; Deja en train de tirer?

		// Cherche la position de creation
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		PARAM_SHOOT pEvp=(PARAM_SHOOT)evtParams[0];
		CPositionInfo pInfo=new CPositionInfo();
		if (pEvp.read_Position(rhPtr, 0x11, pInfo))
		{
			CPositionInfo pInfoDest=new CPositionInfo();
			if (((CPosition)evtParams[1]).read_Position(rhPtr, 0, pInfoDest))
			{
				// Trouve la bonne direction
				int x2=pInfoDest.x;
				int y2=pInfoDest.y;
				int dir=CRun.get_DirFromPente(x2-pInfo.x, y2-pInfo.y);				// Calcul des pentes

				// Va creer la balle
				pHo.shtCreate(pEvp, pInfo.x, pInfo.y, dir);
			}
		}        
	}
}
