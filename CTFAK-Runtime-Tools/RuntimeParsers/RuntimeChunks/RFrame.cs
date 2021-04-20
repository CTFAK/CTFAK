using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.RuntimeParsers.RuntimeChunks
{
    public class RFrame:Frame
    {
        public RFrame(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            var unk = Reader.ReadInt32();
            var unk2 = Reader.ReadInt32();
            var namePtr = Reader.ReadInt32();
            
            var start = Reader.Tell();
            Reader.Seek(namePtr);
            Name = Reader.ReadWideString();
            Reader.Seek(start);
        }
    }
}