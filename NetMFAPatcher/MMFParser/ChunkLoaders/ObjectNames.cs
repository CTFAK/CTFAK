using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.mmfparser.chunkloaders
{
    class ObjectNames : ChunkLoader//Fucking trash
    {
        public override void Print(bool ext)
        {
            
        }

        public override void Read()
        {
            var start = reader.Tell();
            var endpos = start + chunk.size;
            while(true)
            {
                if (reader.Tell() >= endpos+4) break;
                
                Console.WriteLine(reader.ReadWideString());
            }
            
        }
        public ObjectNames(ByteIO reader) : base(reader) { }
        public ObjectNames(Chunk chunk) : base(chunk) { }
    }
}
