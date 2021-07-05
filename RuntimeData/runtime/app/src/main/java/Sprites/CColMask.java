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
// CCOLMASK : masque de collision
//
//----------------------------------------------------------------------------------
package Sprites;


public class CColMask
{
    public static final short CM_TEST_OBSTACLE = 0;
    public static final short CM_TEST_PLATFORM = 1;
    public static final int CM_OBSTACLE = 0x0001;
    public static final int CM_PLATFORM = 0x0002;
    // Collision mask margins
    public static final int COLMASK_XMARGIN = 64;
    public static final int COLMASK_YMARGIN = 16;
    public static int HEIGHT_PLATFORM = 6;

    public static CColMask create (int xx1, int yy1, int xx2, int yy2, int flags)
    {
        CColMask mask = new CColMask ();
        mask.allocNative (xx1, yy1, xx2, yy2, flags);
        return mask;
    }

    public void destroy ()
    {
        if (ptr != 0)
        {
            freeNative ();
            ptr = 0;
        }
    }

    public long ptr;

    private native void allocNative (int xx1, int yy1, int xx2, int yy2, int flags);
    private native void freeNative ();

    public native void setOrigin(int dx, int dy);
    public native void fill(short value);
    public native void fillRectangle(int x1, int y1, int x2, int y2, int val);
    public native void orMask(CMask mask, int xx, int yy, int plans, int val);
    public native void orPlatformMask(CMask sMask, int xx, int yy);
    public native boolean testPoint(int x, int y, int plans);
    public native boolean testRect(int x, int y, int w, int h, int plans);
    public native boolean testMask(CMask mask, int yBase, int xx, int yy, int plans);
}
