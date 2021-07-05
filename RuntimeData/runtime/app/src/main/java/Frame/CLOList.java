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
//----------------------------------------------------------------------------------
//
// CLOLIST : liste de levelobjects
//
//----------------------------------------------------------------------------------
package Frame;

import Application.CRunApp;
import OI.COI;

public class CLOList 
{
	public CLO list[];
	public short handleToIndex[];
	public int nIndex;
	int loFranIndex;

	public CLOList() 
	{
	}

	public void load(CRunApp app)
	{
		nIndex=app.file.readAInt();
		list=new CLO[nIndex];
		int n;
		short maxHandles=0;
		for (n=0; n<nIndex; n++)
		{ 
			list[n]=new CLO();
			list[n].load(app.file);
			if (list[n].loHandle+1>maxHandles)
				maxHandles=(short)(list[n].loHandle+1);
			COI pOI=app.OIList.getOIFromHandle(list[n].loOiHandle);
			list[n].loType=pOI.oiType;
		}
		handleToIndex=new short[maxHandles];
		for (n=0; n<nIndex; n++)
		{
			handleToIndex[list[n].loHandle]=(short)n;
		}
	}
	public CLO getLOFromIndex(short index)
	{
		return list[index];
	}
	public CLO getLOFromHandle(short handle)
	{
		if(handle < handleToIndex.length)
			return list[handleToIndex[handle]];

		return null;
	}

	// Get next LevObj
	public CLO next_LevObj()
	{
		CLO plo;

		if ( loFranIndex < nIndex )
		{
			do
			{
				plo = list[loFranIndex++];
				if ( plo.loType>=COI.OBJ_SPR )
					return plo;
			}while(loFranIndex<nIndex);
		}
		return null;
	}

	// Get first levObj address
	public CLO first_LevObj ()
	{
		loFranIndex=0;
		return next_LevObj();                     
	}
}
