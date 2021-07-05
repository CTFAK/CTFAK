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
// ------------------------------------------------------------------------------
// 
// NO MORE OBJECTS
// 
// ------------------------------------------------------------------------------
package Conditions;

import Events.CQualToOiList;
import Objects.CObject;
import RunLoop.CObjInfo;
import RunLoop.CRun;

public class CND_EXTNOMOREOBJECT extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	// Correction bug jeux K&P
	if (hoPtr==null)
	{
	    return eva2(rhPtr);
	}
	if (evtOi>=0)
	{
	    if (hoPtr.hoOi!=evtOi)
		return false;
	    return true;
	}
	return evaNoMoreObject(rhPtr, 1);
    }
    public boolean eva2(CRun rhPtr)
    {
	return evaNoMoreObject(rhPtr, 0);
    }
    boolean evaNoMoreObject(CRun rhPtr, int sub)
    {
	short oil=evtOiList;

	CObjInfo poil;
	if (oil>=0)
	{
	    // Un objet normal
	    poil=rhPtr.rhOiList[oil];
	    if (poil.oilNObjects==0)
		return true;
	    return false;
	}

	// Un qualifier
	if (oil==-1)
	    return false;
	CQualToOiList pqoi=rhPtr.rhEvtProg.qualToOiList[oil&0x7FFF];
	int count=0;
	int qoi;
    int pqoi_qoiList_length = pqoi.qoiList.length;
	for (qoi=0; qoi<pqoi_qoiList_length; qoi+=2)
	{
	    poil=rhPtr.rhOiList[pqoi.qoiList[qoi+1]];
	    count+=poil.oilNObjects;
	}	 
	count-=sub;									//; Moins un si appel lors de killobject qualifier!
	if (count==0)
	    return true;
	return false;
    }
}
