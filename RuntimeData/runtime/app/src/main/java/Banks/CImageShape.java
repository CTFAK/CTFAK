/* Copyright (c) 1996-2015 Clickteam
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
// CImageShape : stores the shape of an image for the physics engine
//
//----------------------------------------------------------------------------------
package Banks;

import Services.CFile;
import Sprites.CMask;

public class CImageShape
{
    public int[] xArray;
    public int[] yArray;
    public short image;
    public int count;

	public CImageShape()
	{
		image = 0;
		count = 0;
	}

    public void load(CFile f)
    {
		image = (short)f.readAInt();		// image handle
		count = f.readAInt();				// point count
		xArray = null;
		yArray = null;
		if ( count != 0 )
		{
			int i;
			xArray = new int[count];
			yArray = new int[count];
			for (i=0; i<count; i++)
			{
				xArray[i] = f.readAInt();
				yArray[i] = f.readAInt();
			}
		}
	}

    public void CreateShape(CImageBank bank, short img)
    {
	    CImage imageObject = bank.getImageFromHandle(img);
		CMask pMask = imageObject.getMask(0, 0, 1.0, 1.0);
		image = img;
		int width = pMask.getWidth();
		int height = pMask.getHeight();
		count = 0;
		xArray = new int[10];
		yArray = new int[10];

		int x, y, xPrevious, yPrevious;
		int xPos = 0, yPos = 0;

		// Right - bottom
		for (y=height-1, xPos=-1; y>=0; y--)
		{
			for (x=width-1; x>=0; x--)
			{
				if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
				{
					if (x>xPos)
					{
						xPos=x;
						yPos=y;
					}
					break;
				}
			}
		}
		if ( xPos >= 0 )
		{
			xPrevious=xArray[count]=xPos;
			yPrevious=yArray[count]=yPos;
			count++;

			// Right - top
			for (y=0, xPos=-1; y<height; y++)
			{
				for (x=width-1; x>=0; x--)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (x>xPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			CPointTest angle = new CPointTest();
			angle.angle=1000;
			int c;
			if (this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
			{
				for (c = 0; c < count; c++)
				{
					if (xArray[c] == xPos && yArray[c] == yPos)
						break;
				}
				if (c == count)
				{
					xPrevious=xArray[count]=xPos;
					yPrevious=yArray[count++]=yPos;
				}
			}

			// Top - right
			for (x=width-1, yPos=10000; x>=0; x--)
			{
				for (y=0; y<height; y++)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (y<yPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			for (c = 0; c < count; c++)
			{
				if (xArray[c] == xPos && yArray[c] == yPos)
					break;
			}
			if (c == count)
			{
				if (!this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
					count--;
				xPrevious=xArray[count]=xPos;
				yPrevious=yArray[count++]=yPos;
			}
			// Top - left
			for (x=0, yPos=10000; x<width; x++)
			{
				for (y=0; y<height; y++)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (y<yPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			for (c = 0; c < count; c++)
			{
				if (xArray[c] == xPos && yArray[c] == yPos)
					break;
			}
			if (c == count)
			{
				if (!this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
					count--;
				xPrevious=xArray[count]=xPos;
				yPrevious=yArray[count++]=yPos;
			}
			// Left - top
			for (y=0, xPos=10000; y<height; y++)
			{
				for (x=0; x<width; x++)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (x<xPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			for (c = 0; c < count; c++)
			{
				if (xArray[c] == xPos && yArray[c] == yPos)
					break;
			}
			if (c == count)
			{
				if (!this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
					count--;
				xPrevious=xArray[count]=xPos;
				yPrevious=yArray[count++]=yPos;
			}
			// Left - bottom
			for (y=height-1, xPos=10000; y>=0; y--)
			{
				for (x=0; x<width; x++)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (x<xPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			for (c = 0; c < count; c++)
			{
				if (xArray[c] == xPos && yArray[c] == yPos)
					break;
			}
			if (c == count)
			{
				if (!this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
					count--;
				xPrevious=xArray[count]=xPos;
				yPrevious=yArray[count++]=yPos;
			}
			// Bottom - left
			for (x=0, yPos=-1; x<width; x++)
			{
				for (y=height-1; y>=0; y--)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (y>yPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			for (c = 0; c < count; c++)
			{
				if (xArray[c] == xPos && yArray[c] == yPos)
					break;
			}
			if (c == count)
			{
				if (!this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
					count--;
				xPrevious=xArray[count]=xPos;
				yPrevious=yArray[count++]=yPos;
			}
			// Bottom - right
			for (x=width-1, yPos=-1; x>=0; x--)
			{
				for (y=height-1; y>=0; y--)
				{
					if (pMask.testPoint(x, y))		// this.isPoint(pMask, x, y)
					{
						if (y>yPos)
						{
							xPos=x;
							yPos=y;
						}
						break;
					}
				}
			}
			for (c = 0; c < count; c++)
			{
				if (xArray[c] == xPos && yArray[c] == yPos)
					break;
			}
			if (c == count)
			{
				if (!this.PointOK(xPos, yPos, xPrevious, yPrevious, angle))
					count--;
				xArray[count]=xPos;
				yArray[count++]=yPos;
			}
		}
    }

    private boolean PointOK(int xNew, int yNew, int xOld, int yOld, CPointTest angle)
    {
        int deltaX=xNew-xOld;
        int deltaY=yNew-yOld;
        float a=angle.angle;
        angle.angle=(float)(Math.atan2(deltaY, deltaX)*57.2957795);
        if (a==angle.angle)
            return false;
        return true;
    }
}
class CPointTest
{
    public float angle;
}
