//----------------------------------------------------------------------------------
//
// CPARAMKEY: une touche
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;
using Microsoft.Xna.Framework.Input;

namespace RuntimeXNA.Params
{	
	public class PARAM_KEY:CParam
	{
		public Keys key;
        public short mouseKey;
		
		public override void  load(CRunApp app)
		{
			short k = app.file.readAShort();
			key = CKeyConvert.getXnaKey(k);
            mouseKey = k;
		}
	}
}