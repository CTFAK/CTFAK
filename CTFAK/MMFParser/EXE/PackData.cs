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
        private byte[] _header;
        public uint FormatVersion;

        public PackData()
        {

        }

        public void Write(ByteWriter Writer)
        {
            var newWriter = new ByteWriter(new MemoryStream());
            foreach (PackFile item in Items)
            {
                item.Write(newWriter);
            }
            Writer.WriteBytes(_header);
            Writer.WriteInt32(32);
            Writer.WriteInt32((int) (newWriter.Tell()+64));
            Writer.WriteInt32((int) FormatVersion);
            Writer.WriteInt32(0);
            Writer.WriteInt32(0);
            Writer.WriteInt32(Items.Count);
            Writer.WriteWriter(newWriter);
            
            
        }
        public void Read(ByteReader exeReader)
        {
            long start = exeReader.Tell();
            _header = exeReader.ReadBytes(8);

            // exeReader.Skip(8);
            uint headerSize = exeReader.ReadUInt32();
            Debug.Assert(headerSize==32);
            uint dataSize = exeReader.ReadUInt32();

            exeReader.Seek((int)(start + dataSize - 32));
            var uheader = exeReader.ReadAscii(4);
            Logger.Log("SUPERHEADER: "+uheader);
            if (uheader == "PAMU")
            {
                Settings.GameType = GameType.Normal;
                Settings.Unicode = true;
            }
            else if(uheader=="PAME")
            {
                Settings.GameType = GameType.MMFTwo;
                Settings.Unicode = false;
            }
            exeReader.Seek(start + 16);

            FormatVersion = exeReader.ReadUInt32();
            var check = exeReader.ReadInt32();
            Debug.Assert(check==0);
            check = exeReader.ReadInt32();
            Debug.Assert(check==0);

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
            
            var newHeader = exeReader.ReadFourCc();
            
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
            _bingo = exeReader.ReadInt32();
            Data = exeReader.ReadBytes(exeReader.ReadInt32());
            
            Dump();
        }

        public void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) PackFilename.Length);
            Writer.WriteUniversal(PackFilename);
            Writer.WriteInt32(_bingo);
            Writer.WriteInt32(Data.Length);
            Writer.WriteBytes(Data);
            
        }
        public void Dump(string path = "[DEFAULT-PATH]")
        {
            Logger.Log($"Dumping {PackFilename}", true, ConsoleColor.DarkBlue);
            if (path == "[DEFAULT-PATH]")
            {
                switch (Path.GetExtension(PackFilename))
                {
                    case ".exe": path = $"{Settings.EXEPath}\\{PackFilename}";
                        break;
                    case ".dll": path = $"{Settings.DLLPath}\\{PackFilename}";
                        break;
                    case ".mfx": path = $"{Settings.ExtensionPath}\\{PackFilename}";
                        break;
                    default: path = $"{Settings.DumpPath}\\PackData\\{PackFilename}";
                        break;
                }
            }
            File.WriteAllBytes(path, Data);
        }

    }
    
}
