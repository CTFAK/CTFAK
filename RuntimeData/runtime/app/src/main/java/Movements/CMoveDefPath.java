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
// CMOVEDEFPATH : donnees du mouvement path
//
//----------------------------------------------------------------------------------
package Movements;

import Services.CFile;

public class CMoveDefPath extends CMoveDef
{
    public short mtNumber;				// Number of movement 
    public short mtMinSpeed; 			// maxs and min speed in the movements 
    public short mtMaxSpeed;
    public byte mtLoop;					// Loop at end
    public byte mtRepos;				// Reposition at end
    public byte mtReverse;				// Pingpong?
    public CPathStep steps[];
    
    public CMoveDefPath() 
    {
    }
    
    @Override
	public void load(CFile file, int length) 
    {
        mtNumber=file.readAShort();
        mtMinSpeed=file.readAShort();
        mtMaxSpeed=file.readAShort();
        mtLoop=file.readByte();	
        mtRepos=file.readByte();
        mtReverse=file.readByte();
        file.skipBytes(1);

        steps=new CPathStep[mtNumber];
        int n, next;
        long debut;
        for (n=0; n<mtNumber; n++)
        {
            debut=file.getFilePointer();
            steps[n]=new CPathStep();
            file.readUnsignedByte();		// prev
            next=file.readUnsignedByte();
            steps[n].load(file);
            file.seek(debut+next);
        }
    }
    
}
