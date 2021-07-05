package RunLoop;

import Actions.CActExtension;
import Conditions.CCndExtension;
import Expressions.CValue;
import Extensions.CRunBox2DBasePosAndAngle;
import Extensions.CRunExtension;
import Objects.CObject;
import Services.CBinaryFile;
import Services.CFontInfo;
import Services.CRect;
import Sprites.CMask;

import com.badlogic.gdx.physics.box2d.Body;

public class CRunBaseParent extends CRunExtension
{
    public int identifier;
    public float RunFactor = 0.2830f;

    @Override
    public int getNumberOfConditions()
    {
        return 0;
    }

    @Override
    public boolean createRunObject(CBinaryFile file, CCreateObjectInfo cob, int version)
    {
        return false;
    }

    @Override
    public int handleRunObject()
    {
        return REFLAG_ONESHOT;
    }

    @Override
    public void displayRunObject()
    {
    }

    @Override
    public void reinitDisplay ()
    {
    }

    @Override
    public void destroyRunObject(boolean bFast)
    {
    }

    @Override
    public void pauseRunObject()
    {
    }

    @Override
    public void continueRunObject()
    {
    }

    @Override
    public void getZoneInfos()
    {
    }

    @Override
    public boolean condition(int num, CCndExtension cnd)
    {
        return false;
    }

    @Override
    public void action(int num, CActExtension act)
    {
    }

    @Override
    public CValue expression(int num)
    {
        return new CValue(0);
    }

    @Override
    public CMask getRunObjectCollisionMask(int flags)
    {
        return null;
    }

    @Override
    public CFontInfo getRunObjectFont()
    {
        return null;
    }

    @Override
    public void setRunObjectFont(CFontInfo fi, CRect rc)
    {
    }

    @Override
    public int getRunObjectTextColor()
    {
        return 0;
    }

    @Override
    public void setRunObjectTextColor(int rgb)
    {
    }

    public boolean rStartObject()
    {
        return false;
    }

    public void rAddNormalObject(CObject pHo)
    {
    }

    public void rAddObject(CRunMBase mBase)
    {
    }

    public void rRemoveObject(CRunMBase mBase)
    {
    }
    public void AddPhysicsAttractor(CObject pHo)
    {
    }
    
    public Body rCreateBullet(float angle, float speed, CRunMBase pMBase)
    {
        return null;
    }
    
    public void rDestroyBody(Body body)
    {
    }
    
    public void rGetBodyPosition(Body pBody, CRunBox2DBasePosAndAngle o)
    {
    }

    public Body rAddABackdrop(int x, int y, short img, short colType)
    {
        return null;
    }

    public void rSubABackdrop(Body body)
    {

    }

    public boolean isPaused()
    {
    	return false;
    }

}
