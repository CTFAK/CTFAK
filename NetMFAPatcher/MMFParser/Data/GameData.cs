using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.mmfparser.Constants;

namespace NetMFAPatcher.MMFParser.Data
{
    public class GameData
    {
        public int runtime_version;
        public int runtime_subversion;
        public int product_build;
        public int product_version;
        public Products build;
        public ChunkList gameChunks;
        public void Read(ByteIO exeReader)
        {
            string magic = exeReader.ReadAscii(4);


            if (magic == Constants.UNICODE_GAME_HEADER) Constants.isUnicode = true;
            else if (magic == Constants.GAME_HEADER) Constants.isUnicode = false;
            else
            {
                Logger.Log("Header Fucked Up", true, ConsoleColor.Red);
            }

            runtime_version = exeReader.ReadUInt16();
            runtime_subversion = exeReader.ReadUInt16();
            product_version = exeReader.ReadInt32();
            product_build = exeReader.ReadInt32();
            build = (Products)runtime_version;


            Print();
            Logger.Log("Press any key to continue",true,ConsoleColor.Magenta);
            //Console.ReadKey();

            gameChunks = new ChunkList();
            gameChunks.Read(exeReader);

        }
        public void Print()
        {
            Logger.Log("GameData Info:", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Version: {runtime_version}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Subversion: { runtime_subversion}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Version: { product_version}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Build: {product_build}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    {(isUnicode ? "Unicode" : "NonUnicode")} Game", true, ConsoleColor.DarkGreen);

        }


    }
}
