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
// PARAM_PROGRAM un programme a faire fonctionner
//
//----------------------------------------------------------------------------------
package Params;

import Application.CRunApp;

public class PARAM_PROGRAM extends CParam
{
    public short flags;
    public String filename;
    public String command;
    public static final short PRGFLAGS_WAIT=0x0001;
    public static final short PRGFLAGS_HIDE=0x0002;
            
    @Override
	public void load(CRunApp app)
    {
        flags=app.file.readAShort();
        long debut=app.file.getFilePointer();
        filename=app.file.readAString();
        app.file.seek(debut+260);           // _MAX_PATH
        command=app.file.readAString();
    }
}
