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
// CSERVICES : Routines utiles diverses
//
//----------------------------------------------------------------------------------
package Services;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FilenameFilter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.RandomAccessFile;
import java.nio.charset.Charset;
import java.text.NumberFormat;
import java.text.ParseException;
import java.util.Formatter;
import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import Runtime.Log;
import Runtime.MMFRuntime;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.Instrumentation;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Rect;
import android.graphics.Typeface;
import android.hardware.SensorEvent;
import android.net.Uri;
import android.os.Debug;
import android.provider.Settings;
import android.support.annotation.RequiresApi;
import android.text.Layout;
import android.text.StaticLayout;
import android.text.TextPaint;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.Display;
import android.view.Surface;
import android.widget.TextView;


public class CServices
{
    public static short DT_LEFT = 0x0000;
    public static short DT_TOP = 0x0000;
    public static short DT_CENTER = 0x0001;
    public static short DT_RIGHT = 0x0002;
    public static short DT_BOTTOM = 0x0008;
    public static short DT_VCENTER = 0x0004;
    public static short DT_SINGLELINE = 0x0020;
    public static short DT_CALCRECT = 0x0400;
    public static short DT_VALIGN = 0x0800;
    public static short CPTDISPFLAG_INTNDIGITS = 0x000F;
    public static short CPTDISPFLAG_FLOATNDIGITS = 0x00F0;
    public static short CPTDISPFLAG_FLOATNDIGITS_SHIFT = 4;
    public static int   CPTDISPFLAG_FLOATNDECIMALS = 0xF000;
    public static short CPTDISPFLAG_FLOATNDECIMALS_SHIFT = 12;
    public static short CPTDISPFLAG_FLOAT_FORMAT = 0x0200;
    public static short CPTDISPFLAG_FLOAT_USENDECIMALS = 0x0400;
    public static short CPTDISPFLAG_FLOAT_PADD = 0x0800;

    
    public static Layout.Alignment textAlignment (int flags, boolean RTL)
    {
        if ((flags & DT_CENTER) != 0)
            return Layout.Alignment.ALIGN_CENTER;

        else if(!RTL)
        {
	        if ((flags & DT_RIGHT) != 0)
	            return Layout.Alignment.ALIGN_OPPOSITE;
	
	        return Layout.Alignment.ALIGN_NORMAL;
        }
        else
        {
	        if ((flags & DT_RIGHT) != 0)
		        return Layout.Alignment.ALIGN_NORMAL;
	
            return Layout.Alignment.ALIGN_OPPOSITE;
        }
    }

    public static boolean containsRtlChars(String text) 
    {
        for (int i = 0; i < text.length(); i++)
        {
            int c = text.codePointAt(i);
            int direction = Character.getDirectionality(c);
            if ((direction == Character.DIRECTIONALITY_RIGHT_TO_LEFT) 
            		|| (direction == Character.DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC) 
            			|| (direction == Character.DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING) 
            				|| (direction == Character.DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE))
                return true;
        }
        return false;
    }

    /* Very primitive conversion of windows-style wildcard to
     * a regex pattern, used by getFiles.
     *
     * TODO: This should cover most simple cases, but something
     * more robust should be used
     */
    static String wildcardToRegex(String wildcard)
    {
        if(wildcard.equals("*.*"))
            return "";

        if(wildcard.contains("*."))
            return wildcard.toLowerCase()
            				.replace("*.", ".");
        
        return wildcard.toLowerCase()
                       .replace(".", "\\.")
                       .replace("*", "[a-z0-9]*");
    }
    
