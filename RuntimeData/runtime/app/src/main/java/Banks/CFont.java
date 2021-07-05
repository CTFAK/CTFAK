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
// CFONT : une fonte
//
//----------------------------------------------------------------------------------
package Banks;

import Runtime.Log;
import Services.CFile;
import Services.CFontInfo;
import android.graphics.Typeface;

public class CFont
{
    public short useCount = 0;
    public short handle = 0;
    public int lfHeight = 0;
    public int lfWidth = 0;
    public int lfWeight = 0;
    public byte lfItalic = 0;
    public byte lfUnderline = 0;
    public byte lfStrikeOut = 0;
    public String lfFaceName = null;
    public Typeface font = null;

    public CFont()
    {
    }

    public void loadHandle(CFile file)
    {
        handle = (short) file.readAInt();
        if (file.unicode()==false)
        {
            file.skipBytes(0x48);
        }
        else
        {
            file.skipBytes(0x68);
        }
    }

    public void load(CFile file)
    {
        handle = (short) file.readAInt();
        file.skipBytes(12);		    // Trois DWORD d'entete

        long debut = file.getFilePointer();
        lfHeight = file.readAInt();
        if (lfHeight < 0)
        {
            lfHeight = -lfHeight;
        }
        lfWidth = file.readAInt();
        file.readAInt();
        file.readAInt();
        lfWeight = file.readAInt();
        lfItalic = file.readByte();
        lfUnderline = file.readByte();
        lfStrikeOut = file.readByte();
        file.readByte();
        file.readByte();
        file.readByte();
        file.readByte();
        file.readByte();
        lfFaceName = file.readAString();

        // Positionne a la fin
        if (file.unicode()==false)
        {
            file.seek(debut+0x3C);
        }
        else
        {
            file.seek(debut+0x5C);
        }
        font = null;
    }

    public Typeface createFont()
    {
        if (font == null)
        {
        	Log.Log("Creating font...");
        	
        	String faceName=lfFaceName;
            if ((lfFaceName.compareToIgnoreCase("Arial") == 0) || (lfFaceName.compareToIgnoreCase("Tahoma") == 0))
            {
            	faceName=null;
            }
            int style=Typeface.NORMAL;
            if (lfItalic!=0 && lfWeight>=500)
            {
            	style=Typeface.BOLD_ITALIC;
            }
            else 
            {
            	if (lfItalic!=0)
    	        {
    	        	style=Typeface.ITALIC;
    	        }
            	if (lfWeight>=500)
            	{
            		style=Typeface.BOLD;
            	}
            }
            font=Typeface.create(faceName, style);
        }
        return font;   
    }

    public CFontInfo getFontInfo()
    {
        CFontInfo info = new CFontInfo();
        info.lfHeight = lfHeight;
        info.lfWeight = lfWeight;
        info.lfItalic = lfItalic;
        info.lfUnderline = lfUnderline;
        info.lfStrikeOut = lfStrikeOut;
        info.lfFaceName = new String(lfFaceName);
        return info;
    }

    public static CFont createFromFontInfo(CFontInfo info)
    {
        CFont font = new CFont();
        font.lfHeight = info.lfHeight;
        font.lfWeight = info.lfWeight;
        font.lfItalic = info.lfItalic;
        font.lfUnderline = info.lfUnderline;
        font.lfStrikeOut = info.lfStrikeOut;
        font.lfFaceName = info.lfFaceName;
        return font;
    }

    public void createDefaultFont()
    {
        lfHeight = 12;
        lfWeight = 400;
        lfItalic = 0;
        lfUnderline = 0;
        lfStrikeOut = 0;
        lfFaceName = "Arial";
    }
}
