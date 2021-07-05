//----------------------------------------------------------------------------------
//
// COC: classe abstraite d'objectsCommon
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;

namespace RuntimeXNA.OI
{
    public class COC
    {
        public const short OBSTACLE_NONE=0;
        public const short OBSTACLE_SOLID=1;
        public const short OBSTACLE_PLATFORM=2;
        public const short OBSTACLE_LADDER=3;
        public const short OBSTACLE_TRANSPARENT = 4;

        public short ocObstacleType=0;		// Obstacle type
        public short ocColMode=0;			// Collision mode (0 = fine, 1 = box)
        public int ocCx=0;				// Size
        public int ocCy=0;
        public COI oi;

        public virtual void load(CFile file, short type) { }
        public virtual void enumElements(IEnum enumImages, IEnum enumFonts) { }

    }
}
