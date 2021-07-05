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
// CCounter : Objet compteur
//
//----------------------------------------------------------------------------------
package Objects;

import Expressions.CValue;
import Application.CRunApp;
import Banks.*;
import Sprites.*;
import OI.*;
import RunLoop.*;
import Runtime.MMFRuntime;
import Services.*;
import OpenGL.*;

/** Counter object.
 */
public class CCounter extends CObject
{
    public short rsFlags;			/// Type + flags
    public int rsMini;
    public int rsMaxi;				// 
    public CValue rsValue;
    public int rsBoxCx;			/// Dimensions box (for lives, counters, texts)
    public int rsBoxCy;
    public double rsMiniDouble;
    public double rsMaxiDouble;
    public short rsOldFrame;			/// Counter only 
    public byte rsHidden;
    public short rsFont;				/// Temporary font for texts
    public int rsColor1;			/// Bar color
    public int rsColor2;			/// Gradient bar color
    public int displayFlags;

    CTextSurface textSurface;

    public CCounter()
    {
    }

    @Override
	public void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
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
            CDefCounters ctPtr = hoCommon.ocCounters;
            hoImgWidth = rsBoxCx = ctPtr.odCx;
            hoImgHeight = rsBoxCy = ctPtr.odCy;
            displayFlags = ctPtr.odDisplayFlags;
            switch (ctPtr.odDisplayType)
            {
                case 5:	    // CTA_TEXT:
                    rsColor1 = ctPtr.ocColor1;
                    textSurface = new CTextSurface(hoAdRunHeader.rhApp, hoImgWidth, hoImgHeight);
                    break;
                case 2:	    // CTA_VBAR:
                case 3:	    // CTA_HBAR:
                    rsColor1 = ctPtr.ocColor1;
                    rsColor2 = ctPtr.ocColor2;
                    break;
            }
        }

        CDefCounter cPtr = (CDefCounter) hoCommon.ocObject;
        rsMini = cPtr.ctMini;
        rsMaxi = cPtr.ctMaxi;
        rsMiniDouble = rsMini;
        rsMaxiDouble = rsMaxi;
        rsValue = new CValue(cPtr.ctInit);
        rsOldFrame = -1;
	getZoneInfos();		//Using getZoneInfos() to get initial size.
    }

    @Override
	public void handle()
    {
        ros.handle();
        if (roc.rcChanged)
        {
            roc.rcChanged = false;
            modif();
        }
    }

    @Override
	public void modif()
    {
        ros.modifRoutine();
    }

    @Override
	public void display()
    {
        ros.displayRoutine();
    }

    @Override
	public void kill(boolean bFast)
    {
        if (textSurface != null)
            textSurface.recycle();
    }

    @Override
	public void getZoneInfos()
    {
        // Hidden counter?
        hoImgWidth = hoImgHeight = 1;		// 0
        if (hoCommon.ocCounters == null)
        {
            return;
        }
        CDefCounters adCta = hoCommon.ocCounters;

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
        String s = "";
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
                    rsOldFrame = (short) ((vInt - rsMini) * nbl / (rsMaxi - rsMini));
                    if (rsOldFrame>=adCta.nFrames)
                        rsOldFrame=(short)(adCta.nFrames-1);
                }
                img = adCta.frames[rsOldFrame];
                CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                if(ifo != null) {
	                rsBoxCx = hoImgWidth = ifo.getWidth();
	                rsBoxCy = hoImgHeight = ifo.getHeight();
	                hoImgXSpot = ifo.getXSpot();
	                hoImgYSpot = ifo.getYSpot();
                }
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
                    rsOldFrame = (short) (((vInt - rsMini) * nbl) / (rsMaxi - rsMini));
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
                    s = CServices.doubleToString(vDouble, displayFlags);
                }

                int i;
                int dx = 0;
                int dy = 0;
                for (i = 0; i < s.length(); i++)
                {
                    char c = s.charAt(i);
                    img = 0;
                    if (c == '-')
                    {
                        img = adCta.frames[10];		// COUNTER_IMAGE_SIGN_NEG
                    }
                    else if (c == '.')
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
                    if(ifo != null)
                    {
                        dx += ifo.getWidth();
                        dy = Math.max(dy, ifo.getHeight());
                    }
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
                    s = CServices.doubleToString (vDouble, displayFlags);
                }

                // Rectangle
                CRect rc = new CRect();

                rc.left = hoX - hoAdRunHeader.rhWindowX;
                rc.top = hoY - hoAdRunHeader.rhWindowY;
                rc.right = rc.left + rsBoxCx;
                rc.bottom = rc.top + rsBoxCy;
                hoImgWidth = (short) (rc.right - rc.left);
                hoImgHeight = (short) (rc.bottom - rc.top);
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
                short dtflags = (short) (CServices.DT_RIGHT | CServices.DT_VCENTER | CServices.DT_SINGLELINE);
                int x2 = rc.right;

                CRect rTemp = new CRect ();
                rTemp.copyRect (rc);
                textSurface.measureText (s, dtflags, font.getFontInfo(), rTemp, 0);
                ht = rTemp.bottom - rTemp.top;

                rc.right = x2;	// keep zone width
                if (ht != 0)
                {
                    hoImgXSpot = hoImgWidth = (short) (rc.right - rc.left);
                    if (hoImgHeight < (rc.bottom - rc.top))
                    {
                        hoImgHeight = (short) (rc.bottom - rc.top);
                    }
                    hoImgYSpot = hoImgHeight;
                }
                break;
            default:
                hoImgWidth = hoImgHeight = 1;		// 0
                break;
        }
    }

    @Override
	public void draw()
    {
        // Dispatcher suivant l'objet et son ctaType
        // -----------------------------------------
        if (hoCommon.ocCounters == null)
        {
            return;
        }
        CDefCounters adCta = hoCommon.ocCounters;
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
            vInt = (int) vDouble;
        }

        int cx;
        int cy;
        int x;
        int y;
        String s = "";
        int color1, color2;
        color1 = rsColor1;
        color2 = 0;
        switch (adCta.odDisplayType)
        {
            case 4:	    // CTA_ANIM:

                if(rsOldFrame > adCta.frames.length || rsOldFrame < 0)
                    break;

                hoAdRunHeader.spriteGen.pasteSpriteEffect(adCta.frames[rsOldFrame], hoRect.left, hoRect.top, 0, effect, effectParam);
                break;

            case 2:	    // CTA_VBAR:
            case 3:	    // CTA_HBAR:
                int nbl = rsBoxCx;
                if (adCta.odDisplayType == CDefCounters.CTA_VBAR)
                {
                    nbl = rsBoxCy;
                }

                // Si gradient, calcul de la couleur destination
                if (adCta.ocFillType == 2)	// FILLTYPE_GRADIENT
                {
                    color1 = rsColor1;	// shape.ocFillData.ocColor1;
                    color2 = rsColor2;	// shape.ocFillData.ocColor2;
                    int dl = CServices.getRValueJava(color2) - CServices.getRValueJava(color1);
                    int r = ((dl * rsOldFrame) / nbl + CServices.getRValueJava(color1)) & 0xFF;
                    dl = CServices.getGValueJava(color2) - CServices.getGValueJava(color1);
                    int g = ((dl * rsOldFrame) / nbl + CServices.getGValueJava(color1)) & 0xFF;
                    dl = CServices.getBValueJava(color2) - CServices.getBValueJava(color1);
                    int b = ((dl * rsOldFrame) / nbl + CServices.getBValueJava(color1)) & 0xFF;
                    color2 = CServices.RGBJava(r, g, b);

                    if ((adCta.odDisplayFlags & CDefCounters.BARFLAG_INVERSE) != 0)
                    {
                        dl = color1;
                        color1 = color2;
                        color2 = dl;
                    }
                }

                // Display INKEFFECT: ajouter effect flags
                cx = hoRect.right - hoRect.left;
                cy = hoRect.bottom - hoRect.top;
                x = hoRect.left;
                y = hoRect.top;
                synchronized(GLRenderer.inst)
                {
	                switch (adCta.ocFillType)
	                {
	                    case 1:			    // FILLTYPE_SOLID
	                        GLRenderer.inst.fillZone(x, y, cx, cy, color1, effect, effectParam);
	                        break;
	                    case 2:			    // FILLTYPE_GRADIENT
	                        GLRenderer.inst.renderGradient
	                               (x, y, cx, cy, color1, color2, adCta.ocGradientFlags != 0, effect, effectParam);
	                        break;
	                    default:
	                        break;
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
                    s = CServices.doubleToString(vDouble, displayFlags);
                }

                x = hoRect.left;
                y = hoRect.top;

                int i;
                short img;
                for (i = 0; i < s.length(); i++)
                {
                    char cc = s.charAt(i);
                    img = -1;
                    if (cc == '-')
                    {
                        img = adCta.frames[10];		// COUNTER_IMAGE_SIGN_NEG
                    }
                    else if (cc == '.')
                    {
                        img = adCta.frames[12];		// COUNTER_IMAGE_POINT
                    }
                    else if (cc == '+')
                    {
                        img = adCta.frames[11];	// COUNTER_IMAGE_SIGN_PLUS
                    }
                    else if (cc == 'e' || cc == 'E')
                    {
                        img = adCta.frames[13];	// COUNTER_IMAGE_EXP
                    }
                    else if (cc >= '0' && cc <= '9')
                    {
                        img = adCta.frames[cc - '0'];
                    }
                    if(img != -1)
                    {
                        hoAdRunHeader.spriteGen.pasteSpriteEffect(img, x, y, 0, effect, effectParam);
                        CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                        if(ifo != null)
                            x += ifo.getWidth();
                    }
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
                }

                // Get font
                short nFont = rsFont;
                if (nFont == -1)
                {
                    nFont = adCta.odFont;
                }
                CFont font = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(nFont);

                 short dtflags = (short) (CServices.DT_RIGHT | CServices.DT_VCENTER | CServices.DT_SINGLELINE);

                if(hoRect.bottom - hoRect.top != 0)
                {
                   textSurface.setDimension(hoImgWidth, hoImgHeight);
                   textSurface.setText(s, dtflags, rsColor1, font.getFontInfo(), false);
                   textSurface.draw(hoRect.left, hoRect.top, effect, effectParam);
                }

                break;
        }
    }

    @Override
	public CMask getCollisionMask(int flags)
    {
        return null;
    }

    /** Changes the value of the counter.
     */
    public void cpt_Change(CValue pValue)
    {
        if (pValue.getType() == CValue.TYPE_DOUBLE && rsValue.getType() == CValue.TYPE_INT)
		{
			rsValue.convertToDouble();
            display();				// This is what cpt_ToFloat was doing, probably "modified = true" would be ok, but we keep it to avoid unexpected issues
            roc.rcChanged = true;
		}

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
                modif();
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
                modif();
            }
        }
    }

    /** Adds a value to the counter.
     */
    public void cpt_Add(CValue pValue)
    {
        if (pValue.getType() == CValue.TYPE_DOUBLE && rsValue.getType() == CValue.TYPE_INT)
		{
			rsValue.convertToDouble();
            display();				// This is what cpt_ToFloat was doing, probably "modified = true" would be ok, but we keep it to avoid unexpected issues
            roc.rcChanged = true;
		}

        if (rsValue.getType() == CValue.TYPE_INT)
        {
            // Compteur entier
            int value = rsValue.getInt() + pValue.getInt();
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
                modif();
            }
        }
        else
        {
            // Compteur float
            double d = rsValue.getDouble() + pValue.getDouble();
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
                modif();
            }
        }
    }

    /** Subtract a value to the counter.
     */
    public void cpt_Sub(CValue pValue)
    {
        if (pValue.getType() == CValue.TYPE_DOUBLE && rsValue.getType() == CValue.TYPE_INT)
		{
			rsValue.convertToDouble();
            display();				// This is what cpt_ToFloat was doing, probably "modified = true" would be ok, but we keep it to avoid unexpected issues
            roc.rcChanged = true;
		}

        if (rsValue.getType() == CValue.TYPE_INT)
        {
            // Compteur entier
            int value = rsValue.getInt() - pValue.getInt();
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
                modif();
            }
        }
        else
        {
            // Compteur float
            double d = rsValue.getDouble() - pValue.getDouble();
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
                modif();
            }
        }
    }

    /** Sets the minimal value of the counter.
     */
    public void cpt_SetMin(CValue value)
    {
        rsMini = value.getInt();
        rsMiniDouble = value.getDouble();

        if (rsValue.getType() == CValue.TYPE_INT)
        {
            int i = rsValue.getInt();
            if (i < rsMini)
            {
                i = rsMini;
            }
            if (i > rsMaxi)
            {
                i = rsMaxi;
            }
            if (i != rsValue.getInt())
            {
                rsValue.forceInt(i);
                modif();
            }
        }
        else
        {
            double d = rsValue.getDouble();
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
                modif();
            }
        }
    }

    /** Sets the maximal value of the coutner.
     */
    public void cpt_SetMax(CValue value)
    {
        rsMaxi = value.getInt();
        rsMaxiDouble = value.getDouble();

        if (rsValue.getType() == CValue.TYPE_INT)
        {
            int i = rsValue.getInt();
            if (i < rsMini)
            {
                i = rsMini;
            }
            if (i > rsMaxi)
            {
                i = rsMaxi;
            }
            if (i != rsValue.getInt())
            {
                rsValue.forceInt(i);
                modif();
            }
        }
        else
        {
            double d = rsValue.getDouble();
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
                modif();
            }
        }
    }

    /** Sets the first color.
     */
    public void cpt_SetColor1(int rgb)
    {
        rsColor1 = rgb;
        display();
        roc.rcChanged = true;
    }

    /** Sets the second color.
     */
    public void cpt_SetColor2(int rgb)
    {
        rsColor2 = rgb;
        display();
        roc.rcChanged = true;
    }

    /** Returns the value.
     */
    public CValue cpt_GetValue()
    {
        return rsValue;
    }

    /** Returns the minimal value.
     */
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

    /** Returns the maximal value
     */
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

    /** Returns color 1.
     */
    public int cpt_GetColor1()
    {
        return CServices.swapRGB(rsColor1);
    }

    /** Returns color 2.
     */
    public int cpt_GetColor2()
    {
        return CServices.swapRGB(rsColor2);
    }

    /** Returns the font used by the counter.
     */
    public CFontInfo getFont()
    {
        CDefCounters adCta = hoCommon.ocCounters;
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

    /** Sets the font used by the counter.
     */
    public void setFont(CFontInfo info, CRect pRc)
    {
        CDefCounters adCta = hoCommon.ocCounters;
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

    /** Returns the color of the font.
     */
    public int getFontColor()
    {
        return rsColor1;
    }

    /** Changes the font color.
     */
    public void setFontColor(int rgb)
    {
        rsColor1 = rgb;
        modif();
        roc.rcChanged = true;
    }

    @Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
    {
        draw();
    }

    @Override
	public CMask spriteGetMask()
    {
        return null;
    }

    @Override
	public void spriteKill (CSprite spr)
    {
        spr.sprExtraInfo = null;
    }

}
