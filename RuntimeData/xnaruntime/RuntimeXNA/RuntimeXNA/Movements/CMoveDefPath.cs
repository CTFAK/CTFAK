//----------------------------------------------------------------------------------
//
// CMOVEDEFPATH : données du mouvement path
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    class CMoveDefPath : CMoveDef
    {
        public short mtNumber;				// Number of movement 
        public short mtMinSpeed; 			// maxs and min speed in the movements 
        public short mtMaxSpeed;
        public byte mtLoop;					// Loop at end
        public byte mtRepos;				// Reposition at end
        public byte mtReverse;				// Pingpong?
        public CPathStep[] steps;

        public override void load(CFile file, int length)
        {
            mtNumber=file.readAShort();
            mtMinSpeed=file.readAShort();
            mtMaxSpeed=file.readAShort();
            mtLoop=file.readByte();	
            mtRepos=file.readByte();
            mtReverse=file.readByte();
            file.skipBytes(1);

            steps=new CPathStep[mtNumber];
            int n, next;
            int debut;
            for (n=0; n<mtNumber; n++)
            {
                debut=file.getFilePointer();
                steps[n]=new CPathStep();
                file.readUnsignedByte();
                next=file.readUnsignedByte();
                steps[n].load(file);
                file.seek(debut+next);
            }
        }

    }
}
