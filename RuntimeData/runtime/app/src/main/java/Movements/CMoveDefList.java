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
// CMOVEDEFLIST : liste des mouvements d'un objet'
//
//----------------------------------------------------------------------------------
package Movements;

import Services.CFile;

public class CMoveDefList 
{   
    public int nMovements;
    public CMoveDef moveList[];
    
    public CMoveDefList() 
    {
    }
   
    public void load(CFile file) 
    {
        long debut=file.getFilePointer();
        nMovements=file.readAInt();
        moveList=new CMoveDef[nMovements];
        int n;
        for (n=0; n<nMovements; n++)
        {
            file.seek(debut+4+16*n);              // sizeof(MvtHrd)
            
            // Lis les donnee
            int moduleNameOffset=file.readAInt();
            int mvtID=file.readAInt();
            int dataOffset=file.readAInt();
            int dataLength=file.readAInt();
        
            // Lis le debut du header movement
            file.seek(debut + dataOffset);
            short control = file.readAShort();
            short type = file.readAShort();
            byte move = file.readByte();
            byte mo = file.readByte();
            file.skipBytes(2);
            int dirAtStart = file.readAInt();
            switch (type)
            {
                // MVTYPE_STATIC
                case 0:
                    moveList[n]=new CMoveDefStatic();
                    break;
                // MVTYPE_MOUSE
               case 1:
                    moveList[n]=new CMoveDefMouse();
                    break;
                // MVTYPE_RACE
                case 2:
                    moveList[n]=new CMoveDefRace();
                    break;
                // MVTYPE_GENERIC
                case 3:
                    moveList[n]=new CMoveDefGeneric();
                    break;
                // MVTYPE_BALL
                case 4:
                    moveList[n]=new CMoveDefBall();
                    break;
                // MVTYPE_TAPED
                case 5:
                    moveList[n]=new CMoveDefPath();
                    break;
                // MVTYPE_PLATFORM
                case 9:
                    moveList[n]=new CMoveDefPlatform();
                    break;
                // MVTYPE_EXT				
                case 14:
                    moveList[n]=new CMoveDefExtension();
                    break;
            }
            moveList[n].setData(type, control, move, dirAtStart, mo);
            moveList[n].load(file, dataLength - 12);
            if (type == 14)       // MVTYPE_EXT
            {
                file.seek(debut+moduleNameOffset);
                String name=file.readAString();
		name=name.substring(0, name.length()-4);
                ((CMoveDefExtension)moveList[n]).setModuleName(name, mvtID);
            }
        }
    }
}
