using System;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders
{
    class ExtData : ChunkLoader
    {
        public ExtData(Chunk chunk) : base(chunk) { }
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
