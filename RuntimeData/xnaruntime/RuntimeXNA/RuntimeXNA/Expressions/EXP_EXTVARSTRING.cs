//----------------------------------------------------------------------------------
//
// ALTERABLE STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTVARSTRING:CExpOi
	{
		public short number;
		
		public override void  evaluate(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (pHo == null)
			{
                rhPtr.getCurrentResult().forceString("");
				return ;
			}
            rhPtr.getCurrentResult().forceString(pHo.rov.getString(number));
		}
	}
}