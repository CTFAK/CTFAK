using System;
using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders
{
    public class Controls : DataLoader
    {
        public List<PlayerControl> Items;

        public Controls(ByteReader reader) : base(reader)
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
                var item = new PlayerControl(Reader);
                Items.Add(item);
                item.Read();
            }
        }

        public override void Write(ByteWriter writer)
        {
            writer.WriteInt32(Items.Count);
            foreach (var item in Items)
            {
                item.Write(writer);
            }
            
        }
        
    }

    public class PlayerControl : DataLoader
    {
        public int ControlType;
        public int Up;
        public int Down;
        public int Left;
        public int Right;
        public int Button1;
        public int Button2;
        public int Button3;
        public int Button4;


        public PlayerControl(ByteReader reader) : base(reader)
        {
            
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            ControlType = Reader.ReadInt32();
            var count = Reader.ReadInt32();
            Up = Reader.ReadInt32();
            Down = Reader.ReadInt32();
            Left = Reader.ReadInt32();
            Right = Reader.ReadInt32();
            Button1 = Reader.ReadInt32();
            Button2 = Reader.ReadInt32();
            Button3 = Reader.ReadInt32();
            Button4 = Reader.ReadInt32();
            for (int i = 0; i < 8; i++)
            {
                Reader.ReadInt32();
            }
        }

        public override void Write(ByteWriter writer)
        {
            writer.WriteInt32(ControlType);
            writer.WriteUInt32(16);
            writer.WriteInt32(Up);
            writer.WriteInt32(Down);
            writer.WriteInt32(Left);
            writer.WriteInt32(Right);
            writer.WriteInt32(Button1);
            writer.WriteInt32(Button2);
            writer.WriteInt32(Button3);
            writer.WriteInt32(Button4);


        }
    }




}
