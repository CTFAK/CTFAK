//----------------------------------------------------------------------------------
//
// CEVENT : une condition ou une action
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Params;

namespace RuntimeXNA.Events
{
	
	public abstract class CEvent
	{
		public int evtCode;
		public short evtOi;
		public short evtOiList;
		public byte evtFlags;
		public byte evtFlags2;
		public byte evtDefType;
		public byte evtNParams;
		public CParam[] evtParams = null;
		
		public const byte EVFLAGS_REPEAT = (byte) (0x01);
		public const byte EVFLAGS_DONE = (byte) (0x02);
		public const byte EVFLAGS_DEFAULT = (byte) (0x04);
		public const byte EVFLAGS_DONEBEFOREFADEIN = (byte) (0x08);
		public const byte EVFLAGS_NOTDONEINSTART = (byte) (0x10);
		public const byte EVFLAGS_ALWAYS = (byte) (0x20);
		public const byte EVFLAGS_BAD = (byte) (0x40);
		public static byte EVFLAGS_BADOBJECT = (byte)0x80;
		//UPGRADE_NOTE: Final was removed from the declaration of 'EVFLAGS_DEFAULTMASK '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		public static readonly byte EVFLAGS_DEFAULTMASK = (byte) (EVFLAGS_ALWAYS + EVFLAGS_REPEAT + EVFLAGS_DEFAULT + EVFLAGS_DONEBEFOREFADEIN + EVFLAGS_NOTDONEINSTART);
		public const byte EVFLAG2_NOT = (byte) (0x01);
		
		public CEvent()
		{
		}
	}
}