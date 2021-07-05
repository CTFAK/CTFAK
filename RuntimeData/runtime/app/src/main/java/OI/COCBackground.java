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
// COCBACKGROUND : un objet dcor normal
//
//----------------------------------------------------------------------------------
package OI;

import Banks.CImageBank;
import Banks.IEnum;
import Services.CFile;
import Sprites.CMask;
import Sprites.CSprite;

public class COCBackground extends COC
{
    public short ocImage;			// Image
    
    public COCBackground() 
    {
    }
    
    @Override
	public void load(CFile file, short type, COI pOi)
    {
		file.skipBytes(4);		// ocDWSize
		ocObstacleType=file.readAShort();
		ocColMode=file.readAShort();
		ocCx=file.readAInt();
		ocCy=file.readAInt();
		ocImage=file.readAShort();
    }
    
    @Override
	public void enumElements(IEnum enumImages, IEnum enumFonts)
    {
		if (enumImages!=null)
		{
		    short num=enumImages.enumerate(ocImage);
		    if (num!=-1)
		    {
		    	ocImage=num;
		    }
		}
    }

    @Override
	public CMask spriteGetMask()
    {
        return null;
    }

    @Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
    {
    }

}
