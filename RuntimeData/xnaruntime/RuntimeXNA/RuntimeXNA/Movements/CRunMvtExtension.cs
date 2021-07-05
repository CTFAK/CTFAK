//----------------------------------------------------------------------------------
//
// CMOVEEXTENSION : classe abstraite de mouvement extension
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Movements
{
	
	public abstract class CRunMvtExtension
	{
        public CObject ho;
        public CRun rh;

		
		public CRunMvtExtension()
		{
		}
		
		public virtual void init(CObject hoPtr)
		{
			ho = hoPtr;
			rh = ho.hoAdRunHeader;
		}

        public virtual void initialize(CFile file)
        {
        }
        public virtual void kill()
        {
        }
        public virtual bool move()
        {
            return false;
        }
        public virtual void stop(bool bCurrent)
        {
        }
        public virtual void bounce(bool bCurrent)
        {
        }
        public virtual void reverse()
        {
        }
        public virtual void start()
        {
        }
        public virtual int extension(int function, int param)
        {
            return 0;
        }
        public virtual double actionEntry(int action)
        {
            return 0;
        }
        public virtual void setPosition(int x, int y)
        {
        }
        public virtual void setXPosition(int value)
        { 
        }

		public virtual void setYPosition(int Values)
        { 
        }
		public virtual void setSpeed(int value)
        { 
        }
		public virtual int getSpeed()
        { 
            return 0;
        }
		public virtual void setMaxSpeed(int speed)
        {
        }
        public virtual void setDir(int dir)
        {
        }
        public virtual void set8Dirs(int dirs)
        {
        }
        public virtual void setAcc(int acc)
        {
        }
        public virtual void setDec(int dec)
        {
        }
        public virtual void setRotSpeed(int speed)
        {
        }
        public virtual void setGravity(int gravity)
        {
        }
        public virtual int getGravity()
        {
            return 0;
        }
        public virtual void setAcceleration(int acc)
        {
        }
        public virtual int getAcceleration()
        {
            return 0;
        }
        public virtual void setDeceleration(int dec)
        {
        }
        public virtual int getDeceleration()
        {
            return 0;
        }
		
		// Callback routines
		// -------------------------------------------------------------------------
        public double getParamDouble()
        {
            CMoveExtension mvt = (CMoveExtension)ho.rom.rmMovement;
            return mvt.callParam;
        }
        public virtual int dirAtStart(int dir)
		{
			return ho.rom.dirAtStart(ho, dir, 32);
		}
		public virtual void  animations(int anm)
		{
			ho.roc.rcAnim = anm;
			if (ho.roa != null)
			{
				ho.roa.animate();
			}
		}
		public virtual void  collisions()
		{
			ho.hoAdRunHeader.rh3CollisionCount++;
			ho.rom.rmMovement.rmCollisionCount = ho.hoAdRunHeader.rh3CollisionCount;
			ho.hoAdRunHeader.newHandle_Collisions(ho);
		}
		public virtual bool approachObject(int destX, int destY, int originX, int originY, int htFoot, int planCol, CPoint ptDest)
		{
			destX -= ho.hoAdRunHeader.rhWindowX;
			destY -= ho.hoAdRunHeader.rhWindowY;
			originX -= ho.hoAdRunHeader.rhWindowX;
			originY -= ho.hoAdRunHeader.rhWindowY;
			bool bRet = ho.rom.rmMovement.mpApproachSprite(destX, destY, originX, originY, (short) htFoot, (short) planCol, ptDest);
			ptDest.x += ho.hoAdRunHeader.rhWindowX;
			ptDest.y += ho.hoAdRunHeader.rhWindowY;
			return bRet;
		}
		public virtual bool moveIt()
		{
			if (ho.rom.rmMovement.newMake_Move(ho.roc.rcSpeed, ho.roc.rcDir))
			{
				return true;
			}
			return false;
		}
		public virtual bool testPosition(int x, int y, int htFoot, int planCol, bool flag)
		{
			return ho.rom.rmMovement.tst_SpritePosition(x, y, (short) htFoot, (short) planCol, flag);
		}
		public virtual byte getJoystick(int player)
		{
			return ho.hoAdRunHeader.rhPlayer[player];
		}
		public virtual bool colMaskTestRect(int x, int y, int sx, int sy, int layer, int plan)
		{
			if (ho.hoAdRunHeader.rhFrame.bkdCol_TestRect(x - ho.hoAdRunHeader.rhWindowX, y - ho.hoAdRunHeader.rhWindowY, sx, sy, layer, plan))
			{
				return false;
			}
			return true;
		}
		public virtual bool colMaskTestPoint(int x, int y, int layer, int plan)
		{
			if (ho.hoAdRunHeader.rhFrame.bkdCol_TestPoint(x - ho.hoAdRunHeader.rhWindowX, y - ho.hoAdRunHeader.rhWindowY, layer, plan))
			{
				return false;
			}
			return true;
		}
	}
}