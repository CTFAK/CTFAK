using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CTFAK.MMFParser.EXE.Loaders.Events.Expressions;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class ExpressionParameter:ParameterCommon
    {
        public List<Expression> Items;
        public short Comparsion;

        public ExpressionParameter(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            Comparsion = Reader.ReadInt16();
            Items = new List<Expression>();
            while (true)
            {
                var expression = new Expression(Reader);
                expression.Read();
                Items.Add(expression);
                if (expression.ObjectType == 0) break;
                
            }
            
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            
            Writer.WriteInt16(Comparsion);
            foreach (Expression item in Items)
            {
                item.Write(Writer);
            }
            
        }

        public override string ToString()
        {
            return  $"{(Items.Count > 0 ? "=="+Items[0].ToString() : " ")}";;
        }
    }
}