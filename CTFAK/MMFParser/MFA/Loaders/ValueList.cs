﻿using System;
using System.Collections.Generic;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
{
    public class ValueList : DataLoader
    {
        public List<ValueItem> Items = new List<ValueItem>();
        public ValueList(ByteReader reader) : base(reader)
        {
        }
        
        public override void Print(){}
        

        public override void Read()
        {
            var count = Reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new ValueItem(Reader);
                item.Read();
                Items.Add(item);
            }
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Items.Count);
            foreach (var item in Items) item.Write(Writer);
        }
    }
    public class ValueItem: DataLoader
    {
        public object Value;
        public string Name;

        public ValueItem(ByteReader reader) : base(reader)
        {
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Reader.AutoReadUnicode();
            var type = Reader.ReadInt32();
            switch (type)
            {
                case 2://string
                    Value = Helper.AutoReadUnicode(Reader);
                    break;
                case 0://int
                    Value = Reader.ReadInt32();
                    break;
                case 1://double
                    Value = Reader.ReadSingle();
                    break;
            }
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            if (Value is string)
            {
                Writer.WriteInt32(2);
                Writer.AutoWriteUnicode((string)Value);
            }
            else if (Value is int)
            {
                Writer.WriteInt32(0);
                Writer.WriteInt32((int)Value);
            }
            else if (Value is double || Value is float)
            {
                Writer.WriteInt32(1);
                Writer.WriteSingle((float)Value);
            }
        }
    }


}





