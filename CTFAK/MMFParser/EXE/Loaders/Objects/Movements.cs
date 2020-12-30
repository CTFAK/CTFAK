using System.Collections.Generic;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class Movements:ChunkLoader
    {
        public List<Movement> Items;

        public Movements(ByteReader reader) : base(reader)
        {
        }

        public Movements(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Items = new List<Movement>();
            var rootPosition = Reader.Tell();
            var count = Reader.ReadUInt32();
            var currentPos = Reader.Tell();
            for (int i = 0; i < count; i++)
            {
                var mov = new Movement(Reader);
                mov.rootPos = (int) rootPosition;
                mov.Read();
                Items.Add(mov);
                Reader.Seek(currentPos+16);
                currentPos = Reader.Tell();

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

    public class Movement : ChunkLoader
    {
        public int rootPos;
        public short Player;
        public short Type;
        public byte MovingAtStart;
        public int DirectionAtStart;

        public Movement(ByteReader reader) : base(reader)
        {
        }

        public Movement(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var nameOffset = Reader.ReadInt32();
            var movementId = Reader.ReadInt32();
            var newOffset = Reader.ReadInt32();
            var dataSize = Reader.ReadInt32();
            Reader.Seek(rootPos+newOffset);
            Player = Reader.ReadInt16();
            Type = Reader.ReadInt16();
            MovingAtStart = Reader.ReadByte();
            Reader.Skip(3);
            DirectionAtStart = Reader.ReadInt32();
            //TODO: Extension Stuff
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