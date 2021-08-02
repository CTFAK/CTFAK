using CTFAK.Utils;
using System;

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
            if (Reader.Tell() > Reader.Size() - 263)
            {
                Console.WriteLine("E20:  Ran out of bytes reading Event Parameters (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            Flags = Reader.ReadInt16();
            Filename = Reader.ReadAscii(260);
            Command = Reader.ReadAscii();
        }

        public override void Write(ByteWriter Writer)
        {
            if (Filename == null || Command == null) return;
            Writer.WriteInt16(Flags);
            Writer.WriteAscii(Filename);
            Writer.WriteAscii(Command);
        }
    }
}