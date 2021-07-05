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
package Runtime;

import android.annotation.SuppressLint;
import android.app.ActivityManager;
import android.content.Context;
import android.content.pm.ConfigurationInfo;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.PixelFormat;
import android.opengl.GLES10;
import android.os.SystemClock;
import android.view.InputDevice;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.SurfaceHolder;

import java.util.HashSet;
import java.util.Iterator;
import java.util.Locale;
import java.util.Set;

import javax.microedition.khronos.egl.EGL10;
import javax.microedition.khronos.egl.EGL11;
import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.egl.EGLContext;
import javax.microedition.khronos.egl.EGLDisplay;
import javax.microedition.khronos.egl.EGLSurface;
import javax.microedition.khronos.opengles.GL10;
import javax.microedition.khronos.opengles.GL11;

import Application.CJoystickFIRETV;
import Application.CJoystickNEXUSTV;
import Application.CJoystickOUYA;
import Banks.CImage;
import Extensions.CRunFIRETV;
import Extensions.CRunNEXUSTV;
import Extensions.CRunOUYA;
import OpenGL.ES1Renderer;
import OpenGL.ES2Renderer;
import OpenGL.ES3Renderer;
import OpenGL.GLRenderer;


public class SurfaceView extends android.view.SurfaceView implements SurfaceHolder.Callback
{
	public static SurfaceView inst;

	public static boolean hasSurface = false;
	public static int ES = 1;
	private static boolean eglInit = false;

	public int pixelFormat = PixelFormat.RGBA_8888;

	public int MODE_888 = 2;
	public int MODE_565 = 1;
	public int MODE_444 = 0;

	boolean match = true;

	public GLRenderer renderer;

	protected EGL10 egl;

	public EGLDisplay eglDisplay;
	public EGLContext eglContext;
	public EGLSurface eglSurface;
	public EGLConfig  eglConfig;
	public int eglError;

	public static int maxSize;
	
	public boolean LOG_THREADS = true;

	boolean hasUiContext;
	boolean hasType;

	private Set <CImage> images = new HashSet<CImage>();

	private int OuyaPlayer = -1;
	private int nexusPlayer = -1;
	private int firePlayer = -1;
	private boolean initOUYA=false;
	private boolean initFireTV=false;
	private boolean initNexusTV=false;

	@SuppressWarnings("deprecation")
	public SurfaceView(Context context)
	{
		super(context);

		inst = this;

		getHolder().addCallback(this);
		if(MMFRuntime.deviceApi < 17)
			getHolder().setType(SurfaceHolder.SURFACE_TYPE_GPU);
		else
			getHolder().setFormat(PixelFormat.TRANSLUCENT);
			
		pixelFormat = MMFRuntime.inst.getWindowManager().getDefaultDisplay().getPixelFormat();

		Log.Log ("Pixel Format from display: "+pixelFormat);

		if(MMFRuntime.FIRETV && !initFireTV) {
			CRunFIRETV.init(MMFRuntime.inst);
			initFireTV = true;
		}

		if(MMFRuntime.NEXUSTV && !initNexusTV) {
			CRunNEXUSTV.init(MMFRuntime.inst);
			initNexusTV = true;
		}

		if(MMFRuntime.OUYA && !initOUYA) {
			CRunOUYA.init(MMFRuntime.inst);
			initOUYA = true;
		}

		renderer = null;

		hasType = (MMFRuntime.deviceApi > 17);

		Log.Log("hasType: "+(hasType?"latest":"oldest"));
		Log.Log("OpenGL2.0: "+(detectOpenGLES20()?"yes":"no"));

	}

	public void setPreserveContextOnPause(boolean bflag) {
		this.preserve = bflag;
	}

	public void firstWait() {
		egl.eglWaitGL();
	}
	
	public boolean swapBuffers ()
	{
		eglError = EGL10.EGL_NONE;
		/* Does nothing if the surface hasn't yet been created */
		if (!egl.eglSwapBuffers(eglDisplay, eglSurface)) {
			eglError = egl.eglGetError();
			if(eglError == EGL10.EGL_BAD_SURFACE)
				createSurface(this.getHolder());
			return false;
		}
		return true;
	}
	
	public boolean makeSurface() 
	{
		createSurface(this.getHolder());
		return hasSurface;
	}

	boolean preserve = false;
	boolean reinit = false;

	@Override
	public void surfaceCreated (SurfaceHolder holder)
	{
		if(!reinit)
			startEGLContext(holder);

		createSurface(holder);

		if(hasSurface)
			makeCurrent();

		if(!reinit)
			setRenderer(SurfaceView.ES);

		if (reinit)
			MMFRuntime.inst.updateViewport();

		reinit = true;
	}

