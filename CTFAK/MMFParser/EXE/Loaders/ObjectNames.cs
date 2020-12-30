using System;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    class ObjectNames : ChunkLoader//2.5+ trash
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
                if (Reader.Tell() >= endpos) break;
                var name = Reader.ReadWideString();

            }
            
        }
        public ObjectNames(ByteReader reader) : base(reader) { }
        public ObjectNames(Chunk chunk) : base(chunk) { }
    }
}
