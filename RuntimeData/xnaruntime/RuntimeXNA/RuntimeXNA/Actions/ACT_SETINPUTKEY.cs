// -----------------------------------------------------------------------------
//
// SET INPUT KEY
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETINPUTKEY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int touche = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]); //; Numero de la touche
			if (touche >= 8)
			// MAX_KEY
				return ;
			int joueur = evtOi;
			if (joueur >= 4)
			// MAX_PLAYER 
				return ;
			rhPtr.rhApp.pcCtrlKeys[joueur*4+touche] = ((PARAM_KEY) evtParams[1]).key; // Nouvelle touche
		}
	}
}