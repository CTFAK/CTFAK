using System;
using static DotNetCTFDumper.MMFParser.Data.ChunkList;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders
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
