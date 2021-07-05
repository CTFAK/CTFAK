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

package OpenGL;

import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Matrix;
import android.opengl.GLES20;
import android.opengl.GLES30;
import android.os.Handler;
import android.os.Message;
import android.view.View;

import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import javax.microedition.khronos.egl.EGLConfig;

import Application.CRunApp;
import Runtime.CrashReporter;
import Runtime.Log;
import Runtime.MMFRuntime;


public class ES3Renderer extends GLRenderer
{
	public static ES3Renderer inst;

	private long ptr;

	public ES3Renderer()
	{
		System.loadLibrary ("ES3Renderer");

		allocNative ();
	}

	private native void allocNative ();

	@Override
	protected native void setBase (int x, int y);

	@Override
	protected native int getBaseX ();
	@Override
	protected native int getBaseY ();

	@Override
	public native void setLimitX (int limit);
	@Override
	public native void setLimitY (int limit);

	boolean didStretch;

	int oldLimitX;
	int oldLimitY;

	@Override
	public void beginWholeScreenDraw ()
	{
		MMFRuntime runtime =  MMFRuntime.inst;

		setViewport(0, 0, runtime.currentWidth, runtime.currentHeight);
		setProjectionMatrix (0, 0,  runtime.currentWidth, runtime.currentHeight);

		oldLimitX = GLRenderer.limitX;
		GLRenderer.limitX = runtime.currentWidth;
		setLimitX(GLRenderer.limitX);

		oldLimitY = GLRenderer.limitY;
		GLRenderer.limitY = runtime.currentHeight;
		setLimitY(GLRenderer.limitY);
	}

	@Override
	public void endWholeScreenDraw ()
	{
		updateViewport (didStretch);

		GLRenderer.limitX = oldLimitX;
		setLimitX(oldLimitX);

		GLRenderer.limitY = oldLimitY;
		setLimitY(oldLimitY);
	}

	@SuppressLint("NewApi")
	@Override
	public void updateViewport (boolean stretchWindowToViewport)
	{
		didStretch = stretchWindowToViewport;

		/* Be sure not to leave logging information in here, as it can potentially be
		 * called every frame by endWholeScreenDraw.
		 */

		if(MMFRuntime.inst.app.joystick != null)
			MMFRuntime.inst.app.joystick.setPositions();

		setViewport (MMFRuntime.inst.viewportX, MMFRuntime.inst.viewportY,
				MMFRuntime.inst.viewportWidth, MMFRuntime.inst.viewportHeight);

		if(stretchWindowToViewport)
		{
			setProjectionMatrix (0, 0, MMFRuntime.inst.app.gaCxWin, MMFRuntime.inst.app.gaCyWin);
		}
		else
		{
			setProjectionMatrix (0, 0, MMFRuntime.inst.viewportWidth, MMFRuntime.inst.viewportHeight);
		}
	}

	@Override
	protected void scissor (boolean enabled)
	{
		setScissor(enabled);
	}

	@SuppressLint("NewApi")
	@Override
	protected void scissor (int x, int y, int w, int h)
	{
		doScissor (x, y, w, h);
	}

