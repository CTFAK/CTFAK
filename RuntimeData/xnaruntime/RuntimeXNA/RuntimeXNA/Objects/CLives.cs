//----------------------------------------------------------------------------------
//
// CLives : Objet lives
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
    class CLives : CObject, IDrawing
    {
        public short rsPlayer;
        public CValue rsValue;
        public int rsBoxCx;			// Dimensions box (for lives, counters, texts)
        public int rsBoxCy;
        public short rsFont;				// Temporary font for texts
        public int rsColor1;			// Bar color
        public int displayFlags;
        public CRect tempRc = null;

        public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
        {
            rsFont = -1;
            rsColor1 = 0;
            hoImgWidth = hoImgHeight = 1;		// 0

            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;
            hoImgWidth = rsBoxCx = adCta.odCx;
            hoImgHeight = rsBoxCy = adCta.odCy;
            rsColor1 = adCta.ocColor1;
            rsPlayer = adCta.odPlayer;
            displayFlags = adCta.odDisplayFlags;
            rsValue = new CValue(hoAdRunHeader.rhApp.getLives()[rsPlayer - 1]);
        }
        public override void handle()
        {
	        int[] lives=hoAdRunHeader.rhApp.getLives();
	        if ( rsPlayer>0 && rsValue.getInt()!=lives[rsPlayer-1] )
	        {
	            rsValue.forceInt(lives[rsPlayer-1]);
	            roc.rcChanged=true;
	        }
            ros.handle();
            if (roc.rcChanged)
            {
                roc.rcChanged=false;
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
                return;
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;

            int vInt = rsValue.getInt();

            short img;
            CImage ifo;
            string s = CServices.intToString(vInt, displayFlags);
            switch (adCta.odDisplayType)
            {
                case 4:	    // CTA_ANIM:
                    if (vInt != 0)
                    {
                        ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(adCta.frames[0]);
                        int lg = vInt * ifo.width;
                        if (lg <= rsBoxCx)
                        {
                            hoImgWidth = (short)lg;
                            hoImgHeight = ifo.height;
                        }
                        else
                        {
                            hoImgWidth = rsBoxCx;
                            hoImgHeight = ((rsBoxCx / ifo.width) + vInt - 1) * ifo.height;
                        }
                        break;
                    }
                    hoImgWidth = hoImgHeight = 1;		// 0
                    break;
                case 1:	    // CTA_DIGITS:
                    int i;
                    int dx = 0;
                    int dy = 0;
                    for (i = 0; i < s.Length; i++)
                    {
                        char c = s[i];
                        img = 0;
                        if (c == '-')
                            img = adCta.frames[10];		// COUNTER_IMAGE_SIGN_NEG
                        else if (c == '.')
                            img = adCta.frames[12];		// COUNTER_IMAGE_POINT
                        else if (c == '+')
                            img = adCta.frames[11];	// COUNTER_IMAGE_SIGN_PLUS
                        else if (c == 'e' || c == 'E')
                            img = adCta.frames[13];	// COUNTER_IMAGE_EXP
                        else if (c >= '0' && c <= '9')
                            img = adCta.frames[c - '0'];
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
                        nFont = adCta.odFont;
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
                            hoImgHeight = (short)(rc.bottom - rc.top);
                        hoImgYSpot = hoImgHeight;
                    }
                    break;
            }
        }

        public override void draw(SpriteBatchEffect batch)
        {
            if (hoCommon.ocCounters == null)
                return;
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;
            int effect = ros.rsEffect;
            int effectParam = ros.rsEffectParam;

            int vInt = rsValue.getInt();
            String s = CServices.intToString(vInt, displayFlags);
            int x, y;
            CImage ifo;
            switch (adCta.odDisplayType)
            {
                case 4:	    // CTA_ANIM:
                    if (vInt != 0)
                    {
                        ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(adCta.frames[0]);
                        int x1 = hoRect.left;
                        int y1 = hoRect.top;
                        int x2 = hoRect.right;
                        int y2 = hoRect.bottom;
                        for (y = y1; y < y2 && vInt > 0; y += ifo.height)
                        {
                            for (x = x1; x < x2 && vInt > 0; x += ifo.width, vInt -= 1)
                            {
                                hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, adCta.frames[0], x, y, 0, effect, effectParam);
                            }
                        }
                    }
                    break;
                case 1:	    // CTA_DIGITS:
                    x = hoRect.left;
                    y = hoRect.top;

                    int i;
                    short img;
                    for (i = 0; i < s.Length; i++)
                    {
                        char c = s[i];
                        img = 0;
                        if (c == '-')
                            img = adCta.frames[10];		// COUNTER_IMAGE_SIGN_NEG
                        else if (c == '.')
                            img = adCta.frames[12];		// COUNTER_IMAGE_POINT
                        else if (c == '+')
                            img = adCta.frames[11];	// COUNTER_IMAGE_SIGN_PLUS
                        else if (c == 'e' || c == 'E')
                            img = adCta.frames[13];	// COUNTER_IMAGE_EXP
                        else if (c >= '0' && c <= '9')
                            img = adCta.frames[c - '0'];
                        hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, img, x, y, 0, effect, effectParam);
                        ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                        x += ifo.width;
                    }
                    break;

                case 5:	// CTA_TEXT:
                    // Get font
                    short nFont = rsFont;
                    if (nFont == -1)
                        nFont = adCta.odFont;
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

        public CFontInfo getFont()
        {
            CDefCounters adCta = (CDefCounters)hoCommon.ocCounters;
            if (adCta.odDisplayType == 5)	// CTA_TEXT
            {
                short nFont = rsFont;
                if (nFont == -1)
                    nFont = adCta.odFont;
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
