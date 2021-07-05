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

import android.graphics.Bitmap;
import android.graphics.Bitmap.Config;
import android.graphics.BitmapFactory;
import android.os.SystemClock;
import android.util.Log;

import java.io.BufferedInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.HashSet;
import java.util.Set;

import Application.CRunApp;
import OpenGL.ITexture;
import Runtime.MMFRuntime;
import Runtime.SurfaceView;
import Services.CFile;
import Sprites.CMask;

/* This class is abstract because anyone using CImage MUST handle onDestroy */

public abstract class CImage extends ITexture
{
	public static Set <CImage> images = new HashSet<CImage>();

	protected native void allocNative (boolean resample, int renderid);

	protected native void allocNative2
			(boolean resample, short handle, int [] img, int xSpot, int ySpot,
			 int xAP, int yAP, int width, int height, int renderid);

	protected native void allocNative4
			(boolean resample, CFile file, int renderid);

	protected native void allocNative5
			(boolean resample, short handle, byte[] buffer, boolean transparent, boolean first_pixel, int color_transp, int renderid);

	private native void freeNative ();

	static int renderID = SurfaceView.ES;

	public CImage ()
	{
		synchronized(images)
		{
			allocNative ((MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0, renderID);

			images.add (this);
			//Log.v("MMFRuntime", "starting antialias: "+(this.getAntialias()? "yes":"no"));
		}
	}

	public CImage (boolean antialiased)
	{
		synchronized(images)
		{
			allocNative (antialiased, renderID);

			images.add (this);
			//Log.v("MMFRuntime", "starting antialias: "+(this.getAntialias()? "yes":"no"));
		}
	}

	/* For CImageBank */

	public CImage
			(short handle, Bitmap img, int xSpot, int ySpot,
			 int xAP, int yAP, int useCount, int width, int height, boolean antialiased)
	{

		synchronized(img) {
			allocNative2
					(antialiased,
							handle, getBitmapPixels (img), xSpot, ySpot,
							xAP, yAP, width, height, renderID);

			images.add (this);
			//Log.v("MMFRuntime", "from bank antialias: "+(this.getAntialias()? "yes":"no"));
		}
	}

	/* For the joystick images */

	public CImage (String resource, boolean antialiased)
	{
		Bitmap img = null;

		try
		{
			img = BitmapFactory.decodeResource (MMFRuntime.inst.getResources (),
					MMFRuntime.inst.getResourceID (resource));
		}
		catch (Throwable e)
		{
		}

		if (img == null)
			throw new RuntimeException ("Bad image resource : " + resource);

		synchronized(img)
		{
			allocNative2 (antialiased,
					(short) -1, getBitmapPixels (img), 0, 0, 0,
					0, img.getWidth (), img.getHeight (), renderID);

			images.add (this);
		}
		img.recycle();
	}

	/* For the Active Picture object (to load a file) */

	public CImage (InputStream input, boolean antialiased, boolean transparent, boolean first_pixel, int color_transp)
	{
		long start = SystemClock.currentThreadTimeMillis();

		int bufferSize = 128 * 1024;
		byte[] tmp_buffer = new byte[bufferSize];
		ByteArrayOutputStream byteBuffer = new ByteArrayOutputStream();
		byte[] buffer;

		try {
			int len = 0;
			while ((len = input.read(tmp_buffer)) != -1) {
				byteBuffer.write(tmp_buffer, 0, len);
			}
			buffer = byteBuffer.toByteArray();
		} catch (IOException e) {
			Log.v("MMFRuntime", "error reading inputstream for apo ...");
			buffer = null;
		}
		//Log.v("MMFRuntime", "reading buffer took(msecs) "+ (SystemClock.currentThreadTimeMillis()-start));

		if (buffer != null && buffer.length > 0)
		{
			allocNative5 (antialiased, (short) -1, buffer, transparent, first_pixel, color_transp, renderID);
			images.add(this);
		}
		Log.v("MMFRuntime", "load apo texture took(msecs) "+ (SystemClock.currentThreadTimeMillis()-start));
	}

	public long ptr;

	public native CMask getMask(int nFlags, int angle, double scaleX, double scaleY);

	public native int getXSpot ();
	public native int getYSpot ();

	public native void setXSpot (int x);
	public native void setYSpot (int y);

	public native int getXAP ();
	public native int getYAP ();

	public native int setXAP (int x);
	public native int setYAP (int y);

	public native boolean getResampling();
	public native void setResampling (boolean antialias);

	@Override
	public native int getWidth();
	@Override
	public native int getHeight();

	public native int getPixel (int x, int y);

	public native int createTexture(int width, int height, boolean resample);

	public native int createTextureOES(int width, int height, boolean resample);

	@Override
	public native int texture ();

	/**
	 * Can be use as the following example
	 *
	 *	CImage cImage = ho.getImageBank().getImageFromHandle(Images[i]);
	 * 				...
	 * 	int[] mImage = cImage.getRawPixels();
	 * 	if(mImage == null)
	 *  	return;
	 *  image = Bitmap.createBitmap(cImage.getWidth(), cImage.getHeight(), cImage.getFormat());
	 * 	image.setPixels(mImage, 0, cImage.getWidth(), 0, 0, cImage.getWidth(), cImage.getHeight());
	 */
	public native int[] getRawPixels();

	public native short imageFormat();

	public native   int imageBegin();

	public native   int imageSize();

	public native void imageSetData(int [] pixels);

	public enum FORMATS {
		//texture Enums
		RGBA8888,   // 32 bit
		RGBA4444,   // 16 bit
		RGBA5551,   // 16 bit
		RGB888,     // 24 bit
		RGB565,     // 16 bit
		JPEG,		// store JPEG
		PNG			// store PNG
	}

	public Config getFormat() {
		/*
    	texture Enums
    	RGBA8888,   // 32 bit
        RGBA4444,   // 16 bit
        RGBA5551,   // 16 bit
        RGB888,     // 24 bit
        RGB565      // 16 bit
		 */
		int format = imageFormat();

		switch(format)
		{
			case 0:
				return Config.ARGB_8888;

			case 1:
				return Config.ARGB_4444;

			case 3:
				return Config.ALPHA_8;

			case 2:
			case 4:
				return Config.RGB_565;

		};

		return Config.ARGB_8888;
	}

	public native void deuploadNative ();

	public void destroy ()
	{
		if (ptr == 0)
			return;
		synchronized(images) {
			images.remove (this);

			onDestroy ();

			deuploadNative ();
			freeNative ();
		}
		ptr = 0;
	}

	public abstract void onDestroy ();

	/**
	 * update texture with a pixels integer array 
	 *
	 * @param array of pixels
	 * @param width image size
	 * @param height image size
	 * @param flag != 0 a new texture will be created when updating
	 */
	public native void updateWith (int [] pixels, int width, int height);

	/**
	 * create a texture from screen 
	 *
	 * @param x, horizontal position in pixels
	 * @param y, vertical position in pixels
	 * @param width image size
	 * @param height image size
	 * @param vpheight, viewPort height
	 */
	public native void screenAreaToTexture (int x, int y, int width, int height, int vpwidth, int vpheight );

	public void updateWith (Bitmap b)
	{
		updateWith (getBitmapPixels (b), b.getWidth (), b.getHeight ());
	}

	public void imageSetData (Bitmap img)
	{
		if(img == null)
			return;
		int [] pixels = new int [img.getWidth () * img.getHeight ()];
		img.getPixels (pixels, 0, img.getWidth (), 0, 0, img.getWidth (), img.getHeight ());
		imageSetData (pixels);
	}

	public native short getHandle ();

	public native void getInfo (CImageInfo dest, int nAngle, float fScaleX, float fScaleY);

	public native void flipHorizontally ();
	public native void flipVertically ();

	public int [] getBitmapPixels (Bitmap img)
	{
		if(img == null)
			return null;
		int [] pixels = new int [img.getWidth () * img.getHeight ()];
		img.getPixels (pixels, 0, img.getWidth (), 0, 0, img.getWidth (), img.getHeight ());
		return pixels;
	}


}

