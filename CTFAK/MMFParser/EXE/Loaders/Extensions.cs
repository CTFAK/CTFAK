﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class Extensions:ChunkLoader
    {
        internal ushort PreloadExtensions;
        public List<Extension> Items;

        public Extensions(ByteReader reader) : base(reader)
        {
        }


        public override void Read()
        {
            if (Settings.GameType == GameType.OnePointFive)
            {
                // File.WriteAllBytes($"{Settings.DumpPath}\\test.bin",Reader.ReadBytes());
                // return;
            }
            var count = Reader.ReadUInt16();
            PreloadExtensions = Reader.ReadUInt16();
            Items = new List<Extension>();
            for (int i = 0; i < count; i++)
            {
                var ext = new Extension(Reader);
                ext.Read();
                Items.Add(ext);
            }
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) Items.Count);
            Writer.WriteInt16((short) PreloadExtensions);
            foreach (Extension item in Items)
            {
                
                item.Write(Writer);
            }
           
        }


        public override string[] GetReadableData()
        {
            return new[]
            {
                $"Count: {Items.Count}"
            };
        }
    }
    public class Extension:ChunkLoader
    {
        public short Handle;
        public int MagicNumber;
        public int VersionLs;
        public int VersionMs;
        public string Name;
        public string Ext;
        public string SubType;

        public Extension(ByteReader reader) : base(reader)
        {
        }

 

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt16();
            if (size < 0) size = (short) (size * -1);
            Handle = Reader.ReadInt16();
            MagicNumber = Reader.ReadInt32();
            VersionLs = Reader.ReadInt32();
            VersionMs = Reader.ReadInt32();
            string[] arr;
            arr = Reader.ReadUniversal().Split('.');
            Name = arr[0];
            Logger.Log("Found Extension: " + Name + " with id " + Handle);
            Ext = arr[1];
            SubType = Reader.ReadUniversal();
            Reader.Seek(currentPosition + size);

            var newString = string.Empty;
            newString += $"MagicNumber={MagicNumber}\n";
            newString += $"VersionLs={VersionLs}\n";
            newString += $"VersionMs={VersionMs}\n";
            newString += $"SubType={SubType}\n";
            File.WriteAllText($"{Settings.DumpPath}\\ext", newString);
        }

        public override void Write(ByteWriter Writer)
        {
            var newWriter = new ByteWriter(new MemoryStream());
            newWriter.WriteInt16(Handle);
            newWriter.WriteInt32(MagicNumber);
            newWriter.WriteInt32(VersionLs);
            newWriter.WriteInt32(VersionMs);
            newWriter.WriteUnicode(string.Join(".",new string[]{Name,Ext}));
            newWriter.WriteUnicode(SubType);
            Writer.WriteInt16((short) (newWriter.Size()+2));
            Writer.WriteWriter(newWriter);
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}