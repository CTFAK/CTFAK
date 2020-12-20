using System;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders
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
                var name = Reader.ReadWideString();

            }
            
        }
        public ObjectNames(ByteReader reader) : base(reader) { }
        public ObjectNames(Chunk chunk) : base(chunk) { }
    }
}
