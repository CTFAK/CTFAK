/* Copyright (c) 1996-2015 Clickteam
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

import Actions.CActExtension;
import Application.CRunApp;
import Conditions.CCndExtension;
import Expressions.CValue;
import RunLoop.CCreateObjectInfo;
import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CBinaryFile;
import android.accounts.AccountManager;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.MotionEvent;

public class CRunOUYA extends CRunExtension
{

	private CValue expRet;

	public CRunOUYA() {
		expRet = new CValue();
	}

	@Override public int getNumberOfConditions()
	{
		return 57;
	}

	@Override
	public boolean createRunObject(CBinaryFile file, CCreateObjectInfo cob, int version)
	{
		return true;
	}

	public void OnSystemTapped(int flag) {
	}


	public boolean keyDown(int keyCode, KeyEvent msg)
	{
		return true;
	}

	public boolean keyUp(int keyCode, KeyEvent msg)
	{
		return true;
	}

	@Override
	public boolean condition(int num, CCndExtension cnd)
	{
		return false;
	}


	@Override
	public void action(int num, CActExtension act)
	{
		
	}

	@Override
	public CValue expression(int num)
	{
		return null;
	}

	public int getDefTouchPad() {
		return -1;
	}

	/////////////////////////////////////////////////////////////////////////////////////
	//
	//                     Joystick Utilities for Library dependencies
	//
	/////////////////////////////////////////////////////////////////////////////////////
	
	public static void init(Context context)
	{
	}
	
	public static int keyDownAtPlayerNumber(int keyCode, KeyEvent msg)
	{
		return getControllerPlayerNum(msg);
	}
	
	public static int keyUpAtPlayerNumber(int keyCode, KeyEvent msg)
	{
		return getControllerPlayerNum(msg);
	}
	
	public static int onMotionEvent(MotionEvent event) 
	{
		return  getControllerPlayerNum(event);
	}
	
	public static int getControllerPlayerNum(KeyEvent msg) {
		int player = -1;
		return player;
	}
	
	public static int getControllerPlayerNum(MotionEvent msg) {
		int player = -1;
		return player;
	}

}
