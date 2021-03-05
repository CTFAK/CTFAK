using System;
using System.Reflection;
using CTFAK.MMFParser.EXE.Loaders.Events.Parameters;
using CTFAK.MMFParser.EXE.Loaders.Objects;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Viewer
{
    public static class Preprocessor
    {
        public static string ProcessCondition(Condition condition)
        {
            var typeName = ((Constants.ObjectType) (condition.ObjectType)).ToString();
            var metName = Names.ConditionNames[condition.ObjectType][condition.Num];
            var method = typeof(CounterProcessor).GetMethod(metName);
            
            var value = method?.Invoke(null, new[] {condition}) as string;
            return value ?? $"{(method==null ? ($"method is null: {condition.ObjectType}-{condition.Num}") : ("value is null"))}";

        }
    }

    public static class CounterProcessor
    {
        public static string CompareCounter(Condition condition)
        {
            //var frameItems = Program.CleanData.Frameitems.ItemDict;
            if (condition.Items[0].Loader is ExpressionParameter exrp)
            {
                return $"Counter(FromID({condition.ObjectInfo}))->Value {exrp.GetOperator()} {exrp.BuildExpression()}";
            }

            return "error";
        }
    }


}