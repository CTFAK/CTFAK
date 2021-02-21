using System;
using System.IO;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    class ExtData : ChunkLoader
    {
       
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
            var data = Reader.ReadBytes();
            Logger.Log($"Found file {filename}, {data.Length.ToPrettySize()}");
            // File.WriteAllBytes($"{Settings.DumpPath}\\{filename}",data);
        }

        public ExtData(ByteReader reader) : base(reader)
        {
        }
    }
}
