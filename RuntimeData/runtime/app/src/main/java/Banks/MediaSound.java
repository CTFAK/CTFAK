package Banks;

import android.content.Context;
import android.content.res.AssetFileDescriptor;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.media.MediaPlayer.OnErrorListener;
import android.media.MediaPlayer.OnPreparedListener;
import android.net.Uri;
import android.os.SystemClock;
import android.util.Log;

import java.io.IOException;
import java.lang.reflect.Method;

public class MediaSound {

    private Context context;
    private Uri uri;
    private int resourceId;
    private AssetFileDescriptor fd;
    private int handle;

    // which file is getting played
    public static final int URI_PLAYING = 1;
    public static final int RESOURCE_PLAYING = 2;
    public static final int ASSET_PLAYING = 3;
    private int filePlaying;

    // states of the media player
    public static final int STATE_PLAYING = 1;
    public static final int STATE_PAUSED = 2;
    public static final int STATE_STOP = 3;
    public static final int STATE_LOADING = 4;

    // current state
    private int state = STATE_STOP;

    // current mediaPlayer which is playing
    private int mediaPlayerIndex = -1;

    // 3 media players
    private MediaPlayer mp[] = new MediaPlayer[3];

    // current volumes
    private float[] volumes;

    private int nLoops;

    public MediaSound(Context context, float[] vol,  int handle) {
        this.context = context;
        this.volumes = vol;
        this.handle  = handle;
        this.mediaSoundListener = null;
    }

    /**
     * plays the provided uri
     *
     * @param uri
     */
    public void Create(Uri uri, int nLoops) {
        this.uri = uri;
        this.nLoops = nLoops != 0 ? nLoops : Integer.MAX_VALUE;
        filePlaying = URI_PLAYING;
        stop();
        // initialize and set listener to three media players
        int len = (nLoops == 1 ? 1 : mp.length);
        for (int i = 0; i < len; i++) {
            mp[i] = MediaPlayer.create(context, uri);
            mp[i].setOnCompletionListener(completionListener);
            mp[i].setOnErrorListener(errorListener);
            mp[i].setVolume(volumes[0], volumes[1]);
        }

        if (len != 1) {
            mp[0].setNextMediaPlayer(mp[1]);
            mp[1].setNextMediaPlayer(mp[2]);
        }
        mediaPlayerIndex = 0;
        state = STATE_PLAYING;
    }

    /**
     * play file from resource
     *
     * @param resourceId
     */
    public void Create(int resourceId, int nLoops) {
        this.resourceId = resourceId;
        this.nLoops = nLoops != 0 ? nLoops : Integer.MAX_VALUE;
        filePlaying = RESOURCE_PLAYING;
        stop();
        int len = (nLoops == 1 ? 1 : mp.length);
        try {
            AssetFileDescriptor afd = context.getResources().openRawResourceFd(resourceId);
            if (afd == null)
                return;
            for (int i = 0; i < len; i++) {
                if (mp[i] == null)
                    mp[i] = new MediaPlayer();
                if (mp[i] != null) {
                    mp[i].setDataSource(afd.getFileDescriptor(), afd.getStartOffset(), afd.getLength());
                    mp[i].setAudioStreamType(AudioManager.STREAM_MUSIC);
                    mp[i].setOnCompletionListener(completionListener);
                    mp[i].setOnErrorListener(errorListener);
                    mp[i].setVolume(volumes[0], volumes[1]);
                    mp[i].prepare();
                }
            }

            if (len != 1) {
                mp[0].setNextMediaPlayer(mp[1]);
                mp[1].setNextMediaPlayer(mp[2]);
            }
            afd.close();
        } catch (IOException e) {
            if (mp[0] != null)
                mp[0].release();
            if (len != 1) {
                if (mp[1] != null)
                    mp[1].release();
                if (mp[2] != null)
                    mp[2].release();
            }
        }
        mediaPlayerIndex = 0;
        state = STATE_PLAYING;
    }

