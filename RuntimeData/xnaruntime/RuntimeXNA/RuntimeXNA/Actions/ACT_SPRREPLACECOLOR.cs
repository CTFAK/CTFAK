// -----------------------------------------------------------------------------
//
// REPLACE COLOR
//
// -----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Banks;
using RuntimeXNA.Services;
using RuntimeXNA.OI;
using Microsoft.Xna.Framework;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SPRREPLACECOLOR:CAct, IEnum
	{
		internal int mode=0;
		internal int dwMax=0;
		internal short[] pImages=null;
		internal CRun pRh=null;
		
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;

			// Un cran d'animation sans effet
			pHo.roa.animIn(0);
			
			// Recupere parametres
			int oldColor;
			if (evtParams[0].code == 24)
			// PARAM_COLOUR)
				oldColor = ((PARAM_COLOUR) evtParams[0]).color;
			else
			{
				oldColor = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				oldColor = CServices.swapRGB(oldColor);
			}
			
			int newColor;
			if (evtParams[1].code == 24)
			// PARAM_COLOUR)
				newColor = ((PARAM_COLOUR) evtParams[1]).color;
			else
			{
				newColor = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
				newColor = CServices.swapRGB(newColor);
			}
			
			// Changement des couleurs
			// ----------------------------------------------------------------------------
			pRh = rhPtr;
			short oi = pHo.hoOi;
			COI poi = rhPtr.rhApp.OIList.getOIFromHandle(oi);
			if (poi == null)
				return ;
			
			// Get image max
			dwMax = - 1;
			mode = 0;
			poi.enumElements(this, null);
			
			// Rechercher le premier
			CObject pHoFirst = pHo;
			while ((pHoFirst.hoNumPrev & 0x8000) == 0)
				pHoFirst = rhPtr.rhObjectList[pHoFirst.hoNumPrev & 0x7FFF];
			
			// Parcourir la liste
			do 
			{
				if (pHoFirst.roc.rcImage != - 1 && pHoFirst.roc.rcImage > dwMax)
					dwMax = pHoFirst.roc.rcImage;
				if (pHoFirst.roc.rcOldImage != - 1 && pHoFirst.roc.rcOldImage > dwMax)
					dwMax = pHoFirst.roc.rcOldImage;
				
				// Le dernier?
				if ((pHoFirst.hoNumNext & 0x8000) != 0)
					break;
				
				// Next OI
				pHoFirst = rhPtr.rhObjectList[pHoFirst.hoNumNext];
			}
			while (true);
			
			// Allocate memory
			pImages = new short[dwMax + 1];
            int n;
			for (n = 0; n < dwMax + 1; n++)
			{
				pImages[n] = - 1;
            }
			
			// List all images
			mode = 1;
			poi.enumElements(this, null);
			
			// Replace color in all images and create new images
			int i;
			short newImg;
            Texture2D texture = null;
			for (i = 0; i <= dwMax; i++)
			{
				if (pImages[i] == - 1)
					continue;
				
                CImage sourceImg=rhPtr.rhApp.imageBank.getImageFromHandle((short) i);
                int width = sourceImg.width;
                int height = sourceImg.height;
                Color[] pixels = new Color[width * height];
                if (sourceImg.mosaic == 0)
                {                    
                    sourceImg.image.GetData(pixels);
                    CServices.replaceColor(rhPtr.rhApp, pixels, width, height, oldColor, newColor);
                    Texture2D texture2 = new Texture2D(rhPtr.rhApp.spriteBatch.GraphicsDevice, width, height);
                    texture2.SetData(pixels);
                    newImg = rhPtr.rhApp.imageBank.addImage(texture2, sourceImg.xSpot, sourceImg.ySpot, sourceImg.xAP, sourceImg.yAP, (short)0);
                    pImages[i] = newImg;
                }
                else
                {
                    texture = rhPtr.rhApp.imageBank.mosaics[sourceImg.mosaic];
                    texture.GetData(0, sourceImg.mosaicRectangle, pixels, 0, width*height);
                    CServices.replaceColor(rhPtr.rhApp, pixels, width, height, oldColor, newColor);
                    Texture2D texture2 = new Texture2D(rhPtr.rhApp.spriteBatch.GraphicsDevice, width, height);
                    texture2.SetData(pixels);
                    newImg = rhPtr.rhApp.imageBank.addImage(texture2, sourceImg.xSpot, sourceImg.ySpot, sourceImg.xAP, sourceImg.yAP, (short)0);
                    pImages[i] = newImg;
                }
			}
			
			pHoFirst = pHo;
			while ((pHoFirst.hoNumPrev & 0x8000) == 0)
				pHoFirst = rhPtr.rhObjectList[pHoFirst.hoNumPrev & 0x7FFF];
			
			// Parcourir la liste
			do 
			{
				if (pHoFirst.roc.rcImage != - 1 && pImages[pHoFirst.roc.rcImage] != - 1)
				{
					pHoFirst.roc.rcImage = pImages[pHoFirst.roc.rcImage];
				}
				if (pHoFirst.roc.rcOldImage != - 1 && pImages[pHoFirst.roc.rcOldImage] != - 1)
				{
					pHoFirst.roc.rcOldImage = pImages[pHoFirst.roc.rcOldImage];
				}
				if (pHoFirst.roc.rcSprite != null)
				{
					rhPtr.rhApp.spriteGen.modifSprite(pHoFirst.roc.rcSprite, pHoFirst.hoX - rhPtr.rhWindowX, pHoFirst.hoY - rhPtr.rhWindowY, pHoFirst.roc.rcImage);
				}
				
				// Le dernier?
				if ((pHoFirst.hoNumNext & 0x8000) != 0)
					break;
				// Next OI
				pHoFirst = rhPtr.rhObjectList[pHoFirst.hoNumNext];
			}
			while (true);
			
			mode = 2;
			poi.enumElements(this, null);
			
			// Replace old images by new ones
			mode = 3;
			poi.enumElements(this, null);
			
			// Mark OI to reload
			poi.oiLoadFlags |= COI.OILF_TORELOAD;
			
			// Force le redraw
			pHo.roc.rcChanged = true;
		}
		
		public virtual short enumerate(short num)
		{
			switch (mode)
			{
				
				// Comptage des images
				case 0: 
					if (num > dwMax)
						dwMax = num;
					return - 1;

				// Enumeration des images				
				case 1: 
					pImages[num] = 1;
					return - 1;

				// Destruction des images				
				case 2:
                    if (pImages[num]>=0)
                        pRh.rhApp.imageBank.delImage(num);
					return - 1;

				// Incrementation des usecount, remplacement des images				
				case 3:
                    if (pImages[num] >= 0)
                    {
                        CImage image = pRh.rhApp.imageBank.getImageFromHandle(pImages[num]);
                        image.useCount++;
                        return pImages[num];
                    }
                    break;
				}
 			return - 1;
		}
	}
}