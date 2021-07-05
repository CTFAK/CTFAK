//----------------------------------------------------------------------------------
//
// ALTERABLE VALUE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTVAR:CExpOi
	{
		public short number;
		
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			if (pHo.rov != null)
			{
                rhPtr.getCurrentResult().forceValue(pHo.rov.getValue(number));
			}
			else
			{
                rhPtr.getCurrentResult().forceInt(0);
			}
		}
	}
}