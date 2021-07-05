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

import java.util.Collections;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.Set;

import Runtime.Log;

public class INIgroups {

	private String GroupName;

	private ItemInsensitiveMap mItems;

	public INIgroups(String Group)
	{
		mItems = new ItemInsensitiveMap();
		GroupName =  Group;
	}

	public String getGroup()
	{
		return GroupName;
	}

	public void setGroup(String Group)
	{
		GroupName = Group;
	}

	public void removeItem(String Item)
	{
		if (mItems != null && mItems.containsKey(Item))
			mItems.remove(Item);
	}

	public void delItemsGroup()
	{
		if(mItems != null)
			mItems.clear();
	}

	public void setItem(String Item, String Value)
	{
		if(mItems != null)
			mItems.put(Item, new INIitems(Item, Value));
	}

	
	public Map<String, INIitems> getItems()
	{
		return Collections.unmodifiableMap(mItems);
	}
	
	public String[] getItemNames()
	{
		Iterator<String> iterator   = null;
		String[] strArray  = null;
		int      iCntr = 0;

		try
		{
			if (mItems.size() > 0)
			{
				strArray = new String[mItems.size()]; 
				for (iterator = mItems.keySet().iterator();iterator.hasNext();)
				{
					strArray[iCntr] = iterator.next();
					iCntr++;
				}
			}
		}
		catch (NoSuchElementException e)
		{
			Log.Log("No such element error ...");
			return null;
		}
		return strArray;
	}
	
	
	public INIitems getItem(String Item)
	{
		INIitems iniItem = null;

		if (mItems.containsKey(Item))
			iniItem = mItems.get(Item);
		return iniItem;
	}

	@Override
	public String toString()
	{
		Iterator<String> iterator = null;
		Set<String> colKeys  = null;
		INIitems     iniItem = null;
		StringBuffer strBuf  = new StringBuffer();
		String       strRet  = "";

		if(GroupName == null)
			return null;
		
		strBuf.append("[" + GroupName + "]\r\n");
		colKeys = mItems.keySet();
		if (colKeys != null)
		{
			iterator = colKeys.iterator();
			if (iterator != null)
			{
				while (iterator.hasNext())
				{
					iniItem = mItems.get(iterator.next());
					strBuf.append(iniItem.toString());
					strBuf.append("\r\n");
					iniItem = null;
				}
				iterator = null;
			}
			colKeys  = null;
		}
		strRet = strBuf.toString();
		strBuf = null;
		return strRet;
	}
	
	@SuppressWarnings("serial")
	private class ItemInsensitiveMap extends LinkedHashMap<String, INIitems> {

	    @Override
	    public INIitems put(String key, INIitems items) {
	       return super.put(key.toLowerCase(), items);
	    }
	    
	    public INIitems remove(String key) {
		       return super.remove(key.toLowerCase());
		}

	    public INIitems get(String key) {
	       return super.get(key.toLowerCase());
	    }
	    
	    public boolean containsKey(String key) {
		       return super.containsKey(key.toLowerCase());		    
	    }
	}
	
}


