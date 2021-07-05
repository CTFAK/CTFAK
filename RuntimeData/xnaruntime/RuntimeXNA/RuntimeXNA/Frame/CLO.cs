//----------------------------------------------------------------------------------
//
// CLO
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Application;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Frame
{
    public class CLO
    {
        public const short PARENT_NONE=0;
        public const short PARENT_FRAME=1;
        public const short PARENT_FRAMEITEM=2;
        public const short PARENT_QUALIFIER=3;
        
        public short loHandle;			// Le handle
        public short loOiHandle;			// HOI
        public int loX;				// Coords
        public int loY;
        public short loParentType;			// Parent type
        public short loOiParentHandle;		// HOI Parent
        public short loLayer;			// Layer
        public short loType;
        public CSprite[] loSpr;			// Sprite handles for backdrop objects from layers > 1

        public CLO()
        {
            loSpr = new CSprite[4];
            int i;
            for (i = 0; i < 4; i++)
            {
                loSpr[i] = null;
            }
        }
        
        public void load(CFile file)
        {
	        loHandle=file.readAShort();
	        loOiHandle=file.readAShort();
	        loX=file.readAInt();
	        loY=file.readAInt();
	        loParentType=file.readAShort();
	        loOiParentHandle=file.readAShort();
	        loLayer=file.readAShort();
	        file.skipBytes(2);
        }

    }
}
