using System;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.MMFParser.MFALoaders.mfachunks;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders
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

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(this.ObjectType);
            Writer.WriteInt32(Handle);
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32(Transparent);
            Writer.WriteInt32(InkEffect);
            Writer.WriteInt32(InkEffectParameter);
            
            
        }

        public override void Print()
        {
            Console.WriteLine($"Name: {Name}");
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
                var iconHandle = Reader.ReadInt32();
            }
            else
            {
                throw new NotImplementedException("invalid icon");
            }
            var chunks = new ChunkList(Reader);
            chunks.Read();
            if(ObjectType>=32)//extension base
            {
               //TODO: Nonactives

            }
            else
            {
                var loader = new Active(Reader);
                loader.Read();
            }
           
            


        }
        public FrameItem(ByteReader reader):base(reader)
        { }
    }
}
