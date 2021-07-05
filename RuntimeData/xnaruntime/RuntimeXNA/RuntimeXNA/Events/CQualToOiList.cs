//----------------------------------------------------------------------------------
//
// CQUALTOOI : qualifiers
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RuntimeXNA.Events
{
	
	public class CQualToOiList
	{		
		public short qoiCurrentOi = 0;
		public short qoiNext = 0;
		public short qoiActionPos = 0;
		public int qoiCurrentRoutine = 0;
		public int qoiActionCount = 0;
		public int qoiActionLoopCount = 0;
		public bool qoiNextFlag = false;
		public bool qoiSelectedFlag = false;
		public short[] qoiList = null; // Array OINUM / OFFSET
		
		public CQualToOiList()
		{
		}
	}
}