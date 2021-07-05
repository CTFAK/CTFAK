//----------------------------------------------------------------------------------
//
// CFILE : chargement des fichiers dans le bon sens
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeXNA.Services
{
    public class CFile
    {
        private byte[] data;
        public int pointer = 0;
        public bool bUnicode=false;

        public CFile()
        {
        }
        public CFile(CFile file)
        {
            data = file.data;
            pointer = 0;
        }
        public CFile(byte[] dt)
        {
            data = dt;
            pointer=0;
        }
        public CFile(CFile source, int length)
        {
            data = new byte[length];
            int n;
            for (n = 0; n < length; n++)
            {
                data[n] = source.data[source.pointer + n];
            }
            source.pointer += n;
            bUnicode = source.bUnicode;
        }
        public bool isEOF()
        {
            return pointer >= data.Length;
        }


        public void adjustTo8()
        {
            if ((pointer & 0x07) != 0)
            {
                pointer += 8 - (pointer & 0x07);
            }
        }

        public int readUnsignedByte()
        {
            if (pointer < data.Length)
            {
                return data[pointer++] & 255;
            }
            return 0;
        }

        public short readShort()
        {
            int b1 = readUnsignedByte();
            int b2 = readUnsignedByte();
            return (short) ((b1 << 8) | b2);
        }

        public byte readByte()
        {
            if (pointer < data.Length)
            {
                return data[pointer++];
            }
            return 0;
        }

        public byte[] readArray(int size)
        {
            if (size<0)
            {
                size=data.Length;
            }
            byte[] array=new byte[size];
            int n;
            for (n = 0; n < size; n++)
            {
                array[n] = data[pointer++];
            }
            return array;
        }

        public int read(byte[] dest, int size)
        {
            int n;
            for (n = 0; n < size; n++)
            {
                dest[n] = data[pointer++];
            }
            return n;
        }

        public int read(byte[] dest)
        {
            int n;
            for (n = 0; n < dest.Length; n++)
            {
                dest[n] = data[pointer++];
            }
            return n;
        }

        public void skipBytes(int n)
        {
            if (pointer + n >= data.Length)
            {
                n = data.Length - pointer;
            }
            pointer += n;
        }

        public void skipBack(int n)
        {
            int pos = getFilePointer();
            pos -= n;
            if (pos < 0)
            {
                pos = 0;
            }
            seek(pos);
        }

        public void seek(int pos)
        {
            if (pos >= data.Length)
            {
                pos = data.Length;
            }
            pointer = pos;
        }
 
        public int getFilePointer()
        {
            return pointer;
        }

        public void setUnicode(bool unicode)
        {
            bUnicode = unicode;
        }

        public byte readAByte()
        {
            return data[pointer++];
        }

        public short readAShort()
        {
            int b1, b2;
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            return (short) (b2 * 256 + b1);
        }

        public char readAChar()
        {
            int b1, b2;
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            return (char) (b2 * 256 + b1);
        }

        public void readAChar(char[] b) 
        {
            int b1, b2;
            int n;
            for (n=0; n<b.Length; n++)
            {
                b1 = readUnsignedByte();
                b2 = readUnsignedByte();
                b[n]=(char) (b2 * 256 + b1);
            }
        }

        public int readAInt()
        {
            int b1, b2, b3, b4;
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            b3 = readUnsignedByte();
            b4 = readUnsignedByte();
            return b4 * 0x01000000 + b3 * 0x00010000 + b2 * 0x00000100 + b1;
        }

        public int readAColor()
        {
            int b1, b2, b3;
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            b3 = readUnsignedByte();
            readUnsignedByte();
            return b1 * 0x00010000 + b2 * 0x00000100 + b3;
        }

        public float readAFloat()
        {
            int b1, b2, b3, b4;
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            b3 = readUnsignedByte();
            b4 = readUnsignedByte();
            int total = b4 * 0x01000000 + b3 * 0x00010000 + b2 * 0x00000100 + b1;
            return (float) total / (float) 65536.0;
        }

        public double readADouble()
        {
            int b1, b2, b3, b4, b5, b6, b7, b8;
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            b3 = readUnsignedByte();
            b4 = readUnsignedByte();
            b5 = readUnsignedByte();
            b6 = readUnsignedByte();
            b7 = readUnsignedByte();
            b8 = readUnsignedByte();
            long total1 = ((long) b4) * 0x01000000 + ((long) b3) * 0x00010000 + ((long) b2) * 0x00000100 + (long) b1;
            long total2 = ((long) b8) * 0x01000000 + ((long) b7) * 0x00010000 + ((long) b6) * 0x00000100 + (long) b5;
            long total = (total2 << 32) | total1;
            double temp = (double) total / (double) 65536.0;
            return temp / (double) 65536.0;
        }

        public string readAString(int size)
        {
            if (bUnicode==false)
            {
                byte[] b = new byte[size];
                read(b);
                int n;
                for (n = 0; n < size; n++)
                {
                    if (b[n] == 0)
                    {
                        break;
                    }
                }
                int m;
                char[] bb = new char[n];
                for (m = 0; m < n; m++)
                {
                    bb[m] = (char)b[m];
                }
                return new String(bb, 0, n);
            }
            else
            {
                char[] b=new char[size];
                readAChar(b);
                int n;
                for (n=0; n<size; n++)
                {
                    if (b[n]==0)
                    {
                        break;
                    }
                }
                int m;
                char[] bb=new char[n];
                for (m=0; m<n; m++)
                {
                    bb[m]=b[m];
                }
                return new String(bb, 0, n);
            }
        }

        public String readAString()
        {
            string ret = "";
            int debut = getFilePointer();
            if (bUnicode == false)
            {
                int b;
                do
                {
                    b = readUnsignedByte();
                } while (b != 0);
                int end = getFilePointer();
                seek(debut);
                if (end >= debut + 2)
                {
                    int l = (int)(end - debut - 1);
                    char[] c = new char[l];
                    int n;
                    for (n = 0; n < l; n++)
                    {
                        c[n] = (char)readUnsignedByte();
                    }
                    ret = new String(c, 0, l);
                }
                skipBytes(1);
            }
            else
            {
                char b;
                do
                {
                    b = readAChar();
                } while (b != 0);
                int end = getFilePointer();
                seek(debut);
                if (end >= debut + 2)
                {
                    int l = (int)(end - debut - 2) / 2;
                    char[] c = new char[l];
                    readAChar(c);
                    ret = new String(c, 0, l);
                }
                skipBytes(2);
            }
            return ret;
        }

        public String readAStringEOL()
        {
            int debut = getFilePointer();
            String ret = "";

            if (bUnicode==false)
            {
                int b = readUnsignedByte();
                while (b != 10 && b != 13)
                {
                    if (isEOF())
                    {
                        break;
                    }
                    b = readUnsignedByte();
                }
                int end = getFilePointer();
                seek(debut);
                int delta = 1;
                if (b != 10 && b != 13)
                {
                    delta = 0;
                }
                if (end > debut + delta)
                {
                    int l = (int)(end - debut - delta);
                    char[] c = new char[l];
                    int n;
                    for (n = 0; n < l; n++)
                    {
                        c[n] = (char)readUnsignedByte();
                    }
                    ret = new String(c, 0, c.Length);
                }
                if (b == 10 || b == 13)
                {
                    skipBytes(1);
                    int bb = readUnsignedByte();
                    if (b == 10 && bb != 13)
                    {
                        skipBack(1);
                    }
                    if (b == 13 && bb != 10)
                    {
                        skipBack(1);
                    }
                }
            }
            else
            {
                char b = readAChar();
                while (b != 10 && b != 13)
                {
                    if (isEOF())
                    {
                        break;
                    }
                    b = readAChar();
                }
                int end = getFilePointer();
                seek(debut);
                int delta = 2;
                if (b != 10 && b != 13)
                {
                    delta = 0;
                }
                if (end > debut + delta)
                {
                    int l=(int) (end - debut - delta)/2;
                    char[] c = new char[l];
                    readAChar(c);
                    ret = new String(c, 0, c.Length);
                }
                if (b == 10 || b == 13)
                {
                    skipBytes(2);
                    char bb = readAChar();
                    if (b == 10 && bb != 13)
                    {
                        skipBack(2);
                    }
                    if (b == 13 && bb != 10)
                    {
                        skipBack(2);
                    }
                }
            }
            return ret;
        }

        public void skipAString()
        {
            if (bUnicode==false)
            {
                int b;
                do
                {
                    b = readUnsignedByte();
                } while (b != 0);
            }
            else
            {
                int b;
                do
                {
                    b = readShort();
                } while (b != 0);
            }
        }

        public CFontInfo readLogFont()
        {
            CFontInfo info = new CFontInfo();

            info.lfHeight = readAInt();
            if (info.lfHeight < 0)
            {
                info.lfHeight = -info.lfHeight;
            }
            skipBytes(12);	// width - escapement - orientation
            info.lfWeight = readAInt();
            info.lfItalic = (byte)readAByte();
            info.lfUnderline = (byte)readAByte();
            info.lfStrikeOut = (byte)readAByte();
            skipBytes(5);
            info.lfFaceName = readAString(32);

            return info;
        }

        public CFontInfo readLogFont16()
        {
            CFontInfo info = new CFontInfo();

            info.lfHeight = readAShort();
            if (info.lfHeight < 0)
            {
                info.lfHeight = -info.lfHeight;
            }
            skipBytes(6);	// width - escapement - orientation
            info.lfWeight = readAShort();
            info.lfItalic = (byte)readAByte();
            info.lfUnderline = (byte)readAByte();
            info.lfStrikeOut = (byte)readAByte();
            skipBytes(5);
            bool oldUnicode = bUnicode;
            bUnicode = false;
            info.lfFaceName = readAString(32);
            bUnicode = oldUnicode;

            return info;
        }
    }
}
