using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class Int : Short
    {
        public int Value;

        public Int(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Value = Reader.ReadInt32();          
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Value);
        }
    }
}
