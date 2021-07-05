package Expressions;

import RunLoop.CRun;

public class EXP_RANGE extends CExp
{
    public void evaluate(CRun rhPtr)
    {
        rhPtr.rh4CurToken++;
        CValue v = rhPtr.get_ExpressionAny();
        rhPtr.rh4CurToken++;
        CValue min = rhPtr.get_ExpressionAny();
        rhPtr.rh4CurToken++;
        CValue max = rhPtr.get_ExpressionAny();
        if (v.getType()==CValue.TYPE_INT && min.getType()==CValue.TYPE_INT && max.getType()==CValue.TYPE_INT )
        {
            int value=v.getInt();
            int minimum=min.getInt();
            int maximum=max.getInt();
            if (value < minimum)
                value = minimum;
            if (value > maximum)
                value = maximum;
            rhPtr.rh4Results[rhPtr.rh4PosPile].forceInt(value);
        }
        else
        {
            double value=v.getDouble();
            double minimum=min.getDouble();
            double maximum=max.getDouble();
            if (value < minimum)
                value = minimum;
            if (value > maximum)
                value = maximum;
            rhPtr.rh4Results[rhPtr.rh4PosPile].forceDouble(value);
        }
    }
    
    
}
