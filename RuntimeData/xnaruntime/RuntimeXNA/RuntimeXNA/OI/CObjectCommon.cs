//----------------------------------------------------------------------------------
//
// COBJECTCOMMON : Données d'un objet normal
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;
using RuntimeXNA.Values;
using RuntimeXNA.Animations;
using RuntimeXNA.Movements;

namespace RuntimeXNA.OI
{
    public class CObjectCommon : COC
    {
        // Flags 
        public const int OEFLAG_DISPLAYINFRONT=0x0001;
        public const int OEFLAG_BACKGROUND=0x0002;
        public const int OEFLAG_BACKSAVE=0x0004;
        public const int OEFLAG_RUNBEFOREFADEIN=0x0008;
        public const int OEFLAG_MOVEMENTS=0x0010;
        public const int OEFLAG_ANIMATIONS=0x0020;
        public const int OEFLAG_TABSTOP=0x0040;
        public const int OEFLAG_WINDOWPROC=0x0080;
        public const int OEFLAG_VALUES=0x0100;
        public const int OEFLAG_SPRITES=0x0200;
        public const int OEFLAG_INTERNALBACKSAVE=0x0400;
        public const int OEFLAG_SCROLLINGINDEPENDANT=0x0800;
        public const int OEFLAG_QUICKDISPLAY=0x1000;
        public const int OEFLAG_NEVERKILL=0x2000;
        public const int OEFLAG_NEVERSLEEP=0x4000;
        public const int OEFLAG_MANUALSLEEP=0x8000;
        public const int OEFLAG_TEXT=0x10000;
        public const int OEFLAG_DONTCREATEATSTART=0x20000;
        public const short OCFLAGS2_DONTSAVEBKD=0x0001;
        public const short OCFLAGS2_SOLIDBKD=0x0002;
        public const short OCFLAGS2_COLBOX=0x0004;
        public const short OCFLAGS2_VISIBLEATSTART=0x0008;
        public const short OCFLAGS2_OBSTACLESHIFT=4;
        public const short OCFLAGS2_OBSTACLEMASK=0x0030;
        public const short OCFLAGS2_OBSTACLE_SOLID=0x0010;
        public const short OCFLAGS2_OBSTACLE_PLATFORM=0x0020;
        public const short OCFLAGS2_OBSTACLE_LADDER=0x0030;
        public const short OCFLAGS2_AUTOMATICROTATION=0x0040;

        // Flags modifiable by the program
        public const short OEPREFS_BACKSAVE=0x0001;
        public const short OEPREFS_SCROLLINGINDEPENDANT=0x0002;
        public const short OEPREFS_QUICKDISPLAY=0x0004;
        public const short OEPREFS_SLEEP=0x0008;
        public const short OEPREFS_LOADONCALL=0x0010;
        public const short OEPREFS_GLOBAL=0x0020;
        public const short OEPREFS_BACKEFFECTS=0x0040;
        public const short OEPREFS_KILL=0x0080;
        public const short OEPREFS_INKEFFECTS=0x0100;
        public const short OEPREFS_TRANSITIONS=0x0200;
        public const short OEPREFS_FINECOLLISIONS=0x0400;

        public int ocOEFlags;		    // New flags
        public short[] ocQualifiers;	    // Qualifier list
        public short ocFlags2;		    // New news flags, before was ocEvents
        public short ocOEPrefs;		    // Automatically modifiable flags
        public int ocIdentifier;		    // Identifier d'objet
        public int ocBackColor;		    // Background color
        public CRect ocFadeIn=null;     // Fade in 
        public CRect ocFadeOut=null;    // Fade out 
        public CMoveDefList ocMovements=null;     // La liste des mouvements
        public CDefValues ocValues=null;          // Les alterable values par defaut
        public CDefStrings ocStrings=null;        // Les alterable strings
        public CAnimHeader ocAnimations=null;     // Les animations
        public CDefCounters ocCounters=null;   // Settings lives / scores / counter
        public CDefObject ocObject=null;          // L'objet lui meme'
        public byte[] ocExtension=null;	// Les données objets extension
        public int ocVersion=0;
        public int ocID=0;
        public int ocPrivate=0;
        public int ocFadeInLength = 0;
        public int ocFadeOutLength = 0;
        
