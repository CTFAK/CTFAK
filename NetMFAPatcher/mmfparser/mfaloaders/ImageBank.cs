using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmfparser.mfaloaders
{
    class AGMIBank : DataLoader
    {
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Logger.Log("TEX READ");
            var graphicMode = reader.ReadInt32();
            Logger.Log($"GraphicMode:{graphicMode}");
            var paletteVersion = reader.ReadInt16();
            Logger.Log($"PaletteVersion:{paletteVersion}");

            var paletteEntries = reader.ReadInt16();
            Logger.Log($"PaletteEntries:{paletteEntries}");


            for (int i = 0; i < 256; i++)
            {
                reader.ReadColor();
            }
            var count = reader.ReadInt32();
            Logger.Log($"Number of image items: {count.ToString()}");
            for (int i = 0; i < count; i++)
            {
                var item = new ImageItem(reader);
                item.Read();
                
        

            }
            
        }
        public AGMIBank(ByteIO reader) : base(reader)
        {
        }

        public AGMIBank(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }
}
