//----------------------------------------------------------------------------------
//
// CANIMDIR : Une direction d'animation
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;

namespace RuntimeXNA.Animations
{
    public class CAnimDir
    {
        public byte adMinSpeed;					// Minimum speed
        public byte adMaxSpeed;					// Maximum speed
        public short adRepeat;					// Number of loops
        public short adRepeatFrame;				// Where to loop
        public short adNumberOfFrame;			// Number of frames
        public short[] adFrames;

        public void load(CFile file)
        {
            adMinSpeed=file.readAByte();
            adMaxSpeed=file.readAByte();
            adRepeat=file.readAShort();
            adRepeatFrame=file.readAShort();
            adNumberOfFrame=file.readAShort();
        
            adFrames=new short[adNumberOfFrame];
            int n;
            for (n=0; n<adNumberOfFrame; n++)
            {
                adFrames[n]=file.readAShort();
            }
        }

        public void enumElements(IEnum enumImages)
        {
            int n;
            for (n = 0; n < adNumberOfFrame; n++)
            {
                if (enumImages != null)
                {
                    short num = enumImages.enumerate(adFrames[n]);
                    if (num != -1)
                    {
                        adFrames[n] = num;
                    }
                }
            }
        }
    }
}
