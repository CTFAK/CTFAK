using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class Controls : DataLoader
    {
        public List<PlayerControl> Items;

        public Controls(ByteIO reader) : base(reader)
        {
        }

        
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Items = new List<PlayerControl>();
            var count = Reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new mmfparser.mfaloaders.PlayerControl(Reader);
                Items.Add(item);
                item.Read();
            }
        }
    }

    class PlayerControl : DataLoader
    {
        int _controlType;

        

        public PlayerControl(ByteIO reader) : base(reader)
        {
            
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            _controlType = Reader.ReadInt32();
            var count = Reader.ReadInt32();
            var up = Reader.ReadInt32();
            var down = Reader.ReadInt32();
            var left = Reader.ReadInt32();
            var right = Reader.ReadInt32();
            var button1 = Reader.ReadInt32();
            var button2 = Reader.ReadInt32();
            var button3 = Reader.ReadInt32();
            var button4 = Reader.ReadInt32();
            for (int i = 0; i < 8; i++)
            {
                Reader.ReadInt32();
            }



        }
    }




}
