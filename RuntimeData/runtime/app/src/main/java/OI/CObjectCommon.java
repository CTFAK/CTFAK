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
// COBJECTCOMMON : Donn�es d'un objet normal
//
//----------------------------------------------------------------------------------
package OI;

import Animations.CAnimHeader;
import Banks.CImageBank;
import Banks.IEnum;
import Movements.CMoveDefList;
import Services.CFile;
import Sprites.CMask;
import Sprites.CSprite;
import Transitions.CTransitionData;
import Values.CDefStrings;
import Values.CDefValues;


public class CObjectCommon extends COC
{
    // Flags 
    public static final int OEFLAG_DISPLAYINFRONT=0x0001;
    public static final int OEFLAG_BACKGROUND=0x0002;
    public static final int OEFLAG_BACKSAVE=0x0004;
    public static final int OEFLAG_RUNBEFOREFADEIN=0x0008;
    public static final int OEFLAG_MOVEMENTS=0x0010;
    public static final int OEFLAG_ANIMATIONS=0x0020;
    public static final int OEFLAG_TABSTOP=0x0040;
    public static final int OEFLAG_WINDOWPROC=0x0080;
    public static final int OEFLAG_VALUES=0x0100;
    public static final int OEFLAG_SPRITES=0x0200;
    public static final int OEFLAG_INTERNALBACKSAVE=0x0400;
    public static final int OEFLAG_SCROLLINGINDEPENDANT=0x0800;
    public static final int OEFLAG_QUICKDISPLAY=0x1000;
    public static final int OEFLAG_NEVERKILL=0x2000;
    public static final int OEFLAG_NEVERSLEEP=0x4000;
    public static final int OEFLAG_MANUALSLEEP=0x8000;
    public static final int OEFLAG_TEXT=0x10000;
    public static final int OEFLAG_DONTCREATEATSTART=0x20000;
    public static final int OEFLAG_DONTRESETANIMCOUNTER=0x100000;
    public static final short OCFLAGS2_DONTSAVEBKD=0x0001;
    public static final short OCFLAGS2_SOLIDBKD=0x0002;
    public static final short OCFLAGS2_COLBOX=0x0004;
    public static final short OCFLAGS2_VISIBLEATSTART=0x0008;
    public static final short OCFLAGS2_OBSTACLESHIFT=4;
    public static final short OCFLAGS2_OBSTACLEMASK=0x0030;
    public static final short OCFLAGS2_OBSTACLE_SOLID=0x0010;
    public static final short OCFLAGS2_OBSTACLE_PLATFORM=0x0020;
    public static final short OCFLAGS2_OBSTACLE_LADDER=0x0030;
    public static final short OCFLAGS2_AUTOMATICROTATION=0x0040;
    public static final short OCFLAGS2_INITFLAGS=0x0080;

    // Flags modifiable by the program
    public static final short OEPREFS_BACKSAVE=0x0001;
    public static final short OEPREFS_SCROLLINGINDEPENDANT=0x0002;
    public static final short OEPREFS_QUICKDISPLAY=0x0004;
    public static final short OEPREFS_SLEEP=0x0008;
    public static final short OEPREFS_LOADONCALL=0x0010;
    public static final short OEPREFS_GLOBAL=0x0020;
    public static final short OEPREFS_BACKEFFECTS=0x0040;
    public static final short OEPREFS_KILL=0x0080;
    public static final short OEPREFS_INKEFFECTS=0x0100;
    public static final short OEPREFS_TRANSITIONS=0x0200;
    public static final short OEPREFS_FINECOLLISIONS=0x0400;

    
    public int ocOEFlags;		    /// New flags
    public short ocQualifiers[];	    /// Qualifier list
    public short ocFlags2;		    /// New news flags, before was ocEvents
    public short ocOEPrefs;		    /// Automatically modifiable flags
    public int ocIdentifier;		    /// Identifier d'objet
    public int ocBackColor;		    /// Background color
    public CTransitionData ocFadeIn=null;                    /// Fade in 
    public CTransitionData ocFadeOut=null;                   /// Fade out 
    public CMoveDefList ocMovements=null;     /// La liste des mouvements
    public CDefValues ocValues=null;          /// Les alterable values par defaut
    public CDefStrings ocStrings=null;        /// Les alterable strings
    public CAnimHeader ocAnimations=null;     /// Les animations
    public CDefCounters ocCounters=null;   /// Settings lives / scores / counter
    public CDefObject ocObject=null;          /// L'objet lui meme'
    public byte ocExtension[]=null;	/// Les donn�es objets extension
    public int ocVersion=0;
    public int ocID=0;
    public int ocPrivate=0;
    
