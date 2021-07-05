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

import java.nio.ByteBuffer;

import Actions.CActExtension;
import Conditions.CCndExtension;
import Expressions.CNativeExpInstance;
import Objects.CExtension;
import RunLoop.CRun;

public class Native
{
	static
	{	System.loadLibrary ("RuntimeNative");
        System.loadLibrary ("gdx");
	}
	
	public static native void init (String thisApp, String libsDir);
	
	public static native void load (String name, String filename);
	public static native int getNumberOfConditions (String name);
	
	public static native long createRunObject (CRun rhPtr, CExtension ho, String type, ByteBuffer edPtr, int version);
	public static native void destroyRunObject (long handle);
	public static native int handleRunObject (long handle);

	public static native boolean condition (long handle, int num, CCndExtension cnd);
	public static native void action (long handle, int num, CActExtension act);
	public static native void expression(long handle, int num, CNativeExpInstance exp);

	public static native String getABI ();
}
