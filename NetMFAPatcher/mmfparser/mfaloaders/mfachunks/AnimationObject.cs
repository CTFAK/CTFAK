using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders.mfachunks
{
    class AnimationObject:ObjectLoader
    {
        List<Animation> items = new List<Animation>();
        public override void Read()
        {
            base.Read();
            if(reader.ReadByte()!=0)
            {
                var animationCount = reader.ReadInt32();
                for (int i = 0; i < animationCount; i++)
                {
                    var item = new Animation(reader);
                    item.Read();
                    items.Add(item);
                }
            }

        }

        public AnimationObject(ByteIO reader) : base(reader) { }
    }
    class Animation : DataLoader
    {
        public string name = "Animation-UNKNOWN";
        public override void Print()
        {
            Logger.Log($"   Found animation: {name} ");
        }

        public override void Read()
        {
            name = reader.ReadAscii(reader.ReadInt32());
            var directionCount = reader.ReadInt32();
            var directions = new List<AnimationDirection>();
            for (int i = 0; i < directionCount; i++)
            {
                var direction = new AnimationDirection(reader);
                direction.Read();
                directions.Add(direction);
            }
            


        }
        public Animation(ByteIO reader) : base(reader) { }
    }
    class AnimationDirection : DataLoader
    {
        public string name = "Animation-UNKNOWN";
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var index = reader.ReadInt32();
            var minSpeed = reader.ReadInt32();
            var maxSpeed= reader.ReadInt32();
            var repeat= reader.ReadInt32();
            var backTo= reader.ReadInt32();
            var frames = new List<int>();

        }
        public AnimationDirection(ByteIO reader) : base(reader) { }
    }

}

