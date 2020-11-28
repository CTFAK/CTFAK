using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class AppMenu : ChunkLoader
    {
        public List<AppMenuItem> Items = new List<AppMenuItem>();
        public AppMenu(ByteIO reader) : base(reader)
        {
        }

        public AppMenu(Chunk chunk) : base(chunk)
        {
        }
        public override void Print(bool ext)
        {
            
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
            
            Load();
            
            Reader.Seek(currentPosition + accelOffset);
            
            for (int i = 0; i < accelSize/8; i++)
            {
                Reader.ReadByte();
                Reader.Skip(1);
                Reader.ReadInt16();
                Reader.ReadInt16();
                Reader.Skip(2);
            }

        }
        public void Load()
        {
            while(true)
            {
                var newItem = new AppMenuItem(Reader);
                newItem.Read();
                Items.Add(newItem);

                if (newItem.Name.Contains("About")) break;
                if (true)//ByteFlag.getFlag(new_item.flags,4))
                {
                    Load();
                    
                }
                if (true)//ByteFlag.getFlag(new_item.flags, 7))
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
        public AppMenuItem(ByteIO reader) : base(reader)
        {
        }

        public AppMenuItem(Chunk chunk) : base(chunk)
        {
        }
        public override void Print(bool ext)
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
