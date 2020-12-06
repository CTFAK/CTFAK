using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.Data
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

            exeReader.Skip(8);
            uint headerSize = exeReader.ReadUInt32();
            uint dataSize = exeReader.ReadUInt32();

            exeReader.Seek((int)(start + dataSize - 32));
            exeReader.Skip(4);
            exeReader.Seek(start + 16);

            uint formatVersion = exeReader.ReadUInt32();
            exeReader.Skip(8);

            uint count = exeReader.ReadUInt32();

            Logger.Log($"Found {count} Pack Files:", true, ConsoleColor.Blue);

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
            exeReader.BaseStream.Position -= 5;//wtf lol
            header = exeReader.ReadFourCc();

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
            PackFilename = exeReader.ReadWideString(len);
            _bingo = exeReader.ReadInt32();
            Data = exeReader.ReadBytes(exeReader.ReadInt32());
            
            //Dump();
        }
        public void Dump(string path = "[DEFAULT-PATH]")
        {
            Logger.Log($"Dumping {PackFilename}", true, ConsoleColor.DarkBlue);
            var actualPath = path=="[DEFAULT-PATH]" ? ($"{Settings.ExtensionPath}\\{PackFilename}"):path;
            File.WriteAllBytes(actualPath, Data);
        }

    }
    
}
