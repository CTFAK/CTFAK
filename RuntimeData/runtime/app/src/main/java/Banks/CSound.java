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

import android.content.res.AssetFileDescriptor;
import android.content.res.Resources;
import android.media.SoundPool;
import android.net.Uri;
import android.os.Handler;
import android.os.SystemClock;

import java.io.File;
import java.io.IOException;
import java.util.Locale;

import Application.CRunApp;
import Application.CSoundPlayer;
import Runtime.Log;
import Runtime.MMFRuntime;
import Services.CServices;


public class CSound {
    public CRunApp application;
    public CSoundPlayer sPlayer;

    public short handle = -1;

    public boolean bUninterruptible = false;

    public MediaSound mediaSound = null;

    public int length;
    public int position;

    public int soundID = -1;
    public int streamID = -1;

    public long streamStart;
    public long streamDuration;
    public long streamDelay;
    public float streamRate;

    public int resID;
    Uri uri;
    AssetFileDescriptor fd;

    protected boolean bPaused;
    protected boolean bPlaying;
    protected boolean bGPaused;
    protected boolean bPrepared;
    protected boolean bPlayed;
    protected boolean bLoaded;

    public int mRand = 0;

    public int nLoops = 1;

    public float volume = 1.0f;
    public float pan = 0.0f;

    public int origFrequency = -1;
    public int frequency = -1;
    public String soundName;
    public int flags;

    public final long soundStart;

    private Handler mHandlerOffPool = new Handler();

    private Runnable poolKiller = new Runnable() {

        @Override
        public void run() {
            tick();
            bPlaying = false;
        }

    };

    ////////////////////////////////////////////////////////////////
    //
    //              SoundPool Listener for each sound 
    //                 interface with SoundPlayer
    //
    ////////////////////////////////////////////////////////////////

    public CSoundPlayer.OnPoolLoadListener soundPoolLoadListener = new CSoundPlayer.OnPoolLoadListener() {

        CSound sound = null;

        @Override
        public void onLoad(SoundPool soundPool, int sampleId, int status) {

            if (status == 0) {
                Log.Log("Loaded Id: " + sampleId + " and took: " + (SystemClock.currentThreadTimeMillis() - sound.soundStart) + " msecs");
                if (sampleId == sound.soundID) {
                    if (sound.streamID <= 0 && !sound.bLoaded && sound.bPlaying) {
                        final float[] volumes = getVolumes(sound.volume);
                        sound.streamRate = getRate();

                        int streamDelay = 150;

                        sound.streamDuration = Math.round((1.0f / sound.streamRate) * ((sound.length * sound.nLoops) + streamDelay));    //Supporting rate less than 1
                        sound.streamID = sPlayer.soundPool.play(sampleId, volumes[0], volumes[1], 0, sound.nLoops <= 0 ? -1 : sound.nLoops - 1, sound.streamRate);
                        sound.streamStart = (sound.nLoops == 0 ? -1 : System.currentTimeMillis());
                        sound.bLoaded = true;
                        sound.mHandlerOffPool.postDelayed(sound.poolKiller, (sound.streamID != 0 ? 1 : 4) * sound.streamDuration);
                        if (bPaused && streamID > 0)
                            sPlayer.soundPool.pause(sound.streamID);

                    } else
                        sound.resume();
                }
            }
        }

        @Override
        public void setSound(CSound sound) {
            this.sound = sound;
        }

    };

    public float[] getVolumes(float sVolume) {
        float volume;
        if (sVolume < 0)
            volume = Math.abs(sPlayer.getVolume() * this.volume);
        else
            volume = (sPlayer.getVolume() * sVolume);

        float pan = sPlayer.getPan() + this.pan;

        float leftMod = 1.0f, rightMod = 1.0f;

        pan = Math.min(1.0f, Math.max(-1.0f, pan));

        if (pan >= 0.0f)
            leftMod = 1.0f - (pan);

        if (pan <= 0.0f)
            rightMod = 1.0f - (-pan);

        float volumes[] = new float[2];

        volumes[0] = volume * leftMod;
        volumes[1] = volume * rightMod;

        //Log.Log("pan: "+pan +" pan left: "+leftMod+" pan right: "+rightMod);
        this.volume = sVolume;

        return volumes;
    }