	private void setRenderer(int EGLCLientVersion) {

		switch(EGLCLientVersion) {

		case 1:
			renderer = new ES1Renderer();
			((ES1Renderer) renderer).onSurfaceCreated ((GL11) eglContext.getGL(), eglConfig);
			break;
		case 2:
			renderer = new ES2Renderer();
			((ES2Renderer) renderer).onSurfaceCreated (eglConfig);
			break;
		case 3:
			if(android.os.Build.VERSION.SDK_INT > 17 && detectOpenGLES30()) {
				renderer = new ES3Renderer();
				((ES3Renderer) renderer).onSurfaceCreated (eglConfig);
			}
			else {
				renderer = new ES2Renderer();
				((ES2Renderer) renderer).onSurfaceCreated (eglConfig);
			}
			break;
		default:
			renderer = new ES1Renderer();
			((ES1Renderer) renderer).onSurfaceCreated ((GL11) eglContext.getGL(), eglConfig);

		}
	}

	public void startEGLContext(SurfaceHolder holder) {
		Log.Log("Thread " + Thread.currentThread() + " creating surface w/ holder " + holder);

		egl = (EGL10) EGLContext.getEGL();

		eglDisplay = egl.eglGetDisplay(EGL10.EGL_DEFAULT_DISPLAY);

		Log.Log("Got eglDisplay : "+eglDisplay);

		if (eglDisplay == EGL10.EGL_NO_DISPLAY)
			throwEglException("eglGetDisplay failed");


		if(eglInit)
			eglInit = !egl.eglTerminate(eglDisplay);

		int[] version = new int[2];

		eglInit = egl.eglInitialize(eglDisplay, version);

		if(!eglInit)
			throwEglException ("Not Initialized due to "+getErrorString(egl.eglGetError()));

		Log.Log ("eglInitialize called");

		if(ES == 3 && !detectOpenGLES30())	// just in case is not a ES3 device
			ES = 2;

		eglConfig = new CTEGLConfigChooser(hasType ? MODE_888 : MODE_565, ES, true).chooseConfig(egl, eglDisplay);

		if(eglConfig == null) 
		{
			Log.Log("Choose Secondary Context ...");
			eglConfig = new CTEGLConfigChooser(hasType ? MODE_888 : MODE_565, ES, true).secondaryConfig(egl, eglDisplay);
		}

		Log.Log ("Got eglConfig : " + eglConfig);

		eglContext = egl.eglCreateContext
				(eglDisplay, eglConfig,
						EGL10.EGL_NO_CONTEXT, new int []
								{  0x3098 /* EGL_CONTEXT_CLIENT_VERSION */, ES , EGL10.EGL_NONE });

		Log.Log ("Result from Context : " + getErrorString(egl.eglGetError()));

		hasUiContext = !egl.eglGetCurrentContext().equals(EGL10.EGL_NO_CONTEXT);

		Log.Log ("hasUiContext: " + hasUiContext);
		
	}

	public void createSurface(SurfaceHolder holder) {

		if (eglSurface != null && eglSurface != EGL10.EGL_NO_SURFACE) {

			Log.Log ("createSurface: Making current...");

			egl.eglMakeCurrent(eglDisplay, EGL10.EGL_NO_SURFACE, EGL10.EGL_NO_SURFACE, EGL10.EGL_NO_CONTEXT);
			egl.eglDestroySurface(eglDisplay, eglSurface);
		}

		eglSurface = egl.eglCreateWindowSurface(eglDisplay, eglConfig, holder, null);

		if (eglSurface == null || eglSurface == EGL10.EGL_NO_SURFACE) {
			throwEglException("createWindowSurface");
		}

		if (!egl.eglMakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext)) {
			throwEglException("eglMakeCurrent");
		}

		hasSurface = true;
		
