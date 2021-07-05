//----------------------------------------------------------------------------------
//
// CMOVEDEFLIST : liste des mouvements d'un objet'
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    public  class CMoveDefList
    {
        public int nMovements;
        public CMoveDef[] moveList;

        public void load(CFile file)
        {
            int debut = file.getFilePointer();
            nMovements = file.readAInt();
            moveList = new CMoveDef[nMovements];
            int n;
            for (n = 0; n < nMovements; n++)
            {
                file.seek(debut + 4 + 16 * n);              // sizeof(MvtHrd)

                // Lis les donnée
                int moduleNameOffset = file.readAInt();
                int mvtID = file.readAInt();
                int dataOffset = file.readAInt();
                int dataLength = file.readAInt();

                // Lis le debut du header movement
                file.seek(debut + dataOffset);
                short control = file.readAShort();
                short type = file.readAShort();
                byte move = file.readByte();
                byte opt=file.readByte();
                file.skipBytes(2);
                int dirAtStart = file.readAInt();
                switch (type)
                {
                    // MVTYPE_STATIC
                    case 0:
                        moveList[n] = new CMoveDefStatic();
                        break;
                    // MVTYPE_MOUSE
                    case 1:
                        moveList[n] = new CMoveDefMouse();
                        break;
                    // MVTYPE_RACE
                    case 2:
                        moveList[n] = new CMoveDefRace();
                        break;
                    // MVTYPE_GENERIC
                    case 3:
                        moveList[n] = new CMoveDefGeneric();
                        break;
                    // MVTYPE_BALL
                    case 4:
                        moveList[n] = new CMoveDefBall();
                        break;
                    // MVTYPE_TAPED
                    case 5:
                        moveList[n] = new CMoveDefPath();
                        break;
                    // MVTYPE_PLATFORM
                    case 9:
                        moveList[n] = new CMoveDefPlatform();
                        break;
                    // MVTYPE_EXT				
                    case 14:
                        moveList[n] = new CMoveDefExtension();
                        break;
                }
                moveList[n].setData(type, control, move, dirAtStart, opt);
                moveList[n].load(file, dataLength - 12);
                if (type == 14)       // MVTYPE_EXT
                {
                    file.seek(debut + moduleNameOffset);
                    String name = file.readAString();
                    name = name.Substring(0, name.Length - 4);
                    ((CMoveDefExtension) moveList[n]).setModuleName(name, mvtID);
                }
            }
        }

    }
}
