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
// CSPRITE : Un sprite
//
//----------------------------------------------------------------------------------
package Sprites;

import Application.CRunApp;
import Banks.CImage;
import Banks.CImageBank;
import Objects.CObject;
import OpenGL.GLRenderer;
import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CRect;

public class CSprite 
{
	// Defines
	public static final int SF_RAMBO=0x00000001;		// flag "rentre dans tout le monde"
	//public static final int SF_RECALCSURF=0x00000002;		// Recalc surface (if rotation or stretch)
	public static final int SF_PRIVATE=0x00000004;		// flag prive utilise par le yourapplication pour la destruction des fade
	public static final int SF_INACTIF=0x00000008;		// flag "inactif" = reaffichage ssi intersection avec un autre
	public static final int SF_TOHIDE=0x00000010;		// flag "a cacher"
	public static final int SF_TOKILL=0x00000020;		// flag "a detruire"
	public static final int SF_REAF=0x00000040;		    // flag "a reafficher"
	public static final int SF_HIDDEN=0x00000080;		// flag "cache"
	public static final int SF_COLBOX=0x00000100;		// flag "collisions en mode box"
	public static final int SF_NOSAVE=0x00000200;		// flag "do not save background"
	public static final int SF_FILLBACK=0x00000400;		// flag "fill background using a solid colour (sprAdrBack)"
	public static final int SF_DISABLED=0x00000800;
	public static final int SF_REAFINT=0x00001000;		// Internal
	public static final int SF_OWNERDRAW=0x00002000;		// flag "owner draw"
	public static final int SF_OWNERSAVE=0x00004000;		// flag "owner save"
	public static final int SF_FADE=0x00008000;			// Private
	public static final int SF_OBSTACLE=0x00010000;		// Obstacle
	public static final int SF_PLATFORM=0x00020000;		// Platform
	public static final int SF_BACKGROUND=0x00080000;		// Backdrop object
	public static final int SF_SCALE_RESAMPLE=0x00100000;	// Resample when stretching
	public static final int SF_ROTATE_ANTIA=0x00200000;		// Antialiasing for rotations
	public static final int SF_NOHOTSPOT=0x00400000;		// No hot spot
	public static final int SF_OWNERCOLMASK=0x00800000;		// Owner-draw sprite supports collision masks
	public static final int SF_UPDATECOLLIST=0x10000000;
	public static final int SF_ANTIALIASING=0x20000000;		// Antialiasing when drawing
	public static final int EFFECTFLAG_TRANSPARENT=0x10000000;
	public static final int EFFECTFLAG_ANTIALIAS=0x20000000;
	public static final int EFFECT_SEMITRANSP=1;

	// Data
	public CSprite objPrev=null;
	public CSprite objNext=null;
	CImageBank bank;

	public int sprFlags;			/// Flags
	public short sprLayer;			/// Sprite plane (layer)
	public float sprAngle;			/// Angle
	public int sprZOrder;			/// Z-order value

	// Coordinates
	public int sprX;
	public int sprY;

	// Bounding box
	public int sprX1;
	public int sprY1;
	public int sprX2;
	public int sprY2;

	// New coordinates
	public int sprXnew;
	public int sprYnew;

	// New bounding box
	public int sprX1new;
	public int sprY1new;
	public int sprX2new;
	public int sprY2new;

	// Background bounding box
	//public int sprX1z;
	//public int sprY1z;
	//public int sprX2z;
	//public int sprY2z;

	// Scale & Angle
	public float sprScaleX;
	public float sprScaleY;

	// Temporary values for collisions
	//public short sprTempImg;			// use DWORD later?
	//public float sprTempAngle;
	//public float sprTempScaleX;
	//public float sprTempScaleY;

	// Image or owner-draw routine
	public short sprImg;		        /// Numero d'image
	public short sprImgNew;			/// Nouvelle image
	public IDrawable sprRout;			/// Ownerdraw callback routine

	// Ink effect
	public int sprEffect;				/// 0=normal, 1=semi-transparent, > 16 = routine
	public int sprEffectParam;			/// parametre effet (coef transparence, etc...)
	public int effectShader;			/// if this value is -1 means a default shader

	// Fill color (wipe with color mode)
	//public int sprBackColor;

	// Surfaces
	//byte sprBackSurf[]=null;			/// Background surface, if no general background surface
	//CMask sprColMask=null;			/// Collision mask (if stretched or rotated)
	//CMask sprTempColMask=null;		/// Temp collision mask (if stretched or rotated)
	
	// User data
	public CObject sprExtraInfo;

