using System;
using System.Data.OleDb;
using System.IO;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE
{
    public class Exe
    {
        public PackData PackData;
        public GameData GameData;
        public static Exe Instance;
        public void ParseExe(ByteReader exeReader)
        {
            
            Logger.Log($"Executable: {Settings.GameName}\n", true, ConsoleColor.DarkRed);

            string sig = exeReader.ReadAscii(2);
            Logger.Log("EXE Header: " + sig, true, ConsoleColor.Yellow);
            if (sig != "MZ")
            {
                Logger.Log("Invalid executable signature",true,ConsoleColor.Red);
            }

            exeReader.Seek(60, SeekOrigin.Begin);

            UInt16 hdrOffset = exeReader.ReadUInt16();

            exeReader.Seek(hdrOffset, SeekOrigin.Begin);
            string peHdr = exeReader.ReadAscii(2);
            Logger.Log("PE Header: " + peHdr, true, ConsoleColor.Yellow);
            exeReader.Skip(4);

            UInt16 numOfSections = exeReader.ReadUInt16();

            exeReader.Skip(16);
            int optionalHeader = 28 + 68;
            int dataDir = 16 * 8;
            exeReader.Skip(optionalHeader + dataDir);

            int possition = 0;
            for (int i = 0; i < numOfSections; i++)
            {
                long entry = exeReader.Tell();

                string sectionName = exeReader.ReadAscii();

                if (sectionName == ".extra")
                {
                    exeReader.Seek(entry + 20);
                    possition = (int)exeReader.ReadUInt32();
                    break;
                }

                if (i >= numOfSections - 1)
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
            UInt16 firstShort = exeReader.PeekUInt16();
            Logger.Log("First Short: " + firstShort.ToString("X2"), true, ConsoleColor.Yellow);
            if (firstShort == 0x7777) Settings.Old = false;
            else Settings.Old = true;
            if (!Settings.Old)
            {
                PackData = new PackData();
                Logger.Log("Found PackData header!\nReading PackData header.", true, ConsoleColor.Blue);
                PackData.Read(exeReader);
                GameData = new GameData();
                GameData.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                
                
            }
            else
            {
                var oldData = new ChunkList();
                oldData.Read(exeReader);
                GameData = new GameData();
                GameData.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Logger.Log("Failed to find PackData header!\n", true, ConsoleColor.Red);
            }
            
        }
    }
}