	@SuppressLint("NewApi")
	public void onSurfaceCreated(EGLConfig config)
	{
		Log.Log ("ES3Renderer/onSurfaceCreated");

		boolean isFirstCreate = (inst == null);

		inst = this;

		MMFRuntime.installCrashReporter ();

		// Using GLES20 or GLES30 will bring the same result
		gpu = GLES30.glGetString(GLES30.GL_RENDERER);
		gpuVendor = GLES30.glGetString(GLES30.GL_VENDOR);
		glVersion = GLES30.glGetString(GLES30.GL_VERSION);

		Log.Log ("GL version: " + GLES30.glGetString(GLES30.GL_VERSION));

		Log.Log("ES3Renderer : Started - GPU " + gpu + ", vendor " + gpuVendor);

		CrashReporter.addInfo ("GPU", gpu);
		CrashReporter.addInfo ("GPU Vendor", gpuVendor);

		if ((MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_REQUIREGPU) != 0)
		{
			if (gpu.indexOf ("PixelFlinger") != -1)
			{
				MMFRuntime.inst.app.hdr2Options &= ~ CRunApp.AH2OPT_AUTOEND;
				MMFRuntime.inst.mainView.setVisibility(View.INVISIBLE);

				AlertDialog alertDialog;

				alertDialog = new AlertDialog.Builder(MMFRuntime.inst).create();
				alertDialog.setTitle("No GPU detected");

				alertDialog.setButton (DialogInterface.BUTTON_NEUTRAL, "Close", Message.obtain(new Handler (), new Runnable ()
				{   @Override
					public void run ()
				{   System.exit (0);
				}
				}));

				alertDialog.setMessage("This application requires a GPU, but none was detected.");

				alertDialog.setOnDismissListener (new DialogInterface.OnDismissListener()
				{   @Override
					public void onDismiss (DialogInterface d)
				{   System.exit (0);
				}
				});

				alertDialog.show();

				return;
			}
		}

		setInitialSettings();

		// Thread.currentThread().setPriority(Thread.MAX_PRIORITY);

		if (!isFirstCreate)
		{
			if (MMFRuntime.inst.app != null
					&& MMFRuntime.inst.app.run != null)
			{
				MMFRuntime.inst.app.run.reinitDisplay ();
				MMFRuntime.inst.app.run.resume();
			}
		}

		updateViewport(didStretch);
		MMFRuntime.inst.setFrameRate(MMFRuntime.inst.app.gaFrameRate);
	}

	@Override
	public void fillZone (int x, int y, int w, int h, int color)
	{
		renderGradient (x, y, w, h, color, color, true, 0, 0);
	}

	public native void setProjectionMatrix (int x, int y, int w, int h);

	public native void setViewport (int x, int y, int w, int h);

	public native void setScissor (boolean bEnable);

	public native void doScissor (int x, int y, int w, int h);

	public native void setInitialSettings ();

	public native void readScreenPixels(int x, int y, int width, int height, ByteBuffer bb);

	@Override
	public native void clear (int color);

	@Override
	public void fillZone (int x, int y, int w, int h, int color, int inkEffect, int inkEffectParam)
	{
		renderGradient (x, y, w, h, color, color, true, inkEffect, inkEffectParam);
	}

	@Override
	public native void renderPoint
	(ITexture image, int x, int y, int inkEffect, int inkEffectParam);

	@Override
	public native void renderGradient
	(int x, int y, int w, int h, int color1, int color2, boolean vertical, int inkEffect, int inkEffectParam);

	@Override
	public native void setInkEffect
	(int effect, int effectParam);

	@Override
	public native void renderImage
	(ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam);

	@Override
	public native void renderScaledRotatedImage
	(ITexture image, float angle, float sX, float sY, int hX, int hY,
			int x, int y, int w, int h, int inkEffect, int inkEffectParam);

	@Override
	public native void renderScaledRotatedImage2
	(ITexture image, float angle, float sX, float sY, 
			int bHotSpot, int x, int y, int inkEffect, int inkEffectParam);

	@Override
	public native void renderScaledRotatedImageWrapAndFlip
	(ITexture image, float angle, float sX, float sY, int hX, int hY, 
			int x, int y, int w, int h, int inkEffect, int inkEffectParam, int offsetX, int offsetY, int wrap, int flipH, int flipV, int resample, int transp);

	@Override
	public native void renderPattern
	(ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam);

	@Override
	public native void renderLine
	(int xA, int yA, int xB, int yB, int color, int thickness);

	@Override
	public void renderRect
	(int x, int y, int w, int h, int color, int thickness)
	{
		renderGradient(x, y, w - thickness, thickness, color, color, true, 0, 0);
		renderGradient(x, y, thickness, h - thickness, color, color, true, 0, 0);
		renderGradient(x, y + h - thickness, w - thickness, thickness, color, color, true, 0, 0);
		renderGradient(x + w - thickness, y, thickness, h, color, color, true, 0, 0);

	}

