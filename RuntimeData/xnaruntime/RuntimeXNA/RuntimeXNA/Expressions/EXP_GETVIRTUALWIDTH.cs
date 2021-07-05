//----------------------------------------------------------------------------------
//
// VIRTUAL WIDTH
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETVIRTUALWIDTH:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhFrame.leVirtualRect.right);
		}
	}
}