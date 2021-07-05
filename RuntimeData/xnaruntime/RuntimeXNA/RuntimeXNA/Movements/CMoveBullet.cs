//----------------------------------------------------------------------------------
//
// CMOVEBULLET
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;
using RuntimeXNA.Application;
using RuntimeXNA.Animations;

namespace RuntimeXNA.Movements
{
    class CMoveBullet : CMove
    {
        public bool MBul_Wait = false;
        public CObject MBul_ShootObject = null;

        public override void init(CObject ho, CMoveDef mvPtr)
        {
            hoPtr = ho;
            if (hoPtr.roc.rcSprite != null)						// Est-il active?
            {
                hoPtr.roc.rcSprite.setSpriteColFlag(0);		//; Pas dans les collisions
            }
            if (hoPtr.ros != null)
            {
                hoPtr.ros.rsFlags &= ~CRSpr.RSFLAG_VISIBLE;
                hoPtr.ros.obHide();									//; Cache pour le moment
            }
            MBul_Wait = true;
            hoPtr.hoCalculX = 0;
            hoPtr.hoCalculY = 0;
            if (hoPtr.roa != null)
            {
                hoPtr.roa.init_Animation(CAnim.ANIMID_WALK);
            }
            hoPtr.roc.rcSpeed = 0;
            hoPtr.roc.rcCheckCollides = true;			//; Force la detection de collision
            hoPtr.roc.rcChanged = true;
        }
        public void init2(CObject parent)
        {
            hoPtr.roc.rcMaxSpeed = hoPtr.roc.rcSpeed;
            hoPtr.roc.rcMinSpeed = hoPtr.roc.rcSpeed;
            MBul_ShootObject = parent;			// Met l'objet source	
        }
        public override void move()
        {
            if (MBul_Wait)
            {
                // Attend la fin du mouvement d'origine
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (MBul_ShootObject.roa != null)
                {
                    if (MBul_ShootObject.roa.raAnimOn == CAnim.ANIMID_SHOOT)
                        return;
                }
                startBullet();
            }

            // Fait fonctionner la balle
            // ~~~~~~~~~~~~~~~~~~~~~~~~~
            if (hoPtr.roa != null)
            {
                hoPtr.roa.animate();
                if (CRun.bMoveChanged)
                    return;
            }
            newMake_Move(hoPtr.roc.rcSpeed, hoPtr.roc.rcDir);
            if (CRun.bMoveChanged)
                return;

            // Verifie que la balle ne sort pas du terrain (assez loin des bords!)
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (hoPtr.hoX < -64 || hoPtr.hoX > hoPtr.hoAdRunHeader.rhLevelSx + 64 || hoPtr.hoY < -64 || hoPtr.hoY > hoPtr.hoAdRunHeader.rhLevelSy + 64)
            {
                // Detruit la balle, sans explosion!
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                hoPtr.hoCallRoutine = false;
                hoPtr.hoAdRunHeader.destroy_Add(hoPtr.hoNumber);
            }
            if (hoPtr.roc.rcCheckCollides)			//; Faut-il tester les collisions?
            {
                hoPtr.roc.rcCheckCollides = false;		//; Va tester une fois!
                hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
            }
        }
        public void startBullet()
        {
            // Fait demarrer la balle
            // ~~~~~~~~~~~~~~~~~~~~~~
            if (hoPtr.roc.rcSprite != null)				//; Est-il active?
            {
                hoPtr.roc.rcSprite.setSpriteColFlag(CSprite.SF_RAMBO);
            }
            if (hoPtr.ros != null)
            {
                hoPtr.ros.rsFlags |= CRSpr.RSFLAG_VISIBLE;
                hoPtr.ros.obShow();					//; Plus cache
            }
            MBul_Wait = false; 					//; On y va!
            MBul_ShootObject = null;
        }

        public override void setXPosition(int x)
        {
            if (hoPtr.hoX != x)
            {
                hoPtr.hoX = x;
                hoPtr.rom.rmMoveFlag = true;
                hoPtr.roc.rcChanged = true;
                hoPtr.roc.rcCheckCollides = true;					//; Force la detection de collision
            }
        }
        public override void setYPosition(int y)
        {
            if (hoPtr.hoY != y)
            {
                hoPtr.hoY = y;
                hoPtr.rom.rmMoveFlag = true;
                hoPtr.roc.rcChanged = true;
                hoPtr.roc.rcCheckCollides = true;					//; Force la detection de collision
            }
        }


    }
}
