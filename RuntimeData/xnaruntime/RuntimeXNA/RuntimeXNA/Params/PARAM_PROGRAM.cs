//----------------------------------------------------------------------------------
//
// PARAM_PROGRAM un programme a faire fonctionner
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Params
{
	
	public class PARAM_PROGRAM:CParam
	{
		public short flags;
		public System.String filename;
		public System.String command;
		public const short PRGFLAGS_WAIT = (short) (0x0001);
		public const short PRGFLAGS_HIDE = (short) (0x0002);
		
		public override void  load(CRunApp app)
		{
			flags = app.file.readAShort();
			int debut = app.file.getFilePointer();
			filename = app.file.readAString();
			app.file.seek(debut + 260); // _MAX_PATH
			command = app.file.readAString();
		}
	}
}