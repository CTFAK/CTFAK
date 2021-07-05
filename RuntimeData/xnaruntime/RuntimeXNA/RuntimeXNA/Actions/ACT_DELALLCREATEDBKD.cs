// -----------------------------------------------------------------------------
//
// DELETE ALL CREATED BACKGROUND
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_DELALLCREATEDBKD:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int layer = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]) - 1;
			rhPtr.deleteAllBackdrop2(layer);
		}
	}
}