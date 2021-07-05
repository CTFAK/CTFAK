//----------------------------------------------------------------------------------
//
// CPUSHEDEVENT : un evenement pousse
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Objects;

namespace RuntimeXNA.Events
{	
	public class CPushedEvent
	{
		public int routine;
		public int code;
		public int param;
		public CObject pHo;
		public short oi;
		
		public CPushedEvent()
		{
		}
		public CPushedEvent(int r, int c, int p, CObject hoPtr, short o)
		{
			routine = r;
			code = c;
			param = p;
			pHo = hoPtr;
			oi = o;
		}
	}
}