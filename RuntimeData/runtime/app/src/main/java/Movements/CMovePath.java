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
// CMOVEPATH : Mouvement enregistre
//
//----------------------------------------------------------------------------------
package Movements;

import Animations.CAnim;
import Application.CRunFrame;
import Objects.CObject;

public class CMovePath extends CMove
{
    public int MT_Speed;
    public int MT_Sinus;
    public int MT_Cosinus;
    public int MT_Longueur;
    public int MT_XOrigin;
    public int MT_YOrigin;
    public int MT_XDest;
    public int MT_YDest;
    public int MT_MoveNumber;
    public boolean MT_Direction = false;
    public CMoveDefPath MT_Movement;
    public int MT_Calculs;
    public int MT_XStart;
    public int MT_YStart;
    public int MT_Pause;
    public String MT_GotoNode;
    boolean MT_FlagBranch = false;

    public CMovePath()
    {
    }

    @Override
	public void init(CObject ho, CMoveDef mvPtr)
    {
        hoPtr = ho;

        CMoveDefPath mtPtr = (CMoveDefPath) mvPtr;

        MT_XStart = hoPtr.hoX;                    //; Position de depart
        MT_YStart = hoPtr.hoY;

        MT_Direction = false;                            //; Vers l'avant
        MT_Pause = 0;
        hoPtr.hoMark1 = 0;

        MT_Movement = mtPtr;
        hoPtr.roc.rcMinSpeed = mtPtr.mtMinSpeed;            //; Vitesses mini et maxi
        hoPtr.roc.rcMaxSpeed = mtPtr.mtMaxSpeed;
        MT_Calculs = 0;
        MT_GotoNode = null;
        mtGoAvant(0);                                           //; Branche le premier mouvement
        moveAtStart(mvPtr);
        hoPtr.roc.rcSpeed = MT_Speed;
        hoPtr.roc.rcChanged = true;

        if (MT_Movement.mtNumber == 0)
        {
            stop();
        }
    }

    @Override
	public void kill()
    {

    }

    @Override
	public void move()
    {
        hoPtr.hoMark1 = 0;

        // Va faire les animations
        // ~~~~~~~~~~~~~~~~~~~~~~~
        hoPtr.roc.rcAnim = CAnim.ANIMID_WALK;
        if (hoPtr.roa != null) 
        	hoPtr.roa.animate();

        // On est en pause?
        // ~~~~~~~~~~~~~~~
        if (MT_Speed == 0)                        //; Arrete?
        {
            int pause = MT_Pause;                //; Un compteur?
            if (pause == 0)
            {
                hoPtr.roc.rcSpeed = 0;
                hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
                return;
            }
            pause -= hoPtr.hoAdRunHeader.rhTimerDelta;
            //Log.v("speed", " pause "+pause);
            if (pause > 0)
            {
                MT_Pause = pause;
                hoPtr.roc.rcSpeed = 0;
                hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);            //; Va gerer les collisions tout de meme
                return;
            }
            MT_Pause = 0;
            //Log.v("speed", " In Stop antes "+MT_Speed+" StopSpeed "+rmStopSpeed);
            MT_Speed = rmStopSpeed & 0xFF;
            rmStopSpeed = 0;
            //Log.v("speed", " In Stop despues "+MT_Speed+" StopSpeed "+rmStopSpeed);
            hoPtr.roc.rcSpeed = MT_Speed; // &0xFF;
        }

        // Decoupe le mouvement en plus petits troncons
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        int calculs=0x100;
        if ((hoPtr.hoAdRunHeader.rhFrame.leFlags & CRunFrame.LEF_TIMEDMVTS) != 0)
        {
            calculs = (int) (256.0 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef);
        }
        //else
        //{
        //    calculs = 0x100;
        //}
        
