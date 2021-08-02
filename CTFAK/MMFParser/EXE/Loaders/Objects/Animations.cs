using System;
using System.Collections.Generic;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class Animations:ChunkLoader
    {
        public Dictionary<int, Animation> AnimationDict;

        public Animations(ByteReader reader) : base(reader)
        {
        }

        

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(-4);
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt16();
            var count = Reader.ReadInt16();
            //if (Settings.GameType == GameType.TwoFivePlus) Console.WriteLine("reading animation - size " + Reader.Size() + ", there are " + count + " imgs @ "+currentPosition);
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
                else
                {
                    AnimationDict.Add(i,new Animation((ByteReader) null));
                }

            }
            


        }

        public override void Write(ByteWriter Writer)
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

       

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(-4);
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
                else
                {
                    DirectionDict.Add(i,new AnimationDirection((ByteReader) null));
                }
                
            }
        }

        public override void Write(ByteWriter Writer)
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

       

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(4);
            //if (Settings.GameType == GameType.TwoFivePlus) Console.WriteLine("current position: " + Reader.Tell() + " out of size " + Reader.Size());
            long currentPosition = Reader.Tell();
            if (currentPosition >= Reader.Size())
            {
                Console.WriteLine("Rewinding Animations buffer to 4");
                Reader.Seek(4);
                currentPosition = 4;
            }
            if (Reader.Tell() > Reader.Size() - 10)
            {
                Console.WriteLine("E136: Ran out of bytes reading Animations (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            MinSpeed = Reader.ReadSByte();
            MaxSpeed = Reader.ReadSByte();
            Repeat = Reader.ReadInt16();
            BackTo = Reader.ReadInt16();
            var frameCount = Reader.ReadUInt16();
            if (frameCount > 250) //idk
            {
                Console.WriteLine("Invalid amount of frames, skipping");
                if (Settings.GameType == GameType.TwoFivePlus) Console.WriteLine("the frame count is " + frameCount + "/" + MinSpeed + "." + MaxSpeed + "." + Repeat + "." + BackTo);
                return;
            }
            for (int i = 0; i < frameCount; i++)
            {
                if (Reader.Tell() > Reader.Size() - 2)
                {
                    Console.WriteLine("E154: Ran out of bytes reading frame #"+i+"/"+frameCount+" in Animations Frames (" + Reader.Tell() + "/" + Reader.Size() + ")");
                    break;
                }
                var handle = Reader.ReadInt16();
                //if (Settings.GameType == GameType.TwoFivePlus) Console.WriteLine("adding image #" + i);
                Frames.Add(handle);
                
                
            }
            

        }

        public override void Write(ByteWriter Writer)
        {
            throw new System.NotImplementedException();
        }


        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}