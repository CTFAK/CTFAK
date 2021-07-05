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
// CMOVEEXTENSIOn : Mouvement extension
//
//----------------------------------------------------------------------------------
package Movements;

import Objects.CObject;
import Services.CBinaryFile;

public class CMoveExtension extends CMove
{   
    public CRunMvtExtension movement;
    public double callParam1=0;
    public double callParam2=0;

    public CMoveExtension(CRunMvtExtension m) 
    {
    	movement=m;
    }
    
    @Override
	public void init(CObject ho, CMoveDef mvPtr)
    {
        hoPtr=ho;
	
		CMoveDefExtension mdExt=(CMoveDefExtension)mvPtr;
		CBinaryFile file=new CBinaryFile(mdExt.data, ho.hoAdRunHeader.rhApp.bUnicode);
		movement.initialize(file);
		hoPtr.roc.rcCheckCollides=true;			//; Force la detection de collision
		hoPtr.roc.rcChanged=true;
    }
    @Override
	public void kill()
    {
        movement.kill();
    }
    @Override
	public void move()
    {
    	// 285.2 needs to be OR since you could have NO movement but a previous request for change.
    	if(movement.move())
    	{
    		hoPtr.roc.rcChanged = true;   		
    	}
    }
    @Override
	public void stop()
    {
		movement.stop(rmCollisionCount==hoPtr.hoAdRunHeader.rh3CollisionCount);	    // Sprite courant?
    }
    @Override
	public void start()
    {
        movement.start();
    }
    @Override
	public void bounce()
    {
        movement.bounce(rmCollisionCount==hoPtr.hoAdRunHeader.rh3CollisionCount);    // Sprite courant?
    }
    @Override
	public void setSpeed(int speed)
    {
        movement.setSpeed(speed);
    }
    @Override
	public void setMaxSpeed(int speed)
    {
        movement.setMaxSpeed(speed);
    }
    @Override
	public void reverse()
    {        
		movement.reverse();
    }
    @Override
	public void setXPosition(int x)
    {        
		movement.setXPosition(x);
		hoPtr.roc.rcChanged=true;
		hoPtr.roc.rcCheckCollides=true;
    }
    @Override
	public void setYPosition(int y)
    {
		movement.setYPosition(y);
		hoPtr.roc.rcChanged=true;
		hoPtr.roc.rcCheckCollides=true;
    }
    @Override
	public void setDir(int dir)
    {
		movement.setDir(dir);
		hoPtr.roc.rcChanged=true;
		hoPtr.roc.rcCheckCollides=true;	
    }
    public double callMovement(int function, double param)
    {
		callParam1=param;
		return movement.actionEntry(function);
    }
    public double callMovement2(int function, double param1, double param2)
    {
        callParam1=param1;
        callParam2=param2;
        return movement.actionEntry(function);
    }
    public boolean stopped()
    {
        return movement.stopped();
    }
}

