// ------------------------------------------------------------------------------
// 
// COMPARE TO NUMBER OF OBJECTS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTNUMOFOBJECT:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int count = 0;
			
			CObjInfo poil;
			short oil = evtOiList;
			if (oil >= 0)
			{
				// Un objet normal
				poil = rhPtr.rhOiList[oil];
				count = poil.oilNObjects;
			}
			else
			{
				// Un qualifier
				if (oil != - 1)
				{
					CQualToOiList pqoi = rhPtr.rhEvtProg.qualToOiList[oil & 0x7FFF];
					int qoi;
					for (qoi = 0; qoi < pqoi.qoiList.Length; qoi += 2)
					{
						poil = rhPtr.rhOiList[pqoi.qoiList[qoi + 1]];
						count += poil.oilNObjects;
					}
				}
			}
			
			int value_Renamed = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			return CRun.compareTer(count, value_Renamed, ((CParamExpression) evtParams[0]).comparaison);
		}
	}
}