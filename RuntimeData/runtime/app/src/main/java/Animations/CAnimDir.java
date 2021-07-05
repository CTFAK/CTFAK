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
// CANIMDIR : Une direction d'animation
//
//----------------------------------------------------------------------------------
package Animations;

import Banks.IEnum;
import Services.CFile;

public class CAnimDir 
{
    public byte adMinSpeed;					// Minimum speed
    public byte adMaxSpeed;					// Maximum speed
    public short adRepeat;					// Number of loops
    public short adRepeatFrame;				// Where to loop
    public short adNumberOfFrame;			// Number of frames
    public short adFrames[];
    
    public CAnimDir() 
    {
    }

    public void load(CFile file) 
    {
        adMinSpeed=file.readByte();
        adMaxSpeed=file.readByte();
        adRepeat=file.readAShort();
        adRepeatFrame=file.readAShort();
        adNumberOfFrame=file.readAShort();
    
        adFrames=new short[adNumberOfFrame];
        int n;
        for (n=0; n<adNumberOfFrame; n++)
        {
            adFrames[n]=file.readAShort();
        }
    }

    public void enumElements(IEnum enumImages)
    {
        int n;
        for (n=0; n<adNumberOfFrame; n++)
        {
		    if (enumImages!=null)
		    {
				short num=enumImages.enumerate(adFrames[n]);
				if (num!=-1)
				{
				    adFrames[n]=num;
				}
		    }
        }
    }
}
