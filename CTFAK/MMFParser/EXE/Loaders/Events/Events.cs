using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CTFAK.Utils;
using static CTFAK.Settings;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders.Events
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
        public Dictionary<int,Quailifer> QualifiersList = new Dictionary<int,Quailifer>();
        public List<int> NumberOfConditions = new List<int>();
        public List<EventGroup> Items = new List<EventGroup>();


     

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }


        public override string[] GetReadableData() => new[] {$"Event Count: {Items.Count}" };
        

        public override void Read()
        {
            // if (Settings.GameType == GameType.OnePointFive) return;
            while (true)
            {
                var identifier = Reader.ReadAscii(4);
                if (identifier == Header)
                {
                    MaxObjects = Reader.ReadInt16();
                    MaxObjectInfo = Reader.ReadInt16();
                    NumberOfPlayers = Reader.ReadInt16();
                    for (int i = 0; i < 17; i++)
                    {
                        NumberOfConditions.Add(Reader.ReadInt16());
                    }

                    var qualifierCount = Reader.ReadInt16(); //should be 0, so i dont care
                    for (int i = 0; i < qualifierCount; i++)
                    {
                        var newQualifier = new Quailifer(Reader);
                        newQualifier.Read();
                        if(!QualifiersList.ContainsKey(newQualifier.ObjectInfo))QualifiersList.Add(newQualifier.ObjectInfo,newQualifier);
                    }
                }
                else if (identifier == EventCount)
                {
                    var size = Reader.ReadInt32();
                }
                else if (identifier == EventgroupData)
                {
                    var size = Reader.ReadInt32();
                    var endPosition = Reader.Tell() + size;
                    while (true)
                    {
                        var eg = new EventGroup(Reader);
                        eg.Read();
                        Items.Add(eg);
                        
                        if (Reader.Tell() >= endPosition) break;
                    }
                    
                }
                else if (identifier == End) break;
            }
        }

        public Events(ByteReader reader) : base(reader)
        {
        }
    }

    public class Quailifer : DataLoader
    {
        public int ObjectInfo;
        public int Type;
        public int Qualifier;
        List<int> _objects = new List<int>();

        public Quailifer(Chunk chunk) : base(chunk)
        {
        }

        public Quailifer(ByteReader reader) : base(reader)
        {
        }

       

        public override void Read()
        {
            ObjectInfo = Reader.ReadUInt16();
            Type = Reader.ReadInt16();
            Qualifier = ObjectInfo & 0b11111111111;
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt16((ushort) ObjectInfo);
            Writer.WriteInt16((short) Type);
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }
    }


    public class EventGroup : ChunkLoader
    {
        public ushort Flags;
        public int IsRestricted;
        public int RestrictCpt;
        public int Identifier;
        public int Undo;
        public List<Condition> Conditions = new List<Condition>();
        public List<Action> Actions = new List<Action>();
        public int Size;
        public byte NumberOfConditions;
        public byte NumberOfActions;
        public bool isMFA=false;



        public EventGroup(ByteReader reader) : base(reader)
        {
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            Size = Reader.ReadInt16()*-1;
            NumberOfConditions = Reader.ReadByte();
            NumberOfActions = Reader.ReadByte();
            Flags = Reader.ReadUInt16();
            if (Old)
            {
                IsRestricted = Reader.ReadInt16(); //For MFA
                RestrictCpt = Reader.ReadInt16();
                Identifier = Reader.ReadInt16();
                Undo = Reader.ReadInt16();
            }
            else
            {
                if (Settings.Build >= 284&&(Settings.GameType == GameType.Normal))
                {
                    if(Settings.DoMFA)
                    {
                        IsRestricted = Reader.ReadInt16(); //For MFA
                        RestrictCpt = Reader.ReadInt16();
                        Identifier = Reader.ReadInt16();
                        Undo = Reader.ReadInt16();
                    }
                    else
                    {
                        var nop = Reader.ReadInt16();
                        IsRestricted = Reader.ReadInt32();
                        RestrictCpt = Reader.ReadInt32();
                    }
                }
                else
                {
                    IsRestricted = Reader.ReadInt16();
                    RestrictCpt = Reader.ReadInt16();
                    Identifier = Reader.ReadInt16();
                    Undo = Reader.ReadInt16();
                }
            }
            
            // Logger.Log($"Cond: {NumberOfConditions},Act: {NumberOfActions}");
            for (int i = 0; i < NumberOfConditions; i++)
            {
                var item = new Condition(Reader);
                item.Read();
                Conditions.Add(item);
            }

            for (int i = 0; i < NumberOfActions; i++)
            {
                var item = new Action(Reader);
                item.Read();
                Actions.Add(item);
            }
            Reader.Seek(currentPosition + Size);
            // Logger.Log($"COND:{NumberOfConditions}, ACT: {NumberOfActions}");

        }

        public override void Write(ByteWriter Writer)
        {
            ByteWriter newWriter = new ByteWriter(new MemoryStream());
            newWriter.WriteInt8((byte) Conditions.Count);
            newWriter.WriteInt8((byte) Actions.Count);
            newWriter.WriteUInt16(Flags);
            if (Settings.Build >= 284)
            {
                if(Settings.DoMFA)//For MFA
                {
                    newWriter.WriteInt16((short) IsRestricted); 
                    newWriter.WriteInt16((short) RestrictCpt);
                    newWriter.WriteInt16((short) Identifier);
                    newWriter.WriteInt16((short) Undo);
                }
                else
                {
                    newWriter.WriteInt16(0);
                    newWriter.WriteInt32(IsRestricted);
                    newWriter.WriteInt32(RestrictCpt);
                }
            }
            else
            {
                newWriter.WriteInt16((short) IsRestricted);
                newWriter.WriteInt16((short) RestrictCpt);
                newWriter.WriteInt16((short) Identifier);
                newWriter.WriteInt16((short) Undo);
            }
            
            foreach (Condition condition in Conditions)
            {
                var cond = condition;
                Fixer.FixConditions(ref cond);
                condition.Write(newWriter);
            }
           
            foreach (Action action in Actions)
            {
                var act = action;
                Fixer.FixActions(ref act);
                act.Write(newWriter);
            }
            Writer.WriteInt16((short) ((newWriter.Size()+2)*-1));
            
            Writer.WriteWriter(newWriter);

      
        }
    }

    public static class Fixer
    {
        public static void FixConditions(ref Condition cond)
        {
            var num = cond.Num; 
            //Alterable Values:
            if (num == -42) num = -27;
            //Global Values
            //if (num == -28||num == -29||num == -30||num == -31||num == -32||num == -33) num = -8;
            cond.Num = num;
        }
        public static void FixActions(ref Action act)
        {
            var num = act.Num;
            act.Num = num;
        }
        
        
    }
    
    
    
}