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
package Extensions;

import java.nio.ByteBuffer;

import Actions.CActExtension;
import Conditions.CCndExtension;
import Expressions.CNativeExpInstance;
import RunLoop.CCreateObjectInfo;
import Runtime.CrashReporter;
import Runtime.Native;
import Services.CBinaryFile;
import Services.CFontInfo;
import Services.CRect;
import Sprites.CMask;

public class CRunNativeExtension extends CRunExtension
{
	private String extName;
	private long handle;

	
	CRunNativeExtension (String extName)
	{
		this.extName = extName;

        CrashReporter.addInfo ("Native Extension", extName);
	}
	
    @Override
	public int getNumberOfConditions()
    {
    	return Native.getNumberOfConditions(extName);
    }
    
    @Override
	public boolean createRunObject(CBinaryFile file, CCreateObjectInfo cob, int version)
    {
    	ByteBuffer edPtr = null;
    	
    	if (file != null)
    	{
    		edPtr = ByteBuffer.allocateDirect(file.data.length);
    		
    		edPtr.put(file.data);
    		edPtr.position (0);
    	}
    	
    	handle = Native.createRunObject(rh, ho, extName, edPtr, version);
    		
    	return false;
    }
    
    @Override
	public int handleRunObject()
    {

        return Native.handleRunObject(handle);
    }
    
    @Override
	public void displayRunObject()
    {
    }
    
    @Override
	public void destroyRunObject(boolean bFast)
    {

        Native.destroyRunObject(handle);
    }
    
    @Override
	public void pauseRunObject()
    {
    }
    
    @Override
	public void continueRunObject()
    {
    }
    
    @Override
	public void getZoneInfos()
    {
    }
    
    @Override
	public boolean condition(int num, CCndExtension cnd)
    {

        return Native.condition(handle, num, cnd);
    }
    
    @Override
	public void action(int num, CActExtension act)
    {

        Native.action(handle, num, act);
    }
    
    public void nativeExpression (int num, CNativeExpInstance exp)
    {
    	Native.expression(handle, num, exp);
    }
    
    @Override
	public CMask getRunObjectCollisionMask(int flags)
    {
    	return null;
    }
    
    @Override
	public CFontInfo getRunObjectFont()
    {
    	return null;
    }
    
    @Override
	public void setRunObjectFont(CFontInfo fi, CRect rc)
    {
    }
    
    @Override
	public int getRunObjectTextColor()
    {
    	return 0;
    }
    
    @Override
	public void setRunObjectTextColor(int rgb)  
    {
    }
}
