//----------------------------------------------------------------------------------
//
// CQuestion : Objet question
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RuntimeXNA.Objects
{
	
	public class CQuestion:CObject
	{
		int rsBoxCx; // Dimensions box (for lives, counters, texts)
		int rsBoxCy;
		CRect[] rcA=null;
		int currentDown;
        int xMouse;
        int yMouse;
		
		public CQuestion()
		{
		}
		
		public override void  init(CObjectCommon ocPtr, CCreateObjectInfo cob)
		{
		}

        public override void  handle()
		{
			hoAdRunHeader.pause();
            hoAdRunHeader.questionObjectOn = this;
		}
		
	    public void handleQuestion()
	    {
	    	int current;
            xMouse = hoAdRunHeader.rh2MouseX;
            yMouse = hoAdRunHeader.rh2MouseX;

	    	if (currentDown==0)
	    	{
	    		if ((hoAdRunHeader.rh2MouseKeys&0x01)!=0)
	    		{
	    			current=getQuestion();
	    			if (current!=0)
	    			{
	    				currentDown=current;
	    			}
	    		}
	    	}
	    	else
	    	{
	    		if ((hoAdRunHeader.rh2MouseKeys&0x01)==0)
	    		{
    				if (getQuestion()==currentDown)
    				{
                        hoAdRunHeader.rhEvtProg.rhCurParam0 = currentDown;
                        hoAdRunHeader.rhEvtProg.handle_Event(this, (((-80 - 3) << 16) | 4));

                        CDefTexts defTexts = (CDefTexts)hoCommon.ocObject;
                        CDefText ptts = defTexts.otTexts[currentDown];
                        bool bCorrect = (ptts.tsFlags & CDefText.TSF_CORRECT) != 0;
			            if (bCorrect)
			            {
                            hoAdRunHeader.rhEvtProg.handle_Event(this, (((-80 - 1) << 16) | 4));
			            }
			            else
			            {
                            hoAdRunHeader.rhEvtProg.handle_Event(this, (((-80 - 2) << 16) | 4));
			            }
				        hoAdRunHeader.questionObjectOn=null;
				        hoAdRunHeader.resume();
				        hoAdRunHeader.f_KillObject(hoNumber, true);
				        return;
    				}
	    			currentDown=0;
	    		}
	    	}
	    }
	    public int getQuestion()
	    {
	    	int i;
            if (rcA != null)
            {
                for (i = 1; i < rcA.Length; i++)
                {
                    if (xMouse >= rcA[i].left && xMouse < rcA[i].right)
                    {
                        if (yMouse > rcA[i].top && yMouse < rcA[i].bottom)
                        {
                            return i;
                        }
                    }
                }
            }
	    	return 0;
	    }

		//-----------------------------------------//
		//		Affichage d'un bord style bouton	//
		//-----------------------------------------//
		public virtual void border3D(SpriteBatchEffect batch, CRect rc, bool state)
		{
			int color1, color2;
			
			if (state)
			{
				color1 = CServices.RGBJava(128, 128, 128);
				color2 = CServices.RGBJava(255, 255, 255);
			}
			else
			{
				color2 = CServices.RGBJava(128, 128, 128);
				color1 = CServices.RGBJava(255, 255, 255);
			}
			
			// Cadre noir
            hoAdRunHeader.rhApp.services.drawRect(batch, rc, 0x000000, CSpriteGen.BOP_COPY, 0);
			
			// Reflet blanc (ou gris si enfonce)
			CPoint[] pt = new CPoint[3];
			int n;
			for (n = 0; n < 3; n++)
			{
				pt[n] = new CPoint();
			}
			pt[0].x = rc.right - 1;
			if (state == false)
				pt[0].x -= 1;
			pt[0].y = rc.top + 1;
			pt[1].y = rc.top + 1;
			pt[1].x = rc.left + 1;
			pt[2].x = rc.left + 1;
			pt[2].y = rc.bottom;
			if (state == false)
				pt[2].y -= 1;
            hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[0].x, pt[0].y, pt[1].x, pt[1].y, color1, 1, CSpriteGen.BOP_COPY, 0);
            hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[1].x, pt[1].y, pt[2].x, pt[2].y, color1, 1, CSpriteGen.BOP_COPY, 0);
			
			if (state == false)
				pt[0].x -= 1;
			pt[0].y += 1;
			pt[1].x += 1;
			pt[1].y += 1;
			pt[2].x += 1;
			if (state == false)
				pt[2].y -= 1;
            hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[0].x, pt[0].y, pt[1].x, pt[1].y, color1, 1, CSpriteGen.BOP_COPY, 0);
            hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[1].x, pt[1].y, pt[2].x, pt[2].y, color1, 1, CSpriteGen.BOP_COPY, 0);
			
			// Reflet gris fonce
			if (state == false)
			{
				pt[0].x += 2;
				pt[1].x = rc.right - 1;
				pt[1].y = rc.bottom - 1;
				pt[2].y = rc.bottom - 1;
				pt[2].x -= 1;
                hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[0].x, pt[0].y, pt[1].x, pt[1].y, color2, 1, CSpriteGen.BOP_COPY, 0);
                hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[1].x, pt[1].y, pt[2].x, pt[2].y, color2, 1, CSpriteGen.BOP_COPY, 0);
				
				pt[0].x -= 1;
				pt[0].y += 1;
				pt[1].x -= 1;
				pt[1].y -= 1;
				pt[2].x += 1;
				pt[2].y -= 1;
                hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[0].x, pt[0].y, pt[1].x, pt[1].y, color2, 1, CSpriteGen.BOP_COPY, 0);
                hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, pt[1].x, pt[1].y, pt[2].x, pt[2].y, color2, 1, CSpriteGen.BOP_COPY, 0);
            }
		}
		
		//--------------------------------------------------//
		//	Dessiner un bouton reponse d'un objet question	//
		//--------------------------------------------------//
		public void redraw_Answer(SpriteBatchEffect batch, CDefText ptts, CRect lpRc, int color, bool flgRelief, CFont font, bool state)
		{
			CRect rc = new CRect();
			
			rc.copyRect(lpRc);
			border3D(batch, lpRc, state); // Cadre 3D
			rc.left += 2;
			rc.top += 2;
			rc.right -= 4;
			rc.bottom -= 4;
			if (state)
			{
				rc.left += 2;
				rc.top += 2;
			}
			if (flgRelief)
			{
				rc.left += 2;
				rc.top += 2;
				CServices.drawText(batch, ptts.tsText, (short) (CServices.DT_SINGLELINE | CServices.DT_CENTER | CServices.DT_VCENTER), rc, 0xFFFFFF, font, 0, 0);
				rc.left -= 2;
				rc.top -= 2;
			}
			CServices.drawText(batch, ptts.tsText, (short) (CServices.DT_SINGLELINE | CServices.DT_CENTER | CServices.DT_VCENTER), rc, color, font, 0, 0);
		}
		
		public override void draw(SpriteBatchEffect batch)
		{
            // Pointer sur le oc
            CDefTexts defTexts = (CDefTexts)hoCommon.ocObject;

            // Afficher la boite
            CRun prh = hoAdRunHeader;

            // Calcul dimensions boite
            int x = hoX - prh.rhWindowX; // rc.left = 
            int y = hoY - prh.rhWindowY; // rc.top = 

            // Calcul hauteur rectangle reponses
            CDefText ptta = defTexts.otTexts[1];
            int colorA = ptta.tsColor;
            bool flgRelief = (ptta.tsFlags & CDefText.TSF_RELIEF) != 0;
            CFont fontAnswers = prh.rhApp.fontBank.getFontFromHandle(ptta.tsFont);
            CRect rc = new CRect();
            rc.right = 2000;
            CServices.drawText(null, "X", CServices.DT_CALCRECT, rc, colorA, fontAnswers, 0, 0);
            int xa_margin = rc.right * 3 / 2; // Marge = "XX"
            int hta = 4;
            int lgBox = 64;
            int i;
            for (i = 1; i < defTexts.otTexts.Length; i++)
            {
                ptta = defTexts.otTexts[i];
                if (ptta.tsText.Length > 0)
                {
                    rc.right = 2000;
                    rc.bottom = 0;
                    CServices.drawText(null, ptta.tsText, CServices.DT_CALCRECT, rc, colorA, fontAnswers, 0, 0);
                    lgBox = System.Math.Max(lgBox, rc.right + xa_margin * 2 + 4);
                    hta = System.Math.Max(hta, rc.bottom * 3 / 2); // Hauteur bouton reponse
                }
            }
            int hte = System.Math.Max(hta / 4, 2);
            lgBox += xa_margin * 2 + 4; // Ajouter marge en dehors boutons

            // Calcul hauteur rectangle question
            CDefText ptts = defTexts.otTexts[0];
            CFont fontQuestion = prh.rhApp.fontBank.getFontFromHandle(ptts.tsFont);
            rc.right = 2000;
            rc.bottom = 0;
            CServices.drawText(null, "X", CServices.DT_CALCRECT, rc, colorA, fontQuestion, 0, 0);
            int xq_margin = rc.right * 3 / 2; // Marge = "XX"
            rc.right = 2000;
            rc.bottom = 0;
            CServices.drawText(null, ptts.tsText, CServices.DT_CALCRECT, rc, colorA, fontQuestion, 0, 0);
            int htq = rc.bottom * 3 / 2; // hauteur boite question
            lgBox = System.Math.Max(lgBox, rc.right + xq_margin * 2 + 4);

            if (lgBox > prh.rhApp.gaCxWin)
            {
                x += (lgBox - prh.rhApp.gaCxWin) / 2;
                lgBox = prh.rhApp.gaCxWin;
            }
            else if (lgBox > prh.rhFrame.leWidth)
            {
                x += (lgBox - prh.rhFrame.leWidth) / 2;
                lgBox = prh.rhFrame.leWidth;
            }

            short flgCenterQ = CServices.DT_CENTER;
            if (rc.right + xq_margin * 2 + 4 > System.Math.Min((int)prh.rhApp.gaCxWin, (int)prh.rhFrame.leWidth))
                flgCenterQ = CServices.DT_LEFT;

            // Sauvegarder le rectangle
            CRect rcQ = new CRect();
            rcQ.left = x;
            rcQ.top = y;
            rsBoxCx = lgBox;
            rsBoxCy = htq + 1 + (hta + hte) * (defTexts.otTexts.Length - 1) + hte + 4;
            rcQ.right = x + rsBoxCx;
            rcQ.bottom = y + rsBoxCy;

            // Afficher le fond du rectangle
            prh.rhApp.services.fillRect(batch, rcQ, 0xC0C0C0, CSpriteGen.BOP_COPY, 0);
            border3D(batch, rcQ, false); // Cadre 3D

            // Afficher la question elle-meme
            rcQ.left += 2;
            rcQ.top += 2;
            rcQ.right -= 2;
            rcQ.bottom = rcQ.top + htq;
            if ((ptts.tsFlags & CDefText.TSF_RELIEF) != 0)
            {
                rcQ.left += 2;
                rcQ.top += 2;
                CServices.drawText(batch, ptts.tsText, (short)(CServices.DT_SINGLELINE | flgCenterQ | CServices.DT_VCENTER), rcQ, 0xFFFFFF, fontQuestion, 0, 0);
                rcQ.left -= 2;
                rcQ.top -= 2;
            }
            CServices.drawText(batch, ptts.tsText, (short)(CServices.DT_SINGLELINE | flgCenterQ | CServices.DT_VCENTER), rcQ, ptts.tsColor, fontQuestion, 0, 0);
            rcQ.top = rcQ.bottom;
            prh.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, rcQ.left, rcQ.top, rcQ.right, rcQ.bottom, 0x808080, 1, CSpriteGen.BOP_COPY, 0);
            rcQ.top += 1;
            rcQ.bottom += 1;
            prh.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, rcQ.left, rcQ.top, rcQ.right, rcQ.bottom, 0xFFFFFF, 1, CSpriteGen.BOP_COPY, 0);

            // Afficher les reponses
            if (rcA == null)
            {
                rcA = new CRect[defTexts.otTexts.Length];
                for (i = 1; i < defTexts.otTexts.Length; i++)
                {
                    rcA[i] = new CRect();
                    rcA[i].left = x + 2 + xa_margin;
                    rcA[i].right = x + lgBox - 2 - xa_margin; // lpRc->left + lgBox + 2;
                    rcA[i].top = y + 2 + htq + 1 + hte + (hta + hte) * (i - 1);
                    rcA[i].bottom = rcA[i].top + hta;
                }
            }
            for (i = 1; i < defTexts.otTexts.Length; i++)
            {
                ptts = defTexts.otTexts[i];
                bool bFlag = currentDown == i;
                redraw_Answer(batch, ptts, rcA[i], colorA, flgRelief, fontAnswers, bFlag);
            }
        }
	}
}