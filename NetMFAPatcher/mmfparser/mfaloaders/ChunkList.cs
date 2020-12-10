using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders
{
    class ChunkList : DataLoader//This is used for MFA reading/writing
    {
        List<DataLoader> _items = new List<DataLoader>();
        
        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
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


           
        }
        public ChunkList(ByteReader reader) : base(reader) { }
    }
}
