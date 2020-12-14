using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders.mfachunks
{
    public class Behaviours : DataLoader
    {
        List<Behaviour> _items = new List<Behaviour>();
        public override void Write(ByteWriter Writer)
        {
            foreach (Behaviour behaviour in _items)
            {
                behaviour.Write(Writer);
            }
        }

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
        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            Writer.WriteBytes(Data.ReadBytes((int) Data.Size()));
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Reader.AutoReadUnicode();

            Data = new ByteReader(Reader.ReadBytes(Reader.ReadInt32()));
            
        }
        public Behaviour(ByteReader reader) : base(reader) { }
    }
}
