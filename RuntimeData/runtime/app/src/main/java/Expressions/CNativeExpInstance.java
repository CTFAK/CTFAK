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
package Expressions;

import Objects.CExtension;

public class CNativeExpInstance
{
	public CValue result = new CValue(0);
	CExtension ho;
	
	public CNativeExpInstance (CExtension ho)
	{	this.ho = ho;
	}
	
	public int getParamInt ()
	{	return ho.getExpParam().getInt ();
	}
	
	public String getParamString ()
	{	return ho.getExpParam().getString ();
	}
	
	public float getParamFloat ()
	{	return (float) ho.getExpParam().getDouble ();
	}
	
	public void setReturnInt (int value)
	{	result.forceInt(value);
	}
	
	public void setReturnString (String value)
	{	result.forceString(value);
	}
	
	public void setReturnFloat (float value)
	{	result.forceDouble(value);
	}
}