        public override void load(CFile file, short type)
        {
	        // Position de debut
	        int debut=file.getFilePointer();
	        ocQualifiers=new short[8];		    // OC_MAX_QUALIFIERS 
        	
	        // Lis le header
	        int n;
	        file.skipBytes(4);			    // DWORD ocDWSize;	Total size of the structures
	        int oMovements=file.readAShort();	    // WORD Offset of the movements
	        int oAnimations=file.readAShort();	    // WORD Offset of the animations
	        file.skipBytes(2);			    // WORD For version versions > MOULI 
	        int oCounter=file.readAShort();             // WORD Pointer to COUNTER structure
	        int oData=file.readAShort();		    // WORD Pointer to DATA structure
	        file.skipBytes(2);			    // WORD ocFree;
	        ocOEFlags=file.readAInt();		    // New flags
	        for (n=0; n<8; n++)
	            ocQualifiers[n]=file.readAShort();	    // OC_MAX_QUALIFIERS Qualifier list
	        int oExtension=file.readAShort();	    // WORD Extension structure 
	        int oValues=file.readAShort();		    // WORD Values structure
	        int oStrings=file.readAShort();             // WORD String structure
	        ocFlags2=file.readAShort();		    // WORD New news flags, before was ocEvents
	        ocOEPrefs=file.readAShort();		    // WORD Automatically modifiable flags
	        ocIdentifier=file.readAInt();		    // DWORD Identifier d'objet
	        ocBackColor=file.readAColor();		    // COLORREF Background color
	        int oFadeIn=file.readAInt();		    // oFadeIn DWORD Offset fade in 
	        int oFadeOut=file.readAInt();		    // oFadeOut DWORD Offset fade out 
        //	int ocValueNames=file.readAInt();	    // For the debugger
        //	int ocStringNames=file.readAInt();	    
        	
	        // Charge les movements
	        if (oMovements!=0)
	        {
	            file.seek(debut+oMovements);
	            ocMovements=new CMoveDefList();
	            ocMovements.load(file);
	        }
            // Charge les values
            if (oValues!=0)
            {
                file.seek(debut+oValues);
                ocValues=new CDefValues();
                ocValues.load(file);
            }
            // Charge les strings
            if (oStrings!=0)
            {
                file.seek(debut+oStrings);
                ocStrings=new CDefStrings();
                ocStrings.load(file);
            }
            // Charge les animations
            if (oAnimations!=0)
            {
                file.seek(debut+oAnimations);
                ocAnimations=new CAnimHeader();
                ocAnimations.load(file);
            }
            // Les données counters
            if (oCounter!=0)
            {
                file.seek(debut+oCounter);
                ocObject=new CDefCounter();
                ocObject.load(file);
            }
	    // Les données extension
	        if (oExtension!=0)
	        {
                file.seek(debut+oExtension);
	            int size=file.readAInt();
	            file.skipBytes(4);
	            ocVersion=file.readAInt();
	            ocID=file.readAInt();
	            ocPrivate=file.readAInt();
	            size-=20;
	            if (size!=0)
	            {
		            ocExtension=new byte[size];
		            file.read(ocExtension);
	            }
	        }
            
            // Le fade in
            if (oFadeIn != 0)
            {
                file.seek(debut + oFadeIn);
                file.skipBytes(8);
                ocFadeInLength = file.readAInt();
            }
            // Le fade out
            if (oFadeOut != 0)
            {
                file.seek(debut + oFadeOut);
                file.skipBytes(8);
                ocFadeOutLength = file.readAInt();
            }

            // Les données score/live/counter
            if (oData!=0)
            {
                file.seek(debut+oData);
                switch (type)
                {
                    case 3:         // OBJ_TEXT
                    case 4:         // OBJ_QUEST 
                        ocObject=new CDefTexts();
                        ocObject.load(file);
                        break;
                    
                    case 5:         // OBJ_SCORE
                    case 6:         // OBJ_LIVES
                    case 7:         // OBJ_COUNTER
                        ocCounters=new CDefCounters();
                        ocCounters.load(file);
                        break;
                    
                    case 8:         // OBJ_RTF
                        break;

                    case 9:         // OBJ_CCA
                        ocObject=new CDefCCA();
                        ocObject.load(file);
                        break;
                }
            }
        }

        public override void enumElements(IEnum enumImages, IEnum enumFonts)
        {
	        if (ocAnimations!=null)
            {
                ocAnimations.enumElements(enumImages);
            }
	        if (ocObject!=null)
	        {
	            ocObject.enumElements(enumImages, enumFonts);
	        }
	        if (ocCounters!=null)
	        {
	            ocCounters.enumElements(enumImages, enumFonts);
	        }
        }
        
    }
}
