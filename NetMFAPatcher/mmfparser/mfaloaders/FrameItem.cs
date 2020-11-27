
using mmfparser;
using NetMFAPatcher.mmfparser.mfaloaders.mfachunks;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class FrameItem : DataLoader
    {
        public int objectType;
        public int handle;
        public string name;
        public bool transparent;
        public int inkEffect;
        public int inkEffectParameter;
        public int antiAliasing;
        public int flags;
        public int iconType;

        public override void Print()
        {
            Console.WriteLine($"Name: {name}");
        }

        public override void Read()
        {
            objectType = reader.ReadInt32();
            handle = reader.ReadInt32();
            name = Helper.AutoReadUnicode(reader);
            var transparent1 = reader.ReadInt32();
            
            inkEffect = reader.ReadInt32();
            inkEffectParameter = reader.ReadInt32();
            antiAliasing = reader.ReadInt32();

            flags = reader.ReadInt32();
            iconType = reader.ReadInt32();
            if(iconType==1)
            {
                var iconHandle = reader.ReadInt32();
            }
            else
            {
                throw new NotImplementedException("invalid icon");
            }
            var chunks = new mmfparser.mfaloaders.ChunkList(reader);
            chunks.Read();
            if(objectType>=32)//extension base
            {
                //swallow some cum

            }
            else
            {
              
                var loader = new Active(reader);
                loader.Read();


            }
            Print();
            


        }
        public FrameItem(ByteIO reader):base(reader)
        { }
    }
}