    public float getRate() {
        if (frequency <= 0)
            return 1.0f;

        float rate = ((float) frequency) / ((float) origFrequency);

        if (rate < 0.5)
            rate = 0.5f;
        if (rate > 2.0)
            rate = 2.0f;
        return rate;
    }

    public void setVolume(float volume) {
        //Log.Log("Setting volume to : "+volume);
        this.volume = volume;
        soundPoolLoadListener.setSound(this);
    }

    public void updateVolume(float volume) {
        //Log.Log("Updating volume to : "+volume);
        float[] volumes = getVolumes(volume);

        if (soundID == -1) {
            if (mediaSound != null)
                mediaSound.setVolume(volumes);
        } else {
            if (streamID != -1) {
                if (volume == -1) {
                    float newRate = getRate();
                    //streamDuration = (long) ((streamDuration * ((1.0f - streamRate) + 1.0f)) * ((1.0f - newRate) + 1.0f));
                    streamDuration = (long) ((streamDuration / (newRate)));
                    sPlayer.soundPool.setRate(streamID, newRate);
                    streamRate = newRate;
                    if (mHandlerOffPool.getLooper() != null) {
                        mHandlerOffPool.removeCallbacks(poolKiller);
                        mHandlerOffPool.postDelayed(poolKiller, streamDuration);
                    }
                } else {
                    sPlayer.soundPool.setVolume(streamID, volumes[0], volumes[1]);
                }

            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    //
    //	CSound
    //
    /////////////////////////////////////////////////////////////////////////////////////////////
    public CSound(CSoundPlayer p, String name, short handle, int soundID, int frequency, int length, int flags) {
        sPlayer = p;

        this.soundID = soundID;

        this.length = length;

        this.volume = 1.0f;
        this.pan = 0.0f;
        this.frequency = frequency;
        this.origFrequency = frequency;

        this.handle = handle;

        this.soundName = name;
		//Log.Log("New Sound: " + this.soundName + ", freq: " + this.frequency);

        this.flags = flags;

        this.soundStart = SystemClock.currentThreadTimeMillis();

        if (soundID != -1)
            sPlayer.setOnPoolLoadListener(this, soundPoolLoadListener);

        initSoundsFlags();
        //Log.Log("New Sound in full");
    }

    public CSound(CSound sound) {
        sPlayer = sound.sPlayer;

        resID = sound.resID;
        uri = sound.uri;
        fd = sound.fd;

        nLoops = sound.nLoops;

        soundID = sound.soundID;

        volume = sound.volume;
        pan = sound.pan;
        frequency = sound.frequency;
        origFrequency = sound.origFrequency;
        length = sound.length;
        soundName = sound.soundName;

        flags = sound.flags;
        bUninterruptible = sound.bUninterruptible;
        handle = sound.handle;
        soundStart = sound.soundStart;

        soundPoolLoadListener = sound.soundPoolLoadListener;

        //Log.Log("create Sound from sound handler ...");

        if (sound.soundID != -1)
            sPlayer.setOnPoolLoadListener(this, soundPoolLoadListener);

        initSoundsFlags();
        //Log.Log("New Sound from copy");
    }

    public CSound(CSoundPlayer p, String filename) {
        sPlayer = p;

        if (filename.contains("http") || filename.contains("rtsp") || filename.contains("file")) {
            uri = Uri.parse(filename.trim());
        } else if ((uri = CServices.filenameToURI(filename)) == null)
            throw new RuntimeException("Can't open file");


        if(uri.toString().contains("file:") && !uri.toString().contains("android_asset")) {
            File file = new File(uri.getPath());
            if (!file.isFile())
                throw new RuntimeException("Non existing file");
        }

        soundName = filename;

        soundStart = 0;
        resID = 0;

        initSoundsFlags();
    }


    public void load(short h, CRunApp app) {
        handle = h;
        application = app;
        resID = 0;

        if (!MMFRuntime.inst.obbAvailable)
            resID = MMFRuntime.inst.getResourceID(String.format(Locale.US, "raw/s%04d", h));

    }

    private void reload(short h) {
        if (fd == null && MMFRuntime.inst.obbAvailable)
            fd = MMFRuntime.inst.getSoundFromAPK(String.format(Locale.US, "res/raw/s%04d", h));
    }

    private void setPreparedAndLoaded() {
        bPrepared = true;
        bLoaded = true;
    }

    private void initSoundsFlags() {
        bPaused = true;
        bGPaused = false;
        bPrepared = false;
        bLoaded = false;
        bPlaying = false;

    }

    private void createPlayer() {
        if (soundID == -1) {
            reload(handle);
            mediaSound = new MediaSound(MMFRuntime.inst, getVolumes(volume), handle);
            if(mediaSound != null)
            {
                mediaSound.setOnMediaSoundCompletionListener(new MediaSound.MediaSoundListener() {
                    @Override
                    public void OnMediaSoundCompletion() {
                        //Log.Log("On completion ...");
                        removeSound();
                        bPlaying = false;
                        return;
                    }

                });

                try {
                    if(uri != null && uri.getPath().contains("android_asset"))
                    {
                        String lfile = uri.getLastPathSegment();
                        fd = MMFRuntime.inst.getAssets().openFd(lfile);
                        mediaSound.Create(fd, nLoops);
                    } else if (uri != null && (uri.getScheme().contains("http") || uri.getScheme().contains("rtsp") || uri.getScheme().contains("file"))) {
                        mediaSound.Create(uri, nLoops);
                    }  else if (resID != 0) {
                        mediaSound.Create(resID, nLoops);
                    } else if (fd != null) {
                        mediaSound.Create(fd, nLoops);
                    }
                } catch (IOException e) {
                    e.printStackTrace();
                } catch (Resources.NotFoundException e) {
                    e.printStackTrace();
                }

                setPreparedAndLoaded();
            }
         }
    }

    private void removeSound() {
        sPlayer.removeSound(this);
    }

    public void setLoopCount(int count) {
        if (count <= 0)
            this.nLoops = 0;
        else
            this.nLoops = count;
    }

    public void setUninterruptible(boolean bFlag) {
        this.bUninterruptible = bFlag;
    }

    public void start() {
        bPrepared = false;
        bPaused = false;
        bPlaying = true;
        final float[] volumes = getVolumes(volume);

        if (soundID == -1) {
            Log.Log("MediaSound with handle: "+handle);
            if (mediaSound == null)
                createPlayer();

            if (mediaSound != null && bLoaded) {
                mediaSound.setVolume(volumes);
                mediaSound.play(nLoops);
            } else
                bPrepared = false;
        } else {
             try {
                streamRate = getRate();

                streamDelay = 150;

                streamDuration = Math.round((1.0f / streamRate) * ((length * nLoops) + streamDelay));    //Supporting rate less than 1

                //Log.Log("duration set: "+streamDuration+" Rate: "+streamRate+" with a frequency: "+frequency+ " and original of: "+origFrequency);
                streamID = sPlayer.soundPool.play(soundID, volumes[0], volumes[1], 0, nLoops <= 0 ? -1 : nLoops - 1, streamRate);

                if (streamID <= 0) {
                    bPrepared = false;
                    bLoaded = false;
                    Log.Log("asking to play but not ready ...");
                } else {
                    bLoaded = true;
                    mHandlerOffPool.postDelayed(poolKiller, streamDuration);
                }

                //Log.Log("stream ID: " + streamID + " Playing sound ID: " + soundID);

                streamStart = (nLoops == 0 ? -1 : System.currentTimeMillis());

                soundPoolLoadListener.setSound(this);

            } catch (Exception e) {
                e.printStackTrace();
                Log.Log("postponed ...");
            }
        }

    }

    public void stop() {
        if (soundID == -1) {
            if (mediaSound != null) {
                mediaSound.release();
                mediaSound = null;
                Log.Log("Destroying ...");
                bPrepared = false;
            }
        } else {
            if (streamID != -1) {
                try {
                    sPlayer.soundPool.stop(streamID);
                } catch (Exception e) {
                    Log.Log(e.toString());
                }
                streamID = -1;
                bLoaded = false;
            }
        }
        //Log.Log("stopping ...");
        bPlaying = false;
    }

    public boolean isPlaying() {
        if (soundID == -1) {
            try {
                return bPlaying;
            } catch (IllegalStateException e) {
                Log.Log("Checking if playing but " + e);
                return false;
            }
        } else {
            return (streamID != -1);
        }
    }

    public boolean isPaused() {
        return bPaused;
    }

    public void pause() {
        if (soundID == -1) {
            if (mediaSound != null) {
                if (bLoaded)
                    mediaSound.pause();
                bPaused = true;
            }
        } else {
            if (streamID != -1) {
                sPlayer.soundPool.pause(streamID);
                bPaused = true;
            }

        }
        Log.Log("pausing ...");
    }

    public void resume() {
        if (soundID == -1) {
            if (mediaSound != null && bPaused) {
                if (bLoaded) {
                    final float[] volumes = getVolumes(volume);
                    mediaSound.setVolume(volumes);
                    mediaSound.play(-1);
                    //Log.Log("resumed handle: "+this.handle);
                }
                bPaused = false;
            }
        } else {
            if (streamID != -1) {
                sPlayer.soundPool.resume(streamID);
                bPaused = false;
            }
        }
        Log.Log("resuming ...");
    }

    // Runtime Pause
    public void pause2() {
        if (soundID == -1) {
            if (mediaSound != null && mediaSound.isPlaying()) {
                mediaSound.pause();
                bGPaused = true;
            }
        } else {
            if (streamID != -1) {
                sPlayer.soundPool.pause(streamID);
                bGPaused = true;
            }

        }
        Log.Log("pausing 2 ...");
    }

    // Runtime Resume
    public void resume2() {
        if (soundID == -1) {
            if (mediaSound != null && bGPaused) {
                bGPaused = false;
                if (bLoaded) {
                    final float[] volumes = getVolumes(volume);
                    mediaSound.setVolume(volumes);
                    mediaSound.play(-1);
                }
            }
        } else {
            if (streamID != -1) {
                bGPaused = false;
                sPlayer.soundPool.resume(streamID);
            }
        }
        Log.Log("resuming 2 ...");
    }

    public int getDuration() {
        if (soundID == -1) {
            if (mediaSound == null || !bPrepared)
                return length;

            return mediaSound.getDuration();
        } else {
            return length;
        }
    }

    public void setPosition(int position) {
        if (soundID == -1) {
            this.position = -1;
            try {
                if (mediaSound != null) {
                    if (bPrepared) {
                        mediaSound.seekTo(position);
                    } else
                        this.position = position;
                }
            } catch (IllegalStateException e) {
                Log.Log("Set position but " + e);
            }
        }
    }

    public int getPosition() {
        if (soundID == -1) {
            try {
                if (mediaSound != null && bPrepared)
                    return mediaSound.getCurrentPosition();
            } catch (IllegalStateException e) {
                Log.Log("Checking the position but " + e);
            }
        } else {
            if (streamID != -1)
                return -1;
        }

        return 0;
    }

    public void release() {

        bPaused = false;
        bGPaused = false;
        bPrepared = false;
        bPlaying = false;

        if (mediaSound != null) {
            mediaSound.stop();
            mediaSound.release();
            mediaSound = null;
            soundName = null;
            fd = null;
            Log.Log("Release ...");
            bPrepared = false;
        } else {
            if (streamID != -1) {
                sPlayer.soundPool.stop(streamID);
                streamID = -1;
                Log.Log("streamID: " + streamID);
            }
        }
        //Log.Log("released");

    }

    public void tick() {
        if (streamID != -1 && streamStart != -1) {
            sPlayer.removeSound(this);
            bPlaying = false;
            streamID = -1;
            //Log.Log("sound was tick and remove ...");
        }
    }
}
