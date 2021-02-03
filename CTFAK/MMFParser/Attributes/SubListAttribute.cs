using System;

namespace CTFAK.MMFParser.Attributes
{
    public class SubListAttribute:Attribute
    {
        private string sublist;

        public SubListAttribute(string sublist)
        {
            this.sublist = sublist;
        }

        public string FieldName
        {
            get => sublist;
        }

    }
}