using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CTFAK.MMFParser.EXE.Loaders.Events.Expressions;
using CTFAK.MMFParser.EXE.Loaders.Events.Viewer;
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
                //Console.WriteLine($"Found expression {expression.ObjectType}-{expression.Num}=={((ExpressionLoader)expression.Loader)?.Value}");
                if (expression.ObjectType == 0)
                {
                    break;
                }
                else
                {
                    Items.Add(expression);
                    //Console.WriteLine("Adding expression");
                }
                
                // if(expression.Num==23||expression.Num==24||expression.Num==50||expression.Num==16||expression.Num==19)Logger.Log("CUMSHOT "+expression.Num);

                }
            
        }

        public string BuildExpression()
        {
            var str = string.Empty;
            //if (Items == null) return str;
            foreach (Expression item in Items)
            {
                if (item.Loader == null)str+=Names.ExpressionNames[item.ObjectType][item.Num];
                else
                {
                    str += item.Loader.GetType().GetField("Value").GetValue(item.Loader);
                }
                

                

                //str += ; 


            }

            return str;
        }

        public string GetOperator()
        {
            switch (Comparsion)
            {
                case 0: return "==";
                case 1: return "!=";
                case 2: return "<=";
                case 3: return "<";
                case 4: return ">=";
                case 5: return ">";
                    default: return "err";
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
            Writer.WriteInt32(0);
            
        }

        public override string ToString()
        {
            return  $"{(Items.Count > 0 ? "=="+Items[0].ToString() : " ")}";;
        }
    }
}