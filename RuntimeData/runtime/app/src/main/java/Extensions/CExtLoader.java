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

import Application.CRunApp;
import Services.CFile;

public class CExtLoader 
{
    public static final int KPX_BASE=32;
    
    CRunApp app;
    CExtLoad extensions[]=null;
    short numOfConditions[];
    int extMaxHandles;
    
    public CExtLoader(CRunApp a) 
    {
    	app=a;
    }
    public void loadList(CFile file) 
    {
		int extCount=file.readAShort();
		extMaxHandles=file.readAShort();
	
		extensions=new CExtLoad[extMaxHandles];
		numOfConditions=new short[extMaxHandles];
		int n;
		for (n=0; n<extMaxHandles; n++)
		{
		    extensions[n]=null;
		}

		for (n=0; n<extCount; n++)
		{
		    CExtLoad e=new CExtLoad();
		    e.loadInfo(file);

		    CRunExtension ext=e.loadRunObject();

            if (ext != null)
            {
                extensions[e.handle]=e;
                numOfConditions[e.handle]=(short)ext.getNumberOfConditions();
            }
		}
    }
    public CRunExtension loadRunObject(int type)
    {
         type-=KPX_BASE;
         CRunExtension ext=null;
        if (type<extensions.length && extensions[type]!=null)
        {
            ext=extensions[type].loadRunObject();
        }
         return ext;
    }  
    public int getNumberOfConditions(int type)
    {
        type -= KPX_BASE;
        if (type<extensions.length)
        {
            return numOfConditions[type];
        }
        return 0;
    }
}
