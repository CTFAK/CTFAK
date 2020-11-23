using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System.Collections.Generic;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.mmfparser.chunkloaders
{
    class ObjectInfo : ChunkLoader
    {
        public List<Chunk> chunks = new List<Chunk>();
        public int properties = 0;
        public string name = "ERROR";
        public int handle;
        public int objectType;
        public int flags;
        public bool transparent;
        public bool antialias;
        public int inkEffect;
        public int inkEffectValue;
        public int shaderId;
        public int items;
        public ObjectInfo(Chunk chunk) : base(chunk) { }
        public ObjectInfo(ByteIO reader) : base(reader) { }
        public override void Print(bool ext)
        {
            
        }

        public override void Read()
        {
            var infoChunks = new ChunkList();
            infoChunks.verbose = false;
            infoChunks.Read(reader);
            
            foreach (var chunk in infoChunks.chunks)
            {
                chunk.verbose = false;
                var loader = chunk.loader;
                if(loader is ObjectName)
                {
                    var actualLoader = infoChunks.get_loader<ObjectName>(loader);
                    name = actualLoader.value;

                }
                else if(loader is ObjectHeader)
                {
                    var actualLoader = infoChunks.get_loader<ObjectHeader>(loader);
                    handle = actualLoader.handle;
                    objectType = actualLoader.objectType;
                    flags = actualLoader.flags;
                    var inkEffect = actualLoader.inkEffect;
                    transparent = ByteFlag.getFlag(inkEffect, 28);
                    antialias = ByteFlag.getFlag(inkEffect, 29);


                }    



            }
            
        }
    }
    class ObjectName : StringChunk
    {
        public ObjectName(ByteIO reader) : base(reader)
        {
        }

        public ObjectName(Chunk chunk) : base(chunk)
        {
        }
    }
    class ObjectHeader : ChunkLoader
    {
        public int handle;
        public int objectType;
        public int flags;
        public int inkEffect;
        public int inkEffectParameter;
        public ObjectHeader(ByteIO reader) : base(reader)
        {
        }

        public ObjectHeader(Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
            
        }

        public override void Read()
        {
            handle = reader.ReadInt16();
            objectType = reader.ReadInt16();
            flags = reader.ReadUInt16();
            var reserved = reader.ReadInt16();
            inkEffect = (int)reader.ReadUInt32();
            inkEffectParameter = (int)reader.ReadUInt32();
            
        }
    }
}
