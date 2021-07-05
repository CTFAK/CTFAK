//----------------------------------------------------------------------------------
//
// CSOUNDPLAYER : synthetiseur MIDI
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using RuntimeXNA.Banks;
using Microsoft.Xna.Framework.Media;

namespace RuntimeXNA.Application
{
    public class CSoundPlayer
    {
        const int NCHANNELS = 32;
        CRunApp app;
        CSound[] channels = null;
        bool bMultipleSounds = false;
        bool bOn = true;
        int[] volumes;
        bool[] bLocked;
        int[] pans;
        int mainVolume;
        int mainPan;

        public CSoundPlayer(CRunApp a)
        {
            app = a;
            channels = new CSound[NCHANNELS];
            volumes = new int[NCHANNELS];
            pans= new int[NCHANNELS];
            bLocked = new bool[NCHANNELS];
            bOn = true;
            bMultipleSounds = true;

            int n;
            for (n = 0; n < NCHANNELS; n++)
            {
                channels[n] = null;
                volumes[n] = 100;
                pans[n] = 0;
            }
            mainVolume = 100;
            mainPan = 0;
        }
        public void reset()
        {
            int n;
            for (n = 0; n < NCHANNELS; n++)
            {
                bLocked[n] = false;
            }
        }
        public void lockChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                bLocked[channel] = true;
            }
        }
        public void unlockChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                bLocked[channel] = false;
            }
        }
        public void play(short handle, int nLoops, int channel, bool bPrio)
        {
	        int n;
        	
	        if (bOn == false)
	        {
		        return;
	        }
        	
	        CSound sound = app.soundBank.getSoundFromHandle(handle);
	        if (sound == null)
	        {
		        return;
	        }
            if (bMultipleSounds == false)
            {
                channel = 0;
            }
            else
            {
                // Recherche un canal avec le son
                for (n = 0; n < NCHANNELS; n++)
                {
                    if (channels[n] == sound)
                    {
                        sound = CSound.createFromSound(sound);
                        break;
                    }
                }
            }
        	
	        // Lance le son
	        if (channel < 0)
	        {
		        for (n = 0; n < NCHANNELS; n++)
		        {
			        if (channels[n] == null && bLocked[n]==false)
			        {
				        break;
			        }
		        }
		        if (n == NCHANNELS)
		        {
			        // Stoppe le son sur un canal deja en route
			        for (n = 0; n < NCHANNELS; n++)
			        {
				        if (bLocked[n]==false)
				        {
					        if (channels[n] != null)
					        {
						        if (channels[n].bUninterruptible == false)
						        {
                                    break;
						        }
					        }
				        }
			        }
		        }
		        channel = n;
		        if (channel>=0 && channel< NCHANNELS)
		        {
			        volumes[channel]=mainVolume;
		        }
	        }
	        if (channel < 0 || channel >= NCHANNELS)
	        {
		        return;
	        }
	        if (channels[channel] != null)
	        {
		        if (channels[channel].bUninterruptible == true)
		        {
			        return;
		        }
                if (channels[channel] != sound)
                {
                    channels[channel].stop();
                }
            }
	        channels[channel] = sound;
	        sound.play(nLoops, bPrio, volumes[channel], pans[channel]);
        }

        public void setMultipleSounds(bool bMultiple)
        {
            bMultipleSounds = bMultiple;
        }

        public void keepCurrentSounds()
        {
            int n;
            for (n = 0; n < NCHANNELS; n++)
            {
                if (channels[n] != null)
                {
                    if (channels[n].isPlaying())
                    {
                        app.soundBank.setToLoad(channels[n].handle);
                    }
                }
            }
        }

        public void setOnOff(bool bState)
        {
            if (bState != bOn)
            {
                bOn = bState;
                if (bOn == false)
                {
                    stopAllSounds();
                }
            }
        }

        public bool getOnOff()
        {
            return bOn;
        }

        public void stopAllSounds()
        {
            int n;
            for (n = 0; n < NCHANNELS; n++)
            {
                if (channels[n] != null)
                {
                    channels[n].stop();
                    channels[n] = null;
                }
            }
        }

        public void stopSample(short handle)
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        channels[c].stop();
                        channels[c] = null;
                    }
                }
            }
        }

        public void stopChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    channels[channel].stop();
                    channels[channel] = null;
                }
            }
        }

        public bool isSamplePaused(short handle)
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        return channels[c].isPaused();
                    }
                }
            }
            return false;
        }

        public bool isSoundPlaying()
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    return channels[c].isPlaying();
                }
            }
            return false;
        }

        public bool isSamplePlaying(short handle)
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        return channels[c].isPlaying();
                    }
                }
            }
            return false;
        }

        public bool isChannelPlaying(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    return channels[channel].isPlaying();
                }
            }
            return false;
        }

        public bool isChannelPaused(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    return channels[channel].isPaused();
                }
            }
            return false;
        }

        public void pause()
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    channels[c].pause();
                }
            }
        }

        public void pauseChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    channels[channel].pause();
                }
            }
        }

        public void pauseSample(short handle)
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        channels[c].pause();
                    }
                }
            }
        }

        public void resume()
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    channels[c].resume();
                }
            }
        }

        public void resumeChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    channels[channel].resume();
                }
            }
        }

        public void resumeSample(short handle)
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        channels[c].resume();
                    }
                }
            }
        }

        public void setVolumeChannel(int channel, int volume)
        {
            if (volume<0) volume=0;
            if (volume > 100) volume = 100;

            if (channel >= 0 && channel < NCHANNELS)
            {
                volumes[channel] = volume;
                if (channels[channel] != null)
                {
                    channels[channel].setVolume(volume);
                }
            }
        }

        public void setFrequencyChannel(int channel, int frequency)
        {
            if (frequency<0) frequency = 100;
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    channels[channel].setFrequency(frequency);
                }
            }
        }

        public int getVolumeChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    return volumes[channel];
                }
            }
            return 0;
        }

        public int getFrequencyChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    return channels[channel].getFrequency();
                }
            }
            return 0;
        }

        public void setVolumeSample(short handle, int volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        volumes[c] = volume;
                        channels[c].setVolume(volume);
                    }
                }
            }
        }

        public void setFrequencySample(short handle, int frequency)
        {
            if (frequency < 0) frequency= 100;

            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        channels[c].setFrequency(frequency);
                    }
                }
            }
        }

        public void setMainVolume(int volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            mainVolume = volume;
            SoundEffect.MasterVolume=(float)(volume/100.0);
        }

        public int getMainVolume()
        {
            return mainVolume;
        }

        public int getMainPan()
        {
            return mainPan;
        }

        public void setPanChannel(int channel, int pan)
        {
            if (pan < -100) pan = -100;
            if (pan > 100) pan = 100;

            if (channel >= 0 && channel < NCHANNELS)
            {
                pans[channel] = pan;
                if (channels[channel] != null)
                {
                    channels[channel].setPan(pan);
                }
            }
        }

        public int getPanChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    return pans[channel];
                }
            }
            return 0;
        }

        public void setPanSample(short handle, int pan)
        {
            if (pan < -100) pan = -100;
            if (pan > 100) pan = 100;

            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].handle == handle)
                    {
                        pans[c] = pan;
                        channels[c].setPan(pan);
                    }
                }
            }
        }

        public void setMainPan(int pan)
        {
            if (pan < -100) pan = -100;
            if (pan > 100) pan = 100;

            mainPan = pan;

            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                pans[c] = pan;
                if (channels[c] != null)
                {
                    channels[c].setPan(pan);
                }
            }
        }

        int getChannel(string name)
        {
	        int c;
	        for (c = 0; c < NCHANNELS; c++)
	        {
		        if (channels[c] != null)
		        {
			        if (string.Compare(channels[c].name, name)==0)
			        {
				        return c;
			        }
		        }
	        }
	        return -1;
        }

        public int getDurationChannel(int channel)
        {
            if (channel >= 0 && channel < NCHANNELS)
            {
                if (channels[channel] != null)
                {
                    return channels[channel].getDuration();
                }
            }
            return 0;
        }

        public int getVolumeSample(string name)
        {
            int channel = getChannel(name);
            if (channel >= 0)
            {
                return volumes[channel];
            }
            return 0;
        }

        public int getDurationSample(string name)
        {
            int channel = getChannel(name);
            if (channel >= 0)
            {
                return channels[channel].getDuration();
            }
            return 0;
        }

        public int getPanSample(string name)
        {
            int channel = getChannel(name);
            if (channel >= 0)
            {
                return pans[channel];
            }
            return 0;
        }

        public int getFrequencySample(string name)
        {
            int channel = getChannel(name);
            if (channel >= 0)
            {
                return channels[channel].getFrequency();
            }
            return 0;
        }

        public void checkSounds()
        {
            int c;
            for (c = 0; c < NCHANNELS; c++)
            {
                if (channels[c] != null)
                {
                    if (channels[c].checkSound())
                    {
                        channels[c] = null;
                    }
                }
            }
        }
    }
}
