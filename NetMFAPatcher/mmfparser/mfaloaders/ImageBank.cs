using System;
using mmfparser;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.MFALoaders
{
    class AgmiBank : DataLoader
    {
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Logger.Log("TEX READ");
            var graphicMode = Reader.ReadInt32();
            Logger.Log($"GraphicMode:{graphicMode}");
            var paletteVersion = Reader.ReadInt16();
            Logger.Log($"PaletteVersion:{paletteVersion}");

            var paletteEntries = Reader.ReadInt16();
            Logger.Log($"PaletteEntries:{paletteEntries}");


            for (int i = 0; i < 256; i++)
            {
                Reader.ReadColor();
            }
            var count = Reader.ReadInt32();
            Logger.Log($"Number of image items: {count.ToString()}");
            for (int i = 0; i < count; i++)
            {

                var item = new ImageItem(Reader);
                item.IsCompressed = true;
                var currentPos = Reader.Tell();
                item.Read();

                

                
        

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
