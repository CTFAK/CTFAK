using System;
using System.Collections.Generic;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class Behaviours : DataLoader
    {
        List<Behaviour> _items = new List<Behaviour>();
        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(_items.Count);
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
        public byte[] Data;
        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            Writer.WriteUInt32((uint) Data.Length);
            Writer.WriteBytes(Data);
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Reader.AutoReadUnicode();

            Data = Reader.ReadBytes((int) Reader.ReadUInt32());
            
        }
        public Behaviour(ByteReader reader) : base(reader) { }
    }
}
