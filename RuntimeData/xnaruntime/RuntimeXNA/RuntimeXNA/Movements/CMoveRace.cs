//----------------------------------------------------------------------------------
//
// CMOVERACE : Mouvement voiture de course
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
    class CMoveRace : CMove
    {
        public int MR_Bounce;
        public int MR_BounceMu;
        public int MR_Speed;
        public int MR_RotSpeed;
        public int MR_RotCpt;
        public int MR_RotPos;
        public int MR_RotMask;
        public int MR_OkReverse;
        public int MR_OldJoy;
        public int MR_LastBounce;

        // Masque des directions pour le nombre autorise movement race
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static uint[] RaceMask =
        {
            0xFFFFFFF8,
            0xFFFFFFFC,
            0xFFFFFFFE,
            0xFFFFFFFF
        };

        public override void init(CObject ho, CMoveDef mvPtr)
        {
            hoPtr = ho;

            CMoveDefRace mrPtr = (CMoveDefRace)mvPtr;

            // Vitesse / accelerateurs
            MR_Speed = 0;
            hoPtr.roc.rcSpeed = 0;
            MR_Bounce = 0;
            MR_LastBounce = -1;
            hoPtr.roc.rcPlayer = mrPtr.mvControl;
            rmAcc = mrPtr.mrAcc;
            rmAccValue = getAccelerator(mrPtr.mrAcc);
            rmDec = mrPtr.mrDec;
            rmDecValue = getAccelerator(mrPtr.mrDec);
            hoPtr.roc.rcMaxSpeed = mrPtr.mrSpeed;
            hoPtr.roc.rcMinSpeed = 0;
            MR_BounceMu = mrPtr.mrBounceMult;
            MR_OkReverse = mrPtr.mrOkReverse;
            hoPtr.rom.rmReverse = 0;
            MR_OldJoy = 0;
            rmOpt = mrPtr.mvOpt;

            // Rotations
            MR_RotMask = (int)RaceMask[mrPtr.mrAngles];
            MR_RotSpeed = mrPtr.mrRot;
            MR_RotCpt = 0;
            MR_RotPos = hoPtr.roc.rcDir;
            hoPtr.hoCalculX = 0;
            hoPtr.hoCalculY = 0;
            moveAtStart(mvPtr);

            hoPtr.roc.rcChanged = true;
        }

        public override void move()
        {
            int j;
            int add, accel, speed, dir, speed8;
            int dSpeed;

            hoPtr.hoAdRunHeader.rhVBLObjet = 1;

            if (MR_Bounce == 0)
            {
                hoPtr.rom.rmBouncing = false;								//; Gestion flag bouncing...

                j = hoPtr.hoAdRunHeader.rhPlayer[hoPtr.roc.rcPlayer - 1] & 0x0F;

                // Gestion de la direction
                // ~~~~~~~~~~~~~~~~~~~~~~~
                add = 0;
                if ((j & 0x08) != 0)
                {
                    add = -1;
                }
                if ((j & 0x04) != 0)
                {
                    add = 1;
                }
                if (add != 0)
                {
                    dSpeed = MR_RotSpeed;
                    if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & CRunFrame.LEF_TIMEDMVTS) != 0)
                    {
                        dSpeed = (int)(((double)dSpeed) * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
                    }
                    MR_RotCpt += dSpeed;
                    while (MR_RotCpt > 100)
                    {
                        MR_RotCpt -= 100;
                        MR_RotPos += add;
                        MR_RotPos &= 31;
                        hoPtr.roc.rcDir = MR_RotPos & MR_RotMask;
                    }
                    ;
                    hoPtr.roc.rcChanged = true;                                             //; Sprite bouge!
                }

                // Gestion de l'acceleration / ralentissement
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                accel = 0;
                if (hoPtr.rom.rmReverse != 0)
                {
                    if ((j & 0x01) != 0)
                    {
                        accel = 1;
                    }
                    if ((j & 0x02) != 0)
                    {
                        accel = 2;
                    }
                }
                else
                {
                    if ((j & 0x01) != 0)
                    {
                        accel = 2;
                    }
                    if ((j & 0x02) != 0)
                    {
                        accel = 1;
                    }
                }
                speed = MR_Speed;
                while (true)
                {
                    if ((accel & 1) != 0)
                    {
                        // Ralenti
                        if (MR_Speed == 0)
                        {
                            if (MR_OkReverse == 0)
                            {
                                break;
                            }
                            if ((MR_OldJoy & 0x03) != 0)
                            {
                                break;
                            }
                            hoPtr.rom.rmReverse ^= 1;
                            dSpeed = rmAccValue;
                            if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & CRunFrame.LEF_TIMEDMVTS) != 0)
                            {
                                dSpeed = (int)(((double)dSpeed) * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
                            }
                            speed += dSpeed;
                            speed8 = speed >> 8;
                            if (speed8 > hoPtr.roc.rcMaxSpeed)
                            {
                                speed = hoPtr.roc.rcMaxSpeed << 8;
                                MR_Speed = speed;
                            }
                            MR_Speed = speed;
                            break;
                        }
                        dSpeed = rmDecValue;
                        if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & CRunFrame.LEF_TIMEDMVTS) != 0)
                        {
                            dSpeed = (int)(((double)dSpeed) * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
                        }
                        speed -= dSpeed;
                        if (speed < 0)
                        {
                            speed = 0;
                        }
                        MR_Speed = speed;
                    }
                    else if ((accel & 2) != 0)
                    {
                        // Accelere
                        dSpeed = rmAccValue;
                        if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & CRunFrame.LEF_TIMEDMVTS) != 0)
                        {
                            dSpeed = (int)(((double)dSpeed) * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
                        }
                        speed += dSpeed;
                        speed8 = speed >> 8;
                        if (speed8 > hoPtr.roc.rcMaxSpeed)
                        {
                            speed = hoPtr.roc.rcMaxSpeed << 8;
                            MR_Speed = speed;
                        }
                        MR_Speed = speed;
                    }
                    break;
                }
                ;
                MR_OldJoy = j;

                // Fait les animations
                // ~~~~~~~~~~~~~~~~~~~
                hoPtr.roc.rcSpeed = MR_Speed >> 8;
                hoPtr.roc.rcAnim = CAnim.ANIMID_WALK;
                if (hoPtr.roa != null)
                {
                    hoPtr.roa.animate();
                    if (CRun.bMoveChanged)
                        return;
                }

                // Fait le mouvement
                //; ~~~~~~~~~~~~~~~~~
                dir = hoPtr.roc.rcDir;
                if (hoPtr.rom.rmReverse != 0)
                {
                    dir += 16;
                    dir &= 31;
                }
                if (newMake_Move(hoPtr.roc.rcSpeed, dir) == false)
                {
                    return;			// Fait le mouvement
                }
                if (CRun.bMoveChanged)
                    return;
            }

            // Fait rebondir
            // ~~~~~~~~~~~~~
            do
            {
                if (CRun.bMoveChanged)
                    return;
                if (MR_Bounce == 0)
                {
                    break;					//; Passe en mode rebond?
                }
                if (hoPtr.hoAdRunHeader.rhVBLObjet == 0)
                {
                    break;						//; Encore des VBL?
                }
                speed = MR_Speed;
                speed -= rmDecValue;
                if (speed <= 0)
                {
                    MR_Speed = 0;							//; Stop!
                    MR_Bounce = 0;
                    break;
                }
                MR_Speed = speed;							//; Et stocke
                speed >>= 8;
                dir = hoPtr.roc.rcDir;								//; Direction du rebond
                if (MR_Bounce != 0)
                {
                    dir += 16;
                    dir &= 31;
                }
            } while (newMake_Move(speed, dir));
        }

        public override void stop()
        {
            MR_Bounce = 0;
            MR_Speed = 0;
            hoPtr.rom.rmReverse = 0;								//; Plus de marche arriere
            if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)		//; C'est le sprite courant?
            {
                // Le sprite entre dans quelque chose...
                mv_Approach((rmOpt & MVTOPT_8DIR_STICK) != 0);								//; On approche au maximum, sans toucher a la vitesse
                hoPtr.rom.rmMoveFlag = true;
            }
        }

        public override void start()
        {
            rmStopSpeed = 0;
            hoPtr.rom.rmMoveFlag = true;					// Le flag!
        }

        public override void bounce()
        {
            if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)		//; C'est le sprite courant?
            {
                mv_Approach((rmOpt & MVTOPT_8DIR_STICK) != 0);
            }
            if (hoPtr.hoAdRunHeader.rhLoopCount != MR_LastBounce)				//; Un seul bounce a chaque cycle
            {
                MR_Bounce = hoPtr.rom.rmReverse;				//; Initialise le rebond dans la bonne direction
                hoPtr.rom.rmReverse = 0;									//; Plus de marche arriere
                MR_Bounce++;
                if (MR_Bounce >= 16)							//; Securite si bloque
                {
                    stop();
                    return;
                }
                hoPtr.rom.rmMoveFlag = true;
                hoPtr.rom.rmBouncing = true;								//; Pour les evenements
            }
        }

        public override void setSpeed(int speed)
        {
            if (speed < 0)
            {
                speed = 0;
            }
            if (speed > 250)
            {
                speed = 250;
            }
            if (speed > hoPtr.roc.rcMaxSpeed)
            {
                speed = hoPtr.roc.rcMaxSpeed;
            }
            speed <<= 8;
            MR_Speed = speed;
            hoPtr.rom.rmMoveFlag = true;
        }

        public override void setMaxSpeed(int speed)
        {
            if (speed < 0)
            {
                speed = 0;
            }
            if (speed > 250)
            {
                speed = 250;
            }
            hoPtr.roc.rcMaxSpeed = speed;
            speed <<= 8;
            if (MR_Speed > speed)
            {
                MR_Speed = speed;
            }
            hoPtr.rom.rmMoveFlag = true;
        }

        public void MRSetRotSpeed(int speed)
        {
            MR_RotSpeed = speed;
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

        public override void setDir(int dir)
        {
            MR_RotPos = dir;
            hoPtr.roc.rcDir = dir & MR_RotMask;
        }


    }
}
