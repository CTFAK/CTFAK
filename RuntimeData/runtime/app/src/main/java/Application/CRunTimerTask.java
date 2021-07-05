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
package Application;

import Runtime.MMFRuntime;
import Runtime.SurfaceView;
import Runtime.Log;
import android.os.Handler;
import android.os.SystemClock;

public class CRunTimerTask implements Runnable
{
    public long interval;
    Handler handler;

    public boolean dead = false;

    public CRunTimerTask (Handler handler, long interval)
    {
        this.handler = handler;
        this.interval = interval;
    }

    @Override
	public void run ()
        {
        synchronized (this)
        {
            if (dead)
                return;

            long start = SystemClock.uptimeMillis();

            if(SurfaceView.hasSurface)
                SurfaceView.inst.makeCurrentIfNecessary();

            boolean quit = !MMFRuntime.inst.app.playApplication (false);

            if (quit) {
                Log.Log("Application was reported quit");
                MMFRuntime.inst.die();
            }
            else
                schedule(start + interval);
        }
    }

    public void schedule (long when)
    {
        // handler.postDelayed(this, 0);

        handler.postAtTime(this, when);
    }
}
