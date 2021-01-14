using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Shoot:ParameterCommon
    {
        public Position ShootPos;
        public ushort ObjectInstance;
        public ushort ObjectInfo;
        public short ShootSpeed;

        public Shoot(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            ShootPos = new Position(Reader);
            ShootPos.Read();
            ObjectInstance = Reader.ReadUInt16();
            ObjectInfo = Reader.ReadUInt16();
            Reader.Skip(4);
            ShootSpeed = Reader.ReadInt16();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            ShootPos.Write(Writer);
            Writer.WriteUInt16(ObjectInstance);
            Writer.WriteUInt16(ObjectInfo);
            Writer.Skip(4);
            Writer.WriteInt16(ShootSpeed);
        }

        public override string ToString()
        {
            return $"Shoot {ShootPos.X}x{ShootPos.Y}";
        }
    }
}