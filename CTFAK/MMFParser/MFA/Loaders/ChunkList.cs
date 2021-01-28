using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
{
    public class ChunkList : DataLoader//This is used for MFA reading/writing
    {
        public byte[] Saved;
        public List<MFAChunk> Items = new List<MFAChunk>();
        public bool Log=false;

        public T GetOrCreateChunk<T>() where T : MFAChunkLoader, new()
        {
            foreach (MFAChunk chunk in Items)
            {
                if (chunk.Loader.GetType() == typeof(T))
                {
                    return (T) chunk.Loader;
                }
            }
            var newChunk = new MFAChunk(null);
            newChunk.Id=45;
            newChunk.Loader = new T();
            Items.Add(newChunk);
            return (T) newChunk.Loader;
        }

        public bool ContainsChunk<T>() where T : MFAChunkLoader
        {
            foreach (MFAChunk chunk in Items)
            {
                if (chunk.Loader.GetType() == typeof(T))
                {
                    return true;
                }
            }
            return false;
        }

        public MFAChunk NewChunk<T>() where T : MFAChunkLoader, new()
        {
            var newChunk = new MFAChunk(null);
                newChunk.Id = 33;
                newChunk.Loader = new T();
                return newChunk;
        }
        public override void Write(ByteWriter Writer)
        {
            foreach (MFAChunk chunk in Items)
            {
                chunk.Write(Writer);
            }
            Writer.WriteInt8(0);
        } 
        

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var start = base.Reader.Tell();
            while(true)
            {
                var newChunk = new MFAChunk(base.Reader);
                newChunk.Read();
                if(Log)Logger.Log("ChunkID: "+newChunk.Id);
                if(newChunk.Id==0) break;
                else Items.Add(newChunk);
                
                

            }

            var size = base.Reader.Tell() - start;
            base.Reader.Seek(start);
            Saved = base.Reader.ReadBytes((int) size);


        }
        public ChunkList(ByteReader reader) : base(reader) { }
    }


    public class MFAChunk
    {
        
        public ByteReader Reader;
        public MFAChunkLoader Loader;
        public byte Id;
        public byte[] Data;

        public MFAChunk(ByteReader reader)
        {
            Reader = reader;
        }
        public void Read()
        {
            Id = Reader.ReadByte();
            if (Id == 0) return;
            var size = Reader.ReadInt32();
            Data = Reader.ReadBytes(size);
            var dataReader = new ByteReader(Data);
            switch (Id)
            {
                case 33:
                    Loader = new FrameVirtualRect(dataReader);
                    break;
          
                case 45:
                    Loader = new Opacity(dataReader);
                    break;
                default:
                    Loader = null;
                    // Logger.Log($"{Id} - {Data.GetHex()}");
                    break;
                
            }
            Loader?.Read();
            
            

        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt8(Id);
            if (Id == 0) return;
            if (Loader == null)
            {
                writer.WriteInt32(Data.Length);
                writer.WriteBytes(Data); 
            }
            else
            {
                var newWriter = new ByteWriter(new MemoryStream());
                Loader.Write(newWriter);
                writer.WriteInt32((int) newWriter.Size());
                writer.WriteWriter(newWriter);
            }
            
            
            
        }
    }

    public class Opacity : MFAChunkLoader
    {
        public Color RGBCoeff;
        public byte Blend;

        public Opacity(ByteReader dataReader) : base(dataReader){}

        public Opacity() : base()
        {
            
        }


        public override void Read()
        {
            var b = Reader.ReadByte();
            var g = Reader.ReadByte();
            var r = Reader.ReadByte();
            Blend = Reader.ReadByte();
            RGBCoeff = Color.FromArgb(Blend,r,g,b);
            var unk = Reader.ReadInt32();

        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt8(RGBCoeff.B);
            Writer.WriteInt8(RGBCoeff.G);
            Writer.WriteInt8(RGBCoeff.R);
            Writer.WriteInt8(Blend);
            Writer.WriteInt32(0);
        }
    }

    public class FrameVirtualRect:MFAChunkLoader
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public FrameVirtualRect(ByteReader reader) : base(reader){}
        public override void Read()
        {
            Left = Reader.ReadInt32();
            Top = Reader.ReadInt32();
            Right = Reader.ReadInt32();
            Bottom = Reader.ReadInt32();
            
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Left);
            Writer.WriteInt32(Top);
            Writer.WriteInt32(Right);
            Writer.WriteInt32(Bottom);
        }
    }
    public abstract class MFAChunkLoader
    {
        public ByteReader Reader;
        protected MFAChunkLoader(ByteReader reader)
        {
            Reader = reader;
        }

        protected MFAChunkLoader(){}
        

        public abstract void Read();
        public abstract void Write(ByteWriter Writer);
    }
}