	// Colliding sprites
	//ArrayList<CSprite> sprCollisList;			/// liste de sprites entrant en collisions

	public CSprite() 
	{
	}

	public CSprite(CImageBank b) 
	{
		bank=b;
		//sprCollisList=new ArrayList<CSprite>();
		effectShader = -1;	// very sensitive just put as default for now
	}

	////////////////////////////////////////////////
	//
	// Get sprite layer
	//
	public int getSpriteLayer ()
	{
		return sprLayer/2;
	}

	////////////////////////////////////////////////
	//
	// Get Sprite Flags
	//
	public int getSpriteFlags ()
	{
		return sprFlags;
	}
	public int setSpriteFlags (int dwNewFlags)
	{
		int dwOldFlags;
		dwOldFlags = sprFlags;
		sprFlags = dwNewFlags;
		return dwOldFlags;
	}

	////////////////////////////////////////////////
	//
	// Change the collision mode of a sprite (box or fine)
	//
	public int setSpriteColFlag ( int colMode )
	{
		int om;
		om = (sprFlags & SF_RAMBO);
		sprFlags = (sprFlags & ~SF_RAMBO) | colMode;
		return om;
	}

	//----------------------------------------------;
	// Virer le buffer de sauvegarde d'un sprite	;
	//----------------------------------------------;
	public void killSpriteZone ( )
	{
		//sprBackSurf=null;
	}

	////////////////////////////////////////////////
	//
	// Get sprite scale
	//
	public float getSpriteScaleX ()
	{
		return sprScaleX;
	}
	public float getSpriteScaleY ()
	{
		return sprScaleY;
	}
	public boolean getSpriteScaleResample ()
	{
		return (sprFlags & SF_SCALE_RESAMPLE) != 0;
	}

	////////////////////////////////////////////////
	//
	// Get sprite angle
	//
	public float getSpriteAngle ()
	{
		return sprAngle;
	}
	public boolean getSpriteAngleAntiA()
	{
		return ((sprFlags & SF_ROTATE_ANTIA) != 0);
	}

	////////////////////////////////////////////////
	//
	// Get sprite rectangle
	//
	public CRect getSpriteRect()
	{
		CRect prc=new CRect();
		prc.left = sprX1new;
		prc.right = sprX2new;
		prc.top = sprY1new;
		prc.bottom = sprY2new;
		return prc;
	}

