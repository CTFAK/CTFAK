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
// CEVENTPROGRAM : Programme des evenements
//
//----------------------------------------------------------------------------------
package Events;

import java.util.ArrayList;
import java.util.Arrays;

import Actions.CAct;
import Application.CRunApp;
import Application.CRunFrame;
import Banks.IEnum;
import Conditions.CCnd;
import Expressions.CExpOi;
import Expressions.EXP_LOOPINDEXOPT;
import Expressions.EXP_STRING;
import Frame.CLO;
import OI.COI;
import OI.CObjectCommon;
import Objects.CObject;
import Params.CCreate;
import Params.CParam;
import Params.CParamExpression;
import Params.CPosition;
import Params.PARAM_EVERY;
import Params.PARAM_GROUP;
import Params.PARAM_GROUPOINTER;
import Params.PARAM_INT;
import Params.PARAM_OBJECT;
import Params.PARAM_SAMPLE;
import Params.PARAM_SHORT;
import Params.PARAM_TIME;
import Params.PARAM_ZONE;
import Params.PARAM_CHILDEVENT;
import RunLoop.CObjInfo;
import RunLoop.CRun;
import RunLoop.TimerEvents;
import Runtime.Log;
import Sprites.CColMask;
import Sprites.CSprite;
import Sprites.CSpriteGen;

public class CEventProgram
{
    public static final short EVENTS_EXTBASE = 80;
    public static final int PARAMCLICK_DOUBLE = 0x100;
    public static final int EVENTOPTION_BREAKCHILD = 0x0001;
    public CRun rhPtr = null;
    public short maxObjects = 0;
    public short maxOi = 0;
    public short nPlayers = 0;
    public short nConditions[] = new short[7 + COI.OBJ_LAST];         // NUMBEROF_SYSTEMTYPES+OBJ_LAST
    public short nQualifiers = 0;
    public int nEvents = 0;
    public CLoadQualifiers qualifiers[] = null;
    public CEventGroup events[];
    public CQualToOiList qualToOiList[] = null;
    public int listPointers[] = null;
    public CEventGroup eventPointersGroup[] = null;
    public byte eventPointersCnd[] = null;
    public short limitBuffer[] = null;
    public int rhEvents[] = new int[COI.NUMBEROF_SYSTEMTYPES + 1];
    public boolean rhEventAlways = false;
    public int rh4TimerEventsBase;
    short colBuffer[] = null;
    int qualOilPtr;
    int qualOilPos;
    int qualOilPtr2;
    int qualOilPos2;
    public boolean rh4CheckDoneInstart = false;			// Build92 to correct start of frame with fade in
    public CEventGroup rhEventGroup;				// Current group
    public int rhCurCode;                                       // Current event
    public int rh4PickFlags[] = new int[4];				// 00-31
    public boolean rh2ActionLoop = false;			// Actions flag
    public boolean rh2ActionOn = false;			// Flag are we in actions?
    public boolean rh2EnablePick = false;  			// Are we in pick for actions?
    public int rh2EventCount = 0;				// Number of the event
    public int rh2ActionCount = 0;				// Action counter
    public int rh2ActionLoopCount = 0;	  		// Action loops counter
    public int rh4EventCountOR;                         // Number of the event for OR conditions
    public boolean rh4ConditionsFalse = false;
    public boolean rh3DoStop = false;			// Force the test of stop actions
    public CQualToOiList rh2EventQualPos = null;          // ***Position in event objects
    public int rh2EventQualPosNum;			// ***Position in event objects
    public CObject rh2EventPos;				// ***Position in event objects
    public int rh2EventPosOiList; 		// ***Position in oilist for TYPE exploration 
    public CObject rh2EventPrev;				// ***Previous object address
    public CObjInfo rh2EventPrevOiList;				// ***Previous object address
    public int evtNSelectedObjects = 0;
    public boolean repeatFlag = false;
    public short rh2EventType;
    public short rhCurOi;
    public int rhCurParam0;
    public int rhCurParam1;
    public String rhCurParamString;
    public int rh3CurrentMenu;				// For menu II events
    public short rh2CurrentClick = -1;   	// For click events II
    public CObject rh4_2ndObject;	 		// Collision object address
    public boolean bReady = false;
    public ArrayList<CObject> rh2ShuffleBuffer = null;
    public short rhCurObjectNumber;	 		// Object number
    public short rh1stObjectNumber;          // Number, for collisions
    public ArrayList<CPushedEvent> rh2PushedEvents = null;
    public boolean rh2ActionEndRoutine = false;		// End of action routine
    public boolean callEndForEach = false;
    public boolean complexOnLoop = false;
	public PARAM_CHILDEVENT childEventParam = null;
	public ArrayList<ArrayList<SaveSelection>> childEventSelectionStack = new ArrayList<ArrayList<SaveSelection>>();
    public int defaultEvgFlagsMask = CEventGroup.EVGFLAGS_DEFAULTMASK;          // Default flags for eventgroups

    public CEventProgram()
    {
    }

    // ---------------------------------------------------------------------------
    // OBJECT SELECTION FOR EVENTS AND ACTIONS
    // ---------------------------------------------------------------------------
    boolean bts(int array[], int index)
    {
        int d = index / 32;
        int mask = 1 << (index & 31);
        boolean b = (array[d] & mask) != 0;
        array[d] |= mask;
        return b;
    }

    // -------------------------------------------------------------
    // EVENEMENT : RECHERCHE D'UN TYPE DEFINI D'OBJETS, AX=TYPE
    // -------------------------------------------------------------
    public CObject evt_FirstObjectFromType(short nType)
    {
        rh2EventType = nType;
        if (nType == -1)
        {
            CObject pHo;
            CObject pHoStore = null;
            int oil;
            CObjInfo poil;
            boolean bStore = true;
            final int rhPtr_rhOiList_length = rhPtr.rhOiList.length;
            for (oil = 0; oil < rhPtr_rhOiList_length; oil++)
            {
                poil = rhPtr.rhOiList[oil];
                if (bts(rh4PickFlags, poil.oilType) == false)	// Deja vue dans ce groupe d'event (met le flag aussi!) ?
                {
                    pHo = evt_SelectAllFromType(poil.oilType, bStore);
                    if (pHo != null)
                    {
                        pHoStore = pHo;
                        bStore = false;
                    }
                }
            }
            if (pHoStore != null)
            {
                return pHoStore;
            }
        }
        else
        {
            if (bts(rh4PickFlags, nType) == false)                    // Deja vue dans ce groupe d'event (met le flag aussi!) ?
            {
                return evt_SelectAllFromType(nType, true);		// NON, on selectionne tout et on retourne le premier
            }
        }

        int oil = 0;
        CObjInfo oilPtr;
        do
        {
            oilPtr = rhPtr.rhOiList[oil];
            if ((nType != -1 && oilPtr.oilType == nType) || nType == -1)
            {
                if (oilPtr.oilListSelected >= 0)
                {
                    CObject pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
                    rh2EventPrev = null;
                    rh2EventPrevOiList = oilPtr;
                    rh2EventPos = pHo;
                    rh2EventPosOiList = oil;
                    return pHo;
                }
            }
            oil++;									// Un autre OI?
        } while (oil < rhPtr.rhMaxOI);
        return null;
    }

    // Retourne le suivant
    // ------------------- 
    public CObject evt_NextObjectFromType()
    {
        CObject pHo = rh2EventPos;
        CObjInfo oilPtr;
        if (pHo == null)
        {
            oilPtr = rhPtr.rhOiList[rh2EventPosOiList];
            if (oilPtr.oilListSelected >= 0)
            {
                pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
                rh2EventPrev = null;				// Stocke pour la destruction
                rh2EventPrevOiList = oilPtr;
                rh2EventPos = pHo;
                return pHo;
            }
        }
        if (pHo != null)
        {
            if (pHo.hoNextSelected >= 0)
            {
                rh2EventPrev = pHo;				// Stocke pour la destruction
                rh2EventPrevOiList = null;
                pHo = rhPtr.rhObjectList[pHo.hoNextSelected];
                rh2EventPos = pHo;
                return pHo;
            }
        }

        int oil = rh2EventPosOiList;			// Adresse de l'oilist
        oil++;
        while (oil < rhPtr.rhMaxOI)
        {
			oilPtr = rhPtr.rhOiList[oil];
            if ((rh2EventType != -1 && oilPtr.oilType == rh2EventType) || rh2EventType == -1)
            {
                if (oilPtr.oilListSelected >= 0)
                {
                    pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
                    rh2EventPrev = null;
                    rh2EventPrevOiList = oilPtr;
                    rh2EventPos = pHo;
                    rh2EventPosOiList = oil;
                    return pHo;
                }
            }
            oil++;									// Un autre OI?
        }
        return null;
    }

    // Selectionne TOUS les objets de meme type, retourne le premier dans EAX
    // ----------------------------------------------------------------------
    public CObject evt_SelectAllFromType(short nType, boolean bStore)
    {
        int first = -1;
        int evtCount = rh2EventCount;

        int oil = 0;
        CObjInfo oilPtr;
        CObject pHo;
        do
        {
            oilPtr = rhPtr.rhOiList[oil];
            if (oilPtr.oilType == nType && oilPtr.oilEventCount != evtCount) // Deja selectionne dans cet event?
            {
                oilPtr.oilEventCount = evtCount;					// Fabrique la liste
                if (rh4ConditionsFalse)
                {
                    oilPtr.oilListSelected = -1;
                    oilPtr.oilNumOfSelected = 0;
                }
                else
                {
                    oilPtr.oilNumOfSelected = oilPtr.oilNObjects;
                    short num = oilPtr.oilObject;
                    if (num >= 0)
                    {
                        if (first == -1 && bStore == true)
                        {
                            first = num;					// Stocke le premier pour aller plus vite
                            rh2EventPrev = null;
                            rh2EventPrevOiList = oilPtr;
                            rh2EventPosOiList = oil;
                        }
                        do
                        {
                            pHo = rhPtr.rhObjectList[num];
							if (pHo == null)
								return null;
                            pHo.hoNextSelected = pHo.hoNumNext;
                            num = pHo.hoNumNext;
                        } while (num >= 0);

                        num = oilPtr.oilObject;
                    }
                    oilPtr.oilListSelected = num;
                }
            }
            oil++;										// Un autre OI?
        } while (oil < rhPtr.rhMaxOI);

        if (bStore == false)
        {
            return null;
        }
        if (first < 0)
        {
            return null;
        }

        pHo = rhPtr.rhObjectList[first];
		if (pHo == null)
		    return null;
        rh2EventPos = pHo;
        return pHo;
    }

    // -------------------------------------------------------------
    // EVENEMENT : RECHERCHE DEFINI, EDX=OI
    // -------------------------------------------------------------
    public CObject evt_FirstObject(short sEvtOiList)
    {
        CObject pHo;

        evtNSelectedObjects = 0;
        rh2EventQualPos = null;
        rh2EventQualPosNum = -1;

        if (sEvtOiList < 0)
        {
            // Selectionne TOUS les objets ??? partir d'un qualifier
            // ---------------------------------------------------
            if (sEvtOiList == (short) -1)	// -1: pas d'objet du tout dans le jeu!
            {
                return null;
            }
            // Appel de la procedure
            return qualProc(sEvtOiList);
        }

        CObjInfo oilPtr = rhPtr.rhOiList[sEvtOiList];
        if (oilPtr.oilEventCount == rh2EventCount)		// Deja selectionne dans cet event?
        {
            // Prend la liste deja exploree dans un event precedent
            // ----------------------------------------------------
            if (oilPtr.oilListSelected < 0)
            {
                return null;
            }
            pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
            rh2EventPrev = null;
            rh2EventPrevOiList = oilPtr;
            rh2EventPos = pHo;
            rh2EventPosOiList = sEvtOiList;
            evtNSelectedObjects = oilPtr.oilNumOfSelected;
            return pHo;
        }
        else
        {
            // Selectionne TOUS les objets de meme type, retourne le premier dans EAX
            // ----------------------------------------------------------------------
            oilPtr.oilEventCount = rh2EventCount;

            // Si condition OR et conditions fausse, ne selectionne aucun objet
            if (rh4ConditionsFalse)
            {
                oilPtr.oilListSelected = -1;
                oilPtr.oilNumOfSelected = 0;
                return null;
            }

            // Ajoute les objets
            oilPtr.oilListSelected = oilPtr.oilObject;
            if (oilPtr.oilObject < 0)
            {
                oilPtr.oilNumOfSelected = 0;
                return null;
            }
            short num = oilPtr.oilObject;
            do
            {
                pHo = rhPtr.rhObjectList[num];
                num = pHo.hoNumNext;
                pHo.hoNextSelected = num;
            } while (num >= 0);

            pHo = rhPtr.rhObjectList[oilPtr.oilObject];
            rh2EventPrev = null;
            rh2EventPrevOiList = oilPtr;
            rh2EventPos = pHo;
            rh2EventPosOiList = sEvtOiList;
            oilPtr.oilNumOfSelected = oilPtr.oilNObjects;
            evtNSelectedObjects = oilPtr.oilNumOfSelected;
            return pHo;
        }
    }

    CObject qualProc(short sEvtOiList)
    {
        CObject pHo;
        CObjInfo oilPtr;
        int count = 0;

        // Selectionne / Compte tous les objets de tous les groupes
        int qoi = 0;
        short qoiList;
        int addCount;
        CQualToOiList pQoi = qualToOiList[sEvtOiList & 0x7FFF];
        int pQoi_qoiList_length = pQoi.qoiList.length;
        while (qoi < pQoi_qoiList_length)
        {
            qoiList = pQoi.qoiList[qoi + 1];
            oilPtr = rhPtr.rhOiList[qoiList];
            if (oilPtr.oilEventCount == rh2EventCount)
            {
                // Deja selectionnee dans un evenement precedent
                addCount = 0;
                if (oilPtr.oilListSelected >= 0)
                {
                    addCount = oilPtr.oilNumOfSelected;
                    if (rh2EventQualPos == null)
                    {
                        rh2EventQualPos = pQoi;
                        rh2EventQualPosNum = qoi;
                    }
                }
            }
            else
            {
                addCount = 0;
                oilPtr.oilEventCount = rh2EventCount;

                // Si condition OR et conditions fausse, ne selectionne aucun objet
                if (rh4ConditionsFalse)
                {
                    oilPtr.oilListSelected = -1;
                    oilPtr.oilNumOfSelected = 0;
                }
                else
                {
                    oilPtr.oilListSelected = oilPtr.oilObject;
                    if (oilPtr.oilObject < 0)
                    {
                        oilPtr.oilNumOfSelected = 0;
                    }
                    else
                    {
                        if (rh2EventQualPos == null)
                        {
                            rh2EventQualPos = pQoi;
                            rh2EventQualPosNum = qoi;
                        }
                        short num = oilPtr.oilObject;
                        do
                        {
                            pHo = rhPtr.rhObjectList[num];
                            pHo.hoNextSelected = pHo.hoNumNext;
                            num = pHo.hoNumNext;
                        } while (num >= 0);

                        oilPtr.oilNumOfSelected = oilPtr.oilNObjects;
                        addCount = oilPtr.oilNObjects;
                    }
                }
            }
            count += addCount;
            qoi += 2;
        }

        pQoi = rh2EventQualPos;
        if (pQoi != null)
        {
            oilPtr = rhPtr.rhOiList[pQoi.qoiList[rh2EventQualPosNum + 1]];
            rh2EventPrev = null;
            rh2EventPrevOiList = oilPtr;
            pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
            rh2EventPos = pHo;
            rh2EventPosOiList = pQoi.qoiList[rh2EventQualPosNum + 1];
            evtNSelectedObjects = count;
            return pHo;
        }
        return null;
    }


    // ------------------------
    // RETOURNE L'OBJET SUIVANT
    // ------------------------
    public CObject evt_NextObject()
    {
        CObject pHo = rh2EventPos;
        CObjInfo oilPtr;
        if (pHo == null)
        {
            oilPtr = rhPtr.rhOiList[rh2EventPosOiList];
            if (oilPtr.oilListSelected >= 0)
            {
                pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
                rh2EventPrev = null;				// Stocke pour la destruction
                rh2EventPrevOiList = oilPtr;
                rh2EventPos = pHo;
                return pHo;
            }
        }
        if (pHo != null)
        {
            if (pHo.hoNextSelected >= 0)
            {
                rh2EventPrev = pHo;				// Stocke pour la destruction
                rh2EventPrevOiList = null;
                pHo = rhPtr.rhObjectList[pHo.hoNextSelected];
                rh2EventPos = pHo;
                return pHo;
            }
        }
        if (rh2EventQualPos == null)			// Une liste de qualifiers?
        {
            return null;
        }

        // Prend la liste de qualifier suivante
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        do
        {
            rh2EventQualPosNum += 2;
            if (rh2EventQualPosNum >= rh2EventQualPos.qoiList.length)
            {
                return null;
            }
            oilPtr = rhPtr.rhOiList[rh2EventQualPos.qoiList[rh2EventQualPosNum + 1]];
        } while (oilPtr.oilListSelected < 0);

        rh2EventPrev = null;
        rh2EventPrevOiList = oilPtr;
        pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
        rh2EventPos = pHo;
        rh2EventPosOiList = rh2EventQualPos.qoiList[rh2EventQualPosNum + 1];
        return pHo;
    }

    // -------------------------------------------------
    // SELECTIONNE TOUS LES OBJETS RELIES A UN QUALIFIER
    // -------------------------------------------------
    public void evt_AddCurrentQualifier(short qual)
    {
        CQualToOiList pQoi = qualToOiList[qual & 0x7FFF];
        int noil = 0;
        CObjInfo oilPtr;
        int pQoi_qoiList_length = pQoi.qoiList.length;
        while (noil < pQoi_qoiList_length)
        {
            oilPtr = rhPtr.rhOiList[pQoi.qoiList[noil + 1]];
            if (oilPtr.oilEventCount != rh2EventCount)
            {
                oilPtr.oilEventCount = rh2EventCount;
                oilPtr.oilNumOfSelected = 0;
                oilPtr.oilListSelected = -1;
            }
            noil += 2;
        }
        ;
    }

    // -------------------------------------------------
    // DESELECTIONNE TOUS LES OBJETS RELIES A UN QUALIFIER
    // -------------------------------------------------
    public void evt_DeleteCurrentQualifier(short qual)
    {
        CQualToOiList pQoi = qualToOiList[qual & 0x7FFF];
        int noil = 0;
        CObjInfo oilPtr;
        int pQoi_qoiList_length = pQoi.qoiList.length;
        while (noil < pQoi_qoiList_length)
        {
            oilPtr = rhPtr.rhOiList[pQoi.qoiList[noil + 1]];
            oilPtr.oilEventCount = rh2EventCount;
            oilPtr.oilNumOfSelected = 0;
            oilPtr.oilListSelected = -1;
            noil += 2;
        }
    }

