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
// CFONTBANK :banque de fontes
//
//----------------------------------------------------------------------------------
package Banks;

import Services.CFile;
import Services.CFontInfo;

public class CFontBank implements IEnum
{
    CFile file;
    public CFont fonts[]=null;          /// List of CFonts
    int offsetsToFonts[];
    int nFonts=0;
    short handleToIndex[];
    int maxHandlesReel=0;
    int maxHandlesTotal=0;
    short useCount[];
    CFont nullFont=null;
    
    public CFontBank() 
    {
    }
    public void preLoad(CFile file)
    {
		// Nombre d'elements
		int number=file.readAInt();
		int n;
		
		// Explore les handles
		maxHandlesReel=0;
		long debut=file.getFilePointer();
		CFont temp=new CFont();
		for (n=0; n<number; n++)
		{
		    temp.loadHandle(file);
		    maxHandlesReel=Math.max(maxHandlesReel, temp.handle+1);
		}
		file.seek(debut);
		offsetsToFonts=new int[maxHandlesReel];
		for (n=0; n<number; n++)
		{
		    debut=file.getFilePointer();
		    temp.loadHandle(file);
		    offsetsToFonts[temp.handle]=(int)debut;
		}	    
		useCount=new short[maxHandlesReel];
		resetToLoad();
		handleToIndex=null;
		maxHandlesTotal=maxHandlesReel;
		nFonts=0;
		fonts=null;
    }
    public void load(CFile file)
    {
		int n;
		nFonts=0;
		for (n=0; n<maxHandlesReel; n++)
		{
		    if (useCount[n]!=0)
		    {
		    	nFonts++;
		    }
		}
		
		CFont newFonts[]=new CFont[nFonts];
		int count=0;
		int h;
		for (h=0; h<maxHandlesReel; h++)
		{
		    if (useCount[h]!=0)
		    {
				if (fonts!=null && handleToIndex[h]!=-1 && fonts[handleToIndex[h]]!=null)
				{
				    newFonts[count]=fonts[handleToIndex[h]];
				    newFonts[count].useCount=useCount[h];
				}
				else
				{
				    newFonts[count]=new CFont();
				    file.seek(offsetsToFonts[h]);
				    newFonts[count].load(file);
				    newFonts[count].useCount=useCount[h];
				}
				count++;
		    }
		}
		fonts=newFonts;
	
		// Cree la table d'indirection
		handleToIndex=new short[maxHandlesReel];
		for (n=0; n<maxHandlesReel; n++)
		{
		    handleToIndex[n]=-1;
		}
		for (n=0; n<nFonts; n++)
		{
		    handleToIndex[fonts[n].handle]=(short)n;
		}
		maxHandlesTotal=maxHandlesReel;
		
		// Plus rien a charger
		resetToLoad();	
    }
    public CFont getFontFromHandle(short handle)
    {
		// Protection jeux niques
		if (handle==-1)
		{
		    return nullFont;
		}
		// Retourne la fonte
		if (handle>=0 && handle<maxHandlesTotal)
		    if (handleToIndex[handle]!=-1)
		    	return fonts[handleToIndex[handle]];
		return null;
    }
    public CFont getFontFromIndex(short index)
    {
		if (index>=0 && index<nFonts)
		    return fonts[index];
		return null;
    }
    public CFontInfo getFontInfoFromHandle(short handle)
    {
		CFont font=getFontFromHandle(handle);
		return font.getFontInfo();
    }
    public void resetToLoad()
    {
		int n;
		for (n=0; n<maxHandlesReel; n++)
		{
		    useCount[n]=0;
		}
    }
    public void setToLoad(short handle)
    {
		// Protection jeux niques
		if (handle==-1)
		{
		    if (nullFont==null)
		    {
				nullFont=new CFont();
				nullFont.createDefaultFont();		
		    }
		    return;
		}
		useCount[handle]++;
    }
    
    // Entree enumeration
    @Override
	public short enumerate(short num)
    {
		setToLoad(num);
		return -1;
    }
    
    public short addFont(CFontInfo info)
    {
		int h;
		
		// Cherche une fonte identique
		int n;
		for (n=0; n<nFonts; n++)
		{
		    if (fonts[n]==null) continue;
		    if (fonts[n].lfHeight!=info.lfHeight) continue;
		    if (fonts[n].lfWeight!=info.lfWeight) continue; 
		    if (fonts[n].lfItalic!=info.lfItalic) continue; 
		    if (fonts[n].lfUnderline!=info.lfUnderline) continue;
		    if (fonts[n].lfStrikeOut!=info.lfStrikeOut) continue; 
		    if (fonts[n].lfFaceName.equalsIgnoreCase(info.lfFaceName)==false) continue;
		    break;
		}
		if (n<nFonts)
		{
		    return fonts[n].handle;
		}
		
		// Cherche un handle libre
		short hFound=-1;
		for (h=maxHandlesReel; h<maxHandlesTotal; h++)
		{
		    if (handleToIndex[h]==-1)
		    {
				hFound=(short)h;
				break;
		    }		
		}
	
		// Rajouter un handle
		if (hFound==-1)
		{
		    short newHToI[]=new short[maxHandlesTotal+10];
		    for (h=0; h<maxHandlesTotal; h++)
		    {
		    	newHToI[h]=handleToIndex[h];
		    }
		    for (; h<maxHandlesTotal+10; h++)
		    {
		    	newHToI[h]=-1;
		    }
		    hFound=(short)maxHandlesTotal;
		    maxHandlesTotal+=10;
		    handleToIndex=newHToI;
		}
		
		// Cherche une fonte libre
		int f;
		int fFound=-1;
		for (f=0; f<nFonts; f++)
		{
		    if (fonts[f]==null)
		    {
		    	fFound=f;
		    	break;
		    }
		}		
		
		// Rajouter une image?
		if (fFound==-1)
		{
		    CFont newFonts[]=new CFont[nFonts+10];
		    for (f=0; f<nFonts; f++)
		    {
		    	newFonts[f]=fonts[f];
		    }
		    for (; f<nFonts+10; f++)
		    {
		    	newFonts[f]=null;
		    }
		    fFound=nFonts;
		    nFonts+=10;
		    fonts=newFonts;
		}
		
		// Ajoute la nouvelle image
		handleToIndex[hFound]=(short)fFound;
		fonts[fFound]=new CFont();
		fonts[fFound].handle=hFound;
		fonts[fFound].lfHeight=info.lfHeight; 
		fonts[fFound].lfWeight=info.lfWeight; 
		fonts[fFound].lfItalic=info.lfItalic; 
		fonts[fFound].lfUnderline=info.lfUnderline; 
		fonts[fFound].lfStrikeOut=info.lfStrikeOut; 
		fonts[fFound].lfFaceName=new String(info.lfFaceName);
			
		return hFound;
    }
}
