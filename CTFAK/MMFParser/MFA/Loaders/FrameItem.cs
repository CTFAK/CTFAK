using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.MFA.Loaders.mfachunks;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
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
        public DataLoader Loader;

        public override void Write(ByteWriter Writer)
        {
            //Debug.Assert(ObjectType==2);
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
                Logger.Log("IconHandle: "+IconHandle);
            }
            else
            {
                throw new NotImplementedException("invalid icon");
            }
            Chunks = new ChunkList(Reader);
            // Chunks.Log = true;
            Chunks.Read();
            
            if (MFA.defaultObjChunks == null) MFA.defaultObjChunks = Chunks;
            if(ObjectType>=32)//extension base
            {
               Loader = new ExtensionObject(Reader);
               

            }
            else if(ObjectType==7)
            {
                Loader = new Counter(Reader);
            }
            else if(ObjectType==2)
            {
                Loader = new Active(Reader);
            }
            else throw new NotImplementedException("Unsupported object: "+ObjectType);
            Loader.Read();
            


        }
        public FrameItem(ByteReader reader):base(reader)
        { }
    }
}
