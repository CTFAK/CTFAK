using System.Collections.Generic;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK.MMFParser.OLD.Loaders
{
    public class Controls:ChunkLoader
    {
        public List<PlayerControl> Items;

        public Controls(ByteReader reader) : base(reader)
        {
        }

        public Controls(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Items = new List<PlayerControl>();
            for (int i = 0; i < 4; i++)
            {
                var control = new PlayerControl(Reader);
                control.Read();
                Items.Add(control);
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
    public class PlayerControl:ChunkLoader
    {
        public Keys Keys;

        public PlayerControl(ByteReader reader) : base(reader)
        {
        }

        public PlayerControl(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Keys = new Keys(Reader);
            Keys.Read();
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
    public class Keys:ChunkLoader
    {
        public Keys(ByteReader reader) : base(reader)
        {
        }

        public Keys(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var up = Reader.ReadUInt16();
            var down = Reader.ReadUInt16();
            var left = Reader.ReadUInt16();
            var right = Reader.ReadUInt16();
            var btn1 = Reader.ReadUInt16();
            var btn2 = Reader.ReadUInt16();
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