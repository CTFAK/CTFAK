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

import android.Manifest;
import android.Manifest.permission;
import android.accounts.Account;
import android.accounts.AccountManager;
import android.annotation.SuppressLint;
import android.app.KeyguardManager;
import android.bluetooth.BluetoothAdapter;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.os.BatteryManager;
import android.os.Environment;
import android.os.SystemClock;
import android.os.Vibrator;
import android.telephony.TelephonyManager;
import android.util.Log;
import android.view.InputDevice;
import android.view.KeyEvent;
import android.view.View;
import android.view.WindowManager;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import Actions.CActExtension;
import Application.CRunApp;
import Application.CRunApp.MenuEntry;
import Conditions.CCndExtension;
import Expressions.CValue;
import OpenGL.GLRenderer;
import RunLoop.CCreateObjectInfo;
import Runtime.MMFRuntime;
import Runtime.SurfaceView;
import Services.CBinaryFile;
import Services.CFile;
import Services.CServices;

//import java.util.ArrayList;


public class CRunAndroid extends CRunExtension
{
	String deviceID = "";
	String logTag;

	String google_email = "";

	Intent intentOut;

	private KeyguardManager keyguardManager = null;
	@SuppressWarnings("deprecation")
	private KeyguardManager.KeyguardLock lock = null;      

	Map<String, BroadcastReceiver> intentsIn;
	Intent intentIn;

	//private WindowManager.LayoutParams attrs;
	private int PERMISSIONS_ACCOUNTS_REQUEST = 12355586;
	private int PERMISSIONS_PHONE_REQUEST = 122455638;
	private HashMap<String, String> permissionsApi23;
	private boolean enabled_read;
	private boolean enabled_account;
	private boolean expression_request;
	
	private boolean IsControllerGamepad;
	private boolean IsControllerDPAD;
	private boolean IsControllerJoystick;
	public int lastKeyPressed;
	
	public TelephonyManager getTelephonyManager()
	{
		return (TelephonyManager) MMFRuntime.inst.getSystemService(Context.TELEPHONY_SERVICE);
	}

	public ConnectivityManager getConnectivityManager()
	{
		return (ConnectivityManager) MMFRuntime.inst.getSystemService(Context.CONNECTIVITY_SERVICE);
	}

	public Vibrator getVibrator ()
	{
		return (Vibrator) MMFRuntime.inst.getSystemService(Context.VIBRATOR_SERVICE);
	}

	public NetworkInfo getActiveNetworkInfo()
	{
		return getConnectivityManager().getActiveNetworkInfo();
	}

	@Override 
	public int getNumberOfConditions()
	{
		return 25;
	}

	@Override
	public boolean createRunObject(CBinaryFile file, CCreateObjectInfo cob, int version)
	{
		//attrs = MMFRuntime.inst.getWindow().getAttributes();

		intentOut = new Intent();
		intentsIn = new HashMap<String, BroadcastReceiver>();

		logTag = ho.getApplication().appName;

		ho.getApplication().androidObjects.add (this);

		enabled_read = false;
		enabled_account = false;
		
		if(MMFRuntime.deviceApi > 22)
			permissionsApi23 = new HashMap<String, String>();
		
		PackageManager pm = MMFRuntime.inst.getPackageManager();
		// do something
		if(MMFRuntime.deviceApi > 22) {
			if(!MMFRuntime.inst.verifyOkPermissionsApi23(permissionsApi23))
				MMFRuntime.inst.pushForPermissions(permissionsApi23, PERMISSIONS_PHONE_REQUEST);
			else
				enabled_read = true;
		}
		else if (pm.checkPermission(permission.READ_PHONE_STATE, MMFRuntime.inst.getPackageName()) == PackageManager.PERMISSION_GRANTED)
			enabled_read = true;

		// do something
		if(MMFRuntime.deviceApi > 22) {
			if(!MMFRuntime.inst.verifyOkPermissionsApi23(permissionsApi23))
				MMFRuntime.inst.pushForPermissions(permissionsApi23, PERMISSIONS_ACCOUNTS_REQUEST);
			else
				enabled_account = true;
		}
		else if (pm.checkPermission(permission.GET_ACCOUNTS, MMFRuntime.inst.getPackageName()) == PackageManager.PERMISSION_GRANTED)
			enabled_account = true;
		
		IsControllerGamepad = false;
		IsControllerJoystick = false;
		IsControllerDPAD = false;
		
		CheckForInputDevices();
		
		google_email = "";
		
		deviceID = "";
		
		return true;
	}

