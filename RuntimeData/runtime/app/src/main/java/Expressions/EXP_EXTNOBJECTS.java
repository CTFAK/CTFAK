/* Copyright (c) 1996-2013 Clickteam
*
* This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
* 
* Permission is hereby granted to any person obtaining a legal copy 
* of Clickteam Multimedia Fusion 2 to use or modify this source code for 
* debugging, optimizing, or customizing applications created with 
* Clickteam Multimedia Fusion 2. 
* Any other use of this source code is prohibited.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
* IN THE SOFTWARE.
*/
//----------------------------------------------------------------------------------
//
// NUMBER OF OBJECTS
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Events.*;

public class EXP_EXTNOBJECTS extends CExpOi
{
    public void evaluate(CRun rhPtr)
    {        
	// Cherche dans la liste des oi
	short qoil=oiList;
	CObjInfo poil;
	if (qoil>=0)
	{
	    // Un OI Normal
	    poil=rhPtr.rhOiList[qoil];
	    rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(poil.oilNObjects);
	}
	else
	{
	    // Un qualifier
	    int count=0;
	    if (qoil!=-1)
	    {
		CQualToOiList pqoi=rhPtr.rhEvtProg.qualToOiList[qoil&0x7FFF];
		int qoi;
		for (qoi=0; qoi<pqoi.qoiList.length; qoi+=2)
		{
		    poil=rhPtr.rhOiList[pqoi.qoiList[qoi+1]];
		    count+=poil.oilNObjects;
		}
	    }
	    rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(count);
	}
    }    
}
