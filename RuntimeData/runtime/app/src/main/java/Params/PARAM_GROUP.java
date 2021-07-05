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
// PARAM_GROUP groupe d'evenements
//
//----------------------------------------------------------------------------------
package Params;

import Application.CRunApp;

public class PARAM_GROUP extends CParam
{
    public short grpFlags;					// Active / Unactive?
    public short grpId;						// Group identifier
    public static final short GRPFLAGS_INACTIVE=0x0001;
    public static final short GRPFLAGS_CLOSED=0x0002;
    public static final short GRPFLAGS_PARENTINACTIVE=0x0004;
    public static final short GRPFLAGS_GROUPINACTIVE=0x0008;
    public static final short GRPFLAGS_GLOBAL=0x0010;
    
    @Override
	public void load(CRunApp app) 
    {
        grpFlags=app.file.readAShort();
        grpId=app.file.readAShort();
    }
}
