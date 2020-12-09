using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders
{
    class ChunkList : DataLoader//This is used for MFA reading/writing
    {
        List<DataLoader> _items = new List<DataLoader>();
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
                Console.WriteLine("ChunkFound:"+id);




            }


           
        }
        public ChunkList(ByteReader reader) : base(reader) { }
    }
}
