using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class TwoShorts:ParameterCommon
    {
        public short Value1;
        public short Value2;

        public TwoShorts(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            Value1 = Reader.ReadInt16();
            Value2 = Reader.ReadInt16();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteInt16(Value1);
            Writer.WriteInt16(Value2);
        }

        public override string ToString()
        {
            return $"Shorts: {Value1} and {Value2}";
        }
    }
}