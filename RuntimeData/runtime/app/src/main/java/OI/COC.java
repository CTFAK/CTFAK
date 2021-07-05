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
// COC: classe abstraite d'objectsCommon
//
//----------------------------------------------------------------------------------
package OI;

import Banks.IEnum;
import Services.CFile;
import Sprites.CSprite;
import Sprites.IDrawable;

public abstract class COC implements IDrawable
{
    public short ocObstacleType;		// Obstacle type
    public short ocColMode;			// Collision mode (0 = fine, 1 = box)
    public int ocCx;				// Size
    public int ocCy;
	
    public static final short OBSTACLE_NONE=0;
    public static final short OBSTACLE_SOLID=1;
    public static final short OBSTACLE_PLATFORM=2;
    public static final short OBSTACLE_LADDER=3;
    public static final short OBSTACLE_TRANSPARENT=4;
    
    public COC() 
    {
    }

    abstract void load(CFile file, short type, COI pOi);
    abstract void enumElements(IEnum enumImages, IEnum enumFonts);

    @Override
	public void spriteKill (CSprite spr)
    {
    }
    
}
