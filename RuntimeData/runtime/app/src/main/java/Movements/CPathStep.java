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
// CPATHSTEP : un pas de mouvement path
//
//----------------------------------------------------------------------------------
package Movements;

import Services.CFile;

public class CPathStep 
{
//  public byte mdPrevious;
//  public byte mdNext;
    public byte mdSpeed;
    public byte mdDir;
    public short mdDx;
    public short mdDy;
    public short mdCosinus;
    public short mdSinus;
    public short mdLength;
    public short mdPause;
    public String mdName=null;
    
    public CPathStep() 
    {
    }

    public void load(CFile file) 
    {
        mdSpeed=file.readByte();
        mdDir=file.readByte();
        mdDx=file.readAShort();
        mdDy=file.readAShort();
        mdCosinus=file.readAShort();
        mdSinus=file.readAShort();
        mdLength=file.readAShort();
        mdPause=file.readAShort();
        String name=file.readAString();
        if (name.length()>0)
            mdName=name;
    }       

}