	/* ES 3.0 additions (b31) */

	@Override
	public native void renderGradientEllipse
	(int x, int y, int w, int h, int color1, int color2, boolean vertical, int inkEffect, int inkEffectParam);

	@Override
	public native void renderPatternEllipse
	(ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam);

	@Override
	public native void readScreenToTexture
	(ITexture image, int x, int y, int w, int h);   

	@Override
	public native void renderPerspective
	(ITexture image, int x, int y, int w, int h, float fA, float fB, int pDir, int inkEffect, int inkEffectParam);  

	@Override
	public native void renderSinewave
	(ITexture image, int x, int y, int w, int h, float Zoom, float WaveIncrement, float Offset, int pDir, int inkEffect, int inkEffectParam);   


	@Override
	public native int addShaderFromString(String shaderName, String vertexShader, String fragmentShader, String[] shaderVariables, boolean useTexCoord, boolean useColors);

	@Override
	public native int addShaderFromFile(String shaderName, String[] shaderVariables, boolean useTexCoord, boolean useColors);

	@Override
	public native void removeShader(int shaderIndex);

	@Override
	public native void setEffectShader(int shaderIndex);

	@Override
	public native void removeEffectShader();

	@Override
	public native void updateVariable1i(String varName, int value);

	@Override
	public native void updateVariable1ibyIndex(int varIndex, int value);

	@Override
	public native void updateVariable1f(String varName, float value);

	@Override
	public native void updateVariable1fbyIndex(int varIndex, float value);

	@Override
	public native void updateVariable2i(String varName, int value0, int value1);

	@Override
	public native void updateVariable2ibyIndex(int varIndex, int value0, int value1);

	@Override
	public native void updateVariable2f(String varName, float value0, float value1);

	@Override
	public native void updateVariable2fbyIndex(int varIndex, float value0, float value1);

	@Override
	public native void updateVariable3i(String varName, int value0, int value1, int value2);

	@Override
	public native void updateVariable3ibyIndex(int varIndex, int value0, int value1, int value2);

	@Override
	public native void updateVariable3f(String varName, float value0, float value1, float value2);

	@Override
	public native void updateVariable3fbyIndex(int varIndex, float value0, float value1, float value2);

	@Override
	public native void updateVariable4i(String varName, int value0, int value1, int value2, int value3);

	@Override
	public native void updateVariable4ibyIndex(int varIndex, int value0, int value1, int value2, int value3);

	@Override
	public native void updateVariable4f(String varName, float value0, float value1, float value2, float value3);

	@Override
	public native void updateVariable4fbyIndex(int varIndex, float value0, float value1, float value2, float value3);

	@Override
	public native void updateVariableMat4f(String varName, float[] value0);

	@Override
	public native void updateVariableMat4fbyIndex(int varIndex, float[] value0);

	/* Called by native code */

	public String loadShader (String name)
	{
		try
		{
			InputStream stream =
					(MMFRuntime.inst.getApplicationContext().getResources().openRawResource
							(MMFRuntime.inst.getResourceID ("raw/" + name)));

			byte[] bytes = new byte[stream.available()];
			stream.read(bytes);
			return new String(bytes);
		}
		catch (Throwable e)
		{
			return "";
		}
	}