	public void updateBoundingBox()
	{
		// Get image size & hot spot
		CImage ptei=bank.getImageFromHandle(sprImgNew);
		if ( ptei==null )
		{
			sprX1new = sprXnew;
			sprX2new = sprXnew+1;
			sprY1new = sprYnew;
			sprY2new = sprYnew+1;
			return;
		}

		int cx = ptei.getWidth ();
		int cy = ptei.getHeight ();
		int hsx = 0;
		int hsy = 0;
		if ( (sprFlags & SF_NOHOTSPOT) == 0 )
		{
			hsy = ptei.getYSpot ();
			hsx = ptei.getXSpot ();
		}

		// No rotation
		if ( sprAngle == 0 )
		{
			// Pas de stretch en X
			if ( sprScaleX == 1.0f )
			{
				sprX1new = sprXnew - hsx;
				sprX2new = sprX1new + cx;
			}

			// Stretch en X
			else
			{
				sprX1new = sprXnew - (int)(hsx * sprScaleX);
				sprX2new = sprX1new + (int)(cx * sprScaleX);
			}

			// Pas de stretch en Y
			if ( sprScaleY == 1.0f )
			{
				sprY1new = sprYnew - hsy;
				sprY2new = sprY1new + cy;
			}

			// Stretch en Y
			else
			{
				sprY1new = sprYnew - (int)(hsy * sprScaleY);
				sprY2new = sprY1new + (int)(cy * sprScaleY);
			}
		}

		// Rotation
		else
		{
			// Calculate dimensions
			//int x1, y1, x2, y2;

			if ( sprScaleX != 1.0f )
			{
				hsx = (int)(hsx * sprScaleX);
				cx = (int)(cx * sprScaleX);
			}
			//x1 = ptSpr.sprXnew - hsx;
			//x2 = ptSpr.sprX1new + cx;

			if ( sprScaleY != 1.0f )
			{
				hsy = (int)(hsy * sprScaleY);
				cy = (int)(cy * sprScaleY);
			}
			//y1 = ptSpr.sprYnew - hsy;
			//y2 = ptSpr.sprY1new + cy;

			// Rotate
			int nhsx;
			int nhsy;

			int nx1;
			int ny1;

			int nx2;
			int ny2;

			int	nx4;
			int	ny4;

			cx--;	// new
			cy--;	// new

			if ( sprAngle == 90 )
			{
				nx2 = cy;
				ny4 = -cx;

				ny2 = 0;
				nx4 = 0;

				nhsx = hsy;
				nhsy = -hsx;
			}
			else if ( sprAngle == 180 )
			{
				nx2 = 0;
				ny4 = 0;

				ny2 = -cy;
				nx4 = -cx;

				nhsx = -hsx;
				nhsy = -hsy;
			}
			else if ( sprAngle == 270 )
			{
				nx2 = -cy;
				ny4 = cx;

				ny2 = 0;
				nx4 = 0;

				nhsx = -hsy;
				nhsy = hsx;
			}
			else
			{
				double alpha = sprAngle * Math.PI / 180.0;
				float cosa = (float)Math.cos(alpha);
				float sina = (float)Math.sin(alpha);

				nhsx = (int)(hsx * cosa + hsy * sina);
				nhsy = (int)(hsy * cosa - hsx * sina);
				/*
				if ( sina >= 0.0f )
				{
					nx2 = (int)(cy * sina + 0.5f);		// (1.0f-sina));		// 1-sina est ici pour l'arrondi
					ny4 = -(int)(cx * sina + 0.5f);		// (1.0f-sina));
				}
				else
				{
					nx2 = (int)(cy * sina - 0.5f);		// (1.0f-sina));
					ny4 = -(int)(cx * sina - 0.5f);		// (1.0f-sina));
				}

				if ( cosa == 0.0f )
				{
					ny2 = 0;
					nx4 = 0;
				}
				else if ( cosa > 0.0f )
				{
					ny2 = (int)(cy * cosa + 0.5f);		// (1.0f-cosa));
					nx4 = (int)(cx * cosa + 0.5f);		// (1.0f-cosa));
				}
				else
				{
					ny2 = (int)(cy * cosa - 0.5f);		// (1.0f-cosa));
					nx4 = (int)(cx * cosa - 0.5f);		// (1.0f-cosa));
				} */

				nx2 = (int)(cy * sina);	// new
				ny4 = -(int)(cx * sina);	// new
				ny2 = (int)(cy * cosa);	// new
				nx4 = (int)(cx * cosa);	// new
			}

			int nx3 = nx2 + nx4;
			int ny3 = ny2 + ny4;

			// Faire translation par rapport au hotspot
			nx1 = sprXnew - nhsx;
			ny1 = sprYnew - nhsy;
			nx2 += sprXnew - nhsx;
			ny2 += sprYnew - nhsy;
			nx3 += sprXnew - nhsx;
			ny3 += sprYnew - nhsy;
			nx4 += sprXnew - nhsx;
			ny4 += sprYnew - nhsy;

			// Calculer la nouvelle bounding box
			sprX1new = Math.min(nx1,nx2);
			sprX1new = Math.min(sprX1new,nx3);
			sprX1new = Math.min(sprX1new,nx4);

			sprX2new = Math.max(nx1,nx2);
			sprX2new = Math.max(sprX2new,nx3);
			sprX2new = Math.max(sprX2new,nx4);

			sprX2new++;	// new

			sprY1new = Math.min(ny1,ny2);
			sprY1new = Math.min(sprY1new,ny3);
			sprY1new = Math.min(sprY1new,ny4);

			sprY2new = Math.max(ny1,ny2);
			sprY2new = Math.max(sprY2new,ny3);
			sprY2new = Math.max(sprY2new,ny4);

			sprY2new++; // new
		}
	}

