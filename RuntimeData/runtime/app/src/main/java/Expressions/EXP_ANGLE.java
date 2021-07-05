package Expressions;

import RunLoop.CRun;


public class EXP_ANGLE extends CExp
{
    public void evaluate(CRun rhPtr)
    {
        rhPtr.rh4CurToken++;
        int x1 = rhPtr.getExpression().getInt();
        rhPtr.rh4CurToken++;
        int y1 = rhPtr.getExpression().getInt();
        
		double angle=((Math.PI*2 - Math.atan2(y1, x1))%(Math.PI*2))*180/Math.PI;

        rhPtr.rh4Results[rhPtr.rh4PosPile].forceDouble((int)(angle));
    }
}
