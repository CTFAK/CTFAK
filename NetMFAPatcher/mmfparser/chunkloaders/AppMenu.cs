using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.mmfparser.chunkloaders
{
    public class AppMenu : ChunkLoader
    {
        public List<AppMenuItem> items = new List<AppMenuItem>();
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
            var current_position = reader.Tell();
            var header_size = reader.ReadUInt32();
            var menu_offset = reader.ReadInt32();
            var menu_size = reader.ReadInt32();
            if (menu_size == 0) return;
            var accel_offset = reader.ReadInt32();
            var accel_size = reader.ReadInt32();
            reader.Seek(current_position + menu_offset);
            reader.Skip(4);
            
            Load();
            
            reader.Seek(current_position + accel_offset);
            
            for (int i = 0; i < accel_size/8; i++)
            {
                reader.ReadByte();
                reader.Skip(1);
                reader.ReadInt16();
                reader.ReadInt16();
                reader.Skip(2);
            }

        }
        public void Load()
        {
            while(true)
            {
                var new_item = new AppMenuItem(reader);
                new_item.Read();
                items.Add(new_item);

                if (new_item.name.Contains("About")) break;
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
        public string name = "";
        public int flags = 0;
        public int id = 0;
        public string mnemonic = "";
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
            var flags = reader.ReadInt16();
            if (!ByteFlag.getFlag(flags,4))
            {
                id = reader.ReadInt16();
                
            }
            name = reader.ReadWideString();
            
            for (int i = 0; i < name.Length; i++)
            {
                if(name[i]=='&')
                {
                    mnemonic = name[i + 1].ToString().ToUpper();
                }
                name = name.Replace("&", "");
                
            }
            Console.WriteLine(name);
        }
        public void Load()
        {


        }
    }

}
