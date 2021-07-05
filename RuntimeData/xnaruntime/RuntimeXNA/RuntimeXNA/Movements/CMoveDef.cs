//----------------------------------------------------------------------------------
//
// CMOVEDEFLIST : liste des mouvements d'un objet'
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    public class CMoveDef
    {
        // Definition of movement types
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public const short MVTYPE_STATIC=0;
        public const short MVTYPE_MOUSE=1;
        public const short MVTYPE_RACE=2;
        public const short MVTYPE_GENERIC=3;
        public const short MVTYPE_BALL=4;
        public const short MVTYPE_TAPED=5;
        public const short MVTYPE_PLATFORM=9;
        public const short MVTYPE_DISAPPEAR=11;
        public const short MVTYPE_APPEAR=12;
        public const short MVTYPE_BULLET=13;
        public const short MVTYPE_EXT=14;

        public short mvType;
        public short mvControl;
        public byte mvMoveAtStart;
        public int mvDirAtStart;
        public byte mvOpt;

        public virtual void load(CFile file, int length) {}
    
        public void setData(short t, short c, byte m, int d, byte mo)
        {
            mvType=t;
            mvControl=c;
            mvMoveAtStart=m;
            mvDirAtStart=d;
            mvOpt=mo;
        }        
    }
}
