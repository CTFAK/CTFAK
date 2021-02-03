using System;

namespace CTFAK.MMFParser.Attributes
{
    public class CustomVisualNameAttribute:Attribute
    {
        private string _customName;
        private bool _isProp;

        public CustomVisualNameAttribute(string customName,bool isProp)
        {
            _customName = customName;
            _isProp = isProp;
        }

        public string CustomName => _customName;

        public bool IsProp => _isProp;
    }
}