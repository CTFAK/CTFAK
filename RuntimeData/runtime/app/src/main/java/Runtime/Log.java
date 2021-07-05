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
package Runtime;

import android.content.Context;
import android.content.SharedPreferences;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.TimeZone;

public class Log extends Writer
{
    public static void Log(String s)
    {
        android.util.Log.i("MMFRuntime", s);
    }
    
    private String output;
    private static boolean notNormallyEnded;

    @Override
	public void flush ()
    {	
    	Log.Log("[OpenGL] " + output);
    	output = "";
    }
    
    @Override
	public void close ()
    {
    }
    
    @Override
	public void write (char [] buf, int offset, int c)
    {
    	for (int i = offset; i < buf.length; ++ i)
    		output += buf [i];
    }

    public static void CleanLogcat() {
        try {
            Runtime.getRuntime().exec("logcat -c ");
        } catch (IOException e) {
            Log.Log("I/O error when cleaning logcat");
        }
    }

    public static void GenerateLogcat() {
        GenerateLogcat(MMFRuntime.inst);
     }

    public static void GenerateLogcat(Context context) {
        SimpleDateFormat dateFormatGmt = new SimpleDateFormat("dd_MM_yyyy_HH_mm_ss");
        dateFormatGmt.setTimeZone(TimeZone.getTimeZone("GMT"));
        // save logcat in file
        File directory = new File(context.getFilesDir() + "/reports");
        if(!directory.isDirectory())
            directory.mkdirs();

        File outputFile = new File(context.getFilesDir() + "/reports",
                String.format(Locale.ENGLISH, "logcat_%s.txt", dateFormatGmt.format(new Date())));
        try {
            Runtime.getRuntime().exec(
                    "logcat -f " + outputFile.getAbsolutePath());
        } catch (IOException e) {
            Log.Log("I/O error when making logcat");
        }

    }


    public static void CrashLogcat(final Throwable e)
    {
        SimpleDateFormat dateFormatGmt = new SimpleDateFormat("dd_MM_yyyy_HH_mm_ss");
        dateFormatGmt.setTimeZone(TimeZone.getTimeZone("GMT"));
        // save crash in file
        File directory = new File(MMFRuntime.inst.getFilesDir() + "/reports");
        if(!directory.isDirectory())
            directory.mkdirs();

        File outputFile = new File(MMFRuntime.inst.getFilesDir() + "/reports",
                String.format(Locale.ENGLISH, "crash_%s.txt", dateFormatGmt.format(new Date())));

        e.printStackTrace();

        String report = MMFRuntime.version + " : " + e.toString ();

        StackTraceElement [] stack = e.getStackTrace ();

        Log.Log ("Stack has " + stack.length + " elements");

        try
        {
            FileOutputStream fOut = new FileOutputStream(outputFile);
            OutputStreamWriter outWriter = new OutputStreamWriter(fOut);

            for (int i = 0; i < stack.length; ++ i)
                report += "\r\n    " + stack [i].toString ();

            outWriter.append(report);
            outWriter.close();

            fOut.flush();
            fOut.close();
        }
        catch (IOException el)
        {
            Log.Log( "File write failed: " + el.toString());
        }

        GenerateLogcat();
     }

    public static boolean isEnabledReport(Context context)
    {
        SharedPreferences loggerInfo = context.getSharedPreferences("logger_report_info", 0);

        if(loggerInfo != null) {
            if(loggerInfo.getBoolean("logger_cleanAtStart", false))
            {
                try {
                    Runtime.getRuntime().exec("logcat -c ");
                } catch (IOException e) {
                    Log.Log("I/O error when cleaning logcat");
                }
            }
            boolean enabled = loggerInfo.getBoolean("logger_enabled", false);
            notNormallyEnded = loggerInfo.getBoolean("logger_setAtStart", false);
            if(enabled && notNormallyEnded)
            {
                GenerateLogcat(context);
                CleanLogcat();
            }
            return enabled;
         }

        return false;
    }

    public static boolean WasNormallyEnded()
    {
        return !notNormallyEnded;
    }

    public static void AboutToStart(Context context)
    {
        SharedPreferences loggerInfo = context.getSharedPreferences("logger_report_info", 0);

        if(loggerInfo != null) {
            SharedPreferences.Editor editor = loggerInfo.edit();
            editor.putBoolean("logger_setAtStart", true);
            editor.apply();
        }

    }

    public static void ResetAtStart(Context context)
    {
        SharedPreferences loggerInfo = context.getSharedPreferences("logger_report_info", 0);

        if(loggerInfo != null) {
            SharedPreferences.Editor editor = loggerInfo.edit();
            editor.remove("logger_setAtStart");
            editor.commit();
        }

    }

}