    // ----------------------------------------------------------
    // ENLEVE L'OBJET COURANT DE LA LISTE DES OBJETS SELECTIONNES
    // ----------------------------------------------------------
    public void evt_DeleteCurrentObject()
    {
        rh2EventPos.hoOiList.oilNumOfSelected -= 1;					// Un de moins dans l'OiList
        if (rh2EventPrev != null)
        {
            rh2EventPrev.hoNextSelected = rh2EventPos.hoNextSelected;
            rh2EventPos = rh2EventPrev;                                           // Car le courant est vire!
        }
        else
        {
//            rhPtr.rhOiList[rh2EventPosOiList].oilListSelected=rh2EventPos.hoNextSelected;
            rh2EventPrevOiList.oilListSelected = rh2EventPos.hoNextSelected;
            rh2EventPos = null;
        }
    }

    // -----------------------------------------------
    // ADDITIONNE L'OBJET EAX A LA LISTE SI NECESSAIRE
    // -----------------------------------------------
    public void evt_AddCurrentObject(CObject pHo)
    {
        CObjInfo oilPtr = pHo.hoOiList;
        if (oilPtr.oilEventCount != rh2EventCount)
        {
            // Aucune selection
            oilPtr.oilEventCount = rh2EventCount;
            oilPtr.oilListSelected = pHo.hoNumber;
            oilPtr.oilNumOfSelected = 1;
            pHo.hoNextSelected = -1;
        }
        else
        {
            // Objet deja selectionne, evite les doublets
            short oils = oilPtr.oilListSelected;
            if (oils < 0)
            {
                oilPtr.oilListSelected = pHo.hoNumber;
                oilPtr.oilNumOfSelected += 1;
                pHo.hoNextSelected = -1;
            }
            else
            {
                CObject pHo1;
                do
                {
                    if (pHo.hoNumber == oils)
                    {
                        return;
                    }
                    pHo1 = rhPtr.rhObjectList[oils];
                    oils = pHo1.hoNextSelected;
                } while (oils >= 0);

                pHo1.hoNextSelected = pHo.hoNumber;
                pHo.hoNextSelected = -1;
                pHo.hoOiList.oilNumOfSelected += 1;
            }
        }
    }

    // -------------------------------------------
    // FORCE L'OBJET EAX SEUL DANS SA PROPRE LISTE
    // -------------------------------------------
    public void deselectThem(short oil)
    {
        CObjInfo poil = rhPtr.rhOiList[oil];		// Pointe la liste
        poil.oilEventCount = rh2EventCount;
        poil.oilListSelected = -1;
        poil.oilNumOfSelected = 0;
    }

    public void evt_ForceOneObject(short oil, CObject pHo)
    {
        // Deselectionne tous les objets de meme oil
        if (oil >= 0)
        {
            // Un identifier normal
            deselectThem(oil);
        }
        else
        {
            // Un qualifier
            if (oil == -1)
            {
                return;
            }
            CQualToOiList pqoi = qualToOiList[oil & 0x7FFF];
            int qoi;
            int pqoi_qoiList_length = pqoi.qoiList.length;
            for (qoi = 0; qoi < pqoi_qoiList_length; qoi += 2)
            {
                deselectThem(pqoi.qoiList[qoi + 1]);
            }
        }
        
        // Selects the only one
        pHo.hoNextSelected = -1;
        pHo.hoOiList.oilListSelected = pHo.hoNumber;
        pHo.hoOiList.oilNumOfSelected = 1;
        pHo.hoOiList.oilEventCount = rh2EventCount;
    }