    public static File[] getFiles(String pathWithPattern)
    {
        pathWithPattern = pathWithPattern.replace('\\', '/');

        int lastSlash = pathWithPattern.lastIndexOf('/');

        String path;

        if(lastSlash == -1)
            path = pathWithPattern;
        else
            path = pathWithPattern.substring(0, lastSlash + 1);

        final String pattern = wildcardToRegex
            ((lastSlash != -1 && lastSlash < pathWithPattern.length() - 1)
                ? pathWithPattern.substring(lastSlash + 1).toLowerCase()
                : "");

        File directory = new File(path);

        return directory.listFiles(new FilenameFilter()
        {
        	@Override
            public boolean accept(File dir, String name)
            {
				String lowercaseName = name.toLowerCase();
				
                if(pattern.equals(""))
                    return true;
                
                if(lowercaseName.contains(pattern))
                	return true;
                
                return lowercaseName.matches(pattern);
            }
        });
    }

    public static void filterAccelerometer(SensorEvent e, double[] direct, double[] filtered, double[] instant, float nominalG)
    {
    	//Log.v("Accel", " [0]="+e.values[0] / nominalG);
    	//Log.v("Accel", " [1]="+e.values[1] / nominalG);
    	//Log.v("Accel", " [2]="+e.values[2] / nominalG);
        // Information extracted from http://docs.nvidia.com/tegra/data/How_To_Use_the_Android_Accelerometer.html
    	// require invertion in X axis
	    //   X   Y  ex  ey
        // { 1, -1,  0, 1 }  // ROTATION_0
        // {-1, -1,  1, 0 }  // ROTATION_90
        // {-1,  1,  0, 1 }  // ROTATION_180
        // { 1,  1,  1, 0 }  // ROTATION_270
        
        switch (getActualOrientation())
    	{
		    case Surface.ROTATION_0:
	            direct[0] = -(e.values[0] / nominalG);
	            direct[1] = -(e.values[1] / nominalG);
	            direct[2] = e.values[2] / nominalG;
	        	//Log.v("Accel", " Rotation 0 ");
		        break;
		    case Surface.ROTATION_90:       
	            direct[0] =  (e.values[1] / nominalG);
	            direct[1] = -(e.values[0] / nominalG);
	            direct[2] = e.values[2] / nominalG;
	        	//Log.v("Accel", " Rotation 90 ");
	    		break;
		    case Surface.ROTATION_180:
	            direct[0] =  (e.values[0] / nominalG);
	            direct[1] =  (e.values[1] / nominalG);
	            direct[2] = e.values[2] / nominalG;
	        	//Log.v("Accel", " Rotation 180 ");
		        break;
		    case Surface.ROTATION_270:
	            direct[0] = -(e.values[1] / nominalG);
	            direct[1] =  (e.values[0] / nominalG);
	            direct[2] = e.values[2] / nominalG;
	        	//Log.v("Accel", " Rotation 270 ");
		        break;
    	}
    	
        final double filteringFactor = 0.1f;

        filtered[0] = ((direct[0] * filteringFactor) + (filtered[0] * (1.0 - filteringFactor)));
        filtered[1] = ((direct[1] * filteringFactor) + (filtered[1] * (1.0 - filteringFactor)));
        filtered[2] = ((direct[2] * filteringFactor) + (filtered[2] * (1.0 - filteringFactor)));

        instant[0] = direct[0] - ((direct[0] * filteringFactor) + (instant[0] * (1.0 - filteringFactor)));
        instant[1] = direct[1] - ((direct[1] * filteringFactor) + (instant[1] * (1.0 - filteringFactor)));
        instant[2] = direct[2] - ((direct[2] * filteringFactor) + (instant[2] * (1.0 - filteringFactor)));
    }

    public CServices()
    {
    }

    public static String loadFile (String filename)
    {
        String s = "";

        try
        {
            FileInputStream stream = new FileInputStream(filename);

            BufferedReader reader = new BufferedReader
                    (new InputStreamReader
                            (stream, Charset.forName(MMFRuntime.inst.charSet)));

            StringBuilder builder = new StringBuilder ();
            char [] buffer = new char [1024 * 16];

            int cpt;

            while ((cpt = reader.read(buffer, 0, buffer.length)) > 0)
                builder.append(buffer, 0, cpt);

            s = builder.toString();
            
            reader.close();
            stream.close();
        }
        catch (Throwable t)
        {
        }

        return s;
    }

