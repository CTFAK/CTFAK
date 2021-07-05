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
// CSOUNDPLAYER : synthetiseur MIDI
//
//----------------------------------------------------------------------------------
package Application;

import Banks.CMusic;

/** Plays sounds from the sound bank.
 * This object is called from the play sound actions.
 */
public class CMusicPlayer 
{   
    CRunApp app;
    boolean bOn=true;
    CMusic music=null;
    
    public CMusicPlayer(CRunApp a) 
    {
    	app=a;
    }
   
    public void play(short handle, int nLoops)
    {
        CMusic m = app.musicBank.getMusicFromHandle(handle);
        if (m == null)
        {
            return;
        }
    	if (m==music)
    	{
			music.stop();
    	}
		music=m;
		music.setLoopCount(nLoops);
		music.start();
    }
    public void play(String filename, int nLoops)
    {
        try
        {
            music = new CMusic(this, filename);
        }
        catch (Throwable e)
        {
            return;
        }

		music.setLoopCount(nLoops);
		music.start();
    }
    public void stopAllMusics()
    {
    	if (music!=null)
    	{
    		music.stop();
    		music=null;
    	}
    }
    public void stop()
    {
    	if (music!=null)
    	{
    		music.stop();
    		music=null;
    	}
    }
    public void pause()
    {
    	if (music!=null)
    	{
    		music.pause();
    	}
    }
    public void resume()
    {
    	if (music!=null)
    	{
    		music.start();
    	}
    }
    public boolean isMusicPlaying()
    {
    	if (music!=null)
    	{
    		return music.isPlaying();
    	}
    	return false;
    }	
    public boolean isMusicPaused()
    {
    	if (music!=null)
    	{
    		return music.isPaused();
    	}
    	return false;
    }
    public boolean isMusicPlaying(short handle)
    {
    	if (music!=null)
    	{
    		if (music.handle==handle)
    		{
    			return music.isPlaying();
    		}
    	}
    	return false;
    }
    public void removeMusic(CMusic music)
    {
    	music=null;
    }    
}
