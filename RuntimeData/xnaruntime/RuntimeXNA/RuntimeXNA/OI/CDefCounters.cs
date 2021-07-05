//----------------------------------------------------------------------------------
//
// CDEFCOUNTERS : Données d'un objet score / vies / counter
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;

namespace RuntimeXNA.OI
{
    public class CDefCounters
    {
        // Display types
        public const short CTA_HIDDEN=0;
        public const short CTA_DIGITS=1;
        public const short CTA_VBAR=2;
        public const short CTA_HBAR=3;
        public const short CTA_ANIM=4;
        public const short CTA_TEXT=5;    
        public const short BARFLAG_INVERSE=0x0100;

        public int odCx;					// Size: only lives & counters
        public int odCy;
        public short odPlayer;				// Player: only score & lives
        public short odDisplayType;			// CTA_xxx
        public short odDisplayFlags;			// BARFLAG_INVERSE
        public short odFont;					// Font
        public short ocBorderSize;			// Border
        public int ocBorderColor;
        public short ocShape;			// Shape
        public short ocFillType;
        public short ocLineFlags;			// Only for lines in non filled mode
        public int ocColor1;			// Gradient
        public int ocColor2;
        public int ocGradientFlags;
        public short nFrames;
        public short[] frames=null;

        public void load(CFile file)
        {
            file.skipBytes(4);          // size
            odCx=file.readAInt();
            odCy=file.readAInt();
            odPlayer=file.readAShort();
            odDisplayType=file.readAShort();
            odDisplayFlags=file.readAShort();
            odFont=file.readAShort();
            
            switch (odDisplayType)
            {
                case 0:             // CTA_HIDDEN
                    break;
                case 1:             // CTA_DIGITS
                case 4:             // CTA_ANIM
                    nFrames=file.readAShort();
                    frames=new short[nFrames];
                    int n;
                    for (n=0; n<nFrames; n++)
                    {
                        frames[n]=file.readAShort();
                    }
                    break;
                case 2:             // CTA_VBAR
                case 3:             // CTA_HBAR
                case 5:             // CTA_TEXT
                    ocBorderSize=file.readAShort();
                    ocBorderColor=file.readAColor();
                    ocShape=file.readAShort();
                    ocFillType=file.readAShort();
                    if (ocShape==1)		// SHAPE_LINE
                    {
                        ocLineFlags=file.readAShort();
                    }
                    else
                    {
                        switch (ocFillType)
                        {
                            case 1:			    // FILLTYPE_SOLID
                                ocColor1=file.readAColor();
                                break;
                            case 2:			    // FILLTYPE_GRADIENT
                                ocColor1=file.readAColor();
                                ocColor2=file.readAColor();
                                ocGradientFlags=file.readAInt();
                                break;
                            case 3:			    // FILLTYPE_IMAGE
                                break;
                        }
                    }
                    break;
            }
        }

        public void enumElements(IEnum enumImages, IEnum enumFonts)
        {
            short num;
            switch (odDisplayType)
            {
                case 1:             // CTA_DIGITS
                case 4:             // CTA_ANIM
                    int n;
                    for (n = 0; n < nFrames; n++)
                    {
                        if (enumImages != null)
                        {
                            num = enumImages.enumerate(frames[n]);
                            if (num != -1)
                            {
                                frames[n] = num;
                            }
                        }
                    }
                    break;
                case 5:             // CTA_TEXT
                    if (enumFonts != null)
                    {
                        num = enumFonts.enumerate(odFont);
                        if (num != -1)
                        {
                            odFont = num;
                        }
                    }
                    break;
            }
        }
    }
}
