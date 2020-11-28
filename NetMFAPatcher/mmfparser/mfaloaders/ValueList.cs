using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.utils;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class ValueList : DataLoader
    {
        public List<ValueItem> Items = new List<ValueItem>();
        public ValueList(ByteIO reader) : base(reader)
        {
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
                var item = new ValueItem(Reader);
                item.Read();
                Items.Add(item);

            }


        }
    }
    class ValueItem: DataLoader
    {
        public object Value;
        public string Name;

        public ValueItem(ByteIO reader) : base(reader)
        {
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Helper.AutoReadUnicode(Reader);
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
                    Value = Reader.ReadDouble();
                    break;
            }


        }
    }


}





