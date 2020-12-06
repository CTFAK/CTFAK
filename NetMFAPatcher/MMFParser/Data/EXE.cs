﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.Data
{
    public class Exe
    {
        public PackData PackData;
        public GameData GameData;
        public static Exe LatestInst;
        public void ParseExe(ByteIO exeReader)
        {
            Exe.LatestInst = this;
            Logger.Log($"Executable: {Settings.GameName}\n", true, ConsoleColor.DarkRed);

            string sig = exeReader.ReadAscii(2);
            Logger.Log("EXE Header: " + sig, true, ConsoleColor.Yellow);
            if (sig != "MZ")
            {
                Console.WriteLine("Invalid executable signature");
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
                var entry = exeReader.Tell();

                var sectionName = exeReader.ReadAscii();

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

            if (firstShort == 0x7777)
            {
                Logger.Log("Found PackData header!\nReading PackData header.", true, ConsoleColor.Blue);
                PackData = new PackData();
                PackData.Read(exeReader);
                GameData = new GameData();
                GameData.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Logger.Log("Failed to find PackData header!\n", true, ConsoleColor.Red);
            }
            
        }
    }
}
