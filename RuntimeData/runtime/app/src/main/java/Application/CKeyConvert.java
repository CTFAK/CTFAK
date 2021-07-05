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
// CKEYCONVERT : conversion des keycodes en keycodes java
//
//----------------------------------------------------------------------------------
package Application;

import android.view.KeyEvent;

/** Conversion of keycodes from PC to Java.
 * 
 * This class is just a list of keycodes, its methods
 * perform the conversion from PC to Java.
 */
public class CKeyConvert 
{    
	public static final int VK_LBUTTON=KeyEvent.getMaxKeyCode()+1;
	public static final int VK_MBUTTON=KeyEvent.getMaxKeyCode()+2;
	public static final int VK_RBUTTON=KeyEvent.getMaxKeyCode()+3;
	public static final int MAX_VK=KeyEvent.getMaxKeyCode()+5;
	
    public static final int keys[]=
    {
		0x01, VK_LBUTTON,		// LButton
		0x02, VK_RBUTTON,		// RButton
		0x04, VK_MBUTTON,		// MButton
        0x0D, KeyEvent.KEYCODE_ENTER,			// Return
        0x10, KeyEvent.KEYCODE_SHIFT_LEFT,		// Shift
        0x11, KeyEvent.KEYCODE_CTRL_LEFT,		// Control
        0x12, KeyEvent.KEYCODE_ALT_LEFT,		// Alt
        0x20, KeyEvent.KEYCODE_SPACE,			// Space
        0x25, KeyEvent.KEYCODE_DPAD_LEFT,	    // 37,		// Left
        0x26, KeyEvent.KEYCODE_DPAD_UP,	    	// 38,		// Up
        0x27, KeyEvent.KEYCODE_DPAD_RIGHT,    	// 39,		// Right
        0x28, KeyEvent.KEYCODE_DPAD_DOWN,	    // 40,		// Down
        0x08, KeyEvent.KEYCODE_BACK,			// Backspace
        0x24, KeyEvent.KEYCODE_HOME,			// HOME
        0x2E, KeyEvent.KEYCODE_DEL,				// Delete
        0x30, KeyEvent.KEYCODE_0,
        0x31, KeyEvent.KEYCODE_1,
        0x32, KeyEvent.KEYCODE_2,
        0x33, KeyEvent.KEYCODE_3,
        0x34, KeyEvent.KEYCODE_4,
        0x35, KeyEvent.KEYCODE_5,
        0x36, KeyEvent.KEYCODE_6,
        0x37, KeyEvent.KEYCODE_7,
        0x38, KeyEvent.KEYCODE_8,
        0x39, KeyEvent.KEYCODE_9,
        0x41, KeyEvent.KEYCODE_A,		// A
        0x42, KeyEvent.KEYCODE_B,		// b
        0x43, KeyEvent.KEYCODE_C,		// c
        0x44, KeyEvent.KEYCODE_D,		// d
        0x45, KeyEvent.KEYCODE_E,		// e
        0x46, KeyEvent.KEYCODE_F,		// f
        0x47, KeyEvent.KEYCODE_G,		// g
        0x48, KeyEvent.KEYCODE_H,		// h
        0x49, KeyEvent.KEYCODE_I,		// i
        0x4A, KeyEvent.KEYCODE_J,		// j
        0x4B, KeyEvent.KEYCODE_K,		// k	
        0x4C, KeyEvent.KEYCODE_L,		// l
        0x4D, KeyEvent.KEYCODE_M,		// m
        0x4E, KeyEvent.KEYCODE_N,		// n
        0x4F, KeyEvent.KEYCODE_O,		// o
        0x50, KeyEvent.KEYCODE_P,		// p
        0x51, KeyEvent.KEYCODE_Q,		// q
        0x52, KeyEvent.KEYCODE_R,		// r
        0x53, KeyEvent.KEYCODE_S,		// s
        0x54, KeyEvent.KEYCODE_T,		// t
        0x55, KeyEvent.KEYCODE_U,		// u
        0x56, KeyEvent.KEYCODE_V,		// v
        0x57, KeyEvent.KEYCODE_W,		// w
        0x58, KeyEvent.KEYCODE_X,		// x
        0x59, KeyEvent.KEYCODE_Y,		// y
        0x5A, KeyEvent.KEYCODE_Z,		// Z
        0x60, KeyEvent.KEYCODE_0,		// Numpad0
        0x61, KeyEvent.KEYCODE_1, 
        0x62, KeyEvent.KEYCODE_2, 
        0x63, KeyEvent.KEYCODE_3, 
        0x64, KeyEvent.KEYCODE_4, 
        0x65, KeyEvent.KEYCODE_5, 
        0x66, KeyEvent.KEYCODE_6, 
        0x67, KeyEvent.KEYCODE_7, 
        0x68, KeyEvent.KEYCODE_8, 
        0x69, KeyEvent.KEYCODE_9,         
        -1
    };


    
/*    public static int gameActionToKey[]=
    {
    	KeyEvent.KEYCODE_SPACE, KeyEvent.KEYCODE_DPAD_CENTER,
    };*/
            
    /** Returns a Java key from a PC key.
     */
  /*  public static int getActionKey(int key)
    {
    	int n;
    	for (n=0; n<gameActionToKey.length; n+=2)
    	{
    		if (key==gameActionToKey[n])
    		{
    			return gameActionToKey[n+1];
    		}
    	}
		return 0;

    }*/
    
    public static int getJavaKey(int pcKey)
    {
        int n;
        for (n=0; keys[n]!=-1; n+=2)
        {
            if (keys[n]==pcKey)
            {
                return keys[n+1];
            }
        }
        return 0;
    }

}
