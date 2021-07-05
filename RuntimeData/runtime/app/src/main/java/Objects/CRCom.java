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
// CRCOM : Structure commune aux objets animes
//
//----------------------------------------------------------------------------------
package Objects;

import Sprites.CSprite;

/** Common structure of all the objects.
 * Used to store important data for use by the engine.
 */
public class CRCom 
{
    public int rcPlayer=0;					// Player who controls
    public int rcMovementType;				// Number of the current movement
    public CSprite rcSprite=null;					// Sprite ID if defined
    public int rcAnim=0;						// Wanted animation
    public short rcImage=-1;					// Current frame
    public float rcScaleX;					
    public float rcScaleY;
    public float rcAngle;
    public int rcDir=0;						// Current direction
    public int rcSpeed=0;					// Current speed
    public int rcMinSpeed=0;					// Minimum speed
    public int rcMaxSpeed=0;					// Maximum speed
    public boolean rcChanged=false;					// Flag: modified object
    public boolean rcCheckCollides=false;			// For static objects

    public int rcOldX;            			// Previous coordinates
    public int rcOldY;
    public short rcOldImage=-1;
    public float rcOldAngle;
    public int rcOldDir;
    public int rcOldX1;					// For zone detections
    public int rcOldY1;
    public int rcOldX2;
    public int rcOldY2;

    public int rcFadeIn=0;
    public int rcFadeOut=0;
    
    public CRCom() 
    {
    }

    /** Initialisation.
     */
    public void init()
    {
        rcScaleX = 1.0f;
        rcScaleY = 1.0f;
        rcAngle=0;
        rcMovementType = -1;
    }
    
    public void kill(boolean bFast)
    {
    }
}
