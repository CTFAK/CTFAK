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

import Banks.CSound;
import Banks.CSoundChannel;
import Runtime.Log;
import Runtime.MMFRuntime;
import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.content.Context;
import android.media.AudioAttributes;
import android.media.AudioManager;
import android.media.AudioManager.OnAudioFocusChangeListener;
import android.media.SoundPool;
import android.os.Build;


public class CSoundPlayer implements SoundPool.OnLoadCompleteListener
{
	public static final int nChannels = 32;
	public CRunApp app;
	boolean bOn = true;
	protected CSoundChannel channels[];
	public boolean bMultipleSounds = true;

	public SoundPool soundPool = null;

	private float mainVolume = 1.0f;
	private float mainPan = 0.0f;

	protected AudioManager audioMgr;
	private boolean AudioFocus;

	public AudioAttributes attributes;
	
	public void setVolume (float volume)
	{
		if (volume > 1.0f)
			volume = 1.0f;

		if (volume < 0.0f)
			volume = 0.0f;

		if(volume == this.mainVolume)
			return;

		this.mainVolume = volume;

		for(int i = 0; i < channels.length; ++ i) {
			if (channels [i].currentSound != null) {
				channels [i].currentSound.updateVolume (-1);
			}
		}
	}

	public float getVolume ()
	{
		return this.mainVolume;
	}

	public void setPan (float pan)
	{
		if (pan > 1.0f)
			pan = 1.0f;

		if (pan < -1.0f)
			pan = -1.0f;

		this.mainPan = pan;

		for(int i = 0; i < channels.length; ++ i)
			if (channels [i].currentSound != null)
				channels [i].currentSound.updateVolume (-1);
	}

	public float getPan()
	{
		return this.mainPan;
	}
	
	@SuppressLint("NewApi")
	@TargetApi(21)
	protected void createSoundPoolWithBuilder(){
		attributes = new AudioAttributes.Builder()
				.setUsage(AudioAttributes.USAGE_GAME)
				.setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
				.build();

		soundPool = new SoundPool.Builder().setAudioAttributes(attributes).setMaxStreams(30).build();
		Log.Log("soundPool builder ...");
	}

	@SuppressWarnings("deprecation")
	protected void createSoundPoolWithConstructor(){
		soundPool = new SoundPool(10, AudioManager.STREAM_MUSIC, 0);
	}

	@SuppressWarnings("deprecation")
	public CSoundPlayer(CRunApp a)
	{
		app = a;
		attributes = null;
		
		if(audioMgr == null)
			audioMgr = (AudioManager)MMFRuntime.inst.getApplicationContext().getSystemService(Context.AUDIO_SERVICE); 
	
		if (Build.VERSION.SDK_INT >= 17)
		{
			Log.Log("Sample rate for device: "+audioMgr.getProperty("android.media.property.OUTPUT_SAMPLE_RATE"));
		}
		else
			audioMgr.setParameters("android.media.property.OUTPUT_SAMPLE_RATE=44100");
			
		if (Build.VERSION.SDK_INT >= 21) {
			createSoundPoolWithBuilder();
		} else{
			createSoundPoolWithConstructor();
		}

		if (soundPool == null) {
			throw new RuntimeException("Audio: failed to create SoundPool");
		}

		soundPool.setOnLoadCompleteListener(this);

		AudioFocus = false;

		channels = new CSoundChannel [nChannels];

		for (int i = 0; i < channels.length; ++ i)
			channels [i] = new CSoundChannel ();

		this.getVolume();
	}

