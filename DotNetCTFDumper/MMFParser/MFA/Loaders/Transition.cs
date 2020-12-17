using System;
using System.Drawing;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders
{
    public class Transition:DataLoader
    {
        public string Module;
        public string Name;
        public string Id;
        public string TransitionId;
        public int Duration;
        public int Flags;
        public Color Color;
        public byte[] ParameterData;

        public Transition(ByteReader reader) : base(reader)
        {
        }

        public Transition(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Module = Reader.AutoReadUnicode();
            Console.WriteLine(Module);
            Name = Reader.AutoReadUnicode();
            Console.WriteLine(Name);
            Id = Reader.ReadAscii(4);
            Console.WriteLine(Id);
            TransitionId = Reader.ReadAscii(4);
            Console.WriteLine(TransitionId);
            Duration = Reader.ReadInt32();
            Flags = Reader.ReadInt32();
            Color = Reader.ReadColor();
            ParameterData = Reader.ReadBytes(Reader.ReadInt32());

        }

        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Module);
            Writer.AutoWriteUnicode(Name);
            Writer.WriteAscii(Id);
            Writer.WriteAscii(TransitionId);
            Writer.WriteInt32(Duration);
            Writer.WriteInt32(Flags);
            Writer.WriteColor(Color);
            Writer.WriteInt32(ParameterData.Length);
            Writer.WriteBytes(ParameterData);
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}