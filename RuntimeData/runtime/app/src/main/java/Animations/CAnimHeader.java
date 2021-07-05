/* Copyright (c) 1996-2013 Clickteam
 *
 * This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
 * 
 * Permission is hereby granted to any person obtaining a legal copy 
 * of Clickteam Multimedia Fusion 2 to use or modify this source code for 
 * debugging, optimizing, or customizing applications created with 
 * Clickteam Multimedia Fusion 2.  Any other use of this source code is prohibited.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
//----------------------------------------------------------------------------------
//
// CANIMHEADER : header d'un ensemble d'animations
//
//----------------------------------------------------------------------------------
package Animations;

import Banks.IEnum;
import Services.CFile;

public class CAnimHeader 
{
    // Table d'approximation des animations
    // ------------------------------------
    static final short tableApprox[][]=
    {
	    {CAnim.ANIMID_APPEAR,CAnim.ANIMID_WALK,CAnim.ANIMID_RUN,0},		// 0  ANIMID_STOP
	    {CAnim.ANIMID_RUN,CAnim.ANIMID_STOP,0,0},                       // 1  ANIMID_WALK
	    {CAnim.ANIMID_WALK,CAnim.ANIMID_STOP,0,0},                      // 2  ANIMID_RUN
	    {CAnim.ANIMID_STOP,CAnim.ANIMID_WALK,CAnim.ANIMID_RUN,0},		// 3  ANIMID_APPEAR
	    {CAnim.ANIMID_STOP,0,0,0},                                      // 4  ANIMID_DISAPPEAR
	    {CAnim.ANIMID_STOP,CAnim.ANIMID_WALK,CAnim.ANIMID_RUN,0},		// 5  ANIMID_BOUNCE
	    {CAnim.ANIMID_STOP,CAnim.ANIMID_WALK, CAnim.ANIMID_RUN,0},		// 6  ANIMID_SHOOT
	    {CAnim.ANIMID_WALK, CAnim.ANIMID_RUN, CAnim.ANIMID_STOP,0},		// 7  ANIMID_JUMP
	    {CAnim.ANIMID_STOP, CAnim.ANIMID_WALK, CAnim.ANIMID_RUN,0},		// 8  ANIMID_FALL
	    {CAnim.ANIMID_WALK, CAnim.ANIMID_RUN, CAnim.ANIMID_STOP,0},		// 9  ANIMID_CLIMB
	    {CAnim.ANIMID_STOP,CAnim.ANIMID_WALK,CAnim.ANIMID_RUN,0},		// 10 ANIMID_CROUCH
	    {CAnim.ANIMID_STOP,CAnim.ANIMID_WALK,CAnim.ANIMID_RUN,0},		// 11 ANIMID_UNCROUCH
	    {0, 0, 0, 0},
	    {0, 0, 0, 0},
	    {0, 0, 0, 0},
	    {0, 0, 0, 0},
    };

    public int ahAnimMax;
    public CAnim ahAnims[];
    public byte ahAnimExists[];
    
    public CAnimHeader() 
    {
    }

    public void load(CFile file) 
    {
        long debut=file.getFilePointer();
        
        file.skipBytes(2);          // ahSize
        ahAnimMax=file.readAShort();
        
        short offsets[]=new short[ahAnimMax];
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
				boolean bFlag=false;
				if (cptAnim<12)                                     // Si une des nouvelles animations, on approxime pas!
				{
                    for (n=0; n<4; n++)
                    {
						byte a=ahAnimExists[tableApprox[cptAnim][n]];
                        if (a!=0)
						{
                            ahAnims[cptAnim]=ahAnims[tableApprox[cptAnim][n]];
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
    
    // Marque les images charger
    public void enumElements(IEnum enumImages)
    {
        int n;
        for (n=0; n<ahAnimMax; n++)
        {
            if (ahAnimExists[n]!=0)
            {		
                ahAnims[n].enumElements(enumImages);
            }
        }
    }
}
