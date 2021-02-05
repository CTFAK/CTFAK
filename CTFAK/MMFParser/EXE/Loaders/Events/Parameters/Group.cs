using System;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Group : ParameterCommon
    {
        public long Offset;
        public ushort Flags;
        public ushort Id;
        public string Name;
        public int Password;
        private int _unk;
        public byte[] Unk1;
        public byte[] Unk2;

        public Group(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Offset = Reader.Tell() - 24;
            Flags = Reader.ReadUInt16();
            Id = Reader.ReadUInt16();
            Name = Reader.ReadWideString();
            Unk1 = Reader.ReadBytes(190-Name.Length*2);
            Password = Reader.ReadInt32();
            Logger.Log("Mine: "+Checksum.MakeGroupChecksum(Name,""));
            Logger.Log("Orig: "+Password);
            Password = (int) Checksum.MakeGroupChecksum(Name, "");
            Reader.ReadInt16();

        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt16(Flags);
            Writer.WriteUInt16(Id);
            Writer.WriteUnicode(Name, true);
            Writer.WriteBytes(Unk1);
            Writer.WriteInt32(Password);
            Writer.WriteInt16(0);
        }

        public override string ToString()
        {
            return $"Group: {Name}";
        }
    }
}