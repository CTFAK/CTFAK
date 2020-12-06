using System;
using System.Collections.Generic;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.MFALoaders.mfachunks
{
    class Movements : DataLoader
    {
        public List<Movement> Items = new List<Movement>();
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = Reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new Movement(Reader);
                item.Read();
                Items.Add(item);

            }


        }
        public Movements(ByteIO reader) : base(reader) { }
    }
    class Movement : DataLoader
    {
        public string Name="ERROR";

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Helper.AutoReadUnicode(Reader);
            var extension = Helper.AutoReadUnicode(Reader);
            var identifier = Reader.ReadInt32();
            var dataSize = Reader.ReadInt32();
            if(extension.Length>0)
            {
                var newReader = new ByteIO(Reader.ReadBytes(dataSize));


            }
            else
            {
                var player = Reader.ReadInt16();
                var type = Reader.ReadInt16();
                var movingAtStart = Reader.ReadByte();
                Reader.Skip(3);
                var directionAtStart = Reader.ReadInt32();
                //implement types, but i am tired, fuck this shit
            }

        }
        public Movement(ByteIO reader) : base(reader) { }
    }
}
