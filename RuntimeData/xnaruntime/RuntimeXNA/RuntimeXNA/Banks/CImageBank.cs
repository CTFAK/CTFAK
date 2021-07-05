//----------------------------------------------------------------------------------
//
// CIMAGEBANK : Stockage des images
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Banks
{
    public class CImageBank : IEnum
    {
        public CRunApp app;
        public CFile file;
        public CImage[] images;
        public int nHandlesReel;
        public int nHandlesTotal;
        public int nImages;
        int[] offsetsToImage;
        short[] handleToIndex;
        byte[] useCount;
        CRect rcInfo=null;
        CPoint hsInfo=null;
        CPoint apInfo=null;
        public Texture2D[] mosaics=null;
        public Texture2D[] oldMosaics=null;	

        public CImageBank()
        {
        }

        public CImageBank(CRunApp a)
        {
            app = a;
            file=app.file;
        }

        public void preLoad()
        {
            // Nombre de handles
            nHandlesReel = file.readAShort();
            offsetsToImage = new int[nHandlesReel];

            // Repere les positions des images
            int nImg = file.readAShort();
            int n;
            int offset;
            CImage image = new CImage();
            for (n = 0; n < nImg; n++)
            {
                offset = (int) file.getFilePointer();
                image.loadHandle(app.file);
                offsetsToImage[image.handle] = offset;
            }

            // Reservation des tables
            useCount = new byte[nHandlesReel];
            resetToLoad();
            handleToIndex = null;
            nHandlesTotal = nHandlesReel;
            nImages = 0;
            images = null;
        }

        public CImage getImageFromHandle(short handle)
        {
            if (handle >= 0 && handle < nHandlesTotal)
            {
                if (handleToIndex[handle] != -1)
                {
                    return images[handleToIndex[handle]];
                }
            }
            return null;
        }

        public CImage getImageFromIndex(short index)
        {
            if (index >= 0 && index < nImages)
            {
                return images[index];
            }
            return null;
        }

        public void resetToLoad()
        {
            int n;
            for (n = 0; n < nHandlesReel; n++)
            {
                useCount[n] = 0;
            }
        }

        public void setToLoad(short handle)
        {
            useCount[handle]++;
        }

        // Entree enumeration
        public short enumerate(short num)
        {
            setToLoad(num);
            return -1;
        }

	    public void loadMosaic(short handle)
        {
            if (mosaics[handle]==null)
            {
        	    if (oldMosaics!=null && handle<oldMosaics.Length && oldMosaics[handle]!=null)
        	    {
        		    mosaics[handle]=oldMosaics[handle];
        	    }
        	    else
        	    {
                    string imgName = handle.ToString("D4");
                    imgName = "ImgM" + imgName;
                    mosaics[handle] = app.content.Load<Texture2D>(imgName);
		        }
	   	    }
        }
    
        public void load()
        {
            int n;

            // Reset mosaics
            if (app.frame.mosaicMaxHandle > 0)
            {
                int size = app.frame.mosaicMaxHandle;
                if (mosaics != null)
                {
                    oldMosaics = new Texture2D[mosaics.Length];
                    for (n = 0; n < mosaics.Length; n++)
                        oldMosaics[n] = mosaics[n];
                    size = Math.Max(size, mosaics.Length);
                }
                mosaics = new Texture2D[size];
                for (n = 0; n < size; n++)
                    mosaics[n] = null;
            }

            // Combien d'images?
            nImages = 0;
            for (n = 0; n < nHandlesReel; n++)
            {
                if (useCount[n] != 0)
                {
                    nImages++;
                }
            }

            // Charge les images
            CImage[] newImages = new CImage[nImages];
            int count = 0;
            int h;
            for (h = 0; h < nHandlesReel; h++)
            {
                if (useCount[h] != 0)
                {
                    if (images != null && handleToIndex[h] != -1 && images[handleToIndex[h]] != null)
                    {
                        newImages[count] = images[handleToIndex[h]];
                        newImages[count].useCount = useCount[h];
                        if (mosaics != null && oldMosaics != null)
                        {
                            var handle = newImages[count].mosaic;
                            if (handle > 0)
                                mosaics[handle] = oldMosaics[handle];
                        }
                    }
                    else
                    {
                        newImages[count] = new CImage();
                        file.seek(offsetsToImage[h]);
                        newImages[count].load(app);
                        newImages[count].useCount = useCount[h];
                    }
                    count++;
                }
            }
            images = newImages;

            // Cree la table d'indirection
            handleToIndex = new short[nHandlesReel];
            for (n = 0; n < nHandlesReel; n++)
            {
                handleToIndex[n] = -1;
            }
            for (n = 0; n < nImages; n++)
            {
                handleToIndex[images[n].handle] = (short) n;
            }
            nHandlesTotal = nHandlesReel;

            // Plus rien a charger
            resetToLoad();
            oldMosaics = null;
        }

        public CImage getImageInfoEx(short nImage, int nAngle, float fScaleX, float fScaleY)
        {
            CImage ptei;
            CImage pIfo = new CImage();

            ptei = getImageFromHandle(nImage);
            if (ptei != null)
            {
                int cx = ptei.width;
                int cy = ptei.height;
                int hsx = ptei.xSpot;
                int hsy = ptei.ySpot;
                int asx = ptei.xAP;
                int asy = ptei.yAP;

                // No rotation
                if ( nAngle == 0 )
                {
                    // Stretch en X
                    if ( fScaleX != 1.0 )
                    {
                        hsx = (int)(hsx * fScaleX);
                        asx = (int)(asx * fScaleX);
                        cx = (int)(cx * fScaleX);
                    }

                    // Stretch en Y
                    if ( fScaleY != 1.0 )
                    {
                        hsy = (int)(hsy * fScaleY);
                        asy = (int)(asy * fScaleY);
                        cy = (int)(cy * fScaleY);
                    }
                }
                // Rotation
                else
                {
                    // Calculate dimensions
                    if ( fScaleX != 1.0 )
                    {
                        hsx = (int)(hsx * fScaleX);
                        asx = (int)(asx * fScaleX);
                        cx = (int)(cx * fScaleX);
                    }

                    if ( fScaleY != 1.0 )
                    {
                        hsy = (int)(hsy * fScaleY);
                        asy = (int)(asy * fScaleY);
                        cy = (int)(cy * fScaleY);
                    }

                    if (rcInfo==null)
                    {
                        rcInfo=new CRect();
                    }
                    if (hsInfo==null)
                    {
                        hsInfo=new CPoint();
                    }
                    if (apInfo==null)
                    {
                        apInfo=new CPoint();
                    }
                    hsInfo.x = hsx;
                    hsInfo.y = hsy;
                    apInfo.x = asx;
                    apInfo.y = asy;
                    rcInfo.left = rcInfo.top = 0;
                    rcInfo.right = cx;
                    rcInfo.bottom = cy;
                    doRotateRect(rcInfo, hsInfo, apInfo, nAngle);
                    cx = rcInfo.right;
                    cy = rcInfo.bottom;
                    hsx = hsInfo.x;
                    hsy = hsInfo.y;
                    asx = apInfo.x;
                    asy = apInfo.y;
                }
                pIfo.width = (short)cx;
                pIfo.height = (short)cy;
                pIfo.xSpot = (short)hsx;
                pIfo.ySpot = (short)hsy;
                pIfo.xAP = (short)asx;
                pIfo.yAP = (short)asy;

                return pIfo;
            }
            return null;
        }

        void doRotateRect(CRect prc, CPoint pHotSpot, CPoint pActionPoint, double fAngle)
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
                double arad = (double)(fAngle * Math.PI / 180.0);
                cosa = Math.Cos(arad);
                sina = Math.Sin(arad);
            }

            // Rotate top-left point
            double topLeftX;
            double topLeftY;

            // Ditto, optimized
            double nhxcos;
            double nhxsin;
            double nhycos;
            double nhysin;
            if ( pHotSpot == null )
            {
                nhxcos = nhxsin = nhycos = nhysin = 0.0;
                topLeftX = topLeftY = 0;
            }
            else
            {
                nhxcos = -pHotSpot.x * cosa;
                nhxsin = -pHotSpot.x * sina;
                nhycos = -pHotSpot.y * cosa;
                nhysin = -pHotSpot.y * sina;
                topLeftX = nhxcos + nhysin;
                topLeftY = nhycos - nhxsin;
            }

            // Rotate top-right point
            double topRightX;
            double topRightY;

            // Ditto, optimized
            if ( pHotSpot == null )
                x = (double)(prc.right);
            else
                x = (double)(prc.right - pHotSpot.x);
            nhxcos = x * cosa;
            nhxsin = x * sina;
            topRightX = nhxcos + nhysin;
            topRightY = nhycos - nhxsin;

            // Rotate bottom-right point
            double bottomRightX;
            double bottomRightY;

            // Ditto, optimized
            if ( pHotSpot == null )
                y = (double)(prc.bottom);
            else
                y = (double)(prc.bottom - pHotSpot.y);
            nhycos = y * cosa;
            nhysin = y * sina;
            bottomRightX = nhxcos + nhysin;
            bottomRightY = nhycos - nhxsin;

            // Bottom-left
            double bottomLeftX;
            double bottomLeftY;
            bottomLeftX = topLeftX + bottomRightX - topRightX;
            bottomLeftY = topLeftY + bottomRightY - topRightY;

            // Get limits
            double xmin = Math.Min(topLeftX, Math.Min(topRightX, Math.Min(bottomRightX, bottomLeftX)));
            double ymin = Math.Min(topLeftY, Math.Min(topRightY, Math.Min(bottomRightY, bottomLeftY)));
            double xmax = Math.Max(topLeftX, Math.Max(topRightX, Math.Max(bottomRightX, bottomLeftX)));
            double ymax = Math.Max(topLeftY, Math.Max(topRightY, Math.Max(bottomRightY, bottomLeftY)));

            // Update action point position
            if ( pActionPoint != null )
            {
                if ( pHotSpot == null )
                {
                    x = (double)(pActionPoint.x);
                    y = (double)(pActionPoint.y);
                }
                else
                {
                    x = (double)(pActionPoint.x - pHotSpot.x);			// coordinates relative to hot spot
                    y = (double)(pActionPoint.y - pHotSpot.y);
                }
                pActionPoint.x = (int)((x * cosa + y * sina) - xmin);
                pActionPoint.y = (int)((y * cosa - x * sina) - ymin);
            }

            // Update hotspot position
            if ( pHotSpot != null )
            {
                pHotSpot.x = (int)-xmin;
                pHotSpot.y = (int)-ymin;
            }

            // Update rectangle
            prc.right = (int)(xmax - xmin);
            prc.bottom = (int)(ymax - ymin);
        }

        public short addImage(Texture2D img, short xSpot, short ySpot, short xAP, short yAP, short count)
        {
            int h;

            // Cherche un handle libre
            short hFound = -1;
            for (h = nHandlesReel; h < nHandlesTotal; h++)
            {
                if (handleToIndex[h] == -1)
                {
                    hFound = (short) h;
                    break;
                }
            }

            // Rajouter un handle
            if (hFound == -1)
            {
                short[] newHToI = new short[nHandlesTotal + 10];
                for (h = 0; h < nHandlesTotal; h++)
                {
                    newHToI[h] = handleToIndex[h];
                }
                for (; h < nHandlesTotal + 10; h++)
                {
                    newHToI[h] = -1;
                }
                hFound = (short) nHandlesTotal;
                nHandlesTotal += 10;
                handleToIndex = newHToI;
            }

            // Cherche une image libre
            int i;
            int iFound = -1;
            for (i = 0; i < nImages; i++)
            {
                if (images[i] == null)
                {
                    iFound = i;
                    break;
                }
            }

            // Rajouter une image?
            if (iFound == -1)
            {
                CImage[] newImages = new CImage[nImages + 10];
                for (i = 0; i < nImages; i++)
                {
                    newImages[i] = images[i];
                }
                for (; i < nImages + 10; i++)
                {
                    newImages[i] = null;
                }
                iFound = nImages;
                nImages += 10;
                images = newImages;
            }

            // Ajoute la nouvelle image
            handleToIndex[hFound] = (short) iFound;
            images[iFound] = new CImage();
            images[iFound].handle = hFound;
            images[iFound].image = img;
            images[iFound].xSpot = xSpot;
            images[iFound].ySpot = ySpot;
            images[iFound].xAP = xAP;
            images[iFound].yAP = yAP;
            images[iFound].useCount = count;
            images[iFound].width = (short) img.Width;
            images[iFound].height = (short) img.Height;

            return hFound;
        }
        // Detruit une image si plus d'utilisation
        public void delImage(short handle)
        {
            CImage img = getImageFromHandle(handle);
            if (img != null)
            {
                img.useCount--;
                if (img.useCount <= 0)
                {
                    int n;
                    for (n = 0; n < nImages; n++)
                    {
                        if (images[n] == img)
                        {
                            images[n] = null;
                            handleToIndex[handle] = -1;
                            break;
                        }
                    }
                }
            }
        }

        public void loadImageList(short[] handles)
        {
            int h;

            for (h = 0; h < handles.Length; h++)
            {
                if (handles[h] >= 0 && handles[h] < nHandlesTotal)
                {
                    if (offsetsToImage[handles[h]] != 0)
                    {
                        if (getImageFromHandle(handles[h]) == null)
                        {
                            // Cherche une image libre
                            int i;
                            int iFound = -1;
                            for (i = 0; i < nImages; i++)
                            {
                                if (images[i] == null)
                                {
                                    iFound = i;
                                    break;
                                }
                            }
                            // Rajouter une image?
                            if (iFound == -1)
                            {
                                CImage[] newImages = new CImage[nImages + 10];
                                for (i = 0; i < nImages; i++)
                                {
                                    newImages[i] = images[i];
                                }
                                for (; i < nImages + 10; i++)
                                {
                                    newImages[i] = null;
                                }
                                iFound = nImages;
                                nImages += 10;
                                images = newImages;
                            }
                            // Ajoute la nouvelle image
                            handleToIndex[handles[h]] = (short) iFound;
                            images[iFound] = new CImage();
                            images[iFound].useCount = 1;
                            file.seek(offsetsToImage[handles[h]]);
                            images[iFound].load(app);
                        }
                    }
                }
            }
        }


    }
}
