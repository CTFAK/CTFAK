
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
// CMASK : un masque
//
//----------------------------------------------------------------------------------
package Sprites;

public class CMask {

    public static final int SCMF_FULL = 0x0000;
    public static final int SCMF_PLATFORM = 0x0001;
    public static final int GCMF_OBSTACLE = 0x0000;
    public static final int GCMF_PLATFORM = 0x0001;
    public static final int GCMF_FORCEMASK = 0x1000;	// force opaque mask creation

    public CMask()
    {   allocNative ();
    }

    @Override
    public void finalize()
    {	freeNative();
    }
    
    public long ptr;

    private native void allocNative ();
    private native void freeNative ();

    public native boolean testMask
            (int yBase1, int x1, int y1, CMask pMask2, int yBase2, int x2, int y2);

    public native boolean testRect(int yBase1, int xx, int yy, int w, int h);
    public native boolean testPoint(int x1, int y1);

    public native boolean createRotatedMask
            (CMask pMask, double fAngle, double fScaleX, double fScaleY);

    public native void setSpot (int x, int y);

    /* TODO : The overhead of using the JNI to retrieve these is probably horrible. */

    public native int getWidth ();
    public native int getHeight ();

    public native int getSpotX ();
    public native int getSpotY ();

    public native int getLineWidth ();
    public native int getRawValue (int index);
}
