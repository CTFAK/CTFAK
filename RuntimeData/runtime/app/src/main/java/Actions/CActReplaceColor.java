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
// REPLACE COLOR, routines
//
// -----------------------------------------------------------------------------
package Actions;

import Banks.CImage;
import Banks.IEnum;
import OI.COI;
import Objects.CObject;
import RunLoop.CRun;
import android.graphics.Bitmap;

/* TODO : implement with the C renderer */

public class CActReplaceColor implements IEnum
{
    int mode;
    int dwMax;
    short pImages[];
    CRun pRh;
    
    public void execute(CRun rhPtr, CObject pHo, int newColor, int oldColor)
    {
		// Changement des couleurs
		// ----------------------------------------------------------------------------
		pRh=rhPtr;
		short oi = pHo.hoOi;
		COI poi = rhPtr.rhApp.OIList.getOIFromHandle(oi);
		if (poi==null)
		    return;
		
		// Get image max
		dwMax = -1;
		mode=0;
		poi.enumElements(this, null);
	
		// Rechercher le premier
		CObject pHoFirst=pHo;
		while ((pHoFirst.hoNumPrev & 0x8000) == 0)
		    pHoFirst = rhPtr.rhObjectList[pHoFirst.hoNumPrev & 0x7FFF];
	
		// Parcourir la liste
		do 
		{
		    if ( pHoFirst.roc.rcImage!=-1 && pHoFirst.roc.rcImage>dwMax )
		    	dwMax = pHoFirst.roc.rcImage;
		    if ( pHoFirst.roc.rcOldImage!=-1 && pHoFirst.roc.rcOldImage>dwMax )
		    	dwMax = pHoFirst.roc.rcOldImage;
	
		    // Le dernier?
		    if ( (pHoFirst.hoNumNext & 0x8000) != 0 )
		    	break;
	
		    // Next OI
		    pHoFirst=rhPtr.rhObjectList[pHoFirst.hoNumNext];
	
		} while (true);
	
		// Allocate memory
		pImages=new short[dwMax+1];
		int n;
		for (n=0; n<dwMax+1; n++)
		{
		    pImages[n]=-1;
		}
	
		// List all images
		mode=1;
		poi.enumElements(this, null);
	
		// Replace color in all images and create new images
		int i;
		short newImg;
		for (i=0; i<=dwMax; i++)
		{
		    if ( pImages[i] == -1 )
			continue;
	
		    CImage sourceImg = rhPtr.rhApp.imageBank.getImageFromHandle((short)i);

            Bitmap result = null;

            
		    if (result!=null)
		    {
		    	// Create new image in the bank

		    	newImg = rhPtr.rhApp.imageBank.addImage
		    	    (result, (short) sourceImg.getXSpot (), (short) sourceImg.getYSpot (),
                            (short) sourceImg.getXAP (), (short) sourceImg.getYAP (), sourceImg.getResampling());

		    	pImages[i] = newImg;
		    }
		}
	
		// Remplacer images dans les objets de meme OI
		pHoFirst=pHo;
		while ((pHoFirst.hoNumPrev & 0x8000) == 0)
		    pHoFirst = rhPtr.rhObjectList[pHoFirst.hoNumPrev & 0x7FFF];
	
		// Parcourir la liste
		do 
		{
		    if ( pHoFirst.roc.rcImage!=-1 && pImages[pHoFirst.roc.rcImage]!=-1 )
		    {
		    	pHoFirst.roc.rcImage = pImages[pHoFirst.roc.rcImage];
		    }
		    if ( pHoFirst.roc.rcOldImage!=-1 && pImages[pHoFirst.roc.rcOldImage]!=-1 )
		    {
		    	pHoFirst.roc.rcOldImage = pImages[pHoFirst.roc.rcOldImage];
		    }
		    if ( pHoFirst.roc.rcSprite != null )
		    {
		    	rhPtr.spriteGen.modifSprite(pHoFirst.roc.rcSprite, pHoFirst.hoX-rhPtr.rhWindowX, pHoFirst.hoY-rhPtr.rhWindowY, pHoFirst.roc.rcImage);
		    }
	
		    // Le dernier?
		    if ( (pHoFirst.hoNumNext & 0x8000) != 0 )
		    	break;
		    // Next OI
		    pHoFirst=rhPtr.rhObjectList[pHoFirst.hoNumNext];
		    
		} while (true);
	
		mode=2;
		poi.enumElements(this, null);
	
		// Replace old images by new ones
		mode=3;		
		poi.enumElements(this, null);
	
		// Mark OI to reload
		poi.oiLoadFlags |= COI.OILF_TORELOAD;
	
		// Force le redraw
		pHo.roc.rcChanged = true;  
    }
    
    @Override
	public short enumerate(short num)
    {
		switch (mode)
		{
		    // Comptage des images
		    case 0:
				if (num>dwMax)
				    dwMax=num;
				return -1;
		    // Enumeration des images
		    case 1:
				pImages[num]=1;
				return -1;
		    // Destruction des images
		    case 2:
				//pRh.rhApp.imageBank.delImage(num);
				return -1;
		    // Incrementation des usecount, remplacement des images
		    case 3:
				CImage image=pRh.rhApp.imageBank.getImageFromHandle(pImages[num]);
				//image.incUseCount ();
				return pImages[num];		
		}         
		return -1;
    }

}
