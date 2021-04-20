using System;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.RuntimeParsers.RuntimeChunks
{
    public class RSoundItem:SoundItem
    {
        public RSoundItem(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Handle = Reader.ReadUInt32();
            var dataLen = Reader.ReadInt32();
            Checksum = Reader.ReadInt32();
            References = Reader.ReadUInt32();
            var unk1 = Reader.ReadInt32();
            var unk2 = Reader.ReadInt32();
            var unk3 = Reader.ReadInt32();
            Name = Reader.ReadWideString(unk3-1);
            // var len = Reader.ReadInt32();
            Reader.ReadInt16();
            Data = Reader.ReadBytes((int) dataLen);


        }
    }
}