    /**
     * play file from Assets
     *
     * @param fd
     */
    public void Create(AssetFileDescriptor fd, int nLoops) {
        this.fd = fd;
        this.nLoops = nLoops != 0 ? nLoops : Integer.MAX_VALUE;
        filePlaying = ASSET_PLAYING;
        stop();
        int len = (nLoops == 1 ? 1 : mp.length);
        try {
            for (int i = 0; i < len; i++) {
                if (mp[i] == null)
                    mp[i] = new MediaPlayer();
                if(mp[i] != null) {
                    mp[i].setDataSource(fd.getFileDescriptor(), fd.getStartOffset(), fd.getLength());
                    mp[i].setAudioStreamType(AudioManager.STREAM_MUSIC);
                    mp[i].setOnCompletionListener(completionListener);
                    mp[i].setOnErrorListener(errorListener);
                    mp[i].setVolume(volumes[0], volumes[1]);
                    mp[i].prepare();
                }
            }
            if (len != 1) {
                mp[0].setNextMediaPlayer(mp[1]);
                mp[1].setNextMediaPlayer(mp[2]);
            }
            //fd.close();
        } catch (IOException e) {
            if (mp[0] != null)
                mp[0].release();
            if (len != 1) {
                if (mp[1] != null)
                    mp[1].release();
                if (mp[2] != null)
                    mp[2].release();
            }
        }
        mediaPlayerIndex = 0;
        state = STATE_PLAYING;
    }

    public boolean isPlaying() {
        return state == STATE_PLAYING;
    }

    /**
     * play if the mediaplayer is pause
     */
    public void play(int loops) {
        if (loops > -1)
            this.nLoops = loops;
        int cnt = 0;
        while (cnt < 8) {
            try {
                if (state == STATE_PAUSED || state == STATE_PLAYING
                        && mp[mediaPlayerIndex].getAudioSessionId() != -1) {
                    mp[mediaPlayerIndex].start();
                    mp[mediaPlayerIndex].setVolume(volumes[0], volumes[1]);
                    Log.d("MediaSound", "Playing - handle: "+handle);
                    state = STATE_PLAYING;
                    break;
                }
            } catch (Exception e) {
                Log.d("MediaSound", "illegal state when about to play");
                cnt++;
                SystemClock.sleep(17);
            }

        }
    }

    /**
     * pause current playing session
     */
    public void pause() {
        try {
            if (state == STATE_PLAYING) {
                mp[mediaPlayerIndex].pause();
                Log.d("MediaSound", "pausing");
                state = STATE_PAUSED;
            }
        } catch (IllegalStateException e) {
            Log.d("MediaSound", "illegal state when pausing");
        }
    }

