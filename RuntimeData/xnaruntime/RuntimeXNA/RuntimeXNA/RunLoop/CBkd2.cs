using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.RunLoop
{
    public class CBkd2
    {
        public short loHnd;			// 0 
        public short oiHnd;			// 0 
        public int x;
        public int y;
        public short img;
        public short colMode;
        public short nLayer;
        public short obstacleType;
        public CSprite[] pSpr=new CSprite[4];
        public int inkEffect;
        public int inkEffectParam;    
    }
}
