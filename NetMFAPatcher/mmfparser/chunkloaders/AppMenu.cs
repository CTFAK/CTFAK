using System;
using System.Collections.Generic;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.Data.ChunkList;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders
{
    public class AppMenu : ChunkLoader
    {
        public List<AppMenuItem> Items = new List<AppMenuItem>();
        public List<byte> AccelShift;
        public List<short> AccelKey;
        public List<short> AccelId;

        public AppMenu(ByteReader reader) : base(reader)
        {
        }

        public AppMenu(Chunk chunk) : base(chunk)
        {
        }
        public override void Print(bool ext)
        {
            
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var headerSize = Reader.ReadUInt32();
            var menuOffset = Reader.ReadInt32();
            var menuSize = Reader.ReadInt32();
            if (menuSize == 0) return;
            var accelOffset = Reader.ReadInt32();
            var accelSize = Reader.ReadInt32();
            Reader.Seek(currentPosition + menuOffset);
            Reader.Skip(4);
            
            Load(Reader);
            
            Reader.Seek(currentPosition + accelOffset);
            
            for (int i = 0; i < accelSize/8; i++)
            {
                AccelShift = new List<byte>();
                AccelKey = new List<short>();
                AccelId = new List<short>();
                AccelShift.Add(Reader.ReadByte());;
                Reader.Skip(1);
                AccelKey.Add(Reader.ReadInt16());
                AccelId.Add(Reader.ReadInt16());
                Reader.Skip(2);
            }

        }
        public void Load(ByteReader reader)
        {
            while(true)
            {
                var newItem = new AppMenuItem(reader);
                newItem.Read();
                Items.Add(newItem);

                if (newItem.Name.Contains("About")) break;
                if (ByteFlag.GetFlag((uint) newItem.Flags,4))
                {
                    Load(reader);
                    
                }
                if (ByteFlag.GetFlag((uint) newItem.Flags, 7))
                {

                    break;
                }


            }


        }
    }
    public class AppMenuItem : ChunkLoader
    {
        public string Name = "";
        public int Flags = 0;
        public int Id = 0;
        public string Mnemonic = "";
        public AppMenuItem(ByteReader reader) : base(reader)
        {
        }

        public AppMenuItem(Chunk chunk) : base(chunk)
        {
        }
        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            uint flags = (uint) Reader.ReadInt16();
            if (!ByteFlag.GetFlag(flags,4))
            {
                Id = Reader.ReadInt16();
                
            }
            Name = Reader.ReadWideString();
            
            for (int i = 0; i < Name.Length; i++)
            {
                if(Name[i]=='&')
                {
                    Mnemonic = Name[i + 1].ToString().ToUpper();
                }
                Name = Name.Replace("&", "");
                
            }
            Console.WriteLine(Name);
        }
        public void Load()
        {


        }
    }

}
