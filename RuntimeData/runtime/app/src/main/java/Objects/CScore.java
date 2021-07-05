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
// CScore : Objet score
//
//----------------------------------------------------------------------------------
package Objects;

import Application.CRunApp;
import Banks.CFont;
import Banks.CImage;
import Banks.CImageBank;
import Expressions.CValue;
import OI.CDefCounters;
import OI.CObjectCommon;
import OpenGL.CTextSurface;
import RunLoop.CCreateObjectInfo;
import RunLoop.CObjInfo;
import Runtime.MMFRuntime;
import Services.CFontInfo;
import Services.CRect;
import Services.CServices;
import Sprites.CMask;
import Sprites.CSprite;

/** The score object.
 * Very similar to the counter and lives object.
 */
public class CScore extends CObject
{
    public short rsPlayer;
    public CValue rsValue;
    public int rsBoxCx;			// Dimensions box (for lives, counters, texts)
    public int rsBoxCy;
    public short rsFont;				// Temporary font for texts
    public int rsColor1;			// Bar color
    public int displayFlags;
    public CTextSurface textSurface;

	public CScore()
    {
    }

    @Override
	public void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
    {
        rsFont = -1;
        rsColor1 = 0;
        hoImgWidth = hoImgHeight = 1;		// 0

        CDefCounters adCta = hoCommon.ocCounters;
        hoImgWidth = rsBoxCx = adCta.odCx;
        hoImgHeight = rsBoxCy = adCta.odCy;
        rsColor1 = adCta.ocColor1;
        rsPlayer = adCta.odPlayer;
        displayFlags = adCta.odDisplayFlags;
        int scores[] = hoAdRunHeader.rhApp.getScores();
        rsValue = new CValue(scores[rsPlayer - 1]);

	    textSurface = new CTextSurface(hoAdRunHeader.rhApp, hoImgWidth, hoImgHeight);
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

        int vInt = rsValue.getInt();

        short img;
        String s = CServices.intToString(vInt, displayFlags);
        switch (adCta.odDisplayType)
        {
            case 1:	    // CTA_DIGITS:
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
                    CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                    if(ifo != null) {
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
        }
    }

    @Override
	public void draw()
    {
        if (hoCommon.ocCounters == null)
        {
            return;
        }
        CDefCounters adCta = hoCommon.ocCounters;
        int effect = ros.rsEffect;
        int effectParam = ros.rsEffectParam;
        int vInt = rsValue.getInt();

        String s = CServices.intToString(vInt, displayFlags);
        int x, y;
        switch (adCta.odDisplayType)
        {
            case 1:	    // CTA_DIGITS:
                x = hoRect.left;
                y = hoRect.top;

                int i;
                short img;
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
                    hoAdRunHeader.spriteGen.pasteSpriteEffect(img, x, y, 0, effect, effectParam);
                    CImage ifo = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(img);
                    x += ifo.getWidth();
                }
                break;

            case 5:	// CTA_TEXT:
                // Get font
                short nFont = rsFont;
                if (nFont == -1)
                {
                    nFont = adCta.odFont;
                }
                CFont font = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(nFont);

                short dtflags = (short) (CServices.DT_RIGHT | CServices.DT_VCENTER | CServices.DT_SINGLELINE);
                if (hoRect.bottom - hoRect.top != 0)
                {
                    textSurface.setText(s, dtflags, rsColor1, font.getFontInfo(), true);
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
