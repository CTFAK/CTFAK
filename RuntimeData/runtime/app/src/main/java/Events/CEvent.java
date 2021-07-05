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
// CEVENT : une condition ou une action
//
//----------------------------------------------------------------------------------
package Events;
import Params.CParam;

public abstract class CEvent 
{
    public int evtCode;
    public short evtOi;					
    public short evtOiList;				
    public byte evtFlags;				
    public byte evtFlags2;				
    public byte evtDefType;		
    public byte evtNParams;
    public CParam evtParams[]=null;
    
    public static final byte EVFLAGS_REPEAT=0x01;
    public static final byte EVFLAGS_DONE=0x02;
    public static final byte EVFLAGS_DEFAULT=0x04;
    public static final byte EVFLAGS_DONEBEFOREFADEIN=0x08;
    public static final byte EVFLAGS_NOTDONEINSTART=0x10;
    public static final byte EVFLAGS_ALWAYS=0x20;
    public static final byte EVFLAGS_BAD=0x40;
    public static final byte EVFLAGS_BADOBJECT=(byte)0x80;
    public static final byte EVFLAGS_DEFAULTMASK=(byte)(EVFLAGS_ALWAYS+EVFLAGS_REPEAT+EVFLAGS_DEFAULT+EVFLAGS_DONEBEFOREFADEIN+EVFLAGS_NOTDONEINSTART);
    public static final byte EVFLAG2_NOT=0x01;
    
    public CEvent() 
    {
    }
    
}
