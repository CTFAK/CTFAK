//----------------------------------------------------------------------------------
//
// CRCOM : Structure commune aux objets animes
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects
{
    public class CRCom
    {
        public int rcPlayer = 0;					// Player who controls
        public int rcMovementType=0;				// Number of the current movement
        public CSprite rcSprite = null;					// Sprite ID if defined
        public int rcAnim = 0;						// Wanted animation
        public short rcImage = -1;					// Current frame
        public float rcScaleX=1.0F;
        public float rcScaleY=1.0F;
        public int rcAngle=0;
        public int rcDir = 0;						// Current direction
        public int rcSpeed = 0;					// Current speed
        public int rcMinSpeed = 0;					// Minimum speed
        public int rcMaxSpeed = 0;					// Maximum speed
        public bool rcChanged = false;					// Flag: modified object
        public bool rcCheckCollides = false;			// For static objects

        public int rcOldX=0;            			// Previous coordinates
        public int rcOldY=0;
        public short rcOldImage = -1;
        public int rcOldAngle=0;
        public int rcOldDir=0;
        public int rcOldX1=0;					// For zone detections
        public int rcOldY1=0;
        public int rcOldX2=0;
        public int rcOldY2=0;

        public void init()
        {
            rcScaleX = 1.0f;
            rcScaleY = 1.0f;
            rcAngle = 0;
            rcMovementType = -1;
        }

        public void kill(bool bFast)
        {
        }

    }
}
