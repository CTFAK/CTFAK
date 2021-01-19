using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Click:ParameterCommon
    {
        public byte IsDouble;
        public byte Button;

        public Click(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Button = Reader.ReadByte();
            IsDouble = Reader.ReadByte();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt8(Button);
            Writer.WriteInt8(IsDouble);
            
        }

        public override string ToString()
        {
            return $"{Button}-{IsDouble}";
        }
    }
}