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
// CMOVEDEFPLATFORM : donnees du mouvement platforme
//
//----------------------------------------------------------------------------------
package Movements;

import Services.CFile;

public class CMoveDefPlatform extends CMoveDef
{
    public short mpSpeed;
    public short mpAcc;	
    public short mpDec;	
    public short mpJumpControl;
    public short mpGravity;
    public short mpJump;
    
    public CMoveDefPlatform() 
    {
    }

    @Override
	public void load(CFile file, int length)
    {
        mpSpeed=file.readAShort();
        mpAcc=file.readAShort();	
        mpDec=file.readAShort();	
        mpJumpControl=file.readAShort();
        mpGravity=file.readAShort();
        mpJump=file.readAShort();        
    }
    
}
