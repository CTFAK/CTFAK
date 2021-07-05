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
// CDEFTEXTS : liste de textes
//
//----------------------------------------------------------------------------------
package OI;

import Banks.IEnum;
import Services.CFile;

public class CDefTexts extends CDefObject
{
    public int otCx;
    public int otCy;
    public int otNumberOfText;
    public CDefText otTexts[];
    
    public CDefTexts() 
    {
    }
    @Override
	public void load(CFile file)
    {
        long debut=file.getFilePointer();
        file.skipBytes(4);          // Size
        otCx=file.readAInt();
        otCy=file.readAInt();
        otNumberOfText=file.readAInt();
        
        otTexts=new CDefText[otNumberOfText];
        int offsets[]=new int[otNumberOfText];
        int n;
        for (n=0; n<otNumberOfText; n++)
        {
            offsets[n]=file.readAInt();
        }
        for (n=0; n<otNumberOfText; n++)
        {
            otTexts[n]=new CDefText();
            file.seek(debut+offsets[n]);
            otTexts[n].load(file);
        }
    }
    @Override
	public void enumElements(IEnum enumImages, IEnum enumFonts)
    {	
		int n;
		for (n=0; n<otNumberOfText; n++)
		{
		    otTexts[n].enumElements(enumImages, enumFonts);
		}
    }
}
