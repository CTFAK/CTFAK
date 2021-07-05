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
// CBINARYFILE: ficher binaire en indian PC
//
//----------------------------------------------------------------------------------
package Services;

/** This class is used by the extensions to load the data.
 * It converts the PC based data to Java by inverting the bytes.
 */
public class CBinaryFile
{
    public byte data[];
    public int pointer;
    public boolean bUnicode;

    public CBinaryFile(byte[] array, boolean unicode)
    {
        data = array;
        pointer = 0;
        bUnicode=unicode;
    }

    public void setUnicode(boolean unicode)
    {
        bUnicode=unicode;
    }

    public void read(byte[] b)
    {
        int size = b.length;
        int n;
        if (pointer + size <= data.length)
        {
            for (n = 0; n < size; n++)
            {
                b[n] = data[pointer + n];
            }
            pointer += size;
            return;
        }
        for (n = 0; n < size; n++)
        {
            b[n] = 0;
        }
    }

    public void skipBytes(int n)
    {
        if (pointer + n <= data.length)
        {
            pointer += n;
        }
    }

    public int getFilePointer()
    {
        return pointer;
    }

    public void seek(int pos)
    {
        if (pos <= data.length)
        {
            pointer = pos;
        }
    }

    public byte readByte()
    {
        if (pointer + 1 <= data.length)
        {
            return data[pointer++];
        }
        return 0;
    }

    public short readShort()
    {
        if (pointer + 2 <= data.length)
        {
            int b1 = data[pointer++] & 255;
            int b2 = data[pointer++] & 255;
            return (short) (b2 * 256 + b1);
        }
        return 0;
    }

    public int readInt()
    {
        if (pointer + 4 <= data.length)
        {
            int b1 = data[pointer++] & 255;
            int b2 = data[pointer++] & 255;
            int b3 = data[pointer++] & 255;
            int b4 = data[pointer++] & 255;
            return b4 * 0x01000000 + b3 * 0x00010000 + b2 * 0x00000100 + b1;
        }
        return 0;
    }

    public float readFloat()
    {
        int b1 = data[pointer++] & 255;
        int b2 = data[pointer++] & 255;
        int b3 = data[pointer++] & 255;
        int b4 = data[pointer++] & 255;
        int total = b4 * 0x01000000 + b3 * 0x00010000 + b2 * 0x00000100 + b1;
        if (total>0x80000000)
            total-=0xFFFFFFFF;
        return (float)(total/65536.0);
    }


    public int readColor()
    {
        if (pointer + 4 <= data.length)
        {
            int b1 = data[pointer++] & 255;
            int b2 = data[pointer++] & 255;
            int b3 = data[pointer++] & 255;
            pointer++;			// int b4=data[pointer++]&255;
            return b1 * 0x00010000 + b2 * 0x00000100 + b3;
        }
        return 0;
    }

    public char readChar()
    {
        if (pointer + 2 <= data.length)
        {
            int b1, b2;
            b1 = data[pointer++]&255;
            b2 = data[pointer++]&255;
            return (char) (b2 * 256 + b1);
        }
        return 0;
    }

    public void readChar(char b[])
    {
        if (pointer + b.length*2 <= data.length)
        {
            int b1, b2;
            int n;
            for (n=0; n<b.length; n++)
            {
                b1 = data[pointer++]&255;
                b2 = data[pointer++]&255;
                b[n]=(char) (b2 * 256 + b1);
            }
        }
    }

    public String readString(int size)
    {
        if (bUnicode==false)
        {
            byte b[] = new byte[size];
            read(b);
            int n;
            for (n=0; n<size; n++)
            {
                if (b[n]==0)
                {
                    break;
                }
            }
            int m;
            byte bb[]=new byte[n];
            for (m=0; m<n; m++)
            {
                bb[m]=b[m];
            }
            try
            {
                return new String(bb, CFile.charset);
            }
            catch(Exception e)
            {
                return "";
            }
        }
        else
        {
            char b[]=new char[size];
            readChar(b);
            int n;
            for (n=0; n<size; n++)
            {
                if (b[n]==0)
                {
                    break;
                }
            }
            int m;
            char bb[]=new char[n];
            for (m=0; m<n; m++)
            {
                bb[m]=b[m];
            }
            return new String(bb);
        }
    }

    public String readString()
    {
        int debut = getFilePointer();
        if (bUnicode==false)
        {
            byte b;
            do
            {
                b = readByte();
            } while (b != 0);
            int end = getFilePointer();
            seek(debut);
            if (end-debut-1>0)
            {
                byte c[] = new byte[end - debut - 1];
                read(c);
                skipBytes(1);

                try
                {
                    return new String(c, CFile.charset);
                }
                catch(Exception e)
                {
                    return "";
                }
            }
            skipBytes(1);
            return "";
        }
        else
        {
            char b;
            do
            {
                b = readChar();
            } while (b != 0);
            int end = getFilePointer();
            seek(debut);
            if (end-debut-2>0)
            {
                int l=(end - debut - 2)/2;
                char c[] = new char[l];
                readChar(c);
                skipBytes(2);
                return new String(c);
            }
            skipBytes(2);
            return "";
        }
    }

    public CFontInfo readLogFont()
    {
        CFontInfo info = new CFontInfo();

        info.lfHeight = readInt();
        if (info.lfHeight < 0)
        {
            info.lfHeight = -info.lfHeight;
        }
        skipBytes(12);	// width - escapement - orientation
        info.lfWeight = readInt();
        info.lfItalic = readByte();
        info.lfUnderline = readByte();
        info.lfStrikeOut = readByte();
        skipBytes(5);
        info.lfFaceName = readString(32);

        return info;
    }

    public CFontInfo readLogFont16()
    {
        CFontInfo info = new CFontInfo();

        info.lfHeight = readShort();
        if (info.lfHeight < 0)
        {
            info.lfHeight = -info.lfHeight;
        }
        skipBytes(6);	// width - escapement - orientation
        info.lfWeight = readShort();
        info.lfItalic = readByte();
        info.lfUnderline = readByte();
        info.lfStrikeOut = readByte();
        skipBytes(5);
        boolean oldUnicode=bUnicode;
        bUnicode=false;
        info.lfFaceName = readString(32);
        bUnicode=oldUnicode;

        return info;
    }

    public void skipString()
    {
        if (bUnicode==false)
        {
            byte b;
            do
            {
                b = readByte();
            } while (b != 0);
        }
        else
        {
            char b;
            do
            {
                b = readChar();
            } while (b != 0);
        }
    }
}
