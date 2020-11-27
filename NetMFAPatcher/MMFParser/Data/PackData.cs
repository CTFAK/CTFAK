using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.Data
{
    public class PackData
    {
        public PackFile[] items;
        public PackData()
        {

        }
        public void Read(ByteIO exeReader)
        {
            long start = exeReader.Tell();
            byte[] header = exeReader.ReadBytes(8);

            exeReader.Skip(8);
            uint header_size = exeReader.ReadUInt32();
            uint data_size = exeReader.ReadUInt32();

            exeReader.Seek((int)(start + data_size - 32));
            exeReader.Skip(4);
            exeReader.Seek(start + 16);

            uint format_version = exeReader.ReadUInt32();
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
            header = exeReader.ReadFourCC();

            exeReader.Seek(offset);
            for (int i = 0; i < count; i++) new PackFile().Read(exeReader);

            Logger.Log("\nPackdata Done\n", true, ConsoleColor.Blue);

        }
        
    }
    public class PackFile
    {
        string PackFilename = "ERROR";
        int bingo = 0;
        byte[] data;

        public void Read(ByteIO exeReader)
        {
            UInt16 len = exeReader.ReadUInt16();
            PackFilename = exeReader.ReadWideString(len);
            bingo = exeReader.ReadInt32();
            data = exeReader.ReadBytes(exeReader.ReadInt32());
            
            Dump();
        }
        public void Dump()
        {
            Logger.Log($"Dumping {PackFilename}", true, ConsoleColor.DarkBlue);
            string path = $"{Program.DumpPath}\\extensions\\" + PackFilename;
            File.WriteAllBytes(path, data);
        }

    }
    
}