        hoPtr.hoAdRunHeader.rhMT_VBLCount = (short) calculs;
        boolean breakMtNewSpeed;
        while (true)
        {
            breakMtNewSpeed = false;
            hoPtr.hoAdRunHeader.rhMT_VBLStep = (short) calculs;
            calculs *= MT_Speed;
            calculs <<= 5;                        // PIXEL_SPEED;
            if (calculs <= 0x80000)                    //; Pente <8
            {
                calculs &= 0x7FFFFF;			  // Filter added for negative values
                hoPtr.hoAdRunHeader.rhMT_MoveStep = calculs;
            } 
            else
            {
                calculs = 0x80000 >>> 5;            //PIXEL_SPEED;
                calculs /= MT_Speed;
                hoPtr.hoAdRunHeader.rhMT_VBLStep = (short) calculs;                //; Nombre de VBL pour un pas
                hoPtr.hoAdRunHeader.rhMT_MoveStep = 0x80000;
             }
            
            while (true)
            {
                MT_FlagBranch = false;
                boolean flag = mtMove(hoPtr.hoAdRunHeader.rhMT_MoveStep);
                if (flag == true && MT_FlagBranch == false)
                {
                    breakMtNewSpeed = true;                                     //; Arret du mouvement?
                    break;
                }
                if (hoPtr.hoAdRunHeader.rhMT_VBLCount == hoPtr.hoAdRunHeader.rhMT_VBLStep)
                {
                    breakMtNewSpeed = true;
                    break;
                }
                if (hoPtr.hoAdRunHeader.rhMT_VBLCount > hoPtr.hoAdRunHeader.rhMT_VBLStep)
                {
                    hoPtr.hoAdRunHeader.rhMT_VBLCount -= hoPtr.hoAdRunHeader.rhMT_VBLStep;
                    calculs = hoPtr.hoAdRunHeader.rhMT_VBLCount;            //; OUI, on recalcule
                    break;
                }
                calculs = hoPtr.hoAdRunHeader.rhMT_VBLCount * MT_Speed;
                calculs <<= 5;        // PIXEL_SPEED
                mtMove(calculs);
                breakMtNewSpeed = true;
                break;
            };
            if (breakMtNewSpeed)
            	break;
        };
    }

    boolean mtMove(int step)
    {
        // Fait un pas de mouvement
        // ~~~~~~~~~~~~~~~~~~~~~~~~
        step += MT_Calculs;
        int step2 = (step >> 16)&0xFFFF;
        
        if (step2 < MT_Longueur)
        {
            MT_Calculs = step;
            int x = (step2 * MT_Cosinus) / 16384 + MT_XOrigin;        // Fois cosinus-> penteX
            int y = (step2 * MT_Sinus) / 16384 + MT_YOrigin;            // Fois sinus-> penteY

            hoPtr.hoX = x;
            hoPtr.hoY = y;
            hoPtr.roc.rcChanged = true;
            hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);                //; Appel les collisions
            return hoPtr.rom.rmMoveFlag;                    //; Retourne avec les flags
        }

        // Trop Long: tronquer le mouvement, et passer au suivant
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        step2 -= MT_Longueur;
        step = (step2 << 16) | (step & 0xFFFF);
        if (MT_Speed != 0) 
        	step /= MT_Speed;
        step >>= 5;                           // PIXEL_SPEED Nombre de VBL en trop
        hoPtr.hoAdRunHeader.rhMT_VBLCount += (short) (step & 0xFFFF);                //; On additionne

        hoPtr.hoX = MT_XDest;
        hoPtr.hoY = MT_YDest;
        hoPtr.roc.rcChanged = true;
        hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);                    //; Appel les collisions
        if (hoPtr.rom.rmMoveFlag) 
        	return true;

        // Passe au mouvement suivant / precedent
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        hoPtr.hoMark1 = hoPtr.hoAdRunHeader.rhLoopCount;                //; NODE REACHED
        hoPtr.hoMT_NodeName = null;

        // Passe au node suivant
        int number = MT_MoveNumber;
        MT_Calculs = 0;                        //; En cas de message
        if (MT_Direction == false)
        {
            // Mouvement suivant
            // -----------------
            number++;
            if (number < MT_Movement.mtNumber)                    //; Dernier mouvement?
            {
                hoPtr.hoMT_NodeName = MT_Movement.steps[number].mdName;

                // Goto node : on atteint le noeud?
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (MT_GotoNode != null)
                {
                    if (MT_Movement.steps[number].mdName != null)
                    {
                        if (MT_GotoNode.compareToIgnoreCase(MT_Movement.steps[number].mdName) == 0)
                        {
                            MT_MoveNumber = number;                   //; Au cas ou il y a des messages...
                            mtMessages();
                            return mtTheEnd();            //; Fin du mouvement
                        }
                    }
                }

                // Mouvement suivant normal
                // ~~~~~~~~~~~~~~~~~~~~~~~~
                mtGoAvant(number);
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            // Fin du mouvement vers l'avant
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            hoPtr.hoMark2 = hoPtr.hoAdRunHeader.rhLoopCount;    //; END OF PATH
            MT_MoveNumber = number;                               //; Au cas ou il y a des messages...
            if (MT_Direction)                       //; Les messages ont retourne le mouvement: FINI!
            {
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            if (MT_Movement.mtReverse != 0)
            {
                MT_Direction = true;
                number--;
                hoPtr.hoMT_NodeName = MT_Movement.steps[number].mdName;
                mtGoArriere(number);
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            mtReposAtEnd();                    //; Repositionne a la fin si necessaire
            if (MT_Movement.mtLoop == 0)                        //; Loop?
            {
                mtTheEnd();                    //; Fin du mouvement
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            number = 0;
            mtGoAvant(number);
            mtMessages();
            return hoPtr.rom.rmMoveFlag;
        } else
        {
            // Mouvement precedent
            // -------------------

            // Goto node : on atteint le noeud?
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (MT_GotoNode != null)
            {
                if (MT_Movement.steps[number].mdName != null)
                {
                    if (MT_GotoNode.compareToIgnoreCase(MT_Movement.steps[number].mdName) == 0)
                    {
                        mtMessages();
                        return mtTheEnd();            //; Fin du mouvement
                    }
                }
            }
            hoPtr.hoMT_NodeName = MT_Movement.steps[number].mdName;
            MT_Pause = MT_Movement.steps[number].mdPause;
            number--;
            if (number >= 0)                                //; Premier mouvement?
            {
                // Mouvement normal vers l'arriere
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                mtGoArriere(number);
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            // Arrive au debut du mouvement
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            mtReposAtEnd();                    //; Repositionne a la fin si necessaire
            if (MT_Direction == false)
            {
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            if (MT_Movement.mtLoop == 0)
            {
                mtTheEnd();                            //; Fin du mouvement
                mtMessages();
                return hoPtr.rom.rmMoveFlag;
            }
            number = 0;                                    //; Redemarre au debut
            MT_Direction = false;                    //; On repart dans le bon sens
            mtGoAvant(number);
            mtMessages();
            return hoPtr.rom.rmMoveFlag;
        }
    }

    // Un cran en avant
    // ----------------
    void mtGoAvant(int number)
    {
        if (number >= MT_Movement.steps.length)
        {
            stop();
        } else
        {
            MT_Direction = false;
            MT_MoveNumber = number;
            MT_Pause = MT_Movement.steps[number].mdPause;
            MT_Cosinus = MT_Movement.steps[number].mdCosinus;
            MT_Sinus = MT_Movement.steps[number].mdSinus;
            MT_XOrigin = hoPtr.hoX;
            MT_YOrigin = hoPtr.hoY;
            MT_XDest = hoPtr.hoX + MT_Movement.steps[number].mdDx;
            MT_YDest = hoPtr.hoY + MT_Movement.steps[number].mdDy;
            hoPtr.roc.rcDir = MT_Movement.steps[number].mdDir;
            mtBranche();
        }
    }

    // Un cran en arriere
    // ------------------
    void mtGoArriere(int number)
    {
        if (number >= MT_Movement.steps.length)
        {
            stop();
        } else
        {
            MT_Direction = true;
            MT_MoveNumber = number;
            MT_Cosinus = -MT_Movement.steps[number].mdCosinus;
            MT_Sinus = -MT_Movement.steps[number].mdSinus;
            MT_XOrigin = hoPtr.hoX;
            MT_YOrigin = hoPtr.hoY;
            MT_XDest = hoPtr.hoX - MT_Movement.steps[number].mdDx;
            MT_YDest = hoPtr.hoY - MT_Movement.steps[number].mdDy;
            int dir = MT_Movement.steps[number].mdDir;
            dir += 16;
            dir &= 31;
            hoPtr.roc.rcDir = dir;
            mtBranche();
        }
    }

    // Met la fin des calculs
    void mtBranche()
    {
        MT_Longueur = MT_Movement.steps[MT_MoveNumber].mdLength;
        int speed = MT_Movement.steps[MT_MoveNumber].mdSpeed;

        // Faire une pause?
        int pause = MT_Pause;
        if (pause != 0)
        {
            MT_Pause = pause * 20;
            speed |= 0x8000;
            rmStopSpeed = speed;            //; La vitesse de stop
        }
        if (rmStopSpeed != 0)
        {
            speed = 0;                                // Stop!
        }
        if (speed != MT_Speed || speed != 0)
        {
            MT_Speed = speed&0xFF;
            hoPtr.rom.rmMoveFlag = true;
            MT_FlagBranch = true;
        }
        hoPtr.roc.rcSpeed = MT_Speed; 
        //Log.v("speed", "speed "+hoPtr.roc.rcSpeed+" speed "+speed);
    }

    // Envoie les messages NODE REACHED
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void mtMessages()
    {
        if (hoPtr.hoMark1 == hoPtr.hoAdRunHeader.rhLoopCount)
        {
            hoPtr.hoAdRunHeader.rhEvtProg.rhCurParam0 = 0;
            hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, (-20 << 16) | ((hoPtr.hoType) & 0xFFFF));        // CNDL_EXTPATHNODE
            hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, (-35 << 16) | ((hoPtr.hoType) & 0xFFFF));        // CNDL_EXTPATHNODENAME
        }
        if (hoPtr.hoMark2 == hoPtr.hoAdRunHeader.rhLoopCount)
        {
            hoPtr.hoAdRunHeader.rhEvtProg.rhCurParam0 = 0;
            hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, (-21 << 16) | ((hoPtr.hoType) & 0xFFFF));   // CNDL_EXTENDPATH
        }
    }

    // Fin du mouvement
    // ~~~~~~~~~~~~~~~~
    boolean mtTheEnd()
    {
        MT_Speed = 0;
        rmStopSpeed = 0;
        hoPtr.rom.rmMoveFlag = true;
        //MT_FlagBranch=false;
       return true;
    }

    // Repositionner le sprite a la fin?
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void mtReposAtEnd()
    {
        if (MT_Movement.mtRepos != 0)
        {
            hoPtr.hoX = MT_XStart;
            hoPtr.hoY = MT_YStart;
            hoPtr.roc.rcChanged = true;
        }
    }

    // -------------------------------
    public void mtBranchNode(String pName)
    {
        int number;
        for (number = 0; number < MT_Movement.mtNumber; number++)
        {
            if (MT_Movement.steps[number].mdName != null)
            {
                if (pName.compareToIgnoreCase(MT_Movement.steps[number].mdName) == 0)
                {
                    if (MT_Direction == false)
                    {
                        // En avant
                        mtGoAvant(number);
                        hoPtr.hoMark1 = hoPtr.hoAdRunHeader.rhLoopCount;
                        hoPtr.hoMT_NodeName = MT_Movement.steps[number].mdName;
                        hoPtr.hoMark2 = 0;
                        mtMessages();
                    } 
                    else
                    {
                        if (number > 0)
                        {
                            number--;
                            mtGoArriere(number);
                            hoPtr.hoMark1 = hoPtr.hoAdRunHeader.rhLoopCount;
                            hoPtr.hoMT_NodeName = MT_Movement.steps[number].mdName;
                            hoPtr.hoMark2 = 0;
                            mtMessages();
                        }
                    }
                    hoPtr.rom.rmMoveFlag = true;
                    return;
                }
            }
        }
    }

    // Goto node : se rend a un noeud
    // ------------------------------
    void freeMTNode()
    {
        MT_GotoNode = null;
    }

    public void mtGotoNode(String pName)
    {
        int number;

        for (number = 0; number < MT_Movement.mtNumber; number++)
        {
            if (MT_Movement.steps[number].mdName != null)
            {
                if (pName.compareToIgnoreCase(MT_Movement.steps[number].mdName) == 0)
                {
                    if (number == MT_MoveNumber)
                    {
                        if (MT_Calculs == 0)    // Au debut du node
                            return;
                    }

                    freeMTNode();
                    MT_GotoNode = pName;

                    if (MT_Direction == false)
                    {
                        if (number > MT_MoveNumber)
                        {
                            // En avant
                            if (MT_Speed != 0) 
                            	return;
                            if ((rmStopSpeed & 0x08000) != 0) 
                            	start();
                            else 
                            	mtGoAvant(MT_MoveNumber);
                            return;
                        } else
                        {
                            // En arriere
                            if (MT_Speed != 0)
                            {
                                reverse();
                                return;
                            }
                            if ((rmStopSpeed & 0x08000) != 0)
                            {
                                start();
                                reverse();
                            } else
                            {
                                mtGoArriere(MT_MoveNumber - 1);
                            }
                            return;
                        }
                    } else
                    {
                        if (number <= MT_MoveNumber)
                        {
                            // En arriere
                            if (MT_Speed != 0) 
                            	return;
                            if ((rmStopSpeed & 0x08000) != 0) 
                            	start();
                            else
                            {
                                mtGoArriere(MT_MoveNumber - 1);
                            }
                            return;
                        } else
                        {
                            // En avant
                            if (MT_Speed != 0)
                            {
                                reverse();
                                return;
                            }
                            if ((rmStopSpeed & 0x08000) != 0)
                            {
                                start();
                                reverse();
                            } else 
                            	mtGoAvant(MT_MoveNumber);
                            return;
                        }
                    }
                }
            }
        }
    }

    @Override
	public void stop()
    {
        if (rmStopSpeed == 0)
        {
            rmStopSpeed = MT_Speed | 0x08000;
        }
        MT_Speed = 0;
        hoPtr.rom.rmMoveFlag = true;
    }

    @Override
	public void start()
    {
        if ((rmStopSpeed & 0x08000) != 0)
        {
            MT_Speed = rmStopSpeed & 0x07FFF;
            MT_Pause = 0;                            //; Stoppe la pause
            rmStopSpeed = 0;
            hoPtr.rom.rmMoveFlag = true;
        }
    }

    @Override
	public void reverse()
    {
        if (rmStopSpeed == 0)
        {
            hoPtr.rom.rmMoveFlag = true;
            int number = MT_MoveNumber;
            if (MT_Calculs == 0)                    //; Au milieu ou au debut?
            {
                // On est au debut d'un noeud: on passe au suivant / precedent
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                MT_Direction = MT_Direction ? false : true;
                if (MT_Direction)
                {
                    if (number == 0)
                    {
                        MT_Direction = MT_Direction ? false : true;
                        return;
                    }
                    number--;
                    mtGoArriere(number);
                } else
                {
                    mtGoAvant(number);
                }
            } else
            {
                // On est en plein mouvement: on inverse les calculs
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                MT_Direction = MT_Direction ? false : true;                        //; Avant/arriere
                MT_Cosinus = -MT_Cosinus;    //; Les pentes
                MT_Sinus = -MT_Sinus;
                int x1 = MT_XOrigin;                    //; Les coordonnees
                int x2 = MT_XDest;
                MT_XOrigin = x2;
                MT_XDest = x1;
                x1 = MT_YOrigin;
                x2 = MT_YDest;
                MT_YOrigin = x2;
                MT_YDest = x1;
                hoPtr.roc.rcDir += 16;                            //; La direction
                hoPtr.roc.rcDir &= 31;
                int calcul = (MT_Calculs>>16)&0xFFFF;
                calcul = MT_Longueur - calcul;
                MT_Calculs = (calcul << 16) | (MT_Calculs & 0xFFFF);
            }
        }
    }

    // ------------------------------------------
    // Changement de position d'un mouvement PATH
    // ------------------------------------------
    @Override
	public void setXPosition(int x)
    {
        int x2 = hoPtr.hoX;
        hoPtr.hoX = x;

        x2 -= MT_XOrigin;
        x -= x2;
        x2 = MT_XDest - MT_XOrigin + x;
        MT_XDest = x2;
        x2 = MT_XOrigin;
        MT_XOrigin = x;
        x2 -= x;
        MT_XStart -= x2;
        hoPtr.rom.rmMoveFlag = true;
        hoPtr.roc.rcChanged = true;
        hoPtr.roc.rcCheckCollides = true;                    //; Force la detection de collision
    }

    @Override
	public void setYPosition(int y)
    {
        int y2 = hoPtr.hoY;
        hoPtr.hoY = y;

        y2 -= MT_YOrigin;
        y -= y2;
        y2 = MT_YDest - MT_YOrigin + y;
        MT_YDest = y2;
        y2 = MT_YOrigin;
        MT_YOrigin = y;
        y2 -= y;
        MT_YStart -= y2;
        hoPtr.rom.rmMoveFlag = true;
        hoPtr.roc.rcChanged = true;
        hoPtr.roc.rcCheckCollides = true;                    //; Force la detection de collision
    }

    @Override
	public void bounce()
    {

    }

    @Override
	public void setSpeed(int speed)
    {
        if (speed < 0) 
        	speed = 0;
        if (speed > 250) 
        	speed = 250;
        MT_Speed = speed;
        hoPtr.roc.rcSpeed = speed;
        hoPtr.rom.rmMoveFlag = true;
    }

    @Override
	public void setMaxSpeed(int speed)
    {
        setSpeed(speed);
    }

    @Override
	public void setDir(int dir)
    {
    }

}
