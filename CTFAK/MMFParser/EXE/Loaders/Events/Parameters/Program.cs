using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Program:ParameterCommon
    {
        public short Flags;
        public string Filename;
        public string Command;

        public Program(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Flags = Reader.ReadInt16();
            Filename = Reader.ReadAscii(260);
            Command = Reader.ReadAscii();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16(Flags);
            Writer.WriteAscii(Filename);
            Writer.WriteAscii(Command);
        }
    }
}