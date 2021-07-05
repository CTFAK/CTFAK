//----------------------------------------------------------------------------------
//
// CMOVE : Classe de base des mouvements
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

namespace RuntimeXNA.Movements
{
    public class CMove
    {
        // Table de sinus/cosinus sur 32 angles en ",256"
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static int[] Cosinus32 =
        {
            256, 251, 236, 212, 181, 142, 97, 49,
            0, -49, -97, -142, -181, -212, -236, -251,
            -256, -251, -236, -212, -181, -142, -97, -49,
            0, 49, 97, 142, 181, 212, 236, 251
        };
        public static int[] Sinus32 =
        {
            0, -49, -97, -142, -181, -212, -236, -251,
            -256, -251, -236, -212, -181, -142, -97, -49,
            0, 49, 97, 142, 181, 212, 236, 251,
            256, 251, 236, 212, 181, 142, 97, 49
        };

        // Table d'acceleration reguliere de 0 a 100
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static short[] accelerators =
        {
            0x0002, 0x0003, 0x0004, 0x0006, 0x0008, 0x000a, 0x000c, 0x0010, 0x0014, 0x0018,
            0x0030, 0x0038, 0x0040, 0x0048, 0x0050, 0x0058, 0x0060, 0x0068, 0x0070, 0x0078,
            0x0090, 0x00A0, 0x00B0, 0x00c0, 0x00d0, 0x00e0, 0x00f0, 0x0100, 0x0110, 0x0120,
            0x0140, 0x0150, 0x0160, 0x0170, 0x0180, 0x0190, 0x01a0, 0x01b0, 0x01c0, 0x01e0,
            0x0200, 0x0220, 0x0230, 0x0250, 0x0270, 0x0280, 0x02a0, 0x02b0, 0x02d0, 0x02e0,
            0x0300, 0x0310, 0x0330, 0x0350, 0x0360, 0x0380, 0x03a0, 0x03b0, 0x03d0, 0x03e0,
            0x0400, 0x0460, 0x04c0, 0x0520, 0x05a0, 0x0600, 0x0660, 0x06c0, 0x0720, 0x07a0,
            0x0800, 0x08c0, 0x0980, 0x0a80, 0x0b40, 0x0c00, 0x0cc0, 0x0d80, 0x0e80, 0x0f40,
            0x1000, 0x1990, 0x1332, 0x1460, 0x1664, 0x1800, 0x1999, 0x1b32, 0x1cc6, 0x1e64,
            0x2000, 0x266c, 0x2d98, 0x3404, 0x3a70, 0x40dc, 0x4748, 0x4db4, 0x5400, 0x6400,
            0x6400
        };

        // Table: direction joystick . direction KNP
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static sbyte[] Joy2Dir =
        {
            -1, // 0000 Static
            8, // 0001
            24, // 0010
            -1, // 0011 Static
            16, // 0100
            12, // 0101
            20, // 0110
            16, // 0111
            0, // 1000
            4, // 1001
            28, // 1010
            0, // 1011
            -1, // 1100 Static
            8, // 1101
            24, // 1110
            -1				// 1111 Static
        };

        // Table des COS() / SIN() pour la recherche d'une direction   
        public static int[] CosSurSin32 =
        {
            2599, 0, 844, 31, 479, 30, 312, 29, 210, 28, 137, 27, 78, 26, 25, 25, 0, 24
        };
        public static int[] mvap_TableDirs =
        {
            0, -2, 0, 2, 0, -4, 0, 4, 0, -8, 0, 8, -4, 0, -8, 0, 0, 0, // 0
            -2, -2, 2, 2, -4, -4, 4, 4, -8, -8, 8, 8, -4, 4, -8, 8, 0, 0, // 16
            -2, 0, 2, 0, -4, 0, 4, 0, -8, 0, 8, 0, 0, 4, 0, 8, 0, 0, // 32
            -2, 2, 2, -2, -4, 4, 4, -4, -8, 8, 8, -8, 4, 4, 8, 8, 0, 0, // 48
            0, 2, 0, -2, 0, 4, 0, -4, 0, 8, 0, -8, 4, 0, 8, 0, 0, 0, // 64
            2, 2, -2, -2, 4, 4, -4, -4, 8, 8, -8, -8, 4, -4, 8, -8, 0, 0, // 80
            2, 0, -2, 0, 4, 0, -4, 0, 8, 0, -8, 0, 0, -4, 0, -8, 0, 0, // 96
            2, -2, -2, 2, 4, -4, -4, 4, 8, -8, -8, 8, -4, -4, -8, -8, 0, 0	    // 112
        };
        public const int MVTOPT_8DIR_STICK=0x01;

