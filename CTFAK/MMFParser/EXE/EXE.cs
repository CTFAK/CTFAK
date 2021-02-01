using System;
using System.IO;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE
{
    public class Exe
    {
        public static Exe Instance;
        public GameData GameData;
        public PackData PackData;

        public void ParseExe(ByteReader exeReader)
        {
            Logger.Log($"Executable: {Settings.GameName}\n", true, ConsoleColor.DarkRed);

            var sig = exeReader.ReadAscii(2);
            Logger.Log("EXE Header: " + sig, true, ConsoleColor.Yellow);
            if (sig != "MZ") Logger.Log("Invalid executable signature", true, ConsoleColor.Red);

            exeReader.Seek(60);

            var hdrOffset = exeReader.ReadUInt16();

            exeReader.Seek(hdrOffset);
            var peHdr = exeReader.ReadAscii(2);
            Logger.Log("PE Header: " + peHdr, true, ConsoleColor.Yellow);
            exeReader.Skip(4);

            var numOfSections = exeReader.ReadUInt16();

            exeReader.Skip(16);
            var optionalHeader = 28 + 68;
            var dataDir = 16 * 8;
            exeReader.Skip(optionalHeader + dataDir);

            var possition = 0;
            for (var i = 0; i < numOfSections; i++)
            {
                var entry = exeReader.Tell();

                var sectionName = exeReader.ReadAscii();

                if (sectionName == ".extra")
                {
                    exeReader.Seek(entry + 20);
                    possition = (int) exeReader.ReadUInt32(); //Pointer to raw data
                    break;
                }

                if (i >= numOfSections - 1)
                {
                    exeReader.Seek(entry + 16);
                    var size = exeReader.ReadUInt32();
                    var address = exeReader.ReadUInt32(); //Pointer to raw data
                    possition = (int) (address + size);
                    break;
                }

                exeReader.Seek(entry + 40);
            }

            exeReader.Seek(possition);
            
            var firstShort = exeReader.PeekUInt16();
            Logger.Log("First Short: " + firstShort.ToString("X2"), true, ConsoleColor.Yellow);
            if (firstShort == 0x7777) Settings.GameType = GameType.Normal;
            else if (firstShort == 0x222c) Settings.GameType = GameType.OnePointFive;
            else throw new InvalidDataException("Unknown data header: 0x"+firstShort.ToString("X4"));
            
            if (Settings.GameType == GameType.Normal)
            {
                PackData = new PackData();
                Logger.Log("Found PackData header!\nReading PackData header.", true, ConsoleColor.Blue);
                PackData.Read(exeReader);
                GameData = new GameData();
                Program.CleanData = GameData;
                GameData.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Logger.Log("Using old system");
                var oldData = new ChunkList();
                oldData.Read(exeReader);
                GameData = new GameData();
                Program.CleanData = GameData;
                GameData.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
        }
    }
}