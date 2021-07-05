//----------------------------------------------------------------------------------
//
// SCORE
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_PLASCORE:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			int[] scores = rhPtr.rhApp.scores;
            rhPtr.getCurrentResult().forceInt(scores[oi]);
		}
	}
}