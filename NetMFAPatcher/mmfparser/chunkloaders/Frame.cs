using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.chunkloaders
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
        ByteIO reader;
        public string name;
        public string password;
        public int width;
        public int height;
        public byte[] background;
        public int flags;
        int top;
        int bottom;
        int left;
        int right;


        public override void Print()
        {
            Logger.Log($"Frame: {name}", true, ConsoleColor.Green);
            Logger.Log($"   Password: {(password!=null ? password : "None")}", true, ConsoleColor.Green);
            Logger.Log($"   Size: {width}x{height}", true, ConsoleColor.Green);
            Logger.Log($"-------------------------", true, ConsoleColor.Green);
        }

        public override void Read()
        {
            var FrameReader = new ByteIO(chunk.chunk_data);
            var chunks = new ChunkList();

            chunks.verbose = false;
            chunks.Read(FrameReader);

            var name = chunks.get_chunk<FrameName>();
            if (name != null)
            {
                this.name = name.value;
            }
            var password = chunks.get_chunk<FramePassword>();
            if (password != null)
            {
                this.password = password.value;
            }
            var header = chunks.get_chunk<FrameHeader>();
            width = header.width;
            height = header.height;
            background = header.background;
            flags = header.flags;






            foreach (var item in chunks.chunks)
            {
                Directory.CreateDirectory($"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}");
                string path = $"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}\\{chunk.name}.chunk";
                File.WriteAllBytes(path, item.chunk_data);

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
        public int width;
        public int height;
        public int flags;
        public byte[] background;
        public FrameHeader(ByteIO reader) : base(reader)
        {
        }

        public FrameHeader(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print()
        {
            
        }

        public override void Read()
        {
            width = reader.ReadInt32();
            height = reader.ReadInt32();
            background = reader.ReadBytes(4);
            flags = (int)reader.ReadUInt32();
            
            

        }
    }
    class ObjectInstances : ChunkLoader
    {
        public int width;
        public int height;

        public ObjectInstances(ByteIO reader) : base(reader)
        {
        }

        public ObjectInstances(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print()
        {

        }

        public override void Read()
        { 



        }
    }


}