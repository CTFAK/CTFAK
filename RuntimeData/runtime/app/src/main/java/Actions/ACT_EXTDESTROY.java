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
// DESTROY
//
// -----------------------------------------------------------------------------
package Actions;

import OI.CObjectCommon;
import Objects.CObject;
import Objects.CText;
import RunLoop.CRun;
import Sprites.CRSpr;

public class ACT_EXTDESTROY extends CAct
{
    @Override
	public void execute(CRun rhPtr)
    {
	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo==null) 
            return;

	if (pHo.hoType==3)	// OBJ_TEXT)
	{
	    CText pText=(CText)pHo;
	    if ((pText.rsHidden&CRun.COF_FIRSTTEXT)!=0)				//; Le dernier objet texte?
	    {
		pHo.ros.obHide();										//; Cache pour le moment
		pHo.ros.rsFlags&=~CRSpr.RSFLAG_VISIBLE;
		pHo.hoFlags|=CObject.HOF_NOCOLLISION; 
	    }
	    else
	    {
		pHo.hoFlags|=CObject.HOF_DESTROYED;						//; NON: on le detruit!
		rhPtr.destroy_Add(pHo.hoNumber);
	    }
	    return;
	}
	if ((pHo.hoFlags&CObject.HOF_DESTROYED)==0)
	{
	    pHo.hoFlags|=CObject.HOF_DESTROYED;
	    if ( (pHo.hoOEFlags&CObjectCommon.OEFLAG_ANIMATIONS)!=0 || (pHo.hoOEFlags&CObjectCommon.OEFLAG_SPRITES)!=0)
	    {
		// Jouer l'anim disappear
		rhPtr.init_Disappear(pHo);
	    }
	    else
	    {
		// Pas un objet avec animation : destroy
		pHo.hoCallRoutine=false;
		rhPtr.destroy_Add(pHo.hoNumber);
	    }
	}
    }
}
