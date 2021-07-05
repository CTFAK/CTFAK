//----------------------------------------------------------------------------------
//
// NIVEAU COURANT
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GAMLEVEL:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceInt(rhPtr.rhApp.currentFrame);
		}
	}
}