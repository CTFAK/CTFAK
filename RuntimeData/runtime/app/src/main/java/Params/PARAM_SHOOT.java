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
// PARAM_SHOOT : creation d'objets
//
//----------------------------------------------------------------------------------
package Params;

import Application.CRunApp;

public class PARAM_SHOOT extends CCreate
{
    public short shtSpeed;		// Speed
    
    @Override
	public void load(CRunApp app) 
    {
        posOINUMParent=app.file.readAShort();
        posFlags=app.file.readAShort();
        posX=app.file.readAShort();
        posY=app.file.readAShort();
        posSlope=app.file.readAShort();
        posAngle=app.file.readAShort();
        posDir=app.file.readAInt();
        posTypeParent=app.file.readAShort();
        posOiList=app.file.readAShort();
        posLayer=app.file.readAShort();
        cdpHFII=app.file.readAShort();
        cdpOi=app.file.readAShort();
	app.file.skipBytes(4);		//cdpFree
        shtSpeed=app.file.readAShort();
    }
}