    public void release() {
        for (int i = 0; i < mp.length; i++) {
            try {
                if (mp[i] != null && mp[i].isPlaying()) {
                    mp[i].stop();
                    mp[i].release();
                    mp[i] = null;
                }
            } catch (IllegalStateException e) {
                Log.d("MediaSound", "illegal state when releasing");
            }
        }
        state = STATE_STOP;
        if (fd != null) {
            try {
                fd.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }


    /**
     * get current state
     *
     * @return
     */
    public int getState() {
        return state;
    }

    /**
     * stop every MediaPlayer
     */
    public void stop() {
        for (int i = 0; i < mp.length; i++) {
            try {
                if (mp[i] != null) {
                    mp[i].stop();

                    if (mp[i].isPlaying()) {
                        mp[i].release();
                    }
                }
            } catch (IllegalStateException e) {
                Log.d("MediaSound", "illegal state when stopping");
            }
        }
        state = STATE_STOP;
    }

    /**
     * set volume for every MediaPlayer
     *
     * @param volumes
     */
    public void setVolume(float[] volumes) {
        if(volumes[0] == this.volumes[0]
                && volumes[1] == this.volumes[1])
            return;

        this.volumes = volumes;
        for (int i = 0; i < mp.length; i++) {
            try {
                if (mp[i] != null && mp[i].isPlaying()) {
                    mp[i].setVolume(volumes[0], volumes[1]);
                }
            } catch (Exception e) {
                continue;
            }
        }
    }

    public int getDuration() {
        return mp[mediaPlayerIndex].getDuration();
    }

    public int getCurrentPosition() {
        int position = 0;
        if (state == STATE_PLAYING || state == STATE_PAUSED) {
            position = mp[mediaPlayerIndex].getCurrentPosition();
            Log.d("MediaSound", "get position");
        }
        return position;
    }

    public void seekTo(int position) {
        // New SeekTo with more precision from API 26
        try {
            Method m = mp[mediaPlayerIndex].getClass().getMethod("seekTo", new Class[]{Long.class, Integer.class});
            Object args[] = {position, 3};
            m.invoke(null, args);
        } catch (Exception e) {
            //Regular SeekTo
            mp[mediaPlayerIndex].seekTo(position);
        }

    }

    public void setLoops(int loop) {
        this.nLoops = loop;
    }

    /**
     * external Interface for MediaSound
     */

    public interface MediaSoundListener {
        public void OnMediaSoundCompletion();
    }

    private MediaSoundListener mediaSoundListener;

    public void setOnMediaSoundCompletionListener(MediaSoundListener msl) {
        mediaSoundListener = msl;
    }

    /**
     * internal listener which handles looping thing
     */
    private MediaPlayer.OnCompletionListener completionListener = new OnCompletionListener() {

        @Override
        public void onCompletion(MediaPlayer curmp) {
            int mpEnds = 0;
            int mpPlaying = 0;
            int mpNext = 0;
            if (curmp == mp[0]) {
                mpEnds = 0;
                mpPlaying = 1;
                mpNext = 2;
            } else if (curmp == mp[1]) {
                mpEnds = 1;
                mpPlaying = 2;
                mpNext = 0;
            } else if (curmp == mp[2]) {
                mpEnds = 2;
                mpPlaying = 0;
                mpNext = 1;
            }

            if (nLoops != 0 && (--nLoops) == 0) {
                stop();
                mediaSoundListener.OnMediaSoundCompletion();
                return;
            }

            mediaPlayerIndex = mpPlaying;
            Log.d("MediaSound", "Looping - handle: "+ handle +" ending loop " + mpEnds);
            try {
                if (mp[mpNext] != null) {
                    mp[mpNext].release();
                }

                switch (filePlaying) {
                    case URI_PLAYING:
                        mp[mpNext] = MediaPlayer.create(context, uri);
                        break;
                    case ASSET_PLAYING:
                        mp[mpNext] = new MediaPlayer();
                        mp[mpNext].setDataSource(fd.getFileDescriptor(), fd.getStartOffset(), fd.getLength());
                        mp[mpNext].prepare();
                        break;
                    case RESOURCE_PLAYING:
                    {
                        AssetFileDescriptor afd = context.getResources().openRawResourceFd(resourceId);
                        if (afd == null)
                            return;
                        mp[mpNext] = new MediaPlayer();
                        mp[mpNext].setDataSource(afd.getFileDescriptor(), afd.getStartOffset(), afd.getLength());
                        mp[mpNext].prepare();
                        afd.close();
                        //mp[mpNext] = MediaPlayer.create(context, resourceId);
                        break;
                    }
                }

                mp[mpNext].setOnCompletionListener(this);
                mp[mpNext].setOnErrorListener(errorListener);
                mp[mpNext].setVolume(volumes[0], volumes[1]);
                mp[mpPlaying].setNextMediaPlayer(mp[mpNext]);
                mp[mpPlaying].setVolume(volumes[0], volumes[1]);
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    };

    private MediaPlayer.OnErrorListener errorListener = new OnErrorListener() {
        @Override
        public boolean onError(MediaPlayer mp, int what, int extra) {

            mp.reset();

            String szWhat;
            switch (what) {
                case MediaPlayer.MEDIA_ERROR_UNKNOWN:
                    szWhat = "MEDIA_ERROR_UNKNOWN";
                    break;
                case MediaPlayer.MEDIA_ERROR_SERVER_DIED:
                    szWhat = "MEDIA_ERROR_SERVER_DIED";
                    break;
                default:
                    szWhat = "NOT DEFINED";
            }

            String szExtra;
            switch (extra) {
                case MediaPlayer.MEDIA_ERROR_IO:
                    szExtra = "MEDIA_ERROR_IO";
                    break;
                case MediaPlayer.MEDIA_ERROR_MALFORMED:
                    szExtra = "MEDIA_ERROR_MALFORMED";
                    break;
                case MediaPlayer.MEDIA_ERROR_UNSUPPORTED:
                    szExtra = "MEDIA_ERROR_UNSUPPORTED";
                    break;
                case MediaPlayer.MEDIA_ERROR_TIMED_OUT:
                    szExtra = "MEDIA_ERROR_TIMED_OUT";
                    break;
                default:
                    szExtra = "NOT DEFINED";
            }

            Log.d("MediaSound", "Error: " + szWhat + ", " + szExtra + " ...");
            return false;
        }
    };

    private MediaPlayer.OnPreparedListener preparedListener = new OnPreparedListener() {
        @Override
        public void onPrepared(MediaPlayer mp) {
            if (state == STATE_PLAYING)
                mp.start();
        }
    };
}