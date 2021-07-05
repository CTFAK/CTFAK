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

import Objects.CExtension;
import Objects.CObject;

public class CINI
{
	INIFile inifile = null;
	CExtension ho;

    String currentGroup;
    String currentItem;

    int flags = 0;


    public CINI (CExtension ho, String filename, int flags)
    {
        this.flags = flags;
        this.ho = ho;

        currentGroup = "Group";
        currentItem = "Item";

        inifile = new INIFile(ho, filename, flags);
    }

    public void close ()
    {
        if(inifile != null) {
        	inifile.save();
        	inifile.clear();
        	inifile = null;
        }
    }
   
    public void update() {
    	if(inifile != null)
    		inifile.save();
    }
    
    public void setCurrentGroup (String group)
    {
        currentGroup = group;
    }
    
    public void setCurrentItem (String item)
    {
        currentItem = item;
    }

    public void setCurrentFile (String filename)
    {
        if(inifile != null)
        	inifile.setFileName(filename);
    }

    public void setValue (int value)
    {
        if(inifile != null)
        	inifile.setIntegerItem(currentGroup, currentItem, value);
        //update();
    }

    public void saveObjectPos (CObject object)
    {
        String item = "pos." + object.hoOiList.oilName;
        String value = Integer.toString(object.hoX) + "," + Integer.toString(object.hoY);

        if(inifile != null)
        	inifile.setStringItem(currentGroup, item, value);
        //update();
    }

    public void loadObjectPos (CObject object)
    {
        String item = "pos." + object.hoOiList.oilName;
        String value = "";
        
        if(inifile != null)
        	value = inifile.getStringItem(currentGroup, item);

        if(value.length() == 0)
            return;

        String[] tokens = value.split("\\,");

        object.hoX = CServices.parseInt(tokens[0]);
        object.hoY = CServices.parseInt(tokens[1]);

        object.roc.rcChanged = true;
        object.roc.rcCheckCollides = true;
    }

    public void setString (String s)
    {
        if(inifile != null)
        	inifile.setStringItem(currentGroup, currentItem, s);
        //update();
    }

 
    public void setItemValue (String item, int value)
    {
        if(inifile != null)
        	inifile.setIntegerItem(currentGroup, item, value);
        //update();
    }

    public void setGroupItemValue (String group, String item, int value)
    {
        if(inifile != null)
        	inifile.setIntegerItem(group, item, value);
        //update();
    }

    public void setItemString (String item, String s)
    {
        if(inifile != null)
        	inifile.setStringItem(currentGroup, item, s);
        //update();
    }

    public void setGroupItemString (String group, String item, String s)
    {
        if(inifile != null)
        	inifile.setStringItem(group, item, s);
        //update();
    }

    public void deleteItem (String item)
    {
        if(inifile != null)
        	inifile.removeItem(currentGroup, item);
        //update();
   }

    public void deleteGroupItem (String group, String item)
    {
        if(inifile != null)
        	inifile.removeItem(group, item);
        //update();
   }

    public void deleteGroup (String group)
    {
        if(inifile != null)
        	inifile.removeGroup(group);
        //update();
    }

    public int getValue ()
    {
        if(inifile == null)
        	return 0;
    	return inifile.getIntegerItem(currentGroup, currentItem);
    }

    public String getString ()
    {
        if(inifile == null)
        	return "";
        return inifile.getStringItem(currentGroup, currentItem);
    }

    public int getItemValue (String item)
    {
        if(inifile == null)
        	return 0;
        return inifile.getIntegerItem(currentGroup, item);
    }

    public int getGroupItemValue (String group, String item)
    {
     	
    	if(inifile == null)
        	return 0;
        return inifile.getIntegerItem(group, item);
    }

    public String getItemString (String item)
    {
        if(inifile == null)
        	return "";
        return inifile.getStringItem(currentGroup, item);
    }

    public String getGroupItemString (String group, String item)
    {
        if(inifile == null)
        	return "";
        return inifile.getStringItem(group, item);
        
    }



}
