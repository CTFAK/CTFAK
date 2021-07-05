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
// ASK QUESTION
//
// -----------------------------------------------------------------------------
package Actions;

import Events.CQualToOiList;
import Params.CCreate;
import Params.CPositionInfo;
import RunLoop.CRun;

public class ACT_QASK extends CAct
{
	public void execute(CRun rhPtr)
	{
		if (evtOiList>=0)
		{
			qstCreate(rhPtr, evtOi);
			return;
		}

		// Un qualifier: on explore les listes
		if (evtOiList!=-1)
		{
			CQualToOiList qoil=rhPtr.rhEvtProg.qualToOiList[evtOiList&0x7FFF];	    
			int qoi;
			for (qoi=0; qoi<qoil.qoiList.length; qoi+=2)
			{
				qstCreate(rhPtr, qoil.qoiList[qoi]);
			}
		}        
	}
	void qstCreate(CRun rhPtr, short oi)
	{
		// Cherche la position de creation
		CCreate c=(CCreate)evtParams[0];
		CPositionInfo info=new CPositionInfo();

		if (c.read_Position(rhPtr, 0x10, info))
		{
			rhPtr.f_CreateObject(c.cdpHFII, oi, info.x, info.y, info.dir, (short)0, rhPtr.rhFrame.nLayers-1, -1);
		}
	}

}
