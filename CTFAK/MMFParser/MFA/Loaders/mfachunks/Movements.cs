using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class Movements : DataLoader
    {
        public List<Movement> Items = new List<Movement>();
        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt32((uint) Items.Count);
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
        public uint Identifier;
        public short Player;
        public short Type;
        public byte MovingAtStart=1;
        public int DirectionAtStart;
        public int DataSize;
        public byte[] extData=new byte[14];
        public MovementLoader Loader;

        public override void Write(ByteWriter Writer)
        {    
            Writer.AutoWriteUnicode(Name);
            Writer.AutoWriteUnicode(Extension);
            Writer.WriteUInt32(Identifier);
            var newWriter = new ByteWriter(new MemoryStream());

                newWriter.WriteInt16(Player);
                newWriter.WriteInt16(Type);
                newWriter.WriteInt8(MovingAtStart);
                newWriter.Skip(3);
                newWriter.WriteInt32(DirectionAtStart);
                
                // newWriter.WriteBytes(extData);
                
            
            Loader?.Write(newWriter);
            newWriter.Skip(12);
            newWriter.WriteInt16(0);
            Writer.WriteInt32((int) newWriter.Size());
            Writer.WriteWriter(newWriter);
            
            
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Reader.AutoReadUnicode();
            Extension = Reader.AutoReadUnicode();
            Identifier = Reader.ReadUInt32();
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
                extData = Reader.ReadBytes(DataSize-12);
                switch (Type)
                {
                    case 1:
                        Loader = new Mouse(new ByteReader(extData));
                        break;
                    case 5:
                        Loader = new MovementPath(new ByteReader(extData));
                        break;
                    case 4:
                        Loader = new Ball(new ByteReader(extData));
                        break;
                }

                Loader?.Read();
            }

        }
        public Movement(ByteReader reader) : base(reader) { }
    }
}
