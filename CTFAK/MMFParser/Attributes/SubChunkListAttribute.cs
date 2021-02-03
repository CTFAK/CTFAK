using System;
using System.Collections.Generic;
using CTFAK.MMFParser.EXE;

namespace CTFAK.MMFParser.Attributes
{
    
    public class SubChunkListAttribute:Attribute
    {
        private string sublist;

        public SubChunkListAttribute(string sublist)
        {
            this.sublist = sublist;
        }

        public string FieldName
        {
            get => sublist;
        }

        
    }
}