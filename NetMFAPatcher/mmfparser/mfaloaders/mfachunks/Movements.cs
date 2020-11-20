using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders.mfachunks
{
    class Movements : DataLoader
    {
        public List<Movement> items = new List<Movement>();
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new Movement(reader);
                item.Read();
                items.Add(item);

            }


        }
        public Movements(ByteIO reader) : base(reader) { }
    }
    class Movement : DataLoader
    {
        public string name="ERROR";

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            name = reader.ReadAscii(reader.ReadInt32());
            var extension = reader.ReadBytes(reader.ReadInt32());
            var identifier = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            if(extension.Length>0)
            {
                var newReader = new ByteIO(reader.ReadBytes(dataSize));


            }
            else
            {
                var player = reader.ReadInt16();
                var type = reader.ReadInt16();
                var movingAtStart = reader.ReadByte();
                reader.Skip(3);
                var directionAtStart = reader.ReadInt32();
                //implement types, but i am tired, fuck this shit
            }

        }
        public Movement(ByteIO reader) : base(reader) { }
    }
}
