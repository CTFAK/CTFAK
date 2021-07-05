/* Copyright (c) 1996-2019 Clickteam
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
// MultiVar
//
//----------------------------------------------------------------------------------
package Params;

import Application.CRunApp;
import Expressions.CValue;

public class MultiVar
{
	public int index;
	public int op;
	public boolean global;
	public CValue val;

    public MultiVar(CRunApp app, boolean _global, boolean _double)
    {
		index = app.file.readAInt();
		op = app.file.readAInt();
		global = _global;
		if (_double) {
			val = new CValue(0);
			val.forceDouble(app.file.readADouble());
		} else {
			val = new CValue(app.file.readAInt());
			app.file.skipBytes(4);
		}
    }
}
