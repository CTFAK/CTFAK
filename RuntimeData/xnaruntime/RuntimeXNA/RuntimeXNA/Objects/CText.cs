//----------------------------------------------------------------------------------
//
// CText : Objet string
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
using RuntimeXNA.Expressions;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Objects
{
    class CText : CObject, IDrawing
    {
        public short rsFlag;
        public int rsBoxCx;
        public int rsBoxCy;
        public int rsMaxi;
        public int rsMini;
        public byte rsHidden;
        public string rsTextBuffer;
        public short rsFont;
        public int rsTextColor;
        public int deltaY;

        public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
        {
            rsFlag = 0;										// ???? adlo->loFlags;
            CDefTexts txt = (CDefTexts)ocPtr.ocObject;
            hoImgWidth = txt.otCx;
            hoImgHeight = txt.otCy;
            rsBoxCx = txt.otCx;
            rsBoxCy = txt.otCy;

            // Recuperer la couleur et le nombre de phrases
            rsMaxi = txt.otNumberOfText;
            rsTextColor = 0;
            if (txt.otTexts.Length > 0)
            {
                rsTextColor = txt.otTexts[0].tsColor;
            }
            rsHidden = (byte)cob.cobFlags;					// A Toujours?
            rsTextBuffer = null;
            rsFont = -1;
            rsMini = 0;
            if ((rsHidden & CRun.COF_FIRSTTEXT) != 0)
            {
                if (txt.otTexts.Length > 0)
                {
                    rsTextBuffer = txt.otTexts[0].tsText;
                }
                else
                {
                    rsTextBuffer = "";
                }
            }
        }

        public override void handle()
        {
            ros.handle();
            if (roc.rcChanged)
            {
                roc.rcChanged = false;
                modif();
            }
        }

        public override void modif()
        {
            ros.modifRoutine();
        }

        public override void display()
        {
            ros.displayRoutine();
        }

        public override void getZoneInfos()
        {
            CDefTexts txt = (CDefTexts)hoCommon.ocObject;
            short flags = txt.otTexts[0].tsFlags;

            // Rectangle
            CRect rc = new CRect();
            rc.left = hoX - hoAdRunHeader.rhWindowX + 1;
            rc.top = hoY - hoAdRunHeader.rhWindowY;
            rc.right = rc.left + rsBoxCx;
            rc.bottom = rc.top + rsBoxCy;
            hoImgWidth = (short)(rc.right - rc.left);
            hoImgHeight = (short)(rc.bottom - rc.top);
            hoImgXSpot = 0;
            hoImgYSpot = 0;

            // Get font
            short nFont = rsFont;
            if (nFont == -1)
            {
                if (txt.otTexts.Length > 0)
                {
                    nFont = txt.otTexts[0].tsFont;
                }
            }
            CFont font = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(nFont);

            // Calcul dimensions exacte zone
            string s;
            if (rsMini >= 0)
            {
                s = txt.otTexts[rsMini].tsText;
            }
            else
            {
                s = rsTextBuffer;
                if (s == null)
                {
                    s = "";
                }
            }

            /////////////////////////////////////
            // Vertical alignment & multi-line ? do not compute size, because DrawText doesn't support v.align for multi-line texts

            // Allow only the following flags
            short dtflags = (short)(flags & (CServices.DT_LEFT | CServices.DT_CENTER | CServices.DT_RIGHT |
                    CServices.DT_TOP | CServices.DT_BOTTOM | CServices.DT_VCENTER |
                    CServices.DT_SINGLELINE));
            int ht = 0;
//            if ((dtflags & (CServices.DT_BOTTOM | CServices.DT_VCENTER)) == 0 || (dtflags & CServices.DT_SINGLELINE) != 0)
            {
                // Force the following flags
                int x2 = rc.right;
                ht = CServices.drawText(null, s, (short)(dtflags | CServices.DT_CALCRECT), rc, rsTextColor, font, 0, 0);
                rc.right = x2;	// keep zone width
            }

            /////////////////////////////////////
            if (ht != 0)
            {
                deltaY = 0;
                if ((dtflags & CServices.DT_BOTTOM) != 0)
                {
                    deltaY = hoImgHeight - ht;
                }
                else if ((dtflags & CServices.DT_VCENTER) != 0)
                {
                    deltaY = hoImgHeight / 2 - ht/2;
                }
                else
                {
                    hoImgWidth = (short)(rc.right - rc.left);
                    hoImgHeight = (short)(rc.bottom - rc.top);
                }
            }
        }

        public override void draw(SpriteBatchEffect batch)
        {
            int effect = ros.rsEffect&CSpriteGen.BOP_MASK;
            int effectParam = ros.rsEffectParam;
            CDefTexts txt = (CDefTexts)hoCommon.ocObject;
            short flags = txt.otTexts[0].tsFlags;

            // Get font
            short nFont = rsFont;
            if (nFont == -1)
            {
                if (txt.otTexts.Length > 0)
                {
                    nFont = txt.otTexts[0].tsFont;
                }
            }
            CFont font = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(nFont);

            // Affichage
            string s = null;
            if (rsMini >= 0)
            {
                s = txt.otTexts[rsMini].tsText;
            }
            else
            {
                s = rsTextBuffer;
                if (s == null)
                {
                    s = "";
                }
            }

            CRect rc = new CRect();
            rc.copyRect(hoRect);
            rc.offsetRect(hoAdRunHeader.rhApp.xOffset, hoAdRunHeader.rhApp.yOffset);
            rc.top += deltaY;
            rc.left++;

            // Allow only the following flags
            short dtflags = (short)(flags & (CServices.DT_LEFT | CServices.DT_CENTER | CServices.DT_RIGHT |
                    CServices.DT_TOP | CServices.DT_BOTTOM | CServices.DT_VCENTER |
                    CServices.DT_SINGLELINE));
            // Adjust rectangle
            int ht = CServices.drawText(batch, s, (short)(dtflags&~(CServices.DT_VCENTER|CServices.DT_BOTTOM)), rc, rsTextColor, font, effect, effectParam);
        }

        public CFontInfo getFont()
        {
            short nFont = rsFont;
            if (nFont == -1)
            {
                CDefTexts txt = (CDefTexts)hoCommon.ocObject;
                nFont = txt.otTexts[0].tsFont;
            }
            return hoAdRunHeader.rhApp.fontBank.getFontInfoFromHandle(nFont);
        }

        public void setFont(CFontInfo info, CRect pRc)
        {
            rsFont = hoAdRunHeader.rhApp.fontBank.addFont(info);
            if (pRc != null)
            {
                hoImgWidth = rsBoxCx = pRc.right - pRc.left;
                hoImgHeight = rsBoxCy = pRc.bottom - pRc.top;
            }
            modif();
            roc.rcChanged = true;
        }

        public int getFontColor()
        {
            return rsTextColor;
        }

        public void setFontColor(int rgb)
        {
            rsTextColor = rgb;
            modif();
            roc.rcChanged = true;
        }

        public bool txtChange(int num)
        {
            if (num < -1)
            {
                num = -1;							// -1==chaine stockee...
            }
            if (num >= rsMaxi)
            {
                num = rsMaxi - 1;
            }
            if (num == rsMini)
            {
                return false;
            }

            rsMini = num;

            // -------------------------------
            // Recopie le texte dans la chaine
            // -------------------------------
            if (num >= 0)
            {
                CDefTexts txt = (CDefTexts)hoCommon.ocObject;
                txtSetString(txt.otTexts[rsMini].tsText);
            }

            // Reafficher ou pas?
            // ------------------
            if ((ros.rsFlags & CRSpr.RSFLAG_HIDDEN) != 0)
            {
                return false;
            }
            return true;
        }

        public void txtSetString(string s)
        {
            rsTextBuffer = s;
        }

        public override void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
        {
            draw(batch);
        }
        public override void drawableKill() { }
        public override CMask drawableGetMask(int flags) { return null; }
    }
}
