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
// CHAINE NUMERO N
//
//----------------------------------------------------------------------------------
package Expressions;

import RunLoop.*;
import Objects.*;
import OI.*;

public class EXP_STRGETNUMBER extends CExpOi
{
    public void evaluate(CRun rhPtr)
    {        
	CObject pHo=rhPtr.rhEvtProg.get_ExpressionObjects(oiList);
	rhPtr.rh4CurToken++;
	if (pHo==null)
	{
	    rhPtr.rh4Results[rhPtr.rh4PosPile].forceString("");
	    return;
	}
	int num=rhPtr.get_ExpressionInt();		// Demande le numero du texte

	CText pText=(CText)pHo;
	
	// Le texte courant
	if (num<0)
	{
	    if (pText.rsTextBuffer!=null)
		rhPtr.rh4Results[rhPtr.rh4PosPile].forceString(pText.rsTextBuffer);
	    else
		rhPtr.rh4Results[rhPtr.rh4PosPile].forceString("");
	    return;
	}

	// Un texte stocke
	if (num>=pText.rsMaxi) 
	    num=pText.rsMaxi-1;
	CDefTexts txt=(CDefTexts)pHo.hoCommon.ocObject;
	rhPtr.rh4Results[rhPtr.rh4PosPile].forceString(txt.otTexts[num].tsText);
    }    
}
