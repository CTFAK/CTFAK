using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders
{
    public class FrameInstance:DataLoader
    {
        public int X;
        public int Y;
        public uint Layer;
        public int Handle;
        public uint Flags;
        public uint ParentType;
        public uint ParentHandle;
        public uint ItemHandle;

        public FrameInstance(ByteReader reader) : base(reader)
        {
        }

        public FrameInstance(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            X = Reader.ReadInt32();
            Y = Reader.ReadInt32();
            Layer = Reader.ReadUInt32();
            Handle = Reader.ReadInt32();
            Flags = Reader.ReadUInt32();
            ParentType = Reader.ReadUInt32();
            ItemHandle = Reader.ReadUInt32();
            ParentHandle = Reader.ReadUInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(X);
            Writer.WriteInt32(Y);
            Writer.WriteUInt32(Layer);
            Writer.WriteInt32(Handle);
            Writer.WriteUInt32(Flags);
            Writer.WriteUInt32(ParentType);
            Writer.WriteUInt32(ItemHandle);
            Writer.WriteUInt32(ParentHandle);
            
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}