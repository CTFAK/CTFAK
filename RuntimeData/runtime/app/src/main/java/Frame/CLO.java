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
// CLOLIST : liste de levelobjects
//
//----------------------------------------------------------------------------------
package Frame;

import Services.CFile;
import Sprites.CSprite;

public class CLO 
{
    public static final short PARENT_NONE=0;
    public static final short PARENT_FRAME=1;
    public static final short PARENT_FRAMEITEM=2;
    public static final short PARENT_QUALIFIER=3;
    
    public short loHandle;			// Le handle
    public short loOiHandle;			// HOI
    public int loX;				// Coords
    public int loY;
    public short loParentType;			// Parent type
    public short loOiParentHandle;		// HOI Parent
    public short loLayer;			// Layer
    public short loType;
    public CSprite loSpr[];			// Sprite handles for backdrop objects from layers > 1
   
    public CLO() 
    {
		loSpr=new CSprite[4];
		int i;
		for (i=0; i<4; i++)
		{
		    loSpr[i]=null;
		}
    }
    public void load(CFile file)
    {
		loHandle=file.readAShort();
		loOiHandle=file.readAShort();
		loX=file.readAInt();
		loY=file.readAInt();
		loParentType=file.readAShort();
		loOiParentHandle=file.readAShort();
		loLayer=file.readAShort();
		file.skipBytes(2);
    }
}
