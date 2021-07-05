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
// CFONTINFO : informations sur une fonte
//
//----------------------------------------------------------------------------------
package Services;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;

import Runtime.MMFRuntime;
import android.graphics.Typeface;


public class CFontInfo
{
    public int lfHeight = 0;
    public int lfWeight = 0;
    public byte lfItalic = 0;
    public byte lfUnderline = 0;
    public byte lfStrikeOut = 0;
    public String lfFaceName = null;
    public Typeface font = null;
    
    public CFontInfo () {
    }
    
    public void copy(CFontInfo f)
    {
        lfHeight = f.lfHeight;
        lfWeight = f.lfWeight;
        lfItalic = f.lfItalic;
        lfUnderline = f.lfUnderline;
        lfStrikeOut = f.lfStrikeOut;
        lfFaceName = f.lfFaceName;
    }
    public boolean equals(CFontInfo f)
    {
        if (f == null)
            return false;

        return  lfHeight == f.lfHeight &&
            lfWeight == f.lfWeight &&
            lfItalic == f.lfItalic &&
            lfUnderline == f.lfUnderline &&
            lfStrikeOut == f.lfStrikeOut &&
            lfFaceName.equals (f.lfFaceName);
    }
    public Typeface createFont()
    {
    	if(font != null)
    		font = null;
    	
    	if(font == null)
    	{
	    	String faceName=lfFaceName.trim().toLowerCase();
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
	        //**********************************
	        //
	        // let check first if is an internal
	        //             fonts  
	        //
	        //**********************************
	        if(faceName != null) {
	        	if(MMFRuntime.inst.fontUtils.isFontInternal(faceName)) {
	        		font = Typeface.createFromFile(MMFRuntime.inst.fontUtils.InternalFontPath(faceName));
	        		int a = font.getStyle();
	        	}
	        	else if(MMFRuntime.inst.fontUtils.packed && MMFRuntime.inst.fontUtils.isFontPacked(faceName)) {
	        		font = MMFRuntime.inst.fontUtils.loadFontFromAssets(faceName);
	        	}
	        	else 
		        	font = Typeface.create(faceName, style);	        		
	        }
	        else
	        	font = Typeface.create(faceName, style);
    	}
    	return font;
    }
    public void write(DataOutputStream stream) throws IOException
    {
        stream.writeInt(lfHeight);
        stream.writeInt(lfWeight);
        stream.writeByte(lfItalic);
        stream.writeByte(lfUnderline);
        stream.writeByte(lfStrikeOut);
        stream.writeUTF(lfFaceName);
    }
    public void read(DataInputStream stream) throws IOException
    {
        lfHeight=stream.readInt();
        lfWeight=stream.readInt();
        lfItalic=stream.readByte();
        lfUnderline=stream.readByte();
        lfStrikeOut=stream.readByte();
        lfFaceName=stream.readUTF();
    }
}
