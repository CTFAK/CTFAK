using System;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System.Collections.Generic;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class ObjectInfo : ChunkLoader
    {
        public List<Chunk> Chunks = new List<Chunk>();
        public int Properties = 0;
        public string Name = "ERROR";
        public int Handle;
        public int ObjectType;
        public UInt32 Flags;
        public bool Transparent;
        public bool Antialias;
        public int InkEffect;
        public int InkEffectValue;
        public int ShaderId;
        public int Items;

        public ObjectInfo(Chunk chunk) : base(chunk)
        {
        }

        public ObjectInfo(ByteIO reader) : base(reader)
        {
        }

        public override void Print(bool ext)
        {
        }

        public override void Read()
        {
            var infoChunks = new ChunkList();
            infoChunks.Verbose = false;
            infoChunks.Read(Reader);

            foreach (var infoChunk in infoChunks.Chunks)
            {
                infoChunk.Verbose = false;
                var loader = infoChunk.Loader;
                if (loader is ObjectName)
                {
                    var actualLoader = infoChunks.get_loader<ObjectName>(loader);
                    Name = actualLoader.Value;
                }
                else if (loader is ObjectHeader)
                {
                    var actualLoader = infoChunks.get_loader<ObjectHeader>(loader);
                    Handle = actualLoader.Handle;
                    ObjectType = actualLoader.ObjectType;
                    Flags = actualLoader.Flags;
                    UInt32 inkEffect = actualLoader.InkEffect;
                    Transparent = ByteFlag.GetFlag(inkEffect, 28);
                    Antialias = ByteFlag.GetFlag(inkEffect, 29);
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
        public Int16 Handle;
        public Int16 ObjectType;
        public UInt32 Flags;
        public UInt32 InkEffect;
        public UInt32 InkEffectParameter;

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
            Handle = Reader.ReadInt16();
            ObjectType = Reader.ReadInt16();
            Flags = Reader.ReadUInt16();
            Int16 reserved = Reader.ReadInt16();
            InkEffect = Reader.ReadUInt32();
            InkEffectParameter = Reader.ReadUInt32();
        }
    }
}