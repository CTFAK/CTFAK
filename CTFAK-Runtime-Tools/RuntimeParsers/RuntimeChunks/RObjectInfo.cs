using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.RuntimeParsers
{
    public class RObjectInfo:ObjectInfo
    {
        public RObjectInfo(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Handle = Reader.ReadInt16();
            var unk = Reader.ReadInt16();
            var unk1 = Reader.ReadInt32();
            var unk2 = Reader.ReadInt32();
            var unk3 = Reader.ReadInt32();
            var namePtr = Reader.ReadInt32();
            var start = Reader.Tell();
            Reader.Seek(namePtr);
            Name = Reader.ReadWideString();
            Reader.Seek(start);
        }
    }
}