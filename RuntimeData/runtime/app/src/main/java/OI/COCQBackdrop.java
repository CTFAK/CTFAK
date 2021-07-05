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
// COBJECTCOMMON : Donn�es d'un objet normal
//
//----------------------------------------------------------------------------------
package OI;

import Banks.CImage;
import Banks.CImageBank;
import Banks.IEnum;
import OpenGL.GLRenderer;
import Services.CFile;
import Sprites.CMask;
import Sprites.CSprite;
import Sprites.IDrawable;

public class COCQBackdrop extends COC implements IDrawable
{
	public static final short FILLTYPE_NONE=0;
	public static final short FILLTYPE_SOLID=1;
	public static final short FILLTYPE_GRADIENT=2;
	public static final short FILLTYPE_MOTIF=3;

	public static final short SHAPE_NONE=0;
	public static final short SHAPE_LINE=1;
	public static final short SHAPE_RECTANGLE=2;
	public static final short SHAPE_ELLIPSE=3;

	public static final short GRADIENT_HORIZONTAL=0;
	public static final short GRADIENT_VERTICAL=1;

	public static final short LINEF_INVX = 0x0001;
	public static final short LINEF_INVY = 0x0002;

	public short ocBorderSize;			// Border
	public int ocBorderColor;
	public short ocShape;			// Shape
	public short ocFillType;
	public short ocLineFlags;			// Only for lines in non filled mode
	public int ocColor1;			// Gradient
	public int ocColor2;
	public int ocGradientFlags;
	public short ocImage;				// Image
	public COI pCOI;

	public COCQBackdrop()
	{
	}

	@Override
	public void load(CFile file, short type, COI pOi)
	{
		pCOI = pOi;

		file.skipBytes(4);		// ocDWSize
		ocObstacleType = file.readAShort();
		ocColMode = file.readAShort();
		ocCx = file.readAInt();
		ocCy = file.readAInt();
		ocBorderSize = file.readAShort();
		ocBorderColor = file.readAColor();
		ocShape = file.readAShort();

		ocFillType = file.readAShort();
		if (ocShape == 1)		// SHAPE_LINE
		{
			ocLineFlags = file.readAShort();
		}
		else
		{
			switch (ocFillType)
			{
			case 1:			    // FILLTYPE_SOLID
				ocColor1 = ocColor2 = file.readAColor();
				break;
			case 2:			    // FILLTYPE_GRADIENT
				ocColor1 = file.readAColor();
				ocColor2 = file.readAColor();
				ocGradientFlags = file.readAInt();
				break;
			case 3:			    // FILLTYPE_IMAGE
				ocImage = file.readAShort();
				break;
			}
		}
	}

	@Override
	public void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		if (ocFillType == 3)		    // FILLTYPE_IMAGE
		{
			if (enumImages != null)
			{
				short num = enumImages.enumerate(ocImage);
				if (num != -1)
				{
					ocImage = num;
				}
			}
		}
	}

	@Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
	{
		int cx = ocCx;
		int cy = ocCy;

		CImage image;
		//  boolean bFilled = false;
		synchronized(GLRenderer.inst)
		{
			switch (ocFillType) {
			case FILLTYPE_MOTIF:

				image = bank.getImageFromHandle(ocImage);

				switch (ocShape) {
				case SHAPE_RECTANGLE:

					GLRenderer.inst.renderPattern(image, x, y, cx, cy, pCOI.oiInkEffect, pCOI.oiInkEffectParam);
					break;

				case SHAPE_ELLIPSE:

					GLRenderer.inst.renderPatternEllipse(image, x, y, cx, cy, pCOI.oiInkEffect, pCOI.oiInkEffectParam);
					break;
				}

				break;

			case FILLTYPE_GRADIENT:
			case FILLTYPE_SOLID:

				if(ocShape == SHAPE_RECTANGLE)
				{
					GLRenderer.inst.renderGradient
					(x, y, cx, cy, ocColor1, ocColor2, (ocGradientFlags & GRADIENT_VERTICAL) != 0,
					pCOI.oiInkEffect, pCOI.oiInkEffectParam);
				}
				else if(ocShape == SHAPE_ELLIPSE)
				{
					GLRenderer.inst.renderGradientEllipse
					(x, y, cx, cy, ocColor1, ocColor2, (ocGradientFlags & GRADIENT_VERTICAL) != 0,
					pCOI.oiInkEffect, pCOI.oiInkEffectParam);
				}

				break;
			}

			if (ocBorderSize > 0)
			{
				switch (ocShape)
				{
				// SHAPE_LINE
				case 1:
					if ((ocLineFlags & LINEF_INVX) != 0)
					{
						x += cx;
						cx = -cx;
					}
					if ((ocLineFlags & LINEF_INVY) != 0)
					{
						y += cy;
						cy = -cy;
					}
					GLRenderer.inst.renderLine(x, y, x + cx, y + cy, ocBorderColor, ocBorderSize);
					break;
					// SHAPE_RECTANGLE
				case 2:
					GLRenderer.inst.renderRect(x, y, cx, cy, ocBorderColor, ocBorderSize);
					break;
					// SHAPE_ELLIPSE
				case 3:
					/* TODO */
					break;
				}
			}
		}
	}

	@Override
	public CMask spriteGetMask()
	{
		return null;
	}

}
