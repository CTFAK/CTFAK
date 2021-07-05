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
package RunLoop;

import Movements.CRunMvtExtension;
import Objects.CObject;
import Services.CBinaryFile;
import Services.CRect;

import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.physics.box2d.Body;

public class CRunMBase extends CRunMvtExtension
{
    public static final int MTYPE_PLATFORM = 1;
    public static final int MTYPE_OBSTACLE = 2;
    public static final int MTYPE_OBJECT = 3;
    public static final int MTYPE_PARTICULE = 4;
    public static final int MTYPE_ELEMENT = 5;
    public static final int MTYPE_BORDERBOTTOM = 6;
    public static final int MTYPE_BORDERLEFT = 7;
    public static final int MTYPE_BORDERRIGHT = 8;
    public static final int MTYPE_BORDERTOP = 9;
    public static final int MTYPE_FAKEOBJECT = 10;
    public static final float ANGLE_MAGIC = 1.23456789f;
    public static final int MSUBTYPE_OBJECT = 0;
    public static final int MSUBTYPE_BOTTOM = 1;
    public static final int MSUBTYPE_TOP = 2;
    public static final int MSUBTYPE_LEFT = 3;
    public static final int MSUBTYPE_RIGHT = 4;

    public CObject m_pHo = null;
    public Body m_body = null;
    public float m_angle = 0;
    public float m_addVX = 0;
    public float m_addVY = 0;
    public Boolean m_addVFlag = false;
    public int m_identifier = 0;
    public Boolean m_stopFlag = false;
    public double m_currentAngle = 0;
    public int m_eventCount = 0;
    public short m_image = 0;
    public Boolean stopped = false;
    public float m_setVX = 0;
    public float m_setVY = 0;
    public Boolean m_setVFlag = false;
    public Boolean m_platform = false;
    public int x = 0;
    public int y = 0;
    public CRect rc = new CRect();
    public int m_type = 0;
    public int m_subType = MSUBTYPE_OBJECT;
    public CRunMBase m_collidingObject;
    public boolean m_background = false;
    
    public float CRunFactor = 0.333f;
    
    public void InitBase(CObject pHo, int type)
    {
        m_pHo = pHo;
        this.m_stopFlag=false;
        m_currentAngle=0;
        this.m_type = type;
        
    }
    public Boolean CreateBody(){return false;}
    public void CreateJoint(){}
    public void setAngle(float angle){}
    public float getAngle(){return 0;}

    public void SetCollidingObject(CRunMBase object)
    {
        m_collidingObject = object;
    }
    public void PrepareCondition()
    {
        this.m_stopFlag=false;
        this.m_eventCount=this.m_pHo.hoAdRunHeader.rh4EventCount;
    }

    public Boolean IsStop()
    {
        return this.m_stopFlag;
    }

    public void SetStopFlag(Boolean flag)
    {
        this.m_stopFlag=flag;
    }

    public void SetVelocity(float vx, float vy)
    {
        if (!m_platform)
        {
            float angle=m_body.getAngle();
            Vector2 position=m_body.getPosition();
            position.x+=vx/2.56f;
            position.y+=vy/2.56f;
            m_body.setTransform(position, angle);
        }
        else
        {
            m_setVX=vx*22.5f;
            m_setVY=vy*22.5f;
            m_setVFlag=true;
        }
    }

    public void AddVelocity(float vx, float vy)
    {
        m_addVX=vx;
        m_addVY=vy;
        m_addVFlag=true;
    }

    public void ResetAddVelocity()
    {
        if (m_addVFlag)
        {
            m_addVFlag=false;
            m_addVX=0;
            m_addVY=0;
        }
        if (m_setVFlag)
        {
            m_setVFlag=false;
            m_setVX=0;
            m_setVY=0;
        }
    }
    public void SetDensity(int density){}
    public void SetFriction(int friction){}
    public void SetRestitution(int restitution){}
    public void SetGravity(int gravity){}

    @Override
    public void initialize(CBinaryFile file){}
    @Override
    public void kill(){}
    @Override
    public boolean move(){return false;}
    @Override
    public void setPosition(int x, int y){}
    @Override
    public void setXPosition(int x){}
    @Override
    public void setYPosition(int y){}
    @Override
    public void stop(boolean bCurrent){}
    @Override
    public void bounce(boolean bCurrent){}
    @Override
    public void reverse(){}
    @Override
    public void start(){}
    @Override
    public void setSpeed(int speed){}
    @Override
    public void setMaxSpeed(int speed){}
    @Override
    public void setDir(int dir){}
    @Override
    public void setAcc(int acc){}
    @Override
    public void setDec(int dec){}
    @Override
    public void setRotSpeed(int speed){}
    @Override
    public void set8Dirs(int dirs){}
    @Override
    public void setGravity(int gravity){}
    @Override
    public int extension(int function, int param){return 0;}
    @Override
    public double actionEntry(int action){return 0;}
    @Override
    public int getSpeed(){return 0;}
    @Override
    public int getAcceleration(){return 0;}
    @Override
    public int getDeceleration(){return 0;}
    @Override
    public int getGravity(){return 0;}
}
