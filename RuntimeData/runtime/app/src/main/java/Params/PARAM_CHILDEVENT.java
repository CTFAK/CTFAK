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
// PARAM_CHILDEVENT
//
//----------------------------------------------------------------------------------
package Params;

import java.util.ArrayList;
import Application.CRunApp;

public class PARAM_CHILDEVENT extends CParam
{
    public int evgOffsetList;
    public short ois[];
    
    @Override
	public void load(CRunApp app) 
    {
        int nOIs = app.file.readAInt();
		evgOffsetList = 0;
		ois = new short[nOIs*2];
		for(int i=0; i<nOIs*2; i++) {
			ois[i] = app.file.readAShort();
		}
        app.file.skipBytes(4);
    }
}

