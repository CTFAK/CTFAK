// -----------------------------------------------------------------------------
//
// SET NUMBER OF LIVES
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETLIVES:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int value_Renamed = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]); // Expression
			int joueur = evtOi; // Joueur
			rhPtr.actPla_FinishLives(joueur, value_Renamed);
		}
	}
}