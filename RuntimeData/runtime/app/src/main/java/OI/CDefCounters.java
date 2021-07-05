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
// CDEFCOUNTERS : Donnes d'un objet score / vies / counter
//
//----------------------------------------------------------------------------------
package OI;

import Banks.IEnum;
import Services.CFile;

public class CDefCounters 
{
    // Display types
    public static final short CTA_HIDDEN=0;
    public static final short CTA_DIGITS=1;
    public static final short CTA_VBAR=2;
    public static final short CTA_HBAR=3;
    public static final short CTA_ANIM=4;
    public static final short CTA_TEXT=5;    
    public static final short BARFLAG_INVERSE=0x0100;
    
    public int odCx;					// Size: only lives & counters
    public int odCy;
    public short odPlayer;				// Player: only score & lives
    public short odDisplayType;			// CTA_xxx
    public short odDisplayFlags;			// BARFLAG_INVERSE
    public short odFont;					// Font
    public short ocBorderSize;			// Border
    public int ocBorderColor;
    public short ocShape;			// Shape
    public short ocFillType;
    public short ocLineFlags;			// Only for lines in non filled mode
    public int ocColor1;			// Gradient
    public int ocColor2;
    public int ocGradientFlags;
    public short nFrames;
    public short frames[]=null;
    
    public CDefCounters() 
    {
    }
    /** Loads the information from the application file.
     */
    public void load(CFile file)
    {
        file.skipBytes(4);          // size
        odCx=file.readAInt();
        odCy=file.readAInt();
        odPlayer=file.readAShort();
        odDisplayType=file.readAShort();
        odDisplayFlags=file.readAShort();
        odFont=file.readAShort();
        
        switch (odDisplayType)
        {
            case 0:             // CTA_HIDDEN
                break;
            case 1:             // CTA_DIGITS
            case 4:             // CTA_ANIM
                nFrames=file.readAShort();
                frames=new short[nFrames];
                int n;
                for (n=0; n<nFrames; n++)
                {
                    frames[n]=file.readAShort();
                }
                break;
            case 2:             // CTA_VBAR
            case 3:             // CTA_HBAR
            case 5:             // CTA_TEXT
                ocBorderSize=file.readAShort();
                ocBorderColor=file.readAColor();
                ocShape=file.readAShort();
                ocFillType=file.readAShort();
                if (ocShape==1)		// SHAPE_LINE
                {
                    ocLineFlags=file.readAShort();
                }
                else
                {
                    switch (ocFillType)
                    {
                        case 1:			    // FILLTYPE_SOLID
                            ocColor1=file.readAColor();
                            break;
                        case 2:			    // FILLTYPE_GRADIENT
                            ocColor1=file.readAColor();
                            ocColor2=file.readAColor();
                            ocGradientFlags=file.readAInt();
                            break;
                        case 3:			    // FILLTYPE_IMAGE
                            break;
                    }
                }
                break;
        }
    }
    /** Enumerates the images and fonts used in this counter.
     */
    public void enumElements(IEnum enumImages, IEnum enumFonts)
    {
	short num;
	switch(odDisplayType)
	{
            case 1:             // CTA_DIGITS
            case 4:             // CTA_ANIM
		int n;
		for (n=0; n<nFrames; n++)
		{
		    if (enumImages!=null)
		    {
			num=enumImages.enumerate(frames[n]); 
			if (num!=-1)
			{
			    frames[n]=num;
			}
		    }
		}
		break;
            case 5:             // CTA_TEXT
		if (enumFonts!=null)
		{
		    num=enumFonts.enumerate(odFont);
		    if (num!=-1)
		    {
			odFont=num;
		    }
		}
		break;
	}
    }
}