    public static void saveFile (String filename, String s)
    {
        try
        {
            FileOutputStream file = new FileOutputStream (filename, false);
            file.write (s.getBytes(MMFRuntime.inst.charSet));
            file.flush();
            file.close ();
        }
        catch (Throwable t)
        {
        }
    }

    public static int parseInt (String s)
    {
        try
        {
            return Integer.parseInt (s);
        }
        catch (Exception e)
        {
            NumberFormat USFormat = NumberFormat.getNumberInstance(Locale.US);
            
            try {
            	
            	return USFormat.parse(s).intValue();
            	
			} catch (ParseException ep) {}
            	
        	return 0;
        }
    }

    public static Uri filenameToURI (String filename)
    {
    	filename = filename.toLowerCase ();
    	Uri uri = null;
    	
    	if(!filename.contains("android."))
    	{
	    	File file = new File (filename);
	    	uri = Uri.fromFile (file);
    	}
    	else
    	{
    		uri = Uri.parse(filename);
    	}
    	if (uri == null)
    		return null;

    	return uri;

    }

    public static int HIWORD(int ul)
    {
        return ul >> 16;
    }

    public static int LOWORD(int ul)
    {
        return ul & 0x0000FFFF;
    }

    public static int MAKELONG(int lo, int hi)
    {
        return (hi << 16) | (lo & 0xFFFF);
    }

    public static int getRValueJava(int rgb)
    {
        return (rgb >>> 16) & 0xFF;
    }

    public static int getGValueJava(int rgb)
    {
        return (rgb >>> 8) & 0xFF;
    }

    public static int getBValueJava(int rgb)
    {
        return rgb & 0xFF;
    }

    public static int RGBJava(int r, int g, int b)
    {
        return (r & 0xFF) << 16 | (g & 0xFF) << 8 | (b & 0xFF);
    }

    public static int swapRGB(int rgb)
    {
        int r = (rgb >>> 16) & 0xFF;
        int g = (rgb >>> 8) & 0xFF;
        int b = rgb & 0xFF;
        return (b & 0xFF) << 16 | (g & 0xFF) << 8 | (r & 0xFF);
    }

    public static String getWord(String s, int start)
    {
        int n;
        char c = ' ';
        if (s.charAt(start) == '"')
        {
            c = '"';
            start++;
        }
        for (n = start; n < s.length(); n++)
        {
            if (s.charAt(n) == c)
            {
                break;
            }
        }
        return s.substring(start, n);
    }

