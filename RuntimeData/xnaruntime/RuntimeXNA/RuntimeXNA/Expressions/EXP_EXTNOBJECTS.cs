//----------------------------------------------------------------------------------
//
// NUMBER OF OBJECTS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Events;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_EXTNOBJECTS:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
			// Cherche dans la liste des oi
			short qoil = oiList;
			CObjInfo poil;
			if (qoil >= 0)
			{
				// Un OI Normal
				poil = rhPtr.rhOiList[qoil];
                rhPtr.getCurrentResult().forceInt(poil.oilNObjects);
			}
			else
			{
				// Un qualifier
				int count = 0;
				if (qoil != - 1)
				{
					CQualToOiList pqoi = rhPtr.rhEvtProg.qualToOiList[qoil & 0x7FFF];
					int qoi;
					for (qoi = 0; qoi < pqoi.qoiList.Length; qoi += 2)
					{
						poil = rhPtr.rhOiList[pqoi.qoiList[qoi + 1]];
						count += poil.oilNObjects;
					}
				}
                rhPtr.getCurrentResult().forceInt(count);
			}
		}
	}
}