    public CObjectCommon() 
    {
    }

    @Override
	public void load(CFile file, short type, COI pOi)
    {
	// Position de debut
	long debut=file.getFilePointer();
	ocQualifiers=new short[8];		    // OC_MAX_QUALIFIERS 
	
	// Lis le header
	int n;
	file.skipBytes(4);			    // DWORD ocDWSize;	Total size of the structures
	int oMovements=file.readAShort();	    // WORD Offset of the movements
	int oValues=file.readAShort();		    // WORD Values structure
	file.skipBytes(2);			    // WORD For version versions > MOULI 
	int oCounter=file.readAShort();             // WORD Pointer to COUNTER structure
	int oData=file.readAShort();		    // WORD Pointer to DATA structure
	int oExtension=file.readAShort();	    // WORD Extension structure 
	ocOEFlags=file.readAInt();		    // New flags
	for (n=0; n<8; n++)
	    ocQualifiers[n]=file.readAShort();	    // OC_MAX_QUALIFIERS Qualifier list
	int oAnimations=file.readAShort();	    // WORD Offset of the animations
	file.skipBytes(2);
	int oStrings=file.readAShort();             // WORD String structure
	ocFlags2=file.readAShort();		    // WORD New news flags, before was ocEvents
	ocOEPrefs=file.readAShort();		    // WORD Automatically modifiable flags
	ocIdentifier=file.readAInt();		    // DWORD Identifier d'objet
	ocBackColor=file.readAColor();		    // COLORREF Background color
	int oFadeIn=file.readAInt();                // DWORD Offset fade in 
	int oFadeOut=file.readAInt();		    // DWORD Offset fade out 
//	int ocValueNames=file.readAInt();	    // For the debugger
//	int ocStringNames=file.readAInt();	    
	
	// Deprecated in Android HWA
	ocOEFlags &= ~OEFLAG_QUICKDISPLAY;
	
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
            ocValues.load(file, (ocFlags2 & CObjectCommon.OCFLAGS2_INITFLAGS) != 0);
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
        // Les donn�es counters
        if (oCounter!=0)
        {
            file.seek(debut+oCounter);
            ocObject=new CDefCounter();
            ocObject.load(file);
        }
	// Les donn�es extension
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
        if (oFadeIn!=0)
        {
            file.seek(debut+oFadeIn);
            ocFadeIn=new CTransitionData();
            ocFadeIn.load(file);
        }
        // Le fade out
        if (oFadeOut!=0)
        {
            file.seek(debut+oFadeOut);
            ocFadeOut=new CTransitionData();
            ocFadeOut.load(file);
        }
	// Les donn�es score/live/counter
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
                
              /*  case 8:         // OBJ_RTF
                    ocObject=new CDefRtf();
                    ocObject.load(file);
                    
				    // Change les OEFLAGS pour virer les attributs sprite
				    ocOEFlags&=~(OEFLAG_SPRITES|OEFLAG_QUICKDISPLAY|OEFLAG_BACKSAVE);
                    break;*/
                    
                case 9:         // OBJ_CCA
                    ocObject=new CDefCCA();
                    ocObject.load(file);
                    break;
            }
        }
    }
    @Override
	public void enumElements(IEnum enumImages, IEnum enumFonts)
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

    @Override
	public CMask spriteGetMask()
    {
        return null;
    }

    @Override
	public void spriteDraw(CSprite spr, CImageBank bank, int x, int y)
    {
    }
}