		int[] maxTextureSize = new int[1];
		GLES10.glGetIntegerv(GL10.GL_MAX_TEXTURE_SIZE, maxTextureSize, 0);
		Log.Log("Max texture size = " + maxTextureSize[0]);
		maxSize = maxTextureSize[0];
	}


	public boolean verifyContext() {
		EGLContext currentContext = egl.eglGetCurrentContext();
		return (currentContext != EGL10.EGL_NO_CONTEXT && egl.eglGetError() != EGL11.EGL_CONTEXT_LOST);
	}

	@SuppressWarnings("static-access")
	public boolean makeCurrent ()
	{
		if (eglDisplay != null && eglSurface != null && EGLContext.getEGL() != null)
		{
			if (!egl.eglMakeCurrent (eglDisplay, eglSurface, eglSurface, eglContext))
			{
				Log.Log ("!!! eglMakeCurrent FAILED : " + egl.eglGetError());
			}
			GLRenderer.inst = renderer;
			return true;
		}
		else
		{
			GLRenderer.inst = null;

			Log.Log ("Can't make current");
			return false;
		}
	}

	public void makeCurrentIfNecessary ()
	{
		if (hasUiContext)
			makeCurrent();
	}

	public Bitmap drawBitmap() {
		return drawBitmap(0, 0, getWidth(), getHeight());
	}

	public Bitmap drawBitmap(int x, int y, int w, int h) {
		Bitmap bitmap = null;

		if(SurfaceView.inst != null && MMFRuntime.deviceApi < 17)
			makeCurrentIfNecessary();

		synchronized(GLRenderer.inst)
		{
			if(GLRenderer.inst != null)
				bitmap = GLRenderer.inst.drawBitmap(x, y, w, h, (pixelFormat == 1 ? Bitmap.Config.ARGB_8888 : Bitmap.Config.RGB_565));
			Log.Log("the bitmap returned "+(bitmap != null ? "full":"empty"));
		}
		return bitmap;
	}
	
	@Override
	public void surfaceChanged (android.view.SurfaceHolder holder, int format, int w, int h)
	{
		/* Assuming something higher up will already have called updateViewport */	
		Log.Log("Surface Changed");
	}

	@Override
	public void surfaceDestroyed (SurfaceHolder holder)
	{
		Log.Log("onsurfaceDestroyed");
		shutdown();
	}

	@Override
	protected void onDetachedFromWindow() {
		shutdown();
		super.onDetachedFromWindow();
	} 

	@Override
	public void onDraw (Canvas canvas)
	{
		Log.Log("Surface onDraw");
	}


	public void preCleaning ()
	{
		if(CImage.images != null && CImage.images.size() != 0) {

			makeCurrentIfNecessary();	    		

			synchronized(eglContext) {

				images.addAll (CImage.images);

				Iterator <CImage> iterator = images.iterator ();

				while (iterator.hasNext ()) {
					CImage oImage = iterator.next();

					if(oImage != null)
						oImage.destroy();

				}

				CImage.images.clear();

			}
			Log.Log("Finish to clear Textures");

		}

		SystemClock.sleep(100);

	}

	public void destroySurface() {
		if (egl != null) {
			if (eglSurface != null && eglSurface != EGL10.EGL_NO_SURFACE) {
				egl.eglMakeCurrent(eglDisplay, EGL10.EGL_NO_SURFACE, EGL10.EGL_NO_SURFACE, EGL10.EGL_NO_CONTEXT);
				egl.eglDestroySurface(eglDisplay, eglSurface);
				eglSurface = null;
				hasSurface = false;
				Log.Log("No Surface");
			}
		}
	}

	public void terminateContext() {
		if( MMFRuntime.inst.askToDie) {
			if (eglContext != null) {
				egl.eglDestroyContext(eglDisplay, eglContext);
				eglContext = null;
				eglConfig = null;
				reinit = false;
			}
			if (eglDisplay != null) {
				egl.eglTerminate(eglDisplay);
				eglDisplay = null;
				Log.Log("Finishing EGL");
			}

		}
	}

	public void shutdown ()
	{

		preCleaning();
		destroySurface();

		terminateContext();
	}


	byte getPlayerBits(int keyCode)
	{
		switch(keyCode)
		{
		case KeyEvent.KEYCODE_DPAD_UP:
			return 0x01;

		case KeyEvent.KEYCODE_DPAD_LEFT:
			return 0x04;

		case KeyEvent.KEYCODE_DPAD_RIGHT:
			return 0x08;

		case KeyEvent.KEYCODE_DPAD_DOWN:
			return 0x02;

		case KeyEvent.KEYCODE_BUTTON_Y:
			return 0x20;

		case KeyEvent.KEYCODE_DPAD_CENTER:
		case KeyEvent.KEYCODE_BUTTON_A:
			return 0x10;

		};

		return 0x00;
	}

	@Override
	public boolean onKeyDown(final int keyCode, KeyEvent msg)
	{
		// if key is handled by OUYA will not be re-process by runtime
		boolean handled = false;

		if (MMFRuntime.inst.app == null) {
			Log.Log("No App detected");
			return false;
		}

		if(MMFRuntime.OUYA && MMFRuntime.inst.app.ouyaObjects.size() != 0)
		{
			int jplayer = -1;

			if((jplayer = CRunOUYA.keyDownAtPlayerNumber(keyCode, msg)) != -1)
			{
				final byte[] rhPlayer = CJoystickOUYA.rhPlayer;

				rhPlayer[jplayer] |= getPlayerBits(keyCode);

				if(!MMFRuntime.inst.app.ouyaObjects.isEmpty()) {
					for (Iterator <CRunOUYA> it =
							MMFRuntime.inst.app.ouyaObjects.iterator (); it.hasNext (); )
					{
						it.next().keyDown(keyCode, msg);
					}
				}
			}
		}

		if(MMFRuntime.NEXUSTV && MMFRuntime.inst.app.nexustvObjects.size() != 0)
		{
			int jplayer = -1;

			if((jplayer = CRunNEXUSTV.keyDownAtPlayerNumber(keyCode, msg)) != -1)
			{
				final byte[] rhPlayer = CJoystickNEXUSTV.rhPlayer;

				rhPlayer[jplayer] |= getPlayerBits(keyCode);

				if(!MMFRuntime.inst.app.nexustvObjects.isEmpty()) {
					for (Iterator <CRunNEXUSTV> it =
							MMFRuntime.inst.app.nexustvObjects.iterator (); it.hasNext (); )
					{
						handled = it.next().keyDown(keyCode, msg);
					}
				}
			}
		}

		if(MMFRuntime.FIRETV && MMFRuntime.inst.app.firetvObjects.size() != 0)
		{
			int jplayer = -1;

			if((jplayer = CRunFIRETV.keyDownAtPlayerNumber(keyCode, msg)) != -1)
			{
				final byte[] rhPlayer = CJoystickFIRETV.rhPlayer;

				rhPlayer[jplayer] |= getPlayerBits(keyCode);

				if(!MMFRuntime.inst.app.firetvObjects.isEmpty()) {
					for (Iterator <CRunFIRETV> it = MMFRuntime.inst.app.firetvObjects.iterator (); it.hasNext (); )
					{
						it.next().keyDown(keyCode, msg);
					}
				}
			}

		}

		if(!handled && msg != null) {
			MMFRuntime.inst.app.keyDown(msg.getKeyCode());
		}

		return handled || super.onKeyDown(keyCode, msg);
	}

	@Override
	public boolean onKeyUp(final int keyCode, KeyEvent msg)
	{
		// if key is handled by OUYA will not be re-process by runtime
		boolean handled = false;

		if (MMFRuntime.inst.app == null) {
			Log.Log("No App detected");
			return false;
		}

		if(MMFRuntime.OUYA && MMFRuntime.inst.app.ouyaObjects.size() != 0)
		{
			int jplayer = -1;

			if((jplayer = CRunOUYA.keyUpAtPlayerNumber(keyCode, msg)) != -1)
			{
				final byte[] rhPlayer = CJoystickOUYA.rhPlayer;

				rhPlayer[jplayer] &= ~ getPlayerBits(keyCode);

				if(!MMFRuntime.inst.app.ouyaObjects.isEmpty()) {
					for (Iterator <CRunOUYA> it = MMFRuntime.inst.app.ouyaObjects.iterator (); it.hasNext (); )
					{
						it.next().keyUp(keyCode, msg);
					}					
				}
			}
		}

		if(MMFRuntime.NEXUSTV && MMFRuntime.inst.app.nexustvObjects.size() != 0)
		{
			int jplayer = -1;

			if((jplayer = CRunNEXUSTV.keyUpAtPlayerNumber(keyCode, msg)) != -1)
			{
				final byte[] rhPlayer = CJoystickNEXUSTV.rhPlayer;

				rhPlayer[jplayer] &= ~getPlayerBits(keyCode);

				if(!MMFRuntime.inst.app.nexustvObjects.isEmpty()) {
					for (Iterator <CRunNEXUSTV> it =
							MMFRuntime.inst.app.nexustvObjects.iterator (); it.hasNext (); )
					{
						it.next().keyUp(keyCode, msg);
					}
				}
			}
		}

		if(MMFRuntime.FIRETV && MMFRuntime.inst.app.firetvObjects.size() != 0)
		{
			int jplayer = -1;	

			if((jplayer = CRunFIRETV.keyUpAtPlayerNumber(keyCode, msg)) != -1)
			{
				final byte[] rhPlayer = CJoystickFIRETV.rhPlayer;

				rhPlayer[jplayer] &= ~getPlayerBits(keyCode);

				if(!MMFRuntime.inst.app.firetvObjects.isEmpty()) {
					for (Iterator <CRunFIRETV> it = MMFRuntime.inst.app.firetvObjects.iterator (); it.hasNext (); )
					{
						it.next().keyUp(keyCode, msg);
					}
				}
			}

		}

		if(!handled && msg != null) {
			MMFRuntime.inst.app.keyUp(msg.getKeyCode());
		}

		return handled || super.onKeyUp(keyCode, msg);
	}

	public void onTouchPad(final MotionEvent event) {
		if (MMFRuntime.inst.app != null) {
			float x = event.getX();
			float y = event.getY();

			x -= MMFRuntime.inst.viewportX;
			y -= MMFRuntime.inst.viewportY;

			x /= MMFRuntime.inst.scaleX;
			y /= MMFRuntime.inst.scaleY;

			MMFRuntime.inst.app.setCursorPos((int)x,  (int)y);
		}

	}


	@SuppressLint("NewApi")
	@Override
	public boolean onGenericMotionEvent(MotionEvent event)
	{
		boolean handled = false;

		if(MMFRuntime.OUYA) {
			OuyaPlayer = CRunOUYA.onMotionEvent(event);


			//Touchpad enable to work with player 0 when OUYA object is used 
			// or player -1 when OuyaController have not been initialised.
			if(!MMFRuntime.inst.app.ouyaObjects.isEmpty() && MMFRuntime.deviceApi > 11 
					&& OuyaPlayer == MMFRuntime.inst.OuyaTouchPad 
					&& (event.getSource() & InputDevice.SOURCE_CLASS_POINTER) != 0) {
				onTouchPad(event);
			}

			// Player is -1 when Ouyacontroller have not been initialised.
			if(MMFRuntime.deviceApi > 11
					&& OuyaPlayer > -1 
					&& !MMFRuntime.inst.app.ouyaObjects.isEmpty()) {

				int joystick = 0;

				if ((event.getSource() & InputDevice.SOURCE_CLASS_JOYSTICK) != 0) {
					//Get all the axis for the event
					float axisX = event.getAxisValue(MotionEvent.AXIS_X);
					float axisY = event.getAxisValue(MotionEvent.AXIS_Y);
					if (axisX * axisX + axisY * axisY < (0.20f * 0.20f)) {
						//axisX = axisY = 0.0f;
						joystick =0;			  
					} else {
						/*
						if(axisX < -0.35)
							joystick |= 0x04;

						if(axisX > 0.35)
							joystick |= 0x08;

						if(axisY < -0.35)
							joystick |= 0x01;

						if(axisY > 0.35)
							joystick |= 0x02;
						 */
						double angle = (2*joyPI + Math.atan2(-axisY, axisX))%(2*joyPI);
						//Log.Log("X: "+axisX+" Y: "+axisY+" joystick: "+joystick+" Angle: "+angle*180.0/joyPI);
						joystick = joystickResult(angle);
					}
					byte[] rhPlayer = CJoystickOUYA.rhPlayer;

					rhPlayer[OuyaPlayer] = (byte)joystick;
				}

			}
		}
		if(MMFRuntime.NEXUSTV) {
			nexusPlayer = CRunNEXUSTV.onMotionEvent(event);

			// Player is -1 when NexusTVcontroller have not been initialised.
			if(MMFRuntime.deviceApi > 11
					&& nexusPlayer > -1 
					&& !MMFRuntime.inst.app.nexustvObjects.isEmpty()) {

				int joystick = 0;

				if ((event.getSource() & InputDevice.SOURCE_CLASS_JOYSTICK) != 0) {
					//Get all the axis for the event
					float axisX = event.getAxisValue(MotionEvent.AXIS_X);
					float axisY = event.getAxisValue(MotionEvent.AXIS_Y);

					if (axisX * axisX + axisY * axisY < (0.20f * 0.20f)) {
						//axisX = axisY = 0.0f;
						joystick =0;			  
					} else {
						double angle = (2*joyPI + Math.atan2(-axisY, axisX))%(2*joyPI);
						//Log.Log("X: "+axisX+" Y: "+axisY+" joystick: "+joystick+" Angle: "+angle*180.0/joyPI);
						joystick = joystickResult(angle);
					}
					byte[] rhPlayer = CJoystickNEXUSTV.rhPlayer;

					rhPlayer[nexusPlayer] = (byte)joystick;
				}

			}
		}

		if(MMFRuntime.FIRETV) {
			firePlayer = CRunFIRETV.onMotionEvent(event);

			// Player is -1 when FireTV have not been initialised.
			if(MMFRuntime.deviceApi > 11
					&& firePlayer > -1 
					&& !MMFRuntime.inst.app.firetvObjects.isEmpty()) {

				int joystick = 0;

				if ((event.getSource() & InputDevice.SOURCE_CLASS_JOYSTICK) != 0) {
					//Get all the axis for the event
					float axisX = event.getAxisValue(MotionEvent.AXIS_X);
					float axisY = event.getAxisValue(MotionEvent.AXIS_Y);  
					if (axisX * axisX + axisY * axisY < (0.20f * 0.20f)) {
						//axisX = axisY = 0.0f;
						joystick =0;			  
					} else {
						double angle = (2*joyPI + Math.atan2(-axisY, axisX))%(2*joyPI);
						//Log.Log("X: "+axisX+" Y: "+axisY+" joystick: "+joystick+" Angle: "+angle*180.0/joyPI);
						joystick = joystickResult(angle);
					}
					byte[] rhPlayer = CJoystickFIRETV.rhPlayer;

					rhPlayer[firePlayer] = (byte)joystick;

					//Log.Log(" player "+player+" joystick value "+joystick);
				}


			}				

		}

		return handled;
	}

	@Override
	public boolean onTrackballEvent(final MotionEvent event)
	{
		if (MMFRuntime.inst.app != null) {
			if((!MMFRuntime.OUYA && !MMFRuntime.FIRETV) 
					|| (MMFRuntime.deviceApi > 11 
							&& MMFRuntime.OUYA && OuyaPlayer == MMFRuntime.inst.OuyaTouchPad)
							|| (MMFRuntime.deviceApi > 11 
									&& MMFRuntime.FIRETV))
				MMFRuntime.inst.app.trackballMove((int) event.getX(), (int) event.getY());
			MMFRuntime.inst.touchManager.process(event);
		}
		return true;
	}


	@Override
	public void onWindowFocusChanged(boolean hasWindowFocus)
	{
		super.onWindowFocusChanged(hasWindowFocus);
		if(hasWindowFocus && SurfaceView.hasSurface)
			makeCurrentIfNecessary();

		if(hasWindowFocus)
		{
			MMFRuntime.inst.runOnUiThread(new Runnable() {

				@Override
				public void run() {
					MMFRuntime.inst.VisibilityUiChange(1500);
				}

			});
		}
		Log.Log("Surface Windows focus change ...");

	}

	@Override
	public boolean onTouchEvent(final MotionEvent event)
	{
		if (MMFRuntime.inst.app != null && MMFRuntime.inst.touchManager != null) {
			if((!MMFRuntime.OUYA && !MMFRuntime.FIRETV) 
					|| (MMFRuntime.OUYA && OuyaPlayer == MMFRuntime.inst.OuyaTouchPad)
							|| (MMFRuntime.FIRETV))
				synchronized (event) {
					MMFRuntime.inst.touchManager.process(event);
				}
		}
		return true;
	}

	///////////////////////////////////////////////////////////////////////////////
	//
	//           Joy-stick as Internal
	//
	///////////////////////////////////////////////////////////////////////////////
	// Change for smooth operation increasing reference for diagonals value
	//
	private double joyPI = Math.PI;
	private double joyanglezone = 60*Math.PI/180;

	private boolean InsideZone(double angle, double angle_ref, double gap) {
		// check if the angle is in the range, could be ported using degrees instead.
		return (angle > (angle_ref-gap/2) && angle < (angle_ref+gap/2));
	}

	private int joystickResult(double d) {

		int j = 0;

		if (d>=0.0)
		{ 
			while(true) {
				// Right
				if(InsideZone(d, 0, joyanglezone) || InsideZone(d, (joyPI)*2, joyanglezone)) {
					j=8;
					break;
				}
				// Up
				if(InsideZone(d, joyPI/2, joyanglezone)) {
					j=1;
					break;
				}
				// Left
				if(InsideZone(d, (joyPI), joyanglezone)) {
					j=4;
					break;
				}
				// Down
				if(InsideZone(d, (joyPI/4)*6, joyanglezone)) {
					j=2;
					break;
				}
				// Right/Up
				if(InsideZone(d, joyPI/4, joyanglezone)) {
					j=9;
					break;
				}
				// Left/Up
				if(InsideZone(d, (joyPI/4)*3, joyanglezone)) {
					j=5;
					break;
				}
				// Left/Down
				if(InsideZone(d, (joyPI/4)*5, joyanglezone)) {
					j=6;
					break;
				}
				// Right/Down
				if(InsideZone(d, (joyPI/4)*7, joyanglezone)) {
					j=10;
					break;
				}

				break;
			}
		}
		return j;
	}

	///////////////////////////////////////////////////////////////////////////////
	//
	//           GPU Utilities
	//
	///////////////////////////////////////////////////////////////////////////////

	private boolean detectOpenGLES20() 
	{
		ActivityManager am = (ActivityManager) MMFRuntime.inst.getSystemService(Context.ACTIVITY_SERVICE);
		ConfigurationInfo info = am.getDeviceConfigurationInfo();
		Log.Log("asked OpenGL2 detected version "+info.getGlEsVersion());
		return (info.reqGlEsVersion == 0x20000);
	}

	private boolean detectOpenGLES30() 
	{
		ActivityManager am = (ActivityManager) MMFRuntime.inst.getSystemService(Context.ACTIVITY_SERVICE);
		ConfigurationInfo info = am.getDeviceConfigurationInfo();
		Log.Log("asked OpenGL3 detected version "+info.getGlEsVersion());
		return (info.reqGlEsVersion >= 0x30000);
	}

	///////////////////////////////////////////////////////////////////////////////
	//
	//           Context selection
	//
	///////////////////////////////////////////////////////////////////////////////

	public interface EGLConfigChooser {
		EGLConfig chooseConfig(EGL10 egl, EGLDisplay display);
		EGLConfig secondaryConfig(EGL10 egl, EGLDisplay display);
	}

	private abstract class BaseConfigChooser implements EGLConfigChooser {

		protected int eglContextClientVersion;

		public BaseConfigChooser(int version, int[] configSpec) {
			eglContextClientVersion = version;
			mConfigSpec = filterConfigSpec(configSpec);
		}

		@Override
		public EGLConfig chooseConfig(EGL10 egl, EGLDisplay display) {

			int[] num_config = new int[1];
			if (!egl.eglChooseConfig(display, mConfigSpec, null, 0,
					num_config)) {
				throw new IllegalArgumentException("eglChooseConfig failed");
			}

			int numConfigs = num_config[0];

			if (numConfigs <= 0) {
				Log.Log("No config present");
				return null;	// not configuration available at start
			}

			EGLConfig[] configs = new EGLConfig[numConfigs];

			if (!egl.eglChooseConfig(display, mConfigSpec, configs, numConfigs,	num_config)) {
				throw new IllegalArgumentException("eglChooseConfig#2 failed");
			}

			EGLConfig config = chooseConfig(egl, display, configs);

			if (config == null) {
				Log.Log("No config chosen");
			}
			if (config != null) {
				printEGL(egl, eglDisplay ,config);
			}

			return config;
		}

		@Override
		public EGLConfig secondaryConfig(EGL10 egl, EGLDisplay display)
		{
			//Querying number of configurations
			int[] num_conf = new int[1];
			egl.eglGetConfigs(display, null, 0, num_conf);  //if configuration array is null it still returns the number of configurations
			int configurations = num_conf[0];

			//Querying actual configurations
			EGLConfig[] conf = new EGLConfig[configurations];
			egl.eglGetConfigs(display, conf, configurations, num_conf);

			EGLConfig result = chooseConfig(egl, display, conf);

			for(int i = 0; i < configurations; i++)
			{
				result = better(result, conf[i], egl, display);
			}

			Log.Log("EGLConfigChooser Chosen EGLConfig:");
			printEGL(egl, display, result);

			return result;
		}

		private EGLConfig better(EGLConfig a, EGLConfig b, EGL10 egl, EGLDisplay display)
		{
			if(a == null) return b;

			EGLConfig result = null;

			int[] value = new int[1];

			egl.eglGetConfigAttrib(display, a, EGL10.EGL_DEPTH_SIZE, value);
			int depthA = value[0];

			egl.eglGetConfigAttrib(display, b, EGL10.EGL_DEPTH_SIZE, value);
			int depthB = value[0];

			if(depthA > depthB)
				result = a;
			else if(depthA < depthB)
				result = b;
			else //if depthA == depthB
			{
				egl.eglGetConfigAttrib(display, a, EGL10.EGL_RED_SIZE, value);
				int redA = value[0];

				egl.eglGetConfigAttrib(display, b, EGL10.EGL_RED_SIZE, value);
				int redB = value[0];

				if(redA > redB)
					result = a;
				else if(redA < redB)
					result = b;
				else //if redA == redB
				{
					//Don't care
					result = a;
				}
			}

			return result;
		}

		abstract EGLConfig chooseConfig(EGL10 egl, EGLDisplay display, EGLConfig[] configs);

		protected int[] mConfigSpec;

		private int[] filterConfigSpec(int[] configSpec) {
			if (eglContextClientVersion != 2 && eglContextClientVersion != 3) {
				return configSpec;
			}
			/* We know none of the subclasses define EGL_RENDERABLE_TYPE.
			 * And we know the configSpec is well formed.
			 */
			int len = configSpec.length;
			int[] newConfigSpec = new int[len + 2];
			System.arraycopy(configSpec, 0, newConfigSpec, 0, len-1);
			newConfigSpec[len-1] = EGL10.EGL_RENDERABLE_TYPE;
			if (eglContextClientVersion == 2) {
				newConfigSpec[len] = 0x0004;  /* EGL_OPENGL_ES2_BIT */
			} else {
				newConfigSpec[len] = 0x0040; /* EGL_OPENGL_ES3_BIT_KHR */
			}
			newConfigSpec[len+1] = EGL10.EGL_NONE;
			return newConfigSpec;
		}


	}


	/**
	 * This class will choose a RGB_565 or RGB_8888 surface with
	 * or without a depth buffer.
	 *
	 */
	public class CTEGLConfigChooser extends ComponentSizeChooser {

		public CTEGLConfigChooser(int mode, int version, boolean withDepthBuffer) {
			super(version, (mode==MODE_565 ? 5: (mode==MODE_888 ? 8:4)), (mode==MODE_565 ? 6: (mode==MODE_888 ? 8:4)), (mode==MODE_565 ? 5: (mode==MODE_888 ? 8:4)),  (mode==MODE_565 ? 0: (mode==MODE_888 ? (ES==2?8:0):0)), withDepthBuffer ? 16 : 0, 8);
		}
	}

	/**
	 * Choose a configuration with exactly the specified r,g,b,a sizes,
	 * and at least the specified depth and stencil sizes.
	 */
	public class ComponentSizeChooser extends BaseConfigChooser {

		public ComponentSizeChooser(int version, int redSize, int greenSize, int blueSize,
				int alphaSize, int depthSize, int stencilSize) {
			super(version, new int[] {
					EGL10.EGL_SURFACE_TYPE, EGL10.EGL_WINDOW_BIT,
					//EGL10.EGL_SURFACE_TYPE, EGL10.EGL_PBUFFER_BIT,
					EGL10.EGL_RED_SIZE, redSize,
					EGL10.EGL_GREEN_SIZE, greenSize,
					EGL10.EGL_BLUE_SIZE, blueSize,
					EGL10.EGL_ALPHA_SIZE, alphaSize,
					EGL10.EGL_DEPTH_SIZE, depthSize,
					EGL10.EGL_STENCIL_SIZE, stencilSize,
					EGL10.EGL_NONE});
			mValue = new int[1];
			mRedSize = redSize;
			mGreenSize = greenSize;
			mBlueSize = blueSize;
			mAlphaSize = alphaSize;
			mDepthSize = depthSize;
			mStencilSize = stencilSize;
		}
		
		@Override
		public EGLConfig chooseConfig(EGL10 egl, EGLDisplay display, EGLConfig[] configs) {
			for (EGLConfig config : configs) {
				int d = findConfigAttrib(egl, display, config,
						EGL10.EGL_DEPTH_SIZE, 0);
				int s = findConfigAttrib(egl, display, config,
						EGL10.EGL_STENCIL_SIZE, 0);
				if ((d >= mDepthSize) && (s >= mStencilSize)) {
					int r = findConfigAttrib(egl, display, config,
							EGL10.EGL_RED_SIZE, 0);
					int g = findConfigAttrib(egl, display, config,
							EGL10.EGL_GREEN_SIZE, 0);
					int b = findConfigAttrib(egl, display, config,
							EGL10.EGL_BLUE_SIZE, 0);
					int a = findConfigAttrib(egl, display, config,
							EGL10.EGL_ALPHA_SIZE, 0);
					if ((r == mRedSize) && (g == mGreenSize)
							&& (b == mBlueSize) && (a == mAlphaSize)) {
						return config;
					}
				}
			}
			return null;
		}

		private int findConfigAttrib(EGL10 egl, EGLDisplay display,	EGLConfig config, int attribute, int defaultValue) {
			if (egl.eglGetConfigAttrib(display, config, attribute, mValue)) {
				return mValue[0];
			}
			return defaultValue;
		}


		private int[] mValue;
		//Subclasses can adjust these values:
		protected int mRedSize;
		protected int mGreenSize;
		protected int mBlueSize;
		protected int mAlphaSize;
		protected int mDepthSize;
		protected int mStencilSize;

	}

	private void printEGL(EGL10 egl, EGLDisplay display ,EGLConfig conf) {
		int[] value = new int[1];
		if (conf != null)
		{

			Log.Log(String.format(Locale.US, "conf = %s", conf.toString() ) );

			egl.eglGetConfigAttrib(display, conf, EGL10.EGL_RED_SIZE, value);
			Log.Log(String.format(Locale.US, "EGL_RED_SIZE  = %d", value[0] ) );

			egl.eglGetConfigAttrib(display, conf, EGL10.EGL_BLUE_SIZE, value);
			Log.Log(String.format(Locale.US, "EGL_BLUE_SIZE  = %d", value[0] ) );

			egl.eglGetConfigAttrib(display, conf, EGL10.EGL_GREEN_SIZE, value);
			Log.Log(String.format(Locale.US, "EGL_GREEN_SIZE  = %d", value[0] ) );

			egl.eglGetConfigAttrib(display, conf, EGL10.EGL_ALPHA_SIZE, value);
			Log.Log(String.format(Locale.US, "EGL_ALPHA_SIZE  = %d", value[0] ) );

			egl.eglGetConfigAttrib(display, conf, EGL10.EGL_DEPTH_SIZE, value);
			Log.Log(String.format(Locale.US, "EGL_DEPTH_SIZE  = %d", value[0] ) );

			//egl.eglGetConfigAttrib(display, conf, EGL10.EGL_ALPHA_FORMAT, value);
			//Log.Log(String.format(Locale.US, "EGL_ALPHA_FORMAT  = %d", value[0] ) );

			egl.eglGetConfigAttrib(display, conf, EGL10.EGL_ALPHA_MASK_SIZE, value);
			Log.Log(String.format(Locale.US, "EGL_ALPHA_MASK_SIZE  = %d", value[0] ) );

		}

	}

	private void throwEglException(String function) {
		throwEglException(function, egl.eglGetError());
	}

	private void throwEglException(String function, int error) {
		String message = function + " failed: " + getErrorString(error);
		if (LOG_THREADS) {
			Log.Log("throwEglException tid=" + Thread.currentThread().getId() + " " + message);
		}
		throw new RuntimeException(message);
	}

	public static String getErrorString(int error) {
		switch (error) {
		case EGL10.EGL_SUCCESS:
			return "EGL_SUCCESS";
		case EGL10.EGL_NOT_INITIALIZED:
			return "EGL_NOT_INITIALIZED";
		case EGL10.EGL_BAD_ACCESS:
			return "EGL_BAD_ACCESS";
		case EGL10.EGL_BAD_ALLOC:
			return "EGL_BAD_ALLOC";
		case EGL10.EGL_BAD_ATTRIBUTE:
			return "EGL_BAD_ATTRIBUTE";
		case EGL10.EGL_BAD_CONFIG:
			return "EGL_BAD_CONFIG";
		case EGL10.EGL_BAD_CONTEXT:
			return "EGL_BAD_CONTEXT";
		case EGL10.EGL_BAD_CURRENT_SURFACE:
			return "EGL_BAD_CURRENT_SURFACE";
		case EGL10.EGL_BAD_DISPLAY:
			return "EGL_BAD_DISPLAY";
		case EGL10.EGL_BAD_MATCH:
			return "EGL_BAD_MATCH";
		case EGL10.EGL_BAD_NATIVE_PIXMAP:
			return "EGL_BAD_NATIVE_PIXMAP";
		case EGL10.EGL_BAD_NATIVE_WINDOW:
			return "EGL_BAD_NATIVE_WINDOW";
		case EGL10.EGL_BAD_PARAMETER:
			return "EGL_BAD_PARAMETER";
		case EGL10.EGL_BAD_SURFACE:
			return "EGL_BAD_SURFACE";
		case EGL11.EGL_CONTEXT_LOST:
			return "EGL_CONTEXT_LOST";
		default:
			return String.valueOf(error);
		}
	}
}
