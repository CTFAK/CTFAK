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
// CRECT : classe rectangle similaire a celle de windows
//
//----------------------------------------------------------------------------------
package Services;

public class CRect
{
    public int left = 0;
    public int top = 0;
    public int right = 0;
    public int bottom = 0;

    public CRect()
    {
    }

    public void load(CFile file)
    {
        left = file.readAInt();
        top = file.readAInt();
        right = file.readAInt();
        bottom = file.readAInt();
    }

    public void copyRect(CRect srce)
    {
        left = srce.left;
        right = srce.right;
        top = srce.top;
        bottom = srce.bottom;
    }

    public boolean ptInRect(int x, int y)
    {
        if (x >= left && x < right && y >= top && y < bottom)
        {
            return true;
        }
        return false;
    }

    public boolean intersectRect(CRect rc)
    {
        if ((left >= rc.left && left < rc.right) || (right >= rc.left && right < rc.right) || (rc.left >= left && rc.left < right) || (rc.right >= left && rc.right < right))
        {
            if ((top >= rc.top && top < rc.bottom) || (bottom >= rc.top && bottom < rc.bottom) || (rc.top >= top && rc.top < bottom) || (rc.bottom >= top && rc.bottom < bottom))
            {
                return true;
            }
        }
        return false;
    }

    public void inflateRect(int dx, int dy)
    {
        left -= dx;
        top -= dy;
        right += dx;
        top += dy;
    }
}
