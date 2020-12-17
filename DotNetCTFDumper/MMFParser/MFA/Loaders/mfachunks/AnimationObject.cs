﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders.mfachunks
{
    public class AnimationObject:ObjectLoader
    {
        public List<Animation> Items = new List<Animation>();
        public override void Read()
        {
            base.Read();
            if(Reader.ReadByte()!=0)
            {
                var animationCount = Reader.ReadUInt32();
                for (int i = 0; i < animationCount; i++)
                {
                    var item = new Animation(Reader);
                    item.Read();
                    Items.Add(item);
                }
            }
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteInt8(1);
            Writer.WriteUInt32((uint) Items.Count);
            foreach (Animation animation in Items)
            {
                animation.Write(Writer);
            }
        }


        public AnimationObject(ByteReader reader) : base(reader) { }
    }

    public class Animation : DataLoader
    {
        public string Name = "Animation-UNKNOWN";
        public List<AnimationDirection> Directions;

        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32(Directions.Count);
            foreach (AnimationDirection direction in Directions)
            {
                direction.Write(Writer);
            }
        }

        public override void Print()
        {
            Logger.Log($"   Found animation: {Name} ");
        }

        public override void Read()
        {
            Name = Reader.AutoReadUnicode();
            var directionCount = Reader.ReadInt32();
            Directions = new List<AnimationDirection>();
            for (int i = 0; i < directionCount; i++)
            {
                var direction = new AnimationDirection(Reader);
                direction.Read();
                Directions.Add(direction);
            }
            


        }
        public Animation(ByteReader reader) : base(reader) { }
    }

    public class AnimationDirection : DataLoader
    {
        public string Name = "Animation-UNKNOWN";
        public int Index;
        public int MinSpeed;
        public int MaxSpeed;
        public int Repeat;
        public int BackTo;
        public List<int> Frames= new List<int>();

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Index);
            Writer.WriteInt32(MinSpeed);
            Writer.WriteInt32(MaxSpeed);
            Writer.WriteInt32(Repeat);
            Writer.WriteInt32(BackTo);
            Writer.WriteInt32(Frames.Count);
            foreach (int frame in Frames)
            {
                Writer.WriteInt32(frame);
            }
            
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Index = Reader.ReadInt32();
            MinSpeed = Reader.ReadInt32();
            MaxSpeed = Reader.ReadInt32();
            Repeat = Reader.ReadInt32();
            BackTo = Reader.ReadInt32();
            var animCount = Reader.ReadInt32();
            for (int i = 0; i < animCount; i++)
            {
                Frames.Add(Reader.ReadInt32()); 
            }

        }
        public AnimationDirection(ByteReader reader) : base(reader) { }
    }

}

