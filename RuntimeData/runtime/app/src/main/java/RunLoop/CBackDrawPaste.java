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
// -----------------------------------------------------------------------------
//
// CBACKDRAWCLS effacement du fond de l'ecran
//
// -----------------------------------------------------------------------------
package RunLoop;

import Banks.CImage;
import Sprites.CColMask;
import Sprites.CMask;

public class CBackDrawPaste extends CBackDraw
{
    public short img;
    public int x;
    public int y;
    public short typeObst;
    public int inkEffect;
    public int inkEffectParam;
       
    @Override
	public void execute(CRun rhPtr)
    {
		// Demande la largeur et la hauteur de l'image
		CImage ifo=rhPtr.rhApp.imageBank.getImageFromHandle(img);
	
		int xImage = x - rhPtr.rhWindowX;
		int x1Image = xImage;
		int x2Image = x1Image;
		if(ifo != null) {
			x1Image -= ifo.getXSpot();
			x2Image += ifo.getWidth();
		}
		int yImage = y - rhPtr.rhWindowY;
		int y1Image = yImage;
		int y2Image = y1Image;
		if(ifo != null) {
			y1Image -= ifo.getYSpot();
			y2Image += ifo.getHeight();		
		}
		boolean antialiased = false;
		if(ifo != null)
			antialiased = ifo.getResampling();
		// En fonction de type de paste
		CMask mask;
		switch (typeObst)
		{
		    case 0:
				// Un rien
				// -------
				mask=rhPtr.rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
				if (rhPtr.rhFrame.colMask!=null)
	            {
	                rhPtr.rhFrame.colMask.orMask(mask, x1Image, y1Image, CColMask.CM_OBSTACLE | CColMask.CM_PLATFORM, 0);
	            }
				rhPtr.y_Ladder_Sub(0, x1Image, y1Image, x2Image, y2Image);
				break;
		    case 1:
				// Un obstacle
				// -----------
				if (rhPtr.rhFrame.colMask!=null)
                {
                    mask=rhPtr.rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
                    rhPtr.rhFrame.colMask.orMask(mask, x1Image, y1Image, CColMask.CM_OBSTACLE|CColMask.CM_PLATFORM, CColMask.CM_OBSTACLE|CColMask.CM_PLATFORM);
                }
				break;
		    case 2:
				// Une plateforme
				// --------------
				if (rhPtr.rhFrame.colMask!=null)
                {
                    mask=rhPtr.rhApp.imageBank.getImageFromHandle(img).getMask(CMask.GCMF_OBSTACLE, 0, 1.0, 1.0);
                    rhPtr.rhFrame.colMask.orMask(mask, x1Image, y1Image, CColMask.CM_OBSTACLE|CColMask.CM_PLATFORM, 0);
                    rhPtr.rhFrame.colMask.orPlatformMask(mask, x1Image, y1Image);
                }
                break;
		    case 3:
				// Une echelle
				rhPtr.y_Ladder_Add(0, x1Image, y1Image, x2Image, y2Image);
				if (rhPtr.rhFrame.colMask!=null)
                {
                    rhPtr.rhFrame.colMask.fillRectangle(x1Image, y1Image, x2Image, y2Image, 0);
                }
				break;
		    default:
		    	break;
		}
	
		// Paste dans l'image!
		// -------------------
		rhPtr.spriteGen.pasteSpriteEffect(img, x1Image, y1Image, 0, inkEffect, inkEffectParam);
    }
}
