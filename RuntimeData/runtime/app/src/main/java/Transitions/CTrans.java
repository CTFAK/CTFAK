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
// CTRANS : interface avec un effet de transition
//
//----------------------------------------------------------------------------------
package Transitions;

import Services.CFile;
import Services.CRect;
import Services.CServices;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;

public abstract class CTrans
{
    public static final int LEFT_RIGHT = 0;
    public static final int RIGHT_LEFT = 1;
    public static final int TOP_BOTTOM = 2;
    public static final int BOTTOM_TOP = 3;
    public static final int CENTER_LEFTRIGHT = 0;
    public static final int LEFTRIGHT_CENTER = 1;
    public static final int CENTER_TOPBOTTOM = 2;
    public static final int TOPBOTTOM_CENTER = 3;
    public static final int TOP_LEFT = 0;
    public static final int TOP_RIGHT = 1;
    public static final int BOTTOM_LEFT = 2;
    public static final int BOTTOM_RIGHT = 3;
    public static final int CENTER = 4;
    public static final int DIR_HORZ = 0;
    public static final int DIR_VERT = 1;
    public static final int TRFLAG_FADEIN = 0x0001;
    public static final int TRFLAG_FADEOUT = 0x0002;
    public long m_initTime;
    public long m_currentTime;
    public long m_endTime;
    public int m_duration;
    public boolean m_overflow;
    public boolean m_running;
    public boolean m_starting;
    public Bitmap source1;
    public Bitmap source2;
    public Bitmap dest;
    public Canvas g2Dest;
    public Paint p2Dest;

    public CTrans()
    {
    }

    public void start(CTransitionData data, Bitmap display, Bitmap debut, Bitmap fin)
    {
        dest = display;
        g2Dest = new Canvas(dest);
        p2Dest=new Paint();
        source1 = debut;
        source2 = fin;

        m_initTime = System.currentTimeMillis();
        m_duration = data.transDuration;
        if (m_duration == 0)
        {
            m_duration = 1;
        }
        m_currentTime = m_initTime;
        m_endTime = m_initTime + m_duration;
        m_running = true;
        m_starting = true;
    }

    public void finish()
    {
    }

    public boolean isCompleted()
    {
        if (m_running)
        {
            return (System.currentTimeMillis() >= m_endTime);	// m_currentTime >= m_endTime;
        }
        return true;
    }

    public int getDeltaTime()
    {
        m_currentTime = System.currentTimeMillis();
        if (m_currentTime > m_endTime)
        {
            m_currentTime = m_endTime;
        }
        return (int) (m_currentTime - m_initTime);
    }

    public int getTimePos()
    {
        return (int) (m_currentTime - m_initTime);
    }

    public void setTimePos(int msTimePos)
    {
        m_initTime = (m_currentTime - msTimePos);
        m_endTime = m_initTime + m_duration;
    }

    public void blit(Bitmap source)
    {
        g2Dest.drawBitmap(source, 0, 0, p2Dest);
    }

    public void blit(Bitmap source, int xDest, int yDest, int xSrce, int ySrce, int width, int height)
    {
        if (width > 0 && height > 0)
        {
            CServices.drawRegion(g2Dest, p2Dest, source, xSrce, ySrce, width, height, xDest, yDest);
        }
    }

    public void stretch(Bitmap source, int xDest, int yDest, int wDest, int hDest, int xSrce, int ySrce, int wSrce, int hSrce)
    {
        if (wDest > 0 && hDest > 0 && wSrce > 0 && hSrce > 0)
        {
            CServices.drawRegion(g2Dest, p2Dest, source, xSrce, ySrce, wDest, hDest, xDest, yDest);
        }
    }

    public abstract void init(CTransitionData data, CFile file, Bitmap display, Bitmap source, Bitmap dest);

    public abstract CRect[] stepDraw(int flag);

    public abstract void end();
}
