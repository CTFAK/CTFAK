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

package Services;


import android.annotation.SuppressLint;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStreamWriter;
import java.io.Reader;
import java.io.Writer;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.NoSuchElementException;

import Application.CEmbeddedFile;
import Objects.CExtension;
import Runtime.Log;
import Runtime.MMFRuntime;


public class INIFile
{
	private GroupInsensitiveMap IniGroups;
	private CExtension ho;


	private String currentFile;
	private int flags;
	private boolean modified;

	private boolean saving;
	private String iniType;
	private String iniExt;
	
	//private ContentResolver contentResolver;


	public INIFile(CExtension oho, String FullName, int oflag)
	{
		//contentResolver = MMFRuntime.inst.getContentResolver();
		IniGroups = new GroupInsensitiveMap();
		currentFile = FullName;

		flags = oflag;
		ho = oho;
		modified = false;

		loadFile();
	}

	public String getFileName()
	{
		return currentFile;
	}

	public void setFileName(String filename)
	{
		if(IniGroups.size() > 0) 
			save();

		clear();
		IniGroups = null;

		currentFile = filename;
		IniGroups = new GroupInsensitiveMap();

		loadFile();

	}

	public String getStringItem(String Group, String Item)
	{
		INIitems iniItem  = null;
		INIgroups   inigroup   = null;
		String        szRet   = "";

		inigroup = IniGroups.get(Group);
		if (inigroup != null) {
			
			iniItem = inigroup.getItem(Item);

			if (iniItem != null) {
				szRet = iniItem.getItemValue();
				iniItem = null;
			}
			inigroup = null;
		}
		return szRet;
	}


	public Integer getIntegerItem(String Group, String Item)
	{
		INIitems iniItem = null;
		INIgroups inigroup = null;
		String strVal = null;
		int intRet = 0;

		inigroup = IniGroups.get(Group);
		if (inigroup != null) {

			iniItem = inigroup.getItem(Item);

			try
			{
				if (iniItem != null)  {
					strVal = iniItem.getItemValue();

					if (strVal != null) 
						intRet = CServices.parseInt(strVal);
				}
			}
			catch (NumberFormatException NFExIgnore) {
				Log.Log("Number incorrectly formated ...");
			}
			finally
			{
				if(iniItem != null)
					iniItem = null;
			}			
			inigroup = null;
		}
		return intRet;
	}

	public void addSection(String Group)
	{
		INIgroups inigroup   = null;

		inigroup = IniGroups.get(Group);
		if (inigroup == null)
		{
			inigroup = new INIgroups(Group);
			IniGroups.put(Group, inigroup);
		}
		inigroup = null;
		modified = true;
	}

	public void setStringItem(String Group, String Item, String Value)
	{
		INIgroups inigroup   = null;

		inigroup = IniGroups.get(Group);
		if (inigroup == null)
		{
			inigroup = new INIgroups(Group);
			IniGroups.put(Group, inigroup);
		}
		inigroup.setItem(Item, Value);
		modified = true;
	}

	public void setIntegerItem(String Group, String Item, int Value)
	{
		INIgroups inigroup   = null;

		inigroup = IniGroups.get(Group);
		if (inigroup == null)
		{
			inigroup = new INIgroups(Group);
			IniGroups.put(Group, inigroup);
		}
		inigroup.setItem(Item, Integer.toString(Value));
		modified = true;
	}



	public String[] getAllSectionNames()
	{
		Iterator<String>   iterator   = null;
		String[]   strArr = null;
		int        i = 0;

		try
		{
			if (IniGroups.size() > 0)
			{
				strArr = new String[IniGroups.size()];
				for (iterator = IniGroups.keySet().iterator();;iterator.hasNext())
				{
					strArr[i] = iterator.next();
					i++;
				}
			}
		}
		catch (NoSuchElementException NSEExIgnore)
		{
		}
		finally
		{
			if (iterator != null)
				iterator = null;
		}
		return strArr;
	}

	public String[] getItemNames(String Group)
	{
		String[]    strArr = null;
		INIgroups inigroup = null;

		inigroup = IniGroups.get(Group);
		if (inigroup != null)
		{
			strArr = inigroup.getItemNames();
			inigroup = null;
		}
		return strArr;
	}

	public Map<String, INIitems> getItems(String Group)
	{
		INIgroups inigroup = null;
		Map<String, INIitems> map = null;

		inigroup = IniGroups.get(Group);
		if (inigroup != null)
		{
			map = inigroup.getItems();
			inigroup = null;
		}
		return map;
	}

	public void removeItem(String Group, String Item)
	{
		INIgroups inigroup = null;

		inigroup = IniGroups.get(Group);
		if (inigroup != null)
		{
			inigroup.removeItem(Item);
			inigroup = null;
		}
		modified = true;
	}

