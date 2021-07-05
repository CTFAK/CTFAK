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
// CIMAGEBANK : Stockage des images
//
//----------------------------------------------------------------------------------
package Banks;

import java.io.BufferedInputStream;
import java.io.ByteArrayOutputStream;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;

import Application.CRunApp;
import Runtime.MMFRuntime;
import Runtime.SurfaceView;
import Services.CFile;
import Services.CServices;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.SystemClock;
import android.util.Log;

public class CImageBank implements IEnum
{
    public CRunApp app;
    CFile file;
    
    public HashMap<Short, Runnable> runnable = new HashMap<Short, Runnable>();

    int bankSize;
    int bankRunnable;
    
    public class BankSlot
    {
    	public short handle;
    	public boolean antialiased;
    	
    	public BankSlot (short handle)
    	{
    		this.handle = handle;

    		useCount = 0;
    		dataOffset = -1;
			useCountCopy = 0;
    	}

    	public int useCount;

    	public int dataOffset;
    	public BankImage texture;

    	/* only for added images */
    	public Bitmap source;

    	/* used when the texture is unloaded to back up the hotspot etc. */
    	public CImageInfo info_backup;

    	public int useCountCopy;

    	public void uploadTexture()
    	{
    		if (texture != null)
    			return;

    		if (dataOffset != -1)
    		{
    			file.seek (dataOffset);

    			texture = new BankImage (this, file);

    			if (info_backup != null)
    			{
    				texture.setXAP (info_backup.xAP);
    				texture.setYAP (info_backup.yAP);

    				texture.setXSpot (info_backup.xSpot);
    				texture.setYSpot (info_backup.ySpot);
    				texture.setResampling(info_backup.antialias);

    				info_backup = null;
    			}
    		}
    		else if (source != null)
    		{
    			texture = new BankImage
    					(this, CServices.getBitmapPixels (source),
    							info_backup.xSpot, info_backup.ySpot,
    							info_backup.xAP, info_backup.yAP,
    							source.getWidth (), source.getHeight ());
    		}	
    	}
    }

    public ArrayList <BankSlot> images;

    class BankImage extends CImage
    {
        public BankSlot slot;

        public BankImage (BankSlot slot, CFile file)
        {
            this.slot = slot;

            //long start = SystemClock.currentThreadTimeMillis();


            //Log.v("Bank Image slot", "image #: "+ slot.handle);

            allocNative4(slot.antialiased, file, SurfaceView.ES);
            // 292.15 moving this code to native libs for performance
            /*
            if(imageFormat() == FORMATS.JPEG.ordinal() || imageFormat() == FORMATS.PNG.ordinal())
            {
                int scale = 1;
                try {
                    int size = imageSize();
                    int begin = imageBegin();
                    file.seek(begin);

                    byte[] buffer = new byte[size];;
                    file.read(buffer);

                    BitmapFactory.Options options = new BitmapFactory.Options();
                    options.inSampleSize = scale;
                    options.inPreferredConfig = Bitmap.Config.ARGB_8888;

                    Bitmap img = BitmapFactory.decodeByteArray(buffer, 0, buffer.length, options);
                    imageSetData(img);
                    img.recycle();
                 }
                catch(Exception e)
                {
                    Log.v("Bank Image slot", "image from JPEG #:"+ slot.handle);
                }

            }
            */
            //Log.v("Bank Image slot", "image #:"+ slot.handle+ " took(msecs) "+ (SystemClock.currentThreadTimeMillis()-start));
        }

        public BankImage (BankSlot slot, int [] img, int xSpot, int ySpot,
                      int xAP, int yAP, int width, int height)
        {
            this.slot = slot;
 
            allocNative2 (slot.antialiased,
                            slot.handle, img, xSpot, ySpot, xAP, yAP, width, height, SurfaceView.ES);
        }

        @Override
        public void onDestroy ()
        {
        	/* Save the image state */
        	try {
        		CImageInfo info = new CImageInfo ();

        		if(info != null) {
        			info.xAP = getXAP ();
        			info.yAP = getYAP ();
        			info.xSpot = getXSpot ();
        			info.ySpot = getYSpot ();
        			info.antialias = getResampling();
        			//info.height = getHeight();
        			//info.width = getWidth();
        			slot.info_backup = info;
        		}
        	} // i don't care about the exception, just care not to stop execution
        	finally {
        		slot.texture = null;
        	}
        }
    };

    public CImage getImageFromHandle (short handle)
    {

    	if (handle < 0 || handle >= images.size ())
            return null;

        BankSlot slot = images.get (handle);
        
        synchronized(slot) {
        	if (slot == null)
        		return null;
        
        	if(slot instanceof BankSlot) {
        		slot.uploadTexture();
        		return slot.texture;
        	}
        	return null;
        }
        
    }

    public CImageBank(CRunApp a)
    {
        app = a;
    }

    public void preLoad(CFile f)
    {
        file = f;

        int nMaxHandle = file.readAShort();

        images = new ArrayList <BankSlot> (nMaxHandle);

        for (short i = 0; i < nMaxHandle; ++ i)
            images.add (new BankSlot (i));

        // Repere les positions des images
        int nImg = file.readAShort();
        int n;
        for (n = 0; n < nImg; n++)
        {
            int offset = file.getFilePointer ();
            short handle = file.readAShort ();

            images.get (handle).dataOffset = offset;

            file.skipBytes (16);
           	file.skipBytes (file.readAInt ());

        }
        Log.v("Bank Image slot", "preloaded ...");
        bankSize = nImg;
     }

