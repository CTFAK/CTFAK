using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class ValueList : DataLoader
    {
        public List<ValueItem> items = new List<ValueItem>();
        public ValueList(ByteIO reader) : base(reader)
        {
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new ValueItem(reader);
                item.Read();
                items.Add(item);

            }


        }
    }
    class ValueItem: DataLoader
    {
        public object value;
        public string name;

        public ValueItem(ByteIO reader) : base(reader)
        {
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            name = reader.ReadAscii(reader.ReadInt32());
            var type = reader.ReadInt32();
            switch (type)
            {
                case 2://string
                    value = reader.ReadAscii(reader.ReadInt32());
                    break;
                case 0://int
                    value = reader.ReadInt32();
                    break;
                case 1://double
                    value = reader.ReadDouble();
                    break;
            }


        }
    }


}





