package Runtime;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;

public class PermissionsApi23Util {
	
	private static int PERMISSIONS_REQUEST_API23 = 12344456;
	
	private int returnRequest;

	private Activity activity;
	
	private List<String> permissionsNeed;
	private List<String> permissionsRequested;
	private List<String> permissionsTitle;
	private List<String> permissionsApproved;
	private List<String> permissionsManifest;
	
	public List<Integer> permsReturnedCode;
	
	private boolean asked;
	
	public PermissionsApi23Util(Activity a) {
		activity = a;
		permissionsNeed = new ArrayList<String>();
		permissionsRequested = new ArrayList<String>();
		permissionsTitle = new ArrayList<String>();
		permissionsApproved = new ArrayList<String>();
		permsReturnedCode = new ArrayList<Integer>();
		permissionsManifest = new ArrayList<String>();
		
		readManifestPermissions();
	}

	public boolean checkIfPermissionInManifest(String permission) {
		boolean bRet = false;
		if(permissionsManifest.contains(permission))
			bRet = true;
		
		return bRet;
	}
	public int pushPermissionsApi23(HashMap<String,String>permissions, int returnCode) {
		int count = 0;
		for (Map.Entry<String, String> entry : permissions.entrySet()) {
			String key_perm = entry.getKey();
			
			if(!permissionsManifest.contains(key_perm))
				continue;
			
			if(!permissionsRequested.contains(key_perm)) {
				count++;
				permsReturnedCode.add(returnCode);
				permissionsNeed.add(key_perm);
			    if (addPermission(permissionsNeed, key_perm)) {
			    	permissionsTitle.add(entry.getValue());
					permissionsRequested.add(key_perm);
			    }
		    }
		}	
		returnRequest = returnCode;
		return count;
	}
	
	private boolean addPermission(List<String>permissionsNeeded, String permission) {
	    if (checkSelfPermissionApi23(permission) != PackageManager.PERMISSION_GRANTED)
			return true;
	    return false;
	}
	
	private boolean verifyOkPermissionsApi23(List<String>permissionsNeeded) {
		boolean bRet = true;
		for(String permission : permissionsNeeded) {
		    if (permission != null && checkSelfPermissionApi23(permission) != PackageManager.PERMISSION_GRANTED) {
		        bRet &= false;
		        break;
		    }
		}
	    return bRet;
	}
	
	public void setApproved(String[] permissions) {
		permissionsApproved = new ArrayList<String>(Arrays.asList(permissions));
	}
	
	public void clearPermissions() {
    	asked = false;
		permissionsNeed.clear();
		permissionsTitle.clear();
		permissionsRequested.clear();
	}
	
	public void reset() {
	   	asked = false;		
	}
	
	// Using Reflection to avoid error in lower api during building
	public void requestPermissionsApi23(String[] permission, int returnCode) {
		
		//String parameter
		Class[] params = new Class[2];	
		params[0] = String[].class;
		params[1] = Integer.TYPE;

		try
		{
			Class<?> c = activity.getClass();
			Object obj = activity;
			
			if ( c != null) {
				Method m = c.getMethod("requestPermissions", params);
				m.invoke(obj, permission, returnCode);
			}
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}		
	}

	public int checkSelfPermissionApi23(String permission) {
		
		//String parameter
		Class[] params = new Class[1];	
		params[0] = String.class;

		try
		{
			Class<?> c = activity.getClass();
			Object obj = activity;
			
			if ( c != null) {
				Method m = c.getMethod("checkSelfPermission", params);
				Object o = m.invoke(obj, permission);
				if(o != null) {
					Integer i = (Integer)o;
					return i.intValue();
					
				}
			}
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
		
		return -1;
	}
	
	public boolean shouldShowRequestPermissionRationaleApi23(String permission) {
		
		//String parameter
		Class[] params = new Class[1];	
		params[0] = String.class;

		try
		{
			Class<?> c = activity.getClass();
			Object obj = activity;
			
			if ( c != null) {
				Method m = c.getMethod("shouldShowRequestPermissionRationale", params);
				Object o = m.invoke(obj, permission);
				if(o != null) {
					Boolean b = (Boolean)o;
					return b.booleanValue();					
				}
			}
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
		
		return false;
	}

	public void askForPermissionsApi23() {
		
		if(verifyOkPermissionsApi23(permissionsRequested))
			return;
		
	    if (!asked && permissionsRequested.size() > 0) {
	    	asked = true;
	    	if(permissionsRequested.size() >= 1)
	    		returnRequest = PERMISSIONS_REQUEST_API23;

            requestPermissionsApi23(permissionsRequested.toArray(new String[permissionsRequested.size()]), returnRequest);
	    }

	}

	public void askForPermissionsApi23(String message) {

		if(verifyOkPermissionsApi23(permissionsRequested))
			return;

		if (!asked && permissionsRequested.size() > 0) {
			asked = true;
			if(permissionsRequested.size() >= 1)
				returnRequest = PERMISSIONS_REQUEST_API23;

			if (permissionsNeed.size() > 0) {
				// Need Rationale
				showMessageOKCancel(message,
				        new DialogInterface.OnClickListener() {
				            @Override
				            public void onClick(DialogInterface dialog, int which) {
				                if(which == Dialog.BUTTON_POSITIVE)
				                    requestPermissionsApi23(permissionsRequested.toArray(new String[permissionsRequested.size()]), returnRequest);
				                else
				                    MMFRuntime.inst.doFinish();
				            }
				        });
				return;
			}
		}

	}

	private void showMessageOKCancel(String message, DialogInterface.OnClickListener okListener) {
	    new AlertDialog.Builder(activity)
	            .setMessage(message)
	            .setPositiveButton("OK", okListener)
	            .setNegativeButton("Cancel", okListener)
	            .create()
	            .show();
	}

	private void readManifestPermissions() {	
		permissionsManifest.clear();
		if(MMFRuntime.targetApi > 22) {
		    try {
		        PackageInfo info = activity.getPackageManager().getPackageInfo(activity.getApplicationContext().getPackageName(), PackageManager.GET_PERMISSIONS);
		        if (info.requestedPermissions != null) {
		            for (String p : info.requestedPermissions) {
		            	permissionsManifest.add(p);
		            }
		        }
		    } catch (Exception e) {
		        e.printStackTrace();
		    }
		}
	}
}
