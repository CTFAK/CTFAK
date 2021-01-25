using System;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Group:ParameterCommon
    {
        public long Offset;
        public ushort Flags;
        public ushort Id;
        public string Name;
        public int Password;

        public Group(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Offset = Reader.Tell() - 24;
            Flags = Reader.ReadUInt16();
            Id = Reader.ReadUInt16();
            Name = Reader.ReadWideString();
            Password = Reader.ReadInt32();
            Password = Checksum.MakeGroupChecksum("pass", Name);
            Logger.Log("Password: "+Password);
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt16(Flags);
            Writer.WriteUInt16(Id);
            Writer.WriteUnicode(Name,true);
            Writer.WriteInt32(Password);
        }

        public override string ToString()
        {
            return $"Group: {Name}";
        }
    }
}