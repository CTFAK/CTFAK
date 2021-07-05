//----------------------------------------------------------------------------------
//
// CCounter : Objet compteur
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Objects
{
    class CCounter : CObject, IDrawing
    {
        public short rsFlags;			// Type + flags
        public int rsMini;
        public int rsMaxi;				// 
        public CValue rsValue;
        public int rsBoxCx;			// Dimensions box (for lives, counters, texts)
        public int rsBoxCy;
        public double rsMiniDouble;
        public double rsMaxiDouble;
        public short rsOldFrame;			// Counter only 
        public byte rsHidden=0;
        public short rsFont;				// Temporary font for texts
        public int rsColor1;			// Bar color
        public int rsColor2;			// Gradient bar color
        public int displayFlags;
        public Texture2D texture=null;
        public CRect tempRc = null;

        public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
        {
            // Hidden counter?
            rsFlags = 0;			// adlo->loFlags; V2 pourquoi y avait ca en V1???
            rsFont = -1;
            rsColor1 = 0;
            rsColor2 = 0;
            hoImgWidth = hoImgHeight = 1;		// 0

            if (hoCommon.ocCounters == null)
            {
                hoImgWidth = rsBoxCx = 1;
                hoImgHeight = rsBoxCy = 1;
            }
            else
            {
                CDefCounters ctPtr = (CDefCounters)hoCommon.ocCounters;
                hoImgWidth = rsBoxCx = ctPtr.odCx;
                hoImgHeight = rsBoxCy = ctPtr.odCy;
                displayFlags = ctPtr.odDisplayFlags;
                switch (ctPtr.odDisplayType)
                {
                    case 5:	    // CTA_TEXT:
                        rsColor1 = ctPtr.ocColor1;
                        break;
                    case 2:	    // CTA_VBAR:
                    case 3:	    // CTA_HBAR:
                        rsColor1 = ctPtr.ocColor1;
                        rsColor2 = ctPtr.ocColor2;
                        break;
                }
            }

            CDefCounter cPtr = (CDefCounter)hoCommon.ocObject;
            rsMini = cPtr.ctMini;
            rsMaxi = cPtr.ctMaxi;
            rsMiniDouble = (double)rsMini;
            rsMaxiDouble = (double)rsMaxi;
            rsValue = new CValue(cPtr.ctInit);
            rsOldFrame = -1;
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
            // Hidden counter?
            hoImgWidth = hoImgHeight = 1;		// 0
            if (hoCommon.ocCounters == null)
            {
                return;
            }
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;

            int vInt = 0;
            double vDouble = 0;
            if (rsValue.getType() == CValue.TYPE_INT)
            {
                vInt = rsValue.getInt();
            }
            else
            {
                vDouble = rsValue.getDouble();
                vInt = (int)vDouble;
            }

            int nbl;
            short img;
            string s = "";
            switch (adCta.odDisplayType)
            {
                case 4:	    // CTA_ANIM:
                    nbl = adCta.nFrames;
                    nbl -= 1;
                    if (rsMaxi <= rsMini)
                    {
                        rsOldFrame = 0;
                    }
                    else
                    {
                        rsOldFrame = (short)Math.Min(((int)((int)(vInt - rsMini) * (int)nbl) / (int)(rsMaxi - rsMini)), adCta.nFrames-1);
                    }
                    img = adCta.frames[Math.Max(rsOldFrame-1, 0)];
                    CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                    rsBoxCx = hoImgWidth = ifo.width;
                    rsBoxCy = hoImgHeight = ifo.height;
                    hoImgXSpot = ifo.xSpot;
                    hoImgYSpot = ifo.ySpot;
                    break;

                case 2:	    // CTA_VBAR:
                case 3:	    // CTA_HBAR:
                    nbl = rsBoxCx;
                    if (adCta.odDisplayType == CDefCounters.CTA_VBAR)
                    {
                        nbl = rsBoxCy;
                    }
                    if (rsMaxi <= rsMini)
                    {
                        rsOldFrame = 0;
                    }
                    else
                    {
                        rsOldFrame = (short)(((vInt - rsMini) * nbl) / (rsMaxi - rsMini));
                    }
                    if (adCta.odDisplayType == CDefCounters.CTA_HBAR)
                    {
                        hoImgYSpot = 0;
                        hoImgHeight = rsBoxCy;
                        hoImgWidth = rsOldFrame;
                        if ((adCta.odDisplayFlags & CDefCounters.BARFLAG_INVERSE) != 0)
                        {
                            hoImgXSpot = rsOldFrame - rsBoxCx;
                        }
                        else
                        {
                            hoImgXSpot = 0;
                        }
                    }
                    else
                    {
                        hoImgXSpot = 0;
                        hoImgWidth = rsBoxCx;
                        hoImgHeight = rsOldFrame;
                        if ((adCta.odDisplayFlags & CDefCounters.BARFLAG_INVERSE) != 0)
                        {
                            hoImgYSpot = rsOldFrame - rsBoxCy;
                        }
                        else
                        {
                            hoImgYSpot = 0;
                        }
                    }
                    break;

                case 1:	    // CTA_DIGITS:
                    if (rsValue.getType() == CValue.TYPE_INT)
                    {
                        s = CServices.intToString(vInt, displayFlags);
                    }
                    else
                    {
//                        double frac = System.Math.Round(vDouble);
                        s = CServices.doubleToString(vDouble, displayFlags);
/*                        if (vDouble - frac != 0)
                        {
                            s = CServices.doubleToString(vDouble, displayFlags);
                        }
                        else
                        {
                            vInt = (int)vDouble;
                            s = CServices.intToString(vInt, displayFlags);
                        }
*/
                    }

                    int i;
                    int dx = 0;
                    int dy = 0;
                    for (i = 0; i < s.Length; i++)
                    {
                        char c = s[i];
                        img = 0;
                        if (c == '-')
                        {
                            img = adCta.frames[10];		// COUNTER_IMAGE_SIGN_NEG
                        }
                        else if (c == '.' || c==',')
                        {
                            img = adCta.frames[12];		// COUNTER_IMAGE_POINT
                        }
                        else if (c == '+')
                        {
                            img = adCta.frames[11];	// COUNTER_IMAGE_SIGN_PLUS
                        }
                        else if (c == 'e' || c == 'E')
                        {
                            img = adCta.frames[13];	// COUNTER_IMAGE_EXP
                        }
                        else if (c >= '0' && c <= '9')
                        {
                            img = adCta.frames[c - '0'];
                        }
                        ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                        dx += ifo.width;
                        dy = Math.Max(dy, ifo.height);
                    }
                    hoImgWidth = dx;
                    hoImgHeight = dy;
                    hoImgXSpot = dx;
                    hoImgYSpot = dy;
                    break;

                case 5:	    // CTA_TEXT:
                    if (rsValue.getType() == CValue.TYPE_INT)
                    {
                        s = CServices.intToString(vInt, displayFlags);
                    }
                    else
                    {
                        s = CServices.doubleToString(vDouble, displayFlags);
/*                        double frac = System.Math.Round(vDouble);
                        if (vDouble - frac != 0)
                        {
                            s = String.Format("{0:F}", vDouble);
                        }
                        else
                        {
                            vInt = (int)vDouble;
                            s = CServices.intToString(vInt, displayFlags);
                        }
*/
                    }

                    // Rectangle
                    CRect rc = new CRect();

                    rc.left = hoX - hoAdRunHeader.rhWindowX;
                    rc.top = hoY - hoAdRunHeader.rhWindowY;
                    rc.right = rc.left + rsBoxCx;
                    rc.bottom = rc.top + rsBoxCy;
                    hoImgWidth = (short)(rc.right - rc.left);
                    hoImgHeight = (short)(rc.bottom - rc.top);
                    hoImgXSpot = hoImgYSpot = 0;

                    // Get font
                    short nFont = rsFont;
                    if (nFont == -1)
                    {
                        nFont = adCta.odFont;
                    }
                    CFont font = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(nFont);

                    // Get exact size
                    int ht = 0;
                    short dtflags = (short)(CServices.DT_RIGHT | CServices.DT_VCENTER | CServices.DT_SINGLELINE);
                    int x2 = rc.right;
                    ht = CServices.drawText(null, s, (short)(dtflags | CServices.DT_CALCRECT), rc, 0, font, 0, 0);
                    rc.right = x2;	// keep zone width
                    if (ht != 0)
                    {
                        hoImgXSpot = hoImgWidth = (short)(rc.right - rc.left);
                        if (hoImgHeight < (rc.bottom - rc.top))
                        {
                            hoImgHeight = (short)(rc.bottom - rc.top);
                        }
                        hoImgYSpot = hoImgHeight;
                    }
                    break;
                default:
                    hoImgWidth = hoImgHeight = 1;		// 0
                    break;
            }
        }

        public override void draw(SpriteBatchEffect batch)
        {
            // Dispatcher suivant l'objet et son ctaType
            // -----------------------------------------
            if (hoCommon.ocCounters == null)
            {
                return;
            }
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;
            int effect = ros.rsEffect;
            int effectParam = ros.rsEffectParam;

            int vInt = 0;
            double vDouble = 0;
            if (rsValue.getType() == CValue.TYPE_INT)
            {
                vInt = rsValue.getInt();
            }
            else
            {
                vDouble = rsValue.getDouble();
                vInt = (int)vDouble;
            }

            int cx;
            int cy;
            int x;
            int y;
            string s = "";
            int color1, color2;
            color1 = rsColor1;
            color2 = 0;
            switch (adCta.odDisplayType)
            {
                case 4:	    // CTA_ANIM:
                    hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, adCta.frames[Math.Max(rsOldFrame-1, 0)], hoRect.left, hoRect.top, 0, effect, effectParam);
                    break;

                case 2:	    // CTA_VBAR:
                case 3:	    // CTA_HBAR:
                    int nbl = rsBoxCx;
                    if (adCta.odDisplayType == CDefCounters.CTA_VBAR)
                    {
                        nbl = rsBoxCy;
                    }
                    cx = hoRect.right - hoRect.left;
                    cy = hoRect.bottom - hoRect.top;
                    x = hoRect.left;
                    y = hoRect.top;

                    // Si gradient, calcul de la couleur destination
/*                    if (adCta.ocFillType == 2)	// FILLTYPE_GRADIENT
                    {
                    }
*/
                    // Display INKEFFECT: ajouter effect flags
                    switch (adCta.ocFillType)
                    {
                        case 1:			    // FILLTYPE_SOLID
                            Color c = CServices.getColor(color1);
                            hoAdRunHeader.rhApp.services.drawFilledRectangleSub(batch, x + hoAdRunHeader.rhApp.xOffset, y + hoAdRunHeader.rhApp.yOffset, cx, cy, c, CSpriteGen.BOP_COPY, 0);
                            break;
                        case 2:			    // FILLTYPE_GRADIENT
                            if (texture == null)
                            {
                                color1 = rsColor1;	// shape.ocFillData.ocColor1;
                                color2 = rsColor2;	// shape.ocFillData.ocColor2;
                                int dl = CServices.getRValueJava(color2) - CServices.getRValueJava(color1);
                                int r = ((dl * (int)rsOldFrame) / nbl + CServices.getRValueJava(color1)) & 0xFF;
                                dl = CServices.getGValueJava(color2) - CServices.getGValueJava(color1);
                                int g = ((dl * (int)rsOldFrame) / nbl + CServices.getGValueJava(color1)) & 0xFF;
                                dl = CServices.getBValueJava(color2) - CServices.getBValueJava(color1);
                                int b = ((dl * (int)rsOldFrame) / nbl + CServices.getBValueJava(color1)) & 0xFF;
                                color2 = CServices.RGBJava(r, g, b);
                                if ((adCta.odDisplayFlags & CDefCounters.BARFLAG_INVERSE) != 0)
                                {
                                    dl = color1;
                                    color1 = color2;
                                    color2 = dl;
                                }
                                bool bVertical = adCta.ocGradientFlags != 0;
                                texture = CServices.createGradientRectangle(hoAdRunHeader.rhApp, cx, cy, color1, color2, bVertical, 0, 0);
                            }
                            if (texture != null)
                            {
                                hoAdRunHeader.rhApp.tempRect.X = x + hoAdRunHeader.rhApp.xOffset;
                                hoAdRunHeader.rhApp.tempRect.Y = y + hoAdRunHeader.rhApp.yOffset;
                                hoAdRunHeader.rhApp.tempRect.Width = texture.Width;
                                hoAdRunHeader.rhApp.tempRect.Height = texture.Height;
                                batch.Draw(texture, hoAdRunHeader.rhApp.tempRect, null, Color.White);
                            }
                            break;
                    }
                    break;

                case 1:	    // CTA_DIGITS:
                    if (rsValue.getType() == CValue.TYPE_INT)
                    {
                        s = CServices.intToString(vInt, displayFlags);
                    }
                    else
                    {
                        s = CServices.doubleToString(vDouble, displayFlags);
/*                        double frac = System.Math.Round(vDouble);
                        if (vDouble - frac != 0)
                        {
                            s = String.Format("{0:F}", vDouble);
                        }
                        else
                        {
                            vInt = (int)vDouble;
                            s = CServices.intToString(vInt, displayFlags);
                        }
*/
                    }

                    x = hoRect.left;
                    y = hoRect.top;

                    int i;
                    short img;
                    for (i = 0; i < s.Length; i++)
                    {
                        char c = s[i];
                        img = 0;
                        if (c == '-')
                        {
                            img = adCta.frames[10];		// COUNTER_IMAGE_SIGN_NEG
                        }
                        else if (c == '.' || c==',')
                        {
                            img = adCta.frames[12];		// COUNTER_IMAGE_POINT
                        }
                        else if (c == '+')
                        {
                            img = adCta.frames[11];	// COUNTER_IMAGE_SIGN_PLUS
                        }
                        else if (c == 'e' || c == 'E')
                        {
                            img = adCta.frames[13];	// COUNTER_IMAGE_EXP
                        }
                        else if (c >= '0' && c <= '9')
                        {
                            img = adCta.frames[c - '0'];
                        }
                        hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, img, x, y, 0, effect, effectParam);
                        CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                        x += ifo.width;
                    }
                    break;

                case 5:	// CTA_TEXT:
                    if (rsValue.getType() == CValue.TYPE_INT)
                    {
                        s = CServices.intToString(vInt, displayFlags);
                    }
                    else
                    {
                        s = CServices.doubleToString(vDouble, displayFlags);
/*                        double frac = System.Math.Round(vDouble);
                        if (vDouble - frac != 0)
                        {
                            s = String.Format("{0:F}", vDouble);
                        }
                        else
                        {
                            vInt = (int)vDouble;
                            s = CServices.intToString(vInt, displayFlags);
                        }
*/
                    }

                    // Get font
                    short nFont = rsFont;
                    if (nFont == -1)
                    {
                        nFont = adCta.odFont;
                    }
                    CFont font = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(nFont);

                    short dtflags = (short)(CServices.DT_RIGHT | CServices.DT_VCENTER | CServices.DT_SINGLELINE);
                    if (hoRect.bottom - hoRect.top != 0)
                    {
                        if (tempRc == null)
                        {
                            tempRc = new CRect();
                        }
                        tempRc.copyRect(hoRect);
                        tempRc.offsetRect(hoAdRunHeader.rhApp.xOffset, hoAdRunHeader.rhApp.yOffset);
                        CServices.drawText(batch, s, dtflags, tempRc, rsColor1, font, effect, effectParam);
                    }
                    break;
            }
        }


        public void cpt_ToFloat(CValue pValue)
        {
            if (rsValue.getType() == CValue.TYPE_INT)
            {
                if (pValue.getType() == CValue.TYPE_INT)
                {
                    return;
                }
                rsValue.forceDouble((double)rsValue.getInt());
                display();
                roc.rcChanged = true;
            }
            else
            {
                pValue.convertToDouble();
            }
        }

        public void cpt_Change(CValue pValue)
        {
            if (rsValue.getType() == CValue.TYPE_INT)
            {
                // Compteur entier
                int value = pValue.getInt();
                if (value < rsMini)
                {
                    value = rsMini;
                }
                if (value > rsMaxi)
                {
                    value = rsMaxi;
                }
                if (value != rsValue.getInt())
                {
                    rsValue.forceInt(value);
                    texture = null;
                    modif();
                    //		roc.rcChanged=true;
                }
            }
            else
            {
                // Compteur float
                double d = pValue.getDouble();
                if (d < rsMiniDouble)
                {
                    d = rsMiniDouble;
                }
                if (d > rsMaxiDouble)
                {
                    d = rsMaxiDouble;
                }
                if (d != rsValue.getDouble())
                {
                    rsValue.forceDouble(d);
                    //		roc.rcChanged=true;
                    texture = null;
                    modif();
                }
            }
        }

        public void cpt_Add(CValue pValue)
        {
            cpt_ToFloat(pValue);
            CValue val = new CValue(rsValue);
            val.add(pValue);
            cpt_Change(val);
        }

        public void cpt_Sub(CValue pValue)
        {
            cpt_ToFloat(pValue);
            CValue val = new CValue(rsValue);
            val.sub(pValue);
            cpt_Change(val);
        }

        public void cpt_SetMin(CValue value)
        {
            rsMini = value.getInt();
            rsMiniDouble = value.getDouble();
            CValue val = new CValue(rsValue);
            cpt_Change(val);
        }

        public void cpt_SetMax(CValue value)
        {
            rsMaxi = value.getInt();
            rsMaxiDouble = value.getDouble();
            CValue val = new CValue(rsValue);
            cpt_Change(val);
        }

        public void cpt_SetColor1(int rgb)
        {
            rsColor1 = rgb;
            display();
            roc.rcChanged = true;
        }

        public void cpt_SetColor2(int rgb)
        {
            rsColor2 = rgb;
            display();
            roc.rcChanged = true;
        }

        public CValue cpt_GetValue()
        {
            return rsValue;
        }

        public CValue cpt_GetMin()
        {
            CValue v = new CValue();
            if (rsValue.type == CValue.TYPE_INT)
            {
                v.forceInt(rsMini);
            }
            else
            {
                v.forceDouble(rsMiniDouble);
            }
            return v;
        }

        public CValue cpt_GetMax()
        {
            CValue v = new CValue();
            if (rsValue.type == CValue.TYPE_INT)
            {
                v.forceInt(rsMaxi);
            }
            else
            {
                v.forceDouble(rsMaxiDouble);
            }
            return v;
        }

        public int cpt_GetColor1()
        {
            return rsColor1;
        }

        public int cpt_GetColor2()
        {
            return rsColor2;
        }

        public CFontInfo getFont()
        {
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;
            if (adCta.odDisplayType == 5)	// CTA_TEXT
            {
                short nFont = rsFont;
                if (nFont == -1)
                {
                    nFont = adCta.odFont;
                }
                return hoAdRunHeader.rhApp.fontBank.getFontInfoFromHandle(nFont);
            }
            return null;
        }

        public void setFont(CFontInfo info, CRect pRc)
        {
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;
            if (adCta.odDisplayType == 5)	// CTA_TEXT
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
        }

        public int getFontColor()
        {
            return rsColor1;
        }

        public void setFontColor(int rgb)
        {
            rsColor1 = rgb;
            modif();
            roc.rcChanged = true;
        }

        public override void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
        {
            draw(batch);
        }
        public override void drawableKill() { }
        public override CMask drawableGetMask(int flags) { return null; }
    }
}
