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
// CMOVEPLATFORM : Mouvement plateforme
//
//----------------------------------------------------------------------------------
package Movements;

import java.util.ArrayList;

import Animations.CAnim;
import Application.CRunFrame;
import Banks.CImageInfo;
import Objects.CObject;
import RunLoop.CRun;
import Services.CPoint;
import Services.CRect;
import Sprites.CColMask;

/** The platform movement.
 */
public class CMovePlatform extends CMove
{
	public int MP_Type;
	public int MP_Bounce;
	public int MP_BounceMu;
	public int MP_XSpeed;
	public int MP_Gravity;
	public int MP_Jump;
	public int MP_YSpeed;
	public int MP_XMB;
	public int MP_YMB;
	public int MP_HTFOOT;
	public int MP_JumpControl;
	public int MP_JumpStopped;
	public int MP_PreviousDir;
	public CObject MP_ObjectUnder;
	public int MP_XObjectUnder;
	public int MP_YObjectUnder;
	public boolean MP_NoJump=false;

	public static final short MPJC_NOJUMP=0;
	public static final short MPJC_DIAGO=1;
	public static final short MPJC_BUTTON1=2;
	public static final short MPJC_BUTTON2=3;

	public static final short MPTYPE_WALK=0;
	public static final short MPTYPE_CLIMB=1;
	public static final short MPTYPE_JUMP=2;
	public static final short MPTYPE_FALL=3;
	public static final short MPTYPE_CROUCH=4;
	public static final short MPTYPE_UNCROUCH=5;

	public CMovePlatform() 
	{
	}
	@Override
	public void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr=ho;
		CMoveDefPlatform mpPtr=(CMoveDefPlatform)mvPtr;

		hoPtr.hoCalculX=0;
		hoPtr.hoCalculY=0;								//; Raz pds faibles coordonnees
		MP_XSpeed=0;
		hoPtr.roc.rcSpeed=0;							//; Raz vitesse et coef de rebondissement
		MP_Bounce=0;
		hoPtr.roc.rcPlayer=mvPtr.mvControl;				//; Init numero de joueur
		rmAcc=mpPtr.mpAcc;				//; Init Acceleration
		rmAccValue=getAccelerator(rmAcc);	//; Init valeur a ajouter a la vitesse
		rmDec=mpPtr.mpDec;				//; Init Deceleration
		rmDecValue=getAccelerator(rmDec);	//; Valeur a enlever a la vitesse
		hoPtr.roc.rcMaxSpeed=mpPtr.mpSpeed;			//; Vitesse maxi
		hoPtr.roc.rcMinSpeed=0;							//; Vitesse mini

		MP_Gravity=mpPtr.mpGravity;		//; Gravite
		MP_Jump=mpPtr.mpJump;				//; Jump impulsion
		int jump=mpPtr.mpJumpControl;
		if (jump>3) 
			jump=MPJC_DIAGO;
		MP_JumpControl=jump;						//; Jump control
		MP_YSpeed=0;								//; Current Y speed

		MP_JumpStopped=0;
		MP_ObjectUnder=null;

		moveAtStart(mvPtr);						//; Init direction
		MP_PreviousDir=hoPtr.roc.rcDir;
		hoPtr.roc.rcChanged=true;
		MP_Type=MPTYPE_WALK;        
	}    
	@Override
	public void kill()
	{

	}
	@Override
	public void move()
	{
		int x, y;

		hoPtr.hoAdRunHeader.rhVBLObjet=1;
		int joyDir=hoPtr.hoAdRunHeader.rhPlayer[hoPtr.roc.rcPlayer-1];				//; Lire le joystick
		calcMBFoot();

		// Calcul de la vitesse en X
		// -------------------------
		int xSpeed=MP_XSpeed;
		int speed8, dSpeed;
		if (MP_JumpStopped==0)
		{
			if (xSpeed<=0)
			{
				if ((joyDir&4)!=0)								// Gauche
				{
					// Accelere
					dSpeed=rmAccValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
						dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					xSpeed-=dSpeed;
					speed8=xSpeed/256;						// Vitesse reelle
					if (speed8<-hoPtr.roc.rcMaxSpeed)
						xSpeed=-hoPtr.roc.rcMaxSpeed*256;
				}
				else if (xSpeed<0)
				{
					// Ralenti 
					dSpeed=rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
						dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					xSpeed+=dSpeed;
					if (xSpeed>0) 
						xSpeed=0;
				}
				if ((joyDir&8)!=0)								// Droite
				{
					// Changement instantanne de direction
					xSpeed=-xSpeed;
				}
			}
			if (xSpeed>=0)
			{
				if ((joyDir&8)!=0)							
				{
					dSpeed=rmAccValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
						dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					xSpeed+=dSpeed;
					speed8=xSpeed/256;						// Vitesse reelle
					if (speed8>hoPtr.roc.rcMaxSpeed)
						xSpeed=hoPtr.roc.rcMaxSpeed*256;
				}
				else if (xSpeed>0)
				{
					// Ralenti
					dSpeed=rmDecValue;
					if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
						dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
					xSpeed-=dSpeed;
					if (xSpeed<0) 
						xSpeed=0;
				}
				if ((joyDir&4)!=0)
				{
					// Changement brusque de direction
					xSpeed=-xSpeed;
				}
			}
			MP_XSpeed=xSpeed;
		}

		// Calcul de la vitesse en Y
		// -------------------------
		int ySpeed=MP_YSpeed;
		boolean flag=false;
		while(true)
		{
			switch (MP_Type)
			{
			case 2:     // MPTYPE_FALL:
			case 3:     // MPTYPE_JUMP:
				dSpeed=MP_Gravity<<5;
				if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
					dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);                    
				ySpeed=ySpeed+dSpeed;               // GRAVITY_COEF);
				if (ySpeed>0xFA00) 
					ySpeed=0xFA00;
				break;
			case 0:     // MPTYPE_WALK:
				if ((joyDir&1)!=0)
				{
					// Si pas d'echelle sous les pieds, on ne fait rien
					if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB-4)==0x80000000) 
						break;
					MP_Type=MPTYPE_CLIMB;
					flag=true;
					continue;
				}
				if ((joyDir&2)!=0)
				{
					// Si pas d'echelle sous les pieds, on ne fait rien
					if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB+4)==0x80000000) 
						break;
					MP_Type=MPTYPE_CLIMB;
					flag=true;
					continue;
				}
				break;
			case 1:         // MPTYPE_CLIMB:
				if (flag==false)
				{
					MP_JumpStopped=0;
					// Si pas d'echelle sous les pieds, on ne fait rien
					if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB)==0x80000000) 
						if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB-4)==0x80000000)
							break;
				}
				// Calcul de la vitesse en Y
				if (ySpeed<=0)
				{
					if ((joyDir&1)!=0)						// Haut
					{
						// Accelere
						dSpeed=rmAccValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
							dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						ySpeed-=dSpeed;
						speed8=ySpeed/256;						// Vitesse reelle
						if (speed8<-hoPtr.roc.rcMaxSpeed)
							ySpeed=-hoPtr.roc.rcMaxSpeed*256;
					}
					else
					{
						// Ralenti 
						dSpeed=rmDecValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
							dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						ySpeed+=dSpeed;
						if (ySpeed>0) 
							ySpeed=0;
					}
					if ((joyDir&2)!=0)								// Bas
					{
						// Changement instantanne de direction
						ySpeed=-ySpeed;
					}
				}
				if (ySpeed>=0)
				{
					if ((joyDir&2)!=0)							
					{
						dSpeed=rmAccValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
							dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						ySpeed+=dSpeed;
						speed8=ySpeed/256;						// Vitesse reelle
						if (speed8>hoPtr.roc.rcMaxSpeed)
							ySpeed=hoPtr.roc.rcMaxSpeed*256;
					}
					else
					{
						// Ralenti
						dSpeed=rmDecValue;
						if ((hoPtr.hoAdRunHeader.rhFrame.leFlags&CRunFrame.LEF_TIMEDMVTS)!=0)
							dSpeed=(int)((dSpeed)*hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
						ySpeed-=dSpeed;
						if (ySpeed<0) 
							ySpeed=0;
					}
					if ((joyDir&1)!=0)
					{
						// Changement brusque de direction
						ySpeed=-ySpeed;
					}
				}
				break;
			}            
			break;
		}
		MP_YSpeed=ySpeed;

		// Calculer la direction en fonction des vitesses en X et Y
		// --------------------------------------------------------
		int dir=0;                  // DIRID_E;
		if (xSpeed<0) 
			dir=16;                 // DIRID_W;
		int sX=xSpeed;
		int sY=ySpeed;
		if (sY!=0)
		{
			int flags=0;							//; Flags de signe	
			if (sX<0)								//; DX negatif?
			{
				flags|=1;
				sX=-sX;
			}
			if (sY<0)								//; DY negatif?
			{
				flags|=2;
				sY=-sY;
			}	
			sX<<=8;									//; * 256 pour plus de precision
			sX=sX/sY;
			int i;
			for (i=0; ; i+=2)
			{
				if (sX>=CosSurSin32[i]) 
					break;
			}
			dir=CosSurSin32[i+1];
			if ((flags&0x02)!=0)
			{
				dir=-dir+32;
				dir&=31;
			}
			if ((flags&0x01)!=0)
			{
				dir-=8;
				dir&=31;
				dir=-dir;
				dir&=31;
				dir+=8;
				dir&=31;
			}
		}

		//
		// Calculer la vitesse resultante des 2 vitesses en X et en Y
		// ----------------------------------------------------------
		// Si |cos(dir)| > |sin(dir)|,
		//		|vitesse| = |speedX| / |cos(dir)|
		// Sinon,
		//		|vitesse| = |speedY| / |sin(dir)|
		//
		sX=xSpeed;
		int cosinus=Cosinus32[dir];
		int sinus=Sinus32[dir];
		if (cosinus<0) 
			cosinus=-cosinus;
		if (sinus<0) 
			sinus=-sinus;
		if (cosinus<sinus)			// vitesse = speedX / cos
		{
			cosinus=sinus;			//; vitesse = speedY / sin
			sX=ySpeed;
		}
		if (sX<0)					
		{
			sX=-sX;
		}
		sX=sX/cosinus;
		if (sX>250) 
			sX=250;
		hoPtr.roc.rcSpeed=sX;		//; Valeur absolue de la vitesse

		// Calcule la bonne direction en fonction des mouvements
		switch (MP_Type)
		{
		case 1:         // MPTYPE_CLIMB:
			if (ySpeed<0)
				hoPtr.roc.rcDir=8;      // DIRID_N
			else if (ySpeed>0)
				hoPtr.roc.rcDir=24;     // DIRID_S;
			break;
		case 3:         // MPTYPE_FALL:
			hoPtr.roc.rcDir=dir;
			break;
		default:
			if (xSpeed<0)
				hoPtr.roc.rcDir=16;     // DIRID_W;
			else if (xSpeed>0)
				hoPtr.roc.rcDir=0;      // DIRID_E;
			break;
		}

		// Calcule la bonne animation en fonction des mouvements
		switch (MP_Type)
		{
		case 4:      // MPTYPE_CROUCH:
			hoPtr.roc.rcAnim=CAnim.ANIMID_CROUCH;
			break;
		case 5:     // MPTYPE_UNCROUCH:
			hoPtr.roc.rcAnim=CAnim.ANIMID_UNCROUCH;
			break;
		case 3:     // MPTYPE_FALL:
			hoPtr.roc.rcAnim=CAnim.ANIMID_FALL;
			break;
		case 2:     // MPTYPE_JUMP:
			hoPtr.roc.rcAnim=CAnim.ANIMID_JUMP;
			break;
		case 1:     // MPTYPE_CLIMB:
			hoPtr.roc.rcAnim=CAnim.ANIMID_CLIMB;
			break;
		default:
			hoPtr.roc.rcAnim=CAnim.ANIMID_WALK;
			break;
		}

		// Appel des animations
		if (hoPtr.roa!=null)
			hoPtr.roa.animate();
		calcMBFoot();

		// Appel des mouvements
		newMake_Move(hoPtr.roc.rcSpeed, dir);

		// Decide de la conduite a tenir
		// -----------------------------
		if ( (MP_Type==MPTYPE_WALK || MP_Type==MPTYPE_CLIMB) && MP_NoJump==false )
		{
			// Teste le saut
			boolean bJump=false;
			int j=MP_JumpControl;
			if (j!=0)
			{
				j--;
				if (j==0)
				{
					if ( (joyDir&5)==5 ) 
						bJump=true;							// Haut gauche
						if ( (joyDir&9)==9 ) 
							bJump=true;							// Haut droite
				}
				else
				{
					j<<=4;
					if ((joyDir&j)!=0)
						bJump=true;
				}
			}
			if (bJump)
			{
				MP_YSpeed=-MP_Jump<<8;                  // JUMP_COEF;
				MP_Type=MPTYPE_JUMP;
			}
		} 
		switch (MP_Type)
		{
		case 2:         // MPTYPE_JUMP:
			// Si on arrive en haut du saut, on passe en chute
			if (MP_YSpeed>=0)
			{
				MP_Type=MPTYPE_FALL;
			}
			break;

		case 3:         // MPTYPE_FALL:
			// Si un echelle sous les pieds, on s'arrete
			if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB)!=0x80000000) 
			{
				MP_YSpeed=0;
				MP_Type=MPTYPE_CLIMB;
				hoPtr.roc.rcDir=8;          // DIRID_N;
			}
			break;

		case 0:         // MPTYPE_WALK:
			// Monter / descend une echelle?
			if ((joyDir&3)!=0 && (joyDir&12)==0)
			{
				if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB)!=0x80000000) 
				{
					MP_Type=MPTYPE_CLIMB;
					MP_XSpeed=0;
					break;
				}
			}
			// Passer en crouch?
			if ((joyDir&2)!=0)
			{
				if (hoPtr.roa!=null)
				{
					if (hoPtr.roa.anim_Exist(CAnim.ANIMID_CROUCH))			//; Une animation definie?
					{
						MP_XSpeed=0;
						MP_Type=MPTYPE_CROUCH;
					}
				}
			}

			// Un echelle sous les pieds du joueur?
			if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB)!=0x80000000) 
				break;

			// Test si plateforme a moins de 10 pixels du joueur
			if (tst_SpritePosition(hoPtr.hoX, hoPtr.hoY+10, (short)MP_HTFOOT, CColMask.CM_TEST_PLATFORM, true)==false) 
			{
				// On se rapproche du bord
				x=hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX;					//; Coordonnes
				y=hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY;
				int d=y+MP_HTFOOT-1;						//; 15
				CPoint pt=new CPoint();
				mpApproachSprite(x, d, x, y, (short)MP_HTFOOT, CColMask.CM_TEST_PLATFORM, pt);

				hoPtr.hoX=pt.x+hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY=pt.y+hoPtr.hoAdRunHeader.rhWindowY;
				MP_NoJump=false;
			}
			else
			{
				MP_Type=MPTYPE_FALL;
			}
			break;

		case 1:         // MPTYPE_CLIMB:
			// Verifie la presence d'un echelle sous les pieds
			if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB)==0x80000000)
			{
				// Si on monte, on positionne le sprite juste au dessus de l'echelle
				if (MP_YSpeed<0)
				{
					for (sY=0; sY<32; sY++)
					{
						if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB+sY)!=0x80000000)
						{
							hoPtr.hoY+=sY;
							break;
						}
					}
				}
				// Plus d'echelle, on arrete le mouvement
				MP_YSpeed=0;
			}
			// Si on appuie sur gauche / droite on repasse en mouvement walk
			if ((joyDir&12)!=0)
			{
				MP_Type=MPTYPE_WALK;
				MP_YSpeed=0;
			}
			break;

		case 4:         // MPTYPE_CROUCH:
			if ((joyDir&2)==0)
			{
				if (hoPtr.roa!=null)
				{
					if (hoPtr.roa.anim_Exist(CAnim.ANIMID_UNCROUCH))
					{
						MP_Type=MPTYPE_UNCROUCH;
						hoPtr.roc.rcAnim=CAnim.ANIMID_UNCROUCH;
						hoPtr.roa.animate();
						hoPtr.roa.raAnimRepeat=1;					// Force une seule boucle d'animation
						break;
					}
				}
				MP_Type=MPTYPE_WALK;
			}
			break;

		case 5:         // MPTYPE_UNCROUCH:
			if (hoPtr.roa!=null)
			{
				if (hoPtr.roa.raAnimNumberOfFrame==0)
				{
					MP_Type=MPTYPE_WALK;
				}
			}
			break;
		}

		// Gestion marche sur un autre sprite
		if (MP_Type==MPTYPE_WALK || MP_Type==MPTYPE_CROUCH || MP_Type==MPTYPE_UNCROUCH)
		{
			do
			{
				// Regarde l'objet en dessous
				short[] pOiColList = null;
				if ( hoPtr.hoOiList != null )
					pOiColList = hoPtr.hoOiList.oilColList;

				if (hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, pOiColList)==null) 
				{
					ArrayList<CObject> list=hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY+1, pOiColList);
					if (list!=null && list.size()==1) 
					{
						CObject pHo2=list.get(0);
						if (MP_ObjectUnder==null || MP_ObjectUnder!=pHo2)
						{
							if (hoPtr.hoOi!=pHo2.hoOi)
							{
								MP_ObjectUnder=pHo2;
								MP_XObjectUnder=pHo2.hoX;
								MP_YObjectUnder=pHo2.hoY;
								break;
							}
						}
						int dx=pHo2.hoX-MP_XObjectUnder;
						int dy=pHo2.hoY-MP_YObjectUnder;
						MP_XObjectUnder=pHo2.hoX;
						MP_YObjectUnder=pHo2.hoY;

						hoPtr.hoX+=dx;
						hoPtr.hoY+=dy;
						hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
						hoPtr.roc.rcChanged=true;							//; Sprite bouge!
						break;
					}
				}
				MP_ObjectUnder=null;
			}while(false);		
		}
		else
		{
			MP_ObjectUnder=null;
		}
	}
	void mpStopIt()
	{
		hoPtr.roc.rcSpeed=0;
		MP_XSpeed=0;
		MP_YSpeed=0;	
	}
	@Override
	public void stop()
	{
		MP_Bounce=0;

		// Est-ce le sprite courant?
		// -------------------------
		if (rmCollisionCount!=hoPtr.hoAdRunHeader.rh3CollisionCount) 
		{
			mpStopIt();
			return;
		}
		hoPtr.rom.rmMoveFlag=true;				// Le flag!
		int scrX=hoPtr.hoX-hoPtr.hoAdRunHeader.rhWindowX;
		int scrY=hoPtr.hoY-hoPtr.hoAdRunHeader.rhWindowY;
		int x,y, dir;

		// Qui est a l'origine de la collision? 
		// ------------------------------------
		switch (hoPtr.hoAdRunHeader.rhEvtProg.rhCurCode&0xFFFF0000)
		{
		case (-12<<16):         // CNDL_EXTOUTPLAYFIELD:
			// SORTIE DU TERRAIN : RECENTRE LE SPRITE
			// --------------------------------------
			x=hoPtr.hoX-hoPtr.hoImgXSpot;
		y=hoPtr.hoY-hoPtr.hoImgYSpot;
		dir=hoPtr.hoAdRunHeader.quadran_Out(x, y, x+hoPtr.hoImgWidth, y+hoPtr.hoImgHeight);

		x=hoPtr.hoX;
		y=hoPtr.hoY;
		if ((dir&CRun.BORDER_LEFT)!=0)
		{
			x=hoPtr.hoImgXSpot;
			MP_XSpeed=0;
			MP_NoJump=true;
		}
		if ((dir&CRun.BORDER_RIGHT)!=0)
		{
			x=hoPtr.hoAdRunHeader.rhLevelSx-hoPtr.hoImgWidth+hoPtr.hoImgXSpot;
			MP_XSpeed=0;
			MP_NoJump=true;
		}
		if ((dir&CRun.BORDER_TOP)!=0)
		{
			y=hoPtr.hoImgYSpot;
			MP_YSpeed=0;
			MP_NoJump=false;
		}
		if ((dir&CRun.BORDER_BOTTOM)!=0)
		{
			y=hoPtr.hoAdRunHeader.rhLevelSy-hoPtr.hoImgHeight+hoPtr.hoImgYSpot;
			MP_YSpeed=0;
			MP_NoJump=false;
		}
		hoPtr.hoX=x;
		hoPtr.hoY=y;
		if (MP_Type==MPTYPE_JUMP)
		{
			MP_Type=MPTYPE_FALL;
		}
		else
		{
			MP_Type=MPTYPE_WALK;
		}
		MP_JumpStopped=0;
		return;
		case (-13<<16):	    // CNDL_EXTCOLBACK:
		case (-14<<16):	    // CNDL_EXTCOLLISION:
			MP_NoJump=false;
		CPoint pt=new CPoint();
		if (MP_Type==MPTYPE_FALL)
		{
			mpApproachSprite(scrX, scrY, hoPtr.roc.rcOldX-hoPtr.hoAdRunHeader.rhWindowX, hoPtr.roc.rcOldY-hoPtr.hoAdRunHeader.rhWindowY, (short)MP_HTFOOT, CColMask.CM_TEST_PLATFORM, pt);			

			hoPtr.hoX=pt.x+hoPtr.hoAdRunHeader.rhWindowX;
			hoPtr.hoY=pt.y+hoPtr.hoAdRunHeader.rhWindowY;
			MP_Type=MPTYPE_WALK;
			hoPtr.roc.rcChanged=true;

			if (tst_SpritePosition(hoPtr.hoX, hoPtr.hoY+1, (short)0, CColMask.CM_TEST_PLATFORM, true))
			{
				hoPtr.roc.rcSpeed=0;
				MP_XSpeed=0;
			}
			else
			{
				MP_JumpStopped=0;
				hoPtr.roc.rcSpeed=Math.abs(MP_XSpeed/256);
				MP_YSpeed=0;	
			}
			return;
		}
		if (MP_Type==MPTYPE_WALK)
		{
			// Si on marche on essaye de monter sur l'obstacle
			if(mpApproachSprite(scrX, scrY, scrX, scrY-MP_HTFOOT, (short)0, CColMask.CM_TEST_PLATFORM, pt)) //roPtr.rom.MP_HTFOOT
			{
				// Pas de stop, on monte juste sur l'obstacle
				hoPtr.hoX=pt.x+hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY=pt.y+hoPtr.hoAdRunHeader.rhWindowY;
				hoPtr.roc.rcChanged=true;
				return;
			}
			// On essaye de positionner le sprite contre l'obstacle
			if(mpApproachSprite(scrX, scrY, hoPtr.roc.rcOldX-hoPtr.hoAdRunHeader.rhWindowX, hoPtr.roc.rcOldY-hoPtr.hoAdRunHeader.rhWindowY, (short)0, CColMask.CM_TEST_PLATFORM, pt))
			{
				hoPtr.hoX=pt.x+hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY=pt.y+hoPtr.hoAdRunHeader.rhWindowY;
				hoPtr.roc.rcChanged=true;
				mpStopIt();
				return;
			}
		}
		if (MP_Type==MPTYPE_JUMP)
		{
			// Si on marche on essaye de monter sur l'obstacle
			if(mpApproachSprite(scrX, scrY, scrX, scrY-MP_HTFOOT, (short)0, CColMask.CM_TEST_PLATFORM, pt))	//roPtr.rom.MP_HTFOOT
			{
				// Pas de stop, on monte juste sur l'obstacle
				hoPtr.hoX=pt.x+hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY=pt.y+hoPtr.hoAdRunHeader.rhWindowY;
				hoPtr.roc.rcChanged=true;
				return;
			}
			MP_JumpStopped=1;
			MP_XSpeed=0;	
		}
		if (MP_Type==MPTYPE_CLIMB)
		{
			// On essaye de positionner le sprite contre l'obstacle
			if(mpApproachSprite(scrX, scrY, hoPtr.roc.rcOldX-hoPtr.hoAdRunHeader.rhWindowX, hoPtr.roc.rcOldY-hoPtr.hoAdRunHeader.rhWindowY, (short)0, CColMask.CM_TEST_PLATFORM, pt))
			{
				hoPtr.hoX=pt.x+hoPtr.hoAdRunHeader.rhWindowX;
				hoPtr.hoY=pt.y+hoPtr.hoAdRunHeader.rhWindowY;
				hoPtr.roc.rcChanged=true;
				mpStopIt();
				return;
			}
		}
		// Essaye avec l'ancienne image
		hoPtr.roc.rcImage=hoPtr.roc.rcOldImage;
		hoPtr.roc.rcAngle=hoPtr.roc.rcOldAngle;
		if (tst_SpritePosition(hoPtr.hoX, hoPtr.hoY, (short)0, CColMask.CM_TEST_PLATFORM, true))
		{
			return;
		}

		// Rien ne marche, ancienne image, ancienne position
		hoPtr.hoX=hoPtr.roc.rcOldX;
		hoPtr.hoY=hoPtr.roc.rcOldY;
		hoPtr.roc.rcChanged=true;
		break;
		}
	}
	@Override
	public void start()
	{

	}
	@Override
	public void bounce()
	{
		stop();
	}
	@Override
	public void reverse()
	{        
	}
	@Override
	public void setXPosition(int x)
	{        
		if (hoPtr.hoX!=x)
		{
			hoPtr.hoX=x;
			hoPtr.rom.rmMoveFlag=true;
			hoPtr.roc.rcChanged=true;
			hoPtr.roc.rcCheckCollides=true;					//; Force la detection de collision
		}
	}
	@Override
	public void setYPosition(int y)
	{
		if (hoPtr.hoY!=y)
		{
			hoPtr.hoY=y;
			hoPtr.rom.rmMoveFlag=true;
			hoPtr.roc.rcChanged=true;
			hoPtr.roc.rcCheckCollides=true;					//; Force la detection de collision
		}
	}
	@Override
	public void setSpeed(int speed)
	{
		if (speed<0) 
			speed=0;
		if (speed>250) 
			speed=250;
		if (speed>hoPtr.roc.rcMaxSpeed)
		{
			speed=hoPtr.roc.rcMaxSpeed;
		}
		hoPtr.roc.rcSpeed=speed;
		MP_XSpeed=hoPtr.roc.rcSpeed*Cosinus32[hoPtr.roc.rcDir];
		MP_YSpeed=hoPtr.roc.rcSpeed*Sinus32[hoPtr.roc.rcDir];
		hoPtr.rom.rmMoveFlag=true;        
	}
	@Override
	public void setMaxSpeed(int speed)
	{
		if (speed<0) 
			speed=0;
		if (speed>250) 
			speed=250;
		hoPtr.roc.rcMaxSpeed=speed;
		speed<<=8;
		if (MP_XSpeed>speed)
			MP_XSpeed=speed;
		hoPtr.rom.rmMoveFlag=true;
	}
	@Override
	public void setGravity(int gravity)
	{
		MP_Gravity=gravity;
	}
	@Override
	public void setDir(int dir)
	{
		hoPtr.roc.rcDir=dir;
		MP_XSpeed=hoPtr.roc.rcSpeed*Cosinus32[dir];
		MP_YSpeed=hoPtr.roc.rcSpeed*Sinus32[dir];
	}

	//---------------------------------------------------------------------//
	//	Calculer les coordonnees des pieds et la taille du bas du sprite   //
	//---------------------------------------------------------------------//
	void calcMBFoot()
	{
		int height;
		int spotY;

		if (hoPtr.roc.rcImage>=0)
		{
			CImageInfo ifo;
			ifo=hoPtr.hoAdRunHeader.rhApp.imageBank.getImageInfoEx
					(hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY);
			if (ifo != null)
			{
				height = ifo.height;
				spotY = ifo.ySpot;
			}
			else
			{
				height = hoPtr.hoImgHeight;
				spotY = hoPtr.hoImgYSpot;
			}
		}
		else
		{
			height = hoPtr.hoImgHeight;
			spotY = hoPtr.hoImgYSpot;
		}
		MP_XMB=-hoPtr.hoAdRunHeader.rhWindowX;							//&; X ecran du point milieu bas (sous hot spot)
		MP_YMB=height-hoPtr.hoAdRunHeader.rhWindowY-spotY;		//; Y ecran du point milieu bas

		MP_HTFOOT=((height*2)+height)>>>3;		//; Hauteur des pieds
	}

	//-----------------------------------------------------//
	//		Tester s'il y a une echelle sous le joueur //
	//-----------------------------------------------------//
	//	In:		CX,DX = coordonnees		   //
	//							   //
	// Out:	Cy si echelle en (CX+lg/2, DX+ht)	   //
	// FS:DI pointe sur echelle collisionnationnee         //
	//-----------------------------------------------------//
	int check_Ladder(int nLayer, int x, int y)
	{
		CRect prc = hoPtr.hoAdRunHeader.y_GetLadderAt(nLayer, x, y);
		if ( prc!=null )
		{
			return prc.top;
		}
		return 0x80000000;
	}
	//-----------------------------------------------------------------------------//
	//	Collisions avec le decor d'un sprite en mouvement plateforme               //
	//-----------------------------------------------------------------------------//
	public void mpHandle_Background()
	{
		// TEST COLLISION AVEC ECHELLES
		// ----------------------------
		calcMBFoot();
		if (check_Ladder(hoPtr.hoLayer, hoPtr.hoX+MP_XMB, hoPtr.hoY+MP_YMB)!=0x80000000) 
			return;	//; Si echelle juste sous les pieds, pas de collision

		// TEST COLLISION AVEC OBSTACLES SEULEMENT
		// ---------------------------------------
		if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, (short)0, CColMask.CM_TEST_OBSTACLE)==0) // FRAROT
		{
			// TEST COLLISION AVEC LES PLATEFORMES
			// -----------------------------------
			// Si anim = saut & MP_YSpeed < 0, pas de test des plateformes.
			// ------------------------------------------------------------
			if (MP_Type==MPTYPE_JUMP && MP_YSpeed<0) 
				return;

			if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, (short)MP_HTFOOT, CColMask.CM_TEST_PLATFORM)==0) // FRAROT
				return; 
		}
		hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, (-13<<16)|((hoPtr.hoType)&0xFFFF) );	    // CNDL_EXTCOLBACK
	}
}
