// -----------------------------------------------------------------------------
//
// GOTO LEVEL
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_GOLEVEL:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			short level;
			if (evtParams[0].code == 26)
			// PARAM_FRAME	    
			{
				level = (short)((PARAM_SHORT) evtParams[0]).value;
				// Verifie la validite du level
				if (rhPtr.rhApp.HCellToNCell(level) == - 1)
				{
					return ;
				}
			}
			else
			{
				// Avec un calcul
				level = (short) (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]) - 1); // Une expression
				if (level < 0 || level >= 4096)
					return ; // Entre 0 et 4096
				level |= unchecked((short)0x8000);
			}
			rhPtr.rhQuit = CRun.LOOPEXIT_GOTOLEVEL;
			rhPtr.rhQuitParam = (int)level;
		}
	}
}