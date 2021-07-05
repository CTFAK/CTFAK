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

package OpenGL;


import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.text.Layout.Alignment;
import android.text.StaticLayout;
import android.text.TextPaint;
import android.text.TextUtils.TruncateAt;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.widget.TextView;

import Application.CRunApp;
import Banks.CImage;
import Runtime.Log;
import Runtime.MMFRuntime;
import Runtime.SurfaceView;
import Services.CFontInfo;
import Services.CRect;
import Services.CServices;


public class CTextSurface
{
	CRunApp app;

	String prevText;
	short prevFlags;
	int prevColor;
	CFontInfo prevFont;

	public float txtAngle;
	public float txtScaleX;
	public float txtScaleY;
	public int txtSpotX;
	public int txtSpotY;

	public int width;
	public int height;
	public int density;

	public int Imgwidth;
	public int Imgheight;

	//private boolean bAntialias;

	int effect;
	int effectParam;

	int drawOffset;

	public Bitmap textBitmap;
	public Canvas textCanvas;
	public TextPaint textPaint;

	class CTextTexture extends CImage
	{
		public CTextTexture ()
		{
			if(textBitmap != null) {
				int [] pixels = new int [textBitmap.getWidth () * textBitmap.getHeight ()];
				textBitmap.getPixels (pixels, 0, textBitmap.getWidth (), 0, 0, textBitmap.getWidth (), textBitmap.getHeight ());
				allocNative2((MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0, (short) -1, pixels, 0, 0, 0,
						0, textBitmap.getWidth(), textBitmap.getHeight(), SurfaceView.ES);
			}
		}

		@Override
		public void onDestroy ()
		{
			textTexture = null;
		}

	}

	CTextTexture textTexture;

	private void createTextBitmap(int bmpWidth, int bmpHeight)
	{
		recycle();
		if(CServices.checkFitsInMemoryAndCollect(bmpWidth * bmpHeight * density)) {
			textBitmap = Bitmap.createBitmap(bmpWidth, bmpHeight, Bitmap.Config.ARGB_8888);
			textBitmap.eraseColor(Color.TRANSPARENT);

			textCanvas = new Canvas(textBitmap);
		}
	}

	public CTextSurface(CRunApp app, int width, int height)
	{
		this.app = app;
		density = (int)CServices.deviceDensity();

		this.Imgwidth = this.width = width;
		this.Imgheight = this.height = height;

		textPaint = new TextPaint();

		Log.Log(String.format("Create Text Bitmap Width: %d, Heght: %d", width, height));
		createTextBitmap(width, height);

		prevText = "";
		prevFlags = 0;
		prevFont = null;
		prevColor = 0;


		//TextXtras
		txtAngle = 0;
		txtScaleX= 1.0f;
		txtScaleY= 1.0f;
		txtSpotX = 0;
		txtSpotY = 0;
	}

	public void resize(int width, int height, boolean backingOnly) {
		if (!backingOnly) {
			this.width = width;
			this.height = height;
		}

		this.Imgwidth = width;
		this.Imgheight = height;

		if (textBitmap != null && textBitmap.getWidth() >= width && textBitmap.getHeight() >= height)
			return;

		try {
			Log.Log(String.format("Resize Text Bitmap Width: %d, Heght: %d", width, height));
			createTextBitmap(width, height);
		} catch (OutOfMemoryError e) {
			Log.Log("Text too big to create ...");
			textCanvas = null;
			textBitmap = null;
		}

	}

	public void measureText (String s, short flags, CFontInfo font, CRect rect, int newWidth)
	{
		if(textPaint == null )
			return;

		textPaint.setTypeface(font.createFont());
		textPaint.setTextSize(font.lfHeight);
		if(font.lfUnderline != 0)
			textPaint.setUnderlineText(true);
		else
			textPaint.setUnderlineText(false);

		Paint.FontMetrics fm = textPaint.getFontMetrics();
		int paintOffset = -(int)(fm.descent/MMFRuntime.inst.getResources().getDisplayMetrics().density);
		paintOffset = paintOffset > 0 ? paintOffset :0;

		int lWidth = newWidth;
		if ( newWidth == 0 )
			lWidth = width;

		Alignment alignment = CServices.textAlignment(flags, CServices.containsRtlChars(s));
		StaticLayout layout = CServices.FSStaticLayout
				(s, textPaint, lWidth, alignment, 1.0f, 0.0f, false);

		int height = layout.getHeight ()+paintOffset;
		if ((rect.top + height) <= rect.bottom)
			height = rect.bottom - rect.top;

		rect.bottom = rect.top + height;
		rect.right = rect.left + lWidth;
	}