	@Override
	public Bitmap drawBitmap(int x, int y, int w, int h, Bitmap.Config config) {

		x = (int)(x*MMFRuntime.inst.scaleX+0.5f);
		y = (int)(y*MMFRuntime.inst.scaleY+0.5f);

		w = (int)(w*MMFRuntime.inst.scaleX+0.5f);
		h = (int)(h*MMFRuntime.inst.scaleY+0.5f);

		x += MMFRuntime.inst.viewportX;
		y += MMFRuntime.inst.viewportY;

		int screenwidth  = MMFRuntime.inst.currentWidth;
		int screenheight = MMFRuntime.inst.currentHeight;

		int screenshotSize = screenwidth * screenheight;

		ByteBuffer bb = ByteBuffer.allocateDirect(screenshotSize * 4);
		bb.order(ByteOrder.LITTLE_ENDIAN);
		bb.rewind();

		GLES30.glGetError();
		GLES30.glFinish();
		GLES30.glReadPixels(0, 0, screenwidth, screenheight, GLES30.GL_RGBA, GLES30.GL_UNSIGNED_BYTE, bb);
		int glerror = GLES30.glGetError();
		if(glerror != GLES30.GL_NO_ERROR)
			Log.Log("Error: "+Integer.toHexString(glerror));
		bb.rewind();
		Bitmap bmp = Bitmap.createBitmap(screenwidth, screenheight, config);
		bmp.copyPixelsFromBuffer(bb);
		Matrix matrix = new Matrix();
		matrix.preScale(1f, -1f);
		Bitmap bitmap = Bitmap.createBitmap(bmp, 0, 0, bmp.getWidth(), bmp.getHeight(), matrix, false);

		Bitmap ret = Bitmap.createBitmap(w, h, bmp.getConfig());
		Canvas canvas = new Canvas(ret);
		canvas.drawBitmap(bitmap, -x, -y, null);

		bmp.recycle();
		bmp = null;
		bitmap.recycle();
		bitmap = null;
		return ret;
	}

	@Override
	public boolean readFramePixels(int[] pixels, int x, int y, int width, int height, Bitmap.Config config) {

		int w = width;
		int h = height;
		
       x *= MMFRuntime.inst.scaleX;
        y *= MMFRuntime.inst.scaleY;

        w *= MMFRuntime.inst.scaleX;
        h *= MMFRuntime.inst.scaleY;

        x += MMFRuntime.inst.viewportX;
        y += MMFRuntime.inst.viewportY;

		int screenshotSize = w * h;

		ByteBuffer bb = ByteBuffer.allocateDirect(screenshotSize * 4);
		bb.order(ByteOrder.LITTLE_ENDIAN);

		bb.rewind();

		readScreenPixels(x, MMFRuntime.inst.currentHeight - y - h, w, h, bb);

		bb.rewind();

		Bitmap bitmap = Bitmap.createBitmap(w, h, config);
		bitmap.copyPixelsFromBuffer(bb);
		if(bitmap == null)
			return false;
		bb = null;

		Matrix matrix = new Matrix();
		matrix.preScale((float)width/bitmap.getWidth(), -1.0f*height/bitmap.getHeight());
		Bitmap resizedBitmap = Bitmap.createBitmap(bitmap, 0, 0, bitmap.getWidth(), bitmap.getHeight(), matrix, false);

		if(pixels == null)
			pixels = new int[resizedBitmap.getWidth() * resizedBitmap.getHeight()];

		resizedBitmap.getPixels(pixels, 0, resizedBitmap.getWidth(), 0, 0, resizedBitmap.getWidth(), resizedBitmap.getHeight());

		resizedBitmap.recycle();
		bitmap.recycle();
		bitmap=null;

		return true; 
	}

	@Override
	public void readFrameToTexture(ITexture image, int x, int y, int w, int h) {

        x *= MMFRuntime.inst.scaleX;
        y *= MMFRuntime.inst.scaleY;

        w *= MMFRuntime.inst.scaleX;
        h *= MMFRuntime.inst.scaleY;

        x += MMFRuntime.inst.viewportX;
        y += MMFRuntime.inst.viewportY;
	
		readScreenToTexture(image, x, MMFRuntime.inst.currentHeight - y - h, w, h);
		//readScreenToTexture(image, x, y, w, h);
	}

	@Override
	public void renderFlush() {
		GLES20.glFlush();
	}

	@Override
	public void renderBlitFull(ITexture image) {
		renderImage(image, 0, 0, image.getWidth(), image.getHeight(), 0, 0);
	}

	@Override
	public void renderBlit(ITexture image, int xDst, int yDst, int xSrc, int ySrc, int width, int height) {

	}
}
