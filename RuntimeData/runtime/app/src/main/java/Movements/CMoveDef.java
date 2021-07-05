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
// CMOVEDEFLIST : liste des mouvements d'un objet'
//
//----------------------------------------------------------------------------------
package Movements;

import Services.CFile;

public abstract class CMoveDef 
{
    // Definition of movement types
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static final short MVTYPE_STATIC=0;
    public static final short MVTYPE_MOUSE=1;
    public static final short MVTYPE_RACE=2;
    public static final short MVTYPE_GENERIC=3;
    public static final short MVTYPE_BALL=4;
    public static final short MVTYPE_TAPED=5;
    public static final short MVTYPE_PLATFORM=9;
    public static final short MVTYPE_DISAPPEAR=11;
    public static final short MVTYPE_APPEAR=12;
    public static final short MVTYPE_BULLET=13;
    public static final short MVTYPE_EXT=14;

    public short mvType;
    public short mvControl;
    public byte mvMoveAtStart;
    public int mvDirAtStart;
    public byte mvOpt;
    
    public CMoveDef() 
    {
    }
    public abstract void load(CFile file, int length);
    
    /** Sets the data of this movement definition.
     */
    public void setData(short t, short c, byte m, int d, byte mo)
    {
        mvType=t;
        mvControl=c;
        mvMoveAtStart=m;
        mvDirAtStart=d;
        mvOpt=mo;
    }
}
