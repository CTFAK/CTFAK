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
// CPosOnLoop : position of the onloop of one loop in the program
//
//----------------------------------------------------------------------------------
package Events;


public class CPosOnLoop
{
    public String m_name;
    public CEventGroup m_pointers[];
    public boolean m_bOR = false;
    private int m_length;
    private int m_position;
    private int STEP = 4;

    public CPosOnLoop()
    {
    }
    public CPosOnLoop(String name)
    {
        m_name = name;
        m_length = 1;
        m_position = 0;
        m_pointers = new CEventGroup[m_length + 1];
    }
    public void addOnLoop(CEventGroup evgPtr)
    {
        if (m_position >= m_length)
        {
            m_length += STEP;
            CEventGroup temp[] = new CEventGroup[m_length + 1];
            int n;
            for (n=0; n < m_position + 1; n++)
                temp[n] = m_pointers[n];
            m_pointers = temp;
        }
        m_pointers[m_position++] = evgPtr;
        m_pointers[m_position] = null;
    }
}
