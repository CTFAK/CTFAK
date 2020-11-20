using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class ChunkList : DataLoader
    {
        List<DataLoader> items = new List<DataLoader>();
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var start = reader.Tell();
            while(true)
            {
                var id = reader.ReadByte();
                if(id==0) break;
                Console.WriteLine("ChunkFound:"+id);




            }


           
        }
        public ChunkList(ByteIO reader) : base(reader) { }
    }
}
