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
// -----------------------------------------------------------------------------
//
// LOAD FRAME
//
// -----------------------------------------------------------------------------
package Actions;

import Animations.CAnim;
import Animations.CAnimDir;
import Animations.CAnimHeader;
import Banks.CImageBank;
import OI.COI;
import OI.CObjectCommon;
import Objects.CActive;
import Objects.CObject;
import Params.CParamExpression;
import Params.PARAM_INT;
import Params.PARAM_SHORT;
import Params.PARAM_STRING;
import RunLoop.CRun;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

public class ACT_SPRLOADFRAME extends CAct
{
    public void execute(CRun rhPtr)
    {
        CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
        if (pHo == null)
        {
            return;
        }

        // Un cran d'animation sans effet
        pHo.roa.animIn(0);

        // Filename
        String filename;
        if (evtParams[0].code == 40)	    // PARAM_FILENAME )
        {
            filename = ((PARAM_STRING) evtParams[0]).string;
        }
        else
        {
            filename = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
        }

        // Animation
        int nAnim;
        if (evtParams[1].code == 10)	// PARAM_ANIMATION)
        {
            nAnim = ((PARAM_SHORT) evtParams[1]).value;
        }
        else
        {
            nAnim = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
        }

        // Direction
        int nDir;
        if (evtParams[2].code == 29)	    // PARAM_NEWDIRECTION)
        {
            nDir = rhPtr.get_Direction(((PARAM_INT) evtParams[2]).value);
        }
        else
        {
            nDir = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[2]);
        }
        nDir &= 31;

        // Frame
        int nFrame = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[3]);

        // X Hot Spot
        int xHS = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[4]);

        // Y Hot Spot
        int yHS = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[5]);

        // X Action Point
        int xAP = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[6]);

        // Y Action Point
        int yAP = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[7]);

        // Transparent color
/*	int trspColor;
        if (evtParams[8].code==24)	// PARAM_COLOUR)
        trspColor=((PARAM_COLOUR)evtParams[8]).color;
        else
        trspColor=rhPtr.get_EventExpressionInt((CParamExpression)evtParams[8]);
         */

        // Check parameters
        if (filename.length() == 0 || nDir < 0 || nDir >= 32 || nFrame < 0)
        {
            return;
        }

        // Load the object if necessary
        COI poi = rhPtr.rhApp.OIList.getOIFromHandle(pHo.hoOi);
		if ( (poi.oiLoadFlags & COI.OILF_ELTLOADED) == 0 )
			poi.loadOnCall(rhPtr.rhApp);

        // Point to animation
        CObjectCommon ocPtr = pHo.hoCommon;		// Calcule l'adresse de l'anim
        CAnimHeader ahPtr = ocPtr.ocAnimations;		// Pointe AnimHeader
        if (nAnim >= ahPtr.ahAnimMax)
        {
            return;
        }
        if (ahPtr.ahAnimExists[nAnim] == 0)
        {
            return;
        }
        CAnim anPtr = ahPtr.ahAnims[nAnim];

        // Point to direction
        if (anPtr.anDirs[nDir] == null)
        {
            return;
        }
        CAnimDir adPtr = anPtr.anDirs[nDir];

        // Point to frame
        if (nFrame >= adPtr.adNumberOfFrame)
        {
            return;
        }

        // Finds the old image
        short oldImage=adPtr.adFrames[nFrame];

        // OK, frame exists, open file

        Bitmap img;

        try
        {   img = BitmapFactory.decodeFile (filename);
        }
        catch (OutOfMemoryError e)
        {   return;
        }

        int dwWidth = img.getWidth();
        int dwHeight = img.getHeight();
        if (dwWidth <= 0 || dwHeight <= 0)
        {
            return;
        }

        // Create image
        if (xHS == 100000)
        {
            xHS = dwWidth / 2;
        }
        if (xHS == 110000)
        {
            xHS = dwWidth - 1;
        }
        if (yHS == 100000)
        {
            yHS = dwHeight / 2;
        }
        if (yHS == 110000)
        {
            yHS = dwHeight - 1;
        }

        if (xAP == 100000)
        {
            xAP = dwWidth / 2;
        }
        if (xAP == 110000)
        {
            xAP = dwWidth - 1;
        }
        if (yAP == 100000)
        {
            yAP = dwHeight / 2;
        }
        if (yAP == 110000)
        {
            yAP = dwHeight - 1;
        }

        // Create image
        short newImg = rhPtr.rhApp.imageBank.addImage(img, (short) xHS, (short) yHS, (short) xAP, (short) yAP, false);

        // Replace old image by new one
        adPtr.adFrames[nFrame] = newImg;

        // Mark OI to reload
        poi.oiLoadFlags |= COI.OILF_TORELOAD;

        // Changes all the objects of same OI
        short numObject=pHo.hoOiList.oilObject;
        while(numObject>=0)
        {
            CActive pHoObject=(CActive)rhPtr.rhObjectList[numObject];
            if (pHoObject.roc.rcImage==oldImage)
            {
                pHoObject.roc.rcImage=newImg;
                pHoObject.roc.rcChanged=true;
            }
            numObject=pHoObject.hoNumNext;
        }

    }
}
