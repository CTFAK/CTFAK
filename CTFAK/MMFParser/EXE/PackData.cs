using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE
{
    public class PackData
    {
        public List<PackFile> Items = new List<PackFile>();
        public PackData()
        {

        }
        public void Read(ByteReader exeReader)
        {
            long start = exeReader.Tell();
            byte[] header = exeReader.ReadBytes(8);

            // exeReader.Skip(8);
            uint headerSize = exeReader.ReadUInt32();
            Debug.Assert(headerSize==32);
            uint dataSize = exeReader.ReadUInt32();

            exeReader.Seek((int)(start + dataSize - 32));
            Logger.Log(exeReader.ReadAscii(4));
            exeReader.Seek(start + 16);

            uint formatVersion = exeReader.ReadUInt32();
            Debug.Assert(exeReader.ReadInt32()==0);
            Debug.Assert(exeReader.ReadInt32()==0);

            uint count = exeReader.ReadUInt32();

            Logger.Log($"Found {count} Pack Files", true, ConsoleColor.Blue);

            long offset = exeReader.Tell();
            for (int i = 0; i < count; i++)
            {
                if (!exeReader.Check(2)) break;
                UInt16 value = exeReader.ReadUInt16();
                if (!exeReader.Check(value)) break;
                exeReader.ReadBytes(value);
                exeReader.Skip(value);
                if (!exeReader.Check(value)) break;
            }
            
            header = exeReader.ReadFourCc();
            Logger.Log(header.GetHex(4));
            Logger.Log("PACK OFFSET: "+offset);
            exeReader.Seek(offset);
            for (int i = 0; i < count; i++)
            {
                var item = new PackFile();
                item.Read(exeReader);
                Items.Add(item);
                    
            }

            Logger.Log("\nPackdata Done\n", true, ConsoleColor.Blue);

        }
        
    }
    public class PackFile
    {
        public string PackFilename = "ERROR";
        int _bingo = 0;
        public byte[] Data;

        public void Read(ByteReader exeReader)
        {
            UInt16 len = exeReader.ReadUInt16();
            PackFilename = exeReader.ReadUniversal(len);
            
            
            Logger.Log(PackFilename);
            _bingo = exeReader.ReadInt32();
            Data = exeReader.ReadBytes(exeReader.ReadInt32());
            
            Dump();
        }
        public void Dump(string path = "[DEFAULT-PATH]")
        {
            Logger.Log($"Dumping {PackFilename}", true, ConsoleColor.DarkBlue);
            var actualPath = path=="[DEFAULT-PATH]" ? ($"{Settings.ExtensionPath}\\{PackFilename}"):path;
            File.WriteAllBytes(actualPath, Data);
        }

    }
    
}
