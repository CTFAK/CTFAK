// ------------------------------------------------------------------------------
// 
// SCORE
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_SCORE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int[] scores = rhPtr.rhApp.scores;
			return compareCondition(rhPtr, 0, scores[evtOi]);
		}
	}
}