using System;
using System.Text;
using System.IO;
using System.Drawing;

namespace NetMFAPatcher.Utils
{
    public class ByteWriter : BinaryWriter
    {
        public ByteWriter(Stream input) : base(input)
        {
        }

        public ByteWriter(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public ByteWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public ByteWriter(byte[] data) : base(new MemoryStream(data))
        {
        }

        public ByteWriter(string path, FileMode fileMode) : base(new FileStream(path, fileMode))
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

        public bool eof()
        {
            return BaseStream.Position < BaseStream.Length;
        }

        


        


        
        

    }
}