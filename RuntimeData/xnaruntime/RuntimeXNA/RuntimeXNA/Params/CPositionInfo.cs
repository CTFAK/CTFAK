//----------------------------------------------------------------------------------
//
// CPOSITIONINFO: retour de parametres Position
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
namespace RuntimeXNA.Params
{
	
	public class CPositionInfo
	{
		public int x;
		public int y;
		public int dir;
		public int layer;
		public bool bRepeat = false;
		
		public CPositionInfo()
		{
		}
	}
}