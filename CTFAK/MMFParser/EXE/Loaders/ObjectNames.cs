using System;
using System.Collections.Generic;
using System.Linq;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    class ObjectNames : ChunkLoader//2.5+ trash
    {
        public Dictionary<int, string> Names;

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override void Print(bool ext){}
        

        public override string[] GetReadableData()
        {
            return Names.Values.ToArray();
        }

        public override void Read()
        {
            var start = Reader.Tell();
            var end = start + Reader.Size();
            Names = new Dictionary<int,string>();
            int current = 0;
            while(Reader.Tell() < end)
            {
                var name = Reader.ReadWideString();
                Names.Add(current,name);
                current++;
            }

        }
        public ObjectNames(ByteReader reader) : base(reader) { }
        public ObjectNames(Chunk chunk) : base(chunk) { }
    }
}
