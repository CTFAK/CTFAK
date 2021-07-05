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

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.Map;

import javax.net.ssl.HttpsURLConnection;

import Services.CServices;

public class CrashReporter implements Thread.UncaughtExceptionHandler
{
    private static String info = "";
    private HashMap<String, String> postData;

    public static void addInfo (String subject, String data)
    {
        if (info.length() > 0)
            info += "\r\n";

        info += "    " + subject + " : " + data;
    }

	private String makePostDataString(HashMap<String, String> params) throws UnsupportedEncodingException{
		StringBuilder result = new StringBuilder();
		boolean first = true;
		for(Map.Entry<String, String> entry : params.entrySet()){
			if (first)
				first = false;
			else
				result.append('&');

			result.append(URLEncoder.encode(entry.getKey(), "UTF-8"));
			result.append('=');
			result.append(URLEncoder.encode(entry.getValue(), "UTF-8"));
		}

		return result.toString();
	}    
    
    @Override
	public void uncaughtException (Thread t, final Throwable e)
    {
        if(MMFRuntime.inst.enableLoggerReporting)
            Log.CrashLogcat(e);

        if (!MMFRuntime.inst.enableCrashReporting)
        {
            Log.Log ("Crash reporting disabled");
            Thread.getDefaultUncaughtExceptionHandler().uncaughtException (t, e);

            return;
        }

        CrashReporter.addInfo ("Device ID", CServices.getAndroidID());

        Log.Log ("Reporting exception ...");
        
        e.printStackTrace();
        
        String report = MMFRuntime.version + " : " + e.toString ();
        report += "Events line: "+"";

        StackTraceElement [] stack = e.getStackTrace ();

        Log.Log ("Stack has " + stack.length + " elements");

        for (int i = 0; i < stack.length; ++ i)
            report += "\r\n    " + stack [i].toString ();

        if (info.length() > 0)
        {
            report += "\r\n\r\n";
            report += info;
        }
        
        postData.put("report",report);
 
        URL url;
        String response = "";
        
        try {
            url = new URL("http://bugs.clickteam.com/report.php");

            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setReadTimeout(15000);
            conn.setConnectTimeout(15000);
            conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
            conn.setRequestMethod("POST");
            conn.setDoInput(true);
            conn.setDoOutput(true);


            OutputStream os = conn.getOutputStream();
            BufferedWriter writer = new BufferedWriter(
                    new OutputStreamWriter(os, "UTF-8"));
            Object postDataParams;
			writer.write(makePostDataString(postData));

            writer.flush();
            writer.close();
            os.close();
            int responseCode=conn.getResponseCode();

			if (responseCode == HttpsURLConnection.HTTP_OK) {
                String line;
                BufferedReader br=new BufferedReader(new InputStreamReader(conn.getInputStream()));
                while ((line=br.readLine()) != null) {
                    response+=line;
                }
                br.close();
            }
            else {
                response="";    

            }
        } catch (Exception e1) {
            e1.printStackTrace();
        }
        Log.Log ("Reported exception: " + e.toString ());

        Thread.getDefaultUncaughtExceptionHandler().uncaughtException (t, e);
        
    }
    
    
}
