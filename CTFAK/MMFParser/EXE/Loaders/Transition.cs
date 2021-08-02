using System;
using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class Transition:ChunkLoader
    {
        public string Module;
        public string Name;
        public int Duration;
        public int Flags;
        public Color Color;
        public string ModuleFile;
        public byte[] ParameterData;

        public Transition(ByteReader reader) : base(reader)
        {
        }

   

        public override void Read()
        {
            var currentPos = Reader.Tell();
            if (Reader.Tell() > Reader.Size() - 4)
            {
                Console.WriteLine("E28:  Ran out of bytes reading Transitions (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            Module = Reader.ReadAscii(4);
            Name = Reader.ReadAscii(4);
            Duration = Reader.ReadInt32();
            Flags = Reader.ReadInt32();
            Color = Reader.ReadColor();
            var nameOffset = Reader.ReadInt32();
            var parameterOffset = Reader.ReadInt32();
            var parameterSize = Reader.ReadInt32();
            Reader.Seek(currentPos+nameOffset);
            ModuleFile = Reader.ReadAscii();
            Reader.Seek(currentPos+parameterOffset);
            ParameterData = Reader.ReadBytes(parameterSize);
      


        }

        public override void Write(ByteWriter Writer)
        {
            throw new System.NotImplementedException();
        }
        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}