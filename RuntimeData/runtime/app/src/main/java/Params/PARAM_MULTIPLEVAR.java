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
// PARAM_MULTIPLEVAR
//
//----------------------------------------------------------------------------------
package Params;

import java.util.ArrayList;
import Application.CRunApp;
import Params.MultiVar;
import Objects.CObject;
import RunLoop.CRun;
import Expressions.CValue;

public class PARAM_MULTIPLEVAR extends CParam
{
	public int flags;
	public int flagMasks;
	public int flagValues;
	public MultiVar values[];
    
    @Override
	public void load(CRunApp app) 
    {
		flags = app.file.readAInt();
		flagMasks = app.file.readAInt();
		flagValues = app.file.readAInt();
		int mask = 1;
		int nValues;
		for (nValues = 0; nValues < 4; nValues++) {
			if ((flags & mask) == 0)
				break;
			mask <<= 4;
		}
		values = new MultiVar[nValues];
		int maskGlobal = 2;
		int maskDouble = 4;
		int i;
		for (i = 0; i < nValues; i++) {
			MultiVar value = new MultiVar(app, (flags & maskGlobal) != 0, (flags & maskDouble) != 0);
			maskGlobal <<= 4;
			maskDouble <<= 4;
			values[i] = value;
		}
    }

	public boolean evaluate(CObject pHo)
	{
		if (pHo.rov == null)
			return false;

		// Test multiple flags
		if (flagMasks != 0) {
			if ((pHo.rov.rvValueFlags & flagMasks) != flagValues)
				return false;
		}

		// Test values
		int i;
		for (i = 0; i < values.length; i++) {
			MultiVar value = values[i];
			CValue v;
			if (value.global) {
				v = pHo.hoAdRunHeader.rhApp.getGlobalValueAt(value.index);
			}
			else {
				if (value.index>=pHo.rov.rvNumberOfValues)
				{
					if ( !pHo.rov.extendValues(value.index+10) )
						return false;
				}
				v = pHo.rov.getValue(value.index);
			}
			if (!CRun.compareTo(v, value.val, (short)value.op))
				return false;
		}
		return true;
	}

	public boolean evaluateNoGlobal(CObject pHo)
	{
		if (pHo.rov == null)
			return false;

		// Test multiple flags
		if (flagMasks != 0) {
			if ((pHo.rov.rvValueFlags & flagMasks) != flagValues)
				return false;
		}

		// Test values
		int i;
		for (i = 0; i < values.length; i++)
		{
			MultiVar value = values[i];
			CValue v;
			if (value.index>=pHo.rov.rvNumberOfValues)
			{
				if ( !pHo.rov.extendValues(value.index+10) )
					return false;
			}
			v = pHo.rov.getValue(value.index);
			if (!CRun.compareTo(v, value.val, (short)value.op))
				return false;
		}
		return true;
	}
}

