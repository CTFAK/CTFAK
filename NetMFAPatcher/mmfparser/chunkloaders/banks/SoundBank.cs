using System;
using System.Collections.Generic;
using System.IO;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.Data.ChunkList;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Banks
{
    public class SoundBank : ChunkLoader
    {
        public int NumOfItems = 0;
        public int References = 0;
        public List<SoundItem> Items;
        public bool IsCompressed = true;

        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
            $"Number of sounds: {NumOfItems}"
            };
        }
        public void Read(bool dump)
        {
            var cache = Settings.DumpSounds;
            Settings.DumpSounds = dump;
            Read();
            Settings.DumpSounds = cache;

        }

        public override void Read()
        {
            //Implementing for standalone-only because of my lazyness
            
            Reader.Seek(0);//Reset the reader to avoid bugs when dumping more than once
            Items = new List<SoundItem>();
            NumOfItems = Reader.ReadInt32();
            Logger.Log("Found " + NumOfItems + " sounds");
            if (!Settings.DumpSounds) return;

            for (int i = 0; i < NumOfItems; i++)
            {
                if (MainForm.BreakSounds)
                {
                    MainForm.BreakSounds = false;
                    break;
                }

                var item = new SoundItem(Reader);
                item.IsCompressed = IsCompressed;
                item.Read();
                Helper.OnSoundSaved(i, NumOfItems);
                Items.Add(item);


            }
        }
        public void Write(ByteWriter writer)
        {
            writer.WriteInt32(NumOfItems);
            foreach (var item in Items)
            {
                item.Write(writer);
            }
        }

        public SoundBank(ByteReader reader) : base(reader)
        {
        }

        public SoundBank(Chunk chunk) : base(chunk)
        {
        }
    }

    public class SoundBase : ChunkLoader
    {
        public int Handle;
        public string Name = "ERROR";
        public byte[] Data;

        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
        }

        public SoundBase(ByteReader reader) : base(reader)
        {
        }

        public SoundBase(Chunk chunk) : base(chunk)
        {
        }
    }

    public class SoundItem : SoundBase
    {
        public bool Compressed;
        public int Checksum;
        public int References;
        public int Flags;
        public bool IsCompressed = true;

        
        public override void Read()
        {
            var start = Reader.Tell();
            
            Handle = (int) Reader.ReadUInt32();
            Checksum = Reader.ReadInt32();
            References = Reader.ReadInt32();
            var decompressedSize = Reader.ReadInt32();
            Flags = (int)Reader.ReadUInt32(); //flags
            var reserved = Reader.ReadInt32();
            var nameLenght = Reader.ReadInt32();
            ByteReader soundData;
            if (IsCompressed) //compressed
            {
                var size = Reader.ReadInt32();
                soundData = new ByteReader(Decompressor.decompress_block(Reader, size, decompressedSize));
            }
            else
            {
                soundData = new ByteReader(Reader.ReadBytes(decompressedSize));
            }
            if (IsCompressed)
            {
                Name = soundData.ReadWideString(nameLenght);
            }
            else
            {
                Name = soundData.ReadAscii(nameLenght);

            }


            this.Data = soundData.ReadBytes((int) soundData.Size());
            if (Settings.DumpSounds)
            {
                Name = Helper.CleanInput(Name);
                Console.WriteLine($"Dumping {Name}");
                File.WriteAllBytes($"{Settings.SoundPath}\\{Name}.wav", Data);
            }
            //Save($"{Settings.DumpPath}\\SoundBank\\{Name}.wav");
            
        }

        
        public void Write(ByteWriter writer)
        {
            writer.WriteUInt32((uint)Handle);
            writer.WriteInt32(Checksum);
            writer.WriteInt32(References);
            writer.WriteInt32(Data.Length+Name.Length+1);
            writer.WriteInt32(Flags);
            writer.WriteInt32(0);
            writer.WriteInt32(Name.Length+1);
            if (IsCompressed) writer.WriteUnicode(Name);
            else writer.WriteAscii(Name);
            writer.WriteBytes(Data);






        }

        public SoundItem(ByteReader reader) : base(reader)
        {
        }

        public SoundItem(Chunk chunk) : base(chunk)
        {
        }
    }
}