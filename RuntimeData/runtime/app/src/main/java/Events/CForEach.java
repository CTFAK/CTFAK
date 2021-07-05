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
// ------------------------------------------------------------------------------
//
// CForEach : info about the current loop
//
// ------------------------------------------------------------------------------
package Events;

import Objects.CObject;

public class CForEach
{
    private static int STEP = 16;
    public CForEach next = null;
    public int oi;
    public int number = 0;
    public int length = STEP;
    public CObject objects[] = new CObject[STEP];
    public int index;
    public String name;
    public Boolean stop;

    public void addObject(CObject pHo)
    {
        if (number >= length)
        {
            CObject temp[] = new CObject[length + STEP];
            int n;
            for (n = 0; n < length; n++)
                temp[n] = objects[n];
            objects = temp;
            length += STEP;
        }
        objects[number++] = pHo;
    }
}
