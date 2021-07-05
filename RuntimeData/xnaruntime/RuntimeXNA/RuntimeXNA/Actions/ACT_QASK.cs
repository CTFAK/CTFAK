// -----------------------------------------------------------------------------
//
// ASK QUESTION
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Events;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_QASK:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			if (evtOiList >= 0)
			{
				qstCreate(rhPtr, evtOi);
				return ;
			}
			
			// Un qualifier: on explore les listes
			if (evtOiList != - 1)
			{
				CQualToOiList qoil = rhPtr.rhEvtProg.qualToOiList[evtOiList & 0x7FFF];
				int qoi;
				for (qoi = 0; qoi < qoil.qoiList.Length; qoi += 2)
				{
					qstCreate(rhPtr, qoil.qoiList[qoi]);
				}
			}
		}
		internal virtual void  qstCreate(CRun rhPtr, short oi)
		{
			// Cherche la position de creation
			CCreate c = (CCreate) evtParams[0];
			CPositionInfo info = new CPositionInfo();
			
			if (c.read_Position(rhPtr, 0x10, info))
			{
				rhPtr.f_CreateObject(c.cdpHFII, oi, info.x, info.y, info.dir, (short) 0, rhPtr.rhFrame.nLayers - 1, - 1);
			}
		}
	}
}