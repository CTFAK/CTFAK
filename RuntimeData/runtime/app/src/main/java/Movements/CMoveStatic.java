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
// CMOVESTATIC : Mouvement statique
//
//----------------------------------------------------------------------------------
package Movements;

import Objects.CObject;

public class CMoveStatic extends CMove
{
    @Override
	public void init(CObject ho, CMoveDef mvPtr)
    {
        hoPtr = ho;
        hoPtr.roc.rcSpeed = 0;
        hoPtr.roc.rcCheckCollides = true;			//; Force la detection de collision
        hoPtr.roc.rcChanged = true;
    }

    @Override
	public void kill()
    {
    }

    @Override
	public void move()
    {
        if (hoPtr.roa != null)
        {
            if (hoPtr.roa.animate())
            {
				//return; Removed in build 291, see bug #4750
            }
        }
        if (hoPtr.roc.rcCheckCollides)			//; Faut-il tester les collisions?
        {
            hoPtr.roc.rcCheckCollides = false;		//; Va tester une fois!
            hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
        }
    }

    @Override
	public void stop()
    {
    }

    @Override
	public void start()
    {
    }

    @Override
	public void bounce()
    {
    }

    @Override
	public void setSpeed(int speed)
    {
    }

    @Override
	public void setMaxSpeed(int speed)
    {
    }

    @Override
	public void reverse()
    {
    }

    @Override
	public void setXPosition(int x)
    {
        if (hoPtr.hoX != x)
        {
            hoPtr.hoX = x;
            hoPtr.rom.rmMoveFlag = true;
            hoPtr.roc.rcChanged = true;
        }
        hoPtr.roc.rcCheckCollides = true;					//; Force la detection de collision
    }

    @Override
	public void setYPosition(int y)
    {
        if (hoPtr.hoY != y)
        {
            hoPtr.hoY = y;
            hoPtr.rom.rmMoveFlag = true;
            hoPtr.roc.rcChanged = true;
        }
        hoPtr.roc.rcCheckCollides = true;					//; Force la detection de collision
    }

    @Override
	public void setDir(int dir)
    {
    }
}
