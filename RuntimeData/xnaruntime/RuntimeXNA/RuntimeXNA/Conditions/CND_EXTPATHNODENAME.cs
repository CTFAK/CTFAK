// ------------------------------------------------------------------------------
// 
// NODE NAME REACHED?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Movements;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTPATHNODENAME:CCnd, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			System.String pName = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
			if (hoPtr.hoMT_NodeName != null)
			{
				if (String.CompareOrdinal(hoPtr.hoMT_NodeName, pName) == 0)
				{
					return true;
				}
			}
			return false;
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaObject(rhPtr, this);
		}
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			if (hoPtr.roc.rcMovementType != CMoveDef.MVTYPE_TAPED)
				return false;
			if (checkMark(hoPtr.hoAdRunHeader, hoPtr.hoMark1))
			{
				System.String pName = hoPtr.hoAdRunHeader.get_EventExpressionString((CParamExpression) evtParams[0]);
				if (hoPtr.hoMT_NodeName != null)
				{
					if (String.CompareOrdinal(hoPtr.hoMT_NodeName, pName) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}