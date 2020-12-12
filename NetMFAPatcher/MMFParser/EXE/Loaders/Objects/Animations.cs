using System;
using System.Collections.Generic;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders.Objects
{
    public class Animations:ChunkLoader
    {
        public Dictionary<int, Animation> AnimationDict;

        public Animations(ByteReader reader) : base(reader)
        {
        }

        public Animations(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt16();
            var count = Reader.ReadInt16();
            var offsets = new List<short>();
            for (int i = 0; i < count; i++)
            {
                offsets.Add(Reader.ReadInt16());
            }
            AnimationDict = new Dictionary<int,Animation>();
            for (int i = 0; i < offsets.Count; i++)
            {
                var offset = offsets[i];
                if (offset != 0)
                {
                    Reader.Seek(currentPosition+offset);
                    var anim = new Animation(Reader);
                    anim.Read();
                    AnimationDict.Add(i,anim);
                    
                }

            }
            


        }

        public override void Print(bool ext)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Animation : ChunkLoader
    {
        public Dictionary<int, AnimationDirection> DirectionDict;

        public Animation(ByteReader reader) : base(reader)
        {
        }

        public Animation(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var offsets = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                offsets.Add(Reader.ReadInt16());
            }
            
            DirectionDict = new Dictionary<int,AnimationDirection>();
            for (int i = 0; i < offsets.Count; i++)
            {
                var offset = offsets[i];
                if (offset != 0)
                {
                    Reader.Seek(currentPosition+offset);
                    var dir = new AnimationDirection(Reader);
                    dir.Read();
                    DirectionDict.Add(i,dir);
                }
            }
        }

        public override void Print(bool ext)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AnimationDirection : ChunkLoader
    {
        public int MinSpeed;
        public int MaxSpeed;
        public bool HasSingle;
        public int Repeat;
        public int BackTo;
        public List<int> Frames = new List<int>();
        public AnimationDirection(ByteReader reader) : base(reader)
        {
        }

        public AnimationDirection(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            MinSpeed = Reader.ReadSByte();
            MaxSpeed = Reader.ReadSByte();
            Repeat = Reader.ReadInt16();
            BackTo = Reader.ReadInt16();
            var frameCount = Reader.ReadUInt16();
            for (int i = 0; i < frameCount; i++)
            {
                var handle = Reader.ReadInt16();
                Frames.Add(handle);
                Console.WriteLine("Frame Found: "+handle);
                
            }
            

        }

        public override void Print(bool ext)
        {
            
            
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}