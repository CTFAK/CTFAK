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
// SPRITE ADD BACKGROUND
//
// -----------------------------------------------------------------------------
package Actions;

import Objects.CObject;
import Params.PARAM_SHORT;
import RunLoop.CRun;

public class ACT_SPRADDBKD extends CAct
{
    public void execute(CRun rhPtr)
    {
	CObject pHo=rhPtr.rhEvtProg.get_ActionObjects(this);
	if (pHo==null) 
	    return;

	// Un cran d'animation sans effet
	if (pHo.roa!=null)
	    pHo.roa.animIn(0);

	// Layer 0 ? Stocke dans une table
	rhPtr.activeToBackdrop(pHo, ((PARAM_SHORT)evtParams[0]).value, true);
    }
}
