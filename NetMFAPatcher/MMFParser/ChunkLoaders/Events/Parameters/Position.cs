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
            ObjectInfoParent = reader.ReadInt16();
            Flags = reader.ReadUInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Slope = reader.ReadInt16();
            Angle = reader.ReadInt16();
            Direction = reader.ReadSingle();
            TypeParent = reader.ReadInt16();
            ObjectInfoList = reader.ReadInt16();
            Layer = reader.ReadInt16();
            
           
        }
        public override string ToString()
        {
            return $"Object X:{X} Y:{Y} Angle:{Angle} Direction:{Direction} Parent:{ObjectInfoList}";
        }
    }
}