	public void setDimension(int width, int height) {
		Imgwidth  = width;
		Imgheight = height;
	}

	public boolean setText(String s, short flags, int color, CFontInfo font, boolean dynamic)
	{
		if(s.equals(prevText) && color == prevColor && flags == prevFlags && font.equals(prevFont) )
			return false;

		if(textBitmap == null) {		//Change for performance.
			createTextBitmap(width, height);
			if (textBitmap == null)
				return false;
		}

		textBitmap.eraseColor(color & 0x00FFFFFF);

		prevFont = font;
		prevText = s;
		prevColor = color;
		prevFlags = flags;

		CRect rect = new CRect ();

		rect.left = 0;

		if(Imgwidth != width)
			rect.right = Imgwidth;
		else
			rect.right = width;

		rect.top = 0;

		if(Imgheight != height)
			rect.bottom = Imgheight;
		else
			rect.bottom = height;

		manualDrawText(s, flags, rect, color, font, dynamic);
		updateTexture();
		return true;
	}

	public void manualDrawText(String s, short flags, CRect rect, int color, CFontInfo font, boolean dynamic)
	{
		if(textBitmap == null) {
			createTextBitmap(width, height);
			if (textBitmap == null)
				return;
		}

		int rectWidth = rect.right - rect.left;
		int rectHeight = rect.bottom - rect.top;

		Alignment alignment = CServices.textAlignment(flags, CServices.containsRtlChars(s));

		textPaint.setAntiAlias(true);
		textPaint.setColor(0xFF000000|color);
		textPaint.setTypeface(font.createFont());
		textPaint.setTextSize(font.lfHeight);
		textPaint.setFilterBitmap(true);
		if(font.lfUnderline != 0)
			textPaint.setUnderlineText(true);
		else
			textPaint.setUnderlineText(false);

		StaticLayout layout = CServices.FSStaticLayout
				(s, textPaint, rectWidth, alignment, 1.0f, 0.0f, false);

		Paint.FontMetrics fm = textPaint.getFontMetrics();
		int paintOffset = (int)(fm.descent/MMFRuntime.inst.getResources().getDisplayMetrics().density);
		paintOffset = paintOffset < 0 ? paintOffset :0;

		if (dynamic && (height < layout.getHeight () || width < layout.getWidth()))
		{
			resize (rectWidth, layout.getHeight (), true);

			layout.draw (textCanvas);

			if ((flags & CServices.DT_BOTTOM) != 0)
				drawOffset = paintOffset - (layout.getHeight () - rectHeight);
			else if ((flags & CServices.DT_VCENTER) != 0)
				drawOffset = paintOffset - ((layout.getHeight () - rectHeight) / 2);

			Imgheight = layout.getHeight();
			Imgwidth = rectWidth;
		}
		else
		{

			textCanvas.save();

			if ((flags & CServices.DT_BOTTOM) != 0)
				textCanvas.translate (rect.left, rect.bottom - layout.getHeight() + paintOffset);
			else if ((flags & CServices.DT_VCENTER) != 0)
				textCanvas.translate (rect.left, rect.top + rectHeight / 2 - layout.getHeight() / 2 + paintOffset/2);
			else
				textCanvas.translate (rect.left, rect.top);

			textCanvas.clipRect (0, 0, Imgwidth, Imgheight);

			layout.draw (textCanvas);

			textCanvas.restore();
		}
	}