        public CObject hoPtr;
        public int rmAcc;						// Current acceleration
        public int rmDec;						// Current Decelaration 
        public short rmCollisionCount;			// Collision counter
        public int rmStopSpeed;				// If stopped: speed to take again
        public int rmAccValue;					// Acceleration calculation
        public int rmDecValue;					// Deceleration calculation
        public byte rmOpt;


        public bool newMake_Move(int speed, int angle)
        {
            hoPtr.hoAdRunHeader.rh3CollisionCount++;			//; Marque l'objet pour ce cycle
            rmCollisionCount = hoPtr.hoAdRunHeader.rh3CollisionCount;
            hoPtr.rom.rmMoveFlag = false;

            // Mode de gestion du mouvement
            // ----------------------------
            if (speed == 0)
            {
                // On ne bouge pas: appel des collisions directes!
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);		//; Appel les collisions
                return false;
            }

            // Fait le mouvement?
            // ~~~~~~~~~~~~~~~~~~
            int x, y;
            int speedShift;
            if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & CRunFrame.LEF_TIMEDMVTS) != 0)
            {
                speedShift = (int) (((double) speed) * hoPtr.hoAdRunHeader.rh4MvtTimerCoef * 32.0);
            }
            else
            {
                speedShift = speed << 5;
            }
            while (speedShift > 0x0800)
            {
                x = hoPtr.hoX << 16 | hoPtr.hoCalculX & 0x0000FFFF;
                y = hoPtr.hoY << 16 | hoPtr.hoCalculY & 0x0000FFFF;
                x += Cosinus32[angle] * 0x0800;
                y += Sinus32[angle] * 0x0800;
                hoPtr.hoCalculX = x & 0x0000FFFF;
                hoPtr.hoX = (int) ((short) (x >> 16));
                hoPtr.hoCalculY = y & 0x0000FFFF;
                hoPtr.hoY = (int) ((short) (y >> 16));

                if (hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr))		//; Appel les collisions
                {
                    return true;
                }
                if (hoPtr.rom.rmMoveFlag)
                {
                    break;
                }
                speedShift -= 0x0800;
            }
            ;
            if (!hoPtr.rom.rmMoveFlag)
            {
                x = hoPtr.hoX << 16 | hoPtr.hoCalculX & 0x0000FFFF;
                y = hoPtr.hoY << 16 | hoPtr.hoCalculY & 0x0000FFFF;
                x += Cosinus32[angle] * speedShift;
                y += Sinus32[angle] * speedShift;
                hoPtr.hoCalculX = x & 0x0000FFFF;
                hoPtr.hoX = (int) ((short) (x >> 16));
                hoPtr.hoCalculY = y & 0x0000FFFF;
                hoPtr.hoY = (int) ((short) (y >> 16));

                if (hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr))		//; Appel les collisions
                {
                    return true;
                }
            }
            hoPtr.roc.rcChanged = true;                                             //; Sprite bouge!
            if (!hoPtr.rom.rmMoveFlag)
            {
                hoPtr.hoAdRunHeader.rhVBLObjet = 0;			//; Stocke le VBL actuel
            }
            return hoPtr.rom.rmMoveFlag;
        }

        // Initialise le move at start
        public void moveAtStart(CMoveDef mvPtr)
        {
            if (mvPtr.mvMoveAtStart == 0)
            {
                stop();
            }
        }

        public int getAccelerator(int acceleration)
        {
            if (acceleration <= 100)
            {
                return accelerators[acceleration];
            }
            return acceleration << 8;
        }

        public void mv_Approach(bool bStickToObject)
        {
            if (bStickToObject)
            {
                mb_Approach(false);
                return;
            }

            bool flag = false;
            int code = hoPtr.hoAdRunHeader.rhEvtProg.rhCurCode >> 16;
            switch (code)
            {
                case -12:         // CNDL_EXTOUTPLAYFIELD:
                    // --------------------------------------------------------------------------------
                    // Sortie du terrain...
                    // --------------------------------------------------------------------------------
                    // Recadre le sprite dans le terrain
                    // ---------------------------------
                    int x = hoPtr.hoX - hoPtr.hoImgXSpot;
                    int y = hoPtr.hoY - hoPtr.hoImgYSpot;
                    int dir = hoPtr.hoAdRunHeader.quadran_Out(x, y, x + hoPtr.hoImgWidth, y + hoPtr.hoImgHeight);
                    x = hoPtr.hoX;
                    y = hoPtr.hoY;
                    if ((dir & CRun.BORDER_LEFT) != 0)
                    {
                        x = hoPtr.hoImgXSpot;
                    }
                    if ((dir & CRun.BORDER_RIGHT) != 0)
                    {
                        x = hoPtr.hoAdRunHeader.rhLevelSx - hoPtr.hoImgWidth + hoPtr.hoImgXSpot;
                    }
                    if ((dir & CRun.BORDER_TOP) != 0)
                    {
                        y = hoPtr.hoImgYSpot;
                    }
                    if ((dir & CRun.BORDER_BOTTOM) != 0)
                    {
                        y = hoPtr.hoAdRunHeader.rhLevelSy - hoPtr.hoImgHeight + hoPtr.hoImgYSpot;
                    }
                    hoPtr.hoX = x;
                    hoPtr.hoY = y;
                    return;
                case -13:	    // CNDL_EXTCOLBACK:
                case -14:	    // CNDL_EXTCOLLISION:
                    int index = (hoPtr.roc.rcDir >> 2) * 18;
                    do
                    {
                        if (tst_Position(hoPtr.hoX + mvap_TableDirs[index], hoPtr.hoY + mvap_TableDirs[index + 1], flag))
                        {
                            // Positionne le sprite au plus pres de la position
                            // ------------------------------------------------

                            hoPtr.hoX += mvap_TableDirs[index];
                            hoPtr.hoY += mvap_TableDirs[index + 1];
                            return;
                        }
                        index += 2;
                    } while (mvap_TableDirs[index] != 0 || mvap_TableDirs[index + 1] != 0);

                    // On arrive pas : ancienne position / ancienne animation!
                    // -------------------------------------------------------
                    if (flag == false)
                    {
                        hoPtr.hoX = hoPtr.roc.rcOldX;
                        hoPtr.hoY = hoPtr.roc.rcOldY;
                        hoPtr.roc.rcImage = hoPtr.roc.rcOldImage;
                        hoPtr.roc.rcAngle = hoPtr.roc.rcOldAngle;
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

        public void mb_Approach(bool flag)
        {
            int code = hoPtr.hoAdRunHeader.rhEvtProg.rhCurCode >> 16;
            switch (code)
            {
                case -12:         // CNDL_EXTOUTPLAYFIELD:
                    // --------------------------------------------------------------------------------
                    // Sortie du terrain...
                    // --------------------------------------------------------------------------------
                    // Recadre le sprite dans le terrain
                    // ---------------------------------
                    int x = hoPtr.hoX - hoPtr.hoImgXSpot;
                    int y = hoPtr.hoY - hoPtr.hoImgYSpot;
                    int dir = hoPtr.hoAdRunHeader.quadran_Out(x, y, x + hoPtr.hoImgWidth, y + hoPtr.hoImgHeight);
                    x = hoPtr.hoX;
                    y = hoPtr.hoY;
                    if ((dir & CRun.BORDER_LEFT) != 0)
                    {
                        x = hoPtr.hoImgXSpot;
                    }
                    if ((dir & CRun.BORDER_RIGHT) != 0)
                    {
                        x = hoPtr.hoAdRunHeader.rhLevelSx - hoPtr.hoImgWidth + hoPtr.hoImgXSpot;
                    }
                    if ((dir & CRun.BORDER_TOP) != 0)
                    {
                        y = hoPtr.hoImgYSpot;
                    }
                    if ((dir & CRun.BORDER_BOTTOM) != 0)
                    {
                        y = hoPtr.hoAdRunHeader.rhLevelSy - hoPtr.hoImgHeight + hoPtr.hoImgYSpot;
                    }
                    hoPtr.hoX = x;
                    hoPtr.hoY = y;
                    return;
                case -13:	    // CNDL_EXTCOLBACK:
                case -14:	    // CNDL_EXTCOLLISION:
                    // --------------------------------------------------------------------------------
                    // Contre un objet de decor...
                    // --------------------------------------------------------------------------------
                    // Essaye de sortir le sprite de la collision dans les 8 directions, avec la nouvelle image
                    // ----------------------------------------------------------------------------------------
                    CPoint pt = new CPoint();
                    if (mbApproachSprite(hoPtr.hoX, hoPtr.hoY, hoPtr.roc.rcOldX, hoPtr.roc.rcOldY, flag, pt))
                    {
                        hoPtr.hoX = pt.x;
                        hoPtr.hoY = pt.y;
                        return;
                    }
                    int index = (hoPtr.roc.rcDir >> 2) * 18;
                    do
                    {
                        if (tst_Position(hoPtr.hoX + mvap_TableDirs[index], hoPtr.hoY + mvap_TableDirs[index + 1], flag))
                        {
                            // Positionne le sprite au plus pres de la position
                            // ------------------------------------------------

                            hoPtr.hoX += mvap_TableDirs[index];
                            hoPtr.hoY += mvap_TableDirs[index + 1];
                            return;
                        }
                        index += 2;
                    } while (mvap_TableDirs[index] != 0 || mvap_TableDirs[index + 1] != 0);

                    // On arrive pas : ancienne position / ancienne animation!
                    // -------------------------------------------------------
                    if (flag == false)
                    {
                        hoPtr.hoX = hoPtr.roc.rcOldX;
                        hoPtr.hoY = hoPtr.roc.rcOldY;
                        hoPtr.roc.rcImage = hoPtr.roc.rcOldImage;
                        hoPtr.roc.rcAngle = hoPtr.roc.rcOldAngle;
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

        public bool tst_SpritePosition(int x, int y, short htFoot, short planCol, bool flag)
        {
            short sprOi;
            sprOi = -1;
            if (flag)
            {
                sprOi = hoPtr.hoOi;
            }
            CObjInfo oilPtr = hoPtr.hoOiList;

            // Verification de la bordure
            // --------------------------
            if ((oilPtr.oilLimitFlags & 0x000F) != 0)
            {
                int xx = x - hoPtr.hoImgXSpot;
                int yy = y - hoPtr.hoImgYSpot;
                if ((hoPtr.hoAdRunHeader.quadran_Out(xx, yy, xx + hoPtr.hoImgWidth, yy + hoPtr.hoImgHeight) & oilPtr.oilLimitFlags) != 0)
                {
                    return false;
                }
            }

            // Verification du decor
            // ---------------------
            if ((oilPtr.oilLimitFlags & 0x0010) != 0)
            {
                if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, htFoot, planCol) != 0) // FRAROT
                {
                    return false;
                }
            }

            // Verification des sprites
            // ------------------------
            if (oilPtr.oilLimitList == -1)
            {
                return true;
            }

            // Demande les collisions a cette position...
            CArrayList list = hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, oilPtr.oilColList);
            if (list == null)
            {
                return true;
            }

            // Exploration de la liste: recherche les sprites marque STOP pour ce sprite
            short[] lb = hoPtr.hoAdRunHeader.rhEvtProg.limitBuffer;
            int index;
            for (index = 0; index < list.size(); index++)
            {
                CObject hoSprite = (CObject)list.get(index);		//; Le sprite en collision
                short oi = hoSprite.hoOi;
                if (oi != sprOi)						//; Ne pas tenir compte de lui-meme?
                {
                    int ll;
                    for (ll = oilPtr.oilLimitList; lb[ll] >= 0; ll++)
                    {
                        if (lb[ll] == oi)
                        {
                            return false;
                        }
                    }
                }
            }
            // On peut aller
            // -------------
            return true;
        }

        public bool tst_Position(int x, int y, bool flag)
        {
            short sprOi;

            sprOi = -1;
            if (flag)
            {
                sprOi = hoPtr.hoOi;
            }
            CObjInfo oilPtr = hoPtr.hoOiList;

            // Verification de la bordure
            // --------------------------
            if ((oilPtr.oilLimitFlags & 0x000F) != 0)
            {
                int xx = x - hoPtr.hoImgXSpot;
                int yy = y - hoPtr.hoImgYSpot;
                int dir = hoPtr.hoAdRunHeader.quadran_Out(xx, yy, xx + hoPtr.hoImgWidth, yy + hoPtr.hoImgHeight);
                if ((dir & oilPtr.oilLimitFlags) != 0)
                {
                    return false;
                }
            }

            // Verification du decor
            // ---------------------
            if ((oilPtr.oilLimitFlags & 0x0010) != 0)
            {
                if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, (short) 0, CColMask.CM_TEST_PLATFORM) != 0) // FRAROT
                {
                    return false;
                }
            }

            // Verification des sprites
            // ------------------------
            if (oilPtr.oilLimitList == -1)
            {
                return true;
            }

            // Demande les collisions a cette position...
            CArrayList list = hoPtr.hoAdRunHeader.objectAllCol_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, x, y, oilPtr.oilColList);
            if (list == null)
            {
                return true;
            }

            // Exploration de la liste: recherche les sprites marque STOP pour ce sprite
            short[] lb = hoPtr.hoAdRunHeader.rhEvtProg.limitBuffer;
            int index;
            for (index = 0; index < list.size(); index++)
            {
                CObject hoSprite = (CObject)list.get(index);		//; Le sprite en collision
                short oi = hoSprite.hoOi;
                if (oi != sprOi)						//; Ne pas tenir compte de lui-meme?
                {
                    int ll;
                    for (ll = oilPtr.oilLimitList; lb[ll] >= 0; ll++)
                    {
                        if (lb[ll] == oi)
                        {
                            return false;
                        }
                    }
                }
            }

            // On peut aller
            // -------------
            return true;
        }

        public bool mpApproachSprite(int destX, int destY, int maxX, int maxY, short htFoot, short planCol, CPoint ptFinal)
        {
            int presX = destX;
            int presY = destY;
            int loinX = maxX;
            int loinY = maxY;

            int x = (presX + loinX) / 2;
            int y = (presY + loinY) / 2;
            int oldX, oldY;

            do
            {
                if (tst_SpritePosition(x + hoPtr.hoAdRunHeader.rhWindowX, y + hoPtr.hoAdRunHeader.rhWindowY, htFoot, planCol, false))
                {
                    // On peut y aller
                    loinX = x;
                    loinY = y;
                    oldX = x;
                    oldY = y;
                    x = (loinX + presX) / 2;
                    y = (loinY + presY) / 2;
                    if (x == oldX && y == oldY)
                    {
                        if (loinX != presX || loinY != presY)
                        {
                            if (tst_SpritePosition(presX + hoPtr.hoAdRunHeader.rhWindowX, presY + hoPtr.hoAdRunHeader.rhWindowY, htFoot, planCol, false))
                            {
                                x = presX;
                                y = presY;
                            }
                        }
                        ptFinal.x = x;
                        ptFinal.y = y;
                        return true;
                    }
                }
                else
                {
                    // On ne peut pas y aller
                    presX = x;
                    presY = y;
                    oldX = x;
                    oldY = y;
                    x = (loinX + presX) / 2;
                    y = (loinY + presY) / 2;
                    if (x == oldX && y == oldY)
                    {
                        if (loinX != presX || loinY != presY)
                        {
                            if (tst_SpritePosition(loinX + hoPtr.hoAdRunHeader.rhWindowX, loinY + hoPtr.hoAdRunHeader.rhWindowY, htFoot, planCol, false))
                            {
                                ptFinal.x = loinX;
                                ptFinal.y = loinY;
                                return true;
                            }
                        }
                        ptFinal.x = x;
                        ptFinal.y = y;
                        return false;
                    }
                }
            } while (true);
        }

        bool mbApproachSprite(int destX, int destY, int maxX, int maxY, bool flag, CPoint ptFinal)
        {
            int presX = destX;
            int presY = destY;
            int loinX = maxX;
            int loinY = maxY;

            int x = (presX + loinX) / 2;
            int y = (presY + loinY) / 2;
            int oldX, oldY;

            do
            {
                if (tst_Position(x, y, flag))
                {
                    // On peut y aller
                    loinX = x;
                    loinY = y;
                    oldX = x;
                    oldY = y;
                    x = (loinX + presX) / 2;
                    y = (loinY + presY) / 2;
                    if (x == oldX && y == oldY)
                    {
                        if (loinX != presX || loinY != presY)
                        {
                            if (tst_Position(presX, presY, flag))
                            {
                                x = presX;
                                y = presY;
                            }
                        }
                        ptFinal.x = x;
                        ptFinal.y = y;
                        return true;
                    }
                }
                else
                {
                    // On ne peut pas y aller
                    presX = x;
                    presY = y;
                    oldX = x;
                    oldY = y;
                    x = (loinX + presX) / 2;
                    y = (loinY + presY) / 2;
                    if (x == oldX && y == oldY)
                    {
                        if (loinX != presX || loinY != presY)
                        {
                            if (tst_Position(loinX, loinY, flag))
                            {
                                ptFinal.x = loinX;
                                ptFinal.y = loinY;
                                return true;
                            }
                        }
                        ptFinal.x = x;
                        ptFinal.y = y;
                        return false;
                    }
                }
            } while (true);
        }

        public static int getDeltaX(int pente, int angle)
        {
            return (pente * Cosinus32[angle]) / 256;	//; Fois cosinus-> penteX
        }

        public static int getDeltaY(int pente, int angle)
        {
            return (pente * Sinus32[angle]) / 256;		//; Fois sinus-> penteY
        }

        // Changement acceleration / deceleration
        public void setAcc(int acc)
        {
            if (acc > 250)
            {
                acc = 250;
            }
            if (acc < 0)
            {
                acc = 0;
            }
            rmAcc = acc;
            rmAccValue = getAccelerator(acc);
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                mvt.movement.setAcc(acc);
            }
        }

        public void setDec(int dec)
        {
            if (dec > 250)
            {
                dec = 250;
            }
            if (dec < 0)
            {
                dec = 0;
            }
            rmDec = dec;
            rmDecValue = getAccelerator(dec);
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                mvt.movement.setDec(dec);
            }
        }

        public void setRotSpeed(int speed)
        {
            if (speed > 250)
            {
                speed = 250;
            }
            if (speed < 0)
            {
                speed = 0;
            }
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_RACE)
            {
                CMoveRace mRace = (CMoveRace)this;
                mRace.MRSetRotSpeed(speed);
            }
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                mvt.movement.setRotSpeed(speed);
            }
        }

        public void set8Dirs(int dirs)
        {
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_GENERIC)
            {
                CMoveGeneric mGeneric = (CMoveGeneric)this;
                mGeneric.set8Dir(dirs);
            }
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                mvt.movement.set8Dirs(dirs);
            }
        }

        public void setGravity(int gravity)
        {
            if (gravity > 250)
            {
                gravity = 250;
            }
            if (gravity < 0)
            {
                gravity = 0;
            }
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_PLATFORM)
            {
                CMovePlatform mPlatform = (CMovePlatform)this;
                mPlatform.MPSetGravity(gravity);
            }
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                mvt.movement.setGravity(gravity);
            }
        }

        public int getSpeed()
        {
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                return mvt.movement.getSpeed();
            }
            return hoPtr.roc.rcSpeed;
        }

        public int getAcc()
        {
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                return mvt.movement.getAcceleration();
            }
            return rmAcc;
        }

        public int getDec()
        {
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                return mvt.movement.getDeceleration();
            }
            return rmDec;
        }

        public int getGravity()
        {
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_PLATFORM)
            {
                CMovePlatform mp = (CMovePlatform)this;
                return mp.MP_Gravity;
            }
            if (hoPtr.roc.rcMovementType == CMoveDef.MVTYPE_EXT)
            {
                CMoveExtension mvt = (CMoveExtension)this;
                return mvt.movement.getGravity();
            }
            return 0;
        }

        public virtual void init(CObject hoPtr, CMoveDef mvPtr) { }

        public virtual void kill() { }

        public virtual void move() { }

        public virtual void stop() { }

        public virtual void start() { }

        public virtual void bounce() { }

        public virtual void reverse() { }

        public virtual void setXPosition(int x) { }

        public virtual void setYPosition(int u) { }

        public virtual void setSpeed(int speed) { }

        public virtual void setMaxSpeed(int speed) { }

        public virtual void setDir(int dir) { }


    }
}
