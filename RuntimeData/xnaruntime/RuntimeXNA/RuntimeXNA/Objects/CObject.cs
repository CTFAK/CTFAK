//----------------------------------------------------------------------------------
//
// COBJECT : Classe de base d'un objet'
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.OI;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;
using RuntimeXNA.Movements;
using RuntimeXNA.Animations;
using RuntimeXNA.Values;
using RuntimeXNA.Params;
using RuntimeXNA.Frame;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Objects
{
    public class CObject : IDrawing
    {
        public const short HOF_DESTROYED = 0x0001;
        public const short HOF_TRUEEVENT = 0x0002;
        public const short HOF_REALSPRITE = 0x0004;
        public const short HOF_FADEIN = 0x0008;
        public const short HOF_FADEOUT = 0x0010;
        public const short HOF_OWNERDRAW = 0x0020;
        public const short HOF_NOCOLLISION = 0x2000;
        public const short HOF_FLOAT = 0x4000;
        public const short HOF_STRING = unchecked((short)0x8000);

        // HeaderObject
        public short hoNumber=0;					// Number of the object
        public short hoNextSelected=0;				// Selected object list!!! DO NOT CHANGE POSITION!!!
        public CRun hoAdRunHeader=null;                                  // Run-header address
        public short hoHFII=0;					// Number of LevObj
        public short hoOi=0;						// Number of OI
        public short hoNumPrev=0;					// Same OI previous object
        public short hoNumNext=0;					// ... next
        public short hoType=0;					// Type of the object
        public short hoCreationId=0;                                  // Number of creation
        public CObjInfo hoOiList=null;                                   // Pointer to OILIST information
        public int hoEvents=0;					// Pointer to specific events
        public CArrayList hoPrevNoRepeat = null;                       // One-shot event handling
        public CArrayList hoBaseNoRepeat = null;
        public int hoMark1=0;                                         // #of loop marker for the events
        public int hoMark2=0;
        public string hoMT_NodeName=null;				// Name fo the current node for path movements
        public int hoEventNumber=0;                                   // Number of the event called (for extensions)
        public CObjectCommon hoCommon=null;				// Common structure address
        public int hoCalculX=0;					// Low weight value
        public int hoX=0;                                             // X coordinate
        public int hoCalculY=0;					// Low weight value
        public int hoY=0;						// Y coordinate
        public int hoImgXSpot=0;					// Hot spot of the current image
        public int hoImgYSpot=0;
        public int hoImgWidth=0;					// Width of the current picture
        public int hoImgHeight=0;
        public CRect hoRect = new CRect();        				// Display rectangle
        public int hoOEFlags=0;					// Objects flags
        public short hoFlags=0;					// Flags
        public byte hoSelectedInOR=0;                                 // Selection lors d'un evenement OR
        public int hoOffsetValue=0;                                   // Values structure offset
        public int hoLayer=0;                                         // Layer
        public short hoLimitFlags=0;                                  // Collision limitation flags
        public short hoPreviousQuickDisplay = 0;                            // Quickdraw list
        public short hoNextQuickDisplay = 0;                            // Quickdraw list
        public int hoCurrentParam=0;                                  // Address of the current parameter
        public int hoIdentifier=0;                                    // ASCII identifier of the object
        public bool hoCallRoutine=false;
        // Classes de gestion communes
        public CRCom roc;
        public CRMvt rom;
        public CRAni roa;
        public CRVal rov;
        public CRSpr ros;


        // Routines diverses
        public void setScale(float fScaleX, float fScaleY, bool bResample)
        {
            if (roc.rcScaleX != fScaleX || roc.rcScaleY != fScaleY)
            {
                roc.rcScaleX = fScaleX;
                roc.rcScaleY = fScaleY;
                roc.rcChanged = true;

                CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageInfoEx(roc.rcImage, roc.rcAngle, roc.rcScaleX, roc.rcScaleY);
                hoImgWidth = ifo.width;
                hoImgHeight = ifo.height;
                hoImgXSpot = ifo.xSpot;
                hoImgYSpot = ifo.ySpot;
            }
        }

        public void shtCreate(PARAM_SHOOT p, int x, int y, int dir)
        {
            int nLayer = hoLayer;
            int num = hoAdRunHeader.f_CreateObject(p.cdpHFII, p.cdpOi, x, y, dir, (short)(CRun.COF_NOMOVEMENT | CRun.COF_HIDDEN), nLayer, -1);
            if (num >= 0)
            {
                // Cree le movement
                // ----------------
                CObject pHo = hoAdRunHeader.rhObjectList[num];
                if (pHo.rom != null)
                {
                    pHo.rom.initSimple(pHo, CMoveDef.MVTYPE_BULLET, false);
                    pHo.roc.rcDir = dir;						// Met la direction de depart
                    pHo.roc.rcSpeed = p.shtSpeed;					// Met la vitesse
                    CMoveBullet mBullet = (CMoveBullet)pHo.rom.rmMovement;
                    mBullet.init2(this);

                    // Hide object if layer hidden
                    // ---------------------------
                    if (nLayer != -1)
                    {
                        if ((pHo.hoOEFlags & CObjectCommon.OEFLAG_SPRITES) != 0)
                        {
                            // Hide object if layer hidden
                            CLayer layer = hoAdRunHeader.rhFrame.layers[nLayer];
                            if ((layer.dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) != CLayer.FLOPT_VISIBLE)
                            {
                                pHo.ros.obHide();
                            }
                        }
                    }

                    // Met l'objet dans la liste des objets selectionnes
                    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    hoAdRunHeader.rhEvtProg.evt_AddCurrentObject(pHo);

                    // Force l'animation SHOOT si definie
                    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    if ((hoOEFlags & CObjectCommon.OEFLAG_ANIMATIONS) != 0)
                    {
                        if (roa.anim_Exist(CAnim.ANIMID_SHOOT))
                        {
                            roa.animation_Force(CAnim.ANIMID_SHOOT);
                            roa.animation_OneLoop();
                        }
                    }
                }
                else
                {
                    hoAdRunHeader.destroy_Add(pHo.hoNumber);
                }
            }
        }

        // Fonctions de base
        public virtual void init(CObjectCommon ocPtr, CCreateObjectInfo cob) { }

        public virtual void handle() { }

        public virtual void modif() { }

        public virtual void display() { }

        public virtual void kill(bool bFast) { }

        public virtual void killBack() { }

        public virtual void getZoneInfos() { }

        public virtual void draw(SpriteBatchEffect batch) { }

        public virtual CMask getCollisionMask(int flags) { return null; }

        public virtual void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y) { }
        public virtual void drawableKill() { }
        public virtual CMask drawableGetMask(int flags) { return null; }

    }
}
