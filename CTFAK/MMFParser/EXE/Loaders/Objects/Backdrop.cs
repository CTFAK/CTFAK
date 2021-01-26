using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public enum Obstacle
    {
        None = 0,
        Solid = 1,
        Platform = 2,
        Ladder = 3,
        Transparent = 4 
    }

    public enum Collision
    {
        Fine = 0,
        Box = 1
    }
    public class Backdrop:ChunkLoader
    {

        public int Size;
        public Obstacle ObstacleType;
        public Collision CollisionType;
        public int Width;
        public int Height;
        public int Image;
        
        public Backdrop(ByteReader reader) : base(reader)
        {
        }

        public Backdrop(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            if (Settings.GameType == GameType.OnePointFive)
            {
                Size = Reader.ReadInt32();
                ObstacleType = (Obstacle) Reader.ReadInt16();
                CollisionType = (Collision) Reader.ReadInt16();
                Image = Reader.ReadInt16();
            }
            else
            {
                Size = Reader.ReadInt32();
                ObstacleType = (Obstacle) Reader.ReadInt16();
                CollisionType = (Collision) Reader.ReadInt16();
                Width = Reader.ReadInt32();
                Height = Reader.ReadInt32();
                Image = Reader.ReadInt16();
                
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
    public class Quickbackdrop:ChunkLoader
    {
        public int Size;
        public Obstacle ObstacleType;
        public Collision CollisionType;
        public int Width;
        public int Height;
        public int Image;
        public Shape Shape;

        public Quickbackdrop(ByteReader reader) : base(reader)
        {
        }

        public Quickbackdrop(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Size = Reader.ReadInt32();
            ObstacleType = (Obstacle) Reader.ReadInt16();
            CollisionType = (Collision) Reader.ReadInt16();
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Shape = new Shape(Reader);
            Shape.Read();
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
    public class Shape:ChunkLoader
    {
        public short BorderSize;
        public Color BorderColor;
        public short ShapeType;
        public short FillType;
        public short LineFlags;
        public Color Color1;
        public Color Color2;
        public short GradFlags;
        public short Image=15;

        public Shape(ByteReader reader) : base(reader)
        {
        }

        public Shape(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            BorderSize = Reader.ReadInt16();
            BorderColor = Reader.ReadColor();
            ShapeType = Reader.ReadInt16();
            FillType = Reader.ReadInt16();
            if (ShapeType == 1)
            {
                LineFlags = Reader.ReadInt16();
            }
            else if (FillType == 1)
            {
                Color1 = Reader.ReadColor();
            }
            else if (FillType == 2)
            {
                Color1 = Reader.ReadColor();
                Color2 = Reader.ReadColor();
                GradFlags = Reader.ReadInt16();
            }
            // else if(FillType==3)
            // {
                Image = Reader.ReadInt16();
            // }
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
}