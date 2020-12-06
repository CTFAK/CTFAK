using System;
using System.Collections.Generic;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.MFALoaders.mfachunks
{
    class Behaviours : DataLoader
    {
        List<Behaviour> _items = new List<Behaviour>();
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = Reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new Behaviour(Reader);
                item.Read();
                _items.Add(item);
            }
        }
        public Behaviours(ByteReader reader) : base(reader) { }
    }
    class Behaviour : DataLoader
    {
        public string Name = "ERROR";
        public ByteReader Data;
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Reader.ReadAscii(Reader.ReadInt32());
            Data = new ByteReader(Reader.ReadBytes(Reader.ReadInt32()));
            
        }
        public Behaviour(ByteReader reader) : base(reader) { }
    }
}
