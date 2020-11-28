using NetMFAPatcher.mmfparser;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    class FrameName : StringChunk
    {
        public FrameName(ByteIO reader) : base(reader)
        {
        }

        public FrameName(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    class FramePassword : StringChunk
    {
        public FramePassword(ByteIO reader) : base(reader)
        {
        }

        public FramePassword(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    public class Frame : ChunkLoader
    {
        public string Name;
        public string Password;
        public int Width;
        public int Height;
        public byte[] Background;
        public int Flags;
        public int CountOfObjs;
        int _top;
        int _bottom;
        int _left;
        int _right;


        public override void Print(bool ext)
        {
            Logger.Log($"Frame: {Name}", true, ConsoleColor.Green);
            Logger.Log($"   Password: {(Password!=null ? Password : "None")}", true, ConsoleColor.Green);
            Logger.Log($"   Size: {Width}x{Height}", true, ConsoleColor.Green);
            Logger.Log($"   Objects: {CountOfObjs}", true, ConsoleColor.Green);
            Logger.Log($"-------------------------", true, ConsoleColor.Green);
        }

        public override void Read()
        {
            var frameReader = new ByteIO(Chunk.ChunkData);
            var chunks = new ChunkList();

            chunks.Verbose = false;
            chunks.Read(frameReader);

            var name = chunks.get_chunk<FrameName>();
            if (name != null) //Just to be sure
            {
                this.Name = name.Value;
            }
            var password = chunks.get_chunk<FramePassword>();
            if (password != null) //Just to be sure
            {
                this.Password = password.Value;
            }
            var header = chunks.get_chunk<FrameHeader>();
            Width = header.Width;
            Height = header.Height;
            Background = header.Background;
            Flags = header.Flags;
            var objects = chunks.get_chunk<ObjectInstances>();
            if(objects!=null)
            {
                CountOfObjs = objects.CountOfObjects;              
            }






            foreach (var item in chunks.Chunks)
            {
                //Directory.CreateDirectory($"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}");
                //string path = $"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}\\{chunk.name}.chunk";
                //File.WriteAllBytes(path, item.chunk_data);

            }
            
            


        }

        public Frame(ByteIO reader) : base(reader)
        {
        }

        public Frame(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    class FrameHeader : ChunkLoader
    {
        public int Width;
        public int Height;
        public int Flags;
        public byte[] Background;
        public FrameHeader(ByteIO reader) : base(reader)
        {
        }

        public FrameHeader(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
            
        }

        public override void Read()
        {
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Background = Reader.ReadBytes(4);
            Flags = (int)Reader.ReadUInt32();
            
            

        }
    }
    class ObjectInstances : ChunkLoader
    {
        
        public int CountOfObjects=0;
        public List<ObjectInstances> Items = new List<ObjectInstances>();

        public ObjectInstances(ByteIO reader) : base(reader)
        {
        }

        public ObjectInstances(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {

        }

        public override void Read()
        {
            
            CountOfObjects = (int)Reader.ReadUInt32();
            return;
            for (int i = 0; i < CountOfObjects; i++)
            {
                var item = new ObjectInstances(Reader);
                item.Read();
                Items.Add(item);
            }




        }
    }


}