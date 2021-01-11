using System;
using System.Collections.Generic;
using System.IO;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class AppMenu : ChunkLoader
    {
        public List<AppMenuItem> Items = new List<AppMenuItem>();
        public List<byte> AccelShift;
        public List<short> AccelKey;
        public List<short> AccelId;

        public AppMenu(ByteReader reader) : base(reader)
        {
        }

        public AppMenu(Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            List<string> list = new List<string>();
            foreach (AppMenuItem item in Items)
            {
                list.Add(item.Name);
            }

            return list.ToArray();
        }

        public override void Read()
        {
            long currentPosition = Reader.Tell();
            uint headerSize = Reader.ReadUInt32();
            int menuOffset = Reader.ReadInt32();
            int menuSize = Reader.ReadInt32();
            if (menuSize == 0) return;
            int accelOffset = Reader.ReadInt32();
            int accelSize = Reader.ReadInt32();
            Reader.Seek(currentPosition + menuOffset);
            Reader.Skip(4);

            Load(Reader);

            Reader.Seek(currentPosition + accelOffset);
            AccelShift = new List<byte>();
            AccelKey = new List<short>();
            AccelId = new List<short>();
            for (int i = 0; i < accelSize / 8; i++)
            {
                AccelShift.Add(Reader.ReadByte());
                Reader.Skip(1);
                AccelKey.Add(Reader.ReadInt16());
                AccelId.Add(Reader.ReadInt16());
                Reader.Skip(2);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt32(20);
            writer.WriteInt32(20);
            //writer.WriteInt32(0);

            ByteWriter menuDataWriter = new ByteWriter(new MemoryStream());

            foreach (AppMenuItem item in Items)
            {
                item.Write(menuDataWriter);
            }

            
            writer.WriteUInt32((uint) menuDataWriter.BaseStream.Position+4);
            //

            writer.WriteUInt32((uint) (24 + menuDataWriter.BaseStream.Position));
            writer.WriteInt32(AccelKey.Count * 8);
            writer.WriteInt32(0);
            writer.WriteWriter(menuDataWriter);
            
            for (Int32 i = 0; i < AccelKey.Count; i++)
            {
                writer.WriteInt8(AccelShift[i]);
                writer.WriteInt8(0);
                writer.WriteInt16(AccelKey[i]);
                writer.WriteInt16(AccelId[i]);
                writer.WriteInt16(0);
            }
            
        }

        public void Load(ByteReader reader)
        {
            while (true)
            {
                AppMenuItem newItem = new AppMenuItem(reader);
                newItem.Read();
                Items.Add(newItem);

                // if (newItem.Name.Contains("About")) break;
                if (ByteFlag.GetFlag((uint) newItem.Flags, 4))
                {
                    Load(reader);
                }

                if (ByteFlag.GetFlag((uint) newItem.Flags, 7))
                {
                    break;
                }
            }
        }
    }

    public class AppMenuItem : ChunkLoader
    {
        public string Name = "";
        public Int16 Flags = 0;
        public Int16 Id = 0;
        public string Mnemonic = null;

        public AppMenuItem(ByteReader reader) : base(reader)
        {
        }

        public AppMenuItem(Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Flags = Reader.ReadInt16();
            if (!ByteFlag.GetFlag((uint) Flags, 4))
            {
                Id = Reader.ReadInt16();
            }

            Name = Reader.ReadWideString();

            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == '&')
                {
                    Mnemonic = Name[i + 1].ToString();
                    Name = Name.Replace("&", "");
                    break;
                }
            }


        }

        public void Load()
        {
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt16(Flags);
            if (!ByteFlag.GetFlag((uint) Flags, 4))
            {
                writer.WriteInt16(Id);
            }

            String MName = Name;
            if (Mnemonic != null)
            {
                MName = MName.ReplaceFirst(Mnemonic, "&" + Mnemonic);
            }

            writer.WriteUnicode(MName, true);
        }
    }
}