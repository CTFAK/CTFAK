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
            Comparsion = Reader.ReadInt16();
            Items = new List<Expression>();
            while (true)
            {
                var expression = new Expression(Reader);
                expression.Read();
                // Logger.Log($"Found expression {expression.ObjectType}-{expression.Num}=={((ExpressionLoader)expression.Loader)?.Value}");
                if (expression.ObjectType == 0&& expression.Num==0)
                {
                    break;
                }
                else Items.Add(expression);
                // if(expression.Num==23||expression.Num==24||expression.Num==50||expression.Num==16||expression.Num==19)Logger.Log("CUMSHOT "+expression.Num);

            }
            
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16(Comparsion);
            // Logger.Log("ExpressionCount: "+Items.Count);
            foreach (Expression item in Items)
            {
                // Logger.Log("Writing expression: "+item.Num);
                item.Write(Writer);
            }
            
        }

        public override string ToString()
        {
            return  $"{(Items.Count > 0 ? "=="+Items[0].ToString() : " ")}";;
        }
    }
}