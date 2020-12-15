using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Objects;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders
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
            return new string[]
            {
                $"Name: {this.Name}",
                $"Type: {(Constants.ObjectType)this.ObjectType}"
                
            };
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
                    
                    var actualLoader = (ObjectName)(loader);
                    Name = actualLoader.Value;
                }
                else if (loader is ObjectHeader)
                {
                    
                    var actualLoader = (ObjectHeader)(loader);
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
                Properties.ReadNew(ObjectType,this);
            }
        }

        public ImageItem GetPreview()
        {
            ImageItem bmp=null;
            var images = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>();
            if (ObjectType == 2)
            {
                    
                var anims = ((ObjectCommon) (Properties.Loader)).Animations;
                if (anims != null)
                {
                    anims.AnimationDict.TryGetValue(0,
                        out Animation anim);
                    anim.DirectionDict.TryGetValue(0, out AnimationDirection direction);
                    
                        var firstFrameHandle = direction.Frames[0];

                        if (images.Images[firstFrameHandle].Bitmap == null)
                        {
                            Console.WriteLine("Preloading "+firstFrameHandle);
                            images.LoadByHandle(firstFrameHandle);
                        }

                    bmp = images.Images[firstFrameHandle];
                }
            }
            else if (ObjectType == 1)
            {
                images.Images.TryGetValue(((Backdrop) Properties.Loader).Image, out var img);
                bmp = img;
            }

            return bmp;
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
        public ChunkLoader Loader;

        public ObjectProperties(ByteReader reader) : base(reader)
        {
        }

        public ObjectProperties(Chunk chunk) : base(chunk)
        {
        }

        public void ReadNew(int ObjectType,ObjectInfo parent)
        {
            
            //TODO: Fix shit
            Console.WriteLine("Reading properties of "+parent.Name);
            if (ObjectType == 1)//Backdrop
            {
                Loader = new Backdrop(Reader);
            }
            else if(ObjectType==2|| ObjectType==7)
            {
                IsCommon = true;
                Loader = new ObjectCommon(Reader,parent);
            }

            if (Loader != null)
            {
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