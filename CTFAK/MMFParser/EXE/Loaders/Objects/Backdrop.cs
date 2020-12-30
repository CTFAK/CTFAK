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
            Size = Reader.ReadInt32();
            ObstacleType = (Obstacle) Reader.ReadInt16();
            CollisionType = (Collision) Reader.ReadInt16();
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Image = Reader.ReadInt16();

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