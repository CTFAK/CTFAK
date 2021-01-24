using System;
using System.Collections.Generic;
using System.IO;
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

        public override void Write(ByteWriter Writer)
        {
            foreach (MFAChunk chunk in Items)
            {
                chunk.Write(Writer);
            }
            Writer.WriteInt8(0);
            return;
            
        } 
        

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var start = Reader.Tell();
            while(true)
            {
                var newChunk = new MFAChunk(Reader);
                newChunk.Read();
                if(Log)Logger.Log("ChunkID: "+newChunk.Id);
                if(newChunk.Id==0) break;
                else Items.Add(newChunk);
                
                

            }

            var size = Reader.Tell() - start;
            Reader.Seek(start);
            Saved = Reader.ReadBytes((int) size);


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
                default:
                    Loader = null;
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
        public abstract void Read();
        public abstract void Write(ByteWriter Writer);
    }
}
