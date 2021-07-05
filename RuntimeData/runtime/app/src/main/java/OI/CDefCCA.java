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
// CDEFCCA : definitions objet CCA
//
//----------------------------------------------------------------------------------
package OI;

import Banks.IEnum;
import Services.CFile;

public class CDefCCA extends CDefObject
{
    public int odCx;						// Size (ignored)
    public int odCy;
    public short odVersion;					// 0
    public short odNStartFrame;
    public int odOptions;					// Options
    public String odName;
    
    public CDefCCA() 
    {
    }

    @Override
	public void load(CFile file)
    {
        file.skipBytes(4);
        odCx=file.readAInt();
        odCy=file.readAInt();
        odVersion=file.readAShort();
        odNStartFrame=file.readAShort();
        odOptions=file.readAInt();
        file.skipBytes(4+4);                  // odFree+pad bytes
        odName=file.readAString();
    }
    @Override
	public void enumElements(IEnum enumImages, IEnum enumFonts)
    {
    }	
}
