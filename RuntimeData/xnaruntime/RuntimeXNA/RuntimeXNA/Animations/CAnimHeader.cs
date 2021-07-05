//----------------------------------------------------------------------------------
//
// CANIMHEADER : header d'un ensemble d'animations
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
    public class CAnimHeader
    {
        // Table d'approximation des animations
        // ------------------------------------
        static short[] tableApprox =
        {
        3,1,2,0,		// 0  ANIMID_STOP
        2,0,0,0,                           // 1  ANIMID_WALK
        1,0,0,0,                          // 2  ANIMID_RUN
        0,1,2,0,		// 3  ANIMID_APPEAR
        0,0,0,0,                                          // 4  ANIMID_DISAPPEAR
        0,1,2,0,		// 5  ANIMID_BOUNCE
        0,1, 2,0,		// 6  ANIMID_SHOOT
        1, 2, 0,0,		// 7  ANIMID_JUMP
        0, 1, 2,0,		// 8  ANIMID_FALL
        1, 2, 0,0,		// 9  ANIMID_CLIMB
        0,1,2,0,		// 10 ANIMID_CROUCH
        0,1,2,0,		// 11 ANIMID_UNCROUCH
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        };
        
        public short ahAnimMax;
        public CAnim[] ahAnims;
        public byte[] ahAnimExists;

        public void load(CFile file)
        {
            int debut=file.getFilePointer();
            
            file.skipBytes(2);          // ahSize
            ahAnimMax=file.readAShort();
            
            short[] offsets=new short[ahAnimMax];
            int n;
            for (n=0; n<ahAnimMax; n++)
            {
                offsets[n]=file.readAShort();
            }
            
            ahAnims=new CAnim[ahAnimMax];
            ahAnimExists=new byte[ahAnimMax];
            for (n=0; n<ahAnimMax; n++)
            {
                ahAnims[n]=null;
                ahAnimExists[n]=0;
                if (offsets[n]!=0)
                {
                    ahAnims[n]=new CAnim();
                    file.seek(debut+offsets[n]);
                    ahAnims[n].load(file);
                    ahAnimExists[n]=1;
                }
            }
            
            // Approximation des animations
	        int cptAnim;
	        for (cptAnim=0; cptAnim<ahAnimMax; cptAnim++)
	        {
                if (ahAnimExists[cptAnim]==0)
                {
                    // Animation non definie: recherche dans la table d'approximation
                    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		            bool bFlag=false;
		            if (cptAnim<12)                                     // Si une des nouvelles animations, on approxime pas!
		            {
                        for (n=0; n<4; n++)
                        {
			                byte a=ahAnimExists[tableApprox[cptAnim*4+n]];
                            if (a!=0)
			                {
                                ahAnims[cptAnim]=ahAnims[tableApprox[cptAnim*4+n]];
                                bFlag=true;
                                break;
			                }
                        }
		            }
		            if (bFlag==false)
		            {
                        // Pas d'animation disponible: met la premiere trouvee!
                        for (n=0; n<ahAnimMax; n++)
                        {
                    	    if (ahAnimExists[n]!=0)
			                {
                                ahAnims[cptAnim]=ahAnims[n];
                                break;
			                }
                        }
		            }
                }
                else
                {
                    ahAnims[cptAnim].approximate(cptAnim);
                }
            }     
        }

        // Marque les images à charger
        public void enumElements(IEnum enumImages)
        {
            int n;
            for (n = 0; n < ahAnimMax; n++)
            {
                if (ahAnimExists[n] != 0)
                {
                    ahAnims[n].enumElements(enumImages);
                }
            }
        }

    }
}