	public void calcBoundingBox(short newImg, int newX, int newY, float newAngle, float newScaleX, float newScaleY, CRect prc)
	{
		final CImage ptei;

		// Empty rect
		prc.left = prc.top = prc.right = prc.bottom = 0;

		// Get image size & hot spot
		ptei = bank.getImageFromHandle(newImg);
		if (ptei==null)
			return;

		int cx = ptei.getWidth ();
		int cy = ptei.getHeight ();
		int hsx = 0;
		int hsy = 0;
		if ( (sprFlags & SF_NOHOTSPOT) == 0 )
		{
			hsy = ptei.getYSpot ();
			hsx = ptei.getXSpot ();
		}

		// No rotation
		if ( newAngle == 0 )
		{
			// Pas de stretch en X
			if ( newScaleX == 1.0f )
			{
				prc.left = newX - hsx;
				prc.right = prc.left + cx;
			}

			// Stretch en X
			else
			{
				prc.left = newX - (int)(hsx * newScaleX);
				prc.right = prc.left + (int)(cx * newScaleX);
			}

			// Pas de stretch en Y
			if ( newScaleY == 1.0f )
			{
				prc.top = newY - hsy;
				prc.bottom = prc.top + cy;
			}

			// Stretch en Y
			else
			{
				prc.top = newY - (int)(hsy * newScaleY);
				prc.bottom = prc.top + (int)(cy * newScaleY);
			}
		}

		// Rotation
		else
		{
			// Calculate dimensions
			if ( newScaleX != 1.0f )
			{
				hsx = (int)(hsx * newScaleX);
				cx = (int)(cx * newScaleX);
			}

			if ( newScaleY != 1.0f )
			{
				hsy = (int)(hsy * newScaleY);
				cy = (int)(cy * newScaleY);
			}

			// Rotate
			int nhsx;
			int nhsy;

			int nx1;
			int ny1;

			int nx2;
			int ny2;

			int	nx4;
			int	ny4;

			cx--;	// new
			cy--;	// new

			if ( newAngle == 90 )
			{
				nx2 = cy;
				ny4 = -cx;

				ny2 = 0;
				nx4 = 0;

				nhsx = hsy;
				nhsy = -hsx;
			}
			else if ( newAngle == 180 )
			{
				nx2 = 0;
				ny4 = 0;

				ny2 = -cy;
				nx4 = -cx;

				nhsx = -hsx;
				nhsy = -hsy;
			}
			else if ( newAngle == 270 )
			{
				nx2 = -cy;
				ny4 = cx;

				ny2 = 0;
				nx4 = 0;

				nhsx = -hsy;
				nhsy = hsx;
			}
			else
			{
				double alpha = newAngle * Math.PI / 180;
				float cosa = (float)Math.cos(alpha);
				float sina = (float)Math.sin(alpha);

				nhsx = (int)(hsx * cosa + hsy * sina);
				nhsy = (int)(hsy * cosa - hsx * sina);

				nx2 = (int)(cy * sina);	// new
				ny4 = -(int)(cx * sina);	// new
				ny2 = (int)(cy * cosa);	// new
				nx4 = (int)(cx * cosa);	// new
			}

			int nx3 = nx2 + nx4;
			int ny3 = ny2 + ny4;

			// Faire translation par rapport au hotspot
			nx1 = newX - nhsx;
			ny1 = newY - nhsy;
			nx2 += newX - nhsx;
			ny2 += newY - nhsy;
			nx3 += newX - nhsx;
			ny3 += newY - nhsy;
			nx4 += newX - nhsx;
			ny4 += newY - nhsy;

			// Calculer la nouvelle bounding box
			prc.left = Math.min(nx1,nx2);
			prc.left = Math.min(prc.left,nx3);
			prc.left = Math.min(prc.left,nx4);

			prc.right = Math.max(nx1,nx2);
			prc.right = Math.max(prc.right,nx3);
			prc.right = Math.max(prc.right,nx4);

			prc.right++;	// new

			prc.top = Math.min(ny1,ny2);
			prc.top = Math.min(prc.top,ny3);
			prc.top = Math.min(prc.top,ny4);

			prc.bottom = Math.max(ny1,ny2);
			prc.bottom = Math.max(prc.bottom,ny3);
			prc.bottom = Math.max(prc.bottom,ny4);

			prc.bottom++;	// new
		}
	}

	/** Draws the sprite onto the screen.
	 */
	public void draw()
	{
		if(sprX2 < 0 || sprY2 < 0
				|| sprY1 > GLRenderer.limitY
				|| sprX1 > GLRenderer.limitX)
		{
			return;
		}

		final CImage ci=bank.getImageFromHandle(sprImg);
		if(ci == null)
			return;

		synchronized(ci)
		{
			synchronized(GLRenderer.inst) 
			{
				//Log.Log("Draw "+(sprExtraInfo != null ? sprExtraInfo.hoOiList.oilName :"null")+" alias from texture of:"+(ci.getAntialias()?"yes":"no")+" and from sprite is: "+(sprAntialias?"yes":"no"));
				ci.setResampling((MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0);

				if(effectShader != - 1)
					GLRenderer.inst.setEffectShader(effectShader);
				else
					GLRenderer.inst.removeEffectShader();
					
				int bHotSpot = 0;
				if ( (sprFlags&CSprite.SF_NOHOTSPOT) == 0 )
					bHotSpot = 1;
				GLRenderer.inst.renderScaledRotatedImage2(ci, sprAngle, sprScaleX, sprScaleY, bHotSpot, sprX, sprY, sprEffect, sprEffectParam);
				
				if(effectShader != - 1)
					GLRenderer.inst.removeEffectShader();
			}
		}
	}
}
