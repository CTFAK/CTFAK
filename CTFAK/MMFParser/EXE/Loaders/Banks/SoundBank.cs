using System;
using System.Collections.Generic;
using System.IO;
using CTFAK.GUI;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders.Banks
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
        public event MainForm.SaveHandler OnSoundSaved;
        public override void Read()
        {
            if (!Settings.DoMFA)Reader.Seek(0);//Reset the reader to avoid bugs when dumping more than once
            Items = new List<SoundItem>();
            // if (!Settings.DoMFA)return;
            NumOfItems = Reader.ReadInt32(); 
            Logger.Log("Found " + NumOfItems + " sounds",true,ConsoleColor.Green);
            //if (!Settings.DumpSounds) return;

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
                if(!IsCompressed)Logger.Log(item.Name);
                
                OnSoundSaved?.Invoke(i,(int) NumOfItems);
                Items.Add(item);


            }
            Logger.Log("Sounds Success",true,ConsoleColor.Green);
        }
        public void Write(ByteWriter writer)
        {
            writer.WriteInt32(Items.Count);
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
        public int Checksum;
        public int References;
        public int Flags;
        public bool IsCompressed = false;

        
        public override void Read()
        {
            base.Read();
            var start = Reader.Tell();
            
            Handle = (int) Reader.ReadUInt32();
            Checksum = Reader.ReadInt32();
            References = Reader.ReadInt32();
            var decompressedSize = Reader.ReadInt32();
            Flags = (int)Reader.ReadUInt32(); 
            var reserved = Reader.ReadInt32();
            var nameLenght = Reader.ReadInt32();
            ByteReader soundData;
            if (IsCompressed) 
            {
                var size = Reader.ReadInt32();
                soundData = new ByteReader(Decompressor.decompress_block(Reader, size, decompressedSize));
            }
            else
            {
                soundData = new ByteReader(Reader.ReadBytes(decompressedSize));
            }
            Name = soundData.ReadWideString(nameLenght);
            Logger.Log(Name);

            Data = soundData.ReadBytes((int) soundData.Size());
            if (Settings.DumpSounds)
            {
                Name = Helper.CleanInput(Name);
                File.WriteAllBytes($"{Settings.SoundPath}\\{Name}.wav", Data);
            }
            
            
        }

        
        public void Write(ByteWriter writer)
        {
            writer.WriteUInt32((uint)Handle);
            writer.WriteInt32(Checksum);
            writer.WriteInt32(References);
            writer.WriteInt32(Data.Length+(Name.Length*2));
            writer.WriteInt32(Flags);
            writer.WriteInt32(0);
            writer.WriteInt32(Name.Length);
            writer.WriteUnicode(Name);
            // writer.BaseStream.Position -= 4;
            
            
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