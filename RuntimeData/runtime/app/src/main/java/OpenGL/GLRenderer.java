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

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Set;

import Application.CRunApp;
import Runtime.Log;
import Runtime.MMFRuntime;
import android.content.Intent;
import android.graphics.Bitmap;

public abstract class GLRenderer
{
    public static final int BOP_COPY = 0;
    public static final int BOP_BLEND = 1;
    public static final int BOP_INVERT = 2;
    public static final int BOP_XOR = 3;
    public static final int BOP_AND = 4;
    public static final int BOP_OR = 5;
    public static final int BOP_BLEND_REPLEACETRANSP = 6;
    public static final int BOP_DWROP = 7;
    public static final int BOP_ANDNOT = 8;
    public static final int BOP_ADD = 9;
    public static final int BOP_MONO = 10;
    public static final int BOP_SUB = 11;
    public static final int BOP_BLEND_DONTREPLACECOLOR = 12;
    public static final int BOP_EFFECTEX = 13;
    public static final int BOP_MAX = 14;

    public static final int BOP_MASK = 0xFFF;
    public static final int BOP_RGBAFILTER = 0x1000;
    public static final int EFFECTFLAG_TRANSPARENT = 0x10000000;
    public static final int EFFECTFLAG_ANTIALIAS = 0x20000000;
    public static final int EFFECT_MASK = 0xFFFF;

    public static GLRenderer inst;
    
    public static int limitX;
    public static int limitY;

    private long ptr;

    protected Set<ITexture> textures;

    public Log debugWriter;

    public GLRenderer ()
    {
    	textures = new HashSet<ITexture>();
        debugWriter = new Log();
    }

    public String gpu, gpuVendor, glVersion;

    public abstract void beginWholeScreenDraw ();
    public abstract void endWholeScreenDraw ();
    
    public abstract void updateViewport (boolean stretchWindowToViewport);

    public abstract void clear(int color);

    protected abstract void setBase (int x, int y);

    protected abstract int getBaseX ();
    protected abstract int getBaseY ();

    public abstract void setLimitX (int limit);
    public abstract void setLimitY (int limit);
    
    public abstract void fillZone
        (int x, int y, int w, int h, int color, int inkEffect, int inkEffectParam);

    public abstract void fillZone (int x, int y, int w, int h, int color);

    public void drawJoystick()
    {
        if(MMFRuntime.inst.app.joystick != null
                && MMFRuntime.inst.app.appRunningState == CRunApp.SL_FRAMELOOP)
        {
            MMFRuntime.inst.app.joystick.draw();
        }
    }

    protected abstract void scissor (boolean enabled);
    protected abstract void scissor (int x, int y, int w, int h);

    ArrayList<int []> clips = new ArrayList<int []>();

    public void pushClip(int x, int y, int w, int h)
    {
        //x = (int) Math.ceil(x*MMFRuntime.inst.scaleX);
        //y = (int) Math.ceil(y*MMFRuntime.inst.scaleY);
        x *= MMFRuntime.inst.scaleX;
        y *= MMFRuntime.inst.scaleY;

        //w = (int) Math.ceil(w*MMFRuntime.inst.scaleX);
        //h = (int) Math.ceil(h*MMFRuntime.inst.scaleY);
        w *= MMFRuntime.inst.scaleX;
        h *= MMFRuntime.inst.scaleY;

        x += MMFRuntime.inst.viewportX + getBaseX ();
        y += MMFRuntime.inst.viewportY + getBaseY ();

        if(clips.isEmpty())
            scissor (true);

        int [] clip = { x, MMFRuntime.inst.currentHeight - y - h, w, h };
        clips.add(clip);

        scissor (clip[0], clip[1], clip[2], clip[3]);
    }

    public void popClip()
    {
        clips.remove(clips.size() - 1);

        if(clips.isEmpty())
        {
            scissor (false);
        }
        else
        {
            int [] clip = clips.get(clips.size() - 1);
            scissor (clip[0], clip[1], clip[2], clip[3]);
        }
    }

    ArrayList<int []> bases = new ArrayList<int []>();

    public void pushClipAndBase(int x, int y, int w, int h)
    {
        pushClip(x, y, w, h);

        x += getBaseX ();
        y += getBaseY ();

        int [] base = { x, y };
        bases.add(base);

        setBase (x, y);
    }

    public void popClipAndBase()
    {
        popClip();

        bases.remove(bases.size() - 1);

        if(bases.isEmpty())
        {
            setBase (0, 0);
        }
        else
        {
            int [] base = bases.get(bases.size() - 1);
            setBase (base [0], base [1]);
        }
    }

    public void pushBase(int x, int y, int w, int h)
    {
        x += getBaseX ();
        y += getBaseY ();

        int [] base = { x, y };
        bases.add(base);

        setBase (x, y);
    }

    public void popBase()
    {
        bases.remove(bases.size() - 1);

        if(bases.isEmpty())
        {
            setBase (0, 0);
        }
        else
        {
            int [] base = bases.get(bases.size() - 1);
            setBase (base [0], base [1]);
        }
    }

    public abstract void renderPoint
        (ITexture image, int x, int y, int inkEffect, int inkEffectParam);

    public abstract void renderGradient
        (int x, int y, int w, int h, int color1, int color2, boolean vertical, int inkEffect, int inkEffectParam);

    public abstract void setInkEffect
        (int effect, int effectParam);

    public abstract void renderImage
        (ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam);

    public abstract void renderScaledRotatedImage
        (ITexture image, float angle, float sX, float sY, int hX, int hY,
            int x, int y, int w, int h, int inkEffect, int inkEffectParam);

