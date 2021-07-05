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
// CMOVEEXTENSION : classe abstraite de mouvement extension
//
//----------------------------------------------------------------------------------
package Movements;

import Objects.CObject;
import RunLoop.CRun;
import Services.CBinaryFile;
import Services.CPoint;

public abstract class CRunMvtExtension
{    
	public CObject ho;
	public CRun rh;

	public CRunMvtExtension() 
	{
	}

	public void init(CObject hoPtr)
	{
		ho=hoPtr;
		rh=ho.hoAdRunHeader;
	}
	public abstract void initialize(CBinaryFile file);
	public abstract void kill();
	public abstract boolean move();
	public abstract void setPosition(int x, int y);
	public abstract void setXPosition(int x);
	public abstract void setYPosition(int y);
	public abstract void stop(boolean bCurrent);
	public abstract void bounce(boolean bCurrent);
	public abstract void reverse();
	public abstract void start();
	public abstract void setSpeed(int speed);
	public abstract void setMaxSpeed(int speed);
	public abstract void setDir(int dir);
	public abstract void setAcc(int acc);
	public abstract void setDec(int dec);
	public abstract void setRotSpeed(int speed);
	public abstract void set8Dirs(int dirs);
	public abstract void setGravity(int gravity);
	public abstract int extension(int function, int param);
	public abstract double actionEntry(int action);
	public abstract int getSpeed();
	public abstract int getAcceleration();
	public abstract int getDeceleration();
	public abstract int getGravity();
	public int getDir()
	{
		return ho.roc.rcDir;
	}

	public boolean stopped ()
	{
		return ho.roc.rcSpeed == 0;
	}

	// Callback routines
	// -------------------------------------------------------------------------
	public int dirAtStart(int dir)
	{
		return ho.rom.dirAtStart(ho, dir, 32);
	}
	public void animations(int anm)
	{
		ho.hoMark1 = 0;
		ho.roc.rcAnim=anm;
		if (ho.roa!=null)
			ho.roa.animate();
	}
	public void collisions()
	{
		ho.hoAdRunHeader.rh3CollisionCount++;	
		ho.rom.rmMovement.rmCollisionCount=ho.hoAdRunHeader.rh3CollisionCount;
		ho.hoAdRunHeader.newHandle_Collisions(ho);
	}
	public boolean approachObject(int destX, int destY, int originX, int originY, int htFoot, int planCol, CPoint ptDest)
	{
		destX-=ho.hoAdRunHeader.rhWindowX;
		destY-=ho.hoAdRunHeader.rhWindowY;
		originX-=ho.hoAdRunHeader.rhWindowX;
		originY-=ho.hoAdRunHeader.rhWindowY;
		boolean bRet=ho.rom.rmMovement.mpApproachSprite(destX, destY, originX, originY, (short)htFoot, (short)planCol, ptDest);
		ptDest.x+=ho.hoAdRunHeader.rhWindowX;
		ptDest.y+=ho.hoAdRunHeader.rhWindowY;
		return bRet;	    
	}	    
	public boolean moveIt()
	{
		if (ho.rom.rmMovement.newMake_Move(ho.roc.rcSpeed, ho.roc.rcDir))
		{
			return true;
		}
		return false;
	}
	public boolean testPosition(int x, int y, int htFoot, int planCol, boolean flag)
	{
		return ho.rom.rmMovement.tst_SpritePosition(x, y, (short)htFoot, (short)planCol, flag);
	}    
	public byte getJoystick(int player)
	{
		return ho.hoAdRunHeader.rhPlayer[player];
	}
	public boolean colMaskTestRect(int x, int y, int sx, int sy, int layer, int plan)
	{
		if (ho.hoAdRunHeader.rhFrame.bkdCol_TestRect(x-ho.hoAdRunHeader.rhWindowX, y-ho.hoAdRunHeader.rhWindowY, sx, sy, layer, plan))
		{
			return false;
		}
		return true;
	}
	public boolean colMaskTestPoint(int x, int y, int layer, int plan)
	{
		if (ho.hoAdRunHeader.rhFrame.bkdCol_TestPoint(x-ho.hoAdRunHeader.rhWindowX, y-ho.hoAdRunHeader.rhWindowY, layer, plan))
		{
			return false;
		}
		return true;
	}
	public double getParamDouble()
	{
		CMoveExtension mvt=(CMoveExtension)ho.rom.rmMovement;
		return mvt.callParam1;
	}
	public double getParam1()
	{
		CMoveExtension mvt=(CMoveExtension)ho.rom.rmMovement;
		return mvt.callParam1;
	}
	public double getParam2()
	{
		CMoveExtension mvt=(CMoveExtension)ho.rom.rmMovement;
		return mvt.callParam2;
	}

	public int AngleToDir(double d) {
		int nDir=(int)Math.round((d%360)/11.25);
		nDir += (nDir<0 ? 32: 0);
		return nDir;
	}

}
