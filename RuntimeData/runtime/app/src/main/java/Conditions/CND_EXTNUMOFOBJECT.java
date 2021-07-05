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
// COMPARE TO NUMBER OF OBJECTS
// 
// ------------------------------------------------------------------------------
package Conditions;

import Events.CQualToOiList;
import Objects.CObject;
import Params.CParamExpression;
import RunLoop.CObjInfo;
import RunLoop.CRun;

public class CND_EXTNUMOFOBJECT extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
	return eva2(rhPtr);        
    }
    public boolean eva2(CRun rhPtr)
    {
	int count=0;

	CObjInfo poil;
	short oil=evtOiList;
	if (oil>=0)
	{
	    // Un objet normal
	    poil=rhPtr.rhOiList[oil];
	    count=poil.oilNObjects;
	}
	else
	{
	    // Un qualifier
	    if (oil!=-1)
	    {
		CQualToOiList pqoi=rhPtr.rhEvtProg.qualToOiList[oil&0x7FFF];
		int qoi;
	    int pqoi_qoiList_length = pqoi.qoiList.length;
		for (qoi=0; qoi<pqoi_qoiList_length; qoi+=2)
		{
		    poil=rhPtr.rhOiList[pqoi.qoiList[qoi+1]];
		    count+=poil.oilNObjects;
		}
	    }
	}

	int value=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
	return CRun.compareTer(count, value, ((CParamExpression)evtParams[0]).comparaison);
    }
}
