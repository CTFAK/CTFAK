package Expressions;

import RunLoop.CRun;

public class EXP_DISTANCE extends CExp
{
    public void evaluate(CRun rhPtr)
    {
        rhPtr.rh4CurToken++;
        int x1 = rhPtr.getExpression().getInt();
        rhPtr.rh4CurToken++;
        int y1 = rhPtr.getExpression().getInt();
        rhPtr.rh4CurToken++;
        int x2 = rhPtr.getExpression().getInt();
        rhPtr.rh4CurToken++;
        int y2 = rhPtr.getExpression().getInt();
        int deltaX=x2-x1;
        int  deltaY=y2-y1;
        CValue result = new CValue();
        rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt((int)Math.floor(Math.sqrt(deltaX*deltaX+deltaY*deltaY)));
    }
}
