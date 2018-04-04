using System;

namespace Flagrant
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FlagAttribute : Attribute
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Custom { get; set; }
        
        public FlagAttribute(string name)
        {
            Name = name;
        }
    }
}