using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Extension:ParameterCommon
    {
        public short Size;
        public short Type;
        public short Code;
        public byte[] Data;

        public Extension(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Size = Reader.ReadInt16();
            Type = Reader.ReadInt16();
            Code = Reader.ReadInt16();
            Data = Reader.ReadBytes(Size-20);

        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) (Data.Length+6));
            Writer.WriteInt16(Type);
            Writer.WriteInt16(Code);
            Writer.WriteBytes(Data);
        }
    }
}