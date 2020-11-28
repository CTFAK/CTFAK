using System;
using System.Text;
using System.IO;
using System.Drawing;

namespace NetMFAPatcher.Utils
{
    public class ByteIO : BinaryReader
    {
        public ByteIO(Stream input) : base(input)
        {
        }

        public ByteIO(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public ByteIO(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public ByteIO(byte[] data) : base(new MemoryStream(data))
        {
        }

        public ByteIO(string path, FileMode fileMode) : base(new FileStream(path, fileMode))
        {
        }

        public void Seek(Int64 offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            BaseStream.Seek(offset, seekOrigin);
        }

        public void Skip(Int64 count)
        {
            BaseStream.Seek(count, SeekOrigin.Current);
        }
        public byte[] ReadFourCc()
        {
            return Encoding.UTF8.GetBytes(ReadAscii(4));
        }

        public Int64 Tell()
        {
            return BaseStream.Position;
        }

        public Int64 Size()
        {
            return BaseStream.Length;
        }

        public bool Check(int size)
        {
            return Size() - Tell() >= size;
        }

        public bool Eof()
        {
            return BaseStream.Position < BaseStream.Length;
        }

        public UInt16 PeekUInt16()
        {
            UInt16 value = ReadUInt16();
            Seek(-2, SeekOrigin.Current);
            return value;
        }

        public Int16 PeekInt16()
        {
            Int16 value = ReadInt16();
            Seek(-2, SeekOrigin.Current);
            return value;
        }


        public string ReadAscii(int length = -1)
        {
            string str = "";
            if (length >= 0)
            {
                for (int i = 0; i < length; i++)
                {
                    str += Convert.ToChar(ReadByte());
                }
            }
            else
            {
                byte b = ReadByte();
                while (b != 0)
                {
                    str += Convert.ToChar(b);
                    b = ReadByte();
                }
            }

            return str;
        }

        

        public string ReadWideString(int length = -1)
        {
            String str = "";
            if (length >= 0)
            {
                for (int i = 0; i < length; i++)
                {
                    str += Convert.ToChar(ReadUInt16());
                }
            }
            else
            {
                var b = ReadUInt16();
                while (b != 0)
                {
                    str += Convert.ToChar(b);
                    b = ReadUInt16();
                }
            }

            return str;
        }
        public Color ReadColor()
        {
            
            var r = ReadByte();
            var g = ReadByte();
            var b = ReadByte();
            Skip(1);
            Color color = Color.FromArgb(b, g, r);

            return color;

        }

    }
}