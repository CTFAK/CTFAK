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
        public List<PlayerControl> items;

        public Controls(ByteIO reader) : base(reader)
        {
        }

        
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            items = new List<PlayerControl>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new mmfparser.mfaloaders.PlayerControl(reader);
                items.Add(item);
                item.Read();
            }
        }
    }

    class PlayerControl : DataLoader
    {
        int controlType;

        

        public PlayerControl(ByteIO reader) : base(reader)
        {
            
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            controlType = reader.ReadInt32();
            var count = reader.ReadInt32();
            var up = reader.ReadInt32();
            var down = reader.ReadInt32();
            var left = reader.ReadInt32();
            var right = reader.ReadInt32();
            var button1 = reader.ReadInt32();
            var button2 = reader.ReadInt32();
            var button3 = reader.ReadInt32();
            var button4 = reader.ReadInt32();
            for (int i = 0; i < 8; i++)
            {
                reader.ReadInt32();
            }



        }
    }




}
