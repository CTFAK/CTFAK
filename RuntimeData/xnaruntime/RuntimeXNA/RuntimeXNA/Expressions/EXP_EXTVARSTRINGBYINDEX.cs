//----------------------------------------------------------------------------------
//
// STRING BY INDEX
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Values;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTVARSTRINGBYINDEX:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			rhPtr.rh4CurToken++;
			int num = rhPtr.get_ExpressionInt();
			if (pHo == null || num < 0 || num >= CRVal.STRINGS_NUMBEROF_ALTERABLE)
			{
                rhPtr.getCurrentResult().forceString("");
				return ;
			}
            rhPtr.getCurrentResult().forceString(pHo.rov.getString(num));
		}
	}
}