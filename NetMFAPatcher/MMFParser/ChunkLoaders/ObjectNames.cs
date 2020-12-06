using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    class ObjectNames : ChunkLoader//Fucking trash
    {
        public override void Print(bool ext)
        {
            
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var start = Reader.Tell();
            var endpos = start + Chunk.Size;
            while(true)
            {
                if (Reader.Tell() >= endpos+4) break;
                
                Console.WriteLine(Reader.ReadWideString());
            }
            
        }
        public ObjectNames(ByteIO reader) : base(reader) { }
        public ObjectNames(Chunk chunk) : base(chunk) { }
    }
}
