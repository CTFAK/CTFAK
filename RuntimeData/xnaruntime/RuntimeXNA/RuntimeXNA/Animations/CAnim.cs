//----------------------------------------------------------------------------------
//
// CANIM : definition d'une animation
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
    public class CAnim
    {
        // Definition of animation codes
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public const short ANIMID_STOP=0;
        public const short ANIMID_WALK=1;
        public const short ANIMID_RUN=2;
        public const short ANIMID_APPEAR=3;
        public const short ANIMID_DISAPPEAR=4;
        public const short ANIMID_BOUNCE=5;
        public const short ANIMID_SHOOT=6;
        public const short ANIMID_JUMP=7;
        public const short ANIMID_FALL=8;
        public const short ANIMID_CLIMB=9;
        public const short ANIMID_CROUCH=10;
        public const short ANIMID_UNCROUCH=11;
        public const short ANIMID_USER1=12;

        // Table des animations n'ayant qu'une seule vitesse
        // -------------------------------------------------
        static byte[] tableAnimTwoSpeeds=
	    {
	        0,     			                 // 0  ANIMID_STOP
	        1,                                       // 1  ANIMID_WALK
	        1,                                       // 2  ANIMID_RUN
	        0,                                       // 3  ANIMID_APPEAR
	        0,                                       // 4  ANIMID_DISAPPEAR
	        1,                                       // 5  ANIMID_BOUNCE
	        0,                                       // 6  ANIMID_SHOOT
	        1,                                       // 7  ANIMID_JUMP
	        1,                                       // 8  ANIMID_FALL
	        1,                                       // 9  ANIMID_CLIMB
	        1,                                       // 10 ANIMID_CROUCH
	        1,                                       // 11 ANIMID_UNCROUCH
	        1,                                       // 12
	        1,                                       // 13
	        1,                                       // 14
	        1                                        // 15
	    };
        
        
        public CAnimDir[] anDirs;
        public byte[] anTrigo;
        public byte[] anAntiTrigo;
        
        public void load(CFile file)
        {
            int debut=file.getFilePointer();
            
            short[] offsets=new short[32];
            int n;
            for (n=0; n<32; n++)
            {
                offsets[n]=file.readAShort();
            }
            
            anDirs=new CAnimDir[32];
            anTrigo=new byte[32];
            anAntiTrigo=new byte[32];
            for (n=0; n<32; n++)
            {
                anDirs[n]=null;
                anTrigo[n]=0;
                anAntiTrigo[n]=0;
                if (offsets[n]!=0)
                {
                    anDirs[n]=new CAnimDir();
                    file.seek(debut+offsets[n]);
                    anDirs[n].load(file);
                }
            }
        }

        public void enumElements(IEnum enumImages)
        {
            int n;
            for (n=0; n<32; n++)
            {
                if (anDirs[n]!=null)
                {
                    anDirs[n].enumElements(enumImages);
                }
            }
        }

        public void approximate(int nAnim)
        {      
	        // Animation definie: travaille les directions non definies
	        int d, d2, d3;
	        int cpt1, cpt2;
        	
                // Boucle d'exploration des directions
	        for (d=0; d<32; d++)
	        {
                if (anDirs[d]==null)
                {
		            // Boucle d'exploration sens trigonometrique
		            for (d2=0, cpt1=d+1; d2<32; d2++, cpt1++)
		            {
                        cpt1=cpt1&0x1F;
                        if (anDirs[cpt1]!=null)
                        {
                            anTrigo[d]=(byte)cpt1;
            			    break;
                        }
                    }
		            // Boucle d'exploration sens anti-trigonometrique
		            for (d3=0, cpt2=d-1; d3<32; d3++, cpt2--)
		            {
                        cpt2=cpt2&0x1F;
                        if (anDirs[cpt2]!=null)
                        {
                            anAntiTrigo[d]=(byte)cpt2;
            			    break;
                        }
		            }
		            if (cpt1==cpt2 || d2<d3)						//; Les deux pointent sur la meme
		            {
                        anTrigo[d]|=0x40;								//; Trigo plus proche
                    }
		            else if (d3<d2)
		            {
                        anAntiTrigo[d]|=0x40;								//; Anti-trigo plus proche
        		    }
                }
                else
                {
		            // Egalise la vitesse maxi avec la vitesse mini si necessaire
		            if (nAnim<16)
		            {
                        if (tableAnimTwoSpeeds[nAnim]==0)
                        {
            			    anDirs[d].adMinSpeed=anDirs[d].adMaxSpeed;
                        }
		            }
                }
	        }
        }     
    }
}
