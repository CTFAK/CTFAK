package Expressions;


import RunLoop.CRun;

public class EXP_RANDOMRANGE extends CExp
{
    public void evaluate(CRun rhPtr)
    {
        rhPtr.rh4CurToken++;
        int minimum = rhPtr.getExpression().getInt();
        rhPtr.rh4CurToken++;
        int maximum = rhPtr.getExpression().getInt();
        rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(minimum+rhPtr.random((short)((maximum-minimum+1)&0xFFFF)));
        
    }
}
