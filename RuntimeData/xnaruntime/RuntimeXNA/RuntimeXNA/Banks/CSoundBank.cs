//----------------------------------------------------------------------------------
//
// CSOUNDBANK : Stockage des sons
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Banks
{
    public class CSoundBank : IEnum
    {
	    CRunApp app;
	    CSound[] sounds=null;
	    int nHandlesReel;
	    int nHandlesTotal;
	    int nSounds;
	    int[] offsetsToSounds;
	    int[] handleToIndex;
	    int[] useCount;
//        int enumIndex;

	    public void preLoad(CRunApp a)
	    {
            app=a;
		
			// Nombre de handles
			nHandlesReel=app.file.readAShort();
			offsetsToSounds=new int[nHandlesReel];
			
			// Repere les positions des images
			int nSons=app.file.readAShort();
			int n;
			CSound sound=new CSound(a);
			int offset;
			for (n=0; n<nSons; n++)
			{
				offset=app.file.getFilePointer();
			    sound.loadHandle();
			    offsetsToSounds[sound.handle]=offset;
			}
			
			// Reservation des tables
			useCount=new int[nHandlesReel];
			resetToLoad();
			handleToIndex=null;
			nHandlesTotal=nHandlesReel;
			nSounds=0;
			sounds=null;
	    }

	    public CSound getSoundFromHandle(short handle)
	    {
			if (handle>=0 && handle<nHandlesTotal)
			    if (handleToIndex[handle]!=-1)
				return sounds[handleToIndex[handle]];
			return null;
	    }
	    public CSound getSoundFromIndex(int index)
	    {
			if (index>=0 && index<nSounds)
			    return sounds[index];
			return null;
	    }
/*      public CSound getFirstSound()
        {
            for (enumIndex = 0; enumIndex < nSounds; enumIndex++)
            {
                if (sounds[enumIndex] != null)
                {
                    return sounds[enumIndex];
                }
            }
            return null;
        }
        public CSound getNextSound()
        {
            for (enumIndex++; enumIndex < nSounds; enumIndex++)
            {
                if (sounds[enumIndex] != null)
                {
                    return sounds[enumIndex];
                }
            }
            return null;
        }
*/
        public void resetToLoad()
	    {
			int n;
			for (n=0; n<nHandlesReel; n++)
			{
			    useCount[n]=0;
			}
	    }	    
	    public void setToLoad(short handle)
	    {
			useCount[handle]++;
	    }

	    public short enumerate(short num)
	    {
			setToLoad(num);
			return -1;
	    }

	    public void load()
	    {
			int n;
			
			// Combien d'images?
			nSounds=0;
			for (n=0; n<nHandlesReel; n++)
			{
			    if (useCount[n]!=0)
					nSounds++;
			}
		
			// Charge les images
			CSound[] newSounds=new CSound[nSounds];
			int count=0;
			int h;
			for (h=0; h<nHandlesReel; h++)
			{
			    if (useCount[h]!=0)
			    {
					if (sounds!=null && handleToIndex[h]!=-1 && sounds[handleToIndex[h]]!=null)
					{
					    newSounds[count]=sounds[handleToIndex[h]];
					    newSounds[count].useCount=useCount[h];
					}
					else
					{
					    newSounds[count]=new CSound(app);
                        app.file.seek(offsetsToSounds[h]);
                        newSounds[count].load();
					    newSounds[count].useCount=useCount[h];
					}
					count++;
			    }
			}
			sounds=newSounds;
		
			// Cree la table d'indirection
			handleToIndex=new int[nHandlesReel];
			for (n=0; n<nHandlesReel; n++)
			{
			    handleToIndex[n]=-1;
			}
			for (n=0; n<nSounds; n++)
			{
			    handleToIndex[sounds[n].handle]=n;
			}
			nHandlesTotal=nHandlesReel;
			
			// Plus rien a charger
			resetToLoad();
	    }

    }
}
