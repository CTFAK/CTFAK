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
// CQuestion : Objet question
//
//----------------------------------------------------------------------------------
package Objects;

import Banks.CImageBank;
import OI.CDefText;
import OI.CDefTexts;
import OI.CObjectCommon;
import RunLoop.CCreateObjectInfo;
import Runtime.MMFRuntime;
import Sprites.CMask;
import Sprites.CSprite;
import android.app.AlertDialog;
import android.content.DialogInterface;

public class CQuestion extends CObject
{
	public int rsBoxCx;			// Dimensions box (for lives, counters, texts)
	public int rsBoxCy;			

	boolean shown;

	public CQuestion() 
	{
	}

	@Override
	public void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		shown = false;
	}

	AlertDialog alertDialog;

	@Override
	public void handle()
	{
		if (shown)
			return;

		shown = true;

		final CDefTexts texts = (CDefTexts) hoCommon.ocObject;

		AlertDialog.Builder builder = new AlertDialog.Builder (MMFRuntime.inst);

		builder.setTitle(texts.otTexts[0].tsText) ;
		builder.setCancelable(false);

		CharSequence[] options = new CharSequence[texts.otNumberOfText - 1];

		for (int i = 1; i < texts.otNumberOfText; ++ i)
			options [i - 1] = texts.otTexts [i].tsText;

		final CQuestion obj = this;

		builder.setSingleChoiceItems (options, -1, new DialogInterface.OnClickListener()
		{
			@Override
			public void onClick (DialogInterface dialog, int n)
			{
				boolean correct = (texts.otTexts [n + 1].tsFlags & CDefText.TSF_CORRECT) != 0;

				hoAdRunHeader.rhEvtProg.push_Event(1, (((-80-3)<<16)|4), n + 1, obj, (short)0);	    // CNDL_QEQUAL

				if (correct)
					hoAdRunHeader.rhEvtProg.push_Event(1, (((-80-1)<<16)|4), 0, obj, (short)0);	    // CNDL_QEXACT
				else
					hoAdRunHeader.rhEvtProg.push_Event(1, (((-80-2)<<16)|4), 0, obj, (short)0);	    // CNDL_QFALSE

				obj.done();
			}
		});


		alertDialog = builder.create ();

		alertDialog.setOnDismissListener (new DialogInterface.OnDismissListener()
		{
			@Override
			public void onDismiss (DialogInterface d)
			{
				obj.done ();
			}
		});

		alertDialog.show();

		hoAdRunHeader.pause();
	}

	public void done()
	{
		if (alertDialog != null)
		{
			alertDialog.dismiss();
			alertDialog = null;
		}

		try
		{
			hoAdRunHeader.resume();
			hoAdRunHeader.destroy_Add(hoNumber);
		}
		catch(Exception e)
		{
			/* Crashes in destroy_Add have been reported, but really the application could be in any
			 * kind of weird state at this point.  It doesn't matter if destroy_Add doesn't work because
			 * the application is already ending or something, but we'd rather not crash.
			 */
		}
	}

	@Override
	public void draw()
	{
	}

	@Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
	{
		draw();
	}

	@Override
	public void modif()
	{

	}
	@Override
	public void display()
	{

	}            
	@Override
	public void kill(boolean bFast)
	{
	}
	@Override
	public void getZoneInfos()
	{
	}
	@Override
	public CMask getCollisionMask(int flags)
	{
		return null;
	}
	@Override
	public void spriteKill(CSprite spr)
	{
	}
	@Override
	public CMask spriteGetMask()
	{
		return null;
	}
}
