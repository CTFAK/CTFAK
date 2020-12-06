using System;
using NetMFAPatcher.MMFParser.Data;
using System.Collections.Generic;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Objects;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class ObjectInfo : ChunkLoader
    {
        public List<Chunk> Chunks = new List<Chunk>();
        //public int Properties = 0;
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
        public ObjectProperties Properties;

        public ObjectInfo(Chunk chunk) : base(chunk)
        {
        }

        public ObjectInfo(ByteReader reader) : base(reader)
        {
        }

        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
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
                if (loader != null)
                {
                    Console.WriteLine($"Reading {loader.GetType().Name}");
                }
                
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
                else if (loader is ObjectProperties)
                {
                    
                    Properties = (ObjectProperties)loader;
                }
                
            }

            if (Properties != null)
            {
                //Properties.ReadNew(ObjectType);
            }
        }
    }

    public class ObjectName : StringChunk
    {
        public ObjectName(ByteReader reader) : base(reader)
        {
        }

        public ObjectName(Chunk chunk) : base(chunk)
        {
        }
    }

    public class ObjectProperties : ChunkLoader
    {
        public bool IsCommon;
        public ObjectCommon Loader;

        public ObjectProperties(ByteReader reader) : base(reader)
        {
        }

        public ObjectProperties(Chunk chunk) : base(chunk)
        {
        }

        public void ReadNew(int ObjectType)
        {
            Reader.Seek(0);
            //var objType = 2;//THIS IS SHITCODE
            IsCommon = true;//ITS NOT DONE
            if (ObjectType == 2)
            {
                Loader = new ObjectCommon(Reader);
                Loader.Read();
            }
            
        }

        public override void Read()
        {

        }


        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectHeader : ChunkLoader
    {
        public Int16 Handle;
        public Int16 ObjectType;
        public UInt32 Flags;
        public UInt32 InkEffect;
        public UInt32 InkEffectParameter;

        public ObjectHeader(ByteReader reader) : base(reader)
        {
        }

        public ObjectHeader(Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            return null;

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