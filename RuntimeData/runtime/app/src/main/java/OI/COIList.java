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
// COILIST : liste des OI de l'application
//
//----------------------------------------------------------------------------------
package OI;

import Banks.IEnum;
import Services.CChunk;
import Services.CFile;

public class COIList 
{
    public short oiMaxIndex;
    public COI ois[];
    public short oiMaxHandle;
    public short oiHandleToIndex[];
    public byte oiToLoad[];
    public byte oiLoaded[];
    int currentOI;
    
    public COIList() 
    {
    }
   
    public void preLoad(CFile file) 
    {
		// Alloue la table de OI
		oiMaxIndex=(short)file.readAInt();
		ois=new COI[oiMaxIndex];
		
		// Explore les chunks
		int index;
		oiMaxHandle=0;
		for (index=0; index<oiMaxIndex; index++)
		{
		    CChunk chk=new CChunk();
		    long posEnd;
		    while (chk.chID!=CChunk.CHUNK_LAST)
		    {
				chk.readHeader(file);
				if (chk.chSize==0)
				    continue;
				posEnd=file.getFilePointer()+chk.chSize;
				switch(chk.chID)
				{
				    // CHUNK_OBJINFOHEADER
				    case 0x4444:
						ois[index]=new COI();
						ois[index].loadHeader(file);
						if (ois[index].oiHandle>=oiMaxHandle)
						    oiMaxHandle=(short)(ois[index].oiHandle+1);
						break;
				    // CHUNK_OBJINFONAME
				    case 0x4445:
						ois[index].oiName=file.readAString();
						break;
				    // CHUNK_OBJECTSCOMMON
				    case 0x4446:
						ois[index].oiFileOffset=file.getFilePointer();
						break;
				}
				// Positionne a la fin du chunk
				file.seek(posEnd);
		    }
		}
		
		// Table OI To Handle
		oiHandleToIndex=new short[oiMaxHandle];
		for (index=0; index<oiMaxIndex; index++)
		{
		    oiHandleToIndex[ois[index].oiHandle] = (short)index;
		}
	
		// Tables de chargement
		oiToLoad=new byte[oiMaxHandle];
		oiLoaded=new byte[oiMaxHandle];
		int n;
		for (n=0; n<oiMaxHandle; n++)
		{
		    oiToLoad[n]=0;
		    oiLoaded[n]=0;
		}
    }
    public COI getOIFromHandle(short handle)
    {
    	return ois[oiHandleToIndex[handle]];
    }
    public COI getOIFromIndex(short index)
    {
	return ois[index];
    }
    
    // Exploration des OI de la frame courante
    public void resetOICurrent()
    {
		int n;
		for (n=0; n<oiMaxIndex; n++)
		{
		    ois[n].oiFlags&=~COI.OILF_CURFRAME;
		}
    }
    public void setOICurrent(int handle)
    {
    	ois[oiHandleToIndex[handle]].oiFlags|=COI.OILF_CURFRAME;
    }
    public COI getFirstOI()
    {
		int n;
		for (n=0; n<oiMaxIndex; n++)
		{
		    if ((ois[n].oiFlags&COI.OILF_CURFRAME)!=0)
		    {
				currentOI=n;
				return ois[n];
		    }
		}
		return null;
    }
    public COI getNextOI()
    {
		if (currentOI<oiMaxIndex)
		{
		    int n;
		    for (n=currentOI+1; n<oiMaxIndex; n++)
		    {
				if ((ois[n].oiFlags&COI.OILF_CURFRAME)!=0)
				{
				    currentOI=n;
				    return ois[n];
				}
		    }
		}
		return null;
    }
    
    // Chargement des OI
    public void resetToLoad()
    {
		int n;
		for (n=0; n<oiMaxHandle; n++)
		{
		    oiToLoad[n]=0;
		}
    }
    public void setToLoad(int n)
    {
    	oiToLoad[n]=1;
    }
    public void load(CFile file) 
    {
		int h;
		for (h=0; h<oiMaxHandle; h++)
		{
		    if (oiToLoad[h]!=0)
		    {
				if (oiLoaded[h]==0 || (oiLoaded[h]!=0 && (ois[oiHandleToIndex[h]].oiLoadFlags&COI.OILF_TORELOAD)!=0) )
				{
				    ois[oiHandleToIndex[h]].load(file);
				    oiLoaded[h]=1;
				}
		    }
		    else
		    {
				if (oiLoaded[h]!=0)
				{
				    ois[oiHandleToIndex[h]].unLoad();
				    oiLoaded[h]=0;
				}
		    }
		}
		resetToLoad();
    }
    public void enumElements(IEnum enumImages, IEnum enumFonts)
    {
		int h;
		for (h=0; h<oiMaxHandle; h++)
		{
		    if (oiLoaded[h]!=0)
		    {
		    	ois[oiHandleToIndex[h]].enumElements(enumImages, enumFonts);
		    }
		}
    }
}
