//----------------------------------------------------------------------------------
//
// CMASK : un masque
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Banks;
using RuntimeXNA.Application;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RuntimeXNA.Sprites
{
    public class CMask
    {
        public ushort[] mask = null;
        public int lineWidth;
        public int height;
        public int width;
        public int xSpot = 0;
        public int ySpot = 0;
        public const int SCMF_FULL = 0x0000;
        public const int SCMF_PLATFORM = 0x0001;
        public const int GCMF_OBSTACLE = 0x0000;
        public const int GCMF_PLATFORM = 0x0001;

        static ushort[] lMask =
        {
        (ushort) 0xFFFF, // 1111111111111111B
        (ushort) 0x7FFF, // 0111111111111111B
        (ushort) 0x3FFF, // 0011111111111111B
        (ushort) 0x1FFF, // 0001111111111111B
        (ushort) 0x0FFF, // 0000111111111111B
        (ushort) 0x07FF, // 0000011111111111B
        (ushort) 0x03FF, // 0000001111111111B
        (ushort) 0x01FF, // 0000000111111111B
        (ushort) 0x00FF, // 0000000011111111B
        (ushort) 0x007F, // 0000000001111111B
        (ushort) 0x003F, // 0000000000111111B
        (ushort) 0x001F, // 0000000000011111B
        (ushort) 0x000F, // 0000000000001111B
        (ushort) 0x0007, // 0000000000000111B
        (ushort) 0x0003, // 0000000000000011B
        (ushort) 0x0001	// 0000000000000001B
        };
        static ushort[] rMask =
        {
        (ushort) 0x0000, // 1000000000000000B0
        (ushort) 0x8000, // 1000000000000000B0
        (ushort) 0xC000, // 1100000000000000B1
        (ushort) 0xE000, // 1110000000000000B2
        (ushort) 0xF000, // 1111000000000000B3
        (ushort) 0xF800, // 1111100000000000B4
        (ushort) 0xFC00, // 1111110000000000B5
        (ushort) 0xFE00, // 1111111000000000B6
        (ushort) 0xFF00, // 1111111100000000B7
        (ushort) 0xFF80, // 1111111110000000B8
        (ushort) 0xFFC0, // 1111111111000000B9
        (ushort) 0xFFE0, // 1111111111100000B10
        (ushort) 0xFFF0, // 1111111111110000B11
        (ushort) 0xFFF8, // 1111111111111000B12
        (ushort) 0xFFFC, // 1111111111111100B13
        (ushort) 0xFFFE, // 1111111111111110B14
        (ushort) 0xFFFF	// 1111111111111111B15
        };


        public void createMask(CImage image, int nFlags)
        {
            width = image.width;
            height = image.height;
            xSpot = image.xSpot;
            ySpot = image.ySpot;

            int[] pixels=new int[width*height];
            Texture2D texture = image.image;
            Nullable<Rectangle> sourceRect = null;
            if (image.mosaic != 0)
            {
                texture = image.app.imageBank.mosaics[image.mosaic];
                sourceRect = image.mosaicRectangle;
            }
            texture.GetData(0, sourceRect, pixels, 0, width*height);

            int maskWidth = (int)(((width + 15) & 0xFFFFFFF0) / 16);
            mask = new ushort[maskWidth * height + 1];
            lineWidth = maskWidth;
            int x, y;
            for (x = 0; x < maskWidth * height + 1; x++)
            {
                mask[x] = 0;
            }

            int s;
            ushort bm;
            if ((nFlags & GCMF_PLATFORM) == 0)
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        s = (int)((y * maskWidth) + (x & 0xFFFFFFF0) / 16);
                        if ((pixels[y * width + x] & 0xFF000000) != 0)
                        {
                            bm = (ushort) (0x8000 >> (x % 16));
                            mask[s] |= bm;
                        }
                    }
                }
            }
            else
            {
                int endY;
                for (x = 0; x < width; x++)
                {
                    for (y = 0; y < height; y++)
                    {
                        if ((pixels[y * width + x] & 0xFF000000) != 0)
                        {
                            break;
                        }
                    }
                    if (y < height)
                    {
                        endY = Math.Min(height, y + CColMask.HEIGHT_PLATFORM);
                        bm = (ushort) (0x8000 >> (x & 15));
                        for (; y < endY; y++)
                        {
                            if ((pixels[y * width + x] & 0xFF000000) != 0)
                            {
                                s = (y * maskWidth) + x / 16;
                                mask[s] |= bm;
                            }
                        }
                    }
                }
            }
        }

        void rotateRect(ref int pWidth, ref int pHeight, ref int pHX, ref int pHY, double fAngle)
        {
	        double x, y;	// , xo, yo;
	        double cosa, sina;
        	
	        if ( fAngle == 90.0 )
	        {
		        cosa = 0.0;
		        sina = 1.0;
	        }
	        else if ( fAngle == 180.0 )
	        {
		        cosa = -1.0;
		        sina = 0.0;
	        }
	        else if ( fAngle == 270.0 )
	        {
		        cosa = 0.0;
		        sina = -1.0;
	        }
	        else
	        {
		        double arad = fAngle * 0.017453292;     // _PI / 180.0;
		        cosa = Math.Cos(arad);
		        sina = Math.Sin(arad);
	        }
        	
	        // Rotate top-left point
	        int topLeftX, topLeftY;
        	
	        // Ditto, optimized
	        double nhxcos;
	        double nhxsin;
	        double nhycos;
	        double nhysin;
	        nhxcos = -pHX * cosa;
	        nhxsin = -pHX * sina;
	        nhycos = -pHY * cosa;
	        nhysin = -pHY * sina;
	        topLeftX = (int)(nhxcos + nhysin);
	        topLeftY = (int)(nhycos - nhxsin);
        	
	        // Rotate top-right point
	        int topRightX, topRightY;
        	
	        // Ditto, optimized
	        x = (double)(pWidth - pHX);
	        nhxcos = x * cosa;
	        nhxsin = x * sina;
	        topRightX = (int)(nhxcos + nhysin);
	        topRightY = (int)(nhycos - nhxsin);
        	
	        // Rotate bottom-right point
	        int bottomRightX, bottomRightY;
        	
	        // Ditto, optimized
	        y = (double)(pHeight - pHY);
	        nhycos = y * cosa;
	        nhysin = y * sina;
	        bottomRightX = (int)(nhxcos + nhysin);
	        bottomRightY = (int)(nhycos - nhxsin);
        	
	        // Bottom-left
	        int bottomLeftX, bottomLeftY;
	        bottomLeftX = topLeftX + bottomRightX - topRightX;
	        bottomLeftY = topLeftY + bottomRightY - topRightY;
        	
	        // Get limits
            int xmin = Math.Min(topLeftX, Math.Min(topRightX, Math.Min(bottomRightX, bottomLeftX)));
            int ymin = Math.Min(topLeftY, Math.Min(topRightY, Math.Min(bottomRightY, bottomLeftY)));
            int xmax = Math.Max(topLeftX, Math.Max(topRightX, Math.Max(bottomRightX, bottomLeftX)));
            int ymax = Math.Max(topLeftY, Math.Max(topRightY, Math.Max(bottomRightY, bottomLeftY)));
        	
	        // Update hotspot position
	        pHX = -xmin;
	        pHY = -ymin;
        	
	        // Update rectangle
	        pWidth = xmax - xmin;
	        pHeight = ymax - ymin;
        }

        public bool createRotatedMask(CMask pMask, double fAngle, double fScaleX, double fScaleY)
        {
	        int x, y;
        	
	        // Calculate new mask bounding box
	        int cx = pMask.width;
	        int cy = pMask.height;
        	
	        int rcRight, rcBottom;
	        rcRight = (int)(pMask.width * fScaleX);
	        rcBottom = (int)(pMask.height * fScaleY);
        	
	        int hsX, hsY;
	        hsX = (int)(pMask.xSpot * fScaleX);
	        hsY = (int)(pMask.ySpot * fScaleY);
	        rotateRect(ref rcRight, ref rcBottom, ref hsX, ref hsY, fAngle);
	        int newCx = rcRight;
	        int newCy = rcBottom;
	        if ( newCx <= 0 || newCy <= 0 )
		        return false;
        	
	        // Allocate memory for new mask
	        int sMaskWidthWords=pMask.lineWidth;
	        int dMaskWidthShorts = ((newCx + 15) & 0x7FFFFFF0) / 16;
	        mask = new ushort[dMaskWidthShorts * newCy + 1];
	        lineWidth = dMaskWidthShorts;
	        width = newCx;
	        height = newCy;
	        xSpot = hsX;
	        ySpot = hsY;
        	
	        double alpha = (double)(fAngle * 0.017453292);
	        double cosa = Math.Cos(alpha);
	        double sina = Math.Sin(alpha);

            double fxs = (double)((double)cx / 2) - ((double)((double)newCx / 2) * cosa - (double)((double)newCy / 2) * sina) / fScaleX;
            double fys = (double)((double)cy / 2) - ((double)((double)newCx / 2) * sina + (double)((double)newCy / 2) * cosa) / fScaleY;
        	
	        int pbd0 = 0;		
	        int pbd1 = pbd0;
        	
	        int nxs = (int)(fxs * 65536);
	        int nys = (int)(fys * 65536);
	        int ncosa = (int)((cosa * 65536) / fScaleX);
	        int nsina = (int)((sina * 65536) / fScaleY);
        	
	        int newCxMul16 = newCx/16;
	        int newCxMod16 = newCx%16;
        	
	        int ncosa2=(int)((cosa*65536)/fScaleY);
	        int nsina2=(int)((sina*65536)/fScaleX);

			int cxs = cx * 65536;
			int cys = cy * 65536;

	        ushort bMask;
	        ushort b;
	        for (y=0; y<newCy; y++)
	        {
		        int txs = nxs;
		        int tys = nys;
		        int pbd2 = pbd1;
		        int xs, ys;
        		
		        for (x=0; x<newCxMul16; x++)
		        {
			        ushort bd = 0;
        			
			        // 1
			        if ( txs >= 0 && txs < cxs )
			        {
				        if ( tys >= 0 && tys < cys )
				        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
					        b = pMask.mask[ys * sMaskWidthWords + xs/16];
					        if ( (b & bMask)!=0 )
						        bd |= 0x8000;
				        }
			        }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 2
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x4000;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 3
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x2000;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 4
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x1000;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 5
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0800;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 6
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0400;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 7
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0200;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 8
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0100;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 9
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0080;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 10 
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0040;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 11
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0020;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 12
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0010;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 13
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0008;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 14
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0004;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 15
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0002;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        // 16
                    if (txs >= 0 && txs < cxs)
                    {
                        if (tys >= 0 && tys < cys)
                        {
                            xs = txs / 65536;
                            ys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (xs % 16));
                            b = pMask.mask[ys * sMaskWidthWords + xs / 16];
                            if ((b & bMask) != 0)
                                bd |= 0x0001;
                        }
                    }
                    txs += ncosa;
                    tys += nsina;
        			
			        mask[pbd2++] = bd;
		        }
        		
		        if ( newCxMod16!=0 )
		        {
			        ushort bdMask = 0x8000;
			        ushort bdbd = 0;
			        for (x=0; x<newCxMod16; x++, bdMask=(ushort)((bdMask>>1)&0x7FFF))
			        {        				
				        if ( txs >= 0 && txs < cxs && tys >= 0 && tys < cys )
				        {
                            int bdxs = txs / 65536;
                            int bdys = tys / 65536;
                            bMask = (ushort)(0x8000 >> (bdxs % 16));
                            b = pMask.mask[bdys * sMaskWidthWords + bdxs / 16];
                            if ((b & bMask) != 0)
                                bdbd |= bdMask;
				        }
                        txs += ncosa;
                        tys += nsina;
                    }
			        mask[pbd2] = bdbd;
		        }
        		
		        pbd1 += dMaskWidthShorts;
        		
		        nxs -= nsina2;
		        nys += ncosa2;		
	        }
	        return true;			
        }

        public bool testMask(int yBase1, int x1, int y1, CMask pMask2, int yBase2, int x2, int y2)
        {
	        CMask pLeft;
	        CMask pRight;
	        int x1Left, y1Left, x1Right, y1Right;
	        int syLeft, syRight;
	        int yBaseLeft, yBaseRight;
        	
	        if (x1 <= x2)
	        {
		        pLeft = this;
		        pRight = pMask2;
		        yBaseLeft = yBase1;
		        yBaseRight = yBase2;
		        x1Left = x1;
		        y1Left = y1;
		        x1Right = x2;
		        y1Right = y2;
	        }
	        else
	        {
		        pLeft = pMask2;
		        pRight = this;
		        yBaseLeft = yBase2;
		        yBaseRight = yBase1;
		        x1Left = x2;
		        y1Left = y2;
		        x1Right = x1;
		        y1Right = y1;
	        }
	        syLeft = pLeft.height - yBaseLeft;
	        syRight = pRight.height - yBaseRight;
        	
	        if (x1Left >= x1Right + pRight.width || x1Left + pLeft.width <= x1Right)
	        {
		        return false;
	        }
	        if (y1Left >= y1Right + syRight || y1Left + syLeft < y1Right)
	        {
		        return false;
	        }
        	
	        int deltaX = x1Right - x1Left;
	        int offsetX = deltaX / 16;
	        int shiftX = deltaX % 16;
	        int countX = Math.Min(x1Left + pLeft.width - x1Right, pRight.width);
	        countX = (countX + 15) / 16;
        	
	        int deltaYLeft, deltaYRight, countY;
	        if (y1Left <= y1Right)
	        {
		        deltaYLeft = y1Right - y1Left + yBaseLeft;
		        deltaYRight = yBaseRight;
		        countY = Math.Min(y1Left + syLeft, y1Right + syRight) - y1Right;
	        }
	        else
	        {
		        deltaYLeft = yBaseLeft;
		        deltaYRight = y1Left - y1Right + yBaseRight;
		        countY = Math.Min(y1Left + syLeft, y1Right + syRight) - y1Left;
	        }
	        int x, y;
        	
	        int offsetYLeft, offsetYRight;
	        int leftX, middleX;
	        ushort shortX;
	        if (shiftX != 0)
	        {
		        switch (countX)
		        {
			        case 1:
				        for (y = 0; y < countY; y++)
				        {
					        offsetYLeft = (deltaYLeft + y) * pLeft.lineWidth;
					        offsetYRight = (deltaYRight + y) * pRight.lineWidth;
        					
					        // Premier mot
					        leftX = ((int) pLeft.mask[offsetYLeft + offsetX]) << shiftX;
					        shortX = (ushort) leftX;
					        if ((shortX & pRight.mask[offsetYRight]) != 0)
					        {
						        return true;
					        }
        					
					        if (offsetX * 16 + 16 < pLeft.width)
					        {
						        middleX = (((int) pLeft.mask[offsetYLeft + offsetX + 1]) & 0x0000FFFF) << shiftX;
						        shortX = (ushort) (middleX >> 16);
						        if ((shortX & pRight.mask[offsetYRight]) != 0)
						        {
							        return true;
						        }
					        }
				        }
				        break;
			        case 2:
				        for (y = 0; y < countY; y++)
				        {
					        offsetYLeft = (deltaYLeft + y) * pLeft.lineWidth;
					        offsetYRight = (deltaYRight + y) * pRight.lineWidth;
        					
					        // Premier mot
					        leftX = ((int) pLeft.mask[offsetYLeft + offsetX]) << shiftX;
					        shortX = (ushort) leftX;
					        if ((shortX & pRight.mask[offsetYRight]) != 0)
					        {
						        return true;
					        }
					        middleX = (((int) pLeft.mask[offsetYLeft + offsetX + 1]) & 0x0000FFFF) << shiftX;
					        shortX = (ushort) (middleX >> 16);
					        if ((shortX & pRight.mask[offsetYRight]) != 0)
					        {
						        return true;
					        }
        					
					        // Milieu
					        shortX = (ushort) middleX;
					        if ((shortX & pRight.mask[offsetYRight + 1]) != 0)
					        {
						        return true;
					        }

   							if (offsetX + 2<pLeft.lineWidth)
	                        {
	                            middleX = pLeft.mask[offsetYLeft + offsetX + 2] << shiftX;
	                            shortX=(ushort)(middleX>>16);
	                            if ((shortX & pRight.mask[offsetYRight+1]) != 0)
	                            {
	                                return true;
	                            }
	                        }

                        }
				        break;
			        default:
				        for (y = 0; y < countY; y++)
				        {
					        offsetYLeft = (deltaYLeft + y) * pLeft.lineWidth;
					        offsetYRight = (deltaYRight + y) * pRight.lineWidth;
        					
					        // Premier mot
					        leftX = ((int) pLeft.mask[offsetYLeft + offsetX]) << shiftX;
					        shortX = (ushort) leftX;
					        if ((shortX & pRight.mask[offsetYRight]) != 0)
					        {
						        return true;
					        }
        					
					        for (x = 0; x < countX - 1; x++)
					        {
						        middleX = (((int) pLeft.mask[offsetYLeft + offsetX +x+ 1]) & 0x0000FFFF) << shiftX;
						        shortX = (ushort) (middleX >> 16);
						        if ((shortX & pRight.mask[offsetYRight+x]) != 0)
						        {
							        return true;
						        }
        						
						        // Milieu
						        shortX = (ushort) middleX;
						        if ((shortX & pRight.mask[offsetYRight + x + 1]) != 0)
						        {
							        return true;
						        }
					        }

   	                        if (offsetX + x + 1<pLeft.lineWidth)
	                        {
	                            middleX = pLeft.mask[offsetYLeft + offsetX + x + 1] << shiftX;
	                            shortX=(ushort)(middleX>>16);
	                            if ((shortX & pRight.mask[offsetYRight+x]) != 0)
	                            {
	                                return true;
	                            }
	                        }
				        }
				        break;
		        }
	        }
	        else
	        {
		        for (y = 0; y < countY; y++)
		        {
			        offsetYLeft = (deltaYLeft + y) * pLeft.lineWidth;
			        offsetYRight = (deltaYRight + y) * pRight.lineWidth;
        			
			        for (x = 0; x < countX; x++)
			        {
				        leftX = pLeft.mask[offsetYLeft + offsetX + x];
				        if ((pRight.mask[offsetYRight + x] & leftX) != 0)
				        {
					        return true;
				        }
			        }
		        }
	        }
	        return false;
        }

        public bool testRect(int yBase1, int xx, int yy, int w, int h)
        {
	        int x1 = xx;
	        if (x1 < 0)
	        {
		        w += x1;
		        x1 = 0;
	        }
	        int y1 = yy;
	        if (yBase1 != 0 && y1 >= 0)
	        {
		        y1 = yBase1 + y1;
		        h = height - y1;
	        }
	        if (y1 < 0)
	        {
		        h += y1;
		        y1 = 0;
	        }
	        int x2 = x1 + w;
	        if (x2 > width)
	        {
		        x2 = width;
	        }
	        int y2 = y1 + h;
	        if (y2 > height)
	        {
		        y2 = height;
	        }
        	
	        int offset = (y1) * lineWidth;
	        int yCount = y2 - y1;
	        int xCount = (x2 - x1) / 16 + 1;
	        int xOffset = x1 / 16;
	        int x, y;
        	
	        ushort m;
	        int yOffset;
	        for (y = 0; y < yCount; y++)
	        {
		        yOffset = y * lineWidth + offset;
        		
		        switch (xCount)
		        {
			        case 1:
				        m = (ushort) (lMask[x1 & 15] & rMask[(x2 - 1) & 15]);
				        if ((mask[yOffset + xOffset] & m) != 0)
				        {
					        return true;
				        }
				        break;
			        case 2:
				        m = lMask[x1 & 15];
				        if ((mask[yOffset + xOffset] & m) != 0)
				        {
					        return true;
				        }
				        m = rMask[(x2 - 1) & 15];
				        if ((mask[yOffset + xOffset + 1] & m) != 0)
				        {
					        return true;
				        }
				        break;
			        default:
				        m = lMask[x1 & 15];
				        if ((mask[yOffset + xOffset] & m) != 0)
				        {
					        return true;
				        }
				        for (x = 1; x < xCount - 1; x++)
				        {
					        if (mask[yOffset + xOffset + 1] != 0)
					        {
						        return true;
					        }
				        }
				        m = rMask[(x2 - 1) & 15];
				        if ((mask[yOffset + xOffset + x] & m) != 0)
				        {
					        return true;
				        }
				        break;
		        }
	        }
	        return false;
        }

        public bool testPoint(int x1, int y1)
        {
	        if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height)
	        {
		        return false;
	        }
        	
	        int offset = (y1 * lineWidth) + x1 / 16;
	        ushort m = (ushort) (((int)0x8000) >> (x1 & 15));
	        if ((mask[offset] & m) != 0)
	        {
		        return true;
	        }
	        return false;
        }

    }


}