    public static Bitmap replaceColor(Bitmap imgSource, int oldColor, int newColor)
    {
        // Copie l'image source
        int width = imgSource.getWidth();
        int height = imgSource.getHeight();
        int pixels[] = new int[width * height];
        imgSource.getPixels(pixels, 0, width, 0, 0, width, height);

        int x, y;
        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                if ((pixels[y * width + x] & 0xFFFFFF) == oldColor)
                {
                    pixels[y * width + x] = (pixels[y * width + x] & 0xFF000000) | newColor;
                }
            }
        }
        Bitmap imgDest = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
        imgDest.setPixels(pixels, 0, width, 0, 0, width, height);
        return imgDest;
    }

    public static String intToString(int value, int displayFlags)
    {
        String s = Integer.toString(value);
        if ((displayFlags & CPTDISPFLAG_INTNDIGITS) != 0)
        {
            int nDigits = displayFlags & CPTDISPFLAG_INTNDIGITS;
            if (s.length() > nDigits)
            {
                s = s.substring(0, nDigits);
            }
            else
            {
                while (s.length() < nDigits)
                {
                    s = "0" + s;
                }
            }
        }
        return s;
    }
    
    @SuppressWarnings("static-access")
	public static String doubleToString(double value, int displayFlags)
    {
    	boolean  bRemoveTrailingZeros = false;

    	StringBuilder s = new StringBuilder();
     	Formatter formatter = new Formatter(s, Locale.US);
        
        if ( (displayFlags & CPTDISPFLAG_FLOAT_FORMAT) == 0 )
    	{
    		formatter.format("%g", value);
    	}
    	else
    	{
            int nDigits = ((displayFlags & CPTDISPFLAG_FLOATNDIGITS) >> CPTDISPFLAG_FLOATNDIGITS_SHIFT) + 1;
            int nDecimals = -1;
            if ( (displayFlags & CPTDISPFLAG_FLOAT_USENDECIMALS) != 0 )
                nDecimals =((displayFlags & CPTDISPFLAG_FLOATNDECIMALS) >> CPTDISPFLAG_FLOATNDECIMALS_SHIFT);            
            else if ( value > -1.0 && value < 1.0 )
            {
                nDecimals = nDigits;
                bRemoveTrailingZeros = true;
            }
    		
    		String formatFlags = "";
    		String formatWidth = "";
    		String formatPrecision = "";
    		String formatType = "g";
    		
            if ( (displayFlags & CPTDISPFLAG_FLOAT_PADD) != 0 )
    		{
    			formatFlags = "0";
    			if(nDecimals > 0)
    				++nDigits;
    			formatWidth = String.format("%d", nDigits);
    		}
    		
    		if (nDecimals>=0)
    		{
    			formatPrecision = String.format(".%d", nDecimals);
    			formatType = "f";
    		}
    		else
    		{
    			bRemoveTrailingZeros = true;
    		}
    				
    		String format = String.format("%%%s%s%s%s", formatFlags, formatWidth, formatPrecision, formatType);
    		formatter.format(format, value);
    		//Log.Log("format: "+format+ " and result: "+s);
    		
    		if ( bRemoveTrailingZeros && s.indexOf(".") != -1)
    		{
    			for (int i = s.length()-1;  i >= 0; i--) {

    			    if (s.charAt(i) == '0')
    			        s.deleteCharAt(i);
    			    else
    			    	break;
    			}
    		}

    		if (s.charAt(s.length()-1) == '.')
    			s.deleteCharAt(s.length()-1);

   	}

    	return s.toString();
    }
    /*
	public static String DoubleConvertToString(double value, int nDigits, int nDecimals)
	{ 
    	StringBuilder s = new StringBuilder();
     	Formatter formatter = new Formatter(s, Locale.US);
		// Set the sign
		s.append((value >=0 ? "": "-"));
		
		// Handle all as positive values
		DecimalFormat df = new DecimalFormat("#");
        df.setMaximumFractionDigits(12);
        String szDf = df.format(Math.abs(value));
        int length = szDf.length();
		int pPos = szDf.indexOf(".");
		
		if(value > -1.0f && value < 1.0f && nDigits == 0)
			value = Math.round(value);

		value = Math.abs(value);
		
		if (nDecimals == 0)
		{
			s.append(Long.toString((long)value));			
		}
		else if(nDecimals > 0)
		{
			s.append(Long.toString((long)value));
			long frac = Math.round((value - (long)value)*Math.pow(10, nDecimals));
			s.append(".");
			String f = "%0"+nDecimals+"d";
			s.append(String.format(Locale.US, f, frac));
		}
		else if (nDecimals == -1)
		{
			String adouble = Double.toString(value).toLowerCase();
			String DecNotation = "";			
			String NotScientific = "";
			int ExpNotation=0;
			int ExpCnt=0;
			double new_double;

			if(adouble.indexOf("e") != -1)
			{
				DecNotation = adouble.substring(0, adouble.indexOf("e"));	

				ExpNotation += Integer.parseInt(adouble.substring(adouble.indexOf("e")+1));
				new_double = Double.valueOf(DecNotation);
				while(new_double > 1.0)
				{
					new_double /=10.0;
					ExpCnt++;
				}
			}
			else 
			{
				DecNotation = adouble;
				new_double = Double.valueOf(DecNotation);
				while(new_double > 1.0)
				{
					new_double /=10.0;
					ExpCnt++;
				}
			}
			
			if(ExpCnt > 0)
			{
				ExpNotation += (ExpCnt-1);
				ExpCnt = 1;
			}
			
			if(nDigits < ExpNotation+ExpCnt || value < 1.0)
			{
				if(nDigits >= 0)
				{
					double frac = (Math.round(new_double *Math.pow(10, (nDigits))))/Math.pow(10, (nDigits-ExpCnt));
					if(frac - (long)frac > 0)
						s.append(Double.toString(frac));
					else if(frac >0)
						s.append(Long.toString((long)frac));
				}
				if(ExpNotation != 0)
				{
					NotScientific = String.format(Locale.US, "e%c%03d", (ExpNotation >=0 ? '+':'-'),Math.abs(ExpNotation));
					s.append(NotScientific);
				}
			}
			else if (nDigits == ExpNotation+ExpCnt)
			{
				s.append(Long.toString((long)(Math.round(new_double *Math.pow(10, (nDigits)))/Math.pow(10, (nDigits-ExpCnt))*Math.pow(10, (nDigits-ExpCnt)))));				
			}
			else
			{
				s.append(Long.toString((long)value));
				if(nDigits - ExpNotation > 0)
				{
					long frac = Math.round((value - (long)value)*Math.pow(10, nDigits-ExpNotation-ExpCnt));
					if(frac > 0)
					{
						s.append(".");
						s.append(Long.toString(frac));
					}
				}
			}
		
		}
		else if(nDecimals < -1)
		{
			s.append(Long.toString(Math.round(value*Math.pow(10, nDecimals))));
		}
		
		if(nDecimals == 0 && s.indexOf(".") != -1)
		{
			for (int i = s.length()-1;  i >= 0; i--) {
	
			    if (s.charAt(i) == '0')
			        s.deleteCharAt(i);
			    else
			    	break;
			}
		}

		if (s.length() > 0 && s.charAt(s.length()-1) == '.')
			s.deleteCharAt(s.length()-1);

		return s;
	}  
	*/
    
    public static void drawRegion(Canvas g, Paint p, Bitmap source, int sourceX, int sourceY, int sx, int sy, int destX, int destY)
    {
        int width = source.getWidth();
        int height = source.getHeight();
        if (sourceX < 0)
        {
            destX -= sourceX;
            sx += sourceX;
            sourceX = 0;
        }
        if (sourceX + sx > width)
        {
            sx = width - sourceX;
        }
        if (sourceY < 0)
        {
            destY -= sourceY;
            sy += sourceY;
            sourceY = 0;
        }
        if (sourceY + sy > height)
        {
            sy = height - sourceY;
        }
        if (sx > 0 && sy > 0)
        {
        	Rect srcRect=new Rect(sourceX, sourceY, sourceX+sx, sourceY+sy);
        	Rect dstRect=new Rect(destX, destY, destX+sx, destY+sy);
        	g.drawBitmap(source, srcRect, dstRect, p);
        }
    }

    public static int paintTextHeight(Paint p)
    {
    	return (int) (Math.ceil(-p.ascent()) + Math.ceil(p.descent()));
    }
    
    public static int paintTextHeight2(Paint p)
    {
    	return (int) (Math.ceil(p.getFontMetrics().bottom - p.getFontMetrics().top));
    }

    public static String getAndroidID ()
    {
        try
        {
            return Settings.Secure.getString (MMFRuntime.inst.getContentResolver (),
                                             Settings.Secure.ANDROID_ID);
        }
        catch (Throwable e)
        {
            return "";
        }
    }

    public static int [] getBitmapPixels (Bitmap img)
    {
        if(img == null)
            return null;
        int [] pixels = new int [img.getWidth () * img.getHeight ()];
        img.getPixels (pixels, 0, img.getWidth (), 0, 0, img.getWidth (), img.getHeight ());
        return pixels;
    }
    
    public static int getActualOrientation() {
        Display display;         
        display = MMFRuntime.inst.getWindow().getWindowManager().getDefaultDisplay();
        int rotation = display.getRotation();
        //Log.v("Orientation"," Rotation is "+rotation);
        return rotation;	
    }

	public static double deviceDPI(Activity activity)
	{
		DisplayMetrics matrix = new DisplayMetrics();
		activity.getWindowManager().getDefaultDisplay().getMetrics(matrix);

		return (matrix.densityDpi*1.0);
	}
	
	public static float deviceDensity()
	{
		DisplayMetrics matrix = new DisplayMetrics();
		MMFRuntime.inst.getWindowManager().getDefaultDisplay().getMetrics(matrix);

		return (matrix.density);
	}

	public static float getDPFromPixels(float pixels) {
	    DisplayMetrics metrics = new DisplayMetrics();
	    MMFRuntime.inst.getWindowManager().getDefaultDisplay().getMetrics(metrics);
	    pixels /= metrics.density;
	    return pixels;
	}

	public static double getSPFromPixels(double pixels) {
		pixels= (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_SP,
                (float) pixels, MMFRuntime.inst.getResources().getDisplayMetrics());
		return pixels;
	}
	
	public static float getHeightFromFontSize(int font_size, int min, float scale) {
		TextView text = new TextView(MMFRuntime.inst);
		text.setTypeface(Typeface.DEFAULT);
        text.setTextSize(TypedValue.COMPLEX_UNIT_PX, font_size*scale);
        text.setMinHeight(min);
        return text.getTextSize();		
	}
	
	public static float getHeightFromFont(CFontInfo font, float scale) {
		TextPaint text = new TextPaint();
		text.setTypeface(Typeface.DEFAULT);
        text.setTextSize(font.lfHeight*scale);
        text.setSubpixelText(true);
        text.setAntiAlias(true);

        float height = 0;

        // Check if we're running on Android 6.0 or higher
         if (MMFRuntime.deviceApi >= 23) {
             StaticLayout layout = FStaticLayoutNew ("WAQJYyaqjmw", text, 2048, Layout.Alignment.ALIGN_NORMAL, 1.0f, 0.0f, false);
             height = layout.getHeight();
         }
         else {
             StaticLayout layout = FStaticLayoutOld ("WAQJYyaqjmw", text, 2048, Layout.Alignment.ALIGN_NORMAL, 1.0f, 0.0f, false);
             height = layout.getHeight();
         }
        
        return height;
	}

	public static StaticLayout FSStaticLayout(CharSequence source, TextPaint paint, int width, Layout.Alignment alignment, float spacingmult, float spacingadd, boolean includepad)
    {
        if (MMFRuntime.deviceApi >= 23) {
            return FStaticLayoutNew (source, paint, width, alignment, spacingmult, spacingadd, includepad);
        }
        else {
            return FStaticLayoutOld  (source, paint, width, alignment, spacingmult, spacingadd, includepad);
        }
    }

	@TargetApi(23)
	private static StaticLayout FStaticLayoutNew(CharSequence source, TextPaint paint, int width, Layout.Alignment alignment, float spacingmult, float spacingadd, boolean includepad)
    {
        StaticLayout.Builder sb= StaticLayout.Builder.obtain(source, 0, source.length(), paint, width);
        sb.setAlignment(alignment);
        sb.setLineSpacing(spacingadd, spacingmult);
        sb.setIncludePad(includepad);
        StaticLayout layout = sb.build();
        return layout;
    }

    @Deprecated
    private static StaticLayout FStaticLayoutOld(CharSequence source, TextPaint paint, int width, Layout.Alignment alignment, float spacingmult, float spacingadd, boolean includepad)
    {
        StaticLayout layout = new StaticLayout(source, paint, width, alignment, spacingmult, spacingadd, includepad);
        return layout;
    }

    public static double deviceSizeInch()
	{
		DisplayMetrics matrix = new DisplayMetrics();
		MMFRuntime.inst.getWindowManager().getDefaultDisplay().getMetrics(matrix);

		int width = matrix.widthPixels;
		int height = matrix.heightPixels;

		float xdpi = matrix.xdpi;
		float ydpi = matrix.ydpi;

		float screenSizeWidth = width / xdpi;
		float screenSizeHeight= height / ydpi;
		//float screenSizeDpiHorz = xdpi;
		//float screenSizeDpiVert = ydpi;
		//float screenSizeDensDPI = matrix.densityDpi;
		//float screenSizeDensRatio = matrix.density;

		double display = Math.sqrt(screenSizeWidth * screenSizeWidth + screenSizeHeight * screenSizeHeight);
		return display;
	}
	
	public static void simulateKey(final int KeyCode) {

	    new Thread() {
	        @Override
	        public void run() {
	            try {
	                Instrumentation inst = new Instrumentation();
	                inst.sendKeyDownUpSync(KeyCode);
	            } catch (Exception e) {
	                Log.Log("Exception when sendKeyDownUpSync "+ e.toString());
	            }
	        }

	    }.start();
	}

    public static boolean checkBitmapFitsInMemory(long bmpwidth,long bmpheight, int bmpdensity ){
        long reqsize=bmpwidth*bmpheight*bmpdensity;
        long allocNativeHeap = Debug.getNativeHeapAllocatedSize();
        Runtime runtime = Runtime.getRuntime();

        if(runtime.freeMemory() > 2*reqsize)
            return true;

        final long heapPad=(long) Math.max(4*1024*1024, runtime.maxMemory()*0.1);
        String s = String.format("(MB) Max mem. available %.3f HeapPad: %.3f AllocNativeHeap: %.3f",runtime.maxMemory()/1024.0/1024, heapPad/1024.0/1024, allocNativeHeap/1024.0/1024);
        Log.Log(s);
        if ((reqsize + allocNativeHeap + heapPad) >= runtime.maxMemory())
        {
            return false;
        }
        return true;

    }

    public static boolean checkFitsInMemoryAndCollect(long reqsize ){
        long allocNativeHeap = Debug.getNativeHeapAllocatedSize();
        Runtime runtime = Runtime.getRuntime();

        if(runtime.freeMemory() > 2*reqsize)
            return true;

        final long heapPad=(long) Math.max(4*1024*1024, runtime.maxMemory()*0.1);
        String s = String.format("(MB) Max mem. available %.3f HeapPad: %.3f AllocNativeHeap: %.3f",runtime.maxMemory()/1024.0/1024, heapPad/1024.0/1024, allocNativeHeap/1024.0/1024);
        Log.Log(s);
        if ((reqsize + allocNativeHeap + heapPad) >= runtime.maxMemory())
        {
            runtime.gc();
            return false;
        }
        return true;

    }

    public static int getTotalMegaRAM()
    {
        RandomAccessFile reader = null;
        String load = null;
        double totRam = 0;
        int memTotalRAM = 0;

        try {
            reader = new RandomAccessFile("/proc/meminfo", "r");
            load = reader.readLine();

            // Get the Number value from the string
            Pattern p = Pattern.compile("(\\d+)");
            Matcher m = p.matcher(load);
            String value = "";
            while (m.find()) {
                value = m.group(1);
            }
            reader.close();

            totRam = Double.parseDouble(value);
            memTotalRAM = (int)(totRam / 1024.0);


        } catch (IOException e) {
            e.printStackTrace();
        }

        return memTotalRAM;
    }
}
