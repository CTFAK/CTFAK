// -----------------------------------------------------------------------------
//
// SUBTRACT TO LIVES
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SUBLIVES:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int value = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]); // Expression
			int joueur = evtOi; // Joueur
			value = rhPtr.rhApp.lives[joueur] - value;
			rhPtr.actPla_FinishLives(joueur, value);
		}
	}
}