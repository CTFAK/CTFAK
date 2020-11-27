using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.Data
{
    class EXE
    {
        public PackData pack_data;
        public GameData game_data;
        public static EXE LatestInst;
        public void ParseExe(ByteIO exeReader)
        {
            LatestInst = this;
            Logger.Log($"Executable: {Program.GameName}\n", true, ConsoleColor.DarkRed);

            string Header1 = exeReader.ReadAscii(2);
            Logger.Log("EXE Header: " + Header1, true, ConsoleColor.Yellow);
            if (Header1 != "MZ")
            {
                Console.WriteLine("Invalid executable signature");
                Console.ReadKey();
                Environment.Exit(0);
            }

            exeReader.Seek(60, SeekOrigin.Begin);

            UInt16 hdr_offset = exeReader.ReadUInt16();

            exeReader.Seek(hdr_offset, SeekOrigin.Begin);
            string peHdr = exeReader.ReadAscii(2);
            Logger.Log("PE Header: " + peHdr, true, ConsoleColor.Yellow);
            exeReader.Skip(4);

            UInt16 num_of_sections = exeReader.ReadUInt16();

            exeReader.Skip(16);
            int optional_header = 28 + 68;
            int data_dir = 16 * 8;
            exeReader.Skip(optional_header + data_dir);

            int possition = 0;
            for (int i = 0; i < num_of_sections; i++)
            {
                var entry = exeReader.Tell();

                var section_name = exeReader.ReadAscii();

                if (section_name == ".extra")
                {
                    exeReader.Seek(entry + 20);
                    possition = (int)exeReader.ReadUInt32();
                    break;
                }

                if (i >= num_of_sections - 1)
                {
                    exeReader.Seek(entry + 16);
                    uint size = exeReader.ReadUInt32();
                    uint address = exeReader.ReadUInt32();
                    possition = (int)(address + size);
                    break;
                }

                exeReader.Seek(entry + 40);
            }

            exeReader.Seek((int)possition);
            UInt16 first_short = exeReader.PeekUInt16();
            Logger.Log("First Short: " + first_short.ToString("X2"), true, ConsoleColor.Yellow);

            if (first_short == 0x7777)
            {
                Logger.Log("Found PackData header!\nReading PackData header.", true, ConsoleColor.Blue);
                pack_data = new PackData();
                pack_data.Read(exeReader);
                game_data = new GameData();
                game_data.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Logger.Log("Failed to find PackData header!\n", true, ConsoleColor.Red);
            }
            
        }
    }
}
