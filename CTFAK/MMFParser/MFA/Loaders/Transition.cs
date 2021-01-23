using System.Drawing;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
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
            Name = Reader.AutoReadUnicode();
            Id = Reader.ReadAscii(4);
            TransitionId = Reader.ReadAscii(4);
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

        public override void Print(){}
        
    }
}