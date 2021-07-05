//----------------------------------------------------------------------------------
//
// IDENTIFIER
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTIDENTIFIER:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
			int id = (pHo.hoCreationId << 16) | (((int) pHo.hoNumber) & 0xFFFF);
            rhPtr.getCurrentResult().forceInt(id);
		}
	}
}