//----------------------------------------------------------------------------------
//
// CFONT : une fonte
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RuntimeXNA.Application;
using RuntimeXNA.Services;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Banks
{
    public class CFont
    {
        public short useCount = 0;
        public short handle = 0;
        public int lfHeight = 0;
        public byte lfItalic = 0;
        public int lfWeight = 0;
        public string lfFaceName = null;
        public SpriteFont spriteFont=null;
        private ContentManager content;

        public void loadHandle(CFile file)
        {
            handle = (short)file.readAShort();
            if (file.bUnicode == false)
            {
                file.skipBytes(4+4+1+32);
            }
            else
            {
                file.skipBytes(4+4+1+64);
            }
        }

        public void load(CFile file, ContentManager c)
        {
            content = c;

            handle = (short)file.readAShort();

            int debut = file.getFilePointer();
            lfHeight = file.readAInt();
            lfWeight = file.readAInt();
            lfItalic=(byte)file.readAByte();
            lfFaceName = file.readAString();

            // Positionne a la fin
            if (file.bUnicode == false)
            {
                file.seek(debut + 41);
            }
            else
            {
                file.seek(debut + 73);
            }
        }

        public CFontInfo getFontInfo()
        {
            CFontInfo info = new CFontInfo();
            info.lfHeight = lfHeight;
            info.lfWeight = lfWeight;
            info.lfItalic = lfItalic;
            info.lfFaceName = lfFaceName;
            return info;
        }

        public static CFont createFromFontInfo(CFontInfo info, CRunApp app)
        {
            CFont font = new CFont();
            font.content = app.content;
            font.lfHeight = info.lfHeight;
            font.lfWeight = info.lfWeight;
            font.lfItalic = info.lfItalic;
            font.lfFaceName = info.lfFaceName;
            return font;
        }

        public SpriteFont getFont()
        {
            // Cree la fonte
            if (spriteFont == null)
            {
                string name=lfFaceName;
                do
                {
                    int pos = name.IndexOf(' ');
                    if (pos < 0)
                    {
                        break;
                    }
                    name = name.Substring(0, pos) + name.Substring(pos + 1);
                } while (true);
                name+=lfHeight.ToString();
                if (lfWeight>400)
                {
                    name+="Bold";
                }
                if (lfItalic!=0)
                {
                    name+="Italic";
                }
                spriteFont = content.Load<SpriteFont>(name);
            }
            return spriteFont;
        }
    }
}
