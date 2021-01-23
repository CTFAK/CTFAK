using System;
using System.Collections.Generic;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class ObjectInfo : ChunkLoader
    {
        public List<Chunk> Chunks = new List<Chunk>();
        public int ShaderId;
        public int Items;
        private ObjectHeader _header;
        private ObjectName _name;
        private ObjectProperties _properties;
        public ObjectInfo(Chunk chunk) : base(chunk){}
        public ObjectInfo(ByteReader reader) : base(reader){}
        public override void Print(bool ext){}
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

            _header = infoChunks.GetChunk<ObjectHeader>();
            _name = infoChunks.GetChunk<ObjectName>();
            _properties = infoChunks.GetChunk<ObjectProperties>();
            _properties.ReadNew((int) ObjectType,this);
            
        }

        public int Handle => _header.Handle;
        public string Name => _name.Value;
        public ObjectProperties Properties => _properties;
        public Constants.ObjectType ObjectType => (Constants.ObjectType) _header.ObjectType;
        public int Flags => (int) _header.Flags;
        public int InkEffect => (int) _header.InkEffect;
        public int InkEffectValue => (int) _header.InkEffectParameter;
        public bool Transparent => ByteFlag.GetFlag((uint) InkEffect, 28);
        public bool Antialias => ByteFlag.GetFlag((uint) InkEffect, 29);
        

        public ImageItem GetPreview()
        {
            ImageItem bmp=null;
            var images = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>();
            if (ObjectType == Constants.ObjectType.Active)
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
                            images.LoadByHandle(firstFrameHandle);
                        }

                    bmp = images.Images[firstFrameHandle];
                }
            }
            else if (ObjectType == Constants.ObjectType.Backdrop)
            {
                images.Images.TryGetValue(((Backdrop) Properties.Loader).Image, out var img);
                bmp = img;
            }
            else if (ObjectType==Constants.ObjectType.QuickBackdrop)
            {
                images.Images.TryGetValue(((Quickbackdrop) Properties.Loader).Image, out var img);
                bmp = img;
            }
            

            return bmp;
        }

        public List<ObjectInstance> GetInstances()
        {
            var list = new List<ObjectInstance>();
            var frames = Exe.Instance.GameData.Frames;
            foreach (var frame in frames)
            {
                foreach (ObjectInstance instance in frame.Objects)
                {
                    if(instance.ObjectInfo==this.Handle)list.Add(instance);
                }
            }

            return list;
        }
    }

    public class ObjectName : StringChunk
    {
        public ObjectName(ByteReader reader) : base(reader){}
        public ObjectName(Chunk chunk) : base(chunk){}
    }

    public class ObjectProperties : ChunkLoader
    {
        public bool IsCommon;
        public ChunkLoader Loader;

        public ObjectProperties(ByteReader reader) : base(reader){}
        public ObjectProperties(Chunk chunk) : base(chunk){}
        public void ReadNew(int ObjectType,ObjectInfo parent)
        {
            if(ObjectType==0) Loader=new Quickbackdrop(Reader);
            else if (ObjectType == 1) Loader = new Backdrop(Reader);
            else
            {
                IsCommon = true;
                Loader = new ObjectCommon(Reader,parent);
            }
            Loader?.Read();
        }
        public override void Read(){}
        public override void Print(bool ext){}

        public override string[] GetReadableData() => null;

    }

    public class ObjectHeader : ChunkLoader
    {
        public Int16 Handle;
        public Int16 ObjectType;
        public UInt32 Flags;
        public UInt32 InkEffect;
        public UInt32 InkEffectParameter;

        public ObjectHeader(ByteReader reader) : base(reader){}
        public ObjectHeader(Chunk chunk) : base(chunk){}
        public override void Print(bool ext){}
        public override string[] GetReadableData() => null;

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