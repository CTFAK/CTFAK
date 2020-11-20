using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders.mfachunks
{
    class ObjectLoader : DataLoader
    {
        public int objectFlags;
        public int newObjectFlags;
        public Color backgroundColor;
        List<short> qualifiers = new List<short>();
        public ValueList values;
        public ValueList strings;
        public Movements movements;
        public Behaviours behaviours;
        
        public override void Print()
        {
            Logger.Log("Object Loader: ");
            Logger.Log("    Values:");
            foreach (var item in values.items)
            {
                Logger.Log($"       Value {item.name} contains {item.value}");
            }
            Logger.Log("\n    Strings:");
            foreach (var item in strings.items)
            {
                Logger.Log($"       String {item.name} contains {item.value}");
            }
            Logger.Log("\n    Movements:");
            foreach (var item in movements.items)
            {
                Logger.Log($"       Movement {item.name}");
            }
            Logger.Log("\n");
        }

        public override void Read()
        {
            objectFlags = reader.ReadInt32();
            newObjectFlags = reader.ReadInt32();
            backgroundColor = reader.ReadColor();
            var end = reader.Tell() + 2 * 9;
            for (int i = 0; i < 9; i++)
            {
                var value = reader.ReadInt16();
                if(value==-1)
                {
                    break;
                }
                qualifiers.Add(value);
            }
            reader.Seek(end);

            values = new ValueList(reader);
            values.Read();
            strings = new ValueList(reader);
            strings.Read();
            movements = new Movements(reader);
            movements.Read();
            behaviours = new Behaviours(reader);
            behaviours.Read();
            Print();


            
        }
        public ObjectLoader(ByteIO reader) : base(reader) { }
    }
}
