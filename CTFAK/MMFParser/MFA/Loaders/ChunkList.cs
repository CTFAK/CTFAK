using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders
{
    public class ChunkList : DataLoader//This is used for MFA reading/writing
    {
        List<DataLoader> _items = new List<DataLoader>();
        public byte[] Saved;

        public override void Write(ByteWriter Writer)
        {
            if (Saved != null)
            {
                Writer.WriteBytes(Saved);
            }
            else
            {
                Writer.WriteInt8(0);
            }
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var start = Reader.Tell();
            while(true)
            {
                var id = Reader.ReadByte();
                if(id==0) break;
                var data = new ByteReader(Reader.ReadBytes((int) Reader.ReadUInt32()));
                
            }

            var size = Reader.Tell() - start;
            Reader.Seek(start);
            Saved = Reader.ReadBytes((int) size);


        }
        public ChunkList(ByteReader reader) : base(reader) { }
    }
}