	@Override
	public void destroyRunObject(boolean bFast)
	{
		if(MMFRuntime.inst.keyBoardOn)
			MMFRuntime.inst.HideKeyboard(null, true);

		ho.getApplication().androidObjects.remove (this);

		for(BroadcastReceiver receiver : intentsIn.values())
			MMFRuntime.inst.unregisterReceiver (receiver);

		intentsIn.clear();


	}

	@Override
	public int handleRunObject()
	{
		if(MMFRuntime.inst != null) {
			MMFRuntime.inst.askForPermissionsApi23();		
		}
		return REFLAG_ONESHOT;
	}

	@Override
	public void continueRunObject() {
		CheckForInputDevices();
	}
	
	@Override
	public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults, List<Integer> permissionsReturned) {
		Log.d("Android", "On Request permisions returned ...");
		if(permissionsReturned.contains(PERMISSIONS_PHONE_REQUEST)) 
		{
			enabled_read = verifyResponseApi23(permissions, permissionsApi23);
			if(enabled_read)
				deviceID = getThisPhoneId();
			else
				deviceID = "";
		}
		else
			enabled_read = false;
		
		if(permissionsReturned.contains(PERMISSIONS_ACCOUNTS_REQUEST)) 
		{
			enabled_account = verifyResponseApi23(permissions, permissionsApi23);
			if(enabled_account )
				google_email = getEmail();
			else
				google_email = "";
		}
		else
			enabled_account = false;

	} 

	public int menuButtonEvent = -1;
	public int backButtonEvent = -1;
	public int intentEvent = -1;
	public int anyIntentEvent = -1;

	public int menuItemEvent = -1;
	public int anyMenuItemEvent = -1;
	public String menuItem;

	public int asyncLoadCompleteEvent = -1;
	public int asyncLoadFailedEvent = -1;

	@Override
	public boolean condition(int num, CCndExtension cnd)
	{
		switch (num)
		{
		case 0: /* Device has a GPU? */
			return GLRenderer.inst.gpuVendor.compareTo("Android") != 0;

		case 1: /* User is roaming? */
		{
			TelephonyManager tm = getTelephonyManager();

			if(tm == null)
				return false;

			return tm.isNetworkRoaming();
		}

		case 2: /* On extension exception */
			return false;

		case 3: /* Network is connected? */

			try
			{
				return getActiveNetworkInfo().isConnected();
			}
			catch (Throwable t)
			{
				return false;
			}
			//return isOnWIFI();
			
		case 4: /* Device is plugged in? */

			if(!MMFRuntime.inst.batteryReceived)
				return false;

			return MMFRuntime.inst.batteryPlugged == BatteryManager.BATTERY_PLUGGED_AC ||
					MMFRuntime.inst.batteryPlugged == BatteryManager.BATTERY_PLUGGED_USB;

		case 5: /* Device is plugged in to an AC adapter? */

			if(!MMFRuntime.inst.batteryReceived)
				return false;

			return MMFRuntime.inst.batteryPlugged == BatteryManager.BATTERY_PLUGGED_AC;

		case 6: /* Device is plugged in to a USB port? */

			if(!MMFRuntime.inst.batteryReceived)
				return false;

			return MMFRuntime.inst.batteryPlugged == BatteryManager.BATTERY_PLUGGED_USB;

		case 7: /* Battery is charging? */

			if(!MMFRuntime.inst.batteryReceived)
				return false;

			return MMFRuntime.inst.batteryStatus == BatteryManager.BATTERY_STATUS_CHARGING;

		case 8: /* Battery is discharging? */

			if(!MMFRuntime.inst.batteryReceived)
				return false;

			return MMFRuntime.inst.batteryStatus == BatteryManager.BATTERY_STATUS_DISCHARGING;

		case 9: /* Battery is full? */

			if(!MMFRuntime.inst.batteryReceived)
				return false;

			return MMFRuntime.inst.batteryStatus == BatteryManager.BATTERY_STATUS_FULL;

		case 10: /* On back button pressed */

			return ho.getEventCount() == backButtonEvent;

		case 11: /* On home button pressed */

			return false;

		case 12: /* On menu button pressed */

			return (ho.getEventCount() == menuButtonEvent);

		case 13: /* Device is rooted? */

			return MMFRuntime.rooted;

		case 14: /* Bluetooth enabled? */
		{
			BluetoothAdapter adapter = BluetoothAdapter.getDefaultAdapter();

			if (adapter == null || !adapter.isEnabled ())
				return false;

			return true;
		}

		case 15: /* Back button down? */

			return rh.rhApp.keyBuffer[KeyEvent.KEYCODE_BACK] != 0;

		case 16: /* Menu button down? */

			return rh.rhApp.keyBuffer[KeyEvent.KEYCODE_MENU] != 0;

		case 17: /* Button %0 down? */

			int keyCode = cnd.getParamExpression(rh, 0);

			if (keyCode < 0 || keyCode >= rh.rhApp.keyBuffer.length)
				return false;

			return rh.rhApp.keyBuffer[keyCode] != 0;

		case 18: /* On incoming intent with action %0 */

			return intentIn != null && intentIn.getAction().contentEquals(cnd.getParamExpString(rh, 0))
			&& ho.getEventCount() == intentEvent;

		case 19: /* On any incoming intent */

			return intentIn != null && ho.getEventCount() == anyIntentEvent;

		case 20: /* On options menu item selected */

			return ho.getEventCount() == menuItemEvent &&
			menuItem.compareToIgnoreCase(cnd.getParamExpString(rh, 0)) == 0;

		case 21: /* On any options menu item selected */

			return ho.getEventCount() == anyMenuItemEvent;

		case 22: /* On Visible Keyboard */

			return true;

		case 23: /* On Invisible Keyboard */

			return true;

		case 24: /* Is Keyboard Visible? */

			return MMFRuntime.inst.keyBoardOn;

		case 25: /* Is Permission Granted */
		{
			String p = cnd.getParamExpString(rh, 0);
			return MMFRuntime.inst.checkForPermission(p);
		}

		case 26: /* Is Controller with gamepad? */

			return IsControllerGamepad;

		case 27: /* Is controller with joystick? */

			return IsControllerJoystick;

		case 28: /* Is controller with DPAD? */
			return IsControllerDPAD;

		}

		return false;
	}

	private static boolean isOnWIFI() {

		 ConnectivityManager cm = (ConnectivityManager) MMFRuntime.inst.getSystemService(Context.CONNECTIVITY_SERVICE);
		 
		 if(cm != null){

			   NetworkInfo networkInfo = cm.getActiveNetworkInfo();
	
			   boolean isWiFi = networkInfo.getType() == ConnectivityManager.TYPE_WIFI;
	
			   // if user is connected to network
			   //if (isWiFi) {  
			   //   return true;
	
			   //}else {
			   //  return false;
	
			   //}
			   return isWiFi;

		 }
		 else{
		    //cm is null 
		    return false;

		 }  
		
	}	
	
	private void galleryAddPic(String file) {
		Intent mediaScanIntent = new Intent("android.intent.action.MEDIA_SCANNER_SCAN_FILE");
		File f = new File(file);
		Uri contentUri = Uri.fromFile(f);
		mediaScanIntent.setData(contentUri);
		MMFRuntime.inst.sendBroadcast(mediaScanIntent);
	}

	@SuppressLint("NewApi")
	@SuppressWarnings("deprecation")
	@Override
	public void action(int num, CActExtension act)
	{

		switch (num)
		{
		/* Log actions */

		case 0:    	
			Log.d(logTag, act.getParamExpString(rh, 0));
			break;
		case 1:    	
			Log.e(logTag, act.getParamExpString(rh, 0));
			break;
		case 2:    	
			Log.i(logTag, act.getParamExpString(rh, 0));
			break;
		case 3:    	
			Log.v(logTag, act.getParamExpString(rh, 0));
			break;
		case 4:    	
			Log.w(logTag, act.getParamExpString(rh, 0));
			break;

		case 5: /* Set log tag */	
			logTag = act.getParamExpString(rh, 0);
			break;

		case 6: /* Start sleep prevention */

			MMFRuntime.inst.runOnUiThread (new Runnable ()
			{   @Override
				public void run ()
			{   MMFRuntime.inst.getWindow().addFlags
				(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
			}
			});

			break;

		case 7: /* Stop sleep prevention */

			MMFRuntime.inst.runOnUiThread (new Runnable ()
			{   @Override
				public void run ()
			{   MMFRuntime.inst.getWindow().clearFlags
				(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
			}
			});

			break;

		case 8: /* Hide status bar */
                MMFRuntime.inst.ToggleStatusBar(false);
			break;

		case 9: /* Show status bar */
                MMFRuntime.inst.ToggleStatusBar(true);
			break;

		case 10: /* Open URL */
		{
			try
			{
				String url = act.getParamExpString(rh, 0);

				if(url.indexOf("://") == -1)
					url = "http://" + url;

				Intent intent = new Intent("android.intent.action.VIEW", Uri.parse(url));
				MMFRuntime.inst.startActivity(intent);
			}
			catch (Throwable e)
			{
			}

			break;
		}

		case 11: /* Start intent */
		{
			try
			{
				String action = act.getParamExpString(rh, 0);
				if(action.length() > 0)
					intentOut.setAction(action);

				String data = act.getParamExpString(rh, 1);
				if(data.length() > 0)
					intentOut.setData(Uri.parse (data));

				MMFRuntime.inst.startActivity(intentOut);
				
			}
			catch (Throwable e)
			{
				Log.e("MMFRuntime", "Error starting intent: " + e.toString());
			}

			intentOut = null;
			intentOut = new Intent();

			break;
		}

		case 12: /* Vibrate */

			Vibrator vi = getVibrator();
			if(vi == null || !MMFRuntime.inst.checkForPermission(permission.VIBRATE))
				return;

			vi.vibrate (act.getParamExpression (rh, 0));

			break;

		case 13: /* Show keyboard */

			MMFRuntime.inst.HideKeyboard(null, false);
			break;

		case 14: /* Hide keyboard */

			MMFRuntime.inst.HideKeyboard(null, true);
			break;

		case 15: /* Add intent category */

			intentOut.addCategory (act.getParamExpString(rh, 0));
			break;

		case 16: /* Add intent data (string) */

			intentOut.putExtra (act.getParamExpString(rh, 0), act.getParamExpString(rh, 1));
			break;

		case 17: /* Add intent data (boolean) */

			intentOut.putExtra(act.getParamExpString(rh, 0), act.getParamExpression(rh, 1) != 0);
			break;

		case 18: /* Add intent data (long) */

			intentOut.putExtra(act.getParamExpString(rh, 0), (long) act.getParamExpression(rh, 1));
			break;

		case 19: /* Subscribe to action */

			try
			{
				String action = act.getParamExpString(rh,  0);
				if (intentsIn.get(action) != null)
					break;

				BroadcastReceiver receiver = new BroadcastReceiver()
				{
					@Override
					public void onReceive(Context context, Intent intent)
					{
						intentIn = intent;

						intentEvent = ho.getEventCount();
						ho.generateEvent(18, 0);

						anyIntentEvent = ho.getEventCount();
						ho.generateEvent(19, 0);   

						intentIn = null;
					}
				};

				MMFRuntime.inst.registerReceiver
				(receiver, new IntentFilter(action));

				intentsIn.put(action, receiver);
			}
			catch(Exception e)
			{
			}

			break;

		case 20: /* Unsubscribe from action */

			BroadcastReceiver removed = intentsIn.remove(act.getParamExpString(rh, 0));

			if (removed != null)
				MMFRuntime.inst.unregisterReceiver(removed);

			break;     

		case 21: /* Start intent with chooser */
		{
			try
			{
				String action = act.getParamExpString(rh, 0);
				if(action.length() > 0)
					intentOut.setAction(action);

				String data = act.getParamExpString(rh, 1);
				if(data.length() > 0)
					intentOut.setData(Uri.parse (data));

				MMFRuntime.inst.startActivity(Intent.createChooser (intentOut, act.getParamExpString(rh, 2)));

			}
			catch (Throwable e)
			{
				Log.e("MMFRuntime", "Error starting intent: " + e.toString());
			}

			intentOut = null;
			intentOut = new Intent();

			break;
		}

		case 22: /* Enable options menu item */
		{
			String id = act.getParamExpString(rh, 0);

			if(rh.rhApp.androidMenu == null)
				break;

			for(MenuEntry item : rh.rhApp.androidMenu)
			{
				if(item.id.equalsIgnoreCase(id))
				{
					item.disabled = false;
					break;
				}
			}

			MMFRuntime.inst.invalidateOptionsMenu();
			break;
		}

		case 23: /* Disable options menu item */
		{
			String id = act.getParamExpString(rh, 0);

			if(rh.rhApp.androidMenu == null)
				break;

			for(MenuEntry item : rh.rhApp.androidMenu)
			{
				if(item.id.equalsIgnoreCase(id))
				{
					item.disabled = true;
					break;
				}
			}
			MMFRuntime.inst.invalidateOptionsMenu();
			break;
		}

		case 24: /* Show action bar */
			MMFRuntime.inst.runOnUiThread(new Runnable () {

				@Override
				public void run() {
					synchronized(MMFRuntime.inst) {
						MMFRuntime.inst.toggleActionBar(true);
					}              			
				}

			});

			break;     

		case 25: /* Hide Action bar */

			MMFRuntime.inst.runOnUiThread(new Runnable () {

				@Override
				public void run() {
					synchronized(MMFRuntime.inst) {
						MMFRuntime.inst.toggleActionBar(false);
					}              			
				}

			});
			break;     

		case 26: /* Force Landscape layout */

			MMFRuntime.inst.runOnUiThread(new Runnable () {

				@Override
				public void run() {
					synchronized(MMFRuntime.inst) {
						MMFRuntime.isShortOut = true;
						MMFRuntime.inst.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
					}
				}

			});
			break;     

		case 27: /* Force portrait layout */


			MMFRuntime.inst.runOnUiThread(new Runnable () {

				@Override
				public void run() {
					synchronized(MMFRuntime.inst) {
						MMFRuntime.isShortOut = true; 
						MMFRuntime.inst.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
					}
				}

			});
			break;     


		case 28: /* Save Screenshot to %0*/
			String filename = act.getParamExpString(rh, 0);
			if(filename.length() > 0)
				CaptureFromScreen(filename, 0, 0, -1, -1);
			break;     


		case 29: /* Open Menu via action */
			MMFRuntime.inst.runOnUiThread(new Runnable () {

				@Override
				public void run() {
					if (MMFRuntime.deviceApi >= 11)
						MMFRuntime.inst.invalidateOptionsMenu();
					SystemClock.sleep(100);
					MMFRuntime.inst.openOptionsMenu();
				}

			});

			break;

		case 30: /* Disable hardware keys for old devices api 11 and below */

			if(!MMFRuntime.inst.checkForPermission(permission.DISABLE_KEYGUARD))
				break;

			if(keyguardManager == null) {
				keyguardManager = (KeyguardManager) MMFRuntime.inst.getSystemService(Context.KEYGUARD_SERVICE);
				if(lock == null)
					lock = keyguardManager.newKeyguardLock("ALARM_RECEIVE");      
			}

			if(lock != null)
				lock.disableKeyguard();     

			break;

		case 31: /* Enable hardware keys for old devices api 11 and below */

			if(!MMFRuntime.inst.checkForPermission(permission.DISABLE_KEYGUARD))
				break;

			if(keyguardManager == null) {
				keyguardManager = (KeyguardManager) MMFRuntime.inst.getSystemService(Context.KEYGUARD_SERVICE);
				if(lock == null)
					lock = keyguardManager.newKeyguardLock("ALARM_RECEIVE");      
			}

			if(lock != null)
				lock.reenableKeyguard();     

			break;           	

		case 32: /* Send Application to back */
			MMFRuntime.inst.runOnUiThread(new Runnable () {

				@Override
				public void run() {
					synchronized(MMFRuntime.inst) {
						MMFRuntime.inst.sendBack();           	
					}              			
				}

			});
			break;    
			
			
	        //  256 View.SYSTEM_UI_FLAG_LAYOUT_STABLE			api 16
            //  512 View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION	api 16
            // 1024 View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN		api 16
			//	  2 View.SYSTEM_UI_FLAG_HIDE_NAVIGATION			api 14
			//	  1 View.SYSTEM_UI_FLAG_LOW_PROFILE				api 14
            //    4 View.SYSTEM_UI_FLAG_FULLSCREEN				api 16
            // 4096 View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY		api 19
			// 2048 View.SYSTEM_UI_FLAG_IMMERSIVE				api 19

		case 33: /* Show Navigation bar */
			MMFRuntime.inst.hideBar=false;
			MMFRuntime.inst.toggleNavigationBar(true, 1);
			break;     

		case 34: /* Dim Navigation bar */
			MMFRuntime.inst.hideBar=false;
			MMFRuntime.inst.toggleNavigationBar(false, 1);
			break;     

		case 35: /* Hide Navigation Bar */
			MMFRuntime.inst.hideBar=true;
			MMFRuntime.inst.toggleNavigationBar(false, View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION);
			break;     


		case 36: /* Add intent setType */

			intentOut.setType(act.getParamExpString(rh, 0));
			break;

		case 37: /* Disable AutoEnd */

			MMFRuntime.inst.app.hdr2Options &= ~ CRunApp.AH2OPT_AUTOEND;
			break;

		case 38: /* Enable AutoEnd */

			MMFRuntime.inst.app.hdr2Options |= CRunApp.AH2OPT_AUTOEND;
			break;

		case 39: /* Set Character Set */

			if(act.getParamExpression(rh, 0) > 0)
				MMFRuntime.inst.charSet = CFile.charset ;
			else
				MMFRuntime.inst.charSet = CFile.charsetU8 ;

			break;

		case 40: /* Add intent data (Uri) */

			intentOut.putExtra (act.getParamExpString(rh, 0), Uri.parse(act.getParamExpString(rh, 1)));
			break;

		case 41: /* Capture portion of screen at x,y with size w and h */
		{
			String filename2 = act.getParamExpString(rh, 0);
			int x = act.getParamExpression(rh, 1);
			int y = act.getParamExpression(rh, 2);
			int w = act.getParamExpression(rh, 3);
			int h = act.getParamExpression(rh, 4);
			
			if(w+x > ho.hoAdRunHeader.rhLevelSx || w < 0 || x < 0)
				return;
			if(h+y > ho.hoAdRunHeader.rhLevelSy || h < 0 || y < 0)
				return;
			
			if(filename2.length() > 0)
				CaptureFromScreen(filename2, x, y, w, h);

			break;     
		}
		case 42: /* Check for InputDevices */

			IsControllerGamepad = false;
			IsControllerJoystick = false;
			IsControllerDPAD = false;
			
			CheckForInputDevices();
			break;
		case 43: /* Disable menu behavior */

			MMFRuntime.inst.disableMenuBehavior = true;
			break;
		case 44: /* Enable menu behavior */

			MMFRuntime.inst.disableMenuBehavior = false;
			break;
		case 45:
			{
			final int var = act.getParamExpression(rh, 0);
			if (MMFRuntime.deviceApi >= 19) {
				MMFRuntime.inst.runOnUiThread(new Runnable() {
					@Override
					public void run() {
						synchronized (MMFRuntime.inst) {
							if (var != 0)
								MMFRuntime.inst.hideSystemUI();
							else
								MMFRuntime.inst.showSystemUI();
						}
					}

				});
			}
			break;
			}
		case 46: /* Load image async */

			/*String url = act.getParamExpString(rh, 0);
            	CObject object = act.getParamObject(rh, 1);

            	if(! (object instanceof CExtension))
            	    return;

            	if (((CExtension) object).ext instanceof CRunkcpica)
                {
                    final CRunkcpica ext = (CRunkcpica) ((CExtension) object).ext;

                    ho.retrieveHFile(url, new CRunApp.FileRetrievedHandler()
                    {
                        @Override
                        public void onRetrieved(CRunApp.HFile file, java.io.InputStream stream)
                        {
                            try
                            {
                                Log.Log("Android object: Image retrieved, " + stream.available() + " bytes available");
                            }
                            catch(IOException e)
                            {
                            }

                            ext.load(stream);

                            asyncLoadCompleteEvent = ho.getEventCount();
                            ho.generateEvent(25, 0);
                        }

                        @Override
                        public void onFailure()
                        {
                            Log.Log("Android object: Failure w/ async image download");

                            asyncLoadFailedEvent = ho.getEventCount();
                            ho.generateEvent(26, 0);
                        }
                    });
                }                   */

			break;
		}
	}





	private static String getManufacturerSerialNumber() {
		String serial = ""; 
		try {
			Class<?> c = Class.forName("android.os.SystemProperties");
			Method get = c.getMethod("get", String.class, String.class);
			serial = (String) get.invoke(c, "ril.serialnumber", "unknown");
		} catch (Exception ignored) {}

		return serial;
	}


	private String getEmail() {
		String email = null;
		try {
			AccountManager accountManager = AccountManager.get(MMFRuntime.inst.getApplicationContext()); 
			Account account = getAccount(accountManager);

			if (account != null) 
				email = account.name;
		} catch (Exception e)
		{

		}
		return email == null ? "" : email;
	}

	private Account getAccount(AccountManager accountManager) throws SecurityException {
		Account account = null;

		if (enabled_account)
		{

			Log.d("AndroidRuntime", "permission granted for accounts");
			Account[] accounts = accountManager.getAccountsByType("com.google");
			if (accounts.length > 0)
			{
				account = accounts[0];      
			} 
			else
			{
				account = null;
			}
		}
		return account;
	} 

	@Override
	public CValue expression(int num)
	{
		String key;

		switch (num)
		{
		case 0: /* GPU_Name$ */
			return new CValue(GLRenderer.inst.gpu);

		case 1: /* GPU_Vendor$ */
			return new CValue(GLRenderer.inst.gpuVendor);

		case 2: /* DeviceID$ */
		{
			return new CValue(deviceID);
		}

		case 3: /* Operator$ */
		{
			TelephonyManager tm = getTelephonyManager();

			if(tm == null)
				return new CValue("");

			return new CValue(tm.getNetworkOperatorName().trim());
		}

		case 4: /* StackTrace$ */

			return new CValue("");

		case 5: /* AppTitle$() */
			return new CValue(ho.getApplication().appName);

		case 6: /* BatteryPercentage() */

			if(!MMFRuntime.inst.batteryReceived)
				return new CValue(-1);

			return new CValue(100 * ((float) MMFRuntime.inst.batteryLevel / (float) MMFRuntime.inst.batteryScale));

		case 7:
			return new CValue(ho.getApplication().gaCxWin);

		case 8:
			return new CValue(ho.getApplication().gaCyWin);

		case 9:

			return new CValue (MMFRuntime.inst.getFilesDir ().toString ());

		case 10:

			return new CValue (rh.getTempPath ());

		case 11:

			return new CValue (CServices.getAndroidID ());

		case 12:

			return new CValue (MMFRuntime.inst.getClass().getName());

		case 13:

			return new CValue (Environment.getExternalStorageDirectory ().getAbsolutePath());

		case 14:

			return new CValue (MMFRuntime.appVersion);

		case 15:

			return new CValue (MMFRuntime.version);

		case 16:

			//if (MMFRuntime.inst.adView == null)
				return new CValue (0);

			//return new CValue (MMFRuntime.inst.adView.getHeight ());

		case 17:

			return new CValue (GLRenderer.inst.glVersion);

		case 18: /* IntentAction$ */

			if (intentIn == null)
				return new CValue("");

			return new CValue(intentIn.getAction());

		case 19: /* IntentData$ */

			if (intentIn == null)
				return new CValue("");

			return new CValue(intentIn.getDataString());

		case 20: /* IntentExtra_String$ */

			key = ho.getExpParam().getString();

			if (intentIn == null)
				return new CValue("");

			String extra = intentIn.getStringExtra(key);

			return new CValue(extra == null ? "" : extra);

		case 21: /* IntentExtra_Boolean */

			key = ho.getExpParam().getString();

			if (intentIn == null)
				return new CValue("");

			return new CValue(intentIn.getBooleanExtra(key, false) ? 1 : 0);

		case 22: /* IntentExtra_Long */

			key = ho.getExpParam().getString();

			if (intentIn == null)
				return new CValue(0);
			
			return new CValue(intentIn.getIntExtra(key, 0));

		case 23: /* MenuItem$ */

			return new CValue(menuItem);

		case 24: /* Android Api version */

			return new CValue(android.os.Build.VERSION.SDK_INT);

		case 25: /* Manufacturer$ */

			return new CValue(android.os.Build.MANUFACTURER);

		case 26: /* Model$ */

			return new CValue(android.os.Build.MODEL);

		case 27: /* Product$ */

			return new CValue(android.os.Build.PRODUCT);

		case 28: /* PublicStorageDirectory$ */
			// Pictures, Movies, DCIM, Documents, Videos, Downloads
			String type = ho.getExpParam().getString();
			return new CValue (Environment.getExternalStoragePublicDirectory(type).getAbsolutePath());

		case 29: /* DeviceSerialNumber$ */

			return new CValue(getManufacturerSerialNumber());

		case 30: /* AccountEMail$ */
			return new CValue(google_email);
		case 31: /* SecondaryExternalStorage$ */
			String dir = System.getenv("SECONDARY_STORAGE"); //Environment.getExternalStorageDirectory ().getAbsolutePath()
			return new CValue(dir);
		case 32: /* LastButtonPressed */
			return new CValue(lastKeyPressed);
			
		}

		return new CValue(0);
	}

	private void CaptureFromScreen(String filename, int x, int y, int w, int h) {
		
		int typeImg = 0;

        Paint paint = new Paint();
		paint.setFilterBitmap(true);
		
		//  Got Control Layer
		View mView = MMFRuntime.inst.mainView;
		mView.setDrawingCacheEnabled(true);
		Bitmap bmp2 = Bitmap.createBitmap(mView.getDrawingCache());
		mView.setDrawingCacheEnabled(false);

		if(w == -1)
			w = (int)((MMFRuntime.inst.currentWidth - 2*MMFRuntime.inst.viewportX)/MMFRuntime.inst.scaleX);
		if(h == -1)
			h = (int)((MMFRuntime.inst.currentHeight -2*MMFRuntime.inst.viewportY)/MMFRuntime.inst.scaleY);
		
		// Got Surface View
		Bitmap bmp1 = SurfaceView.inst.drawBitmap(x,y,w,h);

		// Create Overlay Bitmap
		//Bitmap bmMix = Bitmap.createBitmap(bmp1.getWidth(), bmp1.getHeight(), bmp1.getConfig());
		
		//Canvas canvas = new Canvas(bmMix);
		//
		Bitmap bmOverlay = Bitmap.createBitmap(bmp1.getWidth(), bmp1.getHeight(), bmp1.getConfig());
		
		Canvas canvas = new Canvas(bmOverlay);

		canvas.drawBitmap(bmp1, 0, 0, paint);
		canvas.drawBitmap(bmp2, 0, 0, paint);
		/*
		Matrix matrix = new Matrix();
        matrix.postScale(1/(float)(MMFRuntime.inst.scaleX), 1/(float)(MMFRuntime.inst.scaleY));
        Bitmap bmOverlayScreen = Bitmap.createBitmap(bmMix, MMFRuntime.inst.viewportX, MMFRuntime.inst.viewportY,  MMFRuntime.inst.currentWidth-2*MMFRuntime.inst.viewportX, MMFRuntime.inst.currentHeight -2*MMFRuntime.inst.viewportY, matrix, true);

        Bitmap bmOverlay = null;
        bmOverlay = Bitmap.createBitmap(bmOverlayScreen);
       
        if(w!= -1 || h != -1)
        {
        	int offsetX = 0;
        	if(ho.hoAdRunHeader.rhApp.viewMode != 1)
        		offsetX = ho.hoAdRunHeader.rhApp.widthSetting/2 - ho.hoAdRunHeader.rhFrame.leWidth/2;
        	bmOverlay = Bitmap.createBitmap(bmOverlay, x+offsetX, y, w, h);
        }
        */
        String filenameArray[] = filename.split("\\.");
		String extension = filenameArray[filenameArray.length-1];

		if(extension == null)
			filename += ".jpg";
		else if(extension.toLowerCase(Locale.US).contains("png"))
			typeImg = 1;

		File mPath = new File(filename);
		FileOutputStream fileos = null;

		try {

			fileos = new FileOutputStream(mPath);
			if(typeImg == 0)
				bmOverlay.compress(Bitmap.CompressFormat.JPEG, 100, fileos);
			else
				bmOverlay.compress(Bitmap.CompressFormat.PNG, 100, fileos);
			fileos.flush();
			fileos.close();

			galleryAddPic(filename);

		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		bmp1.recycle();
		bmp1 = null;
		
		bmp2.recycle();
		bmp2 = null;
		
		//bmMix.recycle();
		//bmMix = null;
		
		bmOverlay.recycle();
		bmOverlay = null;
	}
	
	private String getThisPhoneId()
	{
		String id = null;
		try {
			TelephonyManager tm = getTelephonyManager();

			if(enabled_read && tm != null)
				id = tm.getDeviceId();

		} catch (Exception e)
		{

		}

		return (id == null ? "" : id);
	}
	
	private void CheckForInputDevices() {
		int nGamepad =0;
		int nJoystick=0;
		int nDPAD = 0;
		
		int[] deviceIds = InputDevice.getDeviceIds();
	    for (int deviceId : deviceIds) {
	        InputDevice dev = InputDevice.getDevice(deviceId);
	        int sources = dev.getSources();

	        // Verify that the device has gamepad buttons, control sticks, or both.
	        if ((sources & InputDevice.SOURCE_GAMEPAD) == InputDevice.SOURCE_GAMEPAD)
	        	nGamepad++;
	        
	        if ((sources & InputDevice.SOURCE_JOYSTICK) == InputDevice.SOURCE_JOYSTICK)
	        	nJoystick++;
	        
	        if ((sources & InputDevice.SOURCE_DPAD) == InputDevice.SOURCE_DPAD)
	        	nDPAD++;
	        
	    }
	    IsControllerGamepad = (nGamepad > 0 ? true : false);
	    IsControllerJoystick= (nJoystick > 0 ? true : false);
	    IsControllerDPAD = (nDPAD > 0 ? true : false);
	    if(IsControllerJoystick) {
	    	MMFRuntime.NEXUSTV = true;
			CRunNEXUSTV.init(MMFRuntime.inst);
	    }
	}
}
