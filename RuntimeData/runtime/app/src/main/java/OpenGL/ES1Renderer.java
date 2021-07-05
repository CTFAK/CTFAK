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

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;
import javax.microedition.khronos.opengles.GL11;

import Application.CRunApp;
import Runtime.CrashReporter;
import Runtime.Log;
import Runtime.MMFRuntime;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Matrix;
import android.opengl.GLDebugHelper;
import android.opengl.GLES11;
import android.os.Handler;
import android.os.Message;
import android.view.View;

public class ES1Renderer extends GLRenderer
{
    public GL11 gl;

    public ES1Renderer ()
    {
        System.loadLibrary ("ES1Renderer");

        allocNative ();
    }

    private native void allocNative ();

    boolean didStretch;
    
    int oldLimitX;
    int oldLimitY;
    
    @Override
	public void beginWholeScreenDraw ()
    {
        gl.glMatrixMode(GL10.GL_MODELVIEW);
        gl.glLoadIdentity();

        gl.glMatrixMode(GL10.GL_PROJECTION);
        gl.glLoadIdentity();

        MMFRuntime runtime =  MMFRuntime.inst;

        gl.glViewport(0, 0, runtime.currentWidth, runtime.currentHeight);
        //Get the center of projection
        gl.glOrthof(0, runtime.currentWidth, runtime.currentHeight, 0, -1.0f, 1.0f);

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
    
    @Override
    public void updateViewport (boolean stretchWindowToViewport)
    {
    	didStretch = stretchWindowToViewport;
    	
        /* Be sure not to leave logging information in here, as it can potentially be
         * called every frame by endWholeScreenDraw.
         */

        if(MMFRuntime.inst.app.joystick != null)
            MMFRuntime.inst.app.joystick.setPositions();

        MMFRuntime runtime = MMFRuntime.inst;

        gl.glMatrixMode (GL10.GL_MODELVIEW);
        gl.glLoadIdentity();

        gl.glMatrixMode(GL10.GL_PROJECTION);
        gl.glLoadIdentity();

        if(stretchWindowToViewport)
        {
            gl.glViewport(runtime.viewportX, runtime.viewportY,
                    runtime.viewportWidth, runtime.viewportHeight);

            gl.glOrthof(0f, MMFRuntime.inst.app.gaCxWin, MMFRuntime.inst.app.gaCyWin, 0f, 1.0f, -1.0f);
        }
        else
        {
            gl.glViewport(runtime.viewportX, runtime.viewportY,
                    runtime.viewportWidth, runtime.viewportHeight);

            gl.glOrthof(0f, runtime.viewportWidth, runtime.viewportHeight, 0f, 1.0f, -1.0f);
        }
    }

    public void onSurfaceCreated(GL11 gl, EGLConfig config)
    {
        this.gl = gl;

        Log.Log ("ES1Renderer/onSurfaceCreated");

        boolean isFirstCreate = (inst == null);

        inst = this;

        MMFRuntime.installCrashReporter ();

        if (MMFRuntime.debugLevel >= 2)
        {
            this.gl = (GL11) GLDebugHelper.wrap(gl, GLDebugHelper.CONFIG_CHECK_GL_ERROR |
                    GLDebugHelper.CONFIG_LOG_ARGUMENT_NAMES, debugWriter);
        }

        gpu = gl.glGetString(GL10.GL_RENDERER);
        gpuVendor = gl.glGetString(GL10.GL_VENDOR);
        glVersion = gl.glGetString(GL10.GL_VERSION);

        Log.Log("ES1Renderer : Started - GPU " + gpu + ", vendor " + gpuVendor);

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


        gl.glEnable(GL10.GL_BLEND);
        gl.glBlendFunc(GL10.GL_SRC_ALPHA, GL10.GL_ONE_MINUS_SRC_ALPHA);

        gl.glEnable(GL10.GL_TEXTURE_2D);

        gl.glEnableClientState(GL10.GL_VERTEX_ARRAY);
        gl.glEnableClientState(GL10.GL_TEXTURE_COORD_ARRAY);
        gl.glDisableClientState(GL10.GL_COLOR_ARRAY);

        gl.glDisable (GL10.GL_CULL_FACE);

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
	public native void clear (int color);
    @Override
	public native void fillZone (int x, int y, int w, int h, int color, int inkEffect, int inkEffectParam);
    
    public native void readScreenPixels(int x, int y, int width, int height, ByteBuffer bb);


    @Override
	public void fillZone (int x, int y, int w, int h, int color)
    {
        fillZone (x, y, w, h, color, 0, 0);
    }

    @Override
	protected void scissor (boolean enabled)
    {
        if (enabled)
            gl.glEnable (GL10.GL_SCISSOR_TEST);
        else
            gl.glDisable (GL10.GL_SCISSOR_TEST);
    }

    @Override
	protected void scissor (int x, int y, int w, int h)
    {
        gl.glScissor (x, y, w, h);
    }

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
	public native void renderRect
            (int x, int y, int w, int h, int color, int thickness);

    public void renderPerspective(ITexture image, int x, int y, int w, int h, float fA, float fB, int pDir, int inkEffect, int inkEffectParam) {	
    }
    
    public void readScreenToTexture(ITexture image, int x, int y, int w, int h, int width, int height)
    {
    	     	
    }
    
    public void renderSinewave(ITexture image, int x, int y, int w, int h, float Zoom, float WaveIncrement, float Offset, int pDir, int inkEffect, int inkEffectParam) {	
    }

    /* ES 2.0 only */

	public int addShaderFromString(String shaderName, String vertexShader, String fragmentShader, String[] shaderVariables, boolean useTexCoord, boolean useColors) {
		return -1;
	}

	public int addShaderFromFile(String shaderName, String[] shaderVariables, boolean useTexCoord, boolean useColors) {
		return -1;
	}

	public void removeShader(int shaderIndex) {
	}

	public void setEffectShader(int shaderIndex) {
	}

	public void removeEffectShader() {
	}

	public void updateVariable1i(String varName, int value) {
	}

	public void updateVariable1ibyIndex(int varIndex, int value) {
	}

	public void updateVariable1f(String varName, float value) {
	}

	public void updateVariable1fbyIndex(int varIndex, float value) {
	}

	public void updateVariable2i(String varName, int value0, int value1) {
	}

	public void updateVariable2ibyIndex(int varIndex, int value0, int value1) {
	}

	public void updateVariable2f(String varName, float value0, float value1) {
	}

	public void updateVariable2fbyIndex(int varIndex, float value0, float value1) {
	}

	public void updateVariable3i(String varName, int value0, int value1, int value2) {
	}

	public void updateVariable3ibyIndex(int varIndex, int value0, int value1, int value2) {
	}

	public void updateVariable3f(String varName, float value0, float value1, float value2) {
	}

	public void updateVariable3fbyIndex(int varIndex, float value0, float value1, float value2) {
	}

	public void updateVariable4i(String varName, int value0, int value1, int value2, int value3) {
	}

	public void updateVariable4ibyIndex(int varIndex, int value0, int value1, int value2, int value3) {
	}

	public void updateVariable4f(String varName, float value0, float value1, float value2, float value3) {
	}

	public void updateVariable4fbyIndex(int varIndex, float value0, float value1, float value2, float value3) {
	}

	public void renderImagewithShader(ITexture image, int x, int y, int w, int h, int nshader, int inkEffect, int inkEffectParam) {
	}

	public void updateVariableMat4f(String varName, float[] value0) {
	}

	public void updateVariableMat4fbyIndex(int varIndex, float[] value0) {
	}

   @Override
	public void renderGradientEllipse
            (int x, int y, int w, int h, int color1, int color2, boolean vertical, int inkEffect, int inkEffectParam)
    {
        renderGradient (x, y, w, h, color1, color2, vertical, inkEffect, inkEffectParam);
    }

    @Override
	public void renderPatternEllipse
            (ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam)
    {
        renderPattern (image, x, y, w, h, inkEffect, inkEffectParam);
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

        GLES11.glGetError();
        GLES11.glFinish();
        GLES11.glReadPixels(0, 0, screenwidth, screenheight, GLES11.GL_RGBA, GLES11.GL_UNSIGNED_BYTE, bb);
        int glerror = GLES11.glGetError();
        if(glerror != GLES11.GL_NO_ERROR)
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
     	
     	int w = (int)(width*MMFRuntime.inst.scaleX);
     	int h = (int)(height*MMFRuntime.inst.scaleY);

     	int screenshotSize = w * h;
     	ByteBuffer bb = ByteBuffer.allocateDirect(screenshotSize * 4);
     	bb.order(ByteOrder.nativeOrder());
     	
     	    x = (int)(x*MMFRuntime.inst.scaleX+MMFRuntime.inst.viewportX);
     	    y = (int)(MMFRuntime.inst.viewportHeight+MMFRuntime.inst.viewportY-(y+height)*MMFRuntime.inst.scaleY);
     	
    	readScreenPixels(x, y, w, h, bb);
     	  	
     	int pixelsBuffer[] = new int[screenshotSize];
     	bb.asIntBuffer().get(pixelsBuffer);
     	
     	bb = null; 
    	Bitmap bitmap = Bitmap.createBitmap(w, h, config);
    	bitmap.setPixels(pixelsBuffer, screenshotSize-w, -w, 0, 0, w, h);
    	
        Bitmap resizedBitmap = Bitmap.createScaledBitmap(bitmap, width, height, true);
        bitmap.recycle();

        if(pixels == null)
        	pixels = new int[width * height];
        
     	resizedBitmap.getPixels(pixels, 0, width, 0, 0, width, height);
        resizedBitmap.recycle();
        
     	return true;
     }
       
    @Override
 	public void readFrameToTexture(ITexture image, int x, int y, int width, int height) {

     	int w = (int)(width*MMFRuntime.inst.scaleX);
     	int h= (int)(height*MMFRuntime.inst.scaleY);

  	    x = (int)(x*MMFRuntime.inst.scaleX+MMFRuntime.inst.viewportX);
 	    y = (int)(MMFRuntime.inst.viewportHeight-(y+height)*MMFRuntime.inst.scaleY+MMFRuntime.inst.viewportY);
   	
 	    readScreenToTexture(image, x, y, w, h, width, height);
     }
    
    @Override
    public void renderFlush() {
    	gl.glFlush();
    }
    
    @Override
    public void renderBlitFull(ITexture image) {
    	renderImage(image, 0, 0, image.getWidth(), image.getHeight(), 0, 0);
    }
    
    @Override
    public void renderBlit(ITexture image, int xDst, int yDst, int xSrc, int ySrc, int width, int height) {
    	
    }

	@Override
	public void readScreenToTexture(ITexture image, int x, int y, int w, int h) {
		// TODO Auto-generated method stub
		
	}

}



