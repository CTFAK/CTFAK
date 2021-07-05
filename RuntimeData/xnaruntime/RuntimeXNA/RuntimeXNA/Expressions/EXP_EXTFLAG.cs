//----------------------------------------------------------------------------------
//
// FLAG
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTFLAG:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			rhPtr.rh4CurToken++; // Saute le token
			int num = rhPtr.get_ExpressionInt(); // Le numero du flag
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			num &= 31;
			if (pHo.rov != null)
			{
				int result = 0;
				if (((1 << num) & pHo.rov.rvValueFlags) != 0)
				{
					result = 1;
				}
                rhPtr.getCurrentResult().forceInt(result);
			}
			else
			{
                rhPtr.getCurrentResult().forceInt(0);
			}
		}
	}
}