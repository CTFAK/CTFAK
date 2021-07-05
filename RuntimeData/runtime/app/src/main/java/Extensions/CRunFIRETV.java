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
import Conditions.CCndExtension;
import Expressions.CValue;
import RunLoop.CCreateObjectInfo;
import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CBinaryFile;
import android.content.Context;
import android.view.KeyEvent;
import android.view.MotionEvent;


public class CRunFIRETV extends CRunExtension
{

	// <editor-fold defaultstate="collapsed" desc=" A/C/E Constants ">
	public static final int CNDISDEVICEFTV = 0;

	public static final int CND_LAST = 40;

	public static final int ACTSTARTFRAME = 0;

	public static final int EXPCURRENTPLAYER = 0;
	public static final int EXPLEFTSTICKXCP = 1;
	public static final int EXPLEFTSTICKYCP = 2;
	public static final int EXPRIGHTSTICKXCP = 3;
	public static final int EXPRIGHTSTICKYCP = 4;
	public static final int EXPL2CP = 5;
	public static final int EXPR2CP = 6;
	public static final int EXPLEFTSTICKXPN = 7;
	public static final int EXPLEFTSTICKYPN = 8;
	public static final int EXPRIGHTSTICKXPN = 9;
	public static final int EXPRIGHTSTICKYPN = 10;
	public static final int EXPL2PN = 11;
	public static final int EXPR2PN = 12;

	private int lastButtonPress;
	private int lastButtonReleased;
	
	
	public CRunFIRETV() {
		expRet = new CValue();
	}

	@Override public int getNumberOfConditions()
	{
		return CND_LAST;
	}

	private CValue expRet;


	@Override
	public boolean createRunObject(CBinaryFile file, CCreateObjectInfo cob, int version)
	{
		return true;
	}

	public boolean keyDown(int keyCode, KeyEvent msg)
	{
		return false;
	}

	public boolean keyUp(int keyCode, KeyEvent msg)
	{
		return false;
	}
	
	// Conditions
	// -------------------------------------------------
	public @Override boolean condition(int num, CCndExtension cnd)
	{
		return false;
	}

	// Actions
	// -------------------------------------------------
	public @Override void action(int num, CActExtension act)
	{
	}

	// Expressions
	// -------------------------------------------------
	public @Override CValue expression(int num)
	{
		return null;
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
		return player-1;
	}
	
	public static int getControllerPlayerNum(MotionEvent msg) {
		int player = -1;
		return player-1;
	}

}
