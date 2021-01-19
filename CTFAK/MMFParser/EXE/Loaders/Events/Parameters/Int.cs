using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class Int : Short
    {


        public Int(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Value = (short)Reader.ReadInt32();          
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Value);
        }
    }
}