	/** Plays a simple sound.
	 */
	public void play(short handle, int nLoops, int channel, boolean bPrio)
	{
		int n;
		boolean withChannel = (channel != -1 ? true : false);

		if (bOn == false)
		{
			return;
		}

		CSound sound = app.soundBank.newSoundFromHandle(handle);
		//Log.Log("sound is created? "+(sound == null ? "no":"yes"));
		if (sound == null)
		{
			return;
		}
		if (bMultipleSounds == false)
		{
			channel = 0;
		}

		/* UNINTERRUPTABLE - This option means that the sound cannot be interrupted by a sound that hasn't this option. 
		 * A sound can be interrupted by another one in 2 cases: 
		 * 		(1) when you play the sound on a specific channel and another one is already playing on this channel, or 
		 * 		(2) when you play a sound without specifying a channel and there is no free channel available. 
		 * 			(a) If the playing sound has the option and the new sound hasn't the option, the new sound won't be played. 
		 * 			(b) The sound will be played on the first channel whose sound hasn't this option.
		 */
			
		// Lance le son
		if (channel < 0)
		{
			for (n = 0; n < nChannels; n++)
				if (channels[n].currentSound == null && !channels[n].locked)
					break;

			if (n == nChannels)
			{
				// Stoppe le son sur un canal deja en route
				for (n = 0; n < nChannels; n++) // Rule #2 part (b)
					if(channels[n].stop (false)) // Rule #2 part (a) && channel locked
						break;
			}
			
			channel = n;	// assigned channel for Rule #2
		}
		
		// Rule #1
		if (channel < 0 || channel >= nChannels || !channels [channel].stop(bPrio))
			return;
		
		if(channels[channel].currentSound != null)
			channels[channel].currentSound.release();

		channels[channel].currentSound = sound;

		sound.setUninterruptible(bPrio);
		sound.setLoopCount(nLoops);

		if(withChannel)
		{
			setFrequencyChannel(channel, channels [channel].frequency);	    
			setPanChannel(channel, channels [channel].pan);       	
		}

		sound.setVolume(channels [channel].volume);    
		getAudioFocus(); 
		
		sound.start();
	}

	public void playFile(String filename, int nLoops, int channel, boolean bPrio)
	{
		int n;
		boolean withChannel = (channel != -1 ? true : false);

		if (bOn == false)
		{
			return;
		}

		CSound sound;

		try
		{
			sound = new CSound (this, filename);
		}
		catch(Throwable e)
		{
			return;
		}

		if (bMultipleSounds == false)
		{
			channel = 0;
		}

		// Lance le son
		if (channel < 0)
		{
			for (n = 0; n < nChannels; n++)
				if (channels[n].currentSound == null && !channels [n].locked)
					break;

			if (n == nChannels)
			{
				// Stoppe le son sur un canal deja en route
				for (n = 0; n < nChannels; n++)
					if (channels[n].stop (false))
						break;
			}
			channel = n;
		}

		if (channel < 0 || channel >= nChannels || !channels [channel].stop(false))
			return;

		channels[channel].currentSound = sound;
		sound.setUninterruptible(bPrio);
		sound.setLoopCount(nLoops);
		
		if(withChannel)
		{
			setFrequencyChannel(channel, channels [channel].frequency);	    
			setPanChannel(channel, channels [channel].pan);       	
			sound.setVolume(channels [channel].volume);    
		}
		else
			sound.setVolume(1.0f);    

		getAudioFocus();    
		sound.start();
	}

	public void setMultipleSounds(boolean bMultiple)
	{
		bMultipleSounds = bMultiple;
	}

