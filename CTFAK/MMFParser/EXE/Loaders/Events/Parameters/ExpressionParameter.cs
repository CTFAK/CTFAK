using System.Collections.Generic;
using CTFAK.MMFParser.EXE.Loaders.Events.Expressions;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class ExpressionParameter:ParameterCommon
    {
        public List<Expression> Items;

        public ExpressionParameter(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            var comparsion = Reader.ReadInt16();
            Items = new List<Expression>();
            while (true)
            {
                var expression = new Expression(Reader);
                expression.Read();
                if (expression.ObjectType == 0) break;
                Items.Add(expression);
            }
            
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
        }

        public override string ToString()
        {
            return  $"{(Items.Count > 0 ? "=="+Items[0].ToString() : " ")}";;
        }
    }
}