    public abstract void renderScaledRotatedImage2
    	(ITexture image, float angle, float sX, float sY, 
    		int bHotSpot, int x, int y, int inkEffect, int inkEffectParam);
    
    public abstract void renderScaledRotatedImageWrapAndFlip
	(ITexture image, float angle, float sX, float sY, int hX, int hY,
			int x, int y, int w, int h, int inkEffect, int inkEffectParam, int offsetX, int offsetY, int wrap, int flipH, int flipV, int resample, int transp);
    
     public abstract void renderPattern
        (ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam);

    public abstract void renderLine
        (int xA, int yA, int xB, int yB, int color, int thickness);

    public abstract void renderRect
        (int x, int y, int w, int h, int color, int thickness);

    public abstract void renderGradientEllipse
            (int x, int y, int w, int h, int color1, int color2,
              boolean vertical, int inkEffect, int inkEffectParam);

    public abstract void renderPatternEllipse
        (ITexture image, int x, int y, int w, int h, int inkEffect, int inkEffectParam);

    public abstract void readScreenToTexture(ITexture image, int x, int y, int w, int h);   
    
    public abstract void renderPerspective(ITexture image, int x, int y, int w, int h, float fA, float fB, int pDir, int inkEffect, int inkEffectParam);   

    public abstract void renderSinewave(ITexture image, int x, int y, int w, int h, float Zoom, float WaveIncrement, float Offset, int pDir, int inkEffect, int inkEffectParam);   

    public abstract int addShaderFromString(String shaderName, String vertexShader, String fragmentShader, String[] shaderVariables, boolean useTexCoord, boolean useColors);

	public abstract int addShaderFromFile(String shaderName, String[] shaderVariables, boolean useTexCoord, boolean useColors);

	public abstract void removeShader(int shaderIndex);

	public abstract void setEffectShader(int shaderIndex);

	public abstract void removeEffectShader();

	public abstract void updateVariable1i(String varName, int value);

	/**
	 * update variable value for a specific index variable inside a shader
	 * range from 0  to 5 (max.) only 6 variables allowed per shader
	 * 
	 * @param varIndex range 0 - 5 (0 represent UNIFORM_VAR1)
	 * @param value in this case an integer value
	 */
	public abstract void updateVariable1ibyIndex(int varIndex, int value);

	public abstract void updateVariable1f(String varName, float value);

	/**
	 * update variable value for a specific index variable inside a shader
	 * range from 0  to 5 (max.) only 6 variables allowed per shader
	 * 
	 * @param varIndex range 0 - 5 (0 represent UNIFORM_VAR1)
	 * @param value in this case a float value
	 */
	public abstract void updateVariable1fbyIndex(int varIndex, float value);

	public abstract void updateVariable2i(String varName, int value0, int value1);

	/**
	 * update variable value for a specific index variable inside a shader
	 * range from 0  to 5 (max.) only 6 variables allowed per shader
	 * 
	 * @param varIndex range 0 - 5 (0 represent UNIFORM_VAR1)
	 * @param value0 in this case an integer value
	 * @param value1 in this case an integer value
	 */
	public abstract void updateVariable2ibyIndex(int varIndex, int value0, int value1);

	public abstract void updateVariable2f(String varName, float value0, float value1);

	public abstract void updateVariable2fbyIndex(int varIndex, float value0, float value1);

	public abstract void updateVariable3i(String varName, int value0, int value1, int value2);

	/**
	 * update variable value for a specific index variable inside a shader
	 * range from 0  to 5 (max.) only 6 variables allowed per shader
	 * 
	 * @param varIndex range 0 - 5 (0 represent UNIFORM_VAR1)
	 * @param value0 in this case an integer value
	 * @param value1 in this case an integer value
	 * @param value2 in this case an integer value
	 */
	public abstract void updateVariable3ibyIndex(int varIndex, int value0, int value1, int value2);

	public abstract void updateVariable3f(String varName, float value0, float value1, float value2);

	public abstract void updateVariable3fbyIndex(int varIndex, float value0, float value1, float value2);

	public abstract void updateVariable4i(String varName, int value0, int value1, int value2, int value3);

	/**
	 * update variable value for a specific index variable inside a shader
	 * range from 0  to 5 (max.) only 6 variables allowed per shader
	 * 
	 * @param varIndex range 0 - 5 (0 represent UNIFORM_VAR1)
	 * @param value0 in this case an integer value
	 * @param value1 in this case an integer value
	 * @param value2 in this case an integer value
	 * @param value3 in this case an integer value
	 */
	public abstract void updateVariable4ibyIndex(int varIndex, int value0, int value1, int value2, int value3);

	public abstract void updateVariable4f(String varName, float value0, float value1, float value2, float value3);

	public abstract void updateVariable4fbyIndex(int varIndex, float value0, float value1, float value2, float value3);

	public abstract void updateVariableMat4f(String varName, float[] value0);
	
	public abstract void updateVariableMat4fbyIndex(int varIndex, float[] value0);
	
	
	
	public abstract Bitmap drawBitmap(int x, int y, int width, int height, Bitmap.Config config);

 	public abstract boolean readFramePixels(int[] pixels, int x, int y, int width, int height, Bitmap.Config config);

    public abstract void renderFlush();

    public abstract void renderBlitFull(ITexture image);

    public abstract void renderBlit(ITexture image, int xDst, int yDst, int xSrc, int ySrc, int width, int height);

    public abstract void readFrameToTexture(ITexture image, int x, int y, int width, int height);
}