	public void stopAllSounds()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n] != null)
			{
				channels[n].stop(true);
			}
		}
		resetAudioFocus();
	}

	public void stop(short handle)
	{
		int c;
		for (c = 0; c < nChannels; c++)
		{
			if (channels[c].currentSound != null)
			{
				if (channels[c].currentSound.handle == handle)
				{
					channels[c].stop(true);
				}
			}
		}
	}

	public boolean isSoundPlaying()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.isPlaying())
				{
					return true;
				}
			}
		}
		return false;
	}

	public boolean isSamplePlaying(short handle)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					if (channels[n].currentSound.isPlaying())
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public boolean isSamplePaused(short handle)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					if (channels[n].currentSound.isPaused())
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public boolean isChannelPlaying(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				return channels[channel].currentSound.isPlaying();
			}
		}
		return false;
	}

	public boolean isChannelPaused(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				if (channels[channel].currentSound.isPaused() == true)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void pause(short handle)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					channels[n].currentSound.pause();
				}
			}
		}
	}

	public void resume(short handle)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					channels[n].currentSound.resume();
				}
			}
		}
	}

	public void pause()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				channels[n].currentSound.pause();
			}
		}
	}

	// Runtime Pause
	public void pause2()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				channels[n].currentSound.pause2();
			}
		}
	}

	public void pauseAllChannels()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				channels[n].currentSound.pause();
			}
		}
	}

	public void resume()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				channels[n].currentSound.resume();
			}
		}
	}

	// Runtime Resume
	public void resume2()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				channels[n].currentSound.resume2();
			}
		}
	}

	public void resumeAllChannels()
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				channels[n].currentSound.resume();
			}
		}
	}

	public void pauseChannel(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				channels[channel].currentSound.pause();
			}
		}
	}

	public void stopChannel(int channel)
	{
		if (channel >= 0 && channel < nChannels)
			channels[channel].stop(true);
	}

	public void resumeChannel(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				channels[channel].currentSound.resume();
			}
		}
	}

	public int getChannel(String name)
	{
		for (int n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.soundName.contentEquals(name))
				{
					return n;
				}
			}
		}
		return -1;
	}

	public int getChannelDuration(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				return channels[channel].currentSound.getDuration();
			}
		}
		return 0;
	}

	public int getSampleDuration(String name)
	{
	    int channel = getChannel(name);
	    if (channel >= 0)
	    {
            return channels[channel].currentSound.getDuration();
	    }
	    return 0;
	}

	public void setPositionChannel(int channel, int pos)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				channels[channel].currentSound.setPosition(pos);
			}
		}
	}

	public int getPositionChannel(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			if (channels[channel].currentSound != null)
			{
				return channels[channel].currentSound.getPosition();
			}
		}
		return 0;
	}

	public int getSamplePosition(String name)
	{
	    int channel = getChannel(name);
	    if (channel >= 0)
	    {
            return channels[channel].currentSound.getPosition();
	    }
	    return 0;
	}
	
	public void setFrequencyChannel (int channel, int frequency)
	{
		if (channel >= 0 && channel < nChannels)
		{
			channels[channel].frequency = frequency;

			if (channels[channel].currentSound != null)
			{
				channels[channel].currentSound.frequency = frequency;
				channels[channel].currentSound.updateVolume (-1);
			}
		}
	}

	public void setVolumeChannel (int channel, float volume)
	{
		if (channel >= 0 && channel < nChannels)
		{
			volume = Math.max(0, Math.min(1.0f, volume));
			channels[channel].volume = volume;

			if (channels[channel].currentSound != null)
			{
				channels[channel].currentSound.updateVolume (channels[channel].volume);
			}
		}
	}

	public float getVolumeChannel (int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			return  channels[channel].volume;
		}
		return 0;
	}

	public float getSampleVolume(String name)
	{
	    int channel = getChannel(name);
	    if (channel >= 0)
	    {
            return channels[channel].volume;
	    }
	    return 0;
	}

	public void setPanChannel (int channel, float pan)
	{
		if (channel >= 0 && channel < nChannels)
		{
			pan = Math.max(-1.0f, Math.min(1.0f, pan));
			channels[channel].pan = pan;

			if (channels[channel].currentSound != null)
			{
				channels[channel].pan = pan;
				channels[channel].currentSound.pan = pan;
				channels[channel].currentSound.updateVolume (-1);
			}
		}
	}

	public float getPanChannel (int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			return channels[channel].pan;
		}
		return 0;
	}

	public float getSamplePan(String name)
	{
	    int channel = getChannel(name);
	    if (channel >= 0)
	    {
            return channels[channel].pan;
	    }
	    return 0;
	}

	public void setPosition(short handle, int pos)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					channels[n].currentSound.setPosition(pos);
				}
			}
		}
	}

	public void setVolume(short handle, float volume)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					volume = Math.max(0.0f, Math.min(1.0f, volume));
					channels[n].currentSound.volume = volume;
					channels[n].currentSound.updateVolume (volume);
				}
			}
		}
	}

	public void setFrequency (short handle, int frequency)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					channels[n].currentSound.frequency = frequency;
					channels[n].currentSound.updateVolume (-1);
				}
			}
		}
	}

	public int getFrequency (int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			return channels[channel].frequency;
		}
		return 0;
	}

	public int getSampleFrequency (String name)
	{
		int channel = getChannel(name);
	    if (channel >= 0)
		{
			return channels[channel].frequency;
		}
		return 0;
	}

	public void setPan (short handle, float pan)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound != null)
			{
				if (channels[n].currentSound.handle == handle)
				{
					pan = Math.max(-1.0f, Math.min(1.0f, pan));
					channels[n].currentSound.pan = pan;
					channels[n].currentSound.updateVolume (-1);
				}
			}
		}
	}

	public void removeSound(CSound sound)
	{
		int n;
		for (n = 0; n < nChannels; n++)
		{
			if (channels[n].currentSound == sound)
			{
				channels[n].stop(true);
			}
		}
	}

	public void lockChannel(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			channels[channel].locked = true;
		}
	}

	public void unlockChannel(int channel)
	{
		if (channel >= 0 && channel < nChannels)
		{
			channels[channel].locked = false;
		}
	}

	public void tick ()
	{
		for (int i = 0; i < nChannels; ++ i)
			if (channels [i].currentSound != null)
				channels [i].currentSound.tick ();
	}

	/////////////////////////////////////
	//
	// Audio Manager Utilities - Focus
	//
	/////////////////////////////////////
	private boolean audioFocus;

	private OnAudioFocusChangeListener onAudioFocus = new OnAudioFocusChangeListener() {
		@Override
		public void onAudioFocusChange(int focusChange) {
			switch (focusChange) {
			case AudioManager.AUDIOFOCUS_GAIN:
				//Log.Log( "AUDIOFOCUS_GAIN");
				audioFocus = true;
				break;
			case AudioManager.AUDIOFOCUS_GAIN_TRANSIENT:
				//Log.Log( "AUDIOFOCUS_GAIN_TRANSIENT");
				audioFocus = true;
				break;
			case AudioManager.AUDIOFOCUS_GAIN_TRANSIENT_MAY_DUCK:
				//Log.Log( "AUDIOFOCUS_GAIN_TRANSIENT_MAY_DUCK");
				audioFocus = true;
				break;
			case AudioManager.AUDIOFOCUS_LOSS:
				//Log.Log( "AUDIOFOCUS_LOSS");
				audioFocus = false;
				break;
			case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT:
				//Log.Log( "AUDIOFOCUS_LOSS_TRANSIENT");
				audioFocus = false;
				break;
			case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT_CAN_DUCK:
				//Log.Log( "AUDIOFOCUS_LOSS_TRANSIENT_CAN_DUCK");
				audioFocus = false;
				break;
			case AudioManager.AUDIOFOCUS_REQUEST_FAILED:
				//Log.Log( "AUDIOFOCUS_REQUEST_FAILED");
				audioFocus = false;
				break;
			default:
				//
			}
		}
	};

	public AudioManager getAudioManager()
	{
		return (AudioManager)MMFRuntime.inst.getApplicationContext().getSystemService(Context.AUDIO_SERVICE);
	}

	public void getAudioFocus()
	{
		if(!AudioFocus) {
			synchronized (audioMgr) {
				audioMgr.requestAudioFocus(onAudioFocus, AudioManager.STREAM_MUSIC, AudioManager.AUDIOFOCUS_GAIN);
				AudioFocus = true;
			}
		}
	}

	public void resetAudioFocus()
	{
		synchronized (audioMgr) {
			audioMgr.abandonAudioFocus(onAudioFocus);
			AudioFocus = false;
		}
	}

	/////////////////////////////
	//
	//     SoundPool Listeners
	//
	/////////////////////////////
	public OnPoolLoadListener mPlayListener;

	public interface OnPoolLoadListener {
		public abstract void onLoad(SoundPool soundPool, int sampleId, int status);
		public abstract void setSound(CSound sound);
	}

	public void setOnPoolLoadListener(CSound sound, final OnPoolLoadListener listener) {
		mPlayListener = listener;
		mPlayListener.setSound(sound);
		//Log.Log("Listener set ...");
	}    

	@Override
	public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
		//Log.Log("OnLoadComplete --> sampleId: "+sampleId+" status: "+status+" ...");
		mPlayListener.onLoad(soundPool, sampleId, status);
	}

}

