using System;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    class ExtData : ChunkLoader
    {
        public ExtData(Chunk chunk) : base(chunk) { }
        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var filename = Reader.ReadAscii();
            //var data = reader.ReadBytes();
        }
    }
}
