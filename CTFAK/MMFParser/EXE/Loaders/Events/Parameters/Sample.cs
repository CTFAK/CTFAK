using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class Sample : ParameterCommon
    {
        public int Handle;
        public string Name;
        public int Flags;

        public Sample(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Handle = Reader.ReadInt16();
            Flags = Reader.ReadUInt16();
            Name = Reader.ReadWideString();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) Handle);
            Writer.WriteUInt16((ushort) Flags);
            Name = Name.Replace(" ", "");
            Writer.WriteUnicode(Name);
            Writer.Skip(120);
            Writer.WriteInt16(0);
        }

        public override string ToString()
        {
            return $"Sample '{Name}' handle: {Handle}";
        }
    }
}
