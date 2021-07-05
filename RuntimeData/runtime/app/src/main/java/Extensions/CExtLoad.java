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
// CEXTLOADER: Chargement des extensions
//
//----------------------------------------------------------------------------------
package Extensions;

import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CFile;

public class CExtLoad
{
	short handle;
	String name;
	String subType;

	public CExtLoad()
	{
	}

	public void loadInfo(CFile file)
	{
		long debut = file.getFilePointer();

		short size = (short) Math.abs (file.readAShort());
		handle = file.readAShort();
		file.skipBytes(12);
		name = file.readAString();
		int pos = name.lastIndexOf('.');
		name = name.substring(0, pos);
		int index = name.indexOf('-');
		while (index > 0)
		{
			name = name.substring(0, index) + '_' + name.substring(index + 1);
			index = name.indexOf('-');
		}
		index = name.indexOf(' ');
		while (index > 0)
		{
			name = name.substring(0, index) + '_' + name.substring(index + 1);
			index = name.indexOf(' ');
		}
		subType = file.readAString();

		file.seek(debut + size);
	}

	public CRunExtension loadRunObject()
	{
		CRunExtension object=null;

		if (MMFRuntime.nativeExtensions.contains ("CRun" + name))
		{
			object = new CRunNativeExtension ("CRun" + name);
		}
		else if (name.compareToIgnoreCase("Android")==0)
		{
			object=new CRunAndroid();
		}
		else if (name.compareToIgnoreCase("OUYA")==0)
		{
			object=new CRunOUYA();
		}
		else
		{
			// STARTCUT
			// ENDCUT
		}

		if (object!=null)
		{
			Log.Log("Created extension: " + name);
			return object;
		}

		Log.Log("*** MISSING EXTENSION: " + name);
		return null;
	}
}