	public void removeGroup(String Group)
	{
		INIgroups inigroup = null;

		if(IniGroups.containsKey(Group)) {
			inigroup = IniGroups.get(Group);
			if (inigroup != null)
			{
				inigroup.delItemsGroup();
				inigroup.setGroup(null);
				inigroup = null;
			}
			IniGroups.remove(Group);
		}
		modified = true;
	}

	public boolean save()
	{
		if ( !modified )
			return true;

		Iterator<String> iterator = null;

		BufferedWriter iniWriter = null;
		FileOutputStream fs = null;
		INIgroups  inigroup = null;
		boolean    bRet    = false;
		File       file   = null;
		String     Name   = null;
		String     Temp   = null;

		String filename = makeFile(currentFile);
		String tempname = filename + ".tmp";

		try
		{
			if (filename == null)
				return false;

			if (IniGroups != null && IniGroups.size() > 0)
			{

				synchronized(IniGroups) {

					saving = true;

					iterator = IniGroups.keySet().iterator();

					if(!iterator.hasNext()) {
						saving = false;
						return bRet;
					}
					boolean rnFlag = false;

					file = new File(tempname);
					if (file.exists())
						file.delete();
					fs = new FileOutputStream(file);
					OutputStreamWriter ow = new OutputStreamWriter(fs,(flags & 8) != 0 ? "UTF-8":"iso-8859-1");
					iniWriter = new BufferedWriter(ow, 8192);

					while (iterator.hasNext()) {
						Name = iterator.next();

						inigroup = IniGroups.get(Name);

						if(inigroup != null) {

							Temp = inigroup.toString();
							if(Temp != null && inigroup.getItems().size() > 0) {
								if(rnFlag)
									iniWriter.write("\r\n");

								iniWriter.write(Temp);
								//Log.Log(Temp);

								rnFlag = true;
							}
							else
								rnFlag = false;

							inigroup = null;
						}
					}
					bRet = true;

				}
			}
			else
			{
				file = new File(tempname);
				if (file.exists())
					file.delete();
				fs = new FileOutputStream(file);
				OutputStreamWriter ow = new OutputStreamWriter(fs,(flags & 8) != 0 ? "UTF-8":"iso-8859-1");
				iniWriter = new BufferedWriter(ow, 8192);
				iniWriter.write("\r\n");
				bRet = true;
			}
		}
		catch (IOException e) {
			Log.Log("Witer error: "+e.getMessage());
		}
		finally {

			if (iniWriter != null)
			{
				closeWriter(iniWriter);
				iniWriter = null;
			}

			if (iterator != null) 
				iterator = null;
		}

		if (file != null) 
		{
			try {
				if (file.exists()) {
					// if tmp exists replace it
					File fileINI = new File(filename);

					// if ini exists delete it
					if (fileINI.exists())
						fileINI.delete();

					// Rename tmp to ini
					file.renameTo(fileINI);

					// Saved => reset modified flag
					modified = false;
				}
			}
			catch (Exception e) {
				Log.Log("Replace INI tmp error: "+e.getMessage());
			}
			file = null;
		}

		saving = false;
		return bRet;
	}

	private void loadFile()
	{
		String filename = currentFile;
		INIgroups inigroup = null;

		if(ho != null && !saving)
		{
			try
			{
				IniFile file = openINIFile(filename);

				if(file != null)
				{
					UnicodeReader ur = new UnicodeReader(file.stream, (flags & 8) != 0 ? "UTF-8":"iso-8859-1");
					BufferedReader reader = new BufferedReader(ur, 8192);

					String group = "";

					if(IniGroups.size() > 0)
						IniGroups.clear();

					for(String s = reader.readLine(); s != null; s = reader.readLine())
					{
						try
						{
							if(s.length() == 0 || s.charAt(0) == ';')
								continue;

							if(s.trim().length() > 0 && s.trim().charAt(0) == '[' && s.trim().endsWith("]"))
							{
								inigroup = null;

								if(s.contentEquals("[]"))
									group ="";
								else
									group = s.split("\\[")[1].split("]")[0];
								// Group start reached create new Group
								if (inigroup == null) {
									inigroup = new INIgroups(group);   								
								}
								IniGroups.put(group, inigroup);
								continue;
							}

							int eq = s.indexOf("=");

							if(eq != -1)
							{
								String Item = s.substring(0, eq);
								String value = s.substring(eq + 1, s.length());
								// read the key value pair Item=value
								inigroup.setItem(Item, value);

							}
						}
						catch(Exception e)
						{
							Log.Log(e.toString());
							IniGroups.clear();
						}
					}
					if (reader != null)
					{
						closeReader(reader);
						reader = null;
					}

					if (file != null)
					{
						file.close();
						file = null;
					}
				}
			}
			catch(Exception e)
			{
				Log.Log(e.toString());
				IniGroups.clear();
			}
			finally
			{
				if (inigroup != null) 
					inigroup = null;
				modified = false;
			}
		}
	}

