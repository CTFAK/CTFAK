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
// CDEFTEXT : un element de texte
//
//----------------------------------------------------------------------------------
package OI;

import Banks.IEnum;
import Services.CFile;

public class CDefText 
{
    public short tsFont;					// Font 
    public short tsFlags;				// Flags
    public int tsColor;				// Color
    public String tsText;

    public static final short TSF_LEFT=0x0000;
    public static final short TSF_HCENTER=0x0001;
    public static final short TSF_RIGHT=0x0002;
    public static final short TSF_VCENTER=0x0004;
    public static final short TSF_HALIGN=0x000F;
    public static final short TSF_CORRECT=0x0100;
    public static final short TSF_RELIEF=0x0200;
    
    public CDefText() 
    {
    }
    public void load(CFile file)
    {
        tsFont=file.readAShort();
        tsFlags=file.readAShort();
        tsColor=file.readAColor();
        tsText=file.readAString();
    }
    public void enumElements(IEnum enumImages, IEnum enumFonts)
    {
	if (enumFonts!=null)
	{
	    short num=enumFonts.enumerate(tsFont);
	    if (num!=-1)
	    {
		tsFont=num;
	    }
	}
    }    
}
