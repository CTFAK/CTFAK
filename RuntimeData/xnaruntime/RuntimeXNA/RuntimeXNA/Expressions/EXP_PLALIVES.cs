//----------------------------------------------------------------------------------
//
// LIVES
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLALIVES:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.lives[oi]);
		}
	}
}