	public void manualDrawTextEllipsis(String s, short flags, CRect rect, int color, CFontInfo font, boolean dynamic, int ellipsis_mode)
	{
		if(textCanvas == null) {
			textCanvas = new Canvas();
			if (textCanvas == null)
				return;
		}

		int rectWidth = rect.right - rect.left;
		int rectHeight = rect.bottom - rect.top;

		Alignment alignment = CServices.textAlignment(flags, CServices.containsRtlChars(s));

		TextView view = new TextView(MMFRuntime.inst);
		if(view == null)
			return;
		view.setText(s);

		int widthSpec = View.MeasureSpec.makeMeasureSpec(rectWidth, View.MeasureSpec.EXACTLY);
		int heightSpec = View.MeasureSpec.makeMeasureSpec(rectHeight, View.MeasureSpec.EXACTLY);
		view.measure(widthSpec, heightSpec);

		view.layout(0, 0, view.getMeasuredWidth(), view.getMeasuredHeight());

		view.setBackgroundColor(Color.TRANSPARENT);
		view.setTextColor(0xFF000000|color);
		view.setTypeface(font.createFont());
		view.setTextSize(TypedValue.COMPLEX_UNIT_PX, font.lfHeight);

		if (ellipsis_mode == 0)
		{
			view.setEllipsize(TruncateAt.START);
		}
		else if (ellipsis_mode == 1)
		{
			view.setEllipsize(TruncateAt.END);
		}
		else if (ellipsis_mode == 2)
		{
			view.setEllipsize(TruncateAt.MIDDLE);
		}


		int gravity = 0;

		if ((flags & CServices.DT_CENTER) != 0)
			gravity |= Gravity.CENTER_HORIZONTAL;
		else {
			if(!CServices.containsRtlChars(s))
			{
				if ((flags & CServices.DT_RIGHT) != 0)
					gravity |= Gravity.RIGHT;
				else
					gravity |= Gravity.LEFT;
			}
			else
			{
				if ((flags & CServices.DT_RIGHT) != 0)
					gravity |= Gravity.LEFT;

				else
					gravity |= Gravity.RIGHT;
			}
		}


		if ((flags & CServices.DT_BOTTOM) != 0)
			gravity |= Gravity.BOTTOM;
		else if ((flags & CServices.DT_VCENTER) != 0)
			gravity |= Gravity.CENTER_VERTICAL;
		else
			gravity |= Gravity.TOP;

		view.setGravity(gravity);
		if(MMFRuntime.deviceApi > 16)
			view.setTextAlignment(View.TEXT_ALIGNMENT_GRAVITY);
		else
			view.setGravity(gravity | Gravity.START);
		//Log.Log("Gravity is: "+gravity);

		if((flags & CServices.DT_SINGLELINE) == 0)
			view.setSingleLine(false);
		else
		{
			view.setLines(1);
			view.setMaxLines(1);
		}

		//view.setDrawingCacheEnabled(true);
		view.forceLayout();

		//Translate the Canvas into position and draw it
		textCanvas.save();

		int left = rect.left;

		if ((flags & CServices.DT_BOTTOM) != 0)
			textCanvas.translate (left, rect.bottom - view.getHeight());
		else if ((flags & CServices.DT_VCENTER) != 0)
			textCanvas.translate (left, rect.top + rectHeight / 2 - view.getHeight() / 2);
		else
			textCanvas.translate (left, rect.top);

		textCanvas.clipRect (0, 0, Imgwidth, Imgheight);

		view.draw(textCanvas);
		textCanvas.restore();

		//view.destroyDrawingCache();
	}

	public void updateTexture()
	{
		if(textTexture != null)
			textTexture.updateWith(textBitmap);
		else {
			textTexture = new CTextTexture();
		}
		if(textTexture != null)
		{
			textTexture.setXSpot(txtSpotX);
			textTexture.setYSpot(txtSpotY);
		}
	}

	public void manualClear(int color)
	{
		if(textBitmap != null)
			textBitmap.eraseColor(color & 0x00FFFFFF);
	}


	public void draw(int x, int y, int effect, int effectParam)
	{
		if (textTexture == null)
			updateTexture();

		// Not need of setAntialias it is done when surface is created ???
		textTexture.setResampling((MMFRuntime.inst.app.hdr2Options & CRunApp.AH2OPT_ANTIALIASED) != 0);

		//GLRenderer.inst.renderImage(textTexture, x, y + drawOffset, -1, -1, effect, effectParam);
		int bHotSpot = 0;
		if (textTexture.getXSpot() != 0 || textTexture.getYSpot() != 0 )
			bHotSpot = 1;
		GLRenderer.inst.renderScaledRotatedImage2(textTexture, txtAngle, txtScaleX, txtScaleY, bHotSpot, x+txtSpotX, y + drawOffset+txtSpotY, effect, effectParam);

	}

	public void recycle ()
	{
		if (textBitmap != null)
		{
			textCanvas = null;
			textBitmap.recycle();
			textBitmap = null;
		}

		if (textTexture != null)
			textTexture.destroy (); /* will also set textTexture to null */
	}

	public int getWidth() {
		return this.Imgwidth;
	}

	public int getHeight() {
		return this.Imgheight;
	}

	public void setAngle(int angle)
	{
		txtAngle = (float)(angle%360);
	}

	public void setScaleX(int scalex)
	{
		txtScaleX = (float)(scalex)/100.0f;
	}

	public void setScaleY(int scaley)
	{
		txtScaleY = (float)(scaley)/100.0f;
	}

	public void setHotSpot(int spotX, int spotY)
	{
		txtSpotX = spotX;
		txtSpotY = spotY;
		if(textTexture != null)
		{
			textTexture.setXSpot(txtSpotX);
			textTexture.setYSpot(txtSpotY);
		}
	}
}