    @Override
	public short enumerate(short handle)
    {
        if (handle < 0 || handle > images.size ())
            return -1;

        ++ images.get (handle).useCount;

        return handle;
    }

    public void resetToLoad ()
    {
        Iterator <BankSlot> i = images.iterator ();

        while (i.hasNext ())
        {
           BankSlot b =  i.next ();
           b.useCount = 0;
		   b.useCountCopy = 0;
        }
        runnable.clear();
    }

    public void copyUseCount ()
    {
        Iterator <BankSlot> i = images.iterator ();

        while (i.hasNext ())
        {
           BankSlot b =  i.next ();
           b.useCountCopy = b.useCount;
        }
    }

    public void load()
    {
        Iterator <BankSlot> i = images.iterator ();
               
        runnable.clear();
        bankRunnable = 0;
        
        while (i.hasNext ())
        {
            BankSlot slot = i.next ();

            if (slot.useCount != 0)
            {
                final BankSlot bSlot = slot;
                runnable.put(slot.handle, new Runnable()
                {
                    @Override
                    public void run()
                    {
                       	bSlot.uploadTexture ();
                        bankRunnable++;
                    }
                });
                
                runnable.get(slot.handle).run();
            }
            else
            {
                if (slot.texture != null
                        && slot.dataOffset != -1) /* can't unload added images */
                {
                    slot.texture.destroy ();
                }
            }
        }
    }
 
    public boolean loaded()
    {
    	boolean bRet = false;
    	int count = 0;

    	Iterator <BankSlot> i = images.iterator ();        

    	while (i.hasNext ())
    	{
    		BankSlot slot = i.next ();

    		if (slot.useCount != 0)
    		{
    			count++;
    		}
    	}
    	if(bankRunnable - count < 1)
    		bRet = true;

    	return bRet;
    }

    public void loadUnloaded()
    {
        Iterator <BankSlot> i = images.iterator ();

        while (i.hasNext ())
        {
            BankSlot slot = i.next ();

            if (slot.useCount > slot.useCountCopy && slot.texture == null && slot.dataOffset != -1 )
            {
            	slot.uploadTexture ();
				slot.useCountCopy = slot.useCount;
            }
        }
    }
    
	public void unloadLoaded()
	{
        Iterator <BankSlot> i = images.iterator ();

		//int count = 0;

        while (i.hasNext ())
        {
            BankSlot slot = i.next ();

			int discardCount = slot.useCount - slot.useCountCopy;
			if ( discardCount != 0 )
			{
				slot.useCount = slot.useCountCopy - discardCount;
				slot.useCountCopy = slot.useCount;
				if (slot.useCount == 0 && slot.texture != null )
				{
                    slot.texture.destroy ();
					//count++;
				}
			}
        }

		//Log.v("MMFRuntime", "Discarded " + count + " images");
	}
    
    /* TODO : when can we destroy img and remove the slot? */

    public short addImage(Bitmap img, short xSpot, short ySpot, short xAP, short yAP, boolean antialiased_flag)
    {
        BankSlot slot = new BankSlot ((short) images.size ());
        images.add (slot);

        slot.dataOffset = -1;
        slot.source = img;

        slot.texture = new BankImage
            (slot, CServices.getBitmapPixels (img), xSpot, ySpot, xAP, yAP,
                img.getWidth (), img.getHeight ());
        
        slot.antialiased = antialiased_flag;
        slot.texture.setResampling(antialiased_flag);
        return slot.handle;
    }

    public void loadImageList(short[] handles, boolean antialiased_flag)
    {
        int handles_length = handles.length;
    	for (int i = 0; i < handles_length; ++ i)
        {
            int handle = handles [i];

            if (handle < 0 || handle >= images.size ())
                continue;

            BankSlot slot = images.get (handle);
            synchronized(slot) {
	            if (slot.texture == null && slot.dataOffset != -1 || slot.texture.getResampling() != antialiased_flag)
	            {
	                file.seek (slot.dataOffset);
	                //Log.v("MMFRuntime", "Updating texture for handle: "+handle+" antialias: "+(antialiased_flag?" yes":"no"));
	                slot.antialiased = antialiased_flag;
	                slot.texture = new BankImage (slot, file);
	                if(slot.texture != null)
	                    slot.texture.setResampling(antialiased_flag);
	            }
            }
        }
    }

    public CImageInfo getImageInfoEx(short nImage, float nAngle, float fScaleX, float fScaleY)
    {
        if (nImage > images.size () || nImage < 0)
            return null;

        CImage image = images.get (nImage).texture;

        if (image == null)
            return null;

        CImageInfo info = new CImageInfo ();
        image.getInfo (info, Math.round(nAngle), fScaleX, fScaleY);
        return info;
    }

    public boolean getImageInfoEx2(CImageInfo info, short nImage, float nAngle, float fScaleX, float fScaleY)
    {
        if (nImage > images.size () || nImage < 0)
            return false;

        CImage image = images.get (nImage).texture;
        if (image == null)
            return false;

        image.getInfo (info, Math.round(nAngle), fScaleX, fScaleY);
        return true;
    }
}