	private void closeReader(Reader reader)
	{
		if (reader == null) 
			return;

		try  {
			reader.close();
		}
		catch (IOException IOExIgnore) {
			Log.Log("Error closing INI when reading...");
		}
	}

	private void closeWriter(Writer writer)
	{
		if (writer == null) 
			return;

		try  {
			writer.close();
		}
		catch (IOException IOExIgnore)  {
			Log.Log("Error closing INI when writing...");
		}
	}

	private String makeFile(String cFile) {
		String filename = null;
		iniType = "external";
		if(cFile.length() > 0) {
			if(!cFile.contains("/") && !cFile.contains("\\"))
				filename = MMFRuntime.inst.getFilesDir ().toString ()+"/"+currentFile;
			else if((flags & 0x0004) != 0) {
				String fullpath = cFile;
				fullpath = fullpath.replace('\\', '/');

				int lastSlash = fullpath.lastIndexOf('/');

				if(lastSlash != -1)
					filename = fullpath.substring(lastSlash + 1, fullpath.length() - 1);

				filename = MMFRuntime.inst.getFilesDir ().toString ()+"/"+filename;
				iniType = "internal";
			}
			else
				filename = cFile;
		}
		
		//File file = new File(filename);
		//updateContentFile(file, iniType);
		//file = null;

		return filename;
	}

	private boolean checkFile(String filename)
	{
		boolean bRet  = false;
		File    file = null;

		try
		{
			file = new File(filename);
			bRet = (file.exists() && file.isFile());
		}
		catch (Exception e)
		{
			bRet = false;
		}
		finally
		{
			if (file != null) 
				file = null;
		}
		return bRet;
	}

	public void clear () {
		IniGroups.clear();
		// modified = true;		// restore this line if you use the clear function to reset the content of the object from an action
	}

	public class IniFile
	{
		public InputStream stream;
		public HttpURLConnection connection;

		public void close()
		{
			if(stream != null)
			{
				try
				{	stream.close();
				}
				catch (IOException e)
				{
				}

				stream = null;
			}

			if (connection != null)
			{
				connection.disconnect();
				connection = null;
			}
		}
		@Override
		public void finalize()
		{
			close();
		}
	}

	public IniFile openINIFile(String path)
	{
		IniFile file = new IniFile();

		if (path != null && path.length() > 0)
		{
			if(!checkFile(makeFile(path))) {
				CEmbeddedFile embed = null;
				embed = ho.hoAdRunHeader.rhApp.getEmbeddedFile(path);
				if (embed != null)
				{
					file.stream = embed.getInputStream();
					return file;
				}
			}
			else
			{
				try
				{
					file.stream = new FileInputStream(makeFile(path));
					return file;
				}
				catch (IOException e)
				{
					try
					{
						URL url = new URL(path);

						file.connection = (HttpURLConnection) url.openConnection();
						file.stream = file.connection.getInputStream();

						return file;
					}
					catch(Exception ue)
					{
						Log.Log(ue.toString());
					}
				}
			}
		}

		return null;
	}    

	@SuppressLint("DefaultLocale")
	private class GroupInsensitiveMap extends LinkedHashMap<String, INIgroups> {

		static final long serialVersionUID = 122398735L;
		
		@Override
		public INIgroups put(String key, INIgroups groups) {
			return super.put(key.toLowerCase(), groups);
		}

		public INIgroups get(String key) {
			return super.get(key.toLowerCase());
		}

		public INIgroups remove(String key) {
			return super.remove(key.toLowerCase());
		}

		public boolean containsKey(String key) {
			return super.containsKey(key.toLowerCase());		    
		}
	}

//	@SuppressLint("NewApi")
//	public boolean updateContentFile(File lfile, String type) {
//
//		if(lfile == null || android.os.Build.VERSION.SDK_INT < 11 || MMFRuntime.OUYA)
//			return false;
//		
//		//ContentResolver contentResolver = MMFRuntime.inst.getContentResolver();
//		Uri filesUri = MediaStore.Files.getContentUri(type);
//
//		ContentValues values;
//
//		// Create a media database entry for the file. This step will not actually cause the directory to be created.
//		values = new ContentValues();
//		values.put(MediaStore.Files.FileColumns.DATA, lfile.getAbsolutePath());
//		contentResolver.insert(filesUri, values);
//
//		return lfile.exists();
//	}
	
}
