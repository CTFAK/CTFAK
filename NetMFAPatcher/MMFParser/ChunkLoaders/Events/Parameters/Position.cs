using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Position : ParameterCommon
    {
        public int ObjectInfoParent;
        public int Flags;
        public int X;
        public int Y;
        public int Slope;
        public int Angle;
        public float Direction;
        public int TypeParent;
        public int ObjectInfoList;
        public int Layer;            

        public Position(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            ObjectInfoParent = Reader.ReadInt16();
            Flags = Reader.ReadUInt16();
            X = Reader.ReadInt16();
            Y = Reader.ReadInt16();
            Slope = Reader.ReadInt16();
            Angle = Reader.ReadInt16();
            Direction = Reader.ReadSingle();
            TypeParent = Reader.ReadInt16();
            ObjectInfoList = Reader.ReadInt16();
            Layer = Reader.ReadInt16();
            
           
        }
        public override string ToString()
        {
            return $"Object X:{X} Y:{Y} Angle:{Angle} Direction:{Direction} Parent:{ObjectInfoList}";
        }
    }
}
