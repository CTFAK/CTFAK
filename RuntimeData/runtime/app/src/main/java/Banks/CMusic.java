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
// CSOUND : un echantillon
//
//----------------------------------------------------------------------------------
package Banks;

import java.io.IOException;

import Application.CMusicPlayer;
import Application.CRunApp;
import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CServices;
import android.content.res.AssetFileDescriptor;
import android.media.MediaPlayer;
import android.net.Uri;

public class CMusic implements MediaPlayer.OnCompletionListener, MediaPlayer.OnPreparedListener
{
	public CMusicPlayer mPlayer;
	public short handle = -1;
	public boolean bUninterruptible=false;
	public MediaPlayer mediaPlayer=null;
	public int nLoops;
	public CRunApp application;
	public boolean bPaused=false;
	public int resID;
	public String filename;
	public Uri uri;
    
	boolean bPrepared;

	public CMusic(CMusicPlayer m)
	{
		mPlayer=m;

	}

	public CMusic (CMusicPlayer m, String filename) throws Exception
	{
		mPlayer=m;
		resID = 0;

		uri = CServices.filenameToURI(filename);

		if (uri == null)
			throw new IOException ("Can't open file");

		mediaPlayer = MediaPlayer.create(MMFRuntime.inst, uri);
		mediaPlayer.setOnCompletionListener(this);
	}

	public void load(short h, CRunApp app) 
	{
		handle=h;
		application=app;

		// STARTCUT
		// ENDCUT

		reload();
	}
	public void reload()
	{
		if(mediaPlayer != null)
			mediaPlayer.release();

		mediaPlayer = null;
		
		try {

			mediaPlayer = new MediaPlayer();

			if(mediaPlayer != null) {
				if (uri == null)
				{
					if (resID == 0)
						return;

					AssetFileDescriptor afd = (MMFRuntime.inst.getApplicationContext().getResources()).openRawResourceFd(resID);
					mediaPlayer.setDataSource(afd.getFileDescriptor(), afd.getStartOffset(), afd.getDeclaredLength());
				}
				else
				{
					mediaPlayer.setDataSource(MMFRuntime.inst.getApplicationContext(), uri);
				}

			}
			mediaPlayer.setOnCompletionListener(this);
			mediaPlayer.setOnPreparedListener(this);
			mediaPlayer.prepareAsync();
			
		} catch (IllegalArgumentException e) {
			Log.Log("Error loading music: " + e);
			mediaPlayer = null;
		} catch (SecurityException e) {
			Log.Log("Error loading music: " + e);
			mediaPlayer = null;
		} catch (IllegalStateException e) {
			Log.Log("Error loading music: " + e);
			mediaPlayer = null;
		} catch (IOException e) {
			Log.Log("Error loading music: " + e);
			mediaPlayer = null;
		}
	}
	public void setLoopCount(int count)
	{
		nLoops=count;
	}
	public void start()
	{
		if(mediaPlayer != null && bPrepared)
		{
			mediaPlayer.start();
			bPaused=false;
		}
	}
	public void stop()
	{
		if(mediaPlayer != null)
		{
			mediaPlayer.reset();
			mediaPlayer.release();
			mediaPlayer = null;
			reload();
		}
	}
	public boolean isPlaying()
	{
		return mediaPlayer != null && mediaPlayer.isPlaying();
	}
	public void pause()
	{
		if (mediaPlayer != null && mediaPlayer.isPlaying())
		{
			mediaPlayer.pause();
			bPaused=true;
		}
	}
	public boolean isPaused()
	{
		return bPaused;
	}
	public void release()
	{
		if(mediaPlayer != null)
		{
			mediaPlayer.reset();
			mediaPlayer.release();
			mediaPlayer = null;
		}
	}
	@Override
	public void onCompletion(MediaPlayer mp)
	{
		mediaPlayer.reset();
		mediaPlayer.release();
		reload();
		if (nLoops>0)
		{
			nLoops--;
			if (nLoops==0)
			{
				mPlayer.removeMusic(this);
				return;
			}
			mediaPlayer.start();
		}
	}
	
	@Override
	public void onPrepared(MediaPlayer mp) {
		bPrepared = true;
		mp.start();
	}	
	
}