    // -----------------------------------------------
    // FORCE TOUT UN TYPE EN PICK, DESELECTIONNE TOUT
    // -----------------------------------------------
    public void evt_DeleteCurrentType(short nType)
    {
        bts(rh4PickFlags, nType);

        int oil;
        CObjInfo oilPtr;
        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            oilPtr = rhPtr.rhOiList[oil];
            if (oilPtr.oilType == nType)
            {
                oilPtr.oilEventCount = rh2EventCount;
                oilPtr.oilListSelected = -1;
                oilPtr.oilNumOfSelected = 0;
            }
        }
    }

    // Deslectionne tous les objets courants
    public void evt_DeleteCurrent()
    {
        rh4PickFlags[0] = -1;
        rh4PickFlags[1] = -1;
        rh4PickFlags[2] = -1;
        rh4PickFlags[3] = -1;

        int oil;
        CObjInfo oilPtr;
        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            oilPtr = rhPtr.rhOiList[oil];
            oilPtr.oilEventCount = rh2EventCount;
            oilPtr.oilListSelected = -1;
            oilPtr.oilNumOfSelected = 0;
        }
    }

    // ---------------------------------------------------
    // Gestion des OU dans les objets
    // ---------------------------------------------------
    void evt_MarkSelectedObjects()
    {
        short num;
        CObject pHO;
        int oil;
        CObjInfo oilPtr;

        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            oilPtr = rhPtr.rhOiList[oil];
            if (oilPtr.oilEventCount == rh2EventCount)
            {
                if (oilPtr.oilEventCountOR != rh4EventCountOR)
                {
                    oilPtr.oilEventCountOR = rh4EventCountOR;
                    num = oilPtr.oilObject;
                    while (num >= 0)
                    {
                        pHO = rhPtr.rhObjectList[num];
                        pHO.hoSelectedInOR = 0;
                        num = pHO.hoNumNext;
                    }
                }
                num = oilPtr.oilListSelected;
                while (num >= 0)
                {
                    pHO = rhPtr.rhObjectList[num];
                    pHO.hoSelectedInOR = 1;
                    num = pHO.hoNextSelected;
                }
            }
        }
    }

    // Branche les objets selectionnes dans les OU
    void evt_BranchSelectedObjects()
    {
        short num;
        CObject pHO, pHOPrev;
        int oil;
        CObjInfo oilPtr;

        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            oilPtr = rhPtr.rhOiList[oil];
            if (oilPtr.oilEventCountOR == rh4EventCountOR)
            {
                oilPtr.oilEventCount = rh2EventCount;

                num = oilPtr.oilObject;
                pHOPrev = null;
                while (num >= 0)
                {
                    pHO = rhPtr.rhObjectList[num];
                    if (pHO.hoSelectedInOR != 0)
                    {
                        if (pHOPrev != null)
                        {
                            pHOPrev.hoNextSelected = num;
                        }
                        else
                        {
                            oilPtr.oilListSelected = num;
                        }
                        pHO.hoNextSelected = -1;
                        pHOPrev = pHO;
                    }
                    num = pHO.hoNumNext;
                }
            }
        }
    }

    // ---------------------------------------------------
    // TROUVE L'OBJET COURANT POUR LES EXPRESSIONS >>> ESI
    // ---------------------------------------------------
    public CObject get_ExpressionObjects(short expoi)
    {
        if (rh2ActionOn)					// On est dans les actions ?
        {
            // Dans une action
            rh2EnablePick = false;					// En cas de chooseflag
            return get_CurrentObjects(expoi);	// Pointe l'oiList
        }

        // Dans un evenement
        // -----------------
        CObjInfo oilPtr;
        if (expoi >= 0)
        {
            oilPtr = rhPtr.rhOiList[expoi];
            if (oilPtr.oilEventCount == rh2EventCount)		// Selection actuelle?
            {
                if (oilPtr.oilListSelected >= 0)			// Le premier
                {
                    return rhPtr.rhObjectList[oilPtr.oilListSelected];
                }
                if (oilPtr.oilObject >= 0)					// Prend le premier objet
                {
                    return rhPtr.rhObjectList[oilPtr.oilObject];
                }

                // Pas d'objet!
                // ~~~~~~~~~~~~
                return null;
            }
            else
            {
                if (oilPtr.oilObject >= 0)					// Prend le premier objet
                {
                    return rhPtr.rhObjectList[oilPtr.oilObject];
                }

                // Pas d'objet!
                // ~~~~~~~~~~~~
                return null;
            }
        }

        // Un qualifier: trouve la premiere liste selectionnee
        // ---------------------------------------------------
        CQualToOiList pQoi = qualToOiList[expoi & 0x7FFF];
        int qoi = 0;
        int pQoi_qoiList_length = pQoi.qoiList.length;
        if (qoi >= pQoi_qoiList_length)
        {
            return null;
        }
        // Recherche un objet selectionne
        do
        {
            oilPtr = rhPtr.rhOiList[pQoi.qoiList[qoi + 1]];
            if (oilPtr.oilEventCount == rh2EventCount)
            {
                if (oilPtr.oilListSelected >= 0)	// Le premier selectionne?
                {
                    return rhPtr.rhObjectList[oilPtr.oilListSelected];
                }
            }
            qoi += 2;
        } while (qoi < pQoi_qoiList_length);

        // Pas trouve: prend le premier de la premiere liste disponible
        qoi = 0;
        do
        {
            oilPtr = rhPtr.rhOiList[pQoi.qoiList[qoi + 1]];
            if (oilPtr.oilObject >= 0)							// Le premier selectionne?
            {
                return rhPtr.rhObjectList[oilPtr.oilObject];
            }
            qoi += 2;
        } while (qoi < pQoi_qoiList_length);

        return null;
    }

    // ----------------------------------------------------------------------
    // TROUVE L'OBJET COURANT POUR UN PARAMETRE PARAM_OBJECT DANS UNE ACTION
    // ----------------------------------------------------------------------
    public CObject get_ParamActionObjects(short qoil, CAct pAction)
    {
        rh2EnablePick = true;
        CObject pObject = get_CurrentObjects(qoil);
        if (pObject != null)
        {
            if (repeatFlag == false)
            {
                // Pas de suivant
                return pObject;
            }
            else
            {
                // Un suivant
                pAction.evtFlags |= CAct.ACTFLAGS_REPEAT;		// Refaire cette action
                rh2ActionLoop = true;			 	// Refaire un tour d'actions
                return pObject;
            }
        }
        pAction.evtFlags |= CEvent.EVFLAGS_NOTDONEINSTART;
        return pObject;
    }

    // ----------------------------------------------------------------------
    // TROUVE L'OBJET COURANT POUR LES ACTIONS, MARQUE LES ACTIONS A REFAIRE
    // ----------------------------------------------------------------------
    public CObject get_ActionObjects(CAct pAction)
    {
        pAction.evtFlags &= ~CEvent.EVFLAGS_NOTDONEINSTART;
        rh2EnablePick = true;
        short qoil = pAction.evtOiList;				// Pointe l'oiList
        CObject pObject = get_CurrentObjects(qoil);
        if (pObject != null)
        {
            if (repeatFlag == false)
            {
                // Pas de suivant
                return pObject;
            }
            else
            {
                // Un suivant
                pAction.evtFlags |= CAct.ACTFLAGS_REPEAT;		// Refaire cette action
                rh2ActionLoop = true;			 	// Refaire un tour d'actions
                return pObject;
            }
        }
        pAction.evtFlags |= CEvent.EVFLAGS_NOTDONEINSTART;
        return pObject;
    }

    // --------------------------------------------------------------------------
    // Retourne un objet pour une action. Entree EDX=OiList. Change tout sauf EDI
    // --------------------------------------------------------------------------
    public CObject get_CurrentObjects(short qoil)
    {
        if (qoil >= 0)
        {
            return get_CurrentObject(qoil);
        }
        return get_CurrentObjectQualifier(qoil);
    }

    // -----------------------------------
    // GET ACTION OBJECT POUR OBJET NORMAL
    // -----------------------------------
    public CObject get_CurrentObject(short qoil)
    {
        CObject pHo;
        CObjInfo oilPtr = rhPtr.rhOiList[qoil];

        if (oilPtr.oilActionCount != rh2ActionCount)	//; Premiere exploration?
        {
            oilPtr.oilActionCount = rh2ActionCount;			//; C'est fait pour cette action
            oilPtr.oilActionLoopCount = rh2ActionLoopCount;

            // On recherche le premier dans la liste courante
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (oilPtr.oilEventCount == rh2EventCount)	//; Liste vraiment courante?
            {
                if (oilPtr.oilListSelected >= 0)					//; La liste des objets selectionnes
                {
                    oilPtr.oilCurrentOi = oilPtr.oilListSelected;
                    pHo = rhPtr.rhObjectList[oilPtr.oilListSelected];
                    oilPtr.oilNext = pHo.hoNextSelected;		//; Numero de l'objet suivant
                    if (pHo.hoNextSelected < 0)
                    {
                        oilPtr.oilNextFlag = false;				//; Pas de suivant!
                        oilPtr.oilCurrentRoutine = 1;                     // gao2ndOneOnly;
                        repeatFlag = false;
                        return pHo;
                    }
                    oilPtr.oilNextFlag = true;					//; Un suivant!
                    oilPtr.oilCurrentRoutine = 2;                         // gao2ndCurrent;
                    repeatFlag = true;
                    return pHo;
                }
            }

            // Objet non trouve, on prends tous les objets de meme oi
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (rh2EnablePick)						//; Pick autorise?
            {
                if (oilPtr.oilEventCount == rh2EventCount)	//; Alors juste cet objet?
                {
                    oilPtr.oilCurrentRoutine = 0;                     // gao2ndNone;
                    oilPtr.oilCurrentOi = -1;						// Pas de suivant
                    return null;
                }
            }
            if (oilPtr.oilObject >= 0)							//; Le numero du premier objet Est-il defini?
            {
                oilPtr.oilCurrentOi = oilPtr.oilObject;			//; Stocke le numero de l'objet courant
                pHo = rhPtr.rhObjectList[oilPtr.oilObject];
                if (pHo == null)
                {
                    oilPtr.oilCurrentRoutine = 0;                     // gao2ndNone;
                    oilPtr.oilCurrentOi = -1;						// Pas de suivant
                    return null;
                }
                if (pHo.hoNumNext >= 0)
                {
                    // Plusieurs objets
                    oilPtr.oilNext = pHo.hoNumNext;				// Numero de l'objet
                    oilPtr.oilNextFlag = true;						// Un suivant!
                    oilPtr.oilCurrentRoutine = 3;                 // gao2ndAll;
                    repeatFlag = true;
                    return pHo;
                }
                // Un seul objet
                oilPtr.oilNextFlag = false;							// Pas de suivant!
                oilPtr.oilCurrentRoutine = 1;                     // gao2ndOneOnly;
                repeatFlag = false;
                return pHo;
            }
            else
            {
                oilPtr.oilCurrentRoutine = 0;                     // gao2ndNone;
                oilPtr.oilCurrentOi = -1;						// Pas de suivant
                return null;
            }
        }

        if (oilPtr.oilActionLoopCount != rh2ActionLoopCount)
        {
            short next;
            oilPtr.oilActionLoopCount = rh2ActionLoopCount;	//; C'est fait pour cette boucle
            switch (oilPtr.oilCurrentRoutine)
            {
                // Pas d'objet
                case 0:                             // gao2ndNone
                    repeatFlag = oilPtr.oilNextFlag;
                    return null;
                // Un seul objet
                case 1:                             // gao2ndOneOnly
                    pHo = rhPtr.rhObjectList[oilPtr.oilCurrentOi];
                    repeatFlag = oilPtr.oilNextFlag;
                    return pHo;
                // Objet suivant dans la liste courante
                case 2:                             // gao2ndCurrent
                    oilPtr.oilCurrentOi = oilPtr.oilNext;					//; Numero de l'objet suivant
                    pHo = rhPtr.rhObjectList[oilPtr.oilNext];
                    if (pHo == null)
                    {
                        return null;
                    }
                    next = pHo.hoNextSelected;
                    if (next < 0)
                    {
                        oilPtr.oilNextFlag = false;							// Plus de suivant!
                        next = oilPtr.oilListSelected;
                    }
                    oilPtr.oilNext = next;
                    repeatFlag = oilPtr.oilNextFlag;
                    return pHo;
                // Objet suivant global
                case 3:                             // gao2ndAll
                    oilPtr.oilCurrentOi = oilPtr.oilNext;					//; Stocke le numero de l'objet courant
                    pHo = rhPtr.rhObjectList[oilPtr.oilNext];
                    if (pHo == null)
                    {
                        return null;
                    }
                    next = pHo.hoNumNext;
                    if (next < 0)
                    {
                        oilPtr.oilNextFlag = false;							// Pas de suivant!
                        next = oilPtr.oilObject;							// Repart au debut
                    }
                    oilPtr.oilNext = next;
                    repeatFlag = oilPtr.oilNextFlag;
                    return pHo;
            }
        }
        if (oilPtr.oilCurrentOi < 0)
        {
            return null;					//; Prend l'objet courant
        }
        pHo = rhPtr.rhObjectList[oilPtr.oilCurrentOi];
        repeatFlag = oilPtr.oilNextFlag;
        return pHo;
    }

    // GESTION GETACTION OBJECT AVEC QUALIFIER
    // --------------------------------------------------------------------------
    public CObject get_CurrentObjectQualifier(short qoil)
    {
        CObject pHo;
        short next, num;

        CQualToOiList pqoi = qualToOiList[qoil & 0x7FFF];
        if (pqoi.qoiActionCount != rh2ActionCount)			//; Premiere exploration?
        {
            // PREMIERE EXPLORATION
            // --------------------
            pqoi.qoiActionCount = rh2ActionCount;			//; C'est fait pour cette action
            pqoi.qoiActionLoopCount = rh2ActionLoopCount;

            // On recherche le premier dans les liste courantes
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            num = qoi_GetFirstListSelected(pqoi);					//; La premiere liste avec objet selectionne
            if (num >= 0)
            {
                pqoi.qoiCurrentOi = num;
                pHo = rhPtr.rhObjectList[num];
                if (pHo == null)
                {
                    pqoi.qoiCurrentRoutine = 0;           // qoi2ndNone;
                    pqoi.qoiCurrentOi = -1;						// Pas de suivant!
                    return null;
                }
                next = pHo.hoNextSelected;
                if (next < 0)
                {
                    next = qoi_GetNextListSelected(pqoi);
                    if (next < 0)
                    {
                        pqoi.qoiCurrentRoutine = 1;       // qoi2ndOneOnly;
                        pqoi.qoiNextFlag = false;
                        repeatFlag = false;
                        return pHo;
                    }
                }
                pqoi.qoiNext = next;
                pqoi.qoiCurrentRoutine = 2;               // qoi2ndCurrent;
                pqoi.qoiNextFlag = true;							// Un suivant!
                repeatFlag = true;
                return pHo;
            }

            // Prendre tous?
            // ~~~~~~~~~~~~~
            if (rh2EnablePick)						// Pick autoris????
            {
                if (pqoi.qoiSelectedFlag)					//; Une des listes a ete vue pendant les conditions
                {
                    pqoi.qoiCurrentRoutine = 0;           // qoi2ndNone;
                    pqoi.qoiCurrentOi = -1;						// Pas de suivant!
                    return null;
                }
            }
            num = qoi_GetFirstList(pqoi);
            if (num >= 0)
            {
                pqoi.qoiCurrentOi = num;							//; Stocke le numero de l'objet courant
                pHo = rhPtr.rhObjectList[num];
                if (pHo != null)
                {
                    num = pHo.hoNumNext;
                    if (num < 0)
                    {
                        num = qoi_GetNextList(pqoi);
                        if (num < 0)
                        {
                            pqoi.qoiCurrentRoutine = 1;               // qoi2ndOneOnly;
                            pqoi.qoiNextFlag = false;
                            repeatFlag = false;
                            return pHo;
                        }
                    }
                    pqoi.qoiNext = num;							// Numero de l'objet
                    pqoi.qoiCurrentRoutine = 3;                       // qoi2ndAll;
                    pqoi.qoiNextFlag = true;						// Un suivant
                    repeatFlag = true;
                    return pHo;
                }
            }
            pqoi.qoiCurrentRoutine = 0;       // qoi2ndNone;
            pqoi.qoiCurrentOi = -1;						// Pas de suivant!
            return null;
        }

        if (pqoi.qoiActionLoopCount != rh2ActionLoopCount)		//; Premiere fois dans la boucle?
        {
            pqoi.qoiActionLoopCount = rh2ActionLoopCount;		//; C'est fait pour cette boucle
            switch (pqoi.qoiCurrentRoutine)
            {
                // Pas d'objet
                case 0:                 // qoi2ndNone
                    repeatFlag = pqoi.qoiNextFlag;
                    return null;
                // Un seul objet
                case 1:                 // qoi2ndOneOnly
                    pHo = rhPtr.rhObjectList[pqoi.qoiCurrentOi];
                    repeatFlag = pqoi.qoiNextFlag;
                    return pHo;
                // Objet suivant dans la liste courante
                case 2:                 // qoi2ndCurrent
                    pqoi.qoiCurrentOi = pqoi.qoiNext;					// Numero de l'objet suivant
                    pHo = rhPtr.rhObjectList[pqoi.qoiNext];
                    if (pHo != null)
                    {
                        next = pHo.hoNextSelected;
                        if (next < 0)
                        {
                            next = qoi_GetNextListSelected(pqoi);
                            if (next < 0)
                            {
                                pqoi.qoiNextFlag = false;					// Plus de suivant!
                                next = qoi_GetFirstListSelected(pqoi);                    // Repart au debut
                            }
                        }
                        pqoi.qoiNext = next;
                    }
                    repeatFlag = pqoi.qoiNextFlag;
                    return pHo;
                // Objet suivant global
                case 3:                 // qoi2ndAll
                    pqoi.qoiCurrentOi = pqoi.qoiNext;					// Numero de l'objet suivant
                    pHo = rhPtr.rhObjectList[pqoi.qoiNext];
                    if (pHo != null)
                    {
                        next = pHo.hoNumNext;
                        if (next < 0)
                        {
                            next = qoi_GetNextList(pqoi);
                            if (next < 0)
                            {
                                pqoi.qoiNextFlag = false;					// Plus de suivant
                                next = qoi_GetFirstList(pqoi);			// Repart au debut
                            }
                        }
                        pqoi.qoiNext = next;
                    }
                    repeatFlag = pqoi.qoiNextFlag;
                    return pHo;
            }
        }

        if (pqoi.qoiCurrentOi < 0)
        {
            return null;
        }
        pHo = rhPtr.rhObjectList[pqoi.qoiCurrentOi];
        repeatFlag = pqoi.qoiNextFlag;
        return pHo;
    }

    // Trouve la prochaine liste avec des objets selectionnes
    // ------------------------------------------------------
    short qoi_GetNextListSelected(CQualToOiList pqoi)
    {
        int pos = pqoi.qoiActionPos;
        short qoil;
        CObjInfo oilPtr;
        int pqoi_qoiList_length = pqoi.qoiList.length;
        while (pos < pqoi_qoiList_length)
        {
            qoil = pqoi.qoiList[pos + 1];
            oilPtr = rhPtr.rhOiList[qoil];
            if (oilPtr.oilEventCount == rh2EventCount)	//; Liste vue pendant les conditions?
            {
                pqoi.qoiSelectedFlag = true;						//; Flag: une des liste a ete selectionnee?
                if (oilPtr.oilListSelected >= 0)
                {
                    pqoi.qoiActionPos = (short) (pos + 2);
                    return oilPtr.oilListSelected;
                }
            }
            pos += 2;
        }
        ;										//; La derniere?
        return -1;
    }

    short qoi_GetFirstListSelected(CQualToOiList pqoi)
    {
        pqoi.qoiActionPos = 0;
        pqoi.qoiSelectedFlag = false;
        return qoi_GetNextListSelected(pqoi);
    }

    // Trouve la prochaine liste avec des objets
    // -----------------------------------------
    short qoi_GetNextList(CQualToOiList pqoi)
    {
        int pos = pqoi.qoiActionPos;
        short qoil;
        CObjInfo oilPtr;
        int pqoi_qoiList_length = pqoi.qoiList.length;
        while (pos < pqoi_qoiList_length)
        {
            qoil = pqoi.qoiList[pos + 1];
            oilPtr = rhPtr.rhOiList[qoil];
            if (oilPtr.oilObject >= 0)
            {
                pqoi.qoiActionPos = (short) (pos + 2);
                return oilPtr.oilObject;
            }
            pos += 2;
        }
        ;
        return -1;
    }

    short qoi_GetFirstList(CQualToOiList pqoi)
    {
        pqoi.qoiActionPos = 0;
        return qoi_GetNextList(pqoi);
    }

    public void evt_SaveSelectedObjects(short oiOilList[], ArrayList<SaveSelection> selectedObjects)
	{
	    for (int n = 0; n < oiOilList.length; n += 2)
		{
	        short oil = oiOilList[n + 1];
	        CObjInfo poil = rhPtr.rhOiList[oil];

	        // Selected?
	        if (poil.oilEventCount == rh2EventCount)
			{
	            // Already in the list? 
	            int j;
	            for (j = 0; j < selectedObjects.size(); j++) {
	                if (selectedObjects.get(j).poil == poil)
	                    break;
	            }

	            SaveSelection pSel;
	            if (j < selectedObjects.size())
				{
	                // In the list already => replace selection
	                pSel = selectedObjects.get(j);
	                pSel.selectedInstances.clear();
	            }
	            else
				{
	                // Not in the list yet, add new selection
	                pSel = new SaveSelection(poil);
	                selectedObjects.add(pSel);
	            }

	            // Store selected objects
	            short num = poil.oilListSelected;
	            while (num >= 0)
				{
	                CObject pHoFound = rhPtr.rhObjectList[num];
	                if (pHoFound == null)
	                    break;
	                if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)
	                    pSel.selectedInstances.add((int)num);
	                num = pHoFound.hoNextSelected;
	            }
	        }
	    }
	}

    public void evt_RestoreSelectedObjects(short oiOilList[], ArrayList<SaveSelection> selectedObjects)
	{
	    for (int n = 0; n < oiOilList.length; n += 2)
		{
	        short oil = oiOilList[n + 1];
	        CObjInfo poil = rhPtr.rhOiList[oil];

	        for (int i = 0; i < selectedObjects.size(); i++)
			{
	            SaveSelection sel = selectedObjects.get(i);
	            if (sel.poil == poil)
				{
	                poil.oilEventCount = rh2EventCount;        // TODO: SetObjectEventCount(pRh, poil);
	                poil.oilListSelected = -1;
	                poil.oilNumOfSelected = 0;
	                if (sel.selectedInstances.size() > 0)
					{
	                    CObject pHoPrev = rhPtr.rhObjectList[sel.selectedInstances.get(0)];
	                    if (pHoPrev != null)
						{
	                        poil.oilListSelected = sel.selectedInstances.get(0).shortValue();
	                        poil.oilNumOfSelected++;
	                        for (int j = 1; j < sel.selectedInstances.size(); j++)
							{
	                            short num = sel.selectedInstances.get(j).shortValue();
	                            CObject pHo = rhPtr.rhObjectList[num];
	                            if (pHo == null)
	                                break;
	                            if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)
								{
	                                pHoPrev.hoNextSelected = num;
	                                poil.oilNumOfSelected++;
	                            }
	                            pHoPrev = pHo;
	                        }
	                        pHoPrev.hoNextSelected = -1;
	                    }
	                }
	            }
	        }
	    }
	}

    public void executeChildEvents(PARAM_CHILDEVENT eventParam)
	{
	    // Save object selection and push to stack
	    int newIdx = childEventSelectionStack.size();
	    ArrayList<SaveSelection> selectedObjects = new ArrayList<SaveSelection>();
	    if (newIdx > 0)
		{
            // Copy previous selection
	        ArrayList<SaveSelection> prevSelectedObjects = childEventSelectionStack.get(newIdx - 1);
	        for (int n = 0; n < prevSelectedObjects.size(); n++)
			{
	            SaveSelection prevSel = prevSelectedObjects.get(n);
	            SaveSelection newSel = new SaveSelection(prevSel.poil);
	            for (int j = 0; j < prevSel.selectedInstances.size(); j++) {
	                newSel.selectedInstances.add(prevSel.selectedInstances.get(j));
	            }
	            selectedObjects.add(newSel);
	        }
	    }
	    evt_SaveSelectedObjects(eventParam.ois, selectedObjects);
	    childEventSelectionStack.add(selectedObjects);

	    // Execute child events (only "ALWAYS" events)
	    computeEventList(eventParam.evgOffsetList, null);

	    // Remove object selection from stack
	    childEventSelectionStack.remove(newIdx);
	}

	// ---------------------------------------------------------------------------
    // Entree traitement events non relie a un objet, n'arrete pas le moniteur
    // ---------------------------------------------------------------------------
    public void handle_GlobalEvents(int code)
    {
        int type = (-(short) (code & 0xFFFF));
        int cond = -(int) (short) (code >>> 16);
        int num = listPointers[rhEvents[type] + cond];
        if (num != 0)
        {
            computeEventList(num, null);		// Evalue les evenements
        }
    }
    // ---------------------------------------------------------------------------
    // Entree traitement evenement lie a un objet
    // ---------------------------------------------------------------------------

    public boolean handle_Event(CObject pHo, int code)
    {
        rhCurCode = code; 				// Stocke pour access rapide

        // Des evenements definis?
        // ~~~~~~~~~~~~~~~~~~~~~~~
        int cond = -(int) (short) (code >>> 16);				// Vire le type
        int num = listPointers[pHo.hoEvents + cond];
        if (num != 0)						// Un pointeur direct?
        {
            computeEventList(num, pHo);     // Evalue les evenements
            return true;
        }
        return false;
    }

    // ---------------------------------------------------------------------------
    // 	VERIFIE ET APPELLE LA LISTE DES EVENEMENTS TIMER
    // ---------------------------------------------------------------------------
    public void handle_TimerEvents()
    {
        boolean bDelete = false;
        TimerEvents pEvent=this.rhPtr.rh4TimerEvents;
        while (pEvent!=null)
        {
            if (this.rhPtr.rhTimer>=pEvent.timer)
            {
                if (pEvent.type==TimerEvents.TIMEREVENTTYPE_ONESHOT)
                {
                    this.rhPtr.rhEvtProg.rhCurParamString=pEvent.name;
                    int num = this.listPointers[this.rhEvents[-COI.OBJ_TIMER] + CCnd.NUM_ONEVENT];
                    if (num != 0)
                    {
                        this.computeEventList(num, null);
                    }
                    pEvent.bDelete=true;
                    bDelete = true;
                }
                else
                {
                    if (pEvent.timerPosition==0)
                    {
                        pEvent.timerPosition=this.rhPtr.rhTimer;
                    }
                    while(this.rhPtr.rhTimer>=pEvent.timerPosition)
                    {
                        this.rhPtr.rhEvtProg.rhCurParamString=pEvent.name;
                        this.rhPtr.rhEvtProg.rhCurParam0=pEvent.index;
                        int num = this.listPointers[this.rhEvents[-COI.OBJ_TIMER] + CCnd.NUM_ONEVENT];
                        if (num != 0)
                        {
                            this.computeEventList(num, null);
                        }
                        pEvent.index++;
                        pEvent.loops--;
                        if (pEvent.loops==0)
                        {
                            pEvent.bDelete=true;
                            bDelete = true;
                            break;
                        }
                        pEvent.timerPosition+=pEvent.timerNext;
                    }
                }
            }
            pEvent=pEvent.next;
        }
        if (bDelete)
        {
            pEvent = this.rhPtr.rh4TimerEvents;
            TimerEvents pPrevious = null;
            TimerEvents pNext;
            while (pEvent != null)
            {
                pNext = pEvent.next;
                if (pEvent.bDelete)
                {
                    if (pPrevious == null)
                        this.rhPtr.rh4TimerEvents = pNext;
                    else
                        pPrevious.next = pNext;
                }
                else
                {
                    pPrevious = pEvent;
                }
                pEvent = pNext;
            }
        }
    }

    public void compute_TimerEvents()
    {
        int num;
        //CEventGroup LocaleventPtrGroup[] = eventPointersGroup;
        if(listPointers == null || rhEvents == null)
            return;

        // Avant le fade-in : que les evenements START OF GAME
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        if ((rhPtr.rhGameFlags & CRun.GAMEFLAGS_FIRSTLOOPFADEIN) != 0)
        {
            num = listPointers[rhEvents[-COI.OBJ_GAME] + 1];            // -NUM_START
            if (num != 0)
            {
                listPointers[rhEvents[-COI.OBJ_GAME] + 1] = -1;
                computeEventList(num, null);
                rh4CheckDoneInstart = true;
            }
            return;
        }

        // Les evenements timer
        // ~~~~~~~~~~~~~~~~~~~~
        num = listPointers[rhEvents[-COI.OBJ_TIMER] + 3];               // -NUM_TIMER
        if (num != 0)
        {
            computeEventList(num, null);
        }

        // Les evenements start of game
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        num = listPointers[rhEvents[-COI.OBJ_GAME] + 1];                // -NUM_START
        int num2, count;
        CEventGroup evgPtr, evgGroup;
        CEvent evtPtr;
        if (num != 0)
        {
            if (rh4CheckDoneInstart)
            {
                // Marque DONEBEFOREFADEIN les actions d???ja effectuees : elle ne seront pas r???effectu???es...
                evgGroup = null;
                num2 = num;
                do
                {
                    evgPtr = eventPointersGroup[num2];
                    if (evgPtr != evgGroup)
                    {
                        evgGroup = evgPtr;

                        // Stoppe les actions deja effectuees
                        final int evgPtr_size = evgPtr.evgNCond + evgPtr.evgNAct;
                        for (count = evgPtr.evgNCond; count < evgPtr_size; count++)
                        {
                            evtPtr = evgPtr.evgEvents[count];
                            if ((evtPtr.evtFlags & CEvent.EVFLAGS_NOTDONEINSTART) == 0)		// Une action BAD?
                            {
                                evtPtr.evtFlags |= CEvent.EVFLAGS_DONEBEFOREFADEIN;
                            }
                        }
                    }
                    num2++;
                } while (eventPointersGroup[num2] != null);
            }
            computeEventList(num, null);
            listPointers[rhEvents[-COI.OBJ_GAME] + 1] = 0;		// Une seule fois
            if (rh4CheckDoneInstart)
            {
                // Enleve les flags	
                evgGroup = null;
                num2 = num;
                do
                {
                    evgPtr = eventPointersGroup[num2];
                    if (evgPtr != evgGroup)
                    {
                        evgGroup = evgPtr;
                        // Enleve le flag
                        final int evgPtr_size = evgPtr.evgNCond + evgPtr.evgNAct;
                        for (count = evgPtr.evgNCond; count < evgPtr_size; count++)
                        {
                            evtPtr = evgPtr.evgEvents[count];
                            evtPtr.evtFlags &= ~CEvent.EVFLAGS_DONEBEFOREFADEIN;
                        }
                    }
                    num2++;
                } while (eventPointersGroup[num2] != null);
                rh4CheckDoneInstart = false;
            }
        }

        // Les evenements timer inferieur
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        num = listPointers[rhEvents[-COI.OBJ_TIMER] + 2];            // -NUM_TIMERINF
        if (num != 0)
        {
            computeEventList(num, null);
        }

        // Les evenements timer superieur
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        num = listPointers[rhEvents[-COI.OBJ_TIMER] + 1];            // -NUM_TIMERSUP
        if (num != 0)
        {
            computeEventList(num, null);
        }
    }

    public void restartTimerEvents()
    {
        int time = (int) rhPtr.rhTimer;
        //CEventGroup LocaleventPtrGroup[] = eventPointersGroup;

        int num = listPointers[rhEvents[-COI.OBJ_TIMER] + 3];               // -NUM_TIMER
        CEvent evtPtr;
        CEventGroup evgPtr;
        if (num != 0)
        {
            do
            {
                evgPtr = eventPointersGroup[num];
                evtPtr = evgPtr.evgEvents[eventPointersCnd[num]];
                evtPtr.evtFlags |= CEvent.EVFLAGS_DONE;		    // Marque l'evenement
                PARAM_TIME p = (PARAM_TIME) evtPtr.evtParams[0];
                if (p.timer > time)	// Compare au timer
                {
                    evtPtr.evtFlags &= ~CEvent.EVFLAGS_DONE;
                }
                num++;
            } while (eventPointersGroup[num] != null);
        }
    }

    // ---------------------------------------------------------------------------
    // 	EVALUE ET EXECUTE UNE LISTE D'EVENEMENTS
    // ---------------------------------------------------------------------------
    public void computeEventFastLoopList(CEventGroup pointers[])
    {
        boolean bTrue;
        CEventGroup evgPtr, evgPtr2;
        int num = 0;
        int count;

        do
        {
            evgPtr = pointers[num];
            
            if(evgPtr == null)
            	continue;

            if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_INACTIVE) == 0)	// Un groupe inhibe?
            {
                rhEventGroup = evgPtr;					// Adresse du groupe
                rh4PickFlags[0] = 0;		  		// Pas d'objet choisis dans les evenements
                rh4PickFlags[1] = 0;
                rh4PickFlags[2] = 0;
                rh4PickFlags[3] = 0;

                rh2EventCount += 1;

                for (count = 1; count < evgPtr.evgNCond; count++)
                {
                    if (((CCnd) evgPtr.evgEvents[count]).eva2(rhPtr) == false)
                    {
                        break;
                    }
                }
                if (count == evgPtr.evgNCond)
                {
                    call_Actions(evgPtr);
                }
            }
            num++;
       } while (pointers[num] != null);
    }
    
    public void computeEventList(int num, CObject pHo)
    {
        boolean bTrue;
        //CEventGroup LocaleventPtrGroup[] = eventPointersGroup;
        CEventGroup evgPtr, evgPtr2;
        int count;

        // Evaluation des evenements pour ce sprite
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        rh3DoStop = false;						// En cas de STOP dans les actions
        do
        {
            evgPtr = eventPointersGroup[num];
            
            if(evgPtr == null)
            	continue;

            if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_INACTIVE) == 0)	// Un groupe inhibe?
            {
                rhEventGroup = evgPtr;					// Adresse du groupe
                rh4PickFlags[0] = 0;		  		// Pas d'objet choisis dans les evenements
                rh4PickFlags[1] = 0;
                rh4PickFlags[2] = 0;
                rh4PickFlags[3] = 0;

                // Si pas de OR dans le groupe
                if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_ORINGROUP) == 0)
                {
                    rh2EventCount += 1;
                    rh4ConditionsFalse = false;

                    // Appel de la premiere routine
                    count = 0;
                    if (((CCnd) evgPtr.evgEvents[count]).eva1(rhPtr, pHo))
                    {
                        for (count++; count < evgPtr.evgNCond; count++)
                        {
                            if (((CCnd) evgPtr.evgEvents[count]).eva2(rhPtr) == false)
                            {
                                break;
                            }
                        }
                    }

                    // Appel des actions si le resultat est vrai
                    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    if (count == evgPtr.evgNCond)
                    {
                        if (rh3DoStop)                             	// Groupe FAUX, mais faire event STOP?
                        {
                            // Appeler les actions STOP seulement?
                            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            if (pHo != null)					// Seulement si un objet defini...
                            {
                                call_Stops(pHo);
                            }
                        }
                        else
                        {
                            call_Actions(evgPtr);
							if ( (evgPtr.evgFlags & CEventGroup.EVGFLAGS_BREAK) != 0 )
							{
								break;
							}
                        }
                    }
                    num++;
                }
                else
                {
                    rh4EventCountOR++;
                    if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_ORLOGICAL) == 0)
                    {
                        bTrue = false;
                        do
                        {
                            rh2EventCount++;
                            rh4ConditionsFalse = false;
                            count = eventPointersCnd[num];

                            if (((CCnd) evgPtr.evgEvents[count]).eva1(rhPtr, pHo) == false)
                            {
                                rh4ConditionsFalse = true;
                            }

                            count++;
                            while (count < evgPtr.evgNCond && evgPtr.evgEvents[count].evtCode != ((-24 << 16) | 65535))	 // CNDL_OR
                            {
                                if (((CCnd) evgPtr.evgEvents[count]).eva2(rhPtr) == false)
                                {
                                    rh4ConditionsFalse = true;
                                }
                                count++;
                            }

                            evt_MarkSelectedObjects();						// Stocke les objets
                            if (rh4ConditionsFalse == false)
                            {
                                bTrue = true;
                            }

                            num++;
                            evgPtr = eventPointersGroup[num];
                            if (evgPtr == null)
                            {
                                break;
                            }
                        } while (evgPtr == rhEventGroup);

                        if (bTrue)
                        {
                            rh2EventCount++;
                            evt_BranchSelectedObjects();		// Branche tous les objets
							boolean bBreak = ((rhEventGroup.evgFlags & CEventGroup.EVGFLAGS_BREAK) != 0);
                            call_Actions(rhEventGroup);				// Appelle les actions
							if ( bBreak )
							{
								break;
							}
                        }
                    }
                    else
                    {
                        boolean bFalse;
                        rh4ConditionsFalse = false;

                        bTrue = false;
                        do
                        {
                            rh2EventCount++;
                            bFalse = false;
                            count = eventPointersCnd[num];

                            if (((CCnd) evgPtr.evgEvents[count]).eva1(rhPtr, pHo))
                            {
                                count++;
                                while (count < evgPtr.evgNCond && evgPtr.evgEvents[count].evtCode != ((-25 << 16) | 65535))     // CNDL_ORLOGICAL
                                {
                                    if (((CCnd) evgPtr.evgEvents[count]).eva2(rhPtr) == false)
                                    {
                                        bFalse = true;
                                        break;
                                    }
                                    count++;
                                }
                            }
                            else
                            {
                                bFalse = true;
                            }

                            if (bFalse == false)
                            {
                                evt_MarkSelectedObjects();						// Stocke les objets
                                bTrue = true;
                            }

                            num++;
                            evgPtr = eventPointersGroup[num];
                            if (evgPtr == null)
                            {
                                break;
                            }

                        } while (evgPtr == rhEventGroup);

                        if (bTrue)
                        {
                            rh2EventCount++;
                            evt_BranchSelectedObjects();		// Branche tous les objets
							boolean bBreak = ((rhEventGroup.evgFlags & CEventGroup.EVGFLAGS_BREAK) != 0);
                            call_Actions(rhEventGroup);				// Appelle les actions
							if ( bBreak )
							{
								break;
							}
                        }
                    }
                }
            }
            else
            {
                // Si inactif, saute tous les groupes de condition
                num++;
                if (eventPointersGroup[num] != null)
                {
                    evgPtr2 = eventPointersGroup[num];
                    while (evgPtr2 == evgPtr)
                    {
                        num++;
                        if (eventPointersGroup[num] == null)
                        {
                            break;
                        }
                        evgPtr2 = eventPointersGroup[num];
                    }
                }
            }
        } while (eventPointersGroup[num] != null);
    }

    // ---------------------------------------------------------------------------
    // EXECUTION DES ACTIONS
    // ---------------------------------------------------------------------------
    void call_Actions(CEventGroup pEvg)
    {
		childEventParam = null;

        // Gestion des flags ONCE/NOT ALWAYS/REPEAT/NO MORE
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_LIMITED) != 0)   				// Les actions limitees et validees
        {
            // Flag SHUFFLE
            if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_SHUFFLE) != 0)
            {
                rh2ShuffleBuffer = new ArrayList<CObject>();
            }

            // Flag NOT ALWAYS
            if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_NOTALWAYS) != 0)
            {
                int w_cx = rhPtr.rhLoopCount;
                int w_dx = pEvg.evgInhibit;
                pEvg.evgInhibit = w_cx;
                if (w_cx == w_dx)
                {
                    return;
                }
                w_cx -= 1;
                if (w_cx == w_dx)
                {
                    return;
                }
            }

            // Flag REPEAT
            if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_REPEAT) != 0)
            {
                if (pEvg.evgInhibitCpt != 0)		// sur?
                {
                    pEvg.evgInhibitCpt--;
                }
                else
                {
                    return;
                }
            }

            // Flag NO MORE during
            if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_NOMORE) != 0)
            {
                int dwt = ((int) rhPtr.rhTimer / 10);		// Timer courant / 10
                int dwmax = pEvg.evgInhibitCpt;			// Timer maximum
                if (dwmax != 0 && dwt < dwmax)		// Pas encore pret!
                {
                    return;
                }
                pEvg.evgInhibitCpt = (dwt + pEvg.evgInhibit);		// Plus timer possible= timer maxi
            }
        }


        // Premiere execution : toutes les actions
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        rh2ActionCount++;				// Marqueur de boucle d'actions
        rh2ActionLoop = false;				// Flag boucle
        rh2ActionLoopCount = 0;                         // Numero de boucle d'actions
        rh2ActionOn = true;			 	// On est dans les actions
        int count = 0;                    		// Nombre d'actions
        CAct actPtr;
        do
        {
            actPtr = (CAct) pEvg.evgEvents[count + pEvg.evgNCond];
            if ((actPtr.evtFlags & (CEvent.EVFLAGS_BADOBJECT | CEvent.EVFLAGS_DONEBEFOREFADEIN)) == 0)		// Une action BAD?
            {
                actPtr.evtFlags &= ~CEvent.EVFLAGS_REPEAT;
                actPtr.execute(rhPtr);
            }
            count++;
        } while (count < pEvg.evgNAct);

        if (rh2ActionLoop)		// Encore des actions a faire?
        {
            // Deuxieme execution : juste les actions avec un flag
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            do
            {
                rh2ActionLoop = false;				// Flag boucle
                rh2ActionLoopCount++;                          // Numero de boucle
                count = 0;
                do
                {
                    actPtr = (CAct) pEvg.evgEvents[count + pEvg.evgNCond];
                    if ((actPtr.evtFlags & CEvent.EVFLAGS_REPEAT) != 0)			// Action repeat?
                    {
                        actPtr.evtFlags &= ~CEvent.EVFLAGS_REPEAT;
                        actPtr.execute(rhPtr);
                    }
                    count++;
                } while (count < pEvg.evgNAct);
            } while (rh2ActionLoop);			// Encore des actions a faire?
        }

        // Appeler la routine de fin?
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~
		PARAM_CHILDEVENT tempChildEventParam = childEventParam;
        if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_BREAK) != 0 && (pEvg.evgFlags & CEventGroup.EVGFLAGS_HASPARENT) != 0)
            tempChildEventParam = null;
		childEventParam = null;
        rh2ActionOn = false;			 	// On est plus les actions
        if (rh2ShuffleBuffer != null)
            endShuffle();
        if (this.callEndForEach == true)
            endForEach();
		if (tempChildEventParam != null)
			executeChildEvents(tempChildEventParam);
	}

    // ---------------------------------------------------------------------------
    // EXECUTION UNIQUEMENT DES ACTIONS STOP ET BOUNCE POUR UN SEUL OBJET ESI
    // ---------------------------------------------------------------------------
    void call_Stops(CObject pHo)
    {
        short oi;

        // On ne traite qu'un seul objet!
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        oi = pHo.hoOi;							// L'Oi en question
        rh2EventCount += 1;                                             // des actions...
        evt_AddCurrentObject(pHo);

        // Premiere execution : toutes les actions STOP
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        rh2ActionCount++;					// Marqueur de boucle d'actions
        rh2ActionLoop = false;					// Flag boucle
        rh2ActionLoopCount = 0;			// Numero de boucle d'actions
        rh2ActionOn = true;					// On est dans les actions
        CAct actPtr;
        int count = 0;
        int num, numOi;
        do
        {
            actPtr = (CAct) rhEventGroup.evgEvents[rhEventGroup.evgNCond + count];
            num = actPtr.evtCode & 0xFFFF0000;
            if (num == (4 << 16) || num == (9 << 16))       // ACTL_EXTSTOP - ACTL_EXTBOUNCE
            {
                if (oi == actPtr.evtOi)
                {
                    actPtr.execute(rhPtr);
                }
                else
                {
                    short oil = actPtr.evtOiList;			// Un qualifier?
                    if ((oil & 0x8000) != 0)
                    {
                        CQualToOiList pq = qualToOiList[oil & 0x7FFF];
                        numOi = 0;
                        while (numOi < pq.qoiList.length)
                        {
                            if (pq.qoiList[numOi] == oi)
                            {
                                actPtr.execute(rhPtr);
                                break;
                            }
                            numOi += 2;
                        }
                        ;
                    }
                }
            }
            count++;
        } while (count < rhEventGroup.evgNAct);
        rh2ActionOn = false;					// On est dans les actions
    }

    // Fin des actions shuffle : melange!
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~--------
    void endShuffle()
    {
    	// 28/10/2014 V283.3 Last round of performance
    	ArrayList<CObject> localShuffleBuffer = rh2ShuffleBuffer;
    	int rh2ShuffleBuffer_size = localShuffleBuffer.size();
    	if (rh2ShuffleBuffer_size <= 1)
        {
            return;
        }

        int num1 = rhPtr.random((short) rh2ShuffleBuffer_size);
        int num2;
        do
        {
            num2 = rhPtr.random((short) rh2ShuffleBuffer_size);
        } while (num1 == num2);

        CObject pHo1 = localShuffleBuffer.get(num1);
        CObject pHo2 = localShuffleBuffer.get(num2);

        // Echange les sprites
        int x1 = pHo1.hoX;
        int y1 = pHo1.hoY;
        int x2 = pHo2.hoX;
        int y2 = pHo2.hoY;
        CRun.setXPosition(pHo1, x2);
        CRun.setYPosition(pHo1, y2);
        CRun.setXPosition(pHo2, x1);
        CRun.setYPosition(pHo2, y1);
        rh2ShuffleBuffer.clear();
        rh2ShuffleBuffer = null;
    }

    // --------------------------------------------------------------------------
    // EXTERN EVENTS ENTRY
    // --------------------------------------------------------------------------
    // Menu
    public boolean onCommand(short id)
    {
        if (rhPtr == null)
        {
            return false;
        }
        if (rhPtr.rh2PauseCompteur != 0)
        {
            return false;
        }
        if (bReady == false)
        {
            return false;
        }

        rhCurParam0 = id;
        rh3CurrentMenu = id;		    	// Pour les evenements II
        rhPtr.rh4MenuEaten = false;			// Pour bloquer l'option de menu
        handle_GlobalEvents(((-14 << 16) | 65535));		// CNDL_MENUSELECTED

        return rhPtr.rh4MenuEaten;
    }
	public void onMouseButton(int b, int nClicks) {
		int mouse;

		if (rhPtr == null) {
			return;
		}
		if (rhPtr.rh2PauseCompteur != 0) {
            wakeup(0);
		}
		if (bReady == false) {
			return;
		}

		// Un evenement a traiter?
		// -----------------------
		mouse = b;
		if (nClicks == 2) {
			mouse += PARAMCLICK_DOUBLE;
		}

		rhPtr.rh4TimeOut = 0; // Plus de time-out!

		// Genere les evenements dans le jeu
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		rhCurParam0 = mouse;
		rh2CurrentClick = (short) mouse; // Pour les evenements II
		handle_GlobalEvents(((-5 << 16) | 0xFFFA)); // CNDL_MCLICK Evenement
													// click normal
		handle_GlobalEvents(((-6 << 16) | 0xFFFA)); // CNDL_MCLICKINZONE
													// Evenement click sur une
													// zone

		// Explore les sprites en collision
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		int mx = rhPtr.rhApp.mouseX;
		int my = rhPtr.rhApp.mouseY;

		CSprite spr = null;
		ArrayList<CSprite> list = new ArrayList<CSprite>();
		while (true) {
			spr = rhPtr.spriteGen.spriteCol_TestPoint(spr,
					CSpriteGen.LAYER_ALL, mx, my, 0);
			if (spr == null) {
				break;
			}
			list.add(spr);
		}

		int count;

		int list_size = list.size();
		for (count = 0; count < list_size; count++) {
			spr = list.get(count);
			CObject pHo = spr.sprExtraInfo;
			if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0) {
				rhCurParam1 = pHo.hoOi;
				rh4_2ndObject = pHo;
				handle_GlobalEvents(((-7 << 16) | 0xFFFA)); // CNDL_MCLICKONOBJECT
			}
		}

		// Explore les autres objets en collision
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		// 283.2 increasing performance
		count = rhPtr.rhNObjects;
		CObject[] localObjectList = rhPtr.rhObjectList;
		for (CObject pHo : localObjectList)
		{
			if(pHo != null) {
				--count;
				if ((pHo.hoFlags & (CObject.HOF_REALSPRITE | CObject.HOF_OWNERDRAW)) == 0) {
					int x = pHo.hoX - pHo.hoImgXSpot;
	                                if(x <= mx
							&& (x + pHo.hoImgWidth > mx)) {
						int y = pHo.hoY - pHo.hoImgYSpot;
						if (y <= my
								&& (y + pHo.hoImgHeight > my)) {
							if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0) {
								rhCurParam1 = pHo.hoOi;
								rh4_2ndObject = pHo;
								handle_GlobalEvents(((-7 << 16) | 0xFFFA)); // CNDL_MCLICKONOBJECT
							}
						}
					}
				}
				if(count == 0)
					break;
			}
		}
	}

	public void wakeup(int vk)
	{
		//if (rhPtr != null)
		//{
		if (bReady == false)
		{
			return;
		}

		if (rhPtr.rh2PauseCompteur != 0)
		{
			if (rhPtr.rh4PauseKey == -1)
			{
				rhPtr.resume();
				rhPtr.rh4EndOfPause = rhPtr.rhLoopCount;	    // Pour les evenements II
				handle_GlobalEvents(((-8 << 16) | 0xFFFD));	    // CNDL_ENDOFPAUSE
			}
			if (rhPtr.rh4PauseKey != 0 && rhPtr.rh4PauseKey == vk)
			{
				rhPtr.resume();
				rhPtr.rh4EndOfPause = rhPtr.rhLoopCount;	    // Pour les evenements II
				handle_GlobalEvents(((-8 << 16) | 0xFFFD));	    // CNDL_ENDOFPAUSE
			}
			return;
		}

		rhPtr.rh4TimeOut = 0;				    // Plus de time-out!
	}

    public void onKeyDown(int vk)
    {
        if (rhPtr != null)
        {
            wakeup(vk);

            handle_GlobalEvents(((-9 << 16) | 0xFFFA));		    // CNDL_ANYKEY
        }
    }

    // MouseMove
    public void onMouseMove()
    {
        if (rhPtr != null)
        {
            if (bReady == false)
            {
                return;
            }
            if (rhPtr.rh2PauseCompteur != 0)
            {
                return;
            }
            rhPtr.rh4TimeOut = 0;
        }
    }

    boolean ctoCompare(PARAM_ZONE pZone, CObject pHo)
    {
        if (pHo.hoImgWidth == 0 || pHo.hoImgHeight == 0)
        {
            return false;
        }
        if (pHo.hoX < pZone.x1 || pHo.hoX >= pZone.x2)
        {
            return false;
        }
        if (pHo.hoY < pZone.y1 || pHo.hoY >= pZone.y2)
        {
            return false;
        }
        return true;
    }

    public CObject count_ZoneTypeObjects(PARAM_ZONE pZone, int stop, short type)
    {
        stop++;
        evtNSelectedObjects = 0;

        int oil = 0;
        CObjInfo poilLoop = null;
        CObject pHo;
        int rhPtr_rhOiList_length = rhPtr.rhOiList.length;
        do
        {
            for (; oil < rhPtr_rhOiList_length; oil++)
            {
                poilLoop = rhPtr.rhOiList[oil];
                if (type == 0 || (type != 0 && type == poilLoop.oilType))
                {
                    break;
                }
            }
            if (oil == rhPtr_rhOiList_length)
            {
                return null;
            }

            CObjInfo poil = poilLoop;
            oil++;

            if (poil.oilEventCount != rh2EventCount)
            {
                if (rh4ConditionsFalse == false)
                {
                    // Explore la liste entiere des objets
                    short num = poil.oilObject;
                    while (num >= 0)
                    {
                        pHo = rhPtr.rhObjectList[num];
                        if (pHo == null)
                        {
                            return null;
                        }
                        if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                        {
                            if (ctoCompare(pZone, pHo))
                            {
                                evtNSelectedObjects++;
                                if (evtNSelectedObjects == stop)
                                {
                                    return pHo;
                                }
                            }
                        }
                        num = pHo.hoNumNext;
                    }
                }
            }
            else
            {
                // Explore la liste des objets selectionnes
                short num = poil.oilListSelected;
                while (num >= 0)
                {
                    pHo = rhPtr.rhObjectList[num];
                    if (pHo == null)
                    {
                        return null;
                    }
                    if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                    {
                        if (ctoCompare(pZone, pHo))
                        {
                            evtNSelectedObjects++;
                            if (evtNSelectedObjects == stop)
                            {
                                return pHo;
                            }
                        }
                    }
                    num = pHo.hoNextSelected;
                }
            }
        } while (true);
    }

    // ----------------------------------------
    // Compte / Trouve des objets de type AX
    // ----------------------------------------
    public CObject count_ObjectsFromType(short type, int stop)
    {
        stop++;									// BX a partir de 1!
        evtNSelectedObjects = 0;

        int oil = 0;
        CObjInfo poilLoop = null;
        CObject pHo;
        CObjInfo[] localOilList = rhPtr.rhOiList;
        int rhPtr_rhOiList_length = localOilList.length;
        do
        {
            for (; oil < rhPtr_rhOiList_length; oil++)
            {
                poilLoop = localOilList[oil];
                if (type == 0 || (type != 0 && type == poilLoop.oilType))
                {
                    break;
                }
            }
            if (oil == rhPtr_rhOiList_length)
            {
                return null;
            }

            CObject[] localObjectList = rhPtr.rhObjectList;
            CObjInfo poil = poilLoop;
            oil++;

            if (poil.oilEventCount != rh2EventCount)
            {
                if (rh4ConditionsFalse == false)
                {
                    // Explore la liste entiere des objets
                    short num = poil.oilObject;
                    while (num >= 0)
                    {
                        pHo = localObjectList[num];
                        if (pHo == null)
                        {
                            return null;
                        }
                        if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                        {
                            evtNSelectedObjects++;
                            if (evtNSelectedObjects == stop)
                            {
                                return pHo;
                            }
                        }
                        num = pHo.hoNumNext;
                    }
                }
            }
            else
            {
                // Explore la liste des objets selectionnes
                short num = poil.oilListSelected;
                while (num >= 0)
                {
                    pHo = localObjectList[num];
                    if (pHo == null)
                    {
                        return null;
                    }
                    if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                    {
                        evtNSelectedObjects++;
                        if (evtNSelectedObjects == stop)
                        {
                            return pHo;
                        }
                    }
                    num = pHo.hoNextSelected;
                }
            }
        } while (true);
    }

    // Routine de test de la zone
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~
    boolean czaCompare(PARAM_ZONE pZone, CObject pHo)
    {
        if (pHo.hoX < pZone.x1 || pHo.hoX >= pZone.x2)
        {
            return false;
        }
        if (pHo.hoY < pZone.y1 || pHo.hoY >= pZone.y2)
        {
            return false;
        }
        return true;
    }

    public int select_ZoneTypeObjects(PARAM_ZONE p, short type)
    {
        int cpt = 0;

        int oil = 0;
        CObjInfo poilLoop = null;
        CObject pHoLoop, pHoFound;
        short num;
        CObjInfo[] localOilList = rhPtr.rhOiList;
        CObject[] localObjectList = rhPtr.rhObjectList;
        int rhPtr_rhOiList_length = localOilList.length;
        do
        {
            for (; oil < rhPtr_rhOiList_length; oil++)
            {
                poilLoop = localOilList[oil];
                if (type == 0 || (type != 0 && type == poilLoop.oilType))
                {
                    break;
                }
            }
            if (oil == rhPtr_rhOiList_length)
            {
                return cpt;
            }

            CObjInfo poil = poilLoop;
            oil++;

            if (poil.oilEventCount != rh2EventCount)
            {
                // Explore la liste entiere des objets, et branche les objets dans la zone
                pHoLoop = null;
                poil.oilNumOfSelected = 0;
                poil.oilEventCount = rh2EventCount;
                poil.oilListSelected = -1;
                if (rh4ConditionsFalse == false)
                {
                    num = poil.oilObject;
                    while (num >= 0)
                    {
                        pHoFound = localObjectList[num];
                        if (pHoFound == null)
                        {
                            break;
                        }
                        if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                        {
                            if (czaCompare(p, pHoFound))
                            {
                                cpt++;
                                poil.oilNumOfSelected++;
                                pHoFound.hoNextSelected = -1;
                                if (pHoLoop == null)
                                {
                                    poil.oilListSelected = pHoFound.hoNumber;
                                }
                                else
                                {
                                    pHoLoop.hoNextSelected = pHoFound.hoNumber;
                                }
                                pHoLoop = pHoFound;
                            }
                        }
                        num = pHoFound.hoNumNext;
                    }
                    //;
                }
                continue;
            }

            // Explore la liste des objets selectionnes, et vire les objets non dans la zone
            pHoLoop = null;								// Pour le premier!
            num = poil.oilListSelected;
            while (num >= 0)
            {
                pHoFound = localObjectList[num];
                if (pHoFound == null)
                {
                    break;
                }
                if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)			// Deja detruit?
                {
                    if (czaCompare(p, pHoFound) == false)
                    {
                        poil.oilNumOfSelected--;					// Un de moins!
                        if (pHoLoop == null)
                        {
                            poil.oilListSelected = pHoFound.hoNextSelected;
                        }
                        else
                        {
                            pHoLoop.hoNextSelected = pHoFound.hoNextSelected;	//; oil.oilListSelected IDEM!
                        }
                    }
                    else
                    {
                        cpt++;
                        pHoLoop = pHoFound;
                    }
                }
                num = pHoFound.hoNextSelected;
            }
            ;
            continue;
        } while (true);
    }

    // ---------------------------------------------------
    // Ligne de vue : selectionne les objets sur une ligne
    // ---------------------------------------------------
    boolean losCompare(double x1, double y1, double x2, double y2, CObject pHo)
    {
        double delta;
        int x, y;

        int xLeft = pHo.hoX - pHo.hoImgXSpot;
        int xRight = xLeft + pHo.hoImgWidth;
        int yTop = pHo.hoY - pHo.hoImgYSpot;
        int yBottom = yTop + pHo.hoImgHeight;

        if (x2 - x1 > y2 - y1)
        {
            delta = (y2 - y1) / (x2 - x1);
            if (x2 > x1)
            {
                if (xRight < x1 || xLeft >= x2)
                {
                    return false;
                }
            }
            else
            {
                if (xRight < x2 || xLeft >= x1)
                {
                    return false;
                }
            }
            y = (int) (delta * (xLeft - x1) + y1);
            if (y >= yTop && y < yBottom)
            {
                return true;
            }

            y = (int) (delta * (xRight - x1) + y1);
            if (y >= yTop && y < yBottom)
            {
                return true;
            }

            return false;
        }
        else
        {
            delta = (x2 - x1) / (y2 - y1);
            if (y2 > y1)
            {
                if (yBottom < y1 || yTop >= y2)
                {
                    return false;
                }
            }
            else
            {
                if (yBottom < y2 || yTop >= y1)
                {
                    return false;
                }
            }
            x = (int) (delta * (yTop - y1) + x1);
            if (x >= xLeft && x < xRight)
            {
                return true;
            }

            x = (int) (delta * (yTop - y1) + x1);
            if (x >= xLeft && x < xRight)
            {
                return true;
            }

            return false;
        }
    }

    public int select_LineOfSight(int x1, int y1, int x2, int y2)
    {
        int cpt = 0;

        // Exploration de la liste des objets
        CObjInfo poil;
        int oil;
        CObject pHoLoop, pHoFound;
        short num;
        CObjInfo[] localOilList = rhPtr.rhOiList;
        CObject[] localObjectList = rhPtr.rhObjectList;
        int rhPtr_rhOiList_length = localOilList.length;
        for (oil = 0; oil < rhPtr_rhOiList_length; oil++)
        {
            poil = localOilList[oil];
            if (poil.oilEventCount != rh2EventCount)
            {
                // Explore la liste entiere des objets, et branche les objets dans la zone
                pHoLoop = null;
                poil.oilNumOfSelected = 0;
                poil.oilEventCount = rh2EventCount;
                poil.oilListSelected = -1;

                // Si condition OR et conditions fausse, ne selectionne aucun objet
                if (rh4ConditionsFalse == false)
                {
                    num = poil.oilObject;
                    while (num >= 0)
                    {
                        pHoFound = localObjectList[num];
                        if (pHoFound == null)
                        {
                            break;
                        }
                        if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                        {
                            if (losCompare(x1, y1, x2, y2, pHoFound))
                            {
                                cpt++;
                                poil.oilNumOfSelected++;
                                pHoFound.hoNextSelected = -1;
                                if (pHoLoop == null)
                                {
                                    poil.oilListSelected = pHoFound.hoNumber;
                                }
                                else
                                {
                                    pHoLoop.hoNextSelected = pHoFound.hoNumber;		//; Car idem oilListSelected!
                                }
                                pHoLoop = pHoFound;
                            }
                        }
                        num = pHoFound.hoNumNext;
                    }
                }
                continue;
            }

            // Explore la liste des objets selectionnes, et vire les objets non dans la zone
            pHoLoop = null;								// Pour le premier!
            num = poil.oilListSelected;
            while (num >= 0)
            {
                pHoFound = localObjectList[num];
                if (pHoFound == null)
                {
                    break;
                }
                if ((pHoFound.hoFlags & CObject.HOF_DESTROYED) == 0)			// Deja detruit?
                {
                    if (losCompare(x1, y1, x2, y2, pHoFound) == false)
                    {
                        poil.oilNumOfSelected--;					// Un de moins!
                        if (pHoLoop == null)
                        {
                            poil.oilListSelected = pHoFound.hoNextSelected;
                        }
                        else
                        {
                            pHoLoop.hoNextSelected = pHoFound.hoNextSelected;
                        }
                    }
                    else
                    {
                        cpt++;
                        pHoLoop = pHoFound;
                    }
                }
                num = pHoFound.hoNextSelected;
            }

        }
        return cpt;
    }

    // ----------------------------------------
    // Compte / Trouve un objet dans une zone
    // ----------------------------------------

    // Routine de comptage
    public int czoCountThem(short oil, PARAM_ZONE pZone)
    {
        int count = 0;
        CObject[] localObjectList = rhPtr.rhObjectList;
        CObjInfo poil = rhPtr.rhOiList[oil];
        CObject pHo;
        if (poil.oilEventCount != rh2EventCount)
        {
            if (rh4ConditionsFalse == false)
            {
                // Explore la liste entiere des objets
                short num = poil.oilObject;
                while (num >= 0)
                {
                    pHo = localObjectList[num];
                    if (pHo == null)
                    {
                        return 0;
                    }
                    if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                    {
                        if (czaCompare(pZone, pHo))
                        {
                            count++;
                        }
                    }
                    num = pHo.hoNumNext;
                }
                ;
            }
            return count;
        }

        // Explore la liste des objets selectionnes
        short num = poil.oilListSelected;
        while (num >= 0)
        {
            pHo = localObjectList[num];
            if (pHo == null)
            {
                return 0;
            }
            if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
            {
                if (czaCompare(pZone, pHo))
                {
                    count++;
                }
            }
            num = pHo.hoNextSelected;
        }
        ;
        return count;
    }

    public int count_ZoneOneObject(short oil, PARAM_ZONE pZone)
    {
        // Un objet normal
        if (oil >= 0)
        {
            return czoCountThem(oil, pZone);
        }

        // Un qualifier
        if (oil == -1)
        {
            return 0;
        }
        CQualToOiList pqoi = qualToOiList[oil & 0x7FFF];
        int qoi;
        int count = 0;
        int pqoi_qoiList_length = pqoi.qoiList.length;
        for (qoi = 0; qoi < pqoi_qoiList_length; qoi += 2)
        {
            count += czoCountThem(pqoi.qoiList[qoi + 1], pZone);
        }
        return count;
    }

    // ----------------------------------------
    // Compte / Trouve de objets definiss
    // ----------------------------------------
    // Routine de comptage
    // ~~~~~~~~~~~~~~~~~~~
    CObject countThem(short oil, int stop)
    {
        CObject[] localObjectList = rhPtr.rhObjectList;
        CObjInfo poil = rhPtr.rhOiList[oil];		// Pointe la liste
        CObject pHo;
        if (poil.oilEventCount != rh2EventCount)
        {
            // Si condition OU
            if (rh4ConditionsFalse)
            {
                evtNSelectedObjects = 0;
                return null;
            }

            // Explore la liste entiere des objets
            short num = poil.oilObject;
            while (num >= 0)
            {
                pHo = localObjectList[num];
                if (pHo == null)
                {
                    return null;
                }
                if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)		// Deja detruit?
                {
                    evtNSelectedObjects++;
                    if (evtNSelectedObjects == stop)
                    {
                        return pHo;
                    }
                }
                num = pHo.hoNumNext;
            }
            return null;
        }

        // Explore la liste des objets selectionnes
        short num = poil.oilListSelected;
        while (num >= 0)
        {
            pHo = localObjectList[num];
            if (pHo == null)
            {
                return null;
            }
            if ((pHo.hoFlags & CObject.HOF_DESTROYED) == 0)			// Deja detruit?
            {
                evtNSelectedObjects++;
                if (evtNSelectedObjects == stop)
                {
                    return pHo;
                }
            }
            num = pHo.hoNextSelected;
        }
        return null;
    }

    public CObject count_ObjectsFromOiList(short oil, int stop)
    {
        stop++;									// BX a partir de 1!
        evtNSelectedObjects = 0;
        if (oil >= 0)
        {
            // Un identifier normal
            return countThem(oil, stop);
        }

        // Un qualifier
        if (oil == -1)
        {
            return null;
        }
        CQualToOiList pqoi = qualToOiList[oil & 0x7FFF];
        int qoi;
        int pqoi_qoiList_length = pqoi.qoiList.length;
        for (qoi = 0; qoi < pqoi_qoiList_length; qoi += 2)
        {
            CObject pHo = countThem(pqoi.qoiList[qoi + 1], stop);
            if (pHo != null)
            {
                return pHo;
            }
        }
        return null;
    }

    // Pick un objet a partir de son fixed value
    // -----------------------------------------
    public boolean pickFromId(int value)
    {
        CObject[] localObjectList = rhPtr.rhObjectList;
        int number = value & 0xFFFF;
        if (number >= rhPtr.rhMaxObjects)
        {
            return false;
        }
        CObject pHo = localObjectList[number];
        if (pHo == null)
        {
            return false;
        }

        int code = value >>> 16;
        if (code != pHo.hoCreationId)
        {
            return false;
        }

        // Dans une liste selectionnee ou pas?
        CObjInfo poil = pHo.hoOiList;
        if (poil.oilEventCount == rh2EventCount)
        {
            short next = poil.oilListSelected;
            CObject pHoFound = null;
            while (next >= 0)
            {
                pHoFound = localObjectList[next];
                if (pHo == pHoFound)
                {
                    break;
                }
                next = pHoFound.hoNextSelected;
            }
            ;
            if (pHo != pHoFound)
            {
                return false;
            }
        }
        poil.oilEventCount = rh2EventCount;			// Seul sur la liste!
        poil.oilListSelected = -1;
        poil.oilNumOfSelected = 0;
        pHo.hoNextSelected = -1;
        evt_AddCurrentObject(pHo);
        return true;
    }

    // ---------------------------------------------------------------------------
    // Pousse un evenement pour la fin du cycle
    // ---------------------------------------------------------------------------
    public void push_Event(int routine, int code, int lParam, CObject pHo, short oi)
    {
        CPushedEvent p = new CPushedEvent(routine, code, lParam, pHo, oi);
        if (rh2PushedEvents == null)
        {
            rh2PushedEvents = new ArrayList<CPushedEvent>();
        }
        rh2PushedEvents.add(p);
    }
    // ---------------------------------------------------------------------------
    // Traite tous les evenements pousse
    // ---------------------------------------------------------------------------

    public void handle_PushedEvents()
    {
        if (rh2PushedEvents != null)
        {
            int index;
            for (index = 0; index < rh2PushedEvents.size(); index++)
            {
                CPushedEvent pEvt = rh2PushedEvents.get(index);
                if (pEvt != null)
                {
                    if (pEvt.code != 0)
                    {
                        // Effectue l'un des evenements
                        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        rhCurParam0 = pEvt.param;
                        rhCurOi = pEvt.oi;
                        switch (pEvt.routine)
                        {
                            case 0:
                                handle_GlobalEvents(pEvt.code);
                                break;
                            case 1:
                                handle_Event(pEvt.pHo, pEvt.code);
                                break;
                        }
                    }
                }
            }
            rh2PushedEvents.clear();
        }
    }

    // --------------------------------------------------------------------------
    // CHARGEMENT DES EVENEMENTS
    // --------------------------------------------------------------------------
    public void load(CRunApp app)
    {
        byte code[] = new byte[4];
        int number;
        int n;
        int eventPos = 0;
        while (true)
        {
            app.file.read(code);

            // EVTFILECHUNK_HEAD
            if (code[0] == 'E' && code[1] == 'R' && code[2] == '>' && code[3] == '>')
            {
                maxObjects = app.file.readAShort();
                if (maxObjects < 300)
                {
                    maxObjects = 300;
                }
                maxOi = app.file.readAShort();
                nPlayers = app.file.readAShort();
                for (n = 0; n < 7 + COI.OBJ_LAST; n++)
                {
                    nConditions[n] = app.file.readAShort();
                }
                nQualifiers = app.file.readAShort();
                if (nQualifiers > 0)
                {
                    qualifiers = new CLoadQualifiers[nQualifiers];
                    for (n = 0; n < nQualifiers; n++)
                    {
                        qualifiers[n] = new CLoadQualifiers();
                        qualifiers[n].qOi = app.file.readAShort();
                        qualifiers[n].qType = app.file.readAShort();
                    }
                }
            }
            // EVTFILECHUNK_EVTHEAD
            else if (code[0] == 'E' && code[1] == 'R' && code[2] == 'e' && code[3] == 's')
            {
                app.file.readAInt();
                nEvents = app.file.readAInt();
                events = new CEventGroup[nEvents];
                eventPos = 0;
            }
            // EVTFILECHUNK_EVENTS
            else if (code[0] == 'E' && code[1] == 'R' && code[2] == 'e' && code[3] == 'v')
            {
                app.file.readAInt();
                number = app.file.readAInt();
                for (n = 0; n < number; n++)
                {
                    events[eventPos] = CEventGroup.create(app);
                    eventPos++;
                }
            }
			else if (code[0] == 'E' && code[1] == 'R' && code[2] == 'o' && code[3] == 'p')
			{
				int options;
				options = app.file.readAInt();
				if ((options & CEventProgram.EVENTOPTION_BREAKCHILD) != 0)
				{
					this.defaultEvgFlagsMask |= CEventGroup.EVGFLAGS_BREAK;
				}
			}
            // EVTFILECHUNK_END
            else if (code[0] == '<' && code[1] == '<' && code[2] == 'E' && code[3] == 'R')
            {
                break;
            }
        }
    }

    // Relocation des pathnames
    public void relocatePath(CRunApp app)
    {
        int evg, evt, evp;
        CEventGroup evgPtr;
        CEvent evtPtr;
        CParam evpPtr;
        int events_length = events.length;
        for (evg = 0; evg < events_length; evg++)
        {
            evgPtr = events[evg];
            int evgPtr_size = evgPtr.evgNCond + evgPtr.evgNAct;
            for (evt = 0; evt < evgPtr_size; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];
                for (evp = 0; evp < evtPtr.evtNParams; evp++)
                {
                    evpPtr = evtPtr.evtParams[evp];
                    switch (evpPtr.code)
                    {
                        case 40:	// PARAM_FILENAME
                     /*   case 63:	// PARAM_FILENAME2
                            PARAM_STRING ps = (PARAM_STRING) evpPtr;
                            ps.string = app.relocatePath(ps.string);
                            break;
                        case 33:	// PARAM_PROGRAM
                            PARAM_PROGRAM pp = (PARAM_PROGRAM) evpPtr;
                            pp.filename = app.relocatePath(pp.filename);
                            break; */
                    }
                }
            }
        }
    }

    // PREPARATION DU PROGRAMME POUR LE RUN

    // Inactive tout un groupe et ses sous-groupes
    int inactiveGroup(int evg)
    {
        boolean bQuit;
        CEventGroup evgPtr;
        CEvent evtPtr;
        PARAM_GROUP grpPtr;

        evgPtr = events[evg];
        evgPtr.evgFlags &= this.defaultEvgFlagsMask;
        evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;

        for (evg++  , bQuit=false;;)
        {
            evgPtr = events[evg];
            evgPtr.evgFlags &= this.defaultEvgFlagsMask;
            evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;

            evtPtr = evgPtr.evgEvents[0];
            switch (evtPtr.evtCode)
            {
                case ((-10 << 16) | 65535):		// CNDL_GROUP:
                    grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
                    grpPtr.grpFlags |= PARAM_GROUP.GRPFLAGS_PARENTINACTIVE;
                    evg = inactiveGroup(evg);
                    continue;
                case ((-11 << 16) | 65535):		// CNDL_ENDGROUP:
                    bQuit = true;
                    evg++;
                    break;
            }
            if (bQuit)
            {
                break;
            }
            evg++;
        }
        return evg;
    }

    public void prepareProgram()
    {
        CEventGroup evgPtr;
        CEvent evtPtr;
        PARAM_GROUP grpPtr;
        CParam evpPtr;
        int evg, evt, evp;
        ArrayList<CGroupFind> groups = new ArrayList<CGroupFind>();
        CGroupFind g;
        // Nettoyage des flags groupe
        int events_length = events.length;
        for (evg = 0; evg < events_length;)
        {
            evgPtr = events[evg];
            evgPtr.evgFlags &= this.defaultEvgFlagsMask;

            evtPtr = evgPtr.evgEvents[0];
            if (evtPtr.evtCode == ((-10 << 16) | 65535))	// CNDL_GROUP)
            {
                grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
                g = new CGroupFind();
                g.id = grpPtr.grpId;
                g.evg = evg;
                groups.add(g);
                grpPtr.grpFlags &= ~(PARAM_GROUP.GRPFLAGS_PARENTINACTIVE | PARAM_GROUP.GRPFLAGS_GROUPINACTIVE);

                if ((grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_INACTIVE) != 0)
                {
                    grpPtr.grpFlags |= PARAM_GROUP.GRPFLAGS_GROUPINACTIVE;
                }
            }
            evg++;
        }

        // Activation / desactivation des groupes
        for (evg = 0; evg < events_length;)
        {
            evgPtr = events[evg];
            evgPtr.evgFlags &= this.defaultEvgFlagsMask;

            evtPtr = evgPtr.evgEvents[0];
            if (evtPtr.evtCode == ((-10 << 16) | 65535))	// CNDL_GROUP
            {
                grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
                grpPtr.grpFlags &= ~PARAM_GROUP.GRPFLAGS_PARENTINACTIVE;

                // Groupe entier inactif?
                if ((grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_GROUPINACTIVE) != 0)
                {
                    evg = inactiveGroup(evg);
                    continue;
                }
            }
            evg++;
        }

        // Mise a zero des flags evenements
        for (evg = 0; evg < events_length; evg++)
        {
            evgPtr = events[evg];
            evtPtr = evgPtr.evgEvents[0];
            switch (evtPtr.evtCode)
            {
                case ((-10 << 16) | 65535):	    // CNDL_GROUP
                case ((-11 << 16) | 65535):	    // CNDL_ENDGROUP
                    break;

                default:
                    evgPtr.evgInhibit = 0;
                    evgPtr.evgInhibitCpt = 0;
                    int evgPtr_size = evgPtr.evgNCond + evgPtr.evgNAct;
                    for (evt = 0; evt < evgPtr_size; evt++)
                    {
                        evtPtr = evgPtr.evgEvents[evt];
                        // RAZ des flags conditions / actions
                        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        if (evtPtr.evtCode < 0)
                        {
                            evtPtr.evtFlags &= CEvent.EVFLAGS_DEFAULTMASK;
                        }
                        else
                        {
                            evtPtr.evtFlags &= ~(CAct.ACTFLAGS_REPEAT | CEvent.EVFLAGS_NOTDONEINSTART);
                        }

                        // RAZ des parametres speciaux
                        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        if (evtPtr.evtNParams != 0)
                        {
                            for (evp = 0; evp < evtPtr.evtNParams; evp++)
                            {
                                evpPtr = evtPtr.evtParams[evp];
                                switch (evpPtr.code)
                                {
                                	case 25:
	                                    PARAM_INT pi = (PARAM_INT) evpPtr;
	                                    pi.value2 = 0;
	                                	break;
                                    case 13:	    // PARAM_EVERY
                                        PARAM_EVERY p = (PARAM_EVERY) evpPtr;
                                        p.compteur = p.delay;
                                        break;
                                    case 39:	    // PARAM_GROUPOINTER
                                        PARAM_GROUPOINTER gp = (PARAM_GROUPOINTER) evpPtr;
                                        int n;
                                        int groups_size = groups.size();
                                        for (n = 0; n < groups_size; n++)
                                        {
                                            g = groups.get(n);
                                            if (g.id == gp.id)
                                            {
                                                gp.pointer = (short) g.evg;
                                                break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    // ASSEMBLE LE PROGRAMME : BRANCHE LES POINTEURS DANS CHAQUE OBJET , OPTIMISE ET TOUT
    public void assemblePrograms(CRun run)
    {
        CEventGroup evgPtr;
        CEvent evtPtr;
        CParam evpPtr;

        short o, oo;
        short oi, oi1, oi2;
        short type, type1, type2;
        int nOi, i, n, num;
        short d, evgF, evgM, q, d1, d2;
        int code;
        short fWrap;
        int evtAlways, evtAlwaysPos, evtAlwaysRoot;
        int aTimers, ss;
        boolean bOrBefore;
        int cndOR;
        CObjInfo oilPtr;
        CObject hoPtr;

        rhPtr = run;

        rh2ActionCount = 0;  				// Force le compte des actions a 0

        // Nettoie la curFrame.m_oiList : enleve les blancs, compte les objets
        int oiMax = 0;
        for (nOi = 0  , n=0; n < rhPtr.rhMaxOI; n++)
        {
            if (rhPtr.rhOiList[n].oilOi != -1)
            {
                rhPtr.rhOiList[n].oilActionCount = -1;
                rhPtr.rhOiList[n].oilLimitFlags = 0;
                rhPtr.rhOiList[n].oilLimitList = -1;
                nOi++;
                if (rhPtr.rhOiList[n].oilOi + 1 > oiMax)
                {
                    oiMax = rhPtr.rhOiList[n].oilOi + 1;
                }
            }
        }

        // Fabrique la liste des oi pour chaque qualifier
        qualToOiList = null;
        int oil;
        if (nQualifiers > 0)
        {
            short count[] = new short[nQualifiers];
            for (q = 0; q < nQualifiers; q++)
            {
                oi = (short) ((qualifiers[q].qOi) & 0x7FFF);
                count[q] = 0;
                for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
                {
                    if (rhPtr.rhOiList[oil].oilType == qualifiers[q].qType)
                    {
                        for (n = 0; n < 8 && rhPtr.rhOiList[oil].oilQualifiers[n] != -1; n++)      // MAX_QUALIFIERS
                        {
                            if (oi == rhPtr.rhOiList[oil].oilQualifiers[n])
                            {
                                count[q]++;
                            }
                        }
                    }
                }
            }

            qualToOiList = new CQualToOiList[nQualifiers];
            for (q = 0; q < nQualifiers; q++)
            {
                qualToOiList[q] = new CQualToOiList();

                if (count[q] != 0)
                {
                    qualToOiList[q].qoiList = new short[count[q] * 2];
                }

                i = 0;
                oi = (short) ((qualifiers[q].qOi) & 0x7FFF);
                for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
                {
                    if (rhPtr.rhOiList[oil].oilType == qualifiers[q].qType)
                    {
                        for (n = 0; n < 8 && rhPtr.rhOiList[oil].oilQualifiers[n] != -1; n++)
                        {
                            if (oi == rhPtr.rhOiList[oil].oilQualifiers[n])
                            {
                                qualToOiList[q].qoiList[i * 2] = rhPtr.rhOiList[oil].oilOi;
                                qualToOiList[q].qoiList[i * 2 + 1] = (short) oil;
                                i++;
                            }
                        }
                    }
                }
                qualToOiList[q].qoiActionCount = -1;
            }
        }

        // Poke les offsets des oi dans le programme, prepare les parametres / cree les tables de limitations
        // Marque les evenements a traiter dans la boucle
        // --------------------------------------------------------------------------------------------------

        // 100 actions STOP par objet... 	// YVES: nOi -> nOi+1 pour eviter erreurs si pas d'objet actif
        colBuffer = new short[oiMax * 100 * 2 + 1];
        int colList = 0;
        ArrayList<CPosStartLoop> posStartLoop = new ArrayList<CPosStartLoop>();

        // Boucle d'exploration du programme
        int evg, evt, evp;
        //int events_length = events.length;
        for (evg = 0; evg < events.length; evg++)
        {
            evgPtr = events[evg];
            if(evgPtr == null)
            	continue;

            // Initialisation des parametres / pointeurs sur oilists/qoioilist
            // -------------------------------------------------------------
            int evgPtr_size = evgPtr.evgNAct + evgPtr.evgNCond;
            for (evt = 0; evt < evgPtr_size; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];

                if(evtPtr == null)
                	continue;
                
                // Pas de flag BAD
                evtPtr.evtFlags &= ~CEvent.EVFLAGS_BADOBJECT;

                // Si evenement pour un objet reel, met l'adresse de l'curFrame.m_oiList
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (EVTTYPE(evtPtr.evtCode) >= 0)
                {
                    evtPtr.evtOiList = get_OiListOffset(evtPtr.evtOi, EVTTYPE(evtPtr.evtCode));
                }

                if ( evtPtr.evtCode == ((14 << 16) | 0xFFFF) )      // CAct.ACT_STARTLOOP
                {
                    CParamExpression pExp = (CParamExpression)evtPtr.evtParams[0];
                    pExp.comparaison = 0;
                    if ( pExp.tokens[0].code == ((3 << 16) | 0xFFFF) && pExp.tokens[1].code == 0 )
                    {
                        posStartLoop.add(new CPosStartLoop(((EXP_STRING)pExp.tokens[0]).string, pExp));
                    }
                }

                // Exploration des parametres
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (evtPtr.evtNParams > 0)
                {
                    for (evp = 0; evp < evtPtr.evtNParams; evp++)
                    {
                        evpPtr = evtPtr.evtParams[evp];
                        switch (evpPtr.code)
                        {
                            // Met un parametre buffer 4 a zero
                            case 25:        // PARAM_BUFFER4:
                                ((PARAM_INT) evpPtr).value = 0;
                                break;

                            // Trouve le levobj de creation
                            case 21:        // PARAM_SYSCREATE:
                                if ((evtPtr.evtOi & COI.OIFLAG_QUALIFIER) == 0)
                                {
                                    CLO loPtr;
                                    for (loPtr = rhPtr.rhFrame.LOList.first_LevObj(); loPtr != null; loPtr = rhPtr.rhFrame.LOList.next_LevObj())
                                    {
                                        if (evtPtr.evtOi == loPtr.loOiHandle)
                                        {
                                            ((CCreate) evpPtr).cdpHFII = loPtr.loHandle;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ((CCreate) evpPtr).cdpHFII = -1;
                                }
                            // Met l'adresse du levObj pour create object
                            case 9:         // PARAM_CREATE:
                            case 18:        // PARAM_SHOOT:
                            case 16:        // PARAM_POSITION:
                                oi = ((CPosition) evpPtr).posOINUMParent;
                                if (oi != -1)
                                {
                                    ((CPosition) evpPtr).posOiList = get_OiListOffset(oi, ((CPosition) evpPtr).posTypeParent);
                                }
                                break;

                            // Poke l'adresse de l'objet dans l'curFrame.m_oiList
                            case 1:         // PARAM_OBJECT:
                                ((PARAM_OBJECT) evpPtr).oiList = get_OiListOffset(((PARAM_OBJECT) evpPtr).oi, ((PARAM_OBJECT) evpPtr).type);
                                break;

							case 69:         // PARAM_CHILDEVENT:
								for (int c = 0; c < ((PARAM_CHILDEVENT)evpPtr).ois.length; c += 2)
								{
									((PARAM_CHILDEVENT)evpPtr).ois[c + 1] = get_OiListOffset(((PARAM_CHILDEVENT)evpPtr).ois[c], (short)0);
								}
								break;

                            // Expression : poke l'adresse de l'curFrame.m_oiList dans les parametres objets
                            case 15:        // PARAM_SPEED:
                            case 27:        // PARAM_SAMLOOP:
                            case 28:        // PARAM_MUSLOOP:
                            case 45:        // PARAM_EXPSTRING:
                            case 46:        // PARAM_CMPSTRING:
                            case 22:        // PARAM_EXPRESSION:
                            case 23:        // PARAM_COMPARAISON:
                            case 52:        // PARAM_VARGLOBAL_EXP:
                            case 59:        // PARAM_STRINGGLOBAL_EXP:
                            case 53:        // PARAM_ALTVALUE_EXP:
                            case 62:        // PARAM_ALTSTRING_EXP:
                            case 54:        // PARAM_FLAG_EXP:
                                CParamExpression expPtr = (CParamExpression) evpPtr;
                                for (n = 0; n < expPtr.tokens.length; n++)
                                {
                                    // Un objet avec OI?
                                    if (EVTTYPE(expPtr.tokens[n].code) > 0)
                                    {
                                        CExpOi expOi = (CExpOi) expPtr.tokens[n];
                                        expOi.oiList = get_OiListOffset(expOi.oi, EVTTYPE(expOi.code));
                                    }

									// Loop index optimization
									if ( expPtr.tokens[n].code == ((46 << 16) | 0xFFFF) )		// EXPL_LOOPINDEX
									{
										((EXP_LOOPINDEXOPT)expPtr.tokens[n]).pLoop = null;
										if ( n+2 < expPtr.tokens.length && (expPtr.tokens[n+2].code<=0 || expPtr.tokens[n+2].code>=0x00140000) && expPtr.tokens[n+1].code == ((3 << 16) | 65535) ) // EXP_STRING
										{
											String loopName = ((EXP_STRING)expPtr.tokens[n+1]).getLowercase();
											((EXP_LOOPINDEXOPT)expPtr.tokens[n]).pLoop = rhPtr.addFastLoop(loopName);
										}
									}
									//
                                }
                                ;
                                break;
                        }
                    }
                }
            }

            // Flags par defaut / Listes de limitation
            // ---------------------------------------
            evgF = 0;
            evgM = CEventGroup.EVGFLAGS_ONCE | CEventGroup.EVGFLAGS_LIMITED | CEventGroup.EVGFLAGS_STOPINGROUP;
            for (evt = 0; evt < evgPtr.evgNCond + evgPtr.evgNAct; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];

                if(evtPtr == null)
                	continue;
                
                type = EVTTYPE(evtPtr.evtCode);
                code = evtPtr.evtCode;
                n = 0;
                d1 = 0;
                d2 = 0;
                evpPtr = null;
                if (type >= COI.OBJ_SPR)
                {
                    switch (getEventCode(code))
                    {
                        case (4 << 16):      // ACTL_EXTSTOP:
                        case (9 << 16):      // ACTL_EXTBOUNCE:

                            evgF |= CEventGroup.EVGFLAGS_STOPINGROUP;

                            // Recherche dans le groupe, la cause du STOP-> limitList
                            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                            oi = evtPtr.evtOi;
                            if ((oi & COI.OIFLAG_QUALIFIER) != 0)
                            {
                                for (o = qual_GetFirstOiList2(evtPtr.evtOiList); o != -1; o = qual_GetNextOiList2())
                                {
                                    colList = make_ColList1(evgPtr, colList, rhPtr.rhOiList[o].oilOi);
                                }
                            }
                            else
                            {
                                colList = make_ColList1(evgPtr, colList, oi);
                            }
                            break;
                        case (25 << 16):      // ACTL_EXTSHUFFLE:
                            evgF |= CEventGroup.EVGFLAGS_SHUFFLE;
                            break;
                        case (-14 << 16):     // CNDL_EXTCOLLISION:
							// Build 284.1: separate CNDL_EXTCOLLISION and CNDL_EXTISCOLLIDING
                            evpPtr = evtPtr.evtParams[0];
							AddColList(evtPtr.evtOiList, evtPtr.evtOi, ((PARAM_OBJECT)evpPtr).oiList, ((PARAM_OBJECT)evpPtr).oi);
							AddColList(((PARAM_OBJECT)evpPtr).oiList, ((PARAM_OBJECT)evpPtr).oi, evtPtr.evtOiList, evtPtr.evtOi);

                            // L'objet 1 est-il un sprite?
                            type1 = EVTTYPE(evtPtr.evtCode);
                            if (isTypeRealSprite(type1))
                            {
                                d2 = CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_ONCOLLIDE;
                            }
                            else
                            {
                                d2 = CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_QUICKEXT | CObjInfo.OILIMITFLAGS_ONCOLLIDE;
                            }

                            // L'objet 2 est-il un sprite?
                            type2 = ((PARAM_OBJECT)evpPtr).type;
                            if (isTypeRealSprite(type2))
                            {
                                d1 = CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_ONCOLLIDE;
                            }
                            else
                            {
                                d1 = CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_QUICKEXT | CObjInfo.OILIMITFLAGS_ONCOLLIDE;
                            }
                            n = 3;
                            break;
                        case (-4 << 16):     // CNDL_EXTISCOLLIDING:
                            // L'objet 1 est-il un sprite?
                            type1 = EVTTYPE(evtPtr.evtCode);
                            if (isTypeRealSprite(type1))
                            {
                                d2 = CObjInfo.OILIMITFLAGS_QUICKCOL;
                            }
                            else
                            {
                                d2 = CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_QUICKEXT;
                            }

                            // L'objet 2 est-il un sprite?
                            evpPtr = evtPtr.evtParams[0];
                            type2 = ((PARAM_OBJECT) evtPtr.evtParams[0]).type;
                            if (isTypeRealSprite(type2))
                            {
                                d1 = CObjInfo.OILIMITFLAGS_QUICKCOL;
                            }
                            else
                            {
                                d1 = CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_QUICKEXT;
                            }
                            n = 3;
                            break;
                        case (-11 << 16):     // CNDL_EXTINPLAYFIELD:
                        case (-12 << 16):     // CNDL_EXTOUTPLAYFIELD:
                            d1 = CObjInfo.OILIMITFLAGS_QUICKBORDER;
                            n = 1;
                            break;
                        case (-13 << 16):     // CNDL_EXTCOLBACK:
                            d1 = CObjInfo.OILIMITFLAGS_QUICKBACK;
                            n = 1;
                            break;
                    }
                }
                else
                {
                    switch (code)
                    {
                        case ((-6 << 16) | 65535):      // CNDL_ONCE
                            evgM &= ~CEventGroup.EVGFLAGS_ONCE;
                            break;
                        case ((-7 << 16) | 65535):      // CNDL_NOTALWAYS:
                            evgM |= CEventGroup.EVGFLAGS_NOMORE;
                            break;
                        case ((-5 << 16) | 65535):      // CNDL_REPEAT:
                            evgM |= CEventGroup.EVGFLAGS_NOMORE;
                            break;
                        case ((-4 << 16) | 65535):      // CNDL_NOMORE:
                            evgM |= CEventGroup.EVGFLAGS_NOTALWAYS + CEventGroup.EVGFLAGS_REPEAT;
                            break;
                        case ((-4 << 16) | 0xFFFA):     // CNDL_MONOBJECT:
                            d2 = CObjInfo.OILIMITFLAGS_QUICKCOL;
                            evpPtr = evtPtr.evtParams[0];
                            n = 2;
                            break;
                        case ((-7 << 16) | 0xFFFA):     // CNDL_MCLICKONOBJECT:
                            d2 = CObjInfo.OILIMITFLAGS_QUICKCOL;
                            evpPtr = evtPtr.evtParams[1];
                            n = 2;
                            break;
                    }
                }
                // Poke les flags collision
                if ((n & 1) != 0)
                {
                    for (o = qual_GetFirstOiList(evtPtr.evtOiList); o != -1; o = qual_GetNextOiList())
                    {
                        rhPtr.rhOiList[o].oilLimitFlags |= d1;
                    }
                }
                if ((n & 2) != 0)
                {
                    for (o = qual_GetFirstOiList(((PARAM_OBJECT) evpPtr).oiList); o != -1; o = qual_GetNextOiList())
                    {
                        rhPtr.rhOiList[o].oilLimitFlags |= d2;
                    }
                }
            }
            // Inhibe les anciens flags
            evgPtr.evgFlags &= ~evgM;
            evgPtr.evgFlags |= evgF;
        }
        colBuffer[colList] = -1;

        // Reserve le buffer des pointeurs sur listes d'events
        // ---------------------------------------------------
        int aListPointers[] = new int[COI.NUMBEROF_SYSTEMTYPES + oiMax + 1];

        // Rempli cette table avec les offsets en fonction des types
        ss = 0;
        int alp;
        for (alp = 0  , type = -COI.NUMBEROF_SYSTEMTYPES; type<0; type++, alp++)
        {
            aListPointers[alp] = ss;
            ss += nConditions[COI.NUMBEROF_SYSTEMTYPES + type];
        }
        // Continue avec les OI, la taille juste pour le type de l'oi
        for (oil = 0; oil < rhPtr.rhMaxOI; oil++, alp++)
        {
            aListPointers[alp] = ss;
            if (rhPtr.rhOiList[oil].oilType < COI.KPX_BASE)
            {
                ss += nConditions[COI.NUMBEROF_SYSTEMTYPES + rhPtr.rhOiList[oil].oilType] + EVENTS_EXTBASE + 1;
            }
            else
            {
                ss += rhPtr.rhApp.extLoader.getNumberOfConditions(rhPtr.rhOiList[oil].oilType) + EVENTS_EXTBASE + 1;
            }
        }

        // Reserve le buffer des pointeurs
        int sListPointers = ss;
        listPointers = new int[sListPointers];
        for (n = 0; n < sListPointers; n++)
        {
            listPointers[n] = 0;
        }
        evtAlways = 0;
		evtAlwaysRoot = 0;

        // Explore le programme et repere les evenements
        short wBufNear[] = new short[rhPtr.rhFrame.maxObjects];
        int wPtrNear;
        for (evg = 0; evg < nEvents; evg++)
        {
            evgPtr = events[evg];
            
            if(evgPtr == null)
            	continue;
            
            evgPtr.evgFlags &= ~CEventGroup.EVGFLAGS_ORINGROUP;
            bOrBefore = true;
            cndOR = 0;
            for (evt = 0; evt < evgPtr.evgNCond; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];
                
                if(evtPtr == null)
                	continue;
                type = EVTTYPE(evtPtr.evtCode);
                code = evtPtr.evtCode;
                num = -EVTNUM(code);

                if (bOrBefore && evtPtr.evtCode != ((-42 << 16) | 65535))  // CNDL_ENDCHILDEVENT
                {
                    // Dans la liste des evenements ALWAYS
                    if ((evtPtr.evtFlags & CEvent.EVFLAGS_ALWAYS) != 0)
                    {
                        evtAlways++;
						if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_HASPARENT) == 0)
							evtAlwaysRoot++;
                    }

                    // Dans la liste des evenements generaux si objet systeme
                    if (type < 0)
                    {
                        listPointers[aListPointers[7 + type] + num]++;
                    }
                    else
                    // Un objet normal / qualifier : relie aux objets
                    {
                        wPtrNear = 0;
                        for (o = qual_GetFirstOiList(evtPtr.evtOiList); o != -1; o = qual_GetNextOiList())
                        {
                            listPointers[aListPointers[COI.NUMBEROF_SYSTEMTYPES + o] + num]++;
                            wBufNear[wPtrNear++] = o;
                        }
                        wBufNear[wPtrNear] = -1;
                        // Cas special pour les collisions de sprites : branche aux deux sprites (sauf si meme!)
                        if (getEventCode(code) == (-14 << 16))      // CNDL_EXTCOLLISION
                        {
                            evpPtr = evtPtr.evtParams[0];
                            for (oo = qual_GetFirstOiList(((PARAM_OBJECT) evpPtr).oiList); oo != -1; oo = qual_GetNextOiList())
                            {
                                for (wPtrNear = 0; wBufNear[wPtrNear] != oo && wBufNear[wPtrNear] != -1; wPtrNear++){};
                                if (wBufNear[wPtrNear] == -1)
                                {
                                    listPointers[aListPointers[COI.NUMBEROF_SYSTEMTYPES + oo] + num]++;
                                }
                            }
                        }
                    }
                }
                bOrBefore = false;
                if (evtPtr.evtCode == ((-24 << 16) | 65535) || evtPtr.evtCode == ((-25 << 16) | 65535))     // CNDL_OR - CNDL_ORLOGICAL
                {
                    bOrBefore = true;
                    evgPtr.evgFlags |= CEventGroup.EVGFLAGS_ORINGROUP;
                    // Un seul type de OR dans un groupe
                    if (cndOR == 0)
                    {
                        cndOR = evtPtr.evtCode;
                    }
                    else
                    {
                        evtPtr.evtCode = cndOR;
                    }
                    // Marque les OR Logical
                    if (cndOR == ((-25 << 16) | 65535))       // CNDL_ORLOGICAL)
                    {
                        evgPtr.evgFlags |= CEventGroup.EVGFLAGS_ORLOGICAL;
                    }
                }
				if (evtPtr.evtCode == ((-42 << 16) | 65535))  // CNDL_ENDCHILDEVENT
				{
				    evtAlways++;				// for ending evgOffset (-1)
				    evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;		// set line as inactive
				}
            }
        }

        // Calcule les tailles necessaires, poke les pointeurs dans les listes
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        int sEventPointers = evtAlways + 1;
        int uil;
        for (uil = 0; uil < sListPointers; uil++)
        {
            if (listPointers[uil] != 0)
            {
                ss = listPointers[uil];
                listPointers[uil] = sEventPointers;
                sEventPointers += ss + 1;
            }
        }
        eventPointersGroup = new CEventGroup[sEventPointers];
        eventPointersCnd = new byte[sEventPointers];
        for (n = 0; n < sEventPointers; n++)
        {
            eventPointersGroup[n] = null;
            eventPointersCnd[n] = 0;
        }

        int lposBuffer[] = new int[sListPointers];
        for (n = 0; n < sListPointers; n++)
        {
            lposBuffer[n] = listPointers[n];
        }

        evtAlwaysPos = 0;
        int lposPtr;
		ArrayList<ArrayList<CEventGroup>> nestedEvgOffsetStack = new ArrayList<ArrayList<CEventGroup>>();
		ArrayList<ArrayList<Integer>> nestedEvtOffsetStack = new ArrayList<ArrayList<Integer>>();
		ArrayList<PARAM_CHILDEVENT> nestedEventActionParamStack = new ArrayList<PARAM_CHILDEVENT>();
		int evtAlwaysChildrenPos = (evtAlwaysRoot + 1);
        this.complexOnLoop = false;
        for (evg = 0; evg < nEvents; evg++)
        {
            evgPtr = events[evg];
            
            if(evgPtr == null)
            	continue;
            
            bOrBefore = true;
            for (evt = 0; evt < evgPtr.evgNCond; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];
                
                if(evtPtr == null)
                	continue;

                type = EVTTYPE(evtPtr.evtCode);
                code = evtPtr.evtCode;
                num = -EVTNUM(code);

                if (bOrBefore && evtPtr.evtCode != ((-42 << 16) | 0xFFFF))
                {
                    // Dans la liste des evenements ALWAYS
                    if ((evtPtr.evtFlags & CEvent.EVFLAGS_ALWAYS) != 0)
                    {
						if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_HASPARENT) != 0)
						{
							if (nestedEvgOffsetStack.size() > 0)
							{
								ArrayList<CEventGroup> curEvgList = nestedEvgOffsetStack.get(nestedEvgOffsetStack.size() - 1);
								curEvgList.add(evgPtr);
								ArrayList<Integer> curEvtList = nestedEvtOffsetStack.get(nestedEvtOffsetStack.size() - 1);
								curEvtList.add(evt);
							}
						}
						else
						{
							eventPointersGroup[evtAlwaysPos] = evgPtr;
							eventPointersCnd[evtAlwaysPos] = (byte) evt;
							evtAlwaysPos++;
						}
                    }

                    // Dans la liste des evenements generaux si objet systeme
                    if (type < 0)
                    {
                        lposPtr = aListPointers[COI.NUMBEROF_SYSTEMTYPES + type] + num;
                        eventPointersGroup[lposBuffer[lposPtr]] = evgPtr;
                        eventPointersCnd[lposBuffer[lposPtr]] = (byte) evt;
                        lposBuffer[lposPtr]++;

                        if (evtPtr.evtCode == ((-16 << 16) | 0xFFFF)) // CCnd.CND_ONLOOP
                        {
                            CEvent evtNext;
                            boolean bOR = false;
                            for (n = 0, evtNext = evgPtr.evgEvents[n]; n < evgPtr.evgNCond; n++, evtNext = evgPtr.evgEvents[n])
                            {
                                if (evtNext.evtCode == ((-24 << 16) | 0xFFFF)/*CCnd.CND_OR*/ || evtNext.evtCode == ((-25 << 16) | 0xFFFF)/*CCnd.CND_ORLOGICAL*/)
                                    break;
                            }
                            if (n < evgPtr.evgNCond)
                                bOR = true;

                            CParamExpression pExp = (CParamExpression)evtPtr.evtParams[0];
                            if ( pExp.tokens[0].code == ((3 << 16) | 0xFFFF) && pExp.tokens[1].code == 0 )
                            {
                                String pName = ((EXP_STRING)pExp.tokens[0]).string.toLowerCase();
                                int posStartLoop_size = posStartLoop.size();
                                for (n = 0; n < posStartLoop_size; n++)
                                {
                                    CPosStartLoop pPos = posStartLoop.get(n);
                                    if (pPos.m_name.equalsIgnoreCase(pName))
                                    {
                                        ArrayList<CPosOnLoop> rh4PosOnLoop = rhPtr.rh4PosOnLoop;
                                        if ( rh4PosOnLoop == null )
                                            rhPtr.rh4PosOnLoop = rh4PosOnLoop = new ArrayList<CPosOnLoop>();

                                        CPosOnLoop posOnLoop = null;
                                        int rh4PosOnLoop_size = rh4PosOnLoop.size();
                                        for (n = 0; n < rh4PosOnLoop_size; n++)
                                        {
                                            posOnLoop = rh4PosOnLoop.get(n);
                                            if (pName.equalsIgnoreCase(posOnLoop.m_name))
                                                break;
                                        }
                                        if (n == rh4PosOnLoop.size())
                                        {
                                            posOnLoop = new CPosOnLoop(pName);
                                            rh4PosOnLoop.add(posOnLoop);
                                        }
                                        posOnLoop.addOnLoop(evgPtr);
                                        posOnLoop.m_bOR |= bOR;
                                        pPos.m_expPtr.comparaison = (short)(n + 1);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                this.complexOnLoop = true;
                            }
                        }
                    }
                    else
                    // Un objet normal : relie a l'objet
                    {
                        wPtrNear = 0;
                        for (o = qual_GetFirstOiList(evtPtr.evtOiList); o != -1; o = qual_GetNextOiList())
                        {
                            lposPtr = aListPointers[COI.NUMBEROF_SYSTEMTYPES + o] + num;
                            eventPointersGroup[lposBuffer[lposPtr]] = evgPtr;
                            eventPointersCnd[lposBuffer[lposPtr]] = (byte) evt;
                            lposBuffer[lposPtr]++;
                            wBufNear[wPtrNear++] = o;
                        }
                        wBufNear[wPtrNear] = -1;
                        // Cas special pour les collisions de sprites : branche aux deux sprites (sauf si meme!)
                        if (getEventCode(code) == (-14 << 16))      // CNDL_EXTCOLLISION
                        {
                            evpPtr = evtPtr.evtParams[0];
                            for (oo = qual_GetFirstOiList(((PARAM_OBJECT) evpPtr).oiList); oo != -1; oo = qual_GetNextOiList())
                            {
                                for (wPtrNear = 0; wBufNear[wPtrNear] != oo && wBufNear[wPtrNear] != -1; wPtrNear++){};
                                if (wBufNear[wPtrNear] == -1)
                                {
                                    lposPtr = aListPointers[COI.NUMBEROF_SYSTEMTYPES + oo] + num;
                                    eventPointersGroup[lposBuffer[lposPtr]] = evgPtr;
                                    eventPointersCnd[lposBuffer[lposPtr]] = (byte) evt;
                                    lposBuffer[lposPtr]++;
                                }
                            }
                        }
                    }
                }
                bOrBefore = false;
                if (evtPtr.evtCode == ((-24 << 16) | 65535) || evtPtr.evtCode == ((-25 << 16) | 65535))     // CNDL_OR - CNDL_ORLOGICAL
                {
                    bOrBefore = true;
                }

				if (evtPtr.evtCode == ((-42 << 16) | 0xFFFF))  // CNDL_ENDCHILDEVENT
				{
				    // Copy events
				    if (nestedEvgOffsetStack.size() > 0)
					{
						int idx = nestedEventActionParamStack.size()-1;
				        PARAM_CHILDEVENT param = nestedEventActionParamStack.get(idx);
						nestedEventActionParamStack.remove(idx);
				        param.evgOffsetList = evtAlwaysChildrenPos;

						idx = nestedEvgOffsetStack.size()-1;
						ArrayList<CEventGroup> curEvgList = nestedEvgOffsetStack.get(idx);
						ArrayList<Integer> curEvtList = nestedEvtOffsetStack.get(idx);
						nestedEvgOffsetStack.remove(idx);
						nestedEvtOffsetStack.remove(idx);

				        for (int v = 0; v < curEvgList.size(); v++) {
				            eventPointersGroup[evtAlwaysChildrenPos] = curEvgList.get(v);
				            eventPointersCnd[evtAlwaysChildrenPos] = (byte)curEvtList.get(v).intValue();
				            evtAlwaysChildrenPos++;
				        }
				        eventPointersGroup[evtAlwaysChildrenPos] = null;
				        eventPointersCnd[evtAlwaysChildrenPos] = 0;
				        evtAlwaysChildrenPos++;
				    }
				}
            }

			if ((evgPtr.evgFlags & CEventGroup.EVGFLAGS_HASCHILDREN) != 0)
			{
			    // Add new eventGroup offset list
				ArrayList<CEventGroup> curEvgList = new ArrayList<CEventGroup>();
				ArrayList<Integer> curEvtList = new ArrayList<Integer>();
			    nestedEvgOffsetStack.add(curEvgList);
			    nestedEvtOffsetStack.add(curEvtList);

			    // Look for action "Start child events"
			    for (int c = 0; c < evgPtr.evgNAct; c++)
				{
			        CEvent evtPtr1 = evgPtr.evgEvents[evgPtr.evgNCond + c];
			        if (evtPtr1.evtCode == ((43 << 16) | 0xFFFF))    // ACTL_EXECUTECHILDEVENTS )
			        {
			            if (evtPtr1.evtNParams > 0)
						{
			                PARAM_CHILDEVENT evpPtr1 = (PARAM_CHILDEVENT)evtPtr1.evtParams[0];
			                nestedEventActionParamStack.add(evpPtr1);
			            }
			            break;
			        }
			    }
			}
        }

        // Adresse des conditions timer
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        uil = aListPointers[COI.NUMBEROF_SYSTEMTYPES + COI.OBJ_TIMER];
        aTimers = listPointers[uil - EVTNUM(((-3 << 16) | 0xFFFC))];     // CNDL_TIMER

        // Poke les adresses et les autres flags des pointeurs dans tous OI
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        limitBuffer = new short[oiMax + 1 + colList / 2];
        int limitListStart = 0;
        int limitPos, limitCur;
        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            oilPtr = rhPtr.rhOiList[oil];

            // Poke l'offset dans les events
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            uil = aListPointers[COI.NUMBEROF_SYSTEMTYPES + oil];
            oilPtr.oilEvents = uil;

            // Traitement des flags particuliers
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            int act;
            if ((oilPtr.oilOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0)
            {
                // Recherche les flags WRAP dans les messages OUT OF PLAYFIELD
                fWrap = 0;
                ss = listPointers[uil - EVTNUM(-12 << 16)];     // CNDL_EXTOUTPLAYFIELD
                if (ss != 0)
                {
                    while (eventPointersGroup[ss] != null)
                    {
                        evgPtr = eventPointersGroup[ss];
                        evtPtr = evgPtr.evgEvents[eventPointersCnd[ss]];
                        d = ((PARAM_SHORT) evtPtr.evtParams[0]).value;	// Prend la direction
                        for (act = evg_FindAction(evgPtr, 0)     , n=evgPtr.evgNAct; n>0; n--, act++)
                        {
                            evtPtr = evgPtr.evgEvents[act];
                            if (evtPtr.evtCode == ((8 << 16) | ((oilPtr.oilType) & 0xFFFF))) // ACT_EXTWRAP
                            {
                                fWrap |= d;
                            }
                        }
                        ss++;
                    }
                }
                oilPtr.oilWrap = (byte) fWrap;

                // Fabrique la table de limitations des mouvements
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                oi1 = oilPtr.oilOi;
                for (colList = 0  , limitPos=0; colBuffer[colList] != -1; colList += 2)
                {
                    if (colBuffer[colList] == oi1)
                    {
                        oi2 = colBuffer[colList + 1];
                        if ((oi2 & 0x8000) != 0)
                        {
                            oilPtr.oilLimitFlags |= oi2;
                            continue;
                        }
                        for (limitCur = 0; limitCur < limitPos && limitBuffer[limitListStart + limitCur] != oi2; limitCur++){};
                        if (limitCur == limitPos)
                        {
                            limitBuffer[limitListStart + limitPos++] = oi2;
                        }
                    }
                }
                // Marque la fin...
                if (limitPos > 0)
                {
                    oilPtr.oilLimitList = limitListStart;
                    limitBuffer[limitListStart + limitPos++] = -1;
                    limitListStart += limitPos;
                }
            }
        }

        // Met les adresses des tables de pointeur systeme
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        rhEvents[0] = 0;
        for (n = 1; n <= COI.NUMBEROF_SYSTEMTYPES; n++)
        {
            rhEvents[n] = aListPointers[COI.NUMBEROF_SYSTEMTYPES - n];
        }

        // Poke les adresses et les autres flags des pointeurs dans tous les objets definis
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            oilPtr = rhPtr.rhOiList[oil];

            // Explore tous les objets de meme OI dans le programme
            o = oilPtr.oilObject;
            if ((o & 0x8000) == 0)
            {
                do
                {
                    // Met les oi dans les ro
                    hoPtr = rhPtr.rhObjectList[o];
                    hoPtr.hoEvents = oilPtr.oilEvents;
                    hoPtr.hoOiList = oilPtr;
                    hoPtr.hoLimitFlags = oilPtr.oilLimitFlags;
                    // Flags Wrap pour les objets avec movement
                    if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MOVEMENTS) != 0)
                    {
                        hoPtr.rom.rmWrapping = oilPtr.oilWrap;
                    }
                    // Si le sprite n'est pas implique dans les collisions -> le passe en neutre
                    if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0 && (hoPtr.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKCOL) == 0)
                    {
                        if (hoPtr.roc.rcSprite != null)
                        {
                            hoPtr.roc.rcSprite.setSpriteColFlag(0);
                        }
                    }
                    // Sprite en mode inbitate?
                    if ((hoPtr.hoOEFlags & CObjectCommon.OEFLAG_MANUALSLEEP) == 0)
                    {
                        // On detruit... sauf si...
                        hoPtr.hoOEFlags &= ~CObjectCommon.OEFLAG_NEVERSLEEP;

                        // On teste des collisions avec le decor?
                        if ((hoPtr.hoLimitFlags & CObjInfo.OILIMITFLAGS_QUICKBACK) != 0)
                        {
                            // Si masque des collisions general
                            if ((rhPtr.rhFrame.leFlags & CRunFrame.LEF_TOTALCOLMASK) != 0)
                            {
                                hoPtr.hoOEFlags |= CObjectCommon.OEFLAG_NEVERSLEEP;
                            }
                        }
                        // Ou test des collisions normal
                        if ((hoPtr.hoLimitFlags & (CObjInfo.OILIMITFLAGS_QUICKCOL | CObjInfo.OILIMITFLAGS_QUICKBORDER)) != 0)
                        {
                            hoPtr.hoOEFlags |= CObjectCommon.OEFLAG_NEVERSLEEP;
                        }
                    }
                    o = hoPtr.hoNumNext;
                } while ((o & 0x8000) == 0);
            }
        }
        // Les messages speciaux
        // ~~~~~~~~~~~~~~~~~~~~~
        if (evtAlwaysRoot != 0)
        {
            rhEventAlways = true;
        }
        else
        {
            rhEventAlways = false;
        }
        // Messages Timer (a bulle!)
        if (aTimers != 0)
        {
            rh4TimerEventsBase = aTimers;
        }
        else
        {
            rh4TimerEventsBase = 0;
        }

        // Liberation
        colBuffer = null;
        bReady = true;
    }

	public void AddColList(short oiList, short oiNum, short oiList2, short oiNum2)
	{
		// Must not do it in fade in mode
		if ( (rhPtr.rhGameFlags & CRun.GAMEFLAGS_FIRSTLOOPFADEIN) != 0 )
			return;

		// First object = qualifier?
		if ( oiNum < 0 )
		{
			if ( qualToOiList != null )
			{
				CQualToOiList qoil = qualToOiList[oiList&0x7FFF];
				int index;
				for (index=0; index<qoil.qoiList.length; index+=2)
					AddColList(qoil.qoiList[index+1], qoil.qoiList[index], oiList2, oiNum2);
			}
			return;
		}

		// Second object = qualifier?
		if ( oiNum2 < 0 )
		{
			if ( qualToOiList != null )
			{
				CQualToOiList qoil = qualToOiList[oiList2&0x7FFF];
				int index;
				for (index=0; index<qoil.qoiList.length; index+=2)
					AddColList(oiList, oiNum, qoil.qoiList[index+1], qoil.qoiList[index]);
			}
			return;
		}

		// Normal objects
        CObjInfo oilPtr = rhPtr.rhOiList[oiList];
		int idx = 0;
		if (oilPtr.oilColList == null)
		{
			oilPtr.oilColList = new short[2];
		}
		else
		{
			// Exit if object already in list
			int n;
			idx = oilPtr.oilColList.length;
			for (n=0; n<idx; n+=2)
			{
				if ( oiNum2 == oilPtr.oilColList[n] )
				{
					return;
				}
			}

			short[] newList = Arrays.copyOfRange(oilPtr.oilColList, 0, idx+2);
			oilPtr.oilColList = newList;
		}

		oilPtr.oilColList[idx] = oiNum2;
		oilPtr.oilColList[idx+1] = oiList2;
	}

    public void unBranchPrograms()
    {
        bReady = false;
        qualToOiList = null;
        limitBuffer = null;
        listPointers = null;
        eventPointersGroup = null;
        eventPointersCnd = null;
    }

    // Poke l'adresse de l'curFrame.m_oiList / qualOiList en fonction d'un oi/qual
    short get_OiListOffset(short oi, short type)
    {

        // Un qualifier
        if ((oi & COI.OIFLAG_QUALIFIER) != 0)
        {
            int q;
            for (q = 0; oi != qualifiers[q].qOi || type != qualifiers[q].qType; q++){};
            return (short) (q | 0x8000);
        }
        // Un objet normal
        else
        {
            int n;
            for (n = 0; n < rhPtr.rhMaxOI && rhPtr.rhOiList[n].oilOi != oi; n++){};
            return (short) n;
        }
    }
    // Un type d'objet est-il un vrai sprite?

    boolean isTypeRealSprite(short type)
    {
        int oil;
        for (oil = 0; oil < rhPtr.rhMaxOI; oil++)
        {
            if (rhPtr.rhOiList[oil].oilOi != -1)
            {
                if (rhPtr.rhOiList[oil].oilType == type)
                {
                    if ((rhPtr.rhOiList[oil].oilOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0 && (rhPtr.rhOiList[oil].oilOEFlags & CObjectCommon.OEFLAG_QUICKDISPLAY) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    // Retourne les index curFrame.m_oiList des objets/qualifiers
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    short qual_GetFirstOiList(short o)
    {
        if ((o & 0x8000) == 0)
        {
            qualOilPtr = -1;
            return (o);
        }
        if (o == -1)
        {
            return -1;
        }

        o &= 0x7FFF;
        qualOilPtr = o;
        qualOilPos = 0;
        return qual_GetNextOiList();
    }

    short qual_GetNextOiList()
    {
        short o;
        /*
        if (qualOilPtr == -1)
        {
            return -1;
        }
        if (qualOilPos >= qualToOiList[qualOilPtr].qoiList.length)
        {
            return -1;
        }
        */
        if (qualOilPtr == -1 || qualOilPos >= qualToOiList[qualOilPtr].qoiList.length)
        {
            return -1;
        }        
        o = qualToOiList[qualOilPtr].qoiList[qualOilPos + 1];
        qualOilPos += 2;
        return (o);
    }

    short qual_GetFirstOiList2(short o)
    {
        if ((o & 0x8000) == 0)
        {
            qualOilPtr2 = -1;
            return (o);
        }
        if (o == -1)
        {
            return -1;
        }

        o &= 0x7FFF;
        qualOilPtr2 = o;
        qualOilPos2 = 0;
        return qual_GetNextOiList2();
    }

    short qual_GetNextOiList2()
    {
        short o;
        /*
        if (qualOilPtr2 == -1)
        {
            return -1;
        }
        if (qualOilPos2 >= qualToOiList[qualOilPtr2].qoiList.length)
        {
            return -1;
        }
        */
        if (qualOilPtr2 == -1 || qualOilPos2 >= qualToOiList[qualOilPtr2].qoiList.length)
        {
            return -1;
        }
        o = qualToOiList[qualOilPtr2].qoiList[qualOilPos2 + 1];
        qualOilPos2 += 2;
        return (o);
    }

    // Fabrique la liste des collisions par sprite: ouvre les qualifiers
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    int make_ColList1(CEventGroup evgPtr, int colList, short oi1)
    {
        short oi2;
        short flag;
        int code;
        short o;
        CEvent evtPtr;
        CParam evpPtr;
        int evt;

        CObjInfo[] localOiList = rhPtr.rhOiList;
        for (evt = 0; evt < evgPtr.evgNCond; evt++)
        {
            evtPtr = evgPtr.evgEvents[evt];
            if (EVTTYPE(evtPtr.evtCode) >= 2)
            {
                flag = (short) (0x8000 + CObjInfo.OILIMITFLAGS_BACKDROPS);
                code = getEventCode(evtPtr.evtCode);
                switch (code)
                {
                    case (-14 << 16):         // CNDL_EXTCOLLISION:
                        evpPtr = evtPtr.evtParams[0];
                        for (o = qual_GetFirstOiList(evtPtr.evtOiList); o != -1; o = qual_GetNextOiList())
                        {
                            oi2 = localOiList[o].oilOi;
                            if (oi1 == oi2)
                            {
                                flag = 0;
                                colList = make_ColList2(colList, oi1, ((PARAM_OBJECT) evpPtr).oiList);
                            }
                        }
                        if (flag == 0)
                        {
                            break;
                        }
                        for (o = qual_GetFirstOiList(((PARAM_OBJECT) evpPtr).oiList); o != -1; o = qual_GetNextOiList())
                        {
                            oi2 = localOiList[o].oilOi;
                            if (oi1 == oi2)
                            {
                                colList = make_ColList2(colList, oi1, evtPtr.evtOiList);
                            }
                        }
                        break;
                    case (-12 << 16):         // CNDL_EXTOUTPLAYFIELD:
                        evpPtr = evtPtr.evtParams[0];
                        flag = (short) (0x8000 + ((PARAM_SHORT) evpPtr).value);
                    case (-13 << 16):         // CNDL_EXTCOLBACK:
                        for (o = qual_GetFirstOiList(evtPtr.evtOiList); o != -1; o = qual_GetNextOiList())
                        {
                            oi2 = localOiList[o].oilOi;
                            if (oi1 == oi2)
                            {
                                colBuffer[colList++] = oi1;
                                colBuffer[colList++] = flag;
                            }
                        }
                        break;
                }
            }
        }
        return (colList);
    }

    int make_ColList2(int colList, short oi1, short ol)
    {
        short oi2;
        short o;
        CObjInfo[] localOiList = rhPtr.rhOiList;
        for (o = qual_GetFirstOiList(ol); o != -1; o = qual_GetNextOiList())
        {
            oi2 = localOiList[o].oilOi;

            int pos;
            for (pos = 0; pos < colList; pos += 2)
            {
                if (colBuffer[pos] == oi1 && colBuffer[pos + 1] == oi2)
                {
                    break;
                }
            }
            if (pos == colList)
            {
                colBuffer[colList++] = oi1;
                colBuffer[colList++] = oi2;
            }
        }
        return colList;
    }

    // POSITIONNE LES FLAGS DE MASQUE DE COLLISION
    ///////////////////////////////////////////////
    public int getCollisionFlags()
    {
        CEventGroup evgPtr;
        CEvent evtPtr;
        CParam evpPtr;
        int evg, evt, evp;
        int flag = 0;
        int events_length = events.length;
        for (evg = 0; evg < events_length; evg++)
        {
            evgPtr = events[evg];

            if(evgPtr == null)
            	continue;
            int evgPtr_size = evgPtr.evgNAct + evgPtr.evgNCond;
            for (evt = 0; evt < evgPtr_size; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];

                if(evtPtr == null)
                	continue;
                
                if (evtPtr.evtNParams > 0)
                {
                    for (evp = 0; evp < evtPtr.evtNParams; evp++)
                    {
                        evpPtr = evtPtr.evtParams[evp];
                        if (evpPtr != null && evpPtr.code == 43)	    // PARAM_PASTE
                        {
                            PARAM_SHORT p = (PARAM_SHORT) evpPtr;
                            switch (p.value)
                            {
                                case 1:
                                    flag |= CColMask.CM_OBSTACLE;
                                    break;
                                case 2:
                                    flag |= CColMask.CM_PLATFORM;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        return flag;
    }

    // ENUMERATION DES SONS
    public void enumSounds(IEnum musics, IEnum sounds)
    {
        CEventGroup evgPtr;
        CEvent evtPtr;
        int evg, evt, p;
        PARAM_SAMPLE pSample;

        for (evg = 0; evg < nEvents; evg++)
        {
            evgPtr = events[evg];
            int evgPtr_size = evgPtr.evgNCond + evgPtr.evgNAct;
            for (evt = 0; evt < evgPtr_size; evt++)
            {
                evtPtr = evgPtr.evgEvents[evt];
                
                if(evtPtr == null)
                	continue;
                
                for (p = 0; p < evtPtr.evtNParams; p++)
                {
                    switch (evtPtr.evtParams[p].code)
                    {
                        case 6:	    // PARAM_SAMPLE
                        case 35:    // PARAM_CNDSAMPLE

                            //Log.Log("Calling sounds.enumerate");

                            pSample = (PARAM_SAMPLE) evtPtr.evtParams[p];
                            sounds.enumerate(pSample.sndHandle);
                            break;

                        case 7:	    // PARAM_MUSIC
                        case 36:    // PARAM_CNDMUSIC

                            //Log.Log("Calling musics.enumerate");

                            pSample = (PARAM_SAMPLE) evtPtr.evtParams[p];
                            musics.enumerate(pSample.sndHandle);
                            break;
                    }
                }
            }
        }
    }

    int evg_FindAction(CEventGroup evgPtr, int n)
    {
        return evgPtr.evgNCond + n;
    }

    public short EVTTYPE(int code)
    {
        return (short) (code & 0xFFFF);
    }

    public short EVTNUM(int code)
    {
        return (short) ((code >>> 16) & 0xFFFF);
    }

    public int getEventCode(int code)
    {
        return code & 0xFFFF0000;
    }

    // FOR EACH
    ////////////////////////////////////////////////////////////////////////
    public void endForEach()
    {
    	this.callEndForEach = false;

    	CForEach saveForEach = this.rhPtr.rh4CurrentForEach;
    	CForEach saveForEach2 = this.rhPtr.rh4CurrentForEach2;
    	while(true)
    	{
    		CForEach pForEach=this.rhPtr.rh4ForEachs;
    		CForEach pForEach2 = null;
    		CForEach pPrevious = null;
    		while(pForEach!=null)
    		{
    			if (pForEach.index<0)
    			{
    				pForEach2 = pForEach.next;
    				if (pForEach2 != null)
    				{
    					if (!pForEach.name.equalsIgnoreCase(pForEach2.name))
    						pForEach2 = null;
    				}
    				break;
    			}
    			pPrevious = pForEach;
    			pForEach=pForEach.next;
    		}

    		if (pForEach == null)
    			break;

    		pForEach.stop=false;
    		for (pForEach.index=0; pForEach.index<pForEach.number; pForEach.index++)
    		{
    			this.rhPtr.rh4CurrentForEach=pForEach;
    			this.rhPtr.rh4CurrentForEach2=pForEach2;
    			if (pForEach2 != null)
    				pForEach2.index = pForEach.index;
    			this.rh2ActionOn=false;
    			this.handle_Event(pForEach.objects[pForEach.index], (-41 << 16));    // CNDL_EXTONLOOP);
    			if (pForEach.stop)
    				break;
    		}
    		if (pForEach2 != null)
    		{
    			pForEach2.stop=false;
    			for (pForEach2.index=0; pForEach2.index<pForEach2.number; pForEach2.index++)
    			{
    				this.rhPtr.rh4CurrentForEach=pForEach2;
    				this.rhPtr.rh4CurrentForEach2=pForEach;
    				if (pForEach != null)
    					pForEach.index = pForEach2.index;
    				this.rh2ActionOn=false;
    				this.handle_Event(pForEach2.objects[pForEach2.index], (-41 << 16));    // CNDL_EXTONLOOP);
    				if (pForEach2.stop)
    					break;
    			}
    		}
    		if (pForEach2!=null)
    			pForEach.next = pForEach2.next;
    		if (pPrevious==null)
    			this.rhPtr.rh4ForEachs = pForEach.next;
    		else
    			pPrevious.next = pForEach.next;
    	}
    	this.rhPtr.rh4CurrentForEach=saveForEach;
    	this.rhPtr.rh4CurrentForEach2=saveForEach2;
    }

    public void addForEach(String pName, CObject pHo, short oi)
    {
        CForEach pForEach = this.rhPtr.rh4ForEachs;
        CForEach pForEachPrevious = null;
        while(pForEach!=null)
        {
            if (pName.equalsIgnoreCase(pForEach.name) && pForEach.oi==oi)
            {
                pForEach.addObject(pHo);
                return;
            }
            pForEachPrevious=pForEach;
            pForEach=pForEach.next;
        }

        pForEach = new CForEach();
        if (pForEachPrevious!=null)
            pForEachPrevious.next = pForEach;
        else
            this.rhPtr.rh4ForEachs = pForEach;
        
        pForEach.next = null;
        pForEach.oi = oi;
        pForEach.addObject(pHo);
        pForEach.index = -1;
        pForEach.name = pName;
    }

}
