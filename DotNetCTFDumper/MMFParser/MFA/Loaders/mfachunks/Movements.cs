using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders.mfachunks
{
    public class Movements : DataLoader
    {
        public List<Movement> Items = new List<Movement>();
        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Items.Count);
            foreach (Movement movement in Items)
            {
                movement.Write(Writer);
            }
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = Reader.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new Movement(Reader);
                item.Read();
                Items.Add(item);

            }


        }
        public Movements(ByteReader reader) : base(reader) { }
    }

    public class Movement : DataLoader
    {
        public string Name="ERROR";
        public string Extension;
        public int Identifier;
        public short Player;
        public short Type;
        public byte MovingAtStart=1;
        public int DirectionAtStart;
        public int DataSize;
        public byte[] extData;
        public override void Write(ByteWriter Writer)
        {    
            Writer.AutoWriteUnicode(Name);
            Writer.AutoWriteUnicode(Extension);
            Writer.WriteUInt32((uint) Identifier);
            Writer.WriteInt32(DataSize);
            var newWriter = new ByteWriter(new MemoryStream());
            if (Extension.Length==0)
            {
                
                newWriter.WriteInt16(Player);
                newWriter.WriteInt16(Type);
                newWriter.WriteInt8(MovingAtStart);
                newWriter.Skip(3);
                newWriter.WriteInt32(DirectionAtStart);
                // newWriter.WriteBytes(extData);
            }
            Writer.WriteWriter(newWriter);
            
            
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Helper.AutoReadUnicode(Reader);
            Extension = Helper.AutoReadUnicode(Reader);
            Identifier = (int) Reader.ReadUInt32();
            DataSize = (int) Reader.ReadUInt32();
            if(Extension.Length>0)
            {
                extData = Reader.ReadBytes(DataSize);


            }
            else
            {
                Player = Reader.ReadInt16();
                Type = Reader.ReadInt16();
                MovingAtStart = Reader.ReadByte();
                Reader.Skip(3);
                DirectionAtStart = Reader.ReadInt32();
                Console.WriteLine($"Player: {Player}, Type: {Type}, DirectionAtStart: {DirectionAtStart}, Identifier: {Identifier}");
                extData = Reader.ReadBytes(DataSize-12);
                //ONLY STATIC MOVEMENT IS SUPPORTED RN
                //TODO:Movement Types
                //implement types, but i am tired, fuck this shit
            }

        }
        public Movement(ByteReader reader) : base(reader) { }
    }
}
