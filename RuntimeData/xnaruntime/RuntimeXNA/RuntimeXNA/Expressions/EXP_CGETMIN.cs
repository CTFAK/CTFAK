//----------------------------------------------------------------------------------
//
// VALEUR MINI COMPTEUR
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_CGETMIN:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			CObject hoPtr = rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
			if (hoPtr == null)
			{
                rhPtr.getCurrentResult().forceInt(0);
				return ;
			}
            rhPtr.getCurrentResult().forceValue(((CCounter)hoPtr).cpt_GetMin());
		}
	}
}