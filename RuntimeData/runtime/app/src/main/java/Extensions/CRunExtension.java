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
// CRUNEXTENSION: Classe abstraite run extension
//
//----------------------------------------------------------------------------------
package Extensions;

import java.util.HashMap;
import java.util.List;

import Actions.CActExtension;
import Application.CRunApp;
import Conditions.CCndExtension;
import Expressions.CValue;
import Objects.CExtension;
import RunLoop.CCreateObjectInfo;
import RunLoop.CRun;
import Runtime.MMFRuntime;
import Services.CBinaryFile;
import Services.CFontInfo;
import Services.CRect;
import Sprites.CMask;
import android.content.Intent;
import android.view.Menu;

public abstract class CRunExtension 
{   
    public final static int REFLAG_DISPLAY=1;
    public final static int REFLAG_ONESHOT=2;
    public CExtension ho;
    public CRun rh;
    
    public CRunExtension() 
    {
    }

    public void init(CExtension hoPtr)
    {
		ho=hoPtr;
		rh=hoPtr.hoAdRunHeader;
    }
    
    public int getNumberOfConditions()
    {
    	return 0;
    }
    
    public boolean createRunObject(CBinaryFile file, CCreateObjectInfo cob, int version)
    {
    	return false;
    }
    
    public int handleRunObject()
    {
    	return REFLAG_ONESHOT;
    }
    
    public void displayRunObject()
    {
    }

    public void reinitDisplay ()
    {
    }

    public void destroyRunObject(boolean bFast)
    {
    }
    
    public void pauseRunObject()
    {
    }
    
    public void continueRunObject()
    {
    }

    public void getZoneInfos()
    {
    }
    
    public boolean condition(int num, CCndExtension cnd)
    {
    	return false;
    }
    
    public void action(int num, CActExtension act)
    {
    }
    
    public CValue expression(int num)
    {
    	return new CValue(0);
    }
    
    public CMask getRunObjectCollisionMask(int flags)
    {
    	return null;
    }
    
    public CFontInfo getRunObjectFont()
    {
    	return null;
    }
    
    public void setRunObjectFont(CFontInfo fi, CRect rc)
    {
    }
    
    public int getRunObjectTextColor()
    {
    	return 0;
    }
    
    public void setRunObjectTextColor(int rgb)  
    {
    }

    public boolean onCreateOptionsMenu (Menu menu)
    {
        return false;
    }
    /**
     * Received when you reach the end of game loop
     * used to update some object when all events are considered in a loop
     */
    public void onEndOfGameLoop ()
    {
    }
    /**
     *	Receive the onStart from main activity
	 *	not from the created method but from
	 *	stopped after Restart
     */
	public void onStart()
	{
	}
	
	/**
	 * Receive the onStop method from main activity
	 * useful to grant or stop painting since teh application goes to background
	 */
	public void onStop()
	{
	}
		
	/**
	 * Receive the final onDestroy request and proceed to destroy each running objects
	 * useful to grant that data is saved before application die
	 */
	public void onDestroy()
	{
	   	// receive the onDestroy from main activity
		if(this != null)
			destroyRunObject(false);
	}
	/**
	 * Receive th OnBackPresed from main activity if the default behavior is removed from
	 * android application properties.
	 * always return true if not processed.
	 */
	public boolean onBackPressed()
	{
        return true;
	}
	
	/**
	 * This function is related to API 23 and is need to be code inside each extension that require danger
	 * permission as mentioned in MMFRuntime, method approvedPermissionApi23(String, int)
	 * 
	 * <pre>
	 * e.g:
	 * {@code
	 * @Override
	 * public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults, List<Integer> permissionsReturned) {
	 *		if(permissionsReturned.contains(PERMISSIONS_COMBO_REQUEST))
	 *			enabled_perms = verifyResponseApi23(permissions, permissionsApi23);
	 *		else
	 *			enabled_perms = false;
	 * 	}
     *
	 *   }
	 *}</pre>
	 * @param requestCode integer to use in detect of permission grants and execute teh action(s)
	 * @param permissions string corresponding to the permission to allow 
	 * @param grantResults
	 */
	public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults, List<Integer> permissionsReturned) {
		if(MMFRuntime.deviceApi < 23)
			return;	
	}
	
	public boolean verifyResponseApi23(String permissions[], HashMap<String, String> permissionsRequested) {
		int num = 0;
		for(String s : permissionsRequested.keySet()) {
			for(String p : permissions) {
				if(p.contentEquals(s)) {
					num++;
					break;
				}
			}
		}
		if(num > 0)
			return true;
		
		return false;

	}
	
	/**
	 * Receive the activityResult from main activity
	 * important to compare requestCode to correctly identify the
	 * the activity that started. check also {@link MMFRuntime#onActivityResult(int requestCode, int resultCode, Intent data)}
	 * 
	 * @param requestCode
	 * @param resultCode
	 * @param data
	 */
    public void onActivityResult(int requestCode, int resultCode, Intent data)
    {
    }
    	 	
    /**
     * Each Object can add it owns items to menu
     * for this need to push new menu to global menu
     * 
     * @param menu
     * @return
     */
    public boolean onPrepareOptionsMenu (Menu menu)
    {
        return false;
    }

    /**
     * After a new Intent this method is trigger a scan in all object to act as is need to do.
     * 
     * @param intent
     */
	public void onNewIntent(Intent intent)
	{
    	// receive the onNewIntent from main activity
	}
	
 }
