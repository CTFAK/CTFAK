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
            base.Read();
            Offset = Reader.Tell() - 24;
            Flags = Reader.ReadUInt16();
            Id = Reader.ReadUInt16();
            Name = Reader.ReadWideString();
            Password = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteUInt16(Flags);
            Writer.WriteUInt16(Id);
            Writer.WriteAscii(Name);
            Writer.WriteInt32(Password);
        }

        public override string ToString()
        {
            return $"Group: {Name}";
        }
    }
}