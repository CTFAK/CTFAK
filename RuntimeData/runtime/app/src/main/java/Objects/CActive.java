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
// CACTIVE : Objets actifs
//
//----------------------------------------------------------------------------------
package Objects;

import Banks.CImageBank;
import OI.CObjectCommon;
import RunLoop.CCreateObjectInfo;
import Runtime.Log;
import Sprites.CMask;
import Sprites.CSprite;

public class CActive extends CObject
{    
    public CActive() 
    {
    }

    @Override
	public void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
    {        
    }
    @Override
	public void handle()
    {
        ros.handle();
        if (roc.rcChanged)
        {
            roc.rcChanged=false;
            modif();
        }
    }
    @Override
	public void modif()
    {        
        ros.modifRoutine();
    }
    @Override
	public void display()
    {        
    }
    @Override
	public void kill(boolean bFast)
    {
    }
    @Override
	public void getZoneInfos()
    {
    }
    @Override
	public void draw()
    {
    }
    @Override
	public CMask getCollisionMask(int flags)
    {
	return null;
    }
    @Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
    {
    }
    @Override
	public CMask spriteGetMask()
    {
	return null;
    }
    @Override
	public void spriteKill (CSprite spr)
    {
    }
}
