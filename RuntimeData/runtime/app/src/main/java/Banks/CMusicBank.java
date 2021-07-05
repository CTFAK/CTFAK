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
 // CMUSICBANK : Stockage des musiques
//
//----------------------------------------------------------------------------------
package Banks;

import Application.CMusicPlayer;
import Application.CRunApp;
import Services.CFile;

public class CMusicBank implements IEnum
{
    public CMusic musics[] = null;
    public int nHandlesReel;
    public int nMusics;
    short handleToIndex[];
    short useCount[];
    public CMusicPlayer mPlayer;

    public CMusicBank(CMusicPlayer m)
    {
        mPlayer=m;
    }

    public void preLoad(CFile f)
    {
        // Nombre de handles
        nHandlesReel = f.readAShort();

        // Reservation des tables
        handleToIndex = new short[nHandlesReel];
        int n;
        for (n = 0; n < nHandlesReel; n++)
        {
            handleToIndex[n] = -1;
        }
        useCount = new short[nHandlesReel];
        resetToLoad();
        nMusics = 0;
        musics = null;
    }

    public CMusic getMusicFromHandle(short handle)
    {
        if (musics != null)
        {
            if (handle >= 0 && handle < nHandlesReel)
            {
                if (handleToIndex[handle] != -1)
                {
                    return musics[handleToIndex[handle]];
                }
            }
        }
        return null;
    }

    public CMusic getMusicFromIndex(short index)
    {
        if (musics != null)
        {
            if (index >= 0 && index < nMusics)
            {
                return musics[index];
            }
        }
        return null;
    }

    public void resetToLoad()
    {
        int n;
        for (n = 0; n < nHandlesReel; n++)
        {
            useCount[n] = 0;
        }
    }

    public void setToLoad(short handle)
    {
        useCount[handle]++;
    }

    // Entree enumeration
    @Override
	public short enumerate(short num)
    {
        setToLoad(num);
        return -1;
    }

    public void load(CRunApp app)
    {
        int h;

        // Combien de musiques?
        nMusics = 0;
        for (h = 0; h < nHandlesReel; h++)
        {
            if (useCount[h] != 0)
            {
                nMusics++;
            }
            else
            {
            	if (handleToIndex[h]!=-1)
            	{
            		if (musics[handleToIndex[h]]!=null)
            		{
            			musics[handleToIndex[h]].release();
            		}
            	}
            }
        }
        
        // Charge les images
        CMusic newMusics[] = new CMusic[nMusics];
        int count = 0;
        for (h = 0; h < nHandlesReel; h++)
        {
            if (useCount[h] != 0)
            {
                if (musics != null && handleToIndex[h] != -1 && musics[handleToIndex[h]] != null)
                {
                    newMusics[count] = musics[handleToIndex[h]];
                }
                else
                {
                    newMusics[count] = new CMusic(mPlayer);
                    newMusics[count].load((short)h, app);
                }
                count++;
            }
        }
        musics = newMusics;

        // Cree la table d'indirection
        handleToIndex = new short[nHandlesReel];
        for (h = 0; h < nHandlesReel; h++)
        {
            handleToIndex[h] = -1;
        }
        for (h = 0; h < nMusics; h++)
        {
            handleToIndex[musics[h].handle] = (short) h;
        }

        // Plus rien a charger
        resetToLoad();
    }
    
}
