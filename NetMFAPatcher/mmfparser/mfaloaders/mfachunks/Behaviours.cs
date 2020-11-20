using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders.mfachunks
{
    class Behaviours : DataLoader
    {
        List<Behaviour> items = new List<Behaviour>();
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new Behaviour(reader);
                item.Read();
                items.Add(item);
            }
        }
        public Behaviours(ByteIO reader) : base(reader) { }
    }
    class Behaviour : DataLoader
    {
        public string name = "ERROR";
        public ByteIO data;
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            name = reader.ReadAscii(reader.ReadInt32());
            data = new ByteIO(reader.ReadBytes(reader.ReadInt32()));
            
        }
        public Behaviour(ByteIO reader) : base(reader) { }
    }
}
