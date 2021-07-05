//----------------------------------------------------------------------------------
//
// VAR BY INDEX
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Values;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTVARBYINDEX:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			rhPtr.rh4CurToken++;
			int num = rhPtr.get_ExpressionInt();
			if (pHo == null || num < 0 || num >= CRVal.VALUES_NUMBEROF_ALTERABLE)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
            rhPtr.getCurrentResult().forceValue(pHo.rov.getValue(num));
		}
	}
}