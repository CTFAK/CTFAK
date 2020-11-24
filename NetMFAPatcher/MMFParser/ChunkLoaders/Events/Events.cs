using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events
{
    public class Events : ChunkLoader
    {
        public readonly string Header = "ER>>";
        public readonly string EventCount = "ERes";
        public readonly string EventgroupData = "ERev";
        public readonly string End = "<<ER";

        public int MaxObjects;
        public int MaxObjectInfo;
        public int NumberOfPlayers;
        public List<Quailifer> QualifiersList = new List<Quailifer>();
        public Quailifer[] Quailifers;
        public List<int> NumberOfConditions = new List<int>();


        public Events(Chunk chunk) : base(chunk) { }
        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            while(true)
            {
                var identifier = reader.ReadAscii(4);
                if (identifier == Header)
                {
                    MaxObjects = reader.ReadInt16();
                    MaxObjectInfo = reader.ReadInt16();
                    NumberOfPlayers = reader.ReadInt16();
                    for (int i = 0; i < 17; i++)
                    {
                        NumberOfConditions.Add(reader.ReadInt16());
                    }
                    var QualifierCount = reader.ReadInt16();//should be 0, so i dont give a fuck
                    Quailifers = new Quailifer[QualifierCount + 1];
                    for (int i = 0; i < QualifierCount; i++)
                    {
                        var NewQualifier = new Quailifer(reader);
                        QualifiersList.Add(NewQualifier);//fucking python types
                        //THIS IS NOT DONE

                    }
                }
                else if(identifier==EventCount)
                {
                    var size = reader.ReadInt32();
                }
                else if(identifier==EventgroupData)
                {
                    var size = reader.ReadInt32();
                    var end_position = reader.Tell() + size;
                    while(true)
                    {
                        var eg = new EventGroup(reader);
                        eg.Read();
                    }
                }

            }
            
        }
    }

    public class Quailifer : ChunkLoader
    {
        public int objectInfo;
        public int type;
        public Quailifer qualifier;
        List<int> Objects = new List<int>();        

        public Quailifer(Chunk chunk) : base(chunk) { }
        public Quailifer(ByteIO reader) : base(reader) { }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            objectInfo = reader.ReadUInt16();
            type = reader.ReadInt16();
            qualifier = this;

        }
    }


    public class EventGroup : ChunkLoader
    {
        public int Flags;
        public int IsRestricted;
        public int restrictCPT;
        public int identifier;
        public int undo;
        public List<Condition> Conditions = new List<Condition>();
        public List<Action> Actions = new List<Action>();

        public EventGroup(Chunk chunk) : base(chunk) { }
        public EventGroup(ByteIO reader) : base(reader) { }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = reader.Tell();
            var size = reader.ReadInt16() * -1;
            var NumberOfConditions = reader.ReadByte();
            var NumberOfActions = reader.ReadByte();
            var flags = reader.ReadUInt16();
            var nop = reader.ReadInt16();
            IsRestricted = reader.ReadInt32();
            restrictCPT = reader.ReadInt32();
            for (int i = 0; i < NumberOfConditions; i++)
            {
                var item = new Condition(reader);
                item.Read();
                Conditions.Add(item);
            }
            for (int i = 0; i < NumberOfActions; i++)
            {
                var item = new Action(reader);
                item.Read();
                Actions.Add(item);
            }
            reader.Seek(currentPosition + size);
            if(Conditions[0].items[0].loader!=null)
            {
                Logger.Log(Conditions[0].items[0].loader.ToString());
                Console.ReadKey();

            }
            

        }
    }







}
