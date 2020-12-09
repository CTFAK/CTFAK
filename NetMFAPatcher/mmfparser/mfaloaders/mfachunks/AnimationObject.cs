using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders.mfachunks
{
    class AnimationObject:ObjectLoader
    {
        List<Animation> _items = new List<Animation>();
        public override void Read()
        {
            base.Read();
            if(Reader.ReadByte()!=0)
            {
                var animationCount = Reader.ReadInt32();
                for (int i = 0; i < animationCount; i++)
                {
                    var item = new Animation(Reader);
                    item.Read();
                    _items.Add(item);
                }
            }

        }

        public AnimationObject(ByteReader reader) : base(reader) { }
    }
    class Animation : DataLoader
    {
        public string Name = "Animation-UNKNOWN";
        public override void Print()
        {
            Logger.Log($"   Found animation: {Name} ");
        }

        public override void Read()
        {
            Name = Reader.ReadAscii(Reader.ReadInt32());
            var directionCount = Reader.ReadInt32();
            var directions = new List<AnimationDirection>();
            for (int i = 0; i < directionCount; i++)
            {
                var direction = new AnimationDirection(Reader);
                direction.Read();
                directions.Add(direction);
            }
            


        }
        public Animation(ByteReader reader) : base(reader) { }
    }
    class AnimationDirection : DataLoader
    {
        public string Name = "Animation-UNKNOWN";
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var index = Reader.ReadInt32();
            var minSpeed = Reader.ReadInt32();
            var maxSpeed= Reader.ReadInt32();
            var repeat= Reader.ReadInt32();
            var backTo= Reader.ReadInt32();
            var frames = new List<int>();

        }
        public AnimationDirection(ByteReader reader) : base(reader) { }
    }

}

