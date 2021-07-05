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
// CMOVESTATIC : Mouvement statique
//
//----------------------------------------------------------------------------------
package Movements;

import Animations.CAnim;
import Extensions.CRunBox2DBasePosAndAngle;
import Objects.CObject;
import RunLoop.CRun;
import RunLoop.CRunBaseParent;
import RunLoop.CRunMBase;
import Sprites.CRSpr;
import Sprites.CSprite;

import com.badlogic.gdx.physics.box2d.Body;

public class CMoveBullet extends CMove
{
	public boolean MBul_Wait=false;
	public CObject MBul_ShootObject=null;
	public CRunMBase MBul_MBase=null;
	public Body MBul_Body=null;
	
	public CRunBox2DBasePosAndAngle m_posAndAngle = new CRunBox2DBasePosAndAngle();
	
	@Override
	public void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr=ho;
		if (hoPtr.roc.rcSprite!=null)						// Est-il active?
				{
			hoPtr.roc.rcSprite.setSpriteColFlag(0);		//; Pas dans les collisions
				}
		if ( hoPtr.ros!=null )
		{
			hoPtr.ros.rsFlags&=~CRSpr.RSFLAG_VISIBLE;
			hoPtr.ros.obHide();									//; Cache pour le moment
		}
		MBul_Wait=true;
		hoPtr.hoCalculX=0;
		hoPtr.hoCalculY=0;
		if (hoPtr.roa!=null)
		{
			hoPtr.roa.init_Animation(CAnim.ANIMID_WALK);
		}
		hoPtr.roc.rcSpeed=0;
		hoPtr.roc.rcCheckCollides=true;			//; Force la detection de collision
		hoPtr.roc.rcChanged=true;
	}
	public void init2(CObject parent)
	{
		hoPtr.roc.rcMaxSpeed=hoPtr.roc.rcSpeed;
		hoPtr.roc.rcMinSpeed=hoPtr.roc.rcSpeed;
		MBul_ShootObject=parent;			// Met l'objet source
	}
	@Override
	public void kill()
	{
		if (MBul_Body!=null)
		{
			if(hoPtr.hoAdRunHeader.rh4Box2DObject) {
				CRunBaseParent pBase=hoPtr.hoAdRunHeader.rh4Box2DBase;
				pBase.rDestroyBody(MBul_Body);
			}
			MBul_Body=null;
		}
		if (MBul_MBase!=null)
		{
			MBul_MBase=null;
		}
	}
	@Override
	public void move()
	{
		if (MBul_Wait)
		{
			// Attend la fin du mouvement d'origine
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (MBul_ShootObject.roa!=null)
			{
				if (MBul_ShootObject.roa.raAnimOn==CAnim.ANIMID_SHOOT)
					return;
			}
			startBullet();
		}

		// Fait fonctionner la balle
		// ~~~~~~~~~~~~~~~~~~~~~~~~~
		if (hoPtr.roa!=null)
		{
			hoPtr.roa.animate();
		}
		
		if(MBul_Body != null) {
			CRun rhPtr = hoPtr.hoAdRunHeader;
			if (rhPtr.rh4Box2DObject && rhPtr.rh4Box2DBase!=null)
			{
				CRunBaseParent pBase=hoPtr.hoAdRunHeader.rh4Box2DBase;
				pBase.rGetBodyPosition(MBul_Body, m_posAndAngle);
				hoPtr.hoX=m_posAndAngle.x;
				hoPtr.hoY=m_posAndAngle.y;
		        hoPtr.roc.rcAngle = m_posAndAngle.angle;
		        //hoPtr.roc.rcDir=(int)Math.floor((m_posAndAngle.angle%360)/11.25);
		        hoPtr.roc.rcDir=0;
				hoPtr.roc.rcChanged=true;				
			}
		}
		else
		{
			
			newMake_Move(hoPtr.roc.rcSpeed, hoPtr.roc.rcDir);
		}
		
		// Verifie que la balle ne sort pas du terrain (assez loin des bords!)
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (hoPtr.hoX<-64 || hoPtr.hoX>hoPtr.hoAdRunHeader.rhLevelSx+64 || hoPtr.hoY<-64 || hoPtr.hoY>hoPtr.hoAdRunHeader.rhLevelSy+64)
		{
			// Detruit la balle, sans explosion!
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			hoPtr.hoCallRoutine=false;
			hoPtr.hoAdRunHeader.destroy_Add(hoPtr.hoNumber);
		}
		if (hoPtr.roc.rcCheckCollides)			//; Faut-il tester les collisions?
		{
			hoPtr.roc.rcCheckCollides=false;		//; Va tester une fois!
			hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
		}
	}
	public void startBullet()
	{
		// Fait demarrer la balle
		// ~~~~~~~~~~~~~~~~~~~~~~
		if (hoPtr.roc.rcSprite!=null)				//; Est-il active?
		{
			hoPtr.roc.rcSprite.setSpriteColFlag(CSprite.SF_RAMBO);
		}
		if ( hoPtr.ros!=null )
		{
			hoPtr.ros.rsFlags|=CRSpr.RSFLAG_VISIBLE;
			hoPtr.ros.obShow();					//; Plus cache
		}
		CRun rhPtr = hoPtr.hoAdRunHeader;
		if (rhPtr.rh4Box2DObject && rhPtr.rh4Box2DBase!=null)
		{
			CObject hoParent=MBul_ShootObject;
			CRunMBase pMovement=null;
			pMovement=rhPtr.GetMBase(hoParent);
			if (pMovement!=null)
			{
				CRunMBase pMBase=new CRunMBase();
				MBul_MBase=pMBase;
				pMBase.InitBase(hoPtr, CRunMBase.MTYPE_OBJECT);
				pMBase.m_identifier=rhPtr.rh4Box2DBase.identifier;
				MBul_Body=rhPtr.rh4Box2DBase.rCreateBullet((float)pMovement.m_currentAngle, hoPtr.roc.rcSpeed, pMBase);
				pMBase.m_body = MBul_Body;
				if (MBul_Body==null)
				{
					MBul_MBase=null;
				}
			}
		}
		MBul_Wait=false; 					//; On y va!
		MBul_ShootObject=null;
	}

	@Override
	public void stop()
	{

	}
	@Override
	public void start()
	{

	}
	@Override
	public void bounce()
	{

	}
	@Override
	public void setSpeed(int speed)
	{

	}
	@Override
	public void setMaxSpeed(int speed)
	{

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
	public void setDir(int dir)
	{
	}


}
