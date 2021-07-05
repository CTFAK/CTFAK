//----------------------------------------------------------------------------------
//
// PARAM_GROUPOINTER pointeur sur groupe d'evenements 
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
	public class PARAM_GROUPOINTER:CParam
	{
		public short pointer;
		public short id;
		
		public override void  load(CRunApp app)
		{
			app.file.skipBytes(4);
			id = app.file.readAShort();
		}
	}
}