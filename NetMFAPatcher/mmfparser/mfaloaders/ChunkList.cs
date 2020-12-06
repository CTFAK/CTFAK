using System;
using System.Collections.Generic;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.MFALoaders
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
        public ChunkList(ByteIO reader) : base(reader) { }
    }
}
