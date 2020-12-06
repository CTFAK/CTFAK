using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.MFALoaders
{
    public class AgmiBank : DataLoader
    {
        private int GraphicMode;
        private int PaletteVersion;
        private int PaletteEntries;
        public Dictionary<int,ImageItem> Items = new Dictionary<int, ImageItem>();
        private List<Color> Palette;

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Logger.Log("TEX READ");
            GraphicMode = Reader.ReadInt32();
            Logger.Log($"GraphicMode:{GraphicMode}");
            PaletteVersion = Reader.ReadInt16();
            Logger.Log($"PaletteVersion:{PaletteVersion}");

            PaletteEntries = Reader.ReadInt16();
            Logger.Log($"PaletteEntries:{PaletteEntries}");

            Palette = new List<Color>();//Color[256];
            for (int i = 0; i < 256; i++)
            {
                Palette.Add(Reader.ReadColor());
            }
            var count = Reader.ReadInt32();
            Logger.Log($"Number of image items: {count.ToString()}");
            
            for (int i = 0; i < count; i++)
            {

                var item = new ImageItem(Reader);
                item.Debug = true;
                item.Read();
                item.Save($"{Settings.ImagePath}\\{i}.png");
                Items.Add(item.Handle,item);
                
            }
            
        }

        

        public void Write(ByteWriter writer)
        {
            writer.WriteInt32(GraphicMode);
            writer.WriteInt16((short) PaletteVersion);
            writer.WriteInt16((short) PaletteEntries);
            for (int i = 0; i < 256; i++)
            {
                writer.WriteColor(Palette[i]);
            }
            writer.WriteInt32(Items.Count);
            foreach (var key in Items.Keys)
            {
                Items[key].Write(writer);
            }
        }
        public AgmiBank(ByteIO reader) : base(reader)
        {
        }

        public AgmiBank(Chunk chunk) : base(chunk)
        {
        }
    }
}
