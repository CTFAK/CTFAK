using System;
using System.Diagnostics;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.MFA.Loaders.mfachunks;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders
{
    public class FrameItem : DataLoader
    {
        public int ObjectType;
        public int Handle;
        public string Name;
        public int Transparent;
        public int InkEffect;
        public int InkEffectParameter;
        public int AntiAliasing;
        public int Flags;
        public int IconType;
        public int IconHandle;
        public ChunkList Chunks;
        public Active Loader;

        public override void Write(ByteWriter Writer)
        {
            Debug.Assert(ObjectType==2);
            Writer.WriteInt32(this.ObjectType);
            Writer.WriteInt32(Handle);
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32(Transparent);
            Writer.WriteInt32(InkEffect);
            Writer.WriteInt32(InkEffectParameter);
            Writer.WriteInt32(AntiAliasing);
            Writer.WriteInt32(Flags);
            Writer.WriteInt32(1);
            Writer.WriteInt32(IconHandle);
            Chunks.Write(Writer);
            Loader.Write(Writer);
            


        }

        public override void Print()
        {
            
        }

        public override void Read()
        {
            
            ObjectType = Reader.ReadInt32();
            Handle = Reader.ReadInt32();
            Name = Helper.AutoReadUnicode(Reader);
            Transparent = Reader.ReadInt32();
            
            InkEffect = Reader.ReadInt32();
            InkEffectParameter = Reader.ReadInt32();
            AntiAliasing = Reader.ReadInt32();

            Flags = Reader.ReadInt32();
            
            IconType = Reader.ReadInt32();
            if(IconType==1)
            {
                IconHandle = Reader.ReadInt32();
            }
            else
            {
                throw new NotImplementedException("invalid icon");
            }
            Chunks = new ChunkList(Reader);
            Chunks.Read();
            if(ObjectType>=32)//extension base
            {
               //TODO: Nonactives

            }
            else if(ObjectType==2)
            {
                Loader = new Active(Reader);
                Loader.Read();
            }
           
            


        }
        public FrameItem(ByteReader reader):base(reader)
        { }
    